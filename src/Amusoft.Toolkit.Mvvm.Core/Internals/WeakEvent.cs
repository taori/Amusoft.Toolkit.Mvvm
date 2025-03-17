using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static System.Linq.Expressions.Expression;

namespace Amusoft.Toolkit.Mvvm.Core.Internals;

[ExcludeFromCodeCoverage]
internal class WeakEvent<THandler> where THandler : Delegate
{
	private readonly List<WeakDelegate<THandler>> _handles = new();

	private readonly object _lock = new();
	private THandler _invoker;

	public THandler Invoke
	{
		get
		{
			lock (_lock)
			{
				if (_invoker == null)
					return _invoker = WeakEventDelegateFactory.Create<THandler>(default(THandler), _handles);
				return _invoker;
			}
		}
	}

	public void Add(THandler? @delegate)
	{
		if (@delegate is null)
			return;
		var lockTaken = false;
		try
		{
			lockTaken = Monitor.TryEnter(_lock);
			if (lockTaken && _invoker == null)
				_invoker = WeakEventDelegateFactory.Create(@delegate, _handles);
			_handles.Insert(0, new WeakDelegate<THandler>(CompatReflectionExtensions.GetMethodInfo(@delegate), @delegate.Target));
		}
		finally
		{
			if (lockTaken)
				Monitor.Exit(_lock);
		}
	}

	public void Remove(THandler? @delegate)
	{
		if (@delegate is null)
			return;

		var index = _handles.FindIndex(del => del.IsMatch(@delegate));
		if (index >= 0)
			_handles.RemoveAt(index);
	}
}

[ExcludeFromCodeCoverage]
internal class WeakDelegate<THandler> where THandler : Delegate
{
	private readonly WeakReference<MethodInfo> _method;

	private readonly WeakReference<object> _target;

	/// <summary>
	/// Holder class for method + target information
	/// </summary>
	/// <param name="method">method</param>
	/// <param name="target">target</param>
	public WeakDelegate(MethodInfo method, object target)
	{
		_method = new WeakReference<MethodInfo>(method);
		_target = new WeakReference<object>(target);
	}

	/// <summary>
	/// Retrieves method and target information from their WeakReferences
	/// </summary>
	/// <param name="method">method</param>
	/// <param name="target">target</param>
	/// <returns></returns>
	public bool GetHandleInfo(out MethodInfo method, out object target)
	{
		target = null;
		return _method.TryGetTarget(out method) && (method.IsStatic || _target.TryGetTarget(out target));
	}

	/// <summary>
	/// Checks if the handler is equivalent to the contained WeakReferences
	/// </summary>
	/// <param name="delegate">delegate</param>
	/// <returns>true if delegate is matching</returns>
	public bool IsMatch(THandler @delegate)
	{
		if (!_method.TryGetTarget(out var method))
			return false;

		if (method.IsStatic)
		{
			return ReferenceEquals(method, CompatReflectionExtensions.GetMethodInfo(@delegate));
		}
		else
		{
			if (!_target.TryGetTarget(out var target))
				return false;

			return ReferenceEquals(target, @delegate.Target)
			       && ReferenceEquals(method, CompatReflectionExtensions.GetMethodInfo(@delegate));
		}
	}
}

[ExcludeFromCodeCoverage]
internal static class WeakEventDelegateFactory
{
	private static readonly Type[] NoTypes = [];

	public static THandler Create<THandler>(THandler @delegate, List<WeakDelegate<THandler>> handlerList) where THandler : Delegate
	{
		//	var index = handlerList.Count;
		//	while (index-- > 0)
		//	{
		//		var handle = handlerList[index];
		//		if (handle.GetHandleInfo(out var method, out object target))
		//		{
		//			if (method.IsStatic)
		//			{
		//				method.Invoke(null, parameters);
		//			}
		//			else
		//			{
		//				method.Invoke(target, parameters);
		//			}
		//		}
		//		else
		//		{
		//			handlerList.RemoveAt(index);
		//		}
		//	}

		var delegateMethodInfo = GetMethodInfoByDelegate(@delegate);
		if (!IsSupportedDelegateReturnType(delegateMethodInfo.ReturnType))
			throw new NotSupportedException($"");
		var parameters = delegateMethodInfo.GetParameters().Select(s => Parameter(s.ParameterType, s.Name)).ToArray();
		var handlerListEx = Parameter(typeof(List<WeakDelegate<THandler>>), "items");
		if (delegateMethodInfo.ReturnType == typeof(Task))
		{
			var result = Lambda<THandler>(CreateTaskedCodeBlock<THandler>(handlerListEx, parameters, handlerList, delegateMethodInfo.ReturnType), true, parameters);
			return result.Compile();
		}
		else
		{
			var result = Lambda<THandler>(CreateCodeBlock<THandler>(handlerListEx, parameters, handlerList, delegateMethodInfo.ReturnType), true, parameters);
			return result.Compile();
		}
	}

	private static bool IsSupportedDelegateReturnType(Type type)
	{
		return type == typeof(void) || type == typeof(Task);
	}

	private static MethodInfo GetMethodInfoByDelegate<THandler>(THandler @delegate) where THandler : Delegate
	{
		var methodInfo = @delegate == null
			? CompatReflectionExtensions.GetRuntimeMethods(typeof(THandler)).FirstOrDefault(d => d.Name == nameof(MethodInfo.Invoke))
			: CompatReflectionExtensions.GetMethodInfo(@delegate);
		return methodInfo;
	}

	private static Expression CreateCodeBlock<THandler>(ParameterExpression handlerListEx, ParameterExpression[] parameters, object handlerList, Type methodReturnType) where THandler : Delegate
	{
		var indexEx = Variable(typeof(int), "index");
		var exitLabel = Label(typeof(void), "exitLoop");
		var breakExpression = Break(exitLabel);
		var outVarMethod = Variable(typeof(MethodInfo), "method");
		var outVarTarget = Variable(typeof(object), "target");
		var handleEx = Variable(typeof(WeakDelegate<THandler>), "handle");
		var listIndexerPropertyInfo = GetListItemIndexerProperty<THandler>();
		var listIndexerGetMethod = listIndexerPropertyInfo.GetGetMethod();
		var methodInfoIsStaticGetterInfo = GetMethodInfoIsStaticProperty<THandler>();
		var methodInfoIsStaticGetter = methodInfoIsStaticGetterInfo.GetGetMethod();
		var handlerListCountMethod = GetListCountGetterMethod<THandler>();
		var getHandleInfoMethodInfo = GetWeakDelegateGetHandleInfoMethod<THandler>();
		var zeroExpression = Constant(0, typeof(int));
		var nullExpression = Constant(null, typeof(object));

		var blockExpression = Block(new[] { handlerListEx, indexEx, handleEx, outVarMethod, outVarTarget },
			Assign(handlerListEx, Constant(handlerList)),
			// var index = handles.Count;
			Assign(indexEx, Call(handlerListEx, handlerListCountMethod)),
			Loop(
				IfThenElse(
					GreaterThan(PostDecrementAssign(indexEx), zeroExpression),
					Block(
						// var handle = handles[index];
						Assign(handleEx, Call(handlerListEx, listIndexerGetMethod, indexEx)),
						/*
						 *	if (handle.GetHandleInfo(out var method, out object target))
						 *	{
						 *		if (method.IsStatic)
						 *		{
						 *			method.Invoke(null, parameters);
						 *		}
						 *		else
						 *		{
						 *			method.Invoke(target, parameters);
						 *		}
						 *	}
						 *	else
						 *	{
						 *		handlerList.RemoveAt(index);
						 *	}
						 */
						IfThenElse(
							Call(handleEx, getHandleInfoMethodInfo, outVarMethod, outVarTarget),
							IfThenElse(
								Call(outVarMethod, methodInfoIsStaticGetter),
								Call(outVarMethod, GetMethodInfoInvokeMethod(), new Expression[] { nullExpression, NewArrayInit(typeof(object), parameters.Select(p => Convert(p, typeof(object)))) }),
								Call(outVarMethod, GetMethodInfoInvokeMethod(), new Expression[] { outVarTarget, NewArrayInit(typeof(object), parameters.Select(p => Convert(p, typeof(object)))) })
							),
							Call(handlerListEx, GetDelegateListRemoveAtMethod<THandler>(), indexEx)
						)
					),
					breakExpression
				),
				exitLabel
			)
		);
		return blockExpression;
	}

	private static Expression CreateTaskedCodeBlock<THandler>(ParameterExpression handlerListEx, ParameterExpression[] parameters, List<WeakDelegate<THandler>> handlerList, Type methodInfoReturnType)
		where THandler : Delegate
	{
		var resultExpression = Variable(typeof(Task), "result");
		var tasksExpression = Variable(typeof(List<Task>), "tasks");
		var indexEx = Variable(typeof(int), "index");
		var outVarMethod = Variable(typeof(MethodInfo), "method");
		var outVarTarget = Variable(typeof(object), "target");
		var handleEx = Variable(typeof(WeakDelegate<THandler>), "handle");
		var listIndexerPropertyInfo = GetListItemIndexerProperty<THandler>();
		var listIndexerGetMethod = listIndexerPropertyInfo.GetGetMethod();
		var methodInfoIsStaticGetterInfo = GetMethodInfoIsStaticProperty<THandler>();
		var methodInfoIsStaticGetter = methodInfoIsStaticGetterInfo.GetGetMethod();
		var handlerListCountMethod = GetListCountGetterMethod<THandler>();
		var getHandleInfoMethodInfo = GetWeakDelegateGetHandleInfoMethod<THandler>();
		var zeroExpression = Constant(0, typeof(int));
		var nullExpression = Constant(null, typeof(object));
		var exitLabel = Label(typeof(void), "exitLoop");
		var breakExpression = Break(exitLabel);
		var invokeMethodInfo = GetMethodInfoInvokeMethod();
		var delegateListRemoveAtMethodInfo = GetDelegateListRemoveAtMethod<THandler>();
		var taskListAddMethodInfo = GetTaskListAddMethod();
		var taskWhenAllMethodInfo = GetTaskWhenAllMethod();

		var taskedCodeBlock = Block(new[] { handlerListEx, indexEx, handleEx, outVarMethod, outVarTarget, tasksExpression, resultExpression },
			Assign(handlerListEx, Constant(handlerList)),
			// var index = handles.Count;
			// var tasks = new List<Task>();
			Assign(indexEx, Call(handlerListEx, handlerListCountMethod)),
			Assign(tasksExpression, New(GetTaskListConstructorInfo())),
			Loop(
				IfThenElse(
					GreaterThan(PostDecrementAssign(indexEx), zeroExpression),
					Block(
						// while start
						// var handle = handles[index];
						Assign(handleEx, Call(handlerListEx, listIndexerGetMethod, indexEx)),
						/*
						 *	if (handle.GetHandleInfo(out var method, out object target))
						 *	{
						 *		if (method.IsStatic)
						 *		{
						 *			tasks.Add(method.Invoke(null, parameters));
						 *		}
						 *		else
						 *		{
						 *			tasks.Add(method.Invoke(target, parameters));
						 *		}
						 *	}
						 *	else
						 *	{
						 *		handlerList.RemoveAt(index);
						 *	}
						 *
						 * while end
						 * return Task.WhenAll(tasks);
						 */
						IfThenElse(
							Call(handleEx, getHandleInfoMethodInfo, outVarMethod, outVarTarget),
							IfThenElse(
								Call(outVarMethod, methodInfoIsStaticGetter),
								Call(tasksExpression,
									taskListAddMethodInfo,
									Convert(Call(outVarMethod, invokeMethodInfo, new Expression[] { nullExpression, NewArrayInit(typeof(object), parameters.Select(p => Convert(p, typeof(object)))) }),
										typeof(Task)
									)
								),
								Call(tasksExpression,
									taskListAddMethodInfo,
									Convert(Call(outVarMethod, invokeMethodInfo, new Expression[] { outVarTarget, NewArrayInit(typeof(object), parameters.Select(p => Convert(p, typeof(object)))) }),
										typeof(Task)
									)
								)
							),
							Call(handlerListEx, delegateListRemoveAtMethodInfo, indexEx)
						)
					),
					breakExpression
				),
				exitLabel
			),
			Assign(resultExpression, Call(null, taskWhenAllMethodInfo, tasksExpression))
		);
		return taskedCodeBlock;
	}

	private static ConstructorInfo? GetTaskListConstructorInfo()
	{
		return CompatReflectionExtensions.GetConstructorInfo(typeof(List<Task>), NoTypes);
	}

	private static MethodInfo? GetTaskWhenAllMethod()
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(Task), nameof(Task.WhenAll), typeof(IEnumerable<Task>));
	}

	private static MethodInfo? GetWeakDelegateGetHandleInfoMethod<THandler>() where THandler : Delegate
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(WeakDelegate<THandler>),
			nameof(WeakDelegate<THandler>.GetHandleInfo),
			new[] { typeof(MethodInfo).MakeByRefType(), typeof(object).MakeByRefType() }
		);
	}

	private static MethodInfo? GetListCountGetterMethod<THandler>() where THandler : Delegate
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(List<WeakDelegate<THandler>>), "get_Count");
	}

	private static PropertyInfo? GetMethodInfoIsStaticProperty<THandler>() where THandler : Delegate
	{
		return CompatReflectionExtensions.GetRuntimeProperty(typeof(MethodInfo), nameof(MethodInfo.IsStatic));
	}

	private static PropertyInfo? GetListItemIndexerProperty<THandler>() where THandler : Delegate
	{
		return CompatReflectionExtensions.GetRuntimeProperty(typeof(List<WeakDelegate<THandler>>), "Item");
	}

	private static MethodInfo? GetDelegateListRemoveAtMethod<THandler>() where THandler : Delegate
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(List<WeakDelegate<THandler>>), nameof(IList.RemoveAt), typeof(int));
	}

	private static MethodInfo? GetTaskListAddMethod()
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(List<Task>), nameof(IList.Add), typeof(Task));
	}

	private static MethodInfo? GetMethodInfoInvokeMethod()
	{
		return CompatReflectionExtensions.GetRuntimeMethod(typeof(MethodInfo), nameof(MethodInfo.Invoke), typeof(object), typeof(object[]));
	}
}

[ExcludeFromCodeCoverage]
internal static class CompatReflectionExtensions
{
	public static MethodInfo GetMethodInfo(Delegate source)
	{
		return RuntimeReflectionExtensions.GetMethodInfo(source);
	}

	public static ConstructorInfo? GetConstructorInfo(Type source, Type[] constructorParameterTypes)
	{
		return source.GetConstructor(constructorParameterTypes);
	}

	public static MethodInfo? GetRuntimeMethod(Type source, string name, params Type[] parameters)
	{
		return RuntimeReflectionExtensions.GetRuntimeMethod(source, name, parameters);
	}

	public static PropertyInfo? GetRuntimeProperty(Type source, string name)
	{
		return RuntimeReflectionExtensions.GetRuntimeProperty(source, name);
	}

	public static MethodInfo? GetGetMethod(this PropertyInfo source)
	{
		return source.GetMethod;
	}

	public static IEnumerable<MethodInfo> GetRuntimeMethods(Type source)
	{
		return source.GetRuntimeMethods();
	}
}