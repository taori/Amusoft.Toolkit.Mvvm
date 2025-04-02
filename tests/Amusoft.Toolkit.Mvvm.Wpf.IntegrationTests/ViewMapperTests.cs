// This file is licensed to you under the MIT license.

using System.Windows;

using Amusoft.Toolkit.Mvvm.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

using Shouldly;

using ViewMapperTests.Data.ViewModels;
using ViewMapperTests.Data.Views;

namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests
{
	public class ViewMapperTests : IntegrationTestBase
	{
		[Fact]
		public async Task RunDefaultAssemblyMapping()
		{
			var generatedTemplates = new List<string>();
			DataTemplateGenerator.Generated += (_, args) => generatedTemplates.Add(args);
			
			var serviceProvider = GetServiceProvider(
				wpfConfiguration: options => options.ViewMappings
					.WithAssemblyConfiguration(d => d
						.WithAssembly(typeof(ViewMapperTests).Assembly)
						.WithViewFilter(v => v.FullName!.StartsWith("ViewMapperTests.Data.Views."))
						.WithViewModelFilter(v => v.FullName!.StartsWith("ViewMapperTests.Data.ViewModels."))
					)
			);
			var viewMapper = serviceProvider.GetRequiredService<IViewMapper>();
			var rd = new ResourceDictionary();
			viewMapper.Map(rd);

			await Verify(generatedTemplates);
		}
	}
}

namespace ViewMapperTests.Data
{
	namespace ViewModels
	{
		class TestModelAVM
		{
		}
		class TestModelBVM
		{
		}
	}
	namespace Views
	{
		class TestModelAView
		{
		}
		class TestModelBPage
		{
		}
	}
}