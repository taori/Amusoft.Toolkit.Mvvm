namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// <para><see cref="NavigationModel"/> stores models in a wrapped way, to allow for garbage collection.</para>
/// <para>If a model is disposed and later recreated, placing this interface on a viewmodel will provide a way to have a callback that is called after restoring a viewmodel.</para>
/// </summary>
public interface IRestoreCallback
{
	/// <summary>
	/// Called after a model has been previously disposed, and is later recreated. Use this callback to respond to the recreation of a model
	/// </summary>
	void OnRestored();
}