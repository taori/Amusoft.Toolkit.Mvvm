using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Amusoft.Toolkit.Mvvm.Core;
using Shouldly;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests
{
    public class NamespaceConventionViewModelToViewMapperTests : IntegrationTestBase
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
        
        [Theory]
        [InlineData("VM$", 1)]
        [InlineData("V$", 0)]
        public Task VerifyCustomViewModelTruncateEnd(string vmPattern, int expectedMatches)
        {
            var pattern = new NamespaceConventionViewModelToViewMapper();
            pattern.ViewModelTruncateEndPattern = new Regex(vmPattern);
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);

            var results = pattern.GetResult(input);
            results.Matches.Length.ShouldBe(expectedMatches);
            
            return Task.CompletedTask;
        }
        
        [Theory]
        [InlineData("View$", 1)]
        [InlineData("V$", 0)]
        public Task VerifyCustomViewTruncateEnd(string viewPattern, int expectedMatches)
        {
            var pattern = new NamespaceConventionViewModelToViewMapper();
            pattern.ViewTruncateEndPattern = new Regex(viewPattern);
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);

            var results = pattern.GetResult(input);
            results.Matches.Length.ShouldBe(expectedMatches);
            
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