using InternshipApi.Models;
using InternshipApi.Repositories;

namespace InternshipApi.Services
{
    public class InternshipService
    {
        private readonly IRepository<Internship> _repo;

        public InternshipService(IRepository<Internship> repo)
        {
            _repo = repo;
        }

        public IEnumerable<Internship> GetFeatured() =>
            _repo.GetAll().Where(x => x.IsFeatured);

        public IEnumerable<Internship> Search(string q) =>
            _repo.GetAll().Where(x =>
                x.Title.ToLower().Contains(q.ToLower()) ||
                x.ShortDescription.ToLower().Contains(q.ToLower()));
    }
}
