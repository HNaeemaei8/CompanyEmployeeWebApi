using Contracts.IServices;
using Entities;
using Entities.Context;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repository.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        protected CompanyEmployeeDbContext _context;
        public EmployeeRepository(CompanyEmployeeDbContext context) : base(context)
        {
            _context = context;
        }

        public void 
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            
            CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {

            Delete(employee);

        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        =>
           await FindByCaption(c => c.CompanyId.Equals(companyId) && c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackchanges)
        =>
          await FindByCaption(c => c.CompanyId.Equals(companyId), trackchanges).OrderBy(e => e.Name).ToListAsync();



        //    public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid
        //    companyId, EmployeeParameters employeeParameters, bool
        //    trackChanges) =>
        //    await FindByCaption(e => e.CompanyId.Equals(companyId),
        //   trackChanges)
        //  .OrderBy(e => e.Name)
        // .Skip((employeeParameters.PageNumber - 1) *
        // employeeParameters.PageSize)
        //.Take(employeeParameters.PageSize)
        //.ToListAsync();





        //public async Task<PagedList<Employee>> GetEmployeesAsync(Guid
        //companyId, EmployeeParameters employeeParameters, bool
        //trackChanges)
        //{
        //    var employees = await FindByCaption(e =>
        //   e.CompanyId.Equals(companyId)
        //    && (e.Age >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge)
        //   ,
        //    trackChanges)
        //    .OrderBy(e => e.Name)
        //    .ToListAsync();
        //    return PagedList<Employee>
        //    .ToPagedList(employees, employeeParameters.PageNumber,
        //    employeeParameters.PageSize);
        //}


        //public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        //{
        //    var employees = await FindByCaption(e =>
        //    e.CompanyId.Equals(companyId) && (e.Age >= employeeParameters.MinAge
        //    && e.Age <= employeeParameters.MaxAge),
        //     trackChanges)
        //     .OrderBy(e => e.Name)
        //     .ToListAsync();
        //    return PagedList<Employee>
        //    .ToPagedList(employees, employeeParameters.PageNumber,
        //    employeeParameters.PageSize);
        //}


        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid
companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCaption(e =>
            e.CompanyId.Equals(companyId),
             trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
             .Search(employeeParameters.SearchTeam)
             .OrderBy(e => e.Name)
             .Sort(employeeParameters.OrderBy)
             .ToListAsync();
            return PagedList<Employee>
            .ToPagedList(employees, employeeParameters.PageNumber,
            employeeParameters.PageSize);
        }



    }
}
