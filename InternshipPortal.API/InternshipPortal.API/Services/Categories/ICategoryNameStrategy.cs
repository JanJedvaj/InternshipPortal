
namespace InternshipPortal.API.Services.Categories
{
    public interface ICategoryNameStrategy
    {
        void Validate(string name);
        string Normalize(string name);
    }
}
