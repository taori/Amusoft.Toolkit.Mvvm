using Amusoft.Toolkit.Mvvm.Core;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

namespace Amusoft.Toolkit.Mvvm.Core.UnitTests
{
    public class ConstructorGuards 
    {
        [Fact]
        public void NavigationService()
        {
	        var p1 = new Mock<IModelFactory>();
	        var p4 = new Mock<IRegionRegister>();
	        var p5 = new Mock<IRegionModelHistory>();
	        var noThrow = new NavigationService(NullLogger<NavigationService>.Instance, p1.Object, p4.Object, p5.Object);

	        Assert.Throws<ArgumentNullException>(() => new NavigationService(null!, p1.Object, p4.Object, p5.Object));
	        Assert.Throws<ArgumentNullException>(() => new NavigationService(NullLogger<NavigationService>.Instance, null!, p4.Object, p5.Object));
	        Assert.Throws<ArgumentNullException>(() => new NavigationService(NullLogger<NavigationService>.Instance, p1.Object, null!, p5.Object));
	        Assert.Throws<ArgumentNullException>(() => new NavigationService(NullLogger<NavigationService>.Instance, p1.Object, p4.Object, null!));
        }
        
        [Fact]
        public void ViewMappingEngine()
        {
	        var p1 = new Mock<IEnumerable<IViewModelToViewMapper>>();
	        var p2 = new Mock<ICompositeMappingTypeSourceContainer>();
	        var noThrow = new ViewMappingEngine(NullLogger<ViewMappingEngine>.Instance, [], p2.Object);

	        Assert.Throws<ArgumentNullException>(() => new ViewMappingEngine(null!, [], p2.Object));
	        Assert.Throws<ArgumentNullException>(() => new ViewMappingEngine(NullLogger<ViewMappingEngine>.Instance, null!, p2.Object));
	        Assert.Throws<ArgumentNullException>(() => new ViewMappingEngine(NullLogger<ViewMappingEngine>.Instance, [], null!));
        }
        
        [Fact]
        public void NavigationModelFactory()
        {
	        var p1 = new Mock<IEnumerable<IRestoreStrategyProvider>>();
	        var p2 = new Mock<IActivator>();

	        Assert.Throws<ArgumentNullException>(() => new NavigationModelFactory(null!, p2.Object));
	        Assert.Throws<ArgumentNullException>(() => new NavigationModelFactory(p1.Object, null!));
        }
    }
}
