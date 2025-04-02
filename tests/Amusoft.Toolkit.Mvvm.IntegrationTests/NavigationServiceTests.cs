// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests;

public class NavigationServiceTests : IntegrationTestBase
{
	[Fact]
	public void NoActionCanGoBackFalse()
	{
		var navigationService = GetNavigationService();
		navigationService.CanGoBack("TestRegion").ShouldBeFalse();
	}

	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushOrdinaryNoPrevHistoryCanGoBackIsFalse(string regionName)
	{
		var register = new ObservableRegionRegister();
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		(await navigationService.PushAsync(regionName, new TestDummyModel())).ShouldBeTrue();
		register.ControlsByName.ContainsKey(regionName).ShouldBeTrue();
		navigationService.CanGoBack(regionName).ShouldBeFalse();
	}

	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushOrdinaryWithPrevHistoryCanGoBackIsTrue(string regionName)
	{
		var register = new ObservableRegionRegister();
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		(await navigationService.PushAsync(regionName, new TestDummyModel())).ShouldBeTrue();
		(await navigationService.PushAsync(regionName, new TestDummyModel())).ShouldBeTrue();
		register.ControlsByName.ContainsKey(regionName).ShouldBeTrue();
		navigationService.CanGoBack(regionName).ShouldBeTrue();
	}

	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushGenericNoPrevHistoryCanGoBackIsFalse(string regionName)
	{
		var register = new ObservableRegionRegister();
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		var navigationService = GetNavigationService(collection =>
		{
			collection.AddTransient<TestDummyModel>();
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		(await navigationService.PushAsync<TestDummyModel>(regionName)).ShouldBeTrue();
		register.ControlsByName.ContainsKey(regionName).ShouldBeTrue();
		navigationService.CanGoBack(regionName).ShouldBeFalse();
	}

	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushGenericWithPrevHistoryCanGoBackIsTrue(string regionName)
	{
		var register = new ObservableRegionRegister();
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		var navigationService = GetNavigationService(collection =>
		{
			collection.AddTransient<TestDummyModel>();
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		(await navigationService.PushAsync<TestDummyModel>(regionName)).ShouldBeTrue();
		(await navigationService.PushAsync<TestDummyModel>(regionName)).ShouldBeTrue();
		register.ControlsByName.ContainsKey(regionName).ShouldBeTrue();
		navigationService.CanGoBack(regionName).ShouldBeTrue();
	}

	[Theory]
	[InlineData("Region1", "modelA")]
	[InlineData("Region2", "modelB")]
	public async Task PushGenericModificationChangesModel(string regionName, string modelText)
	{
		var register = new ObservableRegionRegister();
		register.RegisterRegion(new FakeRegionControl(regionName, null));
		var navigationService = GetNavigationService(collection =>
		{
			collection.AddTransient<TestDummyModel>();
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		(await navigationService.PushAsync<TestDummyModel>(regionName, model => model.Text = modelText)).ShouldBeTrue();
		register.ControlsByName.TryGetValue(regionName, out var regionControl).ShouldBeTrue();
		regionControl?.Content.ShouldNotBeNull();

		var casted = Assert.IsAssignableFrom<TestDummyModel>(regionControl!.Content);
		casted.Text.ShouldBe(modelText);
	}


	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushGenericRegionNoExists(string regionName)
	{
		var register = new ObservableRegionRegister();
		var navigationService = GetNavigationService(collection =>
		{
			collection.AddTransient<TestDummyModel>();
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});

		await Assert.ThrowsAsync<MvvmCoreException>(async () => await navigationService.PushAsync<TestDummyModel>(regionName));
	}

	[Theory]
	[InlineData("Region1")]
	[InlineData("Region2")]
	public async Task PushOrdinaryRegionNoExists(string regionName)
	{
		var register = new ObservableRegionRegister();
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});

		await Assert.ThrowsAsync<MvvmCoreException>(async () => await navigationService.PushAsync(regionName, new TestDummyModel()));
	}
	
	
	public static TheoryData<int, string> HistoryCalledWhenModelPushedToViewData = 
		new MatrixTheoryData<int, string>([1, 2, 3, 4], ["Region1", "Region2"]);
	
	[Theory]
	[MemberData(nameof(HistoryCalledWhenModelPushedToViewData))]
	public async Task HistoryCalledWhenModelPushedToView(int navigationSteps, string regionName)
	{
		var register = new ObservableRegionRegister();
		var control = new FakeRegionControl(regionName, new FakeRegionControl(regionName, null));
		register.RegisterRegion(control);
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});

		var historyChangedCount = 0;
		navigationService.OnHistoryChanged += name => { historyChangedCount++; };
		for (int i = 0; i < navigationSteps; i++)
		{
			await navigationService.PushAsync(regionName, new TestDummyModel());
		}
		historyChangedCount.ShouldBe(navigationSteps);
	}
	
	[Fact]
	public async Task HistoryChangedUnbindNotCalledAnymore()
	{
		var regionName = "TestRegion";
		var register = new ObservableRegionRegister();
		var control = new FakeRegionControl(regionName, new FakeRegionControl(regionName, null));
		register.RegisterRegion(control);
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});

		var historyChangedCount = 0;

		void OnNavigationServiceOnOnHistoryChanged(string name)
		{
			historyChangedCount++;
		}

		navigationService.OnHistoryChanged += OnNavigationServiceOnOnHistoryChanged;
		navigationService.OnHistoryChanged -= OnNavigationServiceOnOnHistoryChanged;
		await navigationService.PushAsync(regionName, new TestDummyModel());
		await navigationService.PushAsync(regionName, new TestDummyModel());
		historyChangedCount.ShouldBe(0);
	}

	[Fact]
	public async Task ClearViewSetsContentNull()
	{
		var regionName = "TestRegion";
		var register = new ObservableRegionRegister();
		var control = new FakeRegionControl(regionName, new FakeRegionControl(regionName, null));
		register.RegisterRegion(control);
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});

		var contentModel = new TestDummyModel();
		await navigationService.PushAsync(regionName, contentModel);
		control.Content.ShouldNotBeNull();
		
		navigationService.ClearView(regionName);
		control.Content.ShouldBeNull();
	}

	[Fact]
	public void ClearUnsetRegionThrows()
	{
		var regionName = "TestRegion";
		var register = new ObservableRegionRegister();
		var navigationService = GetNavigationService(collection =>
		{
			collection.Replace(ServiceDescriptor.Singleton<IRegionRegister>(register));
		});
		
		Assert.Throws<MvvmCoreException>(() => navigationService.ClearView(regionName));
	}

	[Fact]
	public void ClearHistoryThrowsIfEmpty()
	{
		var regionName = "TestRegion";
		var navigationService = GetNavigationService();
		
		Assert.Throws<MvvmCoreException>(() => navigationService.ClearHistory(regionName));
	}

	[Fact]
	public async Task ClearHistoryWorksIfExists()
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		regionRegister.RegisterRegion(new FakeRegionControl(regionName, null));

		navigationService.CanGoBack(regionName).ShouldBeFalse();
		await navigationService.PushAsync(regionName, new TestDummyModel());
		await navigationService.PushAsync(regionName, new TestDummyModel());
		navigationService.CanGoBack(regionName).ShouldBeTrue();
		navigationService.ClearHistory(regionName);
		navigationService.CanGoBack(regionName).ShouldBeFalse();
	}

	[Fact]
	public async Task GoBackWorksIfHistoryExists()
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		regionRegister.RegisterRegion(new FakeRegionControl(regionName, null));

		navigationService.CanGoBack(regionName).ShouldBeFalse();
		await navigationService.PushAsync(regionName, new TestDummyModel());
		await navigationService.PushAsync(regionName, new TestDummyModel());
		(await navigationService.GoBackAsync(regionName)).ShouldBeTrue();
	}

	[Fact]
	public async Task GoBackRestoresViewOnControl()
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		var regionControl = new FakeRegionControl(regionName, null);
		regionRegister.RegisterRegion(regionControl);

		navigationService.CanGoBack(regionName).ShouldBeFalse();
		await navigationService.PushAsync(regionName, new TestDummyModel(){Text = "a"});
		await navigationService.PushAsync(regionName, new TestDummyModel(){Text = "b"});
		(await navigationService.GoBackAsync(regionName)).ShouldBeTrue();
		// regionControl.Content.ShouldNotBeNull();
		regionControl.Content.ShouldBeAssignableTo<TestDummyModel>();
		
		((TestDummyModel)regionControl.Content!).Text.ShouldBe("a");
	}

	[Fact]
	public async Task GoBackFailsIfThereIsNoHistory()
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		regionRegister.RegisterRegion(new FakeRegionControl(regionName, null));

		navigationService.CanGoBack(regionName).ShouldBeFalse();
		await navigationService.PushAsync(regionName, new TestDummyModel());

		await Assert.ThrowsAsync<MvvmCoreException>(async () => await navigationService.GoBackAsync(regionName));
	}


	[Theory]
	[InlineData(false, false)]
	[InlineData(true, true)]
	public async Task CurrentNavigationCanAbortNavigation(bool firstModelAllowsNavigation, bool secondPushResult)
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		regionRegister.RegisterRegion(new FakeRegionControl(regionName, null));

		var firstModel = new NavigationObservable() { ResultOfNavigatedFrom = firstModelAllowsNavigation };
		var secondModel = new NavigationObservable() { };

		(await navigationService.PushAsync(regionName, firstModel)).ShouldBeTrue();
		(await navigationService.PushAsync(regionName, secondModel)).ShouldBe(secondPushResult);
	}


	[Fact]
	public async Task VerifyNavigationCallbacks()
	{
		var regionName = "TestRegion";
		var provider = GetServiceProvider();
		var navigationService = provider.GetRequiredService<INavigationService>();
		var regionRegister = provider.GetRequiredService<IRegionRegister>();
		regionRegister.RegisterRegion(new FakeRegionControl(regionName, null));

		var firstModel = new NavigationObservable() { ResultOfNavigatedFrom = true };
		var secondModel = new NavigationObservable() { };

		(await navigationService.PushAsync(regionName, firstModel)).ShouldBeTrue();
		(await navigationService.PushAsync(regionName, secondModel)).ShouldBeTrue();

		firstModel.CallCountNavigatingTo.ShouldBe(1);
		firstModel.CallCountNavigatedTo.ShouldBe(1);
		firstModel.CallCountNavigatedFrom.ShouldBe(1);
		firstModel.NextModel.ShouldBe(secondModel);

		secondModel.CallCountNavigatingTo.ShouldBe(1);
		secondModel.CallCountNavigatedTo.ShouldBe(1);
		secondModel.CallCountNavigatedFrom.ShouldBe(0);
	}
}