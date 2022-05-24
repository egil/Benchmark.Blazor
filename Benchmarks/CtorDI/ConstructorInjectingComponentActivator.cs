using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.AspNetCore.Components;

public class ConstructorInjectingComponentActivator : IComponentActivator
{
    private readonly IServiceProvider provider;

    public ConstructorInjectingComponentActivator(IServiceProvider provider)
    {
        this.provider = provider;
    }

    public IComponent CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type componentType)
    {
        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException($"The type {componentType.FullName} does not implement {nameof(IComponent)}.", nameof(componentType));
        }

        if (TryGetDefaultConstructor(componentType, out var constructor))
        {
            // This is same logic that is inside the default component activator
            // inside Blazor.
            return (IComponent)Activator.CreateInstance(componentType)!;
        }
        else
        {
            // Calling provider.GetService(componentType) directly
            // will not work since the service provider will keep
            // a reference to the created component instance until
            // it itself is disposed. Instead, we use this method
            // that uses the service provider to resolve any ctor
            // dependencies the component type has, but do not
            // create the component itself, and thus does not 
            // attempt to manage the created components life time.
            return (IComponent)ActivatorUtilities.CreateInstance(provider, componentType);
        }
    }

    private static bool TryGetDefaultConstructor(Type componentType, [NotNullWhen(true)] out ConstructorInfo? constructor)
    {
        constructor = null;
        
        if (componentType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, types: Array.Empty<Type>()) is ConstructorInfo ctor)
        {
            constructor = ctor;
            return true;
        }

        return false;
    }
}
