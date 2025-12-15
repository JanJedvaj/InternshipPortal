namespace InternshipPortal.API.Repositories.Abstractions
{
    public interface IWriteRepository<T>
    {
        T Create(T entity);
        T? Update(int id, T entity);
        bool Delete(int id);
    }
}
