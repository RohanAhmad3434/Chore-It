using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Services;
using Parent_Child.Models;
using Microsoft.AspNetCore.Authorization;

[Route("api/family")]
[Authorize(Policy = "ParentPolicy")]
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
        try
        {
            var children = await _service.GetChildrenAsync(parentId);
            return Ok(children);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{parentId}/add-child")]
    public async Task<IActionResult> AddChild(int parentId, [FromBody] ChildRegistrationDto dto)
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
            IsGoogleUser = dto.IsGoogleUser
        };

        try
        {
            var addedChild = await _service.AddChildAsync(parentId, child, dto.Relation);
            return Ok(addedChild);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }


    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetChildProfile(int childId)
    {
        try
        {
            var child = await _service.GetChildProfileAsync(childId);
            if (child == null)
                return NotFound(new { message = "Child not found." });

            return Ok(child);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("{parentId}/assign-child/{childId}")]
    public async Task<IActionResult> AssignChild(int parentId, int childId, [FromBody] AssignChildDto dto)
    {
        try
        {
            var result = await _service.AssignChildAsync(parentId, childId, dto.Relation);
            return Ok(new { message = $"Child (ID: {childId}) successfully assigned to Parent (ID: {parentId}) with relation '{dto.Relation}'." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

}
