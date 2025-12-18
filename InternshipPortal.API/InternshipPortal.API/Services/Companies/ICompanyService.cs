using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Companies
{
    public interface ICompanyService
    {
        IEnumerable<Company> GetAll();
        Company GetById(int id);
        Company Create(Company company);
        Company Update(int id, Company company);
        void Delete(int id);
    }
}
