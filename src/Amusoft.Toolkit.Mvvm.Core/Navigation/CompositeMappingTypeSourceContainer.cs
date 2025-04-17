// This file is licensed to you under the MIT license.

using Microsoft.Extensions.Options;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class CompositeMappingTypeSourceContainer : ICompositeMappingTypeSourceContainer
{
	private readonly IOptions<MvvmOptions> _options;

	public CompositeMappingTypeSourceContainer(IOptions<MvvmOptions> options)
	{
		_options = options;
	}

	public IMappingTypeSource[] GetSources()
	{
		var options = (IViewMappingTypeSourceContainer)_options.Value.ViewMappings;
		return options.Registrations.ToArray();
	}
}