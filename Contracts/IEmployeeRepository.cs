using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);
        IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
        IEnumerable<Employee> GetAllEmployees(bool trackChanges);
    }

}
