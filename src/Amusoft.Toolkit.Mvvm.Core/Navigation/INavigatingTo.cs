// This file is licensed to you under the MIT license.

using System.Threading.Tasks;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Implement this interfaces if you want to react to the <see cref="INavigationService"/> pushing a model to a view before the view is visible
/// </summary>
public interface INavigatingTo
{
	/// <summary>
	/// </summary>
	/// <returns></returns>
	Task OnNavigatingToAsync(FromToNavigationContext context);
}