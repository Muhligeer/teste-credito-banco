using Contracts.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomerRequest request)
    {
        var customerResponse = await _customerService.CreateCustomerAsync(request);
        return CreatedAtAction(nameof(Post), new { id = customerResponse.Id }, customerResponse);
    }
}
