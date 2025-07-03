using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Services;
using Parent_Child.Models;

[Route("api/family")]
[ApiController]
public class FamilyController : ControllerBase
{
    private readonly IFamilyService _service;

    public FamilyController(IFamilyService service)
    {
        _service = service;
    }

    [HttpGet("{parentId}/children")]
    public async Task<IActionResult> GetChildren(int parentId)
    {
        var children = await _service.GetChildrenAsync(parentId);
        return Ok(children);
    }


    [HttpPost("{parentId}/add-child")]
    public async Task<IActionResult> AddChild(int parentId, ChildRegistrationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var child = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = dto.Password,
            DateOfBirth = dto.DateOfBirth,
            Role = "Child",
            IsGoogleUser = dto.IsGoogleUser // ✅ use from DTO
        };


        try
        {
            var addedChild = await _service.AddChildAsync(parentId, child, dto.Relation);
            return Ok(addedChild);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetChildProfile(int childId)
    {
        var child = await _service.GetChildProfileAsync(childId);
        if (child == null) return NotFound("Child not found");
        return Ok(child);
    }


    [HttpPost("{parentId}/assign-child/{childId}")]
    public async Task<IActionResult> AssignChild(int parentId, int childId, [FromBody] AssignChildDto dto)
    {
        try
        {
            var result = await _service.AssignChildAsync(parentId, childId, dto.Relation);
            if (!result)
                return BadRequest("Assignment failed. Child may not exist or is already assigned.");

            return Ok($"Child (ID: {childId}) successfully assigned to Parent (ID: {parentId}) with relation '{dto.Relation}'.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


}
