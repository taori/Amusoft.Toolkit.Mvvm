using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using Amusoft.Toolkit.Mvvm.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests
{
    public class NamespaceConventionViewModelToViewMapperTests : IntegrationTestBase
    {
        [Fact]
        public Task VerifyHasMatches()
        {
	        var sp = GetServiceProvider(setup: collection =>
		        {
		        }
	        );
	        var mapper = sp.GetRequiredService<IEnumerable<IViewModelToViewMapper>>()
		        .OfType<NamespaceConventionViewModelToViewMapper>()
		        .First();
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);
        
            var results = mapper.GetResult(input);
            results.Matches.Length.ShouldBeGreaterThan(0);
            
            return Task.CompletedTask;
        }
        
        [Fact]
        public Task NamespaceConventionReplacable()
        {
	        var sp = GetServiceProvider(setup: collection =>
		        {
			        collection.Configure<NamespaceConventionOptions>(options => { });
		        }
	        );
	        var mapper = sp.GetRequiredService<IEnumerable<IViewModelToViewMapper>>()
		        .OfType<NamespaceConventionViewModelToViewMapper>()
		        .First();
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);

            var results = mapper.GetResult(input);
            results.Matches.Length.ShouldBeGreaterThan(0);
            
            return Task.CompletedTask;
        }
        
        [Theory]
        [InlineData("VM$", 1)]
        [InlineData("V$", 0)]
        public Task VerifyCustomViewModelTruncateEnd(string vmPattern, int expectedMatches)
        {
	        var sp = GetServiceProvider(setup: collection =>
		        {
			        collection.Configure<NamespaceConventionOptions>(options =>
			        {
				        options.ViewModelTruncateEndPattern = new Regex(vmPattern);
			        });
		        }
	        );
	        var mapper = sp.GetRequiredService<IEnumerable<IViewModelToViewMapper>>()
		        .OfType<NamespaceConventionViewModelToViewMapper>()
		        .First();
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);
        
            var results = mapper.GetResult(input);
            results.Matches.Length.ShouldBe(expectedMatches);
            
            return Task.CompletedTask;
        }
        
        [Theory]
        [InlineData("View$", 1)]
        [InlineData("V$", 0)]
        public Task VerifyCustomViewTruncateEnd(string viewPattern, int expectedMatches)
        {
	        var sp = GetServiceProvider(setup: collection =>
		        {
			        collection.Configure<NamespaceConventionOptions>(options =>
			        {
				        options.ViewTruncateEndPattern = new Regex(viewPattern);
			        });
		        }
	        );
	        var mapper = sp.GetRequiredService<IEnumerable<IViewModelToViewMapper>>()
		        .OfType<NamespaceConventionViewModelToViewMapper>()
		        .First();
            var input = new AssemblyMappingTypeSource()
                .WithAssembly(typeof(NamespaceConventionViewModelToViewMapperTests).Assembly)
                .WithViewFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false)
                .WithViewModelFilter(d => d.FullName?.StartsWith("NamespaceConventionPatternTests") ?? false);
        
            var results = mapper.GetResult(input);
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