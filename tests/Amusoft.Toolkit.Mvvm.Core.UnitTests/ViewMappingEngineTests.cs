// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Extensions;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;

using Moq;

using Shouldly;

using ViewMappingEngineTests.ViewModels;
using ViewMappingEngineTests.Views;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests
{
	public class ViewMappingEngineTests
	{
		private (ViewMappingEngine, FakeLogger<ViewMappingEngine>) MockedService(
			Action<Mock<ICompositeMappingTypeSourceContainer>>? compositeMappingTypeSourceContainer = null,
			Func<IViewModelToViewMapper[]>? viewModelToViewMapper = null,
			Action<NamespaceConventionOptions>? namespaceConventionOptions = null)
		{
			var logger = new FakeLogger<ViewMappingEngine>();
			compositeMappingTypeSourceContainer ??= setup => { };
			viewModelToViewMapper ??= () => [];
			var m2 = new Mock<ICompositeMappingTypeSourceContainer>();
			compositeMappingTypeSourceContainer(m2);
			return (new ViewMappingEngine(logger, viewModelToViewMapper(), m2.Object), logger);
		}

		[Fact]
		public async Task MatchGranted()
		{
			var mapper = new NamespaceConventionViewModelToViewMapper(GetConfiguredOptions());
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapper],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestAView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(1);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MatchDuplicated()
		{
			var mapper = new NamespaceConventionViewModelToViewMapper(GetConfiguredOptions());
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapper],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() =>
						{
							Type[] viewModelTypes = [typeof(TestAVM),typeof(TestAVM)];
							Type[] viewTypes = [typeof(TestAView),typeof(TestAView)];
							return [new MockedMappingTypeSource(viewModelTypes, viewTypes)];
						}
					)
			);

			var ex = Assert.Throws<ArgumentException>(() => service.GetMappings().ToArray());
			await Verify(new
			{
				exception = ex.Message, 
				log = logger.Collector.GetFullLoggerText()
			});
		}

		[Fact]
		public async Task MissmatchedViewModel()
		{
			var mapper = new NamespaceConventionViewModelToViewMapper(GetConfiguredOptions());
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapper],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MissmatchBoth()
		{
			var mapper = new NamespaceConventionViewModelToViewMapper(GetConfiguredOptions());
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapper],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestBView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MissmatchedView()
		{
			var mapper = new NamespaceConventionViewModelToViewMapper(GetConfiguredOptions());
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapper],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([], [typeof(TestAView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task VerifyCaseMapperDuplicatesEntries()
		{
			var mapperMock = new Mock<IViewModelToViewMapper>();
			mapperMock
				.Setup(d => d.GetResult(It.IsAny<IMappingTypeSource>()))
				.Returns(new MvvmMappingResult([
					(typeof(TestAVM), typeof(TestAView)),
					(typeof(TestAVM), typeof(TestAView)),
				], [], []));
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [mapperMock.Object],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([], [typeof(TestAView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(1);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		private static IOptions<NamespaceConventionOptions> GetConfiguredOptions(Action<NamespaceConventionOptions>? configure = null)
		{
			var options = new NamespaceConventionOptions();
			configure?.Invoke(options);
			return Options.Create(options);
		}
	}
}

namespace ViewMappingEngineTests.Views
{
	public class TestAView{} 
	public class TestBView{} 
}
namespace ViewMappingEngineTests.ViewModels
{
	public class TestAVM{} 
	public class TestBVM{} 
}