// This file is licensed to you under the MIT license.

using System;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class ModelFactory : IModelFactory
{
	private readonly IServiceProvider _serviceProvider;

	public ModelFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
	
	public T Create<T>()
	{
		return _serviceProvider.GetRequiredService<T>();
	}
}