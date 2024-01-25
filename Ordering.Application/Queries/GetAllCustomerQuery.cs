using MediatR;
using Ordering.Core.Entities;

namespace Ordering.Application.Queries
{
    // Customer query with List<Customer> response
    public record GetAllCustomerQuery : IRequest<List<Customer>>
    {

    }
}
