using Contracts.IServices;
using Entities.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
   public class RepositoryManager:IRepositoryManager
    {
        private CompanyEmployeeDbContext  _dbContext;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(CompanyEmployeeDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_dbContext);
                return _companyRepository;
            }
        }
            

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new
                        EmployeeRepository(_dbContext);
                return _employeeRepository;
            }
        }

        public async Task  SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
