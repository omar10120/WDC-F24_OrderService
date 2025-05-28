using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WDC_F24.Application.DTOs;
using WDC_F24.Application.DTOs.Requests.Orders;
using WDC_F24.Application.DTOs.Responses;
using WDC_F24.Application.Interfaces;
using WDC_F24.Domain.Entities;
using WDC_F24.UtilityServices;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrdersController(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
    {
        _orderService = orderService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        
        if (!ModelState.IsValid)
           return BadRequest(ModelState);
        try
        {
            var result = await _orderService.GetAllAsync();
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [Authorize]
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetCurrentUserOrders()
    {
        var userName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        var UserIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(UserIdentifier, out var userid))
            return Unauthorized("Invalid token");

        var result = await _orderService.GetUserOrdersAsync(userid);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {

            var result = await _orderService.GetByIdAsync(id);
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequestDto dto)
    {

        var userName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
        var UserIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(UserIdentifier) || !Guid.TryParse(UserIdentifier, out var userId))
            return Unauthorized("Invalid token or user ID");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _orderService.AddAsync(dto, userId);
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {

        try
        {
            
            var UserIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(UserIdentifier, out var userid))
                return Unauthorized("Invalid token");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _orderService.DeleteAsync(id , userid);
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {

            var result = await _orderService.UpdateStatusAsync(id, status);
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
