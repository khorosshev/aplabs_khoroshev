using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid employeeId, bool trackChanges);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool trackChanges);
        void CreateEmployeeForCompany(Guid companyId, Employee employee);
        void DeleteEmployee(Employee employee);

    }

}
