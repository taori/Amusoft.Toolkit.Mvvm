namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Provides context for a scenario where the current model is being left
/// </summary>
public class NavigatedFromContext
{
	internal NavigatedFromContext(object nextModel)
	{
		NextModel = nextModel;
	}

	/// <summary>
	/// The destination model
	/// </summary>
	public object NextModel { get; }
}