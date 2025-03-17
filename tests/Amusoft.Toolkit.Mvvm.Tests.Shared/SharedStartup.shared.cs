using System.Runtime.CompilerServices;
using System.Threading.Tasks;
 
namespace Amusoft.Toolkit.Mvvm.Tests.Shared;

public class SharedStartup
{
	[ModuleInitializer]
	public static void Initialize()
	{
		VerifyInitializer.Initialize();
	}
}