using System;
using System.Threading.Tasks;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// <see cref="INavigationService"/> helps you interact with registered regions that were registered using the RegionManager in XAML
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// This event is raised when the navigation history of a region has been changed
    /// </summary>
    event NavigationHistoryChanged OnHistoryChanged;
    
    /// <summary>
    /// Navigates the target region back
    /// </summary>
    /// <param name="regionName"></param>
    /// <returns></returns>
    Task<bool> GoBackAsync(string regionName);

    /// <summary>
    /// Clears the history of a given region
    /// </summary>
    /// <param name="regionName"></param>
    void ClearHistory(string regionName);
    
    /// <summary>
    /// Whether or not it is possible to navigate backwards on the given region
    /// </summary>
    /// <param name="regionName"></param>
    /// <returns></returns>
    bool CanGoBack(string regionName);
    
    /// <summary>
    /// Clears the view which is currently active on a given region name
    /// </summary>
    /// <param name="regionName"></param>
    void ClearView(string regionName);

    /// <summary>
    /// Pushes a model onto the navigation stack of a registered region
    /// </summary>
    /// <param name="regionName"></param>
    /// <param name="model"></param>
    /// <typeparam name="TModel"></typeparam>
    Task<bool> PushAsync<TModel>(string regionName, TModel model) where TModel : class;

    /// <summary>
    /// Pushes a model, which was created via DI onto the navigation stack of a registered region
    /// </summary>
    /// <param name="regionName"></param>
    /// <param name="modification"></param>
    /// <typeparam name="TModel"></typeparam>
    Task<bool> PushAsync<TModel>(string regionName, Action<TModel>? modification = null) where TModel : class;
}

/// <summary>
/// This event is raised when the navigation history of a region has been changed
/// </summary>
public delegate void NavigationHistoryChanged(string regionName);
