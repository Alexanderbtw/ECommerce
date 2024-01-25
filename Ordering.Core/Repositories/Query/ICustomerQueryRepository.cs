using Ordering.Core.Entities;
using Ordering.Core.Repositories.Query.Base;

namespace Ordering.Core.Repositories.Query
{
    // Interface for CustomerQueryRepository
    public interface ICustomerQueryRepository : IQueryRepository<Customer>
    {
        //Custom operation which is not generic
        Task<IReadOnlyList<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Int64 id);
        Task<Customer> GetCustomerByEmail(string email);
    }
}
