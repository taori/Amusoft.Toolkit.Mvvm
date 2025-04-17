namespace Amusoft.Toolkit.Mvvm.Core;

internal interface INavigationModelFactory
{
	NavigationModel Create(object model);
}