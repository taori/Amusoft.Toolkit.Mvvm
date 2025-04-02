// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests;

public class RegionRegisterTests
{
	[Fact]
	public void RegionControlMustNotBeNull()
	{
		var register = Core.RegionRegister.Instance;
		Assert.Throws<ArgumentNullException>(() => register.RegisterRegion(null));
	}
	[Fact]
	public void VerifyNonExisting()
	{
		var register = Core.RegionRegister.Instance;
		register.TryGetRegion("UnknownRegion", out var region).ShouldBeFalse();
		region.ShouldBeNull();
	}
	
	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public void VerifyExisting(string regionName)
	{
		var register = Core.RegionRegister.Instance;
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		register.TryGetRegion(regionName, out var region).ShouldBeTrue();
		region.ShouldNotBeNull();
	}
}