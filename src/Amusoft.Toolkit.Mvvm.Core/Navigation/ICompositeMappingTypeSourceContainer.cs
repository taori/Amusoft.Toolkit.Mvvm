// This file is licensed to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface ICompositeMappingTypeSourceContainer
{
	IMappingTypeSource[] GetSources();
}