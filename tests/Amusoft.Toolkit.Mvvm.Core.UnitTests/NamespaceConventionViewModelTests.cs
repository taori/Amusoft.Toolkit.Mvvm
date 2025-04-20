// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Microsoft.Extensions.Options;

using Moq;

using NamespaceConventionViewModel.ViewModels;

using Shouldly;

using ViewMappingEngineTests.Views;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests
{
	public class NamespaceConventionViewModelTests
	{
		[Fact]
		public void ViewPatternRequiresMatchGroup()
		{
			var options = new NamespaceConventionOptions();
			options.ViewPattern = new(".+");
			var mockOptions = new Mock<IOptions<NamespaceConventionOptions>>();
			mockOptions.Setup(d => d.Value).Returns(options);
			var matcher = new NamespaceConventionViewModelToViewMapper(mockOptions.Object);
			var ex = Assert.Throws<MvvmCoreException>(() => matcher.GetResult(new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestAView)])));
			ex.Message.ShouldBe("The regex group \"match\" is missing.");
		}
		
		[Fact]
		public void ViewModelPatternRequiresMatchGroup()
		{
			var options = new NamespaceConventionOptions();
			options.ViewModelPattern = new(".+");
			var mockOptions = new Mock<IOptions<NamespaceConventionOptions>>();
			mockOptions.Setup(d => d.Value).Returns(options);
			var matcher = new NamespaceConventionViewModelToViewMapper(mockOptions.Object);
			var ex = Assert.Throws<MvvmCoreException>(() => matcher.GetResult(new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestAView)])));
			ex.Message.ShouldBe("The regex group \"match\" is missing.");
		}
	}
}

namespace NamespaceConventionViewModel.Views
{
	public class TestAView{} 
	public class TestBView{} 
}
namespace NamespaceConventionViewModel.ViewModels
{
	public class TestAVM{} 
	public class TestBVM{} 
}