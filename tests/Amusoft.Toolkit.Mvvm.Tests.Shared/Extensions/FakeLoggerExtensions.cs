// This file is licensed to you under the MIT license.

using System.Text;

using Microsoft.Extensions.Logging.Testing;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Extensions;

public static class FakeLoggerExtensions
{
	public static string GetFullLoggerText(this FakeLogCollector source, Func<FakeLogRecord, string>? formatter = default)
	{
		var sb = new StringBuilder();
		var snapshot = source.GetSnapshot();
		formatter ??= record => $"{record.Level} - {record.Message}";
		foreach (var record in snapshot)
		{
			sb.AppendLine(formatter(record));
		}
		return sb.ToString();
	}
}