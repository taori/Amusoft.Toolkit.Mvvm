using System;
using System.Windows;
using System.Windows.Markup;

namespace Amusoft.Toolkit.Mvvm.Wpf;

internal static class DataTemplateGenerator
{
	internal static event EventHandler<string>? Generated;
	
	internal static DataTemplate CreateTemplate(Type viewModelType, Type viewType)
	{
		const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
		var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name);

		var context = new ParserContext();

		context.XamlTypeMapper = new XamlTypeMapper([]);
		context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace!, viewModelType.Assembly.FullName!);
		context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace!, viewType.Assembly.FullName!);

		context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
		context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
		context.XmlnsDictionary.Add("vm", "vm");
		context.XmlnsDictionary.Add("v", "v");

		var template = (DataTemplate)XamlReader.Parse(xaml, context);
		Generated?.Invoke(null, xaml);
		return template;
	}
}
