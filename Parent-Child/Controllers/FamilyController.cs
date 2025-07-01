using Microsoft.AspNetCore.Mvc;
using Parent_Child.DTOs;
using Parent_Child.Services;
using Parent_Child.Models;

[Route("api/family")]
[ApiController]
public class FamilyController : ControllerBase
{
    private readonly IFamilyService _service; // ✅ use the interface

    public FamilyController(IFamilyService service) // ✅ inject the interface
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
        var child = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = dto.Password,
            DateOfBirth = dto.DateOfBirth,
            Relation = dto.Relation
        };

        var addedChild = await _service.AddChildAsync(parentId, child);
        return Ok(addedChild);
    }

    //// Optional: Implement GetChildProfileAsync in IFamilyService and FamilyService if needed
    [HttpGet("child/{childId}")]
    public async Task<IActionResult> GetChildProfile(int childId)
    {
        var child = await _service.GetChildProfileAsync(childId);
        if (child == null) return NotFound("Child not found");
        return Ok(child);
    }
}
