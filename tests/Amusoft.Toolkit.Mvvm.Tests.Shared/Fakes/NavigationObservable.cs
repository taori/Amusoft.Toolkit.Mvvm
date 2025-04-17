// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

internal class NavigationObservable : INavigatingTo, INavigateFrom, INavigatedTo
{
	public bool ResultOfNavigatedFrom = true;

	public int CallCountNavigatingTo = 0;
	public int CallCountNavigatedFrom = 0;
	public int CallCountNavigatedTo = 0;
	public object? NextModel;
	public object? PreviousModel;

	public Task OnNavigatingToAsync(FromToNavigationContext context)
	{
		CallCountNavigatingTo++;
		PreviousModel = context.PreviousModel;
		return Task.CompletedTask;
	}

	public Task<bool> OnNavigatedFromAsync(NavigatedFromContext context)
	{
		CallCountNavigatedFrom++;
		NextModel = context.NextModel;
		return Task.FromResult(ResultOfNavigatedFrom);
	}

	public Task OnNavigatedToAsync(FromToNavigationContext context)
	{
		CallCountNavigatedTo++;
		PreviousModel = context.PreviousModel;
		return Task.CompletedTask;
	}
}