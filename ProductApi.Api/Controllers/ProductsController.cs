using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Enums;

namespace ProductApi.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
	private readonly IProductService _svc;
	private readonly IItemService _itemSvc;

	public ProductsController(IProductService svc, IItemService itemSvc)
	{
		_svc = svc;
		_itemSvc = itemSvc;
	}

	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<ProductDto>), 200)]
	public async Task<IActionResult> GetAll(CancellationToken ct)
		=> Ok(await _svc.GetAllAsync(ct));

	[HttpGet("{id:int}")]
	[ProducesResponseType(typeof(ProductDto), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetById(int id, CancellationToken ct)
		=> Ok(await _svc.GetByIdAsync(id, ct));

	[HttpPost]
	//[Authorize(Roles = nameof(UserRole.Admin))]
	[ProducesResponseType(typeof(ProductDto), 201)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
	{
		var result = await _svc.CreateAsync(dto, User.Identity!.Name ?? "system", ct);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id:int}")]
	//[Authorize(Roles = nameof(UserRole.Admin))]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
	{
		await _svc.UpdateAsync(id, dto, User.Identity!.Name ?? "system", ct);
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	//[Authorize(Roles = nameof(UserRole.Admin))]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Delete(int id, CancellationToken ct)
	{
		await _svc.DeleteAsync(id, ct);
		return NoContent();
	}

	[HttpPost("{id:int}/items")]
	//[Authorize(Roles = nameof(UserRole.Admin))]
	[ProducesResponseType(typeof(ItemDto), 201)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> AddItem(int id, [FromBody] AddItemDto body, CancellationToken ct)
	{
		var dto = body with { ProductId = id };

		var result = await _itemSvc.AddItemAsync(
			dto, User.Identity!.Name ?? "system", ct);

		return CreatedAtAction(nameof(GetItem),
			new { productId = id, itemId = result.Id }, result);
	}

	[ApiExplorerSettings(IgnoreApi = true)]
	[NonAction]
	public IActionResult GetItem(int productId, int itemId) => NoContent();
}
