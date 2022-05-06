namespace Benchmark.Blazor.Benchmarks;

public class Component1Benchmark : IDisposable
{
    private readonly BenchmarkRenderer renderer;

    public Component1Benchmark()
    {
        var services = new ServiceCollection();

        // Add services to inject into components
        // rendered with the renderer here, e.g.:
        // services.AddSingleton<IFoo, Foo>();

        renderer = new BenchmarkRenderer(services.BuildServiceProvider());
    }

    public void Dispose() => renderer.Dispose();

    [Benchmark]
    public async Task<int> InitialRender()
    {
        // Render a component
        var component = await renderer.RenderAsync<Component1>(ParameterView.Empty);

        // Set new parameters and re-render
        await renderer.SetParametersAsync(component, ParameterView.FromDictionary(new Dictionary<string, object?>()
        {
            { "Text", "Foo" }
        }));

        // Re render without setting new parameters
        await renderer.SetParametersAsync(component, ParameterView.Empty);

        return renderer.RenderCount;
    }
}
