using System.Threading.Tasks;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Implement this interfaces if you want to react to the <see cref="INavigationService"/> pushing a model to a view
/// </summary>
public interface INavigatedTo
{
	/// <summary>
	/// </summary>
	/// <returns></returns>
	Task OnNavigatedToAsync(NavigatedToContext context);
}