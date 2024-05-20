using Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryApp.Api.Adapters.Http;

using Commands = Core.Application.Commands;
using Queries = Core.Application.Queries;

public class DeliveryController : DefaultApiController
{
    private readonly IMediator _mediator;

    public DeliveryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task<IActionResult> CreateCourier()
    {
        return Ok();
    }

    public override async Task<IActionResult> CreateOrder()
    {
        return Ok();
    }

    public override async Task<IActionResult> GetActiveOrders()
    {
        var query = new Queries.Orders.GetActiveOrders.Query();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    public override async Task<IActionResult> GetAllCouriers()
    {
        var query = new Queries.Couriers.GetAllCouriers.Query();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    public override async Task<IActionResult> StartWork(Guid courierId)
    {
        var command = new Commands.Couriers.GetOnTheLine.Command { CourierId = courierId };
        var result = await _mediator.Send(command);

        return result ? Ok() : BadRequest();
    }

    public override async Task<IActionResult> StopWork(Guid courierId)
    {
        var command = new Commands.Couriers.GetOffTheLine.Command { CourierId = courierId };
        var result = await _mediator.Send(command);

        return result ? Ok() : BadRequest();
    }
}