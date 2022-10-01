using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }
        public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
trackChanges).SingleOrDefault();
        

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
        FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name);

        public IEnumerable<Employee> GetAllEmployees(bool trackChanges) => FindAll(trackChanges)
        .OrderBy(c => c.Name)
        .ToList();

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void CreateEmployee(Employee employee) => Create(employee);
    }
}
