using Microsoft.Extensions.DependencyInjection;
using Stringification;
namespace InterReact;

internal static class Connector
{
    internal static async Task<IInterReactClient> CreateClientAsync(InterReactOptions options, CancellationToken ct)
    {
        Connection? connection = null;
        try
        {
#pragma warning disable CA2000 // Disposal is handled by InterReact.
            connection = await Connection.CreateAsync(options, ct).ConfigureAwait(false);
#pragma warning restore CA2000

            return new ServiceCollection()
                .AddSingleton(options)
                .AddSingleton(options.Clock)
                .AddSingleton(options.LogFactory) // add LoggerFactor before AddLogging()
                .AddLogging() // so that LogFactory is used rather than the LoggerFactory.
                .AddSingleton(connection) // instance
                .AddSingleton<Stringifier>()
                .AddSingleton<Request>()
                .AddTransient<RequestMessage>() // transient!
                .AddSingleton<Func<RequestMessage>>(s => () => s.GetRequiredService<RequestMessage>()) // factory
                .AddSingleton<Response>() // IObservable<object>
                .AddSingleton<ResponseReader>()
                .AddSingleton<ResponseParser>()
                .AddSingleton<ResponseMessageComposer>()
                .AddSingleton<Service>()
                .AddSingleton<IInterReactClient, InterReactClient>()
                .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true })
                .GetRequiredService<IInterReactClient>();
        }
        catch (Exception)
        {
            if (connection != null)
                await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }
}
