using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;

namespace Benchmark.Blazor.Rendering;

internal class BenchmarkRenderer : Renderer
{
    /// <inheritdoc/>
    public override Dispatcher Dispatcher { get; } = Dispatcher.CreateDefault();

    /// <summary>
    /// Any unhandled exceptions thrown by the Blazor Renderer after 
    /// a call to <see cref="Render{TComponent}(ParameterView)"/> or 
    /// <see cref="SetParametersAsync{TComponent}(TComponent, ParameterView)"/>.    
    /// </summary>
    /// <remarks>
    /// This is reset after each call to <see cref="Render{TComponent}(ParameterView)"/> or
    /// <see cref="SetParametersAsync{TComponent}(TComponent, ParameterView)"/>.
    /// </remarks>
    public Exception? UnhandledException { get; private set; }

    /// <summary>
    /// The number of times the render tree has been undated, e.g. 
    /// when a render cycle has happened (the <see cref="UpdateDisplayAsync(in RenderBatch)"/>)
    /// has been called.
    /// </summary>
    public int RenderCount { get; private set; }

    /// <summary>
    /// Create an instance of the <see cref="BenchmarkRenderer"/>. This is a minimal 
    /// <see cref="Renderer"/> that just allows the rendering of a component.
    /// </summary>
    /// <param name="serviceProvider">
    /// An optional <see cref="IServiceProvider"/> that can be used to inject 
    /// services into the component being rendered. 
    /// Default is an empty <see cref="ServiceProvider"/>.
    /// </param>
    /// <param name="loggerFactory">
    /// An optional <see cref="ILoggerFactory"/> that will collect logs from the <see cref="Renderer"/>.
    /// Default is <see cref="NullLoggerFactory"/>.
    /// </param>
    public BenchmarkRenderer(IServiceProvider? serviceProvider = null, ILoggerFactory? loggerFactory = null)
        : base(serviceProvider ?? new ServiceCollection().BuildServiceProvider(), loggerFactory ?? NullLoggerFactory.Instance)
    {
    }

    /// <summary>
    /// Renders a component of type <typeparamref name="TComponent"/> with the
    /// provided <paramref name="parameters"/>.    
    /// </summary>
    /// <remarks>
    /// Any unhandled exceptions during the first render is captured in the
    /// <see cref="UnhandledException"/> property.
    /// </remarks>
    /// <typeparam name="TComponent">The type of the component to render.</typeparam>
    /// <param name="parameters">
    /// The parameters to pass to the <typeparamref name="TComponent"/> during 
    /// first render. Use <see cref="ParameterView.Empty"/> to pass no parameters.</param>
    /// <returns>The instance of the <typeparamref name="TComponent"/>.</returns>
    public Task<TComponent> Render<TComponent>(ParameterView parameters)
        where TComponent : IComponent, new()
    {
        UnhandledException = null;

        var result = Dispatcher.InvokeAsync(async () =>
        {
            var component = (TComponent)InstantiateComponent(typeof(TComponent));
            var componentId = AssignRootComponentId(component);
            await RenderRootComponentAsync(componentId, parameters).ConfigureAwait(false);
            return component;
        });

        return result;
    }

    /// <summary>
    /// Pass the provided <paramref name="parameters"/> to the <paramref name="component"/>.
    /// For normal components that inherit from <see cref="ComponentBase"/> this causes
    /// the component to go through all its life cycle methods.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <param name="component">
    /// The component to pass new parameters to.
    /// Use <see cref="ParameterView.Empty"/> to pass no parameters and just trigger a re-render.
    /// </param>
    /// <param name="parameters">The parameters to pass to the <paramref name="component"/>.</param>
    /// <returns>A task that completes when the <see cref="IComponent.SetParametersAsync(ParameterView)"/>
    /// method completes.</returns>
    public Task SetParametersAsync<TComponent>(TComponent component, ParameterView parameters)
        where TComponent : IComponent
    {
        UnhandledException = null;

        return Dispatcher.InvokeAsync(() =>
        {
            return component.SetParametersAsync(parameters).ConfigureAwait(false);
        });
    }

    protected override void HandleException(Exception exception)
        => UnhandledException = exception;

    protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
    {
        // This is called after every render and
        // contains the changes/updates to the render tree.
        RenderCount++;
        return Task.CompletedTask;
    }
}