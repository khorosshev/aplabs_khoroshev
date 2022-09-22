using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repositoryContext;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;
        //private IDailyProgressRepository _dailyProgressRepository;
        //private IFoodRepository _foodRepository;
        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }
        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_repositoryContext);
                return _companyRepository;
            }
        }
        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new EmployeeRepository(_repositoryContext);
                return _employeeRepository;
            }
        }

        /*public IFoodRepository Food
        {
            get
            {
                if (_foodRepository == null)
                    _foodRepository = new FoodRepository(_repositoryContext);
                return _foodRepository;
            }
        }
        public IDailyProgressRepository DailyProgress
        {
            get
            {
                if (_dailyProgressRepository == null)
                    _dailyProgressRepository = new DailyProgressRepository(_repositoryContext);
                return _dailyProgressRepository;
            }
        }*/

        public void Save() => _repositoryContext.SaveChanges();
    }
}
