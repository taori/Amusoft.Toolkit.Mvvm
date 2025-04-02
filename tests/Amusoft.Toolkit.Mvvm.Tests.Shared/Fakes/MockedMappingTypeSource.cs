using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

internal class MockedMappingTypeSource : ICompositeMappingTypeSourceContainer, IMappingTypeSource
{
	private readonly Type[] _viewModelTypes;
	private readonly Type[] _viewTypes;

	public MockedMappingTypeSource(Type[] viewModelTypes, Type[] viewTypes)
	{
		_viewModelTypes = viewModelTypes;
		_viewTypes = viewTypes;
	}
		
	public IMappingTypeSource[] GetSources()
	{
		return [this];
	}

	public Type[] GetViewTypes()
	{
		return _viewTypes;
	}

	public Type[] GetViewModelTypes()
	{
		return _viewModelTypes;
	}
}