using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Squadron;

/// <summary>
/// Extensions that makes it easier to await a message on an <see cref="ISubscriptionClient"/>
/// </summary>
public static class SubscriptionClientExtensions
{
    /// <summary>
    /// Await a message to be received as <see cref="Task{T}"/>
    /// </summary>
    /// <param name="client">client where the message is received</param>
    /// <param name="messageHandler">handler to deserialize to <typeparamref name="T"/></param>
    /// <typeparam name="T">Target type</typeparam>
    /// <remarks>Default timeout is 00:10:00.000</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> or <paramref name="messageHandler"/> is null</exception>
    public static Task<T> AwaitMessageAsync<T>(
        this ISubscriptionClient client,
        Func<Message, CancellationToken, Task<T>> messageHandler)
        => AwaitMessageAsync(client, messageHandler, TimeSpan.FromMinutes(10));

    /// <summary>
    /// Await a message to be received as <typeparamref name="T"/>
    /// </summary>
    /// <param name="client">client where the message is received</param>
    /// <param name="messageHandler">handler to deserialize to <typeparamref name="T"/></param>
    /// <typeparam name="T">Target type</typeparam>
    /// <remarks>Default timeout is 00:10:00.000</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> or <paramref name="messageHandler"/> is null</exception>
    public static Task<T> AwaitMessageAsync<T>(
        this ISubscriptionClient client,
        Func<Message, CancellationToken, T> messageHandler)
        => AwaitMessageAsync(client, messageHandler, TimeSpan.FromMinutes(10));

    /// <summary>
    /// Await a message to be received as <see cref="Task{T}"/>
    /// </summary>
    /// <param name="client">client where the message is received</param>
    /// <param name="messageHandler">handler to deserialize to <typeparamref name="T"/></param>
    /// <param name="cancelAfter">Timeout for the operation</param>
    /// <typeparam name="T">Target type</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> or <paramref name="messageHandler"/> is null</exception>
    public static Task<T> AwaitMessageAsync<T>(
        this ISubscriptionClient client,
        Func<Message, CancellationToken, T> messageHandler,
        TimeSpan cancelAfter)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (messageHandler == null) throw new ArgumentNullException(nameof(messageHandler));

        var completion = new TaskCompletionSource<T>();
        using var timeoutToken = new CancellationTokenSource(cancelAfter);
        timeoutToken.Token.Register(() => completion.SetCanceled(), useSynchronizationContext: false);

        client.RegisterMessageHandler((message, token) =>
        {
            try
            {
                T payload = messageHandler.Invoke(message, token);
                completion.SetResult(payload);
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
            return Task.CompletedTask;
        }, args =>
        {
            completion.SetException(args.Exception);
            return Task.CompletedTask;
        });

        return completion.Task;
    }

    /// <summary>
    /// Await a message to be received as <typeparamref name="T"/>
    /// </summary>
    /// <param name="client">client where the message is received</param>
    /// <param name="messageHandler">handler to deserialize to <typeparamref name="T"/></param>
    /// <param name="cancelAfter">Timeout for the operation</param>
    /// <typeparam name="T">Target type</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="client"/> or <paramref name="messageHandler"/> is null</exception>
    public static async Task<T> AwaitMessageAsync<T>(
        this ISubscriptionClient client,
        Func<Message, CancellationToken, Task<T>> messageHandler,
        TimeSpan cancelAfter)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        if (messageHandler == null) throw new ArgumentNullException(nameof(messageHandler));

        var completion = new TaskCompletionSource<T>();
        using var timeoutToken = new CancellationTokenSource(cancelAfter);
        timeoutToken.Token.Register(() => completion.SetCanceled(), useSynchronizationContext: false);

        client.RegisterMessageHandler(async (message, token) =>
        {
            try
            {
                T payload = await messageHandler.Invoke(message, token);
                completion.SetResult(payload);
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        }, args =>
        {
            completion.SetException(args.Exception);
            return Task.CompletedTask;
        });

        return await completion.Task;
    }
}