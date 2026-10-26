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

    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
