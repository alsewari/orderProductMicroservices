using Microsoft.AspNetCore.Mvc;
using UserManagementService.Models;
using UserManagementService.Services;

namespace UserManagementService.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _service.GetByIdAsync(id);

        if (user == null)
            return NotFound("User not found.");

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        var allowedRoles = new[] { "Admin", "Seller", "Customer" };

        if (!allowedRoles.Contains(user.Role))
            return BadRequest("Role must be Admin, Seller, or Customer.");

        var result = await _service.CreateAsync(user);

        if (!result.Success)
            return BadRequest(result.Message);

        return Ok(new
        {
            message = result.Message,
            user = result.User
        });
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] string role)
    {
        var allowedRoles = new[] { "Admin", "Seller", "Customer" };

        if (!allowedRoles.Contains(role))
            return BadRequest("Role must be Admin, Seller, or Customer.");

        var updated = await _service.UpdateRoleAsync(id, role);

        if (!updated)
            return NotFound("User not found or role not changed.");

        return Ok(new { message = "User role updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound("User not found.");

        return Ok(new { message = "User deleted successfully." });
    }
}