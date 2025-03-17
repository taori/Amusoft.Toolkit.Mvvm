namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRestoreStrategy<TModel>
{
	int Priority { get; }
	void CollectRestoreInformation(TModel model);
	TModel Recreate();
}