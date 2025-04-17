// This file is licensed to you under the MIT license.

using System;
using System.Windows;

using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Wpf;

/// <inheritdoc cref="IViewMapper"/>
public class ViewMapper : IViewMapper
{
	private readonly IViewMappingEngine _viewMappingEngine;
	private readonly ITemplateGenerator _templateGenerator;

	/// <summary>
	/// </summary>
	/// <param name="viewMappingEngine"></param>
	/// <param name="templateGenerator"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public ViewMapper(IViewMappingEngine viewMappingEngine, ITemplateGenerator templateGenerator)
	{
		_viewMappingEngine = viewMappingEngine ?? throw new ArgumentNullException(nameof(viewMappingEngine));
		_templateGenerator = templateGenerator ?? throw new ArgumentNullException(nameof(templateGenerator));
	}

	/// <inheritdoc cref="IViewMapper.Map"/>
	public void Map(ResourceDictionary dictionary)
	{
		foreach (var mapping in _viewMappingEngine.GetMappings())
		{
			var dataTemplate = _templateGenerator.CreateTemplate(mapping.viewModel, mapping.view);
			dictionary.Add(dataTemplate.DataTemplateKey!, dataTemplate);
		}
	}
}