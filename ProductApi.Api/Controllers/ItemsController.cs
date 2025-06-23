using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Enums;

namespace ProductApi.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products/{productId:int}/items")]
public class ItemsController : ControllerBase
{
	private readonly IItemService _svc;
	public ItemsController(IItemService svc) => _svc = svc;

	[HttpGet("{itemId:int}")]
	[ProducesResponseType(typeof(ItemDto), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetById(int productId, int itemId, CancellationToken ct)
	{
		return NotFound();
	}

	[HttpDelete("{itemId:int}")]
	//[Authorize(Roles = nameof(UserRole.Admin))]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Delete(int productId, int itemId, CancellationToken ct)
	{
		await _svc.RemoveItemAsync(productId, itemId, ct);
		return NoContent();
	}
}
