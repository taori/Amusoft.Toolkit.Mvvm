# Getting Started

## WPF

- Utilize Microsoft.Extensions.DependencyInjection to provide your viewmodels
- Install `Amusoft.Toolkit.Mvvm.Wpf` via nuget

### Setup App.xaml.cs

```csharp
protected override void OnStartup(StartupEventArgs e)
{
	var sc = new ServiceCollection();
	sc.AddLogging(builder => builder.AddDebug());
	sc.AddMvvmWpf(options =>
	{
		options.ViewMappings
			.WithAssemblyConfiguration(configuration: source => source
				.WithAssembly(typeof(App).Assembly)
				.WithViewFilter(d => d.FullName?.Contains(".Views.") ?? false)
				.WithViewModelFilter(d => d.FullName?.Contains(".ViewModels.") ?? false));
	});
	sc.AddTransient<ViewAVM>();
	sc.AddTransient<ViewBVM>();
	sc.AddTransient<GcVM>();
	sc.AddSingleton<MainVM>();
	this.ServiceProvider = sc.BuildServiceProvider();

	// This maps viewmodels to view implementations. 
	var viewMapper = ServiceProvider.GetRequiredService<IViewMapper>();
	viewMapper.Map(Resources);
	base.OnStartup(e);
}
		
```

### Setup in View

register this namespace:
```xaml
xmlns:amumvvm="http://schemas.amusoft.com/wpf/2025/xaml"
```

within the view:
```xaml        
<ContentControl amumvvm:RegionManager.RegionName="Main"
	 Content="{Binding Path=Content}"
	 Grid.Row="1"/>
```

### Setup in viewModel

Inject INavigationService to use convention based model navigation

```csharp
public MainVM(INavigationService navigationService)
{
	_navigationService = navigationService;
	_navigationService.OnHistoryChanged += OnNavigationServiceOnOnHistoryChanged;
}
```
By default this maps views and viewmodels that end with patterns specified [here](../src/Amusoft.Toolkit.Mvvm.Core/Navigation/NamespaceConventionOptions.cs).

This can change the conventions like this:

```csharp
collection.Configure<NamespaceConventionOptions>(options =>
{
	options.ViewModelPattern = ...
	options.ViewModelTruncateEndPattern = ...
	options.ViewPattern = ...
	options.ViewTruncateEndPattern = ...;
});
```

### Feature interfaces for viewmodels:
- [INavigationService](../src/Amusoft.Toolkit.Mvvm.Core/Navigation/INavigationService.cs)
- [INavigatedTo](../src/Amusoft.Toolkit.Mvvm.Core/Navigation/INavigatedTo.cs) (callback interface if your viewmodel is navigated to, using the INavigationService)
- [INavigatedFrom](../src/Amusoft.Toolkit.Mvvm.Core/Navigation/INavigateFrom.cs) (callback interface to intercept navigation attempts from your current viewmodel)
- [IRestoreCallback](../src/Amusoft.Toolkit.Mvvm.Core/Navigation/IRestoreCallback.cs) (callback to respond to the resurrection of a viewmodel that has been garbage collected)

Those are the key interfaces to interact with this toolkit. There is some extensibility present, which isn't documented yet.