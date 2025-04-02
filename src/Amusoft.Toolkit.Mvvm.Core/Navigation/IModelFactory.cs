// This file is licensed to you under the MIT license.

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// <para>The ModelFactory is responsible for creating an instance for a type in generic calls.</para>
/// <para>By default, the IServiceProvider will be used to do this unless you override the IModelFactory instance</para>
/// </summary>
public interface IModelFactory
{
	/// <summary>
	/// Creates an instance of the specified type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	T Create<T>();
}