// This file is licensed to you under the MIT license.

namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests.Helpers;

public static class ThreadHelper
{
	public static Task RunAsStaThreadAsync(Action action)
	{
		var tcs = new TaskCompletionSource();
		var thread = new Thread(() =>
		{
			try
			{
				action();
				tcs.SetResult();
			}
			catch (Exception e)
			{
				tcs.SetException(e);
			}
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		return tcs.Task;
	}
}