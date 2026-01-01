using InternshipPortal.API.Exceptions;

namespace InternshipPortal.API.Services.Categories
{
    public class DefaultCategoryNameStrategy : ICategoryNameStrategy
    {
        public void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Name je obavezan.");

            if (name.Trim().Length > 100)
                throw new ValidationException("Name ne smije biti duži od 100 znakova.");
        }

        public string Normalize(string name)
        {
            return name.Trim();
        }
    }
}
