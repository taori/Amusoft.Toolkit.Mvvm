using System;
using System.Windows;

namespace Amusoft.Toolkit.Mvvm.Wpf;

/// <summary>
/// The <see cref="ITemplateGenerator"/> interface is responsible for generating a template based on the type of the viewmodel and the view that should represent it
/// </summary>
public interface ITemplateGenerator
{
	/// <summary>
	/// Creates a template
	/// </summary>
	/// <param name="viewModel"></param>
	/// <param name="view"></param>
	/// <returns></returns>
	DataTemplate CreateTemplate(Type viewModel, Type view);
}