using Amusoft.Toolkit.Mvvm.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TestApp.Wpf.ViewModels;

public class GcVM : ObservableObject
{
	private byte[] bytes = new byte[120_000_000]; 
	private readonly INavigationService _navigationService;

	public GcVM(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}
}