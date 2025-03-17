using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Amusoft.Toolkit.Mvvm.Core.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;
    private readonly IRestoreStrategyProvider[] _restoreStrategyProviders;

    public NavigationService(IServiceProvider serviceProvider, IEnumerable<IRestoreStrategyProvider> strategyProviders, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger;
        _restoreStrategyProviders = strategyProviders.ToArray() ?? throw new ArgumentNullException(nameof(strategyProviders));
    }
    
    private readonly ConcurrentDictionary<string, Stack<NavigationModel>> _modelHistoryByRegionName = new();

    private readonly WeakEvent<NavigationHistoryChanged> _onHistoryChanged = new();
    public event NavigationHistoryChanged? OnHistoryChanged
    {
        add{ _onHistoryChanged.Add(value);}
        remove{ _onHistoryChanged.Remove(value);}
    }

    public async Task<bool> GoBackAsync(string regionName)
    {
        try
        {
            if (!_modelHistoryByRegionName.TryGetValue(regionName, out var navStack))
                return false;
            if (!navStack.TryPop(out var navModel))
                return false;
            if (!navModel.TryGetModelInstance(out var model))
                return false;
            if (!RegionRegister.TryGetRegion(regionName, out var control))
                return false;
            if (!await TryUpdateModelOfTheView(model, control, false))
                return false;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to navigate back on the history of the region \"{Name}\".", regionName);
            return false;
        }
        finally
        {
            _onHistoryChanged.Invoke(regionName);
        }
    }

    public void ClearHistory(string regionName)
    {
        _logger.LogDebug("The history for region {Name} is being cleared.", regionName);
        if (!_modelHistoryByRegionName.TryGetValue(regionName, out var stack))
            return;
        
        stack.Clear();
    }

    public bool CanGoBack(string regionName)
    {
        if (!_modelHistoryByRegionName.TryGetValue(regionName, out var stack))
            return false;

        return stack.Count > 0;
    }

    public void ClearView(string regionName)
    {
        if (!RegionRegister.TryGetRegion(regionName, out var control))
            return;
        
        control.Content = null;
    }

    public async Task<bool> PushAsync<TModel>(string regionName, TModel model) where TModel : class
    {
        if (!TryGetRegionByName(regionName, out var control))
            return false;

        return await TryPushModelToView(model, control);
    }

    private bool TryGetRegionByName(string regionName, [NotNullWhen(true)] out IRegionControl? control)
    {
        if (!RegionRegister.TryGetRegion(regionName, out control))
        {
            _logger.LogError("The region \"{Name}\" is not registered.", regionName);
            return false;
        }

        return true;
    }

    private async Task<bool> TryPushModelToView<TModel>(TModel model, IRegionControl control) where TModel : class
    {
        try
        {
            if (!await TryUpdateModelOfTheView(model, control, true))
                return false;

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to push the model to the view.");
            return false;
        }
    }

    private async Task<bool> TryUpdateModelOfTheView<TModel>(TModel model, IRegionControl control, bool addOldToHistory) where TModel : class
    {
        try
        {
            var oldModel = control.Content;
            if (oldModel is INavigateFrom navFrom && !await navFrom.OnNavigatedFromAsync(new NavigatedFromContext(model)))
            {
                _logger.LogDebug("A navigation request from {From} to {To} was declined.", oldModel.GetType().Name, model.GetType().Name);
                return false;
            }

            if (addOldToHistory)
                AddHistoryEntry(control.RegionName, oldModel);

            control.Content = model;

            if (model is INavigatedTo navigatedTo)
                await navigatedTo.OnNavigatedToAsync(new NavigatedToContext(oldModel));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "An error occured while trying to update the Content with the model of \"{Name}\" to the view registered as region \"{Region}\".",
                typeof(TModel).Name,
                control.RegionName
            );
            return false;
        }
        finally
        {
            _onHistoryChanged.Invoke(control.RegionName);
        }
    }

    private void AddHistoryEntry(string regionName, object? oldModel)
    {
        if (oldModel is not null)
        {
            var stack = _modelHistoryByRegionName.GetOrAdd(regionName, _ => new());
            // var method =

            IEnumerable<object> GetStrategies(IRestoreStrategyProvider provider)
            {
                var method = provider.GetType().GetMethod(nameof(IRestoreStrategyProvider.GetStrategies));
                var specificMethod = method.MakeGenericMethod(oldModel.GetType());
                var strategies = specificMethod.Invoke(provider, null);
                return strategies as IEnumerable<object>;
            }
        
            var restoreStrategies = _restoreStrategyProviders
                .SelectMany(d => GetStrategies(d))
                .ToArray();

            var castedStrategies = Array.CreateInstance(typeof(IRestoreStrategy<>).MakeGenericType(oldModel.GetType()), restoreStrategies.Length);
            Array.Copy(restoreStrategies, castedStrategies, restoreStrategies.Length);

            var modelType = typeof(NavigationModel<>).MakeGenericType(oldModel.GetType());
            var instance = Activator.CreateInstance(modelType, args: [oldModel, castedStrategies]);
            stack.Push(instance as NavigationModel);
        }
    }

    public async Task<bool> PushAsync<TModel>(string regionName, Action<TModel>? modification = null) where TModel : class
    {
        try
        {
            if (!TryGetRegionByName(regionName, out var control))
                return false;

            var model = _serviceProvider.GetRequiredService<TModel>();
            modification?.Invoke(model);

            return await TryPushModelToView(model, control);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while trying to push model \"{Name}\" to the view registered as region \"{Region}\".", typeof(TModel).Name, regionName);
            return false;
        }
    }
}
