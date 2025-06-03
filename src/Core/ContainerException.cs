using System;

namespace Squadron;

/// <summary>
/// Represents errors that occur during container initialization
/// </summary>
public class ContainerException : Exception
{
    /// <inheritdoc cref="Exception"/>
    public ContainerException(string message)
        : base(message)
    {
    }

    public ContainerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}