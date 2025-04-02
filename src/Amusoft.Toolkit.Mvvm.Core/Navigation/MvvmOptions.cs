namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Options for the Mvvm WPF Platform
/// </summary>
public class MvvmOptions
{
	/// <summary>
	/// Configuration of view mappings
	/// </summary>
	public ViewMappings ViewMappings { get; set; } = new();
}