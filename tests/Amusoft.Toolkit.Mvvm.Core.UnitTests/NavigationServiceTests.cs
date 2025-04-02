// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Microsoft.Extensions.Logging;

using Moq;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests;

public class NavigationServiceTests
{
	private Core.NavigationService MockedService(
		Action<Mock<IModelFactory>>? serviceProvider = null,
		Action<Mock<Core.IRegionRegister>>? regionRegister = null,
		Action<Mock<Core.IRegionModelHistory>>? regionModelHistory = null,
		Action<Mock<ILogger<Core.NavigationService>>>? logger = null
	)
	{
		logger ??= setup => { };
		serviceProvider ??= setup => { };
		regionRegister ??= setup => { };
		regionModelHistory ??= setup => { };
		var m0 = new Mock<ILogger<Core.NavigationService>>();
		var m1 = new Mock<IModelFactory>();
		var m2 = new Mock<Core.IRegionRegister>();
		var m3 = new Mock<Core.IRegionModelHistory>();
		logger(m0);
		serviceProvider(m1);
		regionRegister(m2);
		regionModelHistory(m3);
		return new Core.NavigationService(m0.Object, m1.Object, m2.Object, m3.Object);
	}

	[Fact]
	public void CannotGoBackIfHistoryReturnsFalse()
	{
		Stack<Core.NavigationModel> history = new();
		var ns = MockedService(
			regionModelHistory: mock => mock
				.Setup(d => d.HasHistory(It.IsAny<string>()))
				.Returns(false)
		);

		ns.CanGoBack("UnknownRegion").ShouldBeFalse();
	}

	[Fact]
	public void CanGoBackIfRegionDoesExist()
	{
		Stack<Core.NavigationModel> history = new();
		history.Push(new Core.NavigationModel<TestDummyModel>(new TestDummyModel(), []));
		var ns = MockedService(
			regionModelHistory: mock => mock
				.Setup(d => d.HasHistory(It.IsAny<string>()))
				.Returns(true)
		);

		ns.CanGoBack("UnknownRegion").ShouldBeTrue();
	}

	[Fact]
	public async Task GoBackExceptionIsCaught()
	{
		var ns = MockedService(
			regionModelHistory: mock => mock
				.Setup(d => d.GetLastRegionModel(It.IsAny<string>()))
				.Throws<MvvmCoreException>()
		);

		await Assert.ThrowsAsync<MvvmCoreException>(async () => await ns.GoBackAsync("UnknownRegion"));
	}

	[Fact]
	public void ClearHistory()
	{
		var ns = MockedService(
			regionModelHistory: mock => mock
				.Setup(d => d.Clear(It.IsAny<string>()))
				.Throws<MvvmCoreException>()
		);

		Assert.Throws<MvvmCoreException>(() => ns.ClearHistory("UnknownRegion"));
	}
}