using System;
using System.Threading.Tasks;

using Amusoft.Toolkit.Mvvm.Core.Internals;

using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class NavigationService : INavigationService
{
	private readonly ILogger<NavigationService> _logger;
	private readonly IModelFactory _modelFactory;

	private readonly WeakEvent<NavigationHistoryChanged> _onHistoryChanged = new();
	private readonly IRegionModelHistory _regionModelHistory;
	private readonly IRegionRegister _regionRegister;

	public NavigationService(ILogger<NavigationService> logger, IModelFactory modelFactory, IRegionRegister regionRegister, IRegionModelHistory regionModelHistory)
	{
		_modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_regionRegister = regionRegister ?? throw new ArgumentNullException(nameof(regionRegister));
		_regionModelHistory = regionModelHistory ?? throw new ArgumentNullException(nameof(regionModelHistory));
	}

	public event NavigationHistoryChanged OnHistoryChanged
	{
		add { _onHistoryChanged.Add(value); }
		remove { _onHistoryChanged.Remove(value); }
	}

	public async Task<bool> GoBackAsync(string regionName)
	{
		var model = _regionModelHistory.GetLastRegionModel(regionName);
		var control = GetRegionControl(regionName);

		return await UpdateModelAsync(model, control, false);
	}

	public void ClearHistory(string regionName)
	{
		_regionModelHistory.Clear(regionName);
	}

	public bool CanGoBack(string regionName)
	{
		return _regionModelHistory.HasHistory(regionName);
	}

	public void ClearView(string regionName)
	{
		var control = GetRegionControl(regionName);
		control.Content = null;
	}

	public async Task<bool> PushAsync<TModel>(string regionName, TModel model) where TModel : class
	{
		return await PushModelToRegionAsync(model, regionName);
	}

	public async Task<bool> PushAsync<TModel>(string regionName, Action<TModel>? modification = null) where TModel : class
	{
		var model = _modelFactory.Create<TModel>();
		modification?.Invoke(model);

		return await PushModelToRegionAsync(model, regionName);
	}

	private IRegionControl GetRegionControl(string regionName)
	{
		if (!_regionRegister.TryGetRegion(regionName, out var control))
			throw new MvvmCoreException($"There was no region control found for region {regionName}.");

		return control;
	}

	private async Task<bool> PushModelToRegionAsync<TModel>(TModel model, string regionName) where TModel : class
	{
		var regionControl = GetRegionControl(regionName);
		return await UpdateModelAsync(model, regionControl, true);
	}

	private async Task<bool> UpdateModelAsync<TModel>(TModel model, IRegionControl control, bool addOldToHistory) where TModel : class
	{
		var oldModel = control.Content;
		if (!await ConfirmNavigationAsync(model, oldModel))
			return false;

		if (addOldToHistory && oldModel is not null)
			_regionModelHistory.PushEntry(control.RegionName, oldModel);

		if (model is INavigatingTo navigatingTo)
			await navigatingTo.OnNavigatingToAsync(new FromToNavigationContext(oldModel));

		control.Content = model;

		if (model is INavigatedTo navigatedTo)
			await navigatedTo.OnNavigatedToAsync(new FromToNavigationContext(oldModel));

		_onHistoryChanged.Invoke(control.RegionName);

		return true;
	}

	private async Task<bool> ConfirmNavigationAsync<TModel>(TModel newModel, object? oldModel) where TModel : class
	{
		if (oldModel is INavigateFrom navFrom && !await navFrom.OnNavigatedFromAsync(new NavigatedFromContext(newModel)))
		{
			_logger.LogInformation("A navigation request from {From} to {To} was declined.", oldModel.GetType().Name, newModel.GetType().Name);
			return false;
		}

		return true;
	}
}