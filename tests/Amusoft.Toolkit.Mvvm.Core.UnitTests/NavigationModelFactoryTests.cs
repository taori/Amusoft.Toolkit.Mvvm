// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Moq;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests;

public class NavigationModelFactoryTests
{
	[Fact]
	public async Task IfActivatorReturnsNonDesiredModelItThrows()
	{
		var activator = new Mock<IActivator>();
		activator
			.Setup(d => d.CreateInstance(It.IsAny<Type>(), It.IsAny<object[]>()))
			.Returns(new object());
		var store = new NavigationModelFactory([], activator.Object);
		var ex = Assert.Throws<MvvmCoreException>(() => store.Create(new TestDummyModel()));
		await Verify(ex.Message);
	}
}