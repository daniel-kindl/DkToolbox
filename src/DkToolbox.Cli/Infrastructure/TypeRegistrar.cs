using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace DkToolbox.Cli.Infrastructure;

internal sealed class TypeRegistrar(IServiceProvider services) : ITypeRegistrar
{
    public ITypeResolver Build()
    {
        return new TypeResolver(services);
    }

    public void Register(Type service, Type implementation)
    {
        // Not used when using existing IServiceProvider
    }

    public void RegisterInstance(Type service, object implementation)
    {
        // Not used when using existing IServiceProvider
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        // Not used when using existing IServiceProvider
    }
}

internal sealed class TypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type)
    {
        return type is null ? null : provider.GetService(type);
    }

    public void Dispose()
    {
        (provider as IDisposable)?.Dispose();
    }
}
