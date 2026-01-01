using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Default implementation of the Internship factory.
    /// Central place for creation + validation rules for Internship.
    /// </summary>
    public class DefaultInternshipFactory : IInternshipFactory
    {
        public Internship CreateNew(Internship internship)
        {
            if (internship == null)
            {
                throw new ValidationException("Tijelo zahtjeva je prazno.");
            }

            Validate(internship);

            // Kreiramo potpuno novu entitet instancu – ne koristimo Id iz requesta
            var entity = new Internship
            {
                // Id = 0  -> EF će ga postaviti
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

            // Validiramo nove vrijednosti
            Validate(updates);

            // Id se ne mijenja – kontrolira ga baza
            existing.Title = updates.Title.Trim();
            existing.ShortDescription = updates.ShortDescription.Trim();
            existing.FullDescription = updates.FullDescription.Trim();
            existing.IsFeatured = updates.IsFeatured;
            existing.Remote = updates.Remote;
            existing.Location = (updates.Location ?? string.Empty).Trim();

            // Tipično PostedAt ostaje onaj originalni – oglas je objavljen tog dana.
            // Ako želiš dopustiti promjenu, ovdje možeš staviti existing.PostedAt = updates.PostedAt;
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

        /// <summary>
        /// Centralno mjesto za osnovnu business validaciju Internship entiteta.
        /// </summary>
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
