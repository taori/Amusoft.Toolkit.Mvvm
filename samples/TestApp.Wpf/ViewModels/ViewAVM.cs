using System.Threading.Tasks;
using System.Windows;
using Amusoft.Toolkit.Mvvm.Core;

namespace TestApp.Wpf.ViewModels;

public class ViewAVM : PageViewVM, INavigatedTo, INavigateFrom
{
	public ViewAVM()
	{
		Title = "This is View A";
	}

	public Task OnNavigatedToAsync(NavigatedToContext context)
	{
		Title += $" - navigated from {context.PreviousModel?.GetType().Name ?? string.Empty}";
		return Task.CompletedTask;
	}

	public Task<bool> OnNavigatedFromAsync(NavigatedFromContext context)
	{
		return Task.FromResult(MessageBox.Show($"Do you want to navigate to {context.NextModel?.GetType().Name}", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
	}
}