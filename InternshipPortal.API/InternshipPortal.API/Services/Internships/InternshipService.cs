using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Repositories.Internships;


namespace InternshipPortal.API.Services.Internships
{
    public class InternshipService : IInternshipService
    {
        private readonly IInternshipRepository _repo;
        private readonly IInternshipFactory _factory;

        public InternshipService(
            IInternshipRepository repo,
            IInternshipFactory factory)
        {
            _repo = repo;
            _factory = factory;
        }

        public IEnumerable<Internship> GetAll()
        {
           
            return _repo.GetAll();
        }

        public Internship GetById(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("Id mora biti veći od nule.");
            }

            return _repo.GetById(id)
                ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public Internship Create(Internship internship)
        {
           
            var entity = _factory.CreateNew(internship);

            return _repo.Create(entity);
        }

        public Internship Update(int id, Internship internship)
        {
            if (id <= 0)
            {
                throw new ValidationException("Id mora biti veći od nule.");
            }

           
            var existing = _repo.GetById(id)
                ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");

          
            var updated = _factory.ApplyUpdates(existing, internship);

          
            return _repo.Update(id, updated)
                ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
        }

        public void Delete(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("Id mora biti veći od nule.");
            }

            if (!_repo.Delete(id))
            {
                throw new NotFoundException($"Praksa s Id={id} nije pronađena.");
            }
        }
    }
}
