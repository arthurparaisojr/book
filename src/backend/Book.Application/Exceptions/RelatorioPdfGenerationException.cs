namespace Book.Application.Exceptions;

public sealed class RelatorioPdfGenerationException : Exception
{
    public RelatorioPdfGenerationException(string message)
        : base(message)
    {
    }
}
