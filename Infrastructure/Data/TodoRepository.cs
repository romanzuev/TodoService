using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Infrastructure.Data
{
    internal sealed class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context;
        }

        public IQueryable<TodoItem> GetAllTodos()
        {
            return _context.TodoItems.AsNoTracking();
        }

        public async Task<TodoItem?> Find(long id, CancellationToken ct)
        {
            return await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        }

        public async Task<TodoItem> Update(TodoItem item, CancellationToken ct)
        {
            _context.Update(item);
            await _context.SaveChangesAsync(ct);

            return item;
        }

        public async Task Create(TodoItem item, CancellationToken ct)
        {
            _context.Add(item);
            await _context.SaveChangesAsync(ct);
        }

        public async Task Delete(TodoItem item, CancellationToken ct)
        {
            _context.Remove(item);
            await _context.SaveChangesAsync(ct);
        }

        public bool Contains(long itemId)
        {
            return _context.TodoItems.Any(e => e.Id == itemId);
        }
    }
}