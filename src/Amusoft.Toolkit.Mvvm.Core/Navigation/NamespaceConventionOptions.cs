// This file is licensed to you under the MIT license.

using System;
using System.Text.RegularExpressions;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// 
/// </summary>
public class NamespaceConventionOptions
{
	
	/// <summary>
	/// Wildcard to match the start of namespace mapping e.g. "ViewModels"
	/// <para>The default is: .+\.ViewModels\.(?&lt;match>.+)</para>
	/// </summary>
	public Regex ViewModelPattern { get; set; } = new(@".+\.ViewModels\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// Removes the end parts from a full name
	/// <para>The default is: (?:ViewModel|Model|VM)</para>
	/// </summary>
	public Regex ViewModelTruncateEndPattern { get; set; } = new("(?:ViewModel|Model|VM)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// <para>Wildcard to match the start of namespace mapping e.g. "Views"</para>
	/// <para>The default is: .+\.Views\.(?&lt;match>.+)</para>
	/// </summary>
	public Regex ViewPattern { get; set; } = new(@".+\.Views\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// Removes the end parts from a full name
	/// </summary>
	public Regex ViewTruncateEndPattern { get; set; } = new("(?:Page|View)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
}