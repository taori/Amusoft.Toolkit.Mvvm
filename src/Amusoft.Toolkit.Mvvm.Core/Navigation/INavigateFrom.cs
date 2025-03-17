using System.Threading.Tasks;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// This interface serves the purpose of being aware of navigation away from the current model
/// </summary>
public interface INavigateFrom
{
	/// <summary>
	/// This method is called prior to attempting to change the view of a given region
	/// </summary>
	/// <param name="context"></param>
	/// <returns>return true if you want to allow navigation, false if you want to prevent it</returns>
	Task<bool> OnNavigatedFromAsync(NavigatedFromContext context);
}