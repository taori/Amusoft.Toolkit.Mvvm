using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class RestorePropertyRestoreStrategyProvider : IRestoreStrategyProvider
{
	private readonly IServiceProvider _serviceProvider;

	public RestorePropertyRestoreStrategyProvider(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
	
	public IEnumerable<IRestoreStrategy<T>> GetStrategies<T>()
	{
		yield return new Strategy<T>(_serviceProvider);
	}
	
	private class Strategy<T> : IRestoreStrategy<T>
	{
		private readonly IServiceProvider _serviceProvider;

		public Strategy(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}
		
		public int Priority { get; }
		
		public void CollectRestoreInformation(T model)
		{
			var properties = typeof(T)
				.GetProperties(BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Instance);
			Storage = properties
				.Where(d => CustomAttributeExtensions.GetCustomAttribute<RestorePropertyAttribute>(d) != null)
				.Select(d => (accessor: d, originalValue: d.GetValue(model)))
				.ToArray();
		}

		private (PropertyInfo accessor, object? originalValue)[] Storage { get; set; } = [];

		public T Recreate()
		{
			var instance = _serviceProvider.GetRequiredService<T>();
			foreach (var tuple in Storage)
			{
				tuple.accessor.SetValue(instance, tuple.originalValue);
			}
			
			return instance;
		}
	}
}

/// <summary>
/// Use this attribute to define properties that are persisted to restore the state of a viewmodel if its reference expired
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class RestorePropertyAttribute : Attribute
{
}