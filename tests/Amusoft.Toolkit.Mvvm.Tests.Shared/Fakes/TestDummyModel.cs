using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

internal class TestDummyModel
{
	public TestDummyModel()
	{
	}
	
	[RestoreProperty]
	public string? Text { get; set; }
}