// This file is licensed to you under the MIT license.

using System.Windows.Controls;

using Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests.Helpers;

using JetBrains.dotMemoryUnit;

using Shouldly;

[assembly: DotMemoryUnit(CollectAllocations = true, FailIfRunWithoutSupport = false)]
namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests;

public class ContentControlRegionControlTests : IntegrationTestBase
{
	private readonly ITestOutputHelper _testOutputHelper;

	public ContentControlRegionControlTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
		DotMemoryUnitTestOutput.SetOutputMethod(testOutputHelper.WriteLine);
	}
	
	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task RegionNameSetCorrectly(string regionName)
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				var control = new ContentControlRegionControl(new ContentControl(), regionName);
				control.RegionName.ShouldBe(regionName);
			}
		);
	}
	
	[Fact]
	public async Task RegionNameThrowsOnNull()
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				var ex = Assert.Throws<ArgumentNullException>(() => new ContentControlRegionControl(new ContentControl(), null));
				ex.ParamName.ShouldBe("regionName");
			}
		);
	}
	
	[Fact]
	public async Task ControlThrowsOnNull()
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				var ex = Assert.Throws<ArgumentNullException>(() => new ContentControlRegionControl(null, "Name"));
				ex.ParamName.ShouldBe("control");
			}
		);
	}
	
	[Theory]
	[InlineData(42)]
	[InlineData(41)]
	public async Task ControlIsDisposable(int value)
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				ContentControlRegionControl? control = null;
				var scope = () =>
				{
					control = new ContentControlRegionControl(new ContentControl(){Content = value}, "test");
				};

				scope();

				control?.Content.ShouldBe(value);
				
				GC.Collect();
				
				control?.Content.ShouldBeNull();
			}
		);
	}
	
	[Theory]
	[InlineData(42)]
	[InlineData(41)]
	public async Task ControlContentSetterWorksWithWeakReference(int value)
	{
		await ThreadHelper.RunAsStaThreadAsync(() =>
			{
				ContentControlRegionControl? control = null;
				var scope = () =>
				{
					control = new ContentControlRegionControl(new ContentControl(), "test");
					control.Content = value;
				};

				scope();

				control?.Content.ShouldBe(value);
				
				GC.Collect();

				if (control is not null)
					control.Content = value;
				
				control?.Content.ShouldBeNull();
			}
		);
	}
}