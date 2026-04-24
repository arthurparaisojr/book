namespace Book.Application.Abstractions.Persistence;

public interface IBookDbConnectionFactory
{
    string ConnectionString { get; }
}
