// This file is licensed to you under the MIT license.

using System.Windows.Controls;

using Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests.Helpers;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests;

public class RegionManagerTests : IntegrationTestBase
{
	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task VerifyRegionNameGetterSetter(string regionName)
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				var contentControl = new ContentControl();
				RegionManager.SetRegionName(contentControl, regionName);
				RegionManager.GetRegionName(contentControl).ShouldBe(regionName);
			}
		);
	}
	
	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	public async Task VerifySetterThrowsIfRegionIsEmpty(string? regionName)
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				var contentControl = new ContentControl();
				var ex = Assert.Throws<RegionManagerException>(() => RegionManager.SetRegionName(contentControl, regionName!));
				ex.Message.ShouldBe("RegionName must be a string of length > 0");
			}
		);
	}
}