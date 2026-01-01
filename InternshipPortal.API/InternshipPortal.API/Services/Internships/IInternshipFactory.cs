using InternshipPortal.API.Data.EF;

namespace InternshipPortal.API.Services.Internships
{
    /// <summary>
    /// Factory responsible for creating and updating Internship entities.
    /// This is your Creational (Factory Method) pattern for the Oglasi module.
    /// </summary>
    public interface IInternshipFactory
    {
        /// <summary>
        /// Creates a new Internship entity from the incoming data.
        /// Applies defaults, normalization and validation.
        /// </summary>
        Internship CreateNew(Internship internship);

        /// <summary>
        /// Applies allowed updates from the DTO to an existing Internship entity.
        /// </summary>
        Internship ApplyUpdates(Internship existing, Internship updates);
    }
}
