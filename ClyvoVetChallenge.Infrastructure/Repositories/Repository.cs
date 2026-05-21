using Microsoft.EntityFrameworkCore;
using ClyvoVetChallenge.Application.Interfaces.Repositories;
using ClyvoVetChallenge.Domain.Commons;
using ClyvoVetChallenge.Infrastructure.Persistence;

namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class Repository<T>(ClyvoVetContext context) : IRepository<T> where T : BaseEntity
{
    protected readonly ClyvoVetContext _context = context;
    protected DbSet<T> Set => _context.Set<T>();

    public IReadOnlyCollection<T> GetAll() => Set.ToList();
    
    public T? GetById(Guid id) => Set.Find(id);
    
    public void Add(T entity) => Set.Add(entity);
    
    public void Update(T entity) => Set.Update(entity);
    
    public void Delete(T entity) => Set.Remove(entity);
    
    public void SaveChanges() => _context.SaveChanges();
}