using System.Windows;
using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Wpf;

internal class DataTemplateResourceAppender : IDataTemplateResourceAppender
{
	private readonly IDataTemplateSource _templateSource;

	public DataTemplateResourceAppender(IDataTemplateSource templateSource)
	{
		_templateSource = templateSource;
	}

	public void AppendTo(ResourceDictionary dictionary)
	{
		foreach (var mapping in _templateSource.GetMappings())
		{
			var dataTemplate = DataTemplateGenerator.CreateTemplate(mapping.viewModel, mapping.view);
			dictionary.Add(dataTemplate.DataTemplateKey, dataTemplate);
		}
	}
}

/// <summary>
/// Appends data templates to the a given resource dictionary
/// </summary>
public interface IDataTemplateResourceAppender
{
	/// <summary>
	/// Appends the configured data templates to the given dictionary
	/// </summary>
	/// <param name="dictionary"></param>
	void AppendTo(ResourceDictionary dictionary);
}