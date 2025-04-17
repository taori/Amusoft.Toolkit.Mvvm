using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// This class handles the settings to map viewmodels to views
/// </summary>
public class ViewMappings : IViewMappingTypeSourceContainer
{
	private readonly List<IMappingTypeSource> _registrations = new();

	/// <summary>
	/// Specifies to use an <see cref="IAssemblyMappingTypeSource"/> to configure which types should be used for mapping
	/// </summary>
	/// <param name="configuration">The configuration used to build the mappings from</param>
	/// <returns></returns>
	public ViewMappings WithAssemblyConfiguration(Action<IAssemblyMappingTypeSource> configuration)
	{
		var c = new AssemblyMappingTypeSource();
		configuration(c);
		_registrations.Add(c);
		return this;
	}

	/// <summary>
	/// Does the mapping based on a given custom configuration
	/// </summary>
	/// <param name="configuration"></param>
	/// <returns></returns>
	public ViewMappings WithSource(IMappingTypeSource configuration)
	{
		_registrations.Add(configuration);
		return this;
	}

	List<IMappingTypeSource> IViewMappingTypeSourceContainer.Registrations => _registrations;
}