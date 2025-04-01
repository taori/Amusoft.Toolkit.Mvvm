using System;
using Amusoft.Toolkit.Mvvm.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.Wpf;

/// <summary> 
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds Services related to MVVM for WPF
	/// </summary>
	/// <param name="services"></param>
	/// <param name="setupAction"></param>
	public static void AddMvvmWpf(this IServiceCollection services, Action<MvvmOptions>? setupAction = null)
	{
		services.AddMvvmCoreServices();
		services.AddSingleton<IDataTemplateResourceAppender, DataTemplateResourceAppender>();
		if (setupAction == null)
		{
			services.AddOptions<MvvmOptions>();
		}
		else
		{
			services.Configure(setupAction);
		}
	}
}