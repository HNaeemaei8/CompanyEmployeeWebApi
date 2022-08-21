using Entities;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
  public  interface IEmployeeRepository
    {
       Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackchanges);

       Task<Employee> GetEmployeeAsync(Guid companyId , Guid id, bool trackChanges);

        //Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId,
        // EmployeeParameters employeeParameters, bool trackChanges);
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
     EmployeeParameters employeeParameters, bool trackChanges);

        void CreateEmployeeForCompany(Guid companyId, Employee employee);

        void DeleteEmployee(Employee employee );
    }
}
