using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Services
{
    internal interface ITodoRepository
    {
        IQueryable<TodoItem> GetAllTodos();
        Task<TodoItem?> Find(long id, CancellationToken ct);
        Task<TodoItem> Update(TodoItem item, CancellationToken ct);
        Task Create(TodoItem item, CancellationToken ct);
        Task Delete(TodoItem item, CancellationToken ct);
        bool Contains(long itemId);
    }
}