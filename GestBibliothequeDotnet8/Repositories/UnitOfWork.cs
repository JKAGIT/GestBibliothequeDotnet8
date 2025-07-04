using GestBibliothequeDotnet8.Donnee;
using GestBibliothequeDotnet8.Models;
using Microsoft.EntityFrameworkCore;

namespace GestBibliothequeDotnet8.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GestBibliothequeDbContext _context;
        private IGenericRepository<Livres> _livres;
        private IGenericRepository<Categories> _categories;
        private IGenericRepository<Utilisateurs> _utilisateurs;
        private IGenericRepository<Usagers> _usagers;
        private IGenericRepository<Emprunts> _emprunts;
        private IGenericRepository<Retours> _retours;
        private IGenericRepository<Reservations> _reservations;

        public UnitOfWork(GestBibliothequeDbContext context)
        {
            _context = context;
            _livres = new GenericRepository<Livres>(_context);
            _categories  = new GenericRepository<Categories>(_context);
            _utilisateurs = new GenericRepository<Utilisateurs>(_context);
            _usagers = new GenericRepository<Usagers>(_context);
            _emprunts = new GenericRepository<Emprunts>(_context);
            _retours = new GenericRepository<Retours>(_context);
            _reservations = new GenericRepository<Reservations>(_context);
        }
        public IGenericRepository<Livres> Livres => _livres;
        public IGenericRepository<Categories> Categories => _categories;
        public IGenericRepository<Utilisateurs> Utilisateurs => _utilisateurs;
        public IGenericRepository<Usagers> Usagers =>_usagers;
        public IGenericRepository<Emprunts> Emprunts => _emprunts;
        public IGenericRepository<Retours> Retours =>_retours;
        public IGenericRepository<Reservations> Reservations =>_reservations;

        public async Task<int> CompleteAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(); 
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
