// ReSharper disable once CheckNamespace
namespace System.Collections.Generic;

internal static class StackExtensions
{
	public static bool TryPop<T>(this Stack<T> stack, out T? value)
	{
		var r = stack.Peek();
		if (r is not null)
		{
			value = stack.Pop();
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}
	
	public static bool TryPeek<T>(this Stack<T> stack, out T? value)
	{
		value = stack.Peek();
		return value is not null;
	}
}