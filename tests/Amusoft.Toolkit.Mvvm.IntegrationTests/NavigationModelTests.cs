// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

using JetBrains.dotMemoryUnit;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

[assembly: DotMemoryUnit(CollectAllocations = true, FailIfRunWithoutSupport = false)]
namespace Amusoft.Toolkit.Mvvm.IntegrationTests;

public class NavigationModelTests : IntegrationTestBase
{
	private readonly ITestOutputHelper _testOutputHelper;

	public NavigationModelTests(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
		DotMemoryUnitTestOutput.SetOutputMethod(testOutputHelper.WriteLine);
	}

	[Fact]
	public void VerifyModelNoRestoreStrategy()
	{
		NavigationModel<ReconstructionModel>? navModel = null;
		var isolator = new Action(() =>
		{
			navModel = new NavigationModel<ReconstructionModel>(new ReconstructionModel() { Id = Guid.Empty }, []);
		});
		isolator();
		
		GC.Collect();
		navModel.ShouldNotBeNull();
		var model = navModel.GetModel();
		model.ShouldBeNull();
	}

	[Theory]
	[InlineData("1766136B-27F8-478E-9A9F-312136CFF6EF")]
	[InlineData("4894D6CC-897C-42BB-8618-9758043B2D76")]
	public void VerifyNavigationModelAttributeRestore(string id)
	{
		var navModelId = Guid.Parse(id);
		NavigationModel<ReconstructionModel>? navModel = null;
		var sp = GetServiceProvider(collection => collection.AddTransient<ReconstructionModel>());
		var isolator = new Action(() =>
		{
			var strategies = new RestorePropertyRestoreStrategyProvider(sp).GetStrategies<ReconstructionModel>().ToArray();
			navModel = new NavigationModel<ReconstructionModel>(new ReconstructionModel() { Id = navModelId }, strategies);
		});
		isolator();
		
		GC.Collect();
		navModel.ShouldNotBeNull();
		var model = navModel.GetModel();
		model.ShouldNotBeNull();
		model.ShouldBeOfType<ReconstructionModel>();
		((ReconstructionModel)model).Id.ShouldBe(navModelId);
	}

	[Fact]
	public void TargetModelCanBeDisposed()
	{
		var baseline = dotMemory.Check(memory => memory
			.GetObjects(d => 
				d.Type.Is<ReconstructionModel>()).ObjectsCount.ShouldBe(0)
		);
		MemoryCheckPoint postCreation;
	
		var isolator = new Action(async() =>
		{
			var regionName = "TestRegion";
			var sp = GetServiceProvider();
			var ns = sp.GetRequiredService<INavigationService>();
			var rs = sp.GetRequiredService<IRegionRegister>();
			var control = new FakeRegionControl(regionName, null);
			rs.RegisterRegion(control);
	
			await ns.PushAsync(regionName, new ReconstructionModel());
	
			postCreation = dotMemory.Check(memory => memory
				.GetTrafficFrom(baseline)
				.Where(d => d.Type.Is<ReconstructionModel>()).AllocatedMemory.ObjectsCount.ShouldBe(1));
	
			await ns.PushAsync(regionName, new object());
		});
	
		isolator();
		
		GC.Collect();
	
		dotMemory.Check(memory => memory
			.GetTrafficFrom(postCreation)
			.Where(d => d.Type.Is<ReconstructionModel>())
			.AllocatedMemory.ObjectsCount.ShouldBe(0)
		);
	}
	
	[Fact]
	public async Task TargetModelCanBeRestored()
	{
		var baseline = dotMemory.Check(memory => memory
			.GetObjects(d => 
				d.Type.Is<ReconstructionModel>()).ObjectsCount.ShouldBe(0)
		);
		MemoryCheckPoint postCreation;
	
		var regionName = "TestRegion";
		var control = new FakeRegionControl(regionName, null);
		var newGuid = Guid.NewGuid();
		INavigationService? ns = null;
		var isolator = new Action(async() =>
		{
			var sp = GetServiceProvider(setup: collection => collection.AddTransient<ReconstructionModel>());
			ns = sp.GetRequiredService<INavigationService>();
			var rs = sp.GetRequiredService<IRegionRegister>();
			rs.RegisterRegion(control);
	
			await ns.PushAsync(regionName, new ReconstructionModel(){Id = newGuid});
	
			postCreation = dotMemory.Check(memory => memory
				.GetTrafficFrom(baseline)
				.Where(d => d.Type.Is<ReconstructionModel>()).AllocatedMemory.ObjectsCount.ShouldBe(1));
	
			await ns.PushAsync(regionName, new object());
		});
	
		isolator();
		
		GC.Collect();
		
		await ns!.GoBackAsync(regionName);
	
		if (control.Content is not ReconstructionModel reconstructionModel)
		{
			Assert.Fail($"Content should be of type ReconstructionModel but it isn't - {control.Content?.GetType().FullName ?? "Unknown"}.");
			return;
		}
		
		reconstructionModel.Id.ShouldBe(newGuid);
	}

	public class ReconstructionModel : IDisposable
	{
		private byte[] bytes = new byte[100_000];
		
		[RestoreProperty]
		public Guid Id { get; set; }

		[RestoreProperty]
		public string? SomeText { get; set; }

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}