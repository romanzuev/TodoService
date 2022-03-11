namespace TodoApi.Errors
{
    internal class TodoNotFoundError
    {
        public TodoNotFoundError(long id)
        {
            NotFoundId = id;
        }
        
        public long NotFoundId { get; }
    }
}