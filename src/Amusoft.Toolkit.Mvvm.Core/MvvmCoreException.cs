using System;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// This exception is thrown if there is any issue within the Mvvm Core
/// </summary>
public class MvvmCoreException : Exception
{
	internal MvvmCoreException(string? message) : base(message)
	{
	}

	internal MvvmCoreException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}