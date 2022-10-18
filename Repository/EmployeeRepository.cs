using Contracts;
using Entities.Models;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.RequestFeatures;
using System.ComponentModel.Design;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) => 
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();


        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
    EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId) &&
           (e.Age
            >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge),
           trackChanges)
            .OrderBy(e => e.Name)
            .ToListAsync();

            return PagedList<Employee>
            .ToPagedList(employees, employeeParameters.PageNumber,
            employeeParameters.PageSize);
        }

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
        FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name);

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }


    }
}
