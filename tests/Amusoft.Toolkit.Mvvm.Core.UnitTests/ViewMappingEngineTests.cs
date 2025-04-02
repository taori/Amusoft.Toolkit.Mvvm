// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Extensions;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;

using Moq;

using Shouldly;

using ViewMappingEngineTests.ViewModels;
using ViewMappingEngineTests.Views;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests
{
	public class ViewMappingEngineTests
	{
		private (Core.ViewMappingEngine, FakeLogger<Core.ViewMappingEngine>) MockedService(
			Action<Mock<Core.ICompositeMappingTypeSourceContainer>>? compositeMappingTypeSourceContainer = null,
			Func<Core.IViewModelToViewMapper[]>? viewModelToViewMapper = null
		)
		{
			var logger = new FakeLogger<Core.ViewMappingEngine>();
			compositeMappingTypeSourceContainer ??= setup => { };
			viewModelToViewMapper ??= () => [];
			var m2 = new Mock<Core.ICompositeMappingTypeSourceContainer>();
			compositeMappingTypeSourceContainer(m2);
			return (new Core.ViewMappingEngine(logger, viewModelToViewMapper(), m2.Object), logger);
		}

		[Fact]
		public async Task MatchGranted()
		{
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [new NamespaceConventionViewModelToViewMapper()],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestAView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(1);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MatchDuplicated()
		{
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [new NamespaceConventionViewModelToViewMapper()],
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
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [new NamespaceConventionViewModelToViewMapper()],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MissmatchBoth()
		{
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [new NamespaceConventionViewModelToViewMapper()],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestBView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task MissmatchedView()
		{
			var (service, logger) = MockedService(
				viewModelToViewMapper: () => [new NamespaceConventionViewModelToViewMapper()],
				compositeMappingTypeSourceContainer:
					mock => mock.Setup(d => d.GetSources()).Returns(() => [new MockedMappingTypeSource([], [typeof(TestAView)])])
			);
		
			service.GetMappings().ToArray().Length.ShouldBe(0);
			await Verify(logger.Collector.GetFullLoggerText());
		}

		[Fact]
		public async Task VerifyCaseMapperDuplicatesEntries()
		{
			var mapperMock = new Mock<Core.IViewModelToViewMapper>();
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