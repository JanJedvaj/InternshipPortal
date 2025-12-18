using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Repositories.Companies
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAll();
        Company? GetById(int id);
        Company Create(Company entity);
        Company? Update(int id, Company entity);
        bool Delete(int id);
    }
}
