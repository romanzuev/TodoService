namespace TodoApi.Errors
{
    internal class TodoAlreadyExists
    {
        public TodoAlreadyExists(long id)
        {
            AlreadyExistsId = id;
        }
        
        public long AlreadyExistsId { get; }
    }
}