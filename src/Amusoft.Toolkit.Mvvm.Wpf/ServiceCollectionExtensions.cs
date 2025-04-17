using System;
using Amusoft.Toolkit.Mvvm.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
	public static void AddMvvmWpf(this IServiceCollection services, Action<MvvmOptions>? setupAction = default)
	{
		services.AddMvvmCoreServices(setupAction);
		services.AddSingleton<ITemplateGenerator, TemplateGenerator>();
		services.AddSingleton<IViewMapper, ViewMapper>();
	}
}