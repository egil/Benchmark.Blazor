namespace Benchmark.Blazor.Benchmarks;

public class Component1Benchmark
{
    private BenchmarkRenderer renderer = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var services = new ServiceCollection();

        // Add services to inject into components
        // rendered with the renderer here, e.g.:
        // services.AddSingleton<IFoo, Foo>();

        renderer = new BenchmarkRenderer(services.BuildServiceProvider());
    }

    [GlobalCleanup]
    public void GlobalCleanup() => renderer.Dispose();

    [Benchmark]
    public async Task<int> InitialRender()
    {
        // Render a component
        var component = await renderer.RenderAsync<Component1>(ParameterView.Empty);

        // Set new parameters and re-render
        await component.SetParametersAsync(ParameterView.FromDictionary(new Dictionary<string, object?>()
    {
        { "Text", "Foo" }
    }));

        // Re render without setting new parameters
        await component.SetParametersAsync(ParameterView.Empty);

        // Remove the component again from the render tree. Stops the
        // renderer from tracking the component.
        component.RemoveComponent();

        return renderer.RenderCount;
    }
}