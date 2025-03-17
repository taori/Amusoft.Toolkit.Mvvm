using CommunityToolkit.Mvvm.ComponentModel;

namespace TestApp.Wpf.ViewModels;

public partial class PageViewVM : ObservableObject
{
	[ObservableProperty]
	private string? _title;
}