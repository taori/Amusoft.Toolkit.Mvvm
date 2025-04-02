// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Moq;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests;

public class RegionModelStoreTests
{
	[Fact]
	public async Task VerifyGetLastModelOfEmptyRegion()
	{
		var navigationModelFactory = new Mock<INavigationModelFactory>();
		var store = new RegionModelStore(navigationModelFactory.Object);
		var regionName = "TestRegion";
		store.PushModel(regionName, new TestDummyModel());
		store.Clear(regionName);
		var ex = Assert.Throws<MvvmCoreException>(() => store.GetLastModel(regionName));
		await Verify(ex.Message);
	}
}