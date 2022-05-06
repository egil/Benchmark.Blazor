# Benchmark.Blazor
This is a sample [benchmark.net](https://github.com/dotnet/BenchmarkDotNet) test app that can be used to 
measure Blazor components. 

Read the [documentation for benchmark.net](https://benchmarkdotnet.org/) to learn how to use it.

## The `BenchmarkRenderer`

A minimal Blazor renderer is included in the form of the `BenchmarkRenderer`, that just allows 
rendering of a component one or more times, with or without a `ParameterView` passed in. It does
not produce any markup or output.

### Usage:

To **create an instance** of the `BenchmarkRenderer`, do the following:

```c#
var renderer = new BenchmarkRenderer();
```

To **enable services to be injected** into the component being measured, do the following:

```c#
var services = new ServiceCollection();

// Add services to inject into components
// rendered with the renderer here, e.g.:
services.AddSingleton<IFoo, Foo>();

var renderer = new BenchmarkRenderer();
```

To **render a component** (any type that implements `IComponent`), do the following:

```c#
// Render without parameters passed to Component1
var component = await renderer.Render<Component1>(ParameterView.Empty);

// Render with parameters passed to Component1
component = await renderer.Render<Component1>(ParameterView.FromDictionary(new Dictionary<string, object?>()
{
    { "Text", "Foo" }
}));
```

To **re-render and optionally pass new parameters** to a component, do the following:

```c#
// Set new parameters and re-render
await renderer.SetParametersAsync(component, ParameterView.FromDictionary(new Dictionary<string, object?>()
{
    { "Text", "Foo" }
}));

// Re render without setting new parameters
await renderer.SetParametersAsync(component, ParameterView.Empty);
```

**Tips:**

- Get the number of render cycles performed by the `BenchmarkRenderer` through `BenchmarkRenderer.RenderCount` property.
- Get any unhandled exception thrown during rendering via the `BenchmarkRenderer.UnhandledException` property. This is reset at the beginning of calls to `BenchmarkRenderer.Render()` and `BenchmarkRenderer.SetParametersAsync()`.
