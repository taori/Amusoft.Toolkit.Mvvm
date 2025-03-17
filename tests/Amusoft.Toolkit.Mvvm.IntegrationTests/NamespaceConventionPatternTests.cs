using System.Data.Common;
using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Tests.Shared;
using Shouldly;
using Xunit;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests
{
    public class NamespaceConventionPatternTests
    {
        [Fact]
        public Task VerifyHasMatches()
        {
            var pattern = new NamespaceConventionPattern();
            var input = new MvvmMappingInput()
                .WithAssembly(typeof(NamespaceConventionPatternTests).Assembly)
                .WithViewFilter(d => d.FullName.StartsWith("NamespaceConventionPatternTests"))
                .WithViewModelFilter(d => d.FullName.StartsWith("NamespaceConventionPatternTests"));

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