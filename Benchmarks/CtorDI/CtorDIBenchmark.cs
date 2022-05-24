namespace Benchmark.Blazor.Benchmarks.CtorDI;

public class CtorDIBenchmark
{
    private BenchmarkRenderer renderer = default!;

    [GlobalSetup(Targets = new[] { nameof(RenderComponentWithoutDeps_UsingPropInjection), nameof(RenderComponentWithPropDep_UsingPropInjection) })]
    public void GlobalSetupNormalInjection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Service>();
        renderer = new BenchmarkRenderer(services.BuildServiceProvider());
    }

    [GlobalSetup(Targets = new[] { nameof(RenderComponentWithoutDeps_UsingCtorInjection), nameof(RenderComponentWithPropDep_UsingCtorInjection), nameof(RenderComponentWithCtorDep_UsingCtorInjection) })]
    public void GlobalSetupCtorInjection()
    {
        var services = new ServiceCollection();
        services.AddSingleton<Service>();
        services.AddSingleton<IComponentActivator, ConstructorInjectingComponentActivator>();
        renderer = new BenchmarkRenderer(services.BuildServiceProvider());
    }

    [GlobalCleanup]
    public void GlobalCleanup() => renderer.Dispose();

    [Benchmark(Baseline = true)]
    public async Task<int> RenderComponentWithoutDeps_UsingPropInjection()
    {
        var component = await renderer.RenderAsync<ComponentWithoutDeps>(ParameterView.Empty);
        component.RemoveComponent();
        return renderer.RenderCount;
    }

    [Benchmark]
    public async Task<int> RenderComponentWithPropDep_UsingPropInjection()
    {
        var component = await renderer.RenderAsync<ComponentWithPropDep>(ParameterView.Empty);
        component.RemoveComponent();
        return renderer.RenderCount;
    }

    [Benchmark]
    public async Task<int> RenderComponentWithoutDeps_UsingCtorInjection()
    {
        var component = await renderer.RenderAsync<ComponentWithoutDeps>(ParameterView.Empty);
        component.RemoveComponent();
        return renderer.RenderCount;
    }

    [Benchmark]
    public async Task<int> RenderComponentWithPropDep_UsingCtorInjection()
    {
        var component = await renderer.RenderAsync<ComponentWithPropDep>(ParameterView.Empty);
        component.RemoveComponent();
        return renderer.RenderCount;
    }

    [Benchmark]
    public async Task<int> RenderComponentWithCtorDep_UsingCtorInjection()
    {
        var component = await renderer.RenderAsync<ComponentWithCtorDep>(ParameterView.Empty);
        component.RemoveComponent();
        return renderer.RenderCount;
    }

}
