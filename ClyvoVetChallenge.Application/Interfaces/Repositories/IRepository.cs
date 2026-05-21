using ClyvoVetChallenge.Domain.Commons;

namespace ClyvoVetChallenge.Application.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IReadOnlyCollection<T> GetAll();
    T? GetById(Guid id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}