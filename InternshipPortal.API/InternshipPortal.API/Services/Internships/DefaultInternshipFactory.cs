using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;

namespace InternshipPortal.API.Services.Internships
{

    public class DefaultInternshipFactory : IInternshipFactory
    {
        public Internship CreateNew(Internship internship)
        {
            if (internship == null)
            {
                throw new ValidationException("Tijelo zahtjeva je prazno.");
            }

            Validate(internship);

            
            var entity = new Internship
            {
                // Id = 0  - EF Ga postavjla 
                Title = internship.Title.Trim(),
                ShortDescription = internship.ShortDescription.Trim(),
                FullDescription = internship.FullDescription.Trim(),
                IsFeatured = internship.IsFeatured,
                Remote = internship.Remote,
                Location = (internship.Location ?? string.Empty).Trim(),
                PostedAt = internship.PostedAt == default
                    ? DateTime.UtcNow
                    : internship.PostedAt,
                Deadline = internship.Deadline,
                CompanyId = internship.CompanyId,
                CategoryId = internship.CategoryId
            };

            return entity;
        }

        public Internship ApplyUpdates(Internship existing, Internship updates)
        {
            if (existing == null)
            {
                throw new ValidationException("Postojeća praksa ne smije biti null.");
            }

            if (updates == null)
            {
                throw new ValidationException("Tijelo zahtjeva je prazno.");
            }

           
            Validate(updates);

           
            existing.Title = updates.Title.Trim();
            existing.ShortDescription = updates.ShortDescription.Trim();
            existing.FullDescription = updates.FullDescription.Trim();
            existing.IsFeatured = updates.IsFeatured;
            existing.Remote = updates.Remote;
            existing.Location = (updates.Location ?? string.Empty).Trim();

            
            if (existing.PostedAt == default)
            {
                existing.PostedAt = updates.PostedAt == default
                    ? DateTime.UtcNow
                    : updates.PostedAt;
            }

            existing.Deadline = updates.Deadline;
            existing.CompanyId = updates.CompanyId;
            existing.CategoryId = updates.CategoryId;

            return existing;
        }

    
        private static void Validate(Internship internship)
        {
            if (string.IsNullOrWhiteSpace(internship.Title))
            {
                throw new ValidationException("Naslov je obvezan.");
            }

            if (string.IsNullOrWhiteSpace(internship.ShortDescription))
            {
                throw new ValidationException("Kratki opis je obvezan.");
            }

            if (string.IsNullOrWhiteSpace(internship.FullDescription))
            {
                throw new ValidationException("Puni opis je obvezan.");
            }

            if (string.IsNullOrWhiteSpace(internship.Location))
            {
                throw new ValidationException("Lokacija je obvezna.");
            }

            if (internship.CompanyId <= 0)
            {
                throw new ValidationException("CompanyId mora biti veći od nule.");
            }

            if (internship.CategoryId <= 0)
            {
                throw new ValidationException("CategoryId mora biti veći od nule.");
            }

            if (internship.Deadline.HasValue &&
                internship.Deadline.Value.Date < DateTime.UtcNow.Date)
            {
                throw new ValidationException("Rok prijave ne može biti u prošlosti.");
            }
        }
    }
}
