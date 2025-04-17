using System;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class Activator : IActivator
{
	public object CreateInstance(Type modelType, object[] arguments)
	{
		return System.Activator.CreateInstance(modelType, args: arguments) ?? 
		       throw new MvvmCoreException($"Failed to create an instance of type {modelType.FullName}");
	}
}