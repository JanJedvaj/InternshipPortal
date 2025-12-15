using InternshipPortal.API.Models;

namespace InternshipPortal.API.Services.Abstractions
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
