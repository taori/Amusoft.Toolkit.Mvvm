// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests;

public class ViewMappingsTests
{
	[Fact]
	public void VerifyCustomSourcesWork()
	{
		var mappings = new ViewMappings();
		mappings.WithSource(new MockedMappingTypeSource([], []));
		((Core.IViewMappingTypeSourceContainer)mappings).Registrations.Count.ShouldBe(1);
		((Core.IViewMappingTypeSourceContainer)mappings).Registrations[0].ShouldBeOfType<MockedMappingTypeSource>();
	}
}