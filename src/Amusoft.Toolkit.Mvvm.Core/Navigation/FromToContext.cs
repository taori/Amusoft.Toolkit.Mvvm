namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Provides context for a scenario where the current model is being entered
/// </summary>
public class FromToNavigationContext
{
	internal FromToNavigationContext(object? previousModel)
	{
		PreviousModel = previousModel;
	}

	/// <summary>
	/// The previous model
	/// </summary>
	public object? PreviousModel { get; }
}