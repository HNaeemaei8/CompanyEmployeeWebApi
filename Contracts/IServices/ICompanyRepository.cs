using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
  public  interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
       Task<Company> GetCompanyAsync(Guid companyId , bool trackchanges);
       void CreateCompany(Company company);
       Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackchanges);
        void DeleteCompany(Company company);
    }
}
