using Basket.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IMediator mediator, ILogger<BasketController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private string GetCustomerId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
                                      ?? throw new UnauthorizedAccessException();

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBasket()
    {
        var query = new GetBasketQuery { CustomerId = GetCustomerId() };
        var result = await _mediator.Send(query);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddItemToBasketCommand command)
    {
        command.CustomerId = GetCustomerId();
        await _mediator.Send(command);
        return Ok(new { message = "Item added to basket" });
    }

    [HttpPut("items/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemQuantity(Guid productId, [FromBody] UpdateItemQuantityRequest request)
    {
        var command = new UpdateBasketItemQuantityCommand
        {
            CustomerId = GetCustomerId(),
            ProductId = productId,
            Quantity = request.Quantity
        };
        await _mediator.Send(command);
        return Ok(new { message = "Item quantity updated" });
    }

    [HttpDelete("items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        var command = new RemoveItemFromBasketCommand
        {
            CustomerId = GetCustomerId(),
            ProductId = productId
        };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearBasket()
    {
        var command = new ClearBasketCommand { CustomerId = GetCustomerId() };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("checkout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutBasketCommand command)
    {
        command.CustomerId = GetCustomerId();
        command.UserName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        var result = await _mediator.Send(command);
        return Ok(new { message = "Checkout successful", orderId = result });
    }
}

public record UpdateItemQuantityRequest(int Quantity);
