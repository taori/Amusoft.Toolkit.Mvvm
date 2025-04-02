using System.Collections.Generic;

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRestoreStrategyProvider
{
	IEnumerable<IRestoreStrategy<T>> GetStrategies<T>();
}
