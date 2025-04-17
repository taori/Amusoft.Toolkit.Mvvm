using System;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// This exception is thrown if there is any issue within the Mvvm Core
/// </summary>
public class MvvmCoreException : Exception
{
	/// <summary>
	/// Constructor for unit test purposes
	/// </summary>
	public MvvmCoreException()
	{
	}

	internal MvvmCoreException(string? message) : base(message)
	{
	}
}