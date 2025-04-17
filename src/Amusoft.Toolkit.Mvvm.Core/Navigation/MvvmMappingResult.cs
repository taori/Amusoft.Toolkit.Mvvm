using System;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Results of the mapping process
/// </summary>
public class MvvmMappingResult
{
	/// <summary>
	/// </summary>
	/// <param name="matches"></param>
	/// <param name="mismatchedViews"></param>
	/// <param name="mismatchedViewModels"></param>
	public MvvmMappingResult((Type viewModelType, Type viewType)[] matches, Type[] mismatchedViews, Type[] mismatchedViewModels)
	{
		Matches = matches;
		MismatchedViews = mismatchedViews;
		MismatchedViewModels = mismatchedViewModels;
	}

	/// <summary>
	/// Matches built from the input
	/// </summary>
	public (Type viewModelType, Type viewType)[] Matches { get; private set; }
	
	/// <summary>
	/// Mismatched entries from the view types
	/// </summary>
	public Type[] MismatchedViews { get; private set; }
	
	/// <summary>
	/// Mismatched entries from the viewmodel types
	/// </summary>
	public Type[] MismatchedViewModels { get; private set; }
}