using Amusoft.Toolkit.Mvvm.Core;
using Shouldly;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests
{
    public class NamespaceConventionViewModelToViewMapperTests
    {
        [Fact]
        public Task VerifyHasMatches()
        {
            var pattern = new NamespaceConventionViewModelToViewMapper();
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);

            var results = pattern.GetResult(input);
            results.Matches.Length.ShouldBeGreaterThan(0);
            
            return Task.CompletedTask;
        }
    }
}

namespace NamespaceConventionPatternTests.Views
{
    public class TestView{} 
}
namespace NamespaceConventionPatternTests.ViewModels
{
    public class TestVM{} 
}