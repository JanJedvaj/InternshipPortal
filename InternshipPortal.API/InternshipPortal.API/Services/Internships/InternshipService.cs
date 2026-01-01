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
            // Bez dodatne logika , kasnije stavim filtraciju tipa samo aktivni i slicn.
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
            // Sav creation + validacija ide kroz Factory (Factory Method pattern)
            var entity = _factory.CreateNew(internship);

            return _repo.Create(entity);
        }

        public Internship Update(int id, Internship internship)
        {
            if (id <= 0)
            {
                throw new ValidationException("Id mora biti veći od nule.");
            }

            // Dohvaćamo postojeći entitet
            var existing = _repo.GetById(id)
                ?? throw new NotFoundException($"Praksa s Id={id} nije pronađena.");

            // Primjenjujemo ažuriranja kroz Factory (ApplyUpdates)
            var updated = _factory.ApplyUpdates(existing, internship);

            // Spremamo promjene – repo vraća finalni entitet
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
/// <summary>
/// 
//////////////STRARI InternshipService.cs 
///
/*
 private readonly IInternshipRepository _repo;

public InternshipService(IInternshipRepository repo)
{
    _repo = repo;
}
*/

/* //////STARI CREATE (logika je u InternshipServicu)
 public Internship Create(Internship internship)
{
    if (internship == null) throw new ValidationException("Tijelo zahtjeva je prazno.");
    if (string.IsNullOrWhiteSpace(internship.Title)) throw new ValidationException("Title je obavezan.");

    internship.PostedAt = internship.PostedAt == default ? DateTime.UtcNow : internship.PostedAt;

    return _repo.Create(internship);
}
+ Stari Update
/*

/////////XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX//////////////////////////////////
///
/// 
//////////////NOVI InternshipService.cs (sada service ovisi o factory-u (IInternshipFactory.cs a ne samo o repository-u)
/*
private readonly IInternshipRepository _repo;
private readonly IInternshipFactory _factory;

public InternshipService(
    IInternshipRepository repo,
    IInternshipFactory factory)
{
    _repo = repo;
    _factory = factory;
}
*/
/* NOVI CREATE (logika je u  i DefaultInternshipFactory.Validate i CreateNew.InternshipService (IInternshipFactory.cs)
 * 
 public Internship Create(Internship internship)
{
    var entity = _factory.CreateNew(internship);
    return _repo.Create(entity);
}
*/
////////////ISTO I ZA UPDATE -> Delete ostaje isti ! /////

/*
 * Key differences:

We first load existing entity from DB (existing).
We apply updates via the factory (so factory controls which fields can change and runs Validate).
Then we send that to the repository.

 Why this is now a Factory Method pattern?
Before: Service knew all details about how to construct and validate an Internship.

Now:

IInternshipFactory = abstraction of object creation.

DefaultInternshipFactory = concrete implementation.

InternshipService = client using the factory.

That is exactly what you want to show for “Creational pattern” ..
 
*/

/// </summary>