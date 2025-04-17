using System;
using System.Windows;

namespace Amusoft.Toolkit.Mvvm.Wpf;

internal class TemplateGenerator : ITemplateGenerator
{
	public DataTemplate CreateTemplate(Type viewModel, Type view)
	{
		return DataTemplateGenerator.CreateTemplate(viewModel, view);
	}
}