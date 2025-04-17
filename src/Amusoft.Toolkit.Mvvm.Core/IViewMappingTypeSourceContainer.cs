// This file is licensed to you under the MIT license.

using System.Collections.Generic;

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IViewMappingTypeSourceContainer
{
	List<IMappingTypeSource> Registrations { get; }
}