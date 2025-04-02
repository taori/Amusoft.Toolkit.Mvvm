namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IViewModelToViewMapper
{
	MvvmMappingResult GetResult(IMappingTypeSource mappingTypeSource);
}