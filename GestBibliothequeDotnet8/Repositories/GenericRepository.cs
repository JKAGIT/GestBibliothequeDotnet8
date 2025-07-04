using GestBibliothequeDotnet8.Donnee;
using GestBibliothequeDotnet8.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime;

namespace GestBibliothequeDotnet8.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GestBibliothequeDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(GestBibliothequeDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
       public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);
            var keyValues = entry.Metadata.FindPrimaryKey().Properties
                .Select(p => entry.Property(p.Name).CurrentValue).ToArray();

            var tracked = _context.Set<T>().Local.FirstOrDefault(e =>
                _context.Entry(e).Metadata.FindPrimaryKey().Properties
                    .Select(p => _context.Entry(e).Property(p.Name).CurrentValue)
                    .SequenceEqual(keyValues));

            if (tracked != null && tracked != entity)
            {
                _context.Entry(tracked).State = EntityState.Detached;
            }

            _dbSet.Update(entity);

        }

                public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            else
                ValidationService.EnregistrementNonTrouve(entity, typeof(T).Name, id);
        }
    }
}

