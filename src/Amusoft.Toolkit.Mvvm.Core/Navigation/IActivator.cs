using System;

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IActivator
{
	object CreateInstance(Type modelType, object[] arguments);
}