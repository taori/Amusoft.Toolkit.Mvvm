// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

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
			var matcher = new NamespaceConventionViewModelToViewMapper();
			matcher.ViewPattern = new(".+");
			var ex = Assert.Throws<MvvmCoreException>(() => matcher.GetResult(new MockedMappingTypeSource([typeof(TestAVM)], [typeof(TestAView)])));
			ex.Message.ShouldBe("The regex group \"match\" is missing.");
		}
		
		[Fact]
		public void ViewModelPatternRequiresMatchGroup()
		{
			var matcher = new NamespaceConventionViewModelToViewMapper();
			matcher.ViewModelPattern = new(".+");
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