using System.Runtime.CompilerServices;
 
namespace Amusoft.Toolkit.Mvvm.Tests.Shared;

public class SharedStartup
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifyInitializer.Initialize();
	}
}