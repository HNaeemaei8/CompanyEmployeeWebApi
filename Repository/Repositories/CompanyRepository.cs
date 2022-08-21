using Contracts.IServices;
using Entities;
using Entities.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
   public  class CompanyRepository:RepositoryBase<Company>,ICompanyRepository
    {
        protected CompanyEmployeeDbContext _context;
        public CompanyRepository(CompanyEmployeeDbContext context):base (context)
        {
            _context = context;
        }

        

        public  void CreateCompany(Company company)
        => Create(company);

        public void DeleteCompany(Company company)
        => Delete(company);

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        => await FindAll(trackChanges).
         OrderBy(c => c.Name).ToListAsync();

        public async Task< IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackchanges)
        => await FindByCaption(x => ids.Contains(x.Id), trackchanges).ToListAsync();

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackchanges)
        =>
          await  FindByCaption(c => c.Id.Equals(companyId), trackchanges).SingleOrDefaultAsync();

        
    }
}
