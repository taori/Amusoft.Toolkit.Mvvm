using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Amusoft.Toolkit.Mvvm.Core;

internal abstract class NavigationModel
{
	public NavigationModel(Type modelType)
	{
		ModelType = modelType;
	}

	public Type ModelType { get; set; }
	
	public abstract object? GetModel();
}

internal class NavigationModel<T> : NavigationModel where T : class
{
	private readonly IRestoreStrategy<T>[] _restoreStrategies;

	public NavigationModel(T model, IRestoreStrategy<T>[] restoreStrategies) : base(model.GetType())
	{
		_restoreStrategies = restoreStrategies;
		_modelReference = new WeakReference<T>(model);
		foreach (var restoreStrategy in restoreStrategies)
		{
			restoreStrategy.CollectRestoreInformation(model);
		}
	}

	private WeakReference<T> _modelReference;

	public override object? GetModel()
	{
		if (_modelReference.TryGetTarget(out var typedModel))
		{
			return typedModel;		
		}

		var restoredModel = _restoreStrategies
			.OrderByDescending(d => d.Priority)
			.FirstOrDefault()?
			.Recreate();

		if (restoredModel == null)
			return null;

		_modelReference = new WeakReference<T>(restoredModel);
		return restoredModel;
	}
}