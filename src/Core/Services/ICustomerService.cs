using Contracts.DTOs;

namespace Core.Services;

public interface ICustomerService
{
    Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request);
}
