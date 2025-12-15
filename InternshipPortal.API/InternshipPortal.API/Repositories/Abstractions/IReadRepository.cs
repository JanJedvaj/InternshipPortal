namespace InternshipPortal.API.Repositories.Abstractions
{
    public interface IReadRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
    }
}
