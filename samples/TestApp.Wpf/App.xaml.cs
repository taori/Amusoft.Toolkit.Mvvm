using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestApp.Wpf.ViewModels;

namespace TestApp.Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			var sc = new ServiceCollection();
			sc.AddLogging(builder => builder.AddDebug());
			sc.AddMvvmWpf(options =>
			{
				options.MappingInput
					.WithAssembly(typeof(App).Assembly)
					.WithViewFilter(d => d.FullName?.Contains(".Views.") ?? false)
					.WithViewModelFilter(d => d.FullName?.Contains(".ViewModels.") ?? false);
			});
			sc.AddTransient<ViewAVM>();
			sc.AddTransient<ViewBVM>();
			sc.AddTransient<GcVM>();
			sc.AddSingleton<MainVM>();
			this.ServiceProvider = sc.BuildServiceProvider();

			var templateSource = ServiceProvider.GetRequiredService<IDataTemplateResourceAppender>();
			templateSource.AppendTo(Current.Resources);
			base.OnStartup(e);
		}

		protected override void OnActivated(EventArgs e)
		{
			if (MainWindow != null)
				MainWindow.DataContext = this.ServiceProvider.GetRequiredService<MainVM>();
			base.OnActivated(e);
		}

		public ServiceProvider ServiceProvider { get; set; } = null!;
	}
}