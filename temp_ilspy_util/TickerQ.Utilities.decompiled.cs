using System;
using System.Buffers;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NCrontab;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.DashboardDtos;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Entities.BaseEntity;
using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Exceptions;
using TickerQ.Utilities.Instrumentation;
using TickerQ.Utilities.Interfaces;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: InternalsVisibleTo("TickerQ")]
[assembly: InternalsVisibleTo("TickerQ.EntityFrameworkCore")]
[assembly: InternalsVisibleTo("TickerQ.Dashboard")]
[assembly: InternalsVisibleTo("TickerQ.Tests")]
[assembly: InternalsVisibleTo("TickerQ.SDK")]
[assembly: InternalsVisibleTo("TickerQ.RemoteExecutor")]
[assembly: InternalsVisibleTo("TickerQ.Instrumentation.OpenTelemetry")]
[assembly: InternalsVisibleTo("TickerQ.Caching.StackExchangeRedis")]
[assembly: InternalsVisibleTo("TickerQ.EntityFrameworkCore.Tests")]
[assembly: InternalsVisibleTo("TickerQ.Caching.StackExchangeRedis.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: TargetFramework(".NETCoreApp,Version=v10.0", FrameworkDisplayName = ".NET 10.0")]
[assembly: AssemblyMetadata("IsTrimmable", "True")]
[assembly: AssemblyMetadata("IsAotCompatible", "True")]
[assembly: AssemblyCompany("Arcenox LLC")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyCopyright("Copyright © 2025 Arcenox LLC")]
[assembly: AssemblyDescription("Simple utilities for queuing and executing cron/time-based jobs in the background.")]
[assembly: AssemblyFileVersion("10.4.0.0")]
[assembly: AssemblyInformationalVersion("10.4.0+c6ed1e7daa90ab3f4c65b40319a153126a910093")]
[assembly: AssemblyProduct("TickerQ.Utilities")]
[assembly: AssemblyTitle("TickerQ.Utilities")]
[assembly: AssemblyMetadata("RepositoryUrl", "https://github.com/arcenox-co/TickerQ")]
[assembly: AssemblyVersion("10.4.0.0")]
[module: RefSafetyRules(11)]
namespace System.Text.RegularExpressions.Generated
{
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.21808")]
	internal sealed class <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__ReplaceRegex_0 : Regex
	{
		private sealed class RunnerFactory : RegexRunnerFactory
		{
			private sealed class Runner : RegexRunner
			{
				protected override void Scan(ReadOnlySpan<char> inputSpan)
				{
					while (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan) && runtextpos != inputSpan.Length)
					{
						runtextpos++;
						if (<RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
					}
				}

				private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
				{
					int num = runtextpos;
					if ((uint)num < (uint)inputSpan.Length)
					{
						int num2 = inputSpan.Slice(num).IndexOfAny(<RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__Utilities.s_whitespace);
						if (num2 >= 0)
						{
							runtextpos = num + num2;
							return true;
						}
					}
					runtextpos = inputSpan.Length;
					return false;
				}

				private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
				{
					int num = runtextpos;
					int start = num;
					ReadOnlySpan<char> readOnlySpan = inputSpan.Slice(num);
					int i;
					for (i = 0; (uint)i < (uint)readOnlySpan.Length && char.IsWhiteSpace(readOnlySpan[i]); i++)
					{
					}
					if (i == 0)
					{
						return false;
					}
					readOnlySpan = readOnlySpan.Slice(i);
					Capture(0, start, runtextpos = num + i);
					return true;
				}
			}

			protected override RegexRunner CreateInstance()
			{
				return new Runner();
			}
		}

		internal static readonly <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__ReplaceRegex_0 Instance = new <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__ReplaceRegex_0();

		private <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__ReplaceRegex_0()
		{
			pattern = "\\s+";
			roptions = RegexOptions.None;
			Regex.ValidateMatchTimeout(<RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__Utilities.s_defaultTimeout);
			internalMatchTimeout = <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__Utilities.s_defaultTimeout;
			factory = new RunnerFactory();
			capsize = 1;
		}
	}
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.21808")]
	internal static class <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__Utilities
	{
		internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

		internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

		internal static readonly SearchValues<char> s_whitespace = SearchValues.Create("\t\n\v\f\r \u0085\u00a0\u1680\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u2028\u2029\u202f\u205f\u3000");
	}
}
namespace TickerQ.Utilities
{
	public readonly struct CronExpression : IEquatable<CronExpression>
	{
		public string Value { get; }

		private CronExpression(string value)
		{
			Value = value;
		}

		public static CronExpression Parse(string expression)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				throw new ArgumentException("Cron expression cannot be null or empty.", "expression");
			}
			if (expression.StartsWith("%") && expression.EndsWith("%"))
			{
				return new CronExpression(expression);
			}
			string text = NormalizeToSixPart(expression);
			if (CronScheduleCache.Get(text) == null)
			{
				throw new ArgumentException("Invalid cron expression: '" + expression + "'.", "expression");
			}
			return new CronExpression(text);
		}

		public static bool TryParse(string expression, out CronExpression result)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				result = default;
				return false;
			}
			if (expression.StartsWith("%") && expression.EndsWith("%"))
			{
				result = new CronExpression(expression);
				return true;
			}
			string text = NormalizeToSixPart(expression);
			if (CronScheduleCache.Get(text) != null)
			{
				result = new CronExpression(text);
				return true;
			}
			result = default;
			return false;
		}

		private static string NormalizeToSixPart(string expression)
		{
			if (expression.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Length != 5)
			{
				return expression.Trim();
			}
			return "0 " + expression.Trim();
		}

		public static implicit operator CronExpression(string expression)
		{
			return Parse(expression);
		}

		public static implicit operator string(CronExpression cron)
		{
			return cron.Value;
		}

		public override string ToString()
		{
			return Value;
		}

		public bool Equals(CronExpression other)
		{
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (obj is CronExpression other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}

		public static bool operator ==(CronExpression left, CronExpression right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CronExpression left, CronExpression right)
		{
			return !left.Equals(right);
		}
	}
	internal static class CronScheduleCache
	{
		private static readonly ConcurrentDictionary<string, CrontabSchedule> Cache = new ConcurrentDictionary<string, CrontabSchedule>(StringComparer.Ordinal);

		private static readonly ParseOptions Opts = new ParseOptions
		{
			IncludingSeconds = true
		};

		public static TimeZoneInfo TimeZoneInfo { get; internal set; } = TimeZoneInfo.Local;

		private static string Normalize(string expr)
		{
			ArgumentNullException.ThrowIfNull(expr, "expr");
			return ReplaceRegex().Replace(expr.Trim(), " ");
		}

		public static CrontabSchedule Get(string expression)
		{
			string key = Normalize(expression);
			return Cache.GetOrAdd(key, (string exp) => CrontabSchedule.TryParse(exp, Opts));
		}

		public static DateTime? GetNextOccurrenceOrDefault(string expression, DateTime dateTime)
		{
			CrontabSchedule val = Get(expression);
			if (val == null)
			{
				return null;
			}
			DateTime dateTime2 = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo);
			DateTime nextOccurrence = val.GetNextOccurrence(dateTime2);
			try
			{
				return TimeZoneInfo.ConvertTimeToUtc(nextOccurrence, TimeZoneInfo);
			}
			catch (ArgumentException)
			{
				DateTime dateTime3 = nextOccurrence;
				for (int i = 0; i < 24; i++)
				{
					dateTime3 = dateTime3.AddHours(1.0);
					try
					{
						DateTime dateTime4 = TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(dateTime3, TimeZoneInfo), TimeZoneInfo);
						return TimeZoneInfo.ConvertTimeToUtc(val.GetNextOccurrence(dateTime4), TimeZoneInfo);
					}
					catch (ArgumentException)
					{
					}
				}
				return null;
			}
		}

		public static bool Invalidate(string expression)
		{
			CrontabSchedule value;
			return Cache.TryRemove(Normalize(expression), out value);
		}

		[GeneratedRegex("\\s+")]
		[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.21808")]
		private static Regex ReplaceRegex()
		{
			return <RegexGenerator_g>F9655617704BD24A9A0A2A15083887759B8A6C7BEC4090EBB0DA50278C1F8A6E2__ReplaceRegex_0.Instance;
		}

		static CronScheduleCache()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected Obj, but got Unknown
		}
	}
	public static class TickerCancellationTokenManager
	{
		private static readonly ConcurrentDictionary<Guid, TickerCancellationTokenDetails> TickerCancellationTokens = new ConcurrentDictionary<Guid, TickerCancellationTokenDetails>();

		private static readonly ConcurrentDictionary<Guid, ConcurrentHashSet<Guid>> ParentIdIndex = new ConcurrentDictionary<Guid, ConcurrentHashSet<Guid>>();

		internal static void AddTickerCancellationToken(CancellationTokenSource cancellationSource, InternalFunctionContext context, bool isDue)
		{
			TickerCancellationTokenDetails value = new TickerCancellationTokenDetails
			{
				FunctionName = context.FunctionName,
				Type = context.Type,
				CancellationSource = cancellationSource,
				IsDue = isDue,
				ParentId = (context.ParentId ?? Guid.Empty)
			};
			TickerCancellationTokens.TryAdd(context.TickerId, value);
			if (context.ParentId.HasValue && context.ParentId.Value != Guid.Empty)
			{
				ParentIdIndex.AddOrUpdate(context.ParentId.Value, (Guid key) =>
				{
					ConcurrentHashSet<Guid> concurrentHashSet = new ConcurrentHashSet<Guid>();
					concurrentHashSet.Add(context.TickerId);
					return concurrentHashSet;
				}, (Guid key, ConcurrentHashSet<Guid> existing) =>
				{
					existing.Add(context.TickerId);
					return existing;
				});
			}
		}

		internal static bool RemoveTickerCancellationToken(Guid tickerId)
		{
			bool flag = TickerCancellationTokens.TryRemove(tickerId, out var value);
			if (flag && value != null)
			{
				try
				{
					value.CancellationSource?.Dispose();
				}
				catch
				{
				}
				if (value.ParentId != Guid.Empty)
				{
					RemoveFromParentIndex(value.ParentId, tickerId);
				}
			}
			return flag;
		}

		internal static void CleanUpTickerCancellationTokens()
		{
			foreach (KeyValuePair<Guid, TickerCancellationTokenDetails> tickerCancellationToken in TickerCancellationTokens)
			{
				try
				{
					tickerCancellationToken.Value.CancellationSource?.Dispose();
				}
				catch
				{
				}
			}
			TickerCancellationTokens.Clear();
			foreach (KeyValuePair<Guid, ConcurrentHashSet<Guid>> item in ParentIdIndex)
			{
				try
				{
					item.Value?.Dispose();
				}
				catch
				{
				}
			}
			ParentIdIndex.Clear();
		}

		public static bool RequestTickerCancellationById(Guid tickerId)
		{
			if (!TickerCancellationTokens.TryGetValue(tickerId, out var value))
			{
				return false;
			}
			try
			{
				value.CancellationSource?.Cancel();
			}
			catch (ObjectDisposedException)
			{
			}
			if (TickerCancellationTokens.TryRemove(tickerId, out var value2))
			{
				try
				{
					value2.CancellationSource?.Dispose();
				}
				catch
				{
				}
				if (value2.ParentId != Guid.Empty)
				{
					RemoveFromParentIndex(value2.ParentId, tickerId);
				}
			}
			return true;
		}

		private static void RemoveFromParentIndex(Guid parentId, Guid tickerId)
		{
			if (ParentIdIndex.TryGetValue(parentId, out var value))
			{
				value.Remove(tickerId);
				if (value.IsEmpty)
				{
					((ICollection<KeyValuePair<Guid, ConcurrentHashSet<Guid>>>)ParentIdIndex).Remove(new KeyValuePair<Guid, ConcurrentHashSet<Guid>>(parentId, value));
				}
			}
		}

		public static bool IsParentRunning(Guid parentId)
		{
			return ParentIdIndex.ContainsKey(parentId);
		}

		public static bool IsParentRunningExcludingSelf(Guid parentId, Guid excludeTickerId)
		{
			if (!ParentIdIndex.TryGetValue(parentId, out var value))
			{
				return false;
			}
			return value.HasOtherItemsBesides(excludeTickerId);
		}
	}
	public class TickerCancellationTokenDetails
	{
		public string FunctionName { get; set; }

		public TickerType Type { get; set; }

		public bool IsDue { get; set; }

		public CancellationTokenSource CancellationSource { get; set; }

		public Guid ParentId { get; set; }
	}
	public class ConcurrentHashSet<T> : IDisposable
	{
		private readonly HashSet<T> _set = new HashSet<T>();

		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public bool IsEmpty
		{
			get
			{
				_lock.EnterReadLock();
				try
				{
					return _set.Count == 0;
				}
				finally
				{
					_lock.ExitReadLock();
				}
			}
		}

		public bool Add(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				return _set.Add(item);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public bool Remove(T item)
		{
			_lock.EnterWriteLock();
			try
			{
				return _set.Remove(item);
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		public bool HasOtherItemsBesides(T excludeItem)
		{
			_lock.EnterReadLock();
			try
			{
				if (_set.Count == 0)
				{
					return false;
				}
				if (_set.Count == 1)
				{
					return !_set.Contains(excludeItem);
				}
				return true;
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public void Dispose()
		{
			_lock?.Dispose();
		}
	}
	internal interface ITickerOptionsSeeding
	{
		bool SeedDefinedCronTickers { get; }

		Func<IServiceProvider, Task> TimeSeederAction { get; }

		Func<IServiceProvider, Task> CronSeederAction { get; }
	}
	internal class TickerExecutionContext
	{
		private long _nextOccurrenceTicks;

		internal volatile InternalFunctionContext[] Functions = Array.Empty<InternalFunctionContext>();

		internal Action<IServiceProvider> ExternalProviderApplicationAction { get; set; }

		internal Action<object> DashboardApplicationAction { get; set; }

		public Action<object, CoreNotifyActionType> NotifyCoreAction { get; set; }

		public string LastHostExceptionMessage { get; set; }

		internal ITickerOptionsSeeding OptionsSeeding { get; set; }

		public void SetNextPlannedOccurrence(DateTime? dt)
		{
			Interlocked.Exchange(ref _nextOccurrenceTicks, dt?.Ticks ?? (-1));
		}

		public DateTime? GetNextPlannedOccurrence()
		{
			long num = Interlocked.Read(in _nextOccurrenceTicks);
			if (num >= 0)
			{
				return new DateTime(num, DateTimeKind.Utc);
			}
			return null;
		}

		public void SetFunctions(ReadOnlySpan<InternalFunctionContext> functions)
		{
			InternalFunctionContext[] array = new InternalFunctionContext[functions.Length];
			functions.CopyTo(array.AsSpan());
			CacheFunctionReferences(array.AsSpan());
			Functions = array;
		}

		private static void CacheFunctionReferences(Span<InternalFunctionContext> functions)
		{
			for (int i = 0; i < functions.Length; i++)
			{
				ref InternalFunctionContext reference = ref functions[i];
				if (TickerFunctionProvider.TickerFunctions.TryGetValue(reference.FunctionName, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
				{
					reference.CachedDelegate = value.Item3;
					reference.CachedPriority = value.Item2;
					reference.CachedMaxConcurrency = value.Item4;
				}
				List<InternalFunctionContext> timeTickerChildren = reference.TimeTickerChildren;
				if (timeTickerChildren != null && timeTickerChildren.Count > 0)
				{
					CacheFunctionReferences(CollectionsMarshal.AsSpan(reference.TimeTickerChildren));
				}
			}
		}
	}
	public sealed class TickerFunctionBuilder
	{
		internal string FunctionName { get; }

		internal TickerFunctionBuilder(string functionName)
		{
			FunctionName = functionName;
		}

		public TickerFunctionBuilder WithCron(CronExpression cronExpression)
		{
			TickerFunctionProvider.Configure(FunctionName, cronExpression.Value);
			return this;
		}

		public TickerFunctionBuilder WithMaxConcurrency(int maxConcurrency)
		{
			string functionName = FunctionName;
			int? maxConcurrency2 = maxConcurrency;
			TickerFunctionProvider.Configure(functionName, null, null, maxConcurrency2);
			return this;
		}

		public TickerFunctionBuilder WithPriority(TickerTaskPriority priority)
		{
			TickerFunctionProvider.Configure(FunctionName, null, priority);
			return this;
		}
	}
	public delegate Task TickerFunctionDelegate(CancellationToken cancellationToken, IServiceProvider serviceProvider, TickerFunctionContext context);
	public static class TickerFunctionProvider
	{
		private static readonly object _buildLock = new object();

		private static Action<Dictionary<string, (string, Type)>> _requestTypeRegistrations;

		private static Action<Dictionary<string, (string RequestType, string RequestExampleJson)>> _requestInfoRegistrations;

		private static Action<Dictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)>> _functionRegistrations;

		private static readonly Dictionary<Type, string> _typeMappings = new Dictionary<Type, string>();

		public static FrozenDictionary<string, (string, Type)> TickerFunctionRequestTypes = FrozenDictionary<string, (string, Type)>.Empty;

		public static FrozenDictionary<string, (string RequestType, string RequestExampleJson)> TickerFunctionRequestInfos = FrozenDictionary<string, (string, string)>.Empty;

		public static FrozenDictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> TickerFunctions = FrozenDictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>.Empty;

		public static bool IsBuilt { get; private set; }

		public static Func<string, string> FunctionNodeResolver { get; set; }

		public static string ResolveNodeName(string functionName)
		{
			if (string.IsNullOrWhiteSpace(functionName))
			{
				return null;
			}
			Func<string, string> functionNodeResolver = FunctionNodeResolver;
			if (functionNodeResolver == null)
			{
				return null;
			}
			string text = functionNodeResolver(functionName);
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return null;
		}

		public static void RegisterTypeMapping(Type type, string functionName)
		{
			_typeMappings[type] = functionName;
		}

		public static string GetFunctionName<T>()
		{
			if (!_typeMappings.TryGetValue(typeof(T), out var value))
			{
				return typeof(T).Name;
			}
			return value;
		}

		public static void ReplaceFunctions(IDictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> functions)
		{
			if (functions == null)
			{
				throw new ArgumentNullException("functions");
			}
			lock (_buildLock)
			{
				TickerFunctions = functions.ToFrozenDictionary();
				IsBuilt = true;
			}
		}

		public static void ReplaceRequestInfo(IDictionary<string, (string RequestType, string RequestExampleJson)> infos)
		{
			if (infos == null)
			{
				throw new ArgumentNullException("infos");
			}
			lock (_buildLock)
			{
				TickerFunctionRequestInfos = infos.ToFrozenDictionary();
			}
		}

		public static void MergeRemoteFunctions(IDictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> remoteFunctions, Func<string, bool> isCurrentlyRemote)
		{
			if (remoteFunctions == null)
			{
				throw new ArgumentNullException("remoteFunctions");
			}
			if (isCurrentlyRemote == null)
			{
				throw new ArgumentNullException("isCurrentlyRemote");
			}
			lock (_buildLock)
			{
				Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> dictionary = new Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>(TickerFunctions.Count + remoteFunctions.Count);
				string key;
				(string, TickerTaskPriority, TickerFunctionDelegate, int) value;
				foreach (KeyValuePair<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> tickerFunction in TickerFunctions)
				{
					tickerFunction.Deconstruct(out key, out value);
					string text = key;
					(string, TickerTaskPriority, TickerFunctionDelegate, int) value2 = value;
					if (!isCurrentlyRemote(text))
					{
						dictionary[text] = value2;
					}
				}
				foreach (KeyValuePair<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> remoteFunction in remoteFunctions)
				{
					remoteFunction.Deconstruct(out key, out value);
					string key2 = key;
					(string, TickerTaskPriority, TickerFunctionDelegate, int) value3 = value;
					dictionary[key2] = value3;
				}
				TickerFunctions = dictionary.ToFrozenDictionary();
				IsBuilt = true;
			}
		}

		public static void MergeRemoteRequestInfo(IDictionary<string, (string RequestType, string RequestExampleJson)> remoteInfos, Func<string, bool> isCurrentlyRemote)
		{
			if (remoteInfos == null)
			{
				throw new ArgumentNullException("remoteInfos");
			}
			if (isCurrentlyRemote == null)
			{
				throw new ArgumentNullException("isCurrentlyRemote");
			}
			lock (_buildLock)
			{
				Dictionary<string, (string, string)> dictionary = new Dictionary<string, (string, string)>(TickerFunctionRequestInfos.Count + remoteInfos.Count);
				string key;
				(string, string) value;
				foreach (KeyValuePair<string, (string, string)> tickerFunctionRequestInfo in TickerFunctionRequestInfos)
				{
					tickerFunctionRequestInfo.Deconstruct(out key, out value);
					string text = key;
					(string, string) value2 = value;
					if (!isCurrentlyRemote(text))
					{
						dictionary[text] = value2;
					}
				}
				foreach (KeyValuePair<string, (string, string)> remoteInfo in remoteInfos)
				{
					remoteInfo.Deconstruct(out key, out value);
					string key2 = key;
					(string, string) value3 = value;
					dictionary[key2] = value3;
				}
				TickerFunctionRequestInfos = dictionary.ToFrozenDictionary();
			}
		}

		public static void RegisterFunctions(IDictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> functions)
		{
			if (functions == null)
			{
				throw new ArgumentNullException("functions");
			}
			if (functions.Count == 0)
			{
				return;
			}
			lock (_buildLock)
			{
				_functionRegistrations = (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)Delegate.Combine(_functionRegistrations, (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)((Dictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> dict) =>
				{
					foreach (var (key, value) in functions)
					{
						dict.TryAdd(key, value);
					}
				}));
			}
		}

		public static void RegisterFunctions(IDictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> functions, int _)
		{
			RegisterFunctions(functions);
		}

		public static void RegisterRequestType(IDictionary<string, (string, Type)> requestTypes)
		{
			if (requestTypes == null)
			{
				throw new ArgumentNullException("requestTypes");
			}
			if (requestTypes.Count == 0)
			{
				return;
			}
			lock (_buildLock)
			{
				_requestTypeRegistrations = (Action<Dictionary<string, (string, Type)>>)Delegate.Combine(_requestTypeRegistrations, (Action<Dictionary<string, (string, Type)>>)((Dictionary<string, (string, Type)> dict) =>
				{
					foreach (var (key, value) in requestTypes)
					{
						dict.TryAdd(key, value);
					}
				}));
			}
		}

		public static void RegisterRequestType(IDictionary<string, (string, Type)> requestTypes, int _)
		{
			RegisterRequestType(requestTypes);
		}

		public static void RegisterRequestInfo(IDictionary<string, (string RequestType, string RequestExampleJson)> requestInfos)
		{
			if (requestInfos == null)
			{
				throw new ArgumentNullException("requestInfos");
			}
			if (requestInfos.Count == 0)
			{
				return;
			}
			lock (_buildLock)
			{
				_requestInfoRegistrations = (Action<Dictionary<string, (string, string)>>)Delegate.Combine(_requestInfoRegistrations, (Action<Dictionary<string, (string, string)>>)((Dictionary<string, (string RequestType, string RequestExampleJson)> dict) =>
				{
					foreach (var (key, value) in requestInfos)
					{
						dict.TryAdd(key, value);
					}
				}));
			}
		}

		public static void Configure(string functionName, string cronExpression = null, TickerTaskPriority? priority = null, int? maxConcurrency = null)
		{
			_functionRegistrations = (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)Delegate.Combine(_functionRegistrations, (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)((Dictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> dict) =>
			{
				if (dict.TryGetValue(functionName, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
				{
					string key = functionName;
					string text = cronExpression;
					if (text == null)
					{
						(text, _, _, _) = value;
					}
					dict[key] = (text, priority ?? value.Item2, value.Item3, maxConcurrency ?? value.Item4);
				}
			}));
		}

		internal static void UpdateCronExpressionsFromIConfiguration(IConfiguration configuration)
		{
			lock (_buildLock)
			{
				_functionRegistrations = (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)Delegate.Combine(_functionRegistrations, (Action<Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>>)((Dictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> dict) =>
				{
					foreach (var (key, tuple2) in dict)
					{
						if (tuple2.Item1.StartsWith('%'))
						{
							string key2 = tuple2.Item1.Trim('%');
							string text2 = configuration[key2];
							if (!string.IsNullOrEmpty(text2))
							{
								dict[key] = (text2, tuple2.Item2, tuple2.Item3, tuple2.Item4);
							}
						}
					}
				}));
			}
		}

		public static void Build()
		{
			lock (_buildLock)
			{
				if (_functionRegistrations != null)
				{
					Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)> dictionary = new Dictionary<string, (string, TickerTaskPriority, TickerFunctionDelegate, int)>(TickerFunctions);
					_functionRegistrations(dictionary);
					TickerFunctions = dictionary.ToFrozenDictionary();
					_functionRegistrations = null;
				}
				if (_requestTypeRegistrations != null)
				{
					Dictionary<string, (string, Type)> dictionary2 = new Dictionary<string, (string, Type)>(TickerFunctionRequestTypes);
					_requestTypeRegistrations(dictionary2);
					TickerFunctionRequestTypes = dictionary2.ToFrozenDictionary();
					_requestTypeRegistrations = null;
				}
				if (_requestInfoRegistrations != null)
				{
					Dictionary<string, (string, string)> dictionary3 = new Dictionary<string, (string, string)>(TickerFunctionRequestInfos);
					_requestInfoRegistrations(dictionary3);
					TickerFunctionRequestInfos = dictionary3.ToFrozenDictionary();
					_requestInfoRegistrations = null;
				}
				IsBuilt = true;
			}
		}
	}
	public static class TickerRequestProvider
	{
		public static async Task<T> GetRequestAsync<T>(TickerFunctionContext context, CancellationToken cancellationToken)
		{
			try
			{
				return await context.ServiceScope.ServiceProvider.GetService<IInternalTickerManager>().GetRequestAsync<T>(context.Id, context.Type, cancellationToken);
			}
			catch (Exception exception)
			{
				context.ServiceScope.ServiceProvider.GetService<ITickerQInstrumentation>().LogRequestDeserializationFailure(typeof(T).FullName, context.FunctionName, context.Id, context.Type, exception);
			}
			return default;
		}

		public static async Task<TickerFunctionContext<T>> ToGenericContextAsync<T>(TickerFunctionContext context, CancellationToken cancellationToken)
		{
			return new TickerFunctionContext<T>(context, await GetRequestAsync<T>(context, cancellationToken));
		}

		public static async Task<T> GetRequestAsync<T>(TickerFunctionContext context, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
		{
			try
			{
				return await context.ServiceScope.ServiceProvider.GetService<IInternalTickerManager>().GetRequestAsync(context.Id, context.Type, typeInfo, cancellationToken);
			}
			catch (Exception exception)
			{
				context.ServiceScope.ServiceProvider.GetService<ITickerQInstrumentation>().LogRequestDeserializationFailure(typeof(T).FullName, context.FunctionName, context.Id, context.Type, exception);
			}
			return default;
		}

		public static async Task<TickerFunctionContext<T>> ToGenericContextAsync<T>(TickerFunctionContext context, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
		{
			return new TickerFunctionContext<T>(context, await GetRequestAsync(context, typeInfo, cancellationToken));
		}
	}
	public readonly struct TickerFunctionRef
	{
		public string Name { get; }

		public TickerFunctionRef(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		public static implicit operator string(TickerFunctionRef r)
		{
			return r.Name;
		}
	}
	public readonly struct TickerFunctionRef<TRequest>
	{
		public string Name { get; }

		public TickerFunctionRef(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		public static implicit operator string(TickerFunctionRef<TRequest> r)
		{
			return r.Name;
		}
	}
	public static class TickerHelper
	{
		private static readonly byte[] GZipSignature = new byte[4] { 31, 139, 8, 0 };

		public static JsonSerializerOptions RequestJsonSerializerOptions { get; set; } = new JsonSerializerOptions();

		public static bool UseGZipCompression { get; set; } = false;

		public static byte[] CreateTickerRequest<T>(T data)
		{
			if (data is byte[] array)
			{
				if (UseGZipCompression && array.Length >= GZipSignature.Length && array.TakeLast(GZipSignature.Length).SequenceEqual(GZipSignature))
				{
					return array;
				}
				if (!UseGZipCompression)
				{
					return array;
				}
			}
			byte[] array2 = ((data is byte[] array3) ? array3 : JsonSerializer.SerializeToUtf8Bytes(data, RequestJsonSerializerOptions));
			if (!UseGZipCompression)
			{
				return array2;
			}
			Span<byte> span;
			Span<byte> span2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
				{
					gZipStream.Write(array2);
				}
				span = memoryStream.GetBuffer().AsSpan();
				span2 = span.Slice(0, (int)memoryStream.Length);
			}
			byte[] array4 = new byte[span2.Length + GZipSignature.Length];
			Span<byte> destination = array4.AsSpan();
			span2.CopyTo(destination);
			span = GZipSignature.AsSpan();
			int length = span2.Length;
			span.CopyTo(destination.Slice(length, destination.Length - length));
			return array4;
		}

		public static T ReadTickerRequest<T>(byte[] gzipBytes)
		{
			if (gzipBytes == null || gzipBytes.Length == 0)
			{
				return default;
			}
			string text = ReadTickerRequestAsString(gzipBytes);
			if (string.IsNullOrWhiteSpace(text))
			{
				return default;
			}
			if (RequestJsonSerializerOptions?.TypeInfoResolver?.GetTypeInfo(typeof(T), RequestJsonSerializerOptions) is JsonTypeInfo<T> jsonTypeInfo)
			{
				return JsonSerializer.Deserialize(text, jsonTypeInfo);
			}
			return JsonSerializer.Deserialize<T>(text, RequestJsonSerializerOptions);
		}

		public static T ReadTickerRequest<T>(byte[] gzipBytes, JsonTypeInfo<T> typeInfo)
		{
			if (gzipBytes == null || gzipBytes.Length == 0)
			{
				return default;
			}
			string text = ReadTickerRequestAsString(gzipBytes);
			if (string.IsNullOrWhiteSpace(text))
			{
				return default;
			}
			return JsonSerializer.Deserialize(text, typeInfo);
		}

		public static byte[] CreateTickerRequest<T>(T data, JsonTypeInfo<T> typeInfo)
		{
			byte[] array = JsonSerializer.SerializeToUtf8Bytes(data, typeInfo);
			if (!UseGZipCompression)
			{
				return array;
			}
			Span<byte> span;
			Span<byte> span2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
				{
					gZipStream.Write(array);
				}
				span = memoryStream.GetBuffer().AsSpan();
				span2 = span.Slice(0, (int)memoryStream.Length);
			}
			byte[] array2 = new byte[span2.Length + GZipSignature.Length];
			Span<byte> destination = array2.AsSpan();
			span2.CopyTo(destination);
			span = GZipSignature.AsSpan();
			int length = span2.Length;
			span.CopyTo(destination.Slice(length, destination.Length - length));
			return array2;
		}

		public static string ReadTickerRequestAsString(byte[] gzipBytes)
		{
			if (!UseGZipCompression)
			{
				return Encoding.UTF8.GetString(gzipBytes);
			}
			if (!gzipBytes.TakeLast(GZipSignature.Length).SequenceEqual(GZipSignature))
			{
				throw new Exception("The bytes are not GZip compressed.");
			}
			using MemoryStream stream = new MemoryStream(gzipBytes.Take(gzipBytes.Length - GZipSignature.Length).ToArray());
			using GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
			using StreamReader streamReader = new StreamReader(stream2);
			return streamReader.ReadToEnd();
		}
	}
	public class TickerOptionsBuilder<TTimeTicker, TCronTicker> : ITickerOptionsSeeding where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		private readonly TickerExecutionContext _tickerExecutionContext;

		private readonly SchedulerOptionsBuilder _schedulerOptions;

		internal bool RequestGZipCompressionEnabled { get; set; }

		internal bool SeedDefinedCronTickers { get; set; } = true;

		internal bool RegisterBackgroundServices { get; set; } = true;

		internal Func<IServiceProvider, Task> TimeSeederAction { get; set; }

		internal Func<IServiceProvider, Task> CronSeederAction { get; set; }

		bool ITickerOptionsSeeding.SeedDefinedCronTickers => SeedDefinedCronTickers;

		Func<IServiceProvider, Task> ITickerOptionsSeeding.TimeSeederAction => TimeSeederAction;

		Func<IServiceProvider, Task> ITickerOptionsSeeding.CronSeederAction => CronSeederAction;

		internal Action<IServiceCollection> ExternalProviderConfigServiceAction { get; set; }

		internal Action<IServiceCollection> DashboardServiceAction { get; set; }

		internal Type TickerExceptionHandlerType { get; private set; }

		public TimeSpan MinPollingInterval
		{
			get
			{
				return _schedulerOptions.MinPollingInterval;
			}
			set
			{
				_schedulerOptions.MinPollingInterval = value;
			}
		}

		internal JsonSerializerOptions RequestJsonSerializerOptions { get; set; }

		internal TickerOptionsBuilder(TickerExecutionContext tickerExecutionContext, SchedulerOptionsBuilder schedulerOptions)
		{
			_tickerExecutionContext = tickerExecutionContext;
			_schedulerOptions = schedulerOptions;
			tickerExecutionContext.OptionsSeeding = this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> ConfigureScheduler(Action<SchedulerOptionsBuilder> schedulerOptionsBuilder)
		{
			schedulerOptionsBuilder?.Invoke(_schedulerOptions);
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> WithJsonContext(IJsonTypeInfoResolver context)
		{
			RequestJsonSerializerOptions = new JsonSerializerOptions
			{
				TypeInfoResolver = context
			};
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> ConfigureRequestJsonOptions(Action<JsonSerializerOptions> configure)
		{
			if (RequestJsonSerializerOptions == null)
			{
				JsonSerializerOptions jsonSerializerOptions = (RequestJsonSerializerOptions = new JsonSerializerOptions());
			}
			configure?.Invoke(RequestJsonSerializerOptions);
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> UseGZipCompression()
		{
			RequestGZipCompressionEnabled = true;
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> SkipStaleCronOccurrencesOnStartup(TimeSpan? threshold = null)
		{
			_schedulerOptions.StaleCronOccurrenceThreshold = threshold ?? TimeSpan.FromSeconds(5L);
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> IgnoreSeedDefinedCronTickers()
		{
			SeedDefinedCronTickers = false;
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> DisableBackgroundServices()
		{
			RegisterBackgroundServices = false;
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> UseTickerSeeder(Func<ITimeTickerManager<TTimeTicker>, Task> timeSeeder)
		{
			if (timeSeeder == null)
			{
				return this;
			}
			TimeSeederAction = async (IServiceProvider sp) =>
			{
				ITimeTickerManager<TTimeTicker> requiredService = sp.GetRequiredService<ITimeTickerManager<TTimeTicker>>();
				await timeSeeder(requiredService).ConfigureAwait(continueOnCapturedContext: false);
			};
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> UseTickerSeeder(Func<ICronTickerManager<TCronTicker>, Task> cronSeeder)
		{
			if (cronSeeder == null)
			{
				return this;
			}
			CronSeederAction = async (IServiceProvider sp) =>
			{
				ICronTickerManager<TCronTicker> requiredService = sp.GetRequiredService<ICronTickerManager<TCronTicker>>();
				await cronSeeder(requiredService).ConfigureAwait(continueOnCapturedContext: false);
			};
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> UseTickerSeeder(Func<ITimeTickerManager<TTimeTicker>, Task> timeSeeder, Func<ICronTickerManager<TCronTicker>, Task> cronSeeder)
		{
			UseTickerSeeder(timeSeeder);
			UseTickerSeeder(cronSeeder);
			return this;
		}

		public TickerOptionsBuilder<TTimeTicker, TCronTicker> SetExceptionHandler<THandler>() where THandler : ITickerExceptionHandler
		{
			TickerExceptionHandlerType = typeof(THandler);
			return this;
		}

		internal void UseExternalProviderApplication(Action<IServiceProvider> action)
		{
			_tickerExecutionContext.ExternalProviderApplicationAction = action;
		}

		internal void UseDashboardApplication(Action<object> action)
		{
			_tickerExecutionContext.DashboardApplicationAction = action;
		}
	}
	public class SchedulerOptionsBuilder
	{
		public TimeZoneInfo SchedulerTimeZone = TimeZoneInfo.Local;

		public string NodeIdentifier { get; set; } = Environment.MachineName;

		public int MaxConcurrency { get; set; } = Environment.ProcessorCount;

		public TimeSpan IdleWorkerTimeOut { get; set; } = TimeSpan.FromMinutes(1L);

		public TimeSpan FallbackIntervalChecker { get; set; } = TimeSpan.FromSeconds(30L);

		public TimeSpan MinPollingInterval { get; set; } = TimeSpan.FromSeconds(1L);

		public TimeSpan StaleCronOccurrenceThreshold { get; set; } = TimeSpan.Zero;
	}
}
namespace TickerQ.Utilities.Temps
{
	internal class NoOpTickerQDispatcher : ITickerQDispatcher
	{
		public bool IsEnabled => false;

		public Task DispatchAsync(InternalFunctionContext[] contexts, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}

		public Task DispatchAsync(InternalFunctionContext context, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}
	}
	internal class NoOpTickerQHostScheduler : ITickerQHostScheduler
	{
		public bool IsRunning => false;

		public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.CompletedTask;
		}

		public void RestartIfNeeded(DateTime? dateTime)
		{
		}

		public void Restart()
		{
		}
	}
	internal class NoOpTickerQNotificationHubSender : ITickerQNotificationHubSender
	{
		public Task AddCronTickerNotifyAsync(object cronTicker)
		{
			return Task.CompletedTask;
		}

		public Task UpdateCronTickerNotifyAsync(object cronTicker)
		{
			return Task.CompletedTask;
		}

		public Task RemoveCronTickerNotifyAsync(Guid id)
		{
			return Task.CompletedTask;
		}

		public Task AddTimeTickerNotifyAsync(Guid id)
		{
			return Task.CompletedTask;
		}

		public Task AddTimeTickersBatchNotifyAsync()
		{
			return Task.CompletedTask;
		}

		public Task UpdateTimeTickerNotifyAsync(object timeTicker)
		{
			return Task.CompletedTask;
		}

		public Task RemoveTimeTickerNotifyAsync(Guid id)
		{
			return Task.CompletedTask;
		}

		public void UpdateActiveThreads(string activeThreads)
		{
		}

		public void UpdateNextOccurrence(DateTime? nextOccurrence)
		{
		}

		public void UpdateHostStatus(bool active)
		{
		}

		public void UpdateHostException(string exceptionMessage)
		{
		}

		public Task UpdateNodeHeartBeatAsync(JsonElement nodeHeartBeat)
		{
			return Task.CompletedTask;
		}

		public Task AddCronOccurrenceAsync(Guid groupId, object occurrence)
		{
			return Task.CompletedTask;
		}

		public Task UpdateCronOccurrenceAsync(Guid groupId, object occurrence)
		{
			return Task.CompletedTask;
		}

		public Task UpdateTimeTickerFromInternalFunctionContext<TTimeTickerEntity>(InternalFunctionContext internalFunctionContext) where TTimeTickerEntity : TimeTickerEntity<TTimeTickerEntity>, new()
		{
			return Task.CompletedTask;
		}

		public Task UpdateCronOccurrenceFromInternalFunctionContext<TCronTickerEntity>(InternalFunctionContext internalFunctionContext) where TCronTickerEntity : CronTickerEntity, new()
		{
			return Task.CompletedTask;
		}

		public Task CanceledTickerNotifyAsync(Guid id)
		{
			return Task.CompletedTask;
		}
	}
	internal class NoOpTickerQRedisContext : ITickerQRedisContext
	{
		public IDistributedCache DistributedCache => null;

		public bool HasRedisConnection => false;

		public Task<TResult[]> GetOrSetArrayAsync<TResult>(string cacheKey, Func<CancellationToken, Task<TResult[]>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class
		{
			return factory(cancellationToken);
		}

		public Task<string[]> GetDeadNodesAsync()
		{
			return Task.FromResult(Array.Empty<string>());
		}

		public Task NotifyNodeAliveAsync()
		{
			return Task.CompletedTask;
		}
	}
}
namespace TickerQ.Utilities.Models
{
	public class InternalFunctionContext
	{
		private static readonly ConcurrentDictionary<string, (Action<InternalFunctionContext, object> Setter, string Name)> SetterCache = new ConcurrentDictionary<string, (Action<InternalFunctionContext, object>, string)>();

		public HashSet<string> ParametersToUpdate { get; set; } = new HashSet<string>();

		[JsonIgnore]
		public TickerFunctionDelegate CachedDelegate { get; set; }

		public TickerTaskPriority CachedPriority { get; set; }

		public int CachedMaxConcurrency { get; set; }

		public string FunctionName { get; set; }

		public Guid TickerId { get; set; }

		public Guid? ParentId { get; set; }

		public TickerType Type { get; set; }

		public int Retries { get; set; }

		public int RetryCount { get; set; }

		public TickerStatus Status { get; set; }

		public long ElapsedTime { get; set; }

		public string ExceptionDetails { get; set; }

		public DateTime ExecutedAt { get; set; }

		public int[] RetryIntervals { get; set; }

		public bool ReleaseLock { get; set; }

		public DateTime ExecutionTime { get; set; }

		public RunCondition RunCondition { get; set; }

		public List<InternalFunctionContext> TimeTickerChildren { get; set; } = new List<InternalFunctionContext>();

		[DynamicDependency(DynamicallyAccessedMemberTypes.PublicProperties, typeof(InternalFunctionContext))]
		public InternalFunctionContext SetProperty<T>(Expression<Func<InternalFunctionContext, T>> property, T value)
		{
			if (ParametersToUpdate == null)
			{
				HashSet<string> hashSet = (ParametersToUpdate = new HashSet<string>());
			}
			if (property.Body is MemberExpression memberExpression)
			{
				MemberInfo member = memberExpression.Member;
				PropertyInfo prop = member as PropertyInfo;
				if ((object)prop != null)
				{
					(Action<InternalFunctionContext, object>, string) orAdd = SetterCache.GetOrAdd(prop.Name, (string _) =>
					{
						ParameterExpression parameterExpression = Expression.Parameter(typeof(InternalFunctionContext), "obj");
						ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "val");
						return (Setter: Expression.Lambda<Action<InternalFunctionContext, object>>(Expression.Assign(Expression.Property(parameterExpression, prop), Expression.Convert(parameterExpression2, prop.PropertyType)), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile(), Name: prop.Name);
					});
					orAdd.Item1(this, value);
					ParametersToUpdate.Add(orAdd.Item2);
					return this;
				}
			}
			throw new ArgumentException("Expression must point to a property", "property");
		}

		public InternalFunctionContext ResetUpdateProps()
		{
			ParametersToUpdate.Clear();
			return this;
		}

		public HashSet<string> GetPropsToUpdate()
		{
			return ParametersToUpdate;
		}
	}
	public class InternalManagerContext(Guid id)
	{
		public Guid Id { get; set; } = id;

		public string FunctionName { get; set; }

		public string Expression { get; set; }

		public int Retries { get; set; }

		public int[] RetryIntervals { get; set; }

		public NextCronOccurrence NextCronOccurrence { get; set; }
	}
	public class NextCronOccurrence(Guid id, DateTime createdAt)
	{
		public Guid Id { get; set; } = id;

		public DateTime CreatedAt { get; set; }
	}
	public class PaginationResult<T>
	{
		public IEnumerable<T> Items { get; set; }

		public int TotalCount { get; set; }

		public int PageNumber { get; set; }

		public int PageSize { get; set; }

		public int TotalPages => (int)Math.Ceiling((double)TotalCount / (double)PageSize);

		public bool HasPreviousPage => PageNumber > 1;

		public bool HasNextPage => PageNumber < TotalPages;

		public int FirstItemIndex => (PageNumber - 1) * PageSize + 1;

		public int LastItemIndex => Math.Min(PageNumber * PageSize, TotalCount);

		public PaginationResult()
		{
			Items = new List<T>();
		}

		public PaginationResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
		{
			Items = items ?? new List<T>();
			TotalCount = totalCount;
			PageNumber = pageNumber;
			PageSize = pageSize;
		}
	}
	public class TickerResult<TEntity> where TEntity : class
	{
		public readonly bool IsSucceeded;

		public readonly int AffectedRows;

		public readonly TEntity Result;

		public readonly Exception Exception;

		internal TickerResult(Exception exception)
			: this(false)
		{
			Exception = exception;
		}

		internal TickerResult(TEntity result)
			: this(true)
		{
			Result = result;
		}

		internal TickerResult(int affectedRows)
			: this(true)
		{
			AffectedRows = affectedRows;
		}

		private TickerResult(bool isSucceeded)
		{
			IsSucceeded = isSucceeded;
		}

		internal TickerResult(TEntity result, int affectedRows)
			: this(true)
		{
			Result = result;
			AffectedRows = affectedRows;
		}
	}
}
namespace TickerQ.Utilities.Managers
{
	public class FluentChainTickerBuilder<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
	{
		public class FirstChildBuilder
		{
			private readonly FluentChainTickerBuilder<TTimeTicker> _mainBuilder;

			private readonly TTimeTicker _child;

			private readonly int _childIndex;

			internal FirstChildBuilder(FluentChainTickerBuilder<TTimeTicker> mainBuilder, TTimeTicker child, int childIndex)
			{
				_mainBuilder = mainBuilder;
				_child = child;
				_childIndex = childIndex;
			}

			public FirstChildBuilder WithFirstGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][0])
				{
					throw new InvalidOperationException("First grandchild of first child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][0] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FirstChildBuilder WithSecondGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][1])
				{
					throw new InvalidOperationException("Second grandchild of first child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][1] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FirstChildBuilder WithThirdGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][2])
				{
					throw new InvalidOperationException("Third grandchild of first child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][2] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FirstChildBuilder WithFourthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][3])
				{
					throw new InvalidOperationException("Fourth grandchild of first child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][3] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FirstChildBuilder WithFifthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][4])
				{
					throw new InvalidOperationException("Fifth grandchild of first child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][4] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public SecondChildBuilder WithSecondChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithSecondChild(configure);
			}

			public ThirdChildBuilder WithThirdChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithThirdChild(configure);
			}

			public FourthChildBuilder WithFourthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFourthChild(configure);
			}

			public FifthChildBuilder WithFifthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFifthChild(configure);
			}

			public TTimeTicker Build()
			{
				return _mainBuilder.Build();
			}

			public static implicit operator TTimeTicker(FirstChildBuilder builder)
			{
				return builder.Build();
			}
		}

		public class SecondChildBuilder
		{
			private readonly FluentChainTickerBuilder<TTimeTicker> _mainBuilder;

			private readonly TTimeTicker _child;

			private readonly int _childIndex;

			internal SecondChildBuilder(FluentChainTickerBuilder<TTimeTicker> mainBuilder, TTimeTicker child, int childIndex)
			{
				_mainBuilder = mainBuilder;
				_child = child;
				_childIndex = childIndex;
			}

			public SecondChildBuilder WithFirstGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][0])
				{
					throw new InvalidOperationException("First grandchild of second child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][0] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public SecondChildBuilder WithSecondGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][1])
				{
					throw new InvalidOperationException("Second grandchild of second child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][1] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public SecondChildBuilder WithThirdGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][2])
				{
					throw new InvalidOperationException("Third grandchild of second child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][2] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public SecondChildBuilder WithFourthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][3])
				{
					throw new InvalidOperationException("Fourth grandchild of second child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][3] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public SecondChildBuilder WithFifthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][4])
				{
					throw new InvalidOperationException("Fifth grandchild of second child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][4] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public ThirdChildBuilder WithThirdChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithThirdChild(configure);
			}

			public FourthChildBuilder WithFourthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFourthChild(configure);
			}

			public FifthChildBuilder WithFifthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFifthChild(configure);
			}

			public TTimeTicker Build()
			{
				return _mainBuilder.Build();
			}

			public static implicit operator TTimeTicker(SecondChildBuilder builder)
			{
				return builder.Build();
			}
		}

		public class ThirdChildBuilder
		{
			private readonly FluentChainTickerBuilder<TTimeTicker> _mainBuilder;

			private readonly TTimeTicker _child;

			private readonly int _childIndex;

			internal ThirdChildBuilder(FluentChainTickerBuilder<TTimeTicker> mainBuilder, TTimeTicker child, int childIndex)
			{
				_mainBuilder = mainBuilder;
				_child = child;
				_childIndex = childIndex;
			}

			public ThirdChildBuilder WithFirstGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][0])
				{
					throw new InvalidOperationException("First grandchild of third child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][0] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public ThirdChildBuilder WithSecondGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][1])
				{
					throw new InvalidOperationException("Second grandchild of third child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][1] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public ThirdChildBuilder WithThirdGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][2])
				{
					throw new InvalidOperationException("Third grandchild of third child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][2] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public ThirdChildBuilder WithFourthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][3])
				{
					throw new InvalidOperationException("Fourth grandchild of third child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][3] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public ThirdChildBuilder WithFifthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][4])
				{
					throw new InvalidOperationException("Fifth grandchild of third child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][4] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FourthChildBuilder WithFourthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFourthChild(configure);
			}

			public FifthChildBuilder WithFifthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFifthChild(configure);
			}

			public TTimeTicker Build()
			{
				return _mainBuilder.Build();
			}

			public static implicit operator TTimeTicker(ThirdChildBuilder builder)
			{
				return builder.Build();
			}
		}

		public class FourthChildBuilder
		{
			private readonly FluentChainTickerBuilder<TTimeTicker> _mainBuilder;

			private readonly TTimeTicker _child;

			private readonly int _childIndex;

			internal FourthChildBuilder(FluentChainTickerBuilder<TTimeTicker> mainBuilder, TTimeTicker child, int childIndex)
			{
				_mainBuilder = mainBuilder;
				_child = child;
				_childIndex = childIndex;
			}

			public FourthChildBuilder WithFirstGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][0])
				{
					throw new InvalidOperationException("First grandchild of fourth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][0] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FourthChildBuilder WithSecondGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][1])
				{
					throw new InvalidOperationException("Second grandchild of fourth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][1] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FourthChildBuilder WithThirdGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][2])
				{
					throw new InvalidOperationException("Third grandchild of fourth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][2] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FourthChildBuilder WithFourthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][3])
				{
					throw new InvalidOperationException("Fourth grandchild of fourth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][3] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FourthChildBuilder WithFifthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][4])
				{
					throw new InvalidOperationException("Fifth grandchild of fourth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][4] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FifthChildBuilder WithFifthChild(Action<ChildBuilder<TTimeTicker>> configure)
			{
				return _mainBuilder.WithFifthChild(configure);
			}

			public TTimeTicker Build()
			{
				return _mainBuilder.Build();
			}

			public static implicit operator TTimeTicker(FourthChildBuilder builder)
			{
				return builder.Build();
			}
		}

		public class FifthChildBuilder
		{
			private readonly FluentChainTickerBuilder<TTimeTicker> _mainBuilder;

			private readonly TTimeTicker _child;

			private readonly int _childIndex;

			internal FifthChildBuilder(FluentChainTickerBuilder<TTimeTicker> mainBuilder, TTimeTicker child, int childIndex)
			{
				_mainBuilder = mainBuilder;
				_child = child;
				_childIndex = childIndex;
			}

			public FifthChildBuilder WithFirstGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][0])
				{
					throw new InvalidOperationException("First grandchild of fifth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][0] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FifthChildBuilder WithSecondGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][1])
				{
					throw new InvalidOperationException("Second grandchild of fifth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][1] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FifthChildBuilder WithThirdGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][2])
				{
					throw new InvalidOperationException("Third grandchild of fifth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][2] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FifthChildBuilder WithFourthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][3])
				{
					throw new InvalidOperationException("Fourth grandchild of fifth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][3] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public FifthChildBuilder WithFifthGrandChild(Action<GrandChildBuilder<TTimeTicker>> configure)
			{
				if (_mainBuilder._grandChildrenUsed[_childIndex][4])
				{
					throw new InvalidOperationException("Fifth grandchild of fifth child has already been configured");
				}
				_mainBuilder._grandChildrenUsed[_childIndex][4] = true;
				TTimeTicker val = _mainBuilder.CreateGrandChild(_child);
				GrandChildBuilder<TTimeTicker> obj = new GrandChildBuilder<TTimeTicker>(val);
				configure(obj);
				_child.Children.Add(val);
				return this;
			}

			public TTimeTicker Build()
			{
				return _mainBuilder.Build();
			}

			public static implicit operator TTimeTicker(FifthChildBuilder builder)
			{
				return builder.Build();
			}
		}

		private readonly TTimeTicker _rootTicker;

		private readonly bool[] _childrenUsed = new bool[5];

		private readonly bool[][] _grandChildrenUsed = new bool[5][];

		private FluentChainTickerBuilder()
		{
			_rootTicker = new TTimeTicker
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				Children = new List<TTimeTicker>()
			};
			for (int i = 0; i < 5; i++)
			{
				_grandChildrenUsed[i] = new bool[5];
			}
		}

		public static FluentChainTickerBuilder<TTimeTicker> BeginWith(Action<ParentBuilder<TTimeTicker>> configure)
		{
			FluentChainTickerBuilder<TTimeTicker> fluentChainTickerBuilder = new FluentChainTickerBuilder<TTimeTicker>();
			ParentBuilder<TTimeTicker> obj = new ParentBuilder<TTimeTicker>(fluentChainTickerBuilder._rootTicker);
			configure(obj);
			return fluentChainTickerBuilder;
		}

		public FirstChildBuilder WithFirstChild(Action<ChildBuilder<TTimeTicker>> configure)
		{
			if (_childrenUsed[0])
			{
				throw new InvalidOperationException("First child has already been configured");
			}
			_childrenUsed[0] = true;
			TTimeTicker val = CreateChild();
			ChildBuilder<TTimeTicker> obj = new ChildBuilder<TTimeTicker>(val);
			configure(obj);
			_rootTicker.Children.Add(val);
			return new FirstChildBuilder(this, val, 0);
		}

		public SecondChildBuilder WithSecondChild(Action<ChildBuilder<TTimeTicker>> configure)
		{
			if (_childrenUsed[1])
			{
				throw new InvalidOperationException("Second child has already been configured");
			}
			_childrenUsed[1] = true;
			TTimeTicker val = CreateChild();
			ChildBuilder<TTimeTicker> obj = new ChildBuilder<TTimeTicker>(val);
			configure(obj);
			_rootTicker.Children.Add(val);
			return new SecondChildBuilder(this, val, 1);
		}

		public ThirdChildBuilder WithThirdChild(Action<ChildBuilder<TTimeTicker>> configure)
		{
			if (_childrenUsed[2])
			{
				throw new InvalidOperationException("Third child has already been configured");
			}
			_childrenUsed[2] = true;
			TTimeTicker val = CreateChild();
			ChildBuilder<TTimeTicker> obj = new ChildBuilder<TTimeTicker>(val);
			configure(obj);
			_rootTicker.Children.Add(val);
			return new ThirdChildBuilder(this, val, 2);
		}

		public FourthChildBuilder WithFourthChild(Action<ChildBuilder<TTimeTicker>> configure)
		{
			if (_childrenUsed[3])
			{
				throw new InvalidOperationException("Fourth child has already been configured");
			}
			_childrenUsed[3] = true;
			TTimeTicker val = CreateChild();
			ChildBuilder<TTimeTicker> obj = new ChildBuilder<TTimeTicker>(val);
			configure(obj);
			_rootTicker.Children.Add(val);
			return new FourthChildBuilder(this, val, 3);
		}

		public FifthChildBuilder WithFifthChild(Action<ChildBuilder<TTimeTicker>> configure)
		{
			if (_childrenUsed[4])
			{
				throw new InvalidOperationException("Fifth child has already been configured");
			}
			_childrenUsed[4] = true;
			TTimeTicker val = CreateChild();
			ChildBuilder<TTimeTicker> obj = new ChildBuilder<TTimeTicker>(val);
			configure(obj);
			_rootTicker.Children.Add(val);
			return new FifthChildBuilder(this, val, 4);
		}

		private TTimeTicker CreateChild()
		{
			return new TTimeTicker
			{
				Id = Guid.NewGuid(),
				ParentId = _rootTicker.Id,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				Children = new List<TTimeTicker>()
			};
		}

		private TTimeTicker CreateGrandChild(TTimeTicker parent)
		{
			return new TTimeTicker
			{
				Id = Guid.NewGuid(),
				ParentId = parent.Id,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow,
				Children = new List<TTimeTicker>()
			};
		}

		public TTimeTicker Build()
		{
			return _rootTicker;
		}

		public static implicit operator TTimeTicker(FluentChainTickerBuilder<TTimeTicker> builder)
		{
			return builder.Build();
		}
	}
	public class ParentBuilder<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
	{
		private readonly TTimeTicker _parent;

		internal ParentBuilder(TTimeTicker parent)
		{
			_parent = parent;
		}

		public ParentBuilder<TTimeTicker> SetFunction(string functionName)
		{
			_parent.Function = functionName;
			return this;
		}

		public ParentBuilder<TTimeTicker> SetDescription(string description)
		{
			_parent.Description = description;
			return this;
		}

		public ParentBuilder<TTimeTicker> SetExecutionTime(DateTime executionTime)
		{
			_parent.ExecutionTime = executionTime;
			return this;
		}

		public ParentBuilder<TTimeTicker> SetRequest<T>(T request)
		{
			_parent.Request = TickerHelper.CreateTickerRequest(request);
			return this;
		}

		public ParentBuilder<TTimeTicker> SetRetries(int retries, params int[] intervals)
		{
			_parent.Retries = retries;
			_parent.RetryIntervals = intervals;
			return this;
		}
	}
	public class ChildBuilder<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
	{
		private readonly TTimeTicker _child;

		internal ChildBuilder(TTimeTicker child)
		{
			_child = child;
		}

		public ChildBuilder<TTimeTicker> SetFunction(string functionName)
		{
			_child.Function = functionName;
			return this;
		}

		public ChildBuilder<TTimeTicker> SetDescription(string description)
		{
			_child.Description = description;
			return this;
		}

		public ChildBuilder<TTimeTicker> SetRunCondition(RunCondition condition)
		{
			_child.RunCondition = condition;
			return this;
		}

		public ChildBuilder<TTimeTicker> SetExecutionTime(DateTime executionTime)
		{
			_child.ExecutionTime = executionTime;
			return this;
		}

		public ChildBuilder<TTimeTicker> SetRequest<T>(T request)
		{
			_child.Request = TickerHelper.CreateTickerRequest(request);
			return this;
		}

		public ChildBuilder<TTimeTicker> SetRetries(int retries, params int[] intervals)
		{
			_child.Retries = retries;
			_child.RetryIntervals = intervals;
			return this;
		}
	}
	public class GrandChildBuilder<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
	{
		private readonly TTimeTicker _grandChild;

		internal GrandChildBuilder(TTimeTicker grandChild)
		{
			_grandChild = grandChild;
		}

		public GrandChildBuilder<TTimeTicker> SetFunction(string functionName)
		{
			_grandChild.Function = functionName;
			return this;
		}

		public GrandChildBuilder<TTimeTicker> SetDescription(string description)
		{
			_grandChild.Description = description;
			return this;
		}

		public GrandChildBuilder<TTimeTicker> SetRunCondition(RunCondition condition)
		{
			_grandChild.RunCondition = condition;
			return this;
		}

		public GrandChildBuilder<TTimeTicker> SetExecutionTime(DateTime executionTime)
		{
			_grandChild.ExecutionTime = executionTime;
			return this;
		}

		public GrandChildBuilder<TTimeTicker> SetRequest<T>(T request)
		{
			_grandChild.Request = TickerHelper.CreateTickerRequest(request);
			return this;
		}

		public GrandChildBuilder<TTimeTicker> SetRetries(int retries, params int[] intervals)
		{
			_grandChild.Retries = retries;
			_grandChild.RetryIntervals = intervals;
			return this;
		}
	}
	public static class FluentChainTickerBuilderExtensions
	{
		public static FluentChainTickerBuilder<TTimeTicker> BeginWith<TTimeTicker>(Action<ParentBuilder<TTimeTicker>> configure) where TTimeTicker : TimeTickerEntity<TTimeTicker>, new()
		{
			return FluentChainTickerBuilder<TTimeTicker>.BeginWith(configure);
		}
	}
	internal class InternalTickerManager<TTimeTicker, TCronTicker> : IInternalTickerManager where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		private readonly ITickerPersistenceProvider<TTimeTicker, TCronTicker> _persistenceProvider;

		private readonly ITickerClock _clock;

		private readonly ITickerQNotificationHubSender _notificationHubSender;

		public InternalTickerManager(ITickerPersistenceProvider<TTimeTicker, TCronTicker> persistenceProvider, ITickerClock clock, ITickerQNotificationHubSender notificationHubSender)
		{
			_persistenceProvider = persistenceProvider;
			_clock = clock ?? throw new ArgumentNullException("clock");
			_notificationHubSender = notificationHubSender;
		}

		public async Task<(TimeSpan TimeRemaining, InternalFunctionContext[] Functions)> GetNextTickers(CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime now = _clock.UtcNow;
			Task<(DateTime Key, InternalManagerContext[] Items)?> minCronGroupTask = GetEarliestCronTickerGroupAsync(cancellationToken);
			Task<TimeTickerEntity[]> minTimeTickersTask = _persistenceProvider.GetEarliestTimeTickers(cancellationToken);
			InlineArray2<Task> buffer = default;
			buffer[0] = minCronGroupTask;
			buffer[1] = minTimeTickersTask;
			await Task.WhenAll(buffer).ConfigureAwait(continueOnCapturedContext: false);
			(DateTime Key, InternalManagerContext[] Items)? minCronGroup = await minCronGroupTask.ConfigureAwait(continueOnCapturedContext: false);
			TimeTickerEntity[] minTimeTickers = await minTimeTickersTask.ConfigureAwait(continueOnCapturedContext: false);
			DateTime? dateTime = minCronGroup?.Key;
			DateTime? dateTime2 = ((minTimeTickers.Length != 0) ? minTimeTickers[0].ExecutionTime : ((DateTime?)null));
			if (!dateTime.HasValue && !dateTime2.HasValue)
			{
				return (TimeRemaining: Timeout.InfiniteTimeSpan, Functions: Array.Empty<InternalFunctionContext>());
			}
			bool flag = false;
			bool includeTimeTickers = false;
			TimeSpan timeRemaining;
			if (!dateTime.HasValue)
			{
				includeTimeTickers = true;
				timeRemaining = SafeRemaining(dateTime2.Value, now);
			}
			else if (!dateTime2.HasValue)
			{
				flag = true;
				timeRemaining = SafeRemaining(dateTime.Value, now);
			}
			else
			{
				DateTime dateTime3 = new DateTime(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day, dateTime.Value.Hour, dateTime.Value.Minute, dateTime.Value.Second);
				DateTime dateTime4 = new DateTime(dateTime2.Value.Year, dateTime2.Value.Month, dateTime2.Value.Day, dateTime2.Value.Hour, dateTime2.Value.Minute, dateTime2.Value.Second);
				if (dateTime3 == dateTime4)
				{
					flag = true;
					includeTimeTickers = true;
					DateTime target = ((dateTime < dateTime2) ? dateTime.Value : dateTime2.Value);
					timeRemaining = SafeRemaining(target, now);
				}
				else if (dateTime < dateTime2)
				{
					flag = true;
					timeRemaining = SafeRemaining(dateTime.Value, now);
				}
				else
				{
					includeTimeTickers = true;
					timeRemaining = SafeRemaining(dateTime2.Value, now);
				}
			}
			if (!flag && !includeTimeTickers)
			{
				return (TimeRemaining: Timeout.InfiniteTimeSpan, Functions: Array.Empty<InternalFunctionContext>());
			}
			InternalFunctionContext[] cronFunctions = Array.Empty<InternalFunctionContext>();
			InternalFunctionContext[] timeFunctions = Array.Empty<InternalFunctionContext>();
			if (flag && minCronGroup.HasValue)
			{
				cronFunctions = await QueueNextCronTickersAsync(minCronGroup.Value, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (includeTimeTickers && minTimeTickers.Length != 0)
			{
				timeFunctions = await QueueNextTimeTickersAsync(minTimeTickers, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (cronFunctions.Length == 0 && timeFunctions.Length == 0)
			{
				return (TimeRemaining: timeRemaining, Functions: Array.Empty<InternalFunctionContext>());
			}
			if (cronFunctions.Length == 0)
			{
				return (TimeRemaining: timeRemaining, Functions: timeFunctions);
			}
			if (timeFunctions.Length == 0)
			{
				return (TimeRemaining: timeRemaining, Functions: cronFunctions);
			}
			InternalFunctionContext[] array = new InternalFunctionContext[cronFunctions.Length + timeFunctions.Length];
			cronFunctions.AsSpan().CopyTo(array.AsSpan(0, cronFunctions.Length));
			timeFunctions.AsSpan().CopyTo(array.AsSpan(cronFunctions.Length, timeFunctions.Length));
			return (TimeRemaining: timeRemaining, Functions: array);
		}

		private static TimeSpan SafeRemaining(DateTime target, DateTime now)
		{
			TimeSpan timeSpan = target - now;
			if (!(timeSpan < TimeSpan.Zero))
			{
				return timeSpan;
			}
			return TimeSpan.Zero;
		}

		private async Task<InternalFunctionContext[]> QueueNextTimeTickersAsync(TimeTickerEntity[] minTimeTickers, CancellationToken cancellationToken = default(CancellationToken))
		{
			List<InternalFunctionContext> results = new List<InternalFunctionContext>();
			await foreach (TimeTickerEntity item in _persistenceProvider.QueueTimeTickers(minTimeTickers, cancellationToken))
			{
				results.Add(new InternalFunctionContext
				{
					FunctionName = item.Function,
					TickerId = item.Id,
					Type = TickerType.TimeTicker,
					Retries = item.Retries,
					RetryIntervals = item.RetryIntervals,
					ParentId = item.ParentId,
					ExecutionTime = (item.ExecutionTime ?? _clock.UtcNow),
					TimeTickerChildren = item.Children.Select((TimeTickerEntity ch) => new InternalFunctionContext
					{
						FunctionName = ch.Function,
						TickerId = ch.Id,
						Type = TickerType.TimeTicker,
						Retries = ch.Retries,
						RetryIntervals = ch.RetryIntervals,
						ParentId = ch.ParentId,
						RunCondition = (ch.RunCondition ?? RunCondition.OnAnyCompletedStatus),
						TimeTickerChildren = ch.Children.Select((TimeTickerEntity gch) => new InternalFunctionContext
						{
							FunctionName = gch.Function,
							TickerId = gch.Id,
							Type = TickerType.TimeTicker,
							Retries = gch.Retries,
							RetryIntervals = gch.RetryIntervals,
							ParentId = gch.ParentId,
							RunCondition = (gch.RunCondition ?? RunCondition.OnAnyCompletedStatus)
						}).ToList()
					}).ToList()
				});
				await _notificationHubSender.UpdateTimeTickerNotifyAsync(item);
			}
			return results.ToArray();
		}

		private async Task<InternalFunctionContext[]> QueueNextCronTickersAsync((DateTime Key, InternalManagerContext[] Items) minCronTicker, CancellationToken cancellationToken = default(CancellationToken))
		{
			List<InternalFunctionContext> results = new List<InternalFunctionContext>();
			await foreach (CronTickerOccurrenceEntity<TCronTicker> item in _persistenceProvider.QueueCronTickerOccurrences(minCronTicker, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				results.Add(new InternalFunctionContext
				{
					ParentId = item.CronTickerId,
					FunctionName = item.CronTicker.Function,
					TickerId = item.Id,
					Type = TickerType.CronTickerOccurrence,
					Retries = item.CronTicker.Retries,
					RetryIntervals = item.CronTicker.RetryIntervals,
					ExecutionTime = item.ExecutionTime
				});
				if (item.CreatedAt == item.UpdatedAt && _notificationHubSender != null)
				{
					await _notificationHubSender.AddCronOccurrenceAsync(item.CronTickerId, item).ConfigureAwait(continueOnCapturedContext: false);
				}
				else if (_notificationHubSender != null)
				{
					await _notificationHubSender.UpdateCronOccurrenceAsync(item.CronTickerId, item).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return results.ToArray();
		}

		private async Task<(DateTime Key, InternalManagerContext[] Items)?> GetEarliestCronTickerGroupAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime now = _clock.UtcNow;
			CronTickerEntity[] cronTickers = await _persistenceProvider.GetAllCronTickerExpressions(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			Guid[] ids = cronTickers.Select((CronTickerEntity x) => x.Id).ToArray();
			return EarliestCronTickerGroup(cronTickers, now, await _persistenceProvider.GetEarliestAvailableCronOccurrence(ids, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
		}

		private static (DateTime Next, InternalManagerContext[] Items)? EarliestCronTickerGroup(CronTickerEntity[] cronTickers, DateTime now, CronTickerOccurrenceEntity<TCronTicker> earliestStored)
		{
			DateTime? dateTime = null;
			InternalManagerContext internalManagerContext = null;
			List<InternalManagerContext> list = null;
			foreach (CronTickerEntity cronTickerEntity in cronTickers)
			{
				DateTime? nextOccurrenceOrDefault = CronScheduleCache.GetNextOccurrenceOrDefault(cronTickerEntity.Expression, now);
				if (!nextOccurrenceOrDefault.HasValue)
				{
					continue;
				}
				if (earliestStored != null)
				{
					DateTime executionTime = earliestStored.ExecutionTime;
					DateTime? dateTime2 = nextOccurrenceOrDefault;
					if (dateTime2.HasValue && executionTime == dateTime2.GetValueOrDefault() && cronTickerEntity.Id == earliestStored.CronTickerId)
					{
						continue;
					}
				}
				DateTime value = nextOccurrenceOrDefault.Value;
				if (dateTime.HasValue)
				{
					DateTime executionTime = value;
					DateTime? dateTime2 = dateTime;
					if (!(executionTime < dateTime2))
					{
						executionTime = value;
						dateTime2 = dateTime;
						if (executionTime == dateTime2)
						{
							if (list == null)
							{
								list = new List<InternalManagerContext>(2) { internalManagerContext };
							}
							list.Add(new InternalManagerContext(cronTickerEntity.Id)
							{
								FunctionName = cronTickerEntity.Function,
								Expression = cronTickerEntity.Expression,
								Retries = cronTickerEntity.Retries,
								RetryIntervals = cronTickerEntity.RetryIntervals
							});
						}
						continue;
					}
				}
				dateTime = value;
				internalManagerContext = new InternalManagerContext(cronTickerEntity.Id)
				{
					FunctionName = cronTickerEntity.Function,
					Expression = cronTickerEntity.Expression,
					Retries = cronTickerEntity.Retries,
					RetryIntervals = cronTickerEntity.RetryIntervals
				};
				list = null;
			}
			if (earliestStored != null)
			{
				DateTime executionTime2 = earliestStored.ExecutionTime;
				InternalManagerContext internalManagerContext2 = new InternalManagerContext(earliestStored.CronTickerId)
				{
					FunctionName = earliestStored.CronTicker.Function,
					Expression = earliestStored.CronTicker.Expression,
					Retries = earliestStored.CronTicker.Retries,
					RetryIntervals = earliestStored.CronTicker.RetryIntervals,
					NextCronOccurrence = new NextCronOccurrence(earliestStored.Id, earliestStored.CreatedAt)
				};
				if (!dateTime.HasValue || executionTime2 < dateTime.Value)
				{
					return (executionTime2, new InternalManagerContext[1] { internalManagerContext2 });
				}
				if (executionTime2 == dateTime.Value)
				{
					if (list == null)
					{
						return (dateTime.Value, new InternalManagerContext[2] { internalManagerContext, internalManagerContext2 });
					}
					list.Add(internalManagerContext2);
					return (dateTime.Value, list.ToArray());
				}
				InternalManagerContext[] item = ((list != null) ? list.ToArray() : new InternalManagerContext[1] { internalManagerContext });
				return (dateTime.Value, item);
			}
			if (!dateTime.HasValue)
			{
				return null;
			}
			InternalManagerContext[] item2 = ((list != null) ? list.ToArray() : new InternalManagerContext[1] { internalManagerContext });
			return (dateTime.Value, item2);
		}

		public async Task SetTickersInProgress(InternalFunctionContext[] resources, CancellationToken cancellationToken = default(CancellationToken))
		{
			InternalFunctionContext unifiedFunctionContext = new InternalFunctionContext().SetProperty((InternalFunctionContext x) => x.Status, TickerStatus.InProgress);
			Guid[] array = (from x in resources
				where x.Type == TickerType.CronTickerOccurrence
				select x.TickerId).ToArray();
			Guid[] timeTickerIds = (from x in resources
				where x.Type == TickerType.TimeTicker
				select x.TickerId).ToArray();
			if (array.Length != 0 && timeTickerIds.Length != 0)
			{
				Task task = _persistenceProvider.UpdateCronTickerOccurrencesWithUnifiedContext(array, unifiedFunctionContext, cancellationToken);
				Task task2 = _persistenceProvider.UpdateTimeTickersWithUnifiedContext(timeTickerIds, unifiedFunctionContext, cancellationToken);
				InlineArray2<Task> buffer = default;
				buffer[0] = task;
				buffer[1] = task2;
				await Task.WhenAll(buffer).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				if (array.Length != 0)
				{
					await _persistenceProvider.UpdateCronTickerOccurrencesWithUnifiedContext(array, unifiedFunctionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (timeTickerIds.Length != 0)
				{
					await _persistenceProvider.UpdateTimeTickersWithUnifiedContext(timeTickerIds, unifiedFunctionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			foreach (InternalFunctionContext internalFunctionContext in resources)
			{
				internalFunctionContext.Status = TickerStatus.InProgress;
				if (internalFunctionContext.Type == TickerType.TimeTicker)
				{
					await _notificationHubSender.UpdateTimeTickerFromInternalFunctionContext<TTimeTicker>(internalFunctionContext).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await _notificationHubSender.UpdateCronOccurrenceFromInternalFunctionContext<TCronTicker>(internalFunctionContext).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		public async Task ReleaseAcquiredResources(InternalFunctionContext[] resources, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (resources == null)
			{
				InlineArray2<Task> buffer = default;
				buffer[0] = _persistenceProvider.ReleaseAcquiredCronTickerOccurrences(Array.Empty<Guid>(), cancellationToken);
				buffer[1] = _persistenceProvider.ReleaseAcquiredTimeTickers(Array.Empty<Guid>(), cancellationToken);
				await Task.WhenAll(buffer);
				return;
			}
			Guid[] array = ((resources.Length == 0) ? Array.Empty<Guid>() : (from x in resources
				where x.Type == TickerType.CronTickerOccurrence
				select x.TickerId).ToArray());
			if (array.Length != 0)
			{
				await _persistenceProvider.ReleaseAcquiredCronTickerOccurrences(array, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			Guid[] array2 = ((resources.Length == 0) ? Array.Empty<Guid>() : (from x in resources
				where x.Type == TickerType.TimeTicker
				select x.TickerId).ToArray());
			if (array2.Length != 0)
			{
				await _persistenceProvider.ReleaseAcquiredTimeTickers(array2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task UpdateTickerAsync(InternalFunctionContext functionContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (functionContext.Type == TickerType.CronTickerOccurrence)
			{
				await _persistenceProvider.UpdateCronTickerOccurrence(functionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await _notificationHubSender.UpdateCronOccurrenceFromInternalFunctionContext<TCronTicker>(functionContext).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				await _persistenceProvider.UpdateTimeTicker(functionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				await _notificationHubSender.UpdateTimeTickerFromInternalFunctionContext<TTimeTicker>(functionContext).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task UpdateSkipTimeTickersWithUnifiedContextAsync(InternalFunctionContext[] resources, CancellationToken cancellationToken = default(CancellationToken))
		{
			InternalFunctionContext functionContext = new InternalFunctionContext().SetProperty((InternalFunctionContext x) => x.Status, TickerStatus.Skipped).SetProperty((InternalFunctionContext x) => x.ExecutedAt, _clock.UtcNow).SetProperty((InternalFunctionContext x) => x.ExceptionDetails, "Rule RunCondition did not match!");
			if (resources.Length != 0)
			{
				await _persistenceProvider.UpdateTimeTickersWithUnifiedContext(resources.Select((InternalFunctionContext x) => x.TickerId).ToArray(), functionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			foreach (InternalFunctionContext internalFunctionContext in resources)
			{
				internalFunctionContext.ExecutedAt = _clock.UtcNow;
				internalFunctionContext.Status = TickerStatus.Skipped;
				internalFunctionContext.ExceptionDetails = "Rule RunCondition did not match!";
				if (internalFunctionContext.Type == TickerType.TimeTicker)
				{
					await _notificationHubSender.UpdateTimeTickerFromInternalFunctionContext<TTimeTicker>(internalFunctionContext).ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					await _notificationHubSender.UpdateCronOccurrenceFromInternalFunctionContext<TCronTicker>(internalFunctionContext).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}

		public async Task<T> GetRequestAsync<T>(Guid tickerId, TickerType type, CancellationToken cancellationToken = default(CancellationToken))
		{
			byte[] array = ((type != TickerType.CronTickerOccurrence) ? (await _persistenceProvider.GetTimeTickerRequest(tickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) : (await _persistenceProvider.GetCronTickerOccurrenceRequest(tickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
			byte[] array2 = array;
			return (array2 == null || array2.Length == 0) ? default(T) : TickerHelper.ReadTickerRequest<T>(array2);
		}

		public async Task<T> GetRequestAsync<T>(Guid tickerId, TickerType type, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default(CancellationToken))
		{
			byte[] array = ((type != TickerType.CronTickerOccurrence) ? (await _persistenceProvider.GetTimeTickerRequest(tickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) : (await _persistenceProvider.GetCronTickerOccurrenceRequest(tickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
			byte[] array2 = array;
			return (array2 == null || array2.Length == 0) ? default(T) : TickerHelper.ReadTickerRequest(array2, typeInfo);
		}

		public async Task<InternalFunctionContext[]> RunTimedOutTickers(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<InternalFunctionContext> results = new List<InternalFunctionContext>();
			await foreach (TimeTickerEntity item in _persistenceProvider.QueueTimedOutTimeTickers(cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				results.Add(new InternalFunctionContext
				{
					FunctionName = item.Function,
					TickerId = item.Id,
					Type = TickerType.TimeTicker,
					Retries = item.Retries,
					RetryIntervals = item.RetryIntervals,
					ParentId = item.ParentId,
					ExecutionTime = (item.ExecutionTime ?? _clock.UtcNow),
					TimeTickerChildren = item.Children.Select((TimeTickerEntity ch) => new InternalFunctionContext
					{
						FunctionName = ch.Function,
						TickerId = ch.Id,
						Type = TickerType.TimeTicker,
						Retries = ch.Retries,
						RetryIntervals = ch.RetryIntervals,
						ParentId = ch.ParentId,
						RunCondition = (ch.RunCondition ?? RunCondition.OnAnyCompletedStatus),
						TimeTickerChildren = ch.Children.Select((TimeTickerEntity gch) => new InternalFunctionContext
						{
							FunctionName = gch.Function,
							TickerId = gch.Id,
							Type = TickerType.TimeTicker,
							Retries = gch.Retries,
							RetryIntervals = gch.RetryIntervals,
							ParentId = gch.ParentId,
							RunCondition = (gch.RunCondition ?? RunCondition.OnAnyCompletedStatus)
						}).ToList()
					}).ToList()
				});
				await _notificationHubSender.UpdateTimeTickerNotifyAsync(item).ConfigureAwait(continueOnCapturedContext: false);
			}
			await foreach (CronTickerOccurrenceEntity<TCronTicker> item2 in _persistenceProvider.QueueTimedOutCronTickerOccurrences(cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				InternalFunctionContext internalFunctionContext = new InternalFunctionContext
				{
					FunctionName = item2.CronTicker.Function,
					TickerId = item2.Id,
					Type = TickerType.CronTickerOccurrence,
					Retries = item2.CronTicker.Retries,
					RetryIntervals = item2.CronTicker.RetryIntervals,
					ParentId = item2.CronTickerId,
					ExecutionTime = item2.ExecutionTime
				};
				results.Add(internalFunctionContext);
				await _notificationHubSender.UpdateCronOccurrenceFromInternalFunctionContext<TCronTicker>(internalFunctionContext).ConfigureAwait(continueOnCapturedContext: false);
			}
			return results.ToArray();
		}

		public async Task MigrateDefinedCronTickers((string, string)[] cronExpressions, CancellationToken cancellationToken = default(CancellationToken))
		{
			await _persistenceProvider.MigrateDefinedCronTickers(cronExpressions, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task DeleteTicker(Guid tickerId, TickerType type, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (type == TickerType.CronTickerOccurrence)
			{
				await _persistenceProvider.RemoveCronTickers(new Guid[1] { tickerId }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				await _persistenceProvider.RemoveTimeTickers(new Guid[1] { tickerId }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task<int> SkipStaleCronOccurrencesAsync(TimeSpan staleThreshold, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _persistenceProvider.SkipStaleCronOccurrencesAsync(staleThreshold, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		public async Task ReleaseDeadNodeResources(string instanceIdentifier, CancellationToken cancellationToken = default(CancellationToken))
		{
			Task task = _persistenceProvider.ReleaseDeadNodeOccurrenceResources(instanceIdentifier, cancellationToken);
			Task task2 = _persistenceProvider.ReleaseDeadNodeTimeTickerResources(instanceIdentifier, cancellationToken);
			InlineArray2<Task> buffer = default;
			buffer[0] = task;
			buffer[1] = task2;
			await Task.WhenAll(buffer).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
	internal class TickerManager<TTimeTicker, TCronTicker> : ICronTickerManager<TCronTicker>, ITimeTickerManager<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		private readonly ITickerPersistenceProvider<TTimeTicker, TCronTicker> _persistenceProvider;

		private readonly ITickerQHostScheduler _tickerQHostScheduler;

		private readonly ITickerClock _clock;

		private readonly ITickerQNotificationHubSender _notificationHubSender;

		private readonly ITickerQDispatcher _dispatcher;

		private readonly TickerExecutionContext _executionContext;

		public TickerManager(ITickerPersistenceProvider<TTimeTicker, TCronTicker> persistenceProvider, ITickerQHostScheduler tickerQHostScheduler, ITickerClock clock, ITickerQNotificationHubSender notificationHubSender, TickerExecutionContext executionContext, ITickerQDispatcher dispatcher)
		{
			_persistenceProvider = persistenceProvider;
			_tickerQHostScheduler = tickerQHostScheduler ?? throw new ArgumentNullException("tickerQHostScheduler");
			_clock = clock;
			_notificationHubSender = notificationHubSender;
			_executionContext = executionContext ?? throw new ArgumentNullException("executionContext");
			_dispatcher = dispatcher ?? throw new ArgumentNullException("dispatcher");
		}

		Task<TickerResult<TCronTicker>> ICronTickerManager<TCronTicker>.AddAsync(TCronTicker entity, CancellationToken cancellationToken)
		{
			return AddCronTickerAsync(entity, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.AddAsync(TTimeTicker entity, CancellationToken cancellationToken)
		{
			return AddTimeTickerAsync(entity, cancellationToken);
		}

		Task<TickerResult<TCronTicker>> ICronTickerManager<TCronTicker>.UpdateAsync(TCronTicker cronTicker, CancellationToken cancellationToken)
		{
			return UpdateCronTickerAsync(cronTicker, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.UpdateAsync(TTimeTicker timeTicker, CancellationToken cancellationToken)
		{
			return UpdateTimeTickerAsync(timeTicker, cancellationToken);
		}

		Task<TickerResult<TCronTicker>> ICronTickerManager<TCronTicker>.DeleteAsync(Guid id, CancellationToken cancellationToken)
		{
			return DeleteCronTickerAsync(id, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.DeleteAsync(Guid id, CancellationToken cancellationToken)
		{
			return DeleteTimeTickerAsync(id, cancellationToken);
		}

		Task<TickerResult<List<TTimeTicker>>> ITimeTickerManager<TTimeTicker>.AddBatchAsync(List<TTimeTicker> entities, CancellationToken cancellationToken)
		{
			return AddTimeTickersBatchAsync(entities, cancellationToken);
		}

		Task<TickerResult<List<TTimeTicker>>> ITimeTickerManager<TTimeTicker>.UpdateBatchAsync(List<TTimeTicker> timeTickers, CancellationToken cancellationToken)
		{
			return UpdateTimeTickersBatchAsync(timeTickers, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.DeleteBatchAsync(List<Guid> ids, CancellationToken cancellationToken)
		{
			return DeleteTimeTickersBatchAsync(ids, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.AddAsync<TFunction>(DateTime? executionTime, CancellationToken cancellationToken)
		{
			string functionName = TickerFunctionProvider.GetFunctionName<TFunction>();
			TTimeTicker entity = new TTimeTicker
			{
				Function = functionName,
				ExecutionTime = executionTime
			};
			return AddTimeTickerAsync(entity, cancellationToken);
		}

		Task<TickerResult<TTimeTicker>> ITimeTickerManager<TTimeTicker>.AddAsync<TFunction, TRequest>(DateTime? executionTime, TRequest request, CancellationToken cancellationToken)
		{
			string functionName = TickerFunctionProvider.GetFunctionName<TFunction>();
			TTimeTicker entity = new TTimeTicker
			{
				Function = functionName,
				ExecutionTime = executionTime,
				Request = TickerHelper.CreateTickerRequest(request)
			};
			return AddTimeTickerAsync(entity, cancellationToken);
		}

		Task<TickerResult<List<TCronTicker>>> ICronTickerManager<TCronTicker>.AddBatchAsync(List<TCronTicker> entities, CancellationToken cancellationToken)
		{
			return AddCronTickersBatchAsync(entities, cancellationToken);
		}

		Task<TickerResult<List<TCronTicker>>> ICronTickerManager<TCronTicker>.UpdateBatchAsync(List<TCronTicker> cronTickers, CancellationToken cancellationToken)
		{
			return UpdateCronTickersBatchAsync(cronTickers, cancellationToken);
		}

		Task<TickerResult<TCronTicker>> ICronTickerManager<TCronTicker>.DeleteBatchAsync(List<Guid> ids, CancellationToken cancellationToken)
		{
			return DeleteCronTickersBatchAsync(ids, cancellationToken);
		}

		private async Task<TickerResult<TTimeTicker>> AddTimeTickerAsync(TTimeTicker entity, CancellationToken cancellationToken)
		{
			if (entity.Id == Guid.Empty)
			{
				entity.Id = Guid.NewGuid();
			}
			if (TickerFunctionProvider.TickerFunctions.All((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> x) => x.Key != entity?.Function))
			{
				return new TickerResult<TTimeTicker>(new TickerValidatorException("Cannot find TickerFunction with name " + entity?.Function));
			}
			entity.ExecutionTime = ((!entity.ExecutionTime.HasValue) ? _clock.UtcNow : ConvertToUtcIfNeeded(entity.ExecutionTime.Value));
			entity.CreatedAt = _clock.UtcNow;
			entity.UpdatedAt = _clock.UtcNow;
			try
			{
				DateTime now = _clock.UtcNow;
				DateTime executionTime = entity.ExecutionTime.Value;
				await _persistenceProvider.AddTimeTickers(new TTimeTicker[1] { entity }, cancellationToken);
				await _notificationHubSender.AddTimeTickerNotifyAsync(entity.Id).ConfigureAwait(continueOnCapturedContext: false);
				if (_dispatcher.IsEnabled && executionTime <= now.AddSeconds(1.0))
				{
					TimeTickerEntity[] array = await _persistenceProvider.AcquireImmediateTimeTickersAsync(new Guid[1] { entity.Id }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (array.Length != 0)
					{
						InternalFunctionContext[] array2 = BuildImmediateContextsFromNonGeneric(array);
						CacheFunctionReferences(array2.AsSpan());
						await _dispatcher.DispatchAsync(array2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				else
				{
					_tickerQHostScheduler.RestartIfNeeded(executionTime);
				}
				return new TickerResult<TTimeTicker>(entity);
			}
			catch (Exception exception)
			{
				return new TickerResult<TTimeTicker>(exception);
			}
		}

		private async Task<TickerResult<TCronTicker>> AddCronTickerAsync(TCronTicker entity, CancellationToken cancellationToken)
		{
			if (entity.Id == Guid.Empty)
			{
				entity.Id = Guid.NewGuid();
			}
			if (TickerFunctionProvider.TickerFunctions.All((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> x) => x.Key != entity?.Function))
			{
				return new TickerResult<TCronTicker>(new TickerValidatorException("Cannot find TickerFunction with name " + entity?.Function));
			}
			DateTime? nextOccurrenceOrDefault = CronScheduleCache.GetNextOccurrenceOrDefault(entity.Expression, _clock.UtcNow);
			if (nextOccurrenceOrDefault.HasValue)
			{
				DateTime nextOccurrence = nextOccurrenceOrDefault.GetValueOrDefault();
				entity.CreatedAt = _clock.UtcNow;
				entity.UpdatedAt = _clock.UtcNow;
				try
				{
					await _persistenceProvider.InsertCronTickers(new TCronTicker[1] { entity }, cancellationToken);
					_tickerQHostScheduler.RestartIfNeeded(nextOccurrence);
					await _notificationHubSender.AddCronTickerNotifyAsync(entity);
					return new TickerResult<TCronTicker>(entity);
				}
				catch (Exception exception)
				{
					return new TickerResult<TCronTicker>(exception);
				}
			}
			return new TickerResult<TCronTicker>(new TickerValidatorException("Cannot parse expression " + entity.Expression));
		}

		private async Task<TickerResult<TTimeTicker>> UpdateTimeTickerAsync(TTimeTicker timeTicker, CancellationToken cancellationToken)
		{
			if (timeTicker == null)
			{
				return new TickerResult<TTimeTicker>(new TickerValidatorException("Ticker must not be null!"));
			}
			if (!timeTicker.ExecutionTime.HasValue)
			{
				return new TickerResult<TTimeTicker>(new TickerValidatorException("Ticker ExecutionTime must not be null!"));
			}
			timeTicker.UpdatedAt = _clock.UtcNow;
			timeTicker.ExecutionTime = ConvertToUtcIfNeeded(timeTicker.ExecutionTime.Value);
			try
			{
				int affectedRows = await _persistenceProvider.UpdateTimeTickers(new TTimeTicker[1] { timeTicker }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (_executionContext.Functions.Any((InternalFunctionContext x) => x.TickerId == timeTicker.Id))
				{
					_tickerQHostScheduler.Restart();
				}
				else
				{
					_tickerQHostScheduler.RestartIfNeeded(timeTicker.ExecutionTime);
				}
				return new TickerResult<TTimeTicker>(timeTicker, affectedRows);
			}
			catch (Exception exception)
			{
				return new TickerResult<TTimeTicker>(exception);
			}
		}

		private async Task<TickerResult<TCronTicker>> UpdateCronTickerAsync(TCronTicker cronTicker, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cronTicker == null)
			{
				return new TickerResult<TCronTicker>(new Exception("Cron ticker must not be null!"));
			}
			if (TickerFunctionProvider.TickerFunctions.All((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> x) => x.Key != cronTicker?.Function))
			{
				return new TickerResult<TCronTicker>(new TickerValidatorException("Cannot find TickerFunction with name " + cronTicker.Function));
			}
			DateTime? nextOccurrenceOrDefault = CronScheduleCache.GetNextOccurrenceOrDefault(cronTicker.Expression, _clock.UtcNow);
			if (nextOccurrenceOrDefault.HasValue)
			{
				DateTime nextOccurrence = nextOccurrenceOrDefault.GetValueOrDefault();
				try
				{
					cronTicker.UpdatedAt = _clock.UtcNow;
					int affectedRows = await _persistenceProvider.UpdateCronTickers(new TCronTicker[1] { cronTicker }, cancellationToken);
					InternalFunctionContext internalFunctionContext = _executionContext.Functions.FirstOrDefault((InternalFunctionContext x) => x.ParentId == cronTicker.Id);
					if (internalFunctionContext != null)
					{
						internalFunctionContext.ResetUpdateProps().SetProperty((InternalFunctionContext x) => x.ExecutionTime, nextOccurrence);
						await _persistenceProvider.UpdateCronTickerOccurrence(internalFunctionContext, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						_tickerQHostScheduler.Restart();
					}
					_tickerQHostScheduler.RestartIfNeeded(nextOccurrence);
					return new TickerResult<TCronTicker>(cronTicker, affectedRows);
				}
				catch (Exception exception)
				{
					return new TickerResult<TCronTicker>(exception);
				}
			}
			return new TickerResult<TCronTicker>(new TickerValidatorException("Cannot parse expression " + cronTicker.Expression));
		}

		private async Task<TickerResult<TCronTicker>> DeleteCronTickerAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			int num = await _persistenceProvider.RemoveCronTickers(new Guid[1] { id }, cancellationToken);
			if (num > 0 && _executionContext.Functions.Any((InternalFunctionContext x) => x.ParentId == id))
			{
				_tickerQHostScheduler.Restart();
			}
			return new TickerResult<TCronTicker>(num);
		}

		private async Task<TickerResult<TTimeTicker>> DeleteTimeTickerAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			int num = await _persistenceProvider.RemoveTimeTickers(new Guid[1] { id }, cancellationToken);
			if (num > 0 && _executionContext.Functions.Any((InternalFunctionContext x) => x.TickerId == id))
			{
				_tickerQHostScheduler.Restart();
			}
			return new TickerResult<TTimeTicker>(num);
		}

		private static DateTime ConvertToUtcIfNeeded(DateTime dateTime)
		{
			return dateTime.Kind switch
			{
				DateTimeKind.Utc => dateTime, 
				DateTimeKind.Local => dateTime.ToUniversalTime(), 
				DateTimeKind.Unspecified => TimeZoneInfo.ConvertTimeToUtc(dateTime, CronScheduleCache.TimeZoneInfo), 
				_ => dateTime, 
			};
		}

		private static void CacheFunctionReferences(Span<InternalFunctionContext> functions)
		{
			for (int i = 0; i < functions.Length; i++)
			{
				ref InternalFunctionContext reference = ref functions[i];
				if (TickerFunctionProvider.TickerFunctions.TryGetValue(reference.FunctionName, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
				{
					reference.CachedDelegate = value.Item3;
					reference.CachedPriority = value.Item2;
					reference.CachedMaxConcurrency = value.Item4;
				}
				List<InternalFunctionContext> timeTickerChildren = reference.TimeTickerChildren;
				if (timeTickerChildren != null && timeTickerChildren.Count > 0)
				{
					CacheFunctionReferences(CollectionsMarshal.AsSpan(reference.TimeTickerChildren));
				}
			}
		}

		private static InternalFunctionContext[] BuildImmediateContextsFromNonGeneric(IEnumerable<TimeTickerEntity> tickers)
		{
			return tickers.Select(BuildContextFromNonGeneric).ToArray();
		}

		private static InternalFunctionContext BuildContextFromNonGeneric(TimeTickerEntity ticker)
		{
			return new InternalFunctionContext
			{
				FunctionName = ticker.Function,
				TickerId = ticker.Id,
				Type = TickerType.TimeTicker,
				Retries = ticker.Retries,
				RetryIntervals = ticker.RetryIntervals,
				ParentId = ticker.ParentId,
				ExecutionTime = (ticker.ExecutionTime ?? DateTime.UtcNow),
				RunCondition = (ticker.RunCondition ?? RunCondition.OnAnyCompletedStatus),
				TimeTickerChildren = ticker.Children.Select(BuildContextFromNonGeneric).ToList()
			};
		}

		private async Task<TickerResult<List<TTimeTicker>>> AddTimeTickersBatchAsync(List<TTimeTicker> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (entities == null || entities.Count == 0)
			{
				return new TickerResult<List<TTimeTicker>>(entities ?? new List<TTimeTicker>());
			}
			HashSet<string> hashSet = new HashSet<string>(TickerFunctionProvider.TickerFunctions.Keys);
			List<Guid> immediateTickers = new List<Guid>();
			DateTime utcNow = _clock.UtcNow;
			DateTime earliestForNonImmediate = default;
			foreach (TTimeTicker entity in entities)
			{
				if (entity.Id == Guid.Empty)
				{
					entity.Id = Guid.NewGuid();
				}
				if (!hashSet.Contains(entity.Function))
				{
					return new TickerResult<List<TTimeTicker>>(new TickerValidatorException("Cannot find TickerFunction with name " + entity?.Function));
				}
				TTimeTicker val = entity;
				DateTime? executionTime = val.ExecutionTime;
				executionTime.GetValueOrDefault();
				if (!executionTime.HasValue)
				{
					DateTime value = utcNow;
					DateTime? executionTime2 = value;
					val.ExecutionTime = executionTime2;
				}
				entity.ExecutionTime = ConvertToUtcIfNeeded(entity.ExecutionTime.Value);
				entity.CreatedAt = utcNow;
				entity.UpdatedAt = utcNow;
				if (entity.ExecutionTime.Value <= utcNow.AddSeconds(1.0))
				{
					immediateTickers.Add(entity.Id);
				}
				else if (earliestForNonImmediate == default(DateTime) || entity.ExecutionTime <= earliestForNonImmediate)
				{
					earliestForNonImmediate = entity.ExecutionTime.Value;
				}
			}
			try
			{
				await _persistenceProvider.AddTimeTickers(entities.ToArray(), cancellationToken);
				await _notificationHubSender.AddTimeTickersBatchNotifyAsync().ConfigureAwait(continueOnCapturedContext: false);
				if (_dispatcher.IsEnabled && immediateTickers.Count > 0)
				{
					TimeTickerEntity[] array = await _persistenceProvider.AcquireImmediateTimeTickersAsync(immediateTickers.ToArray(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					if (array.Length != 0)
					{
						InternalFunctionContext[] array2 = BuildImmediateContextsFromNonGeneric(array);
						CacheFunctionReferences(array2.AsSpan());
						await _dispatcher.DispatchAsync(array2, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				if (earliestForNonImmediate != default(DateTime))
				{
					_tickerQHostScheduler.RestartIfNeeded(earliestForNonImmediate);
				}
				return new TickerResult<List<TTimeTicker>>(entities);
			}
			catch (Exception exception)
			{
				return new TickerResult<List<TTimeTicker>>(exception);
			}
		}

		private async Task<TickerResult<List<TCronTicker>>> AddCronTickersBatchAsync(List<TCronTicker> entities, CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TCronTicker> validEntities = new List<TCronTicker>();
			List<Exception> list = new List<Exception>();
			List<DateTime> nextOccurrences = new List<DateTime>();
			foreach (TCronTicker entity in entities)
			{
				if (entity.Id == Guid.Empty)
				{
					entity.Id = Guid.NewGuid();
				}
				if (TickerFunctionProvider.TickerFunctions.All((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> x) => x.Key != entity?.Function))
				{
					list.Add(new TickerValidatorException("Cannot find TickerFunction with name " + entity?.Function));
					continue;
				}
				DateTime? nextOccurrenceOrDefault = CronScheduleCache.GetNextOccurrenceOrDefault(entity.Expression, _clock.UtcNow);
				if (nextOccurrenceOrDefault.HasValue)
				{
					DateTime valueOrDefault = nextOccurrenceOrDefault.GetValueOrDefault();
					entity.CreatedAt = _clock.UtcNow;
					entity.UpdatedAt = _clock.UtcNow;
					validEntities.Add(entity);
					nextOccurrences.Add(valueOrDefault);
				}
				else
				{
					list.Add(new TickerValidatorException("Cannot parse expression " + entity.Expression));
				}
			}
			if (list.Any())
			{
				return new TickerResult<List<TCronTicker>>(list.First());
			}
			try
			{
				await _persistenceProvider.InsertCronTickers(validEntities.ToArray(), cancellationToken);
				if (validEntities.Count != 0)
				{
					DateTime value = nextOccurrences.Min();
					_tickerQHostScheduler.RestartIfNeeded(value);
					foreach (TCronTicker item in validEntities)
					{
						await _notificationHubSender.AddCronTickerNotifyAsync(item);
					}
				}
				return new TickerResult<List<TCronTicker>>(validEntities);
			}
			catch (Exception exception)
			{
				return new TickerResult<List<TCronTicker>>(exception);
			}
		}

		private async Task<TickerResult<List<TTimeTicker>>> UpdateTimeTickersBatchAsync(List<TTimeTicker> timeTickers, CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TTimeTicker> validTickers = new List<TTimeTicker>();
			List<Exception> list = new List<Exception>();
			bool needsRestart = false;
			foreach (TTimeTicker timeTicker in timeTickers)
			{
				if (timeTicker == null)
				{
					list.Add(new TickerValidatorException("Ticker must not be null!"));
					continue;
				}
				if (!timeTicker.ExecutionTime.HasValue)
				{
					list.Add(new TickerValidatorException("Ticker ExecutionTime must not be null!"));
					continue;
				}
				timeTicker.UpdatedAt = _clock.UtcNow;
				timeTicker.ExecutionTime = ConvertToUtcIfNeeded(timeTicker.ExecutionTime.Value);
				if (_executionContext.Functions.Any((InternalFunctionContext x) => x.TickerId == timeTicker.Id))
				{
					needsRestart = true;
				}
				validTickers.Add(timeTicker);
			}
			if (list.Any())
			{
				return new TickerResult<List<TTimeTicker>>(list.First());
			}
			try
			{
				int affectedRows = await _persistenceProvider.UpdateTimeTickers(validTickers.ToArray(), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (needsRestart)
				{
					_tickerQHostScheduler.Restart();
				}
				else if (validTickers.Any())
				{
					DateTime? dateTime = validTickers.Min((TTimeTicker t) => t.ExecutionTime);
					_tickerQHostScheduler.RestartIfNeeded(dateTime);
				}
				return new TickerResult<List<TTimeTicker>>(validTickers, affectedRows);
			}
			catch (Exception exception)
			{
				return new TickerResult<List<TTimeTicker>>(exception);
			}
		}

		private async Task<TickerResult<List<TCronTicker>>> UpdateCronTickersBatchAsync(List<TCronTicker> cronTickers, CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TCronTicker> validTickers = new List<TCronTicker>();
			List<Exception> list = new List<Exception>();
			List<DateTime> nextOccurrences = new List<DateTime>();
			bool needsRestart = false;
			List<InternalFunctionContext> internalFunctionsToUpdate = new List<InternalFunctionContext>();
			foreach (TCronTicker cronTicker in cronTickers)
			{
				if (cronTicker == null)
				{
					list.Add(new Exception("Cron ticker must not be null!"));
					continue;
				}
				if (TickerFunctionProvider.TickerFunctions.All((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> x) => x.Key != cronTicker?.Function))
				{
					list.Add(new TickerValidatorException("Cannot find TickerFunction with name " + cronTicker.Function));
					continue;
				}
				DateTime? nextOccurrenceOrDefault = CronScheduleCache.GetNextOccurrenceOrDefault(cronTicker.Expression, _clock.UtcNow);
				if (nextOccurrenceOrDefault.HasValue)
				{
					DateTime valueOrDefault = nextOccurrenceOrDefault.GetValueOrDefault();
					cronTicker.UpdatedAt = _clock.UtcNow;
					InternalFunctionContext internalFunctionContext = _executionContext.Functions.FirstOrDefault((InternalFunctionContext x) => x.ParentId == cronTicker.Id);
					if (internalFunctionContext != null)
					{
						internalFunctionContext.ResetUpdateProps().SetProperty((InternalFunctionContext x) => x.ExecutionTime, valueOrDefault);
						internalFunctionsToUpdate.Add(internalFunctionContext);
						needsRestart = true;
					}
					validTickers.Add(cronTicker);
					nextOccurrences.Add(valueOrDefault);
				}
				else
				{
					list.Add(new TickerValidatorException("Cannot parse expression " + cronTicker.Expression));
				}
			}
			if (list.Any())
			{
				return new TickerResult<List<TCronTicker>>(list.First());
			}
			try
			{
				int affectedRows = await _persistenceProvider.UpdateCronTickers(validTickers.ToArray(), cancellationToken);
				foreach (InternalFunctionContext item in internalFunctionsToUpdate)
				{
					await _persistenceProvider.UpdateCronTickerOccurrence(item, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (needsRestart)
				{
					_tickerQHostScheduler.Restart();
				}
				else if (nextOccurrences.Any())
				{
					DateTime value = nextOccurrences.Min();
					_tickerQHostScheduler.RestartIfNeeded(value);
				}
				return new TickerResult<List<TCronTicker>>(validTickers, affectedRows);
			}
			catch (Exception exception)
			{
				return new TickerResult<List<TCronTicker>>(exception);
			}
		}

		private async Task<TickerResult<TTimeTicker>> DeleteTimeTickersBatchAsync(List<Guid> ids, CancellationToken cancellationToken = default(CancellationToken))
		{
			int num = await _persistenceProvider.RemoveTimeTickers(ids.ToArray(), cancellationToken);
			if (num > 0 && _executionContext.Functions.Any((InternalFunctionContext x) => ids.Contains(x.TickerId)))
			{
				_tickerQHostScheduler.Restart();
			}
			return new TickerResult<TTimeTicker>(num);
		}

		private async Task<TickerResult<TCronTicker>> DeleteCronTickersBatchAsync(List<Guid> ids, CancellationToken cancellationToken = default(CancellationToken))
		{
			int num = await _persistenceProvider.RemoveCronTickers(ids.ToArray(), cancellationToken);
			if (num > 0 && _executionContext.Functions.Any((InternalFunctionContext x) => ids.Contains(x.ParentId ?? Guid.Empty)))
			{
				_tickerQHostScheduler.Restart();
			}
			return new TickerResult<TCronTicker>(num);
		}
	}
}
namespace TickerQ.Utilities.Interfaces
{
	public interface ITickerClock
	{
		DateTime UtcNow { get; }
	}
	public interface ITickerDashboardDataService<TTimeTicker, TCronTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		Task<PaginationResult<TimeTickerFlatDto>> GetTimeTickersFlatAsync(TimeTickerQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken));

		Task<TimeTickerFlatDto> GetTimeTickerByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<TimeTickerFlatDto>> GetTimeTickerChildrenAsync(Guid parentId, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<CronTickerFlatDto>> GetCronTickersFlatAsync(CronTickerQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken));

		Task<CronTickerFlatDto> GetCronTickerByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<CronOccurrenceFlatDto>> GetCronOccurrencesFlatAsync(Guid cronTickerId, CronOccurrenceQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken));

		Task<CronOccurrenceFlatDto> GetCronOccurrenceByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<ExecutionFlatDto>> GetExecutionsFlatAsync(ExecutionQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken));

		Task<byte[]> GetTimeTickerRequestAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<byte[]> GetCronOccurrenceRequestAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<(TickerStatus Status, int Count)>> GetOverallStatusesAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<(string NodeName, int JobCount)>> GetNodeJobsAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<TimeTickerFlatDto>> GetUpcomingTickersAsync(int count, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<ExecutionFlatDto>> GetRecentActivityAsync(int count, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<NodeDto>> GetNodesAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<FunctionInfoDto>> GetNodeFunctionsAsync(string nodeName, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<FunctionInfoDto>> GetAllFunctionsAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<HostStatusDto> GetHostStatusAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<NextTickerDto> GetNextTickerAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<GraphBucketDto>> GetTimeTickersGraphAsync(int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<GraphBucketDto>> GetCronTickersGraphAsync(int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<GraphBucketDto>> GetCronTickerGraphByIdAsync(Guid cronTickerId, int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<GraphBucketDto>> GetCronOccurrencesGraphAsync(Guid cronTickerId, CancellationToken cancellationToken = default(CancellationToken));

		Task<bool> ToggleCronTickerAsync(Guid cronTickerId, bool isEnabled, CancellationToken cancellationToken = default(CancellationToken));

		Task RunCronTickerOnDemandAsync(Guid cronTickerId, CancellationToken cancellationToken = default(CancellationToken));

		bool CancelTicker(Guid tickerId);

		Task StartHostAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task StopHostAsync(CancellationToken cancellationToken = default(CancellationToken));

		void RestartHost();
	}
	internal interface ITickerDashboardRepository<TTimeTicker, TCronTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		Task<TTimeTicker[]> GetTimeTickersAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<TTimeTicker>> GetTimeTickersPaginatedAsync(int pageNumber, int pageSize, TickerStatus? status = null, string search = null, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<Tuple<TickerStatus, int>>> GetTimeTickerFullDataAsync(CancellationToken cancellationToken);

		Task<IList<TickerGraphData>> GetTimeTickersGraphSpecificDataAsync(int pastDays, int futureDays, CancellationToken cancellationToken);

		Task<IList<TickerGraphData>> GetCronTickersGraphSpecificDataAsync(int pastDays, int futureDays, CancellationToken cancellationToken);

		Task<IList<TickerGraphData>> GetCronTickersGraphSpecificDataByIdAsync(Guid id, int pastDays, int futureDays, CancellationToken cancellationToken);

		Task<IList<Tuple<TickerStatus, int>>> GetCronTickerFullDataAsync(CancellationToken cancellationToken);

		Task<CronTickerEntity[]> GetCronTickersAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<CronTickerEntity>> GetCronTickersPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));

		Task AddOnDemandCronTickerOccurrenceAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<CronTickerOccurrenceEntity<TCronTicker>[]> GetCronTickersOccurrencesAsync(Guid guid, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<CronTickerOccurrenceEntity<TCronTicker>>> GetCronTickersOccurrencesPaginatedAsync(Guid guid, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<CronOccurrenceTickerGraphData>> GetCronTickersOccurrencesGraphDataAsync(Guid guid, CancellationToken cancellationToken = default(CancellationToken));

		Task<bool> ToggleCronTickerAsync(Guid id, bool isEnabled, CancellationToken cancellationToken = default(CancellationToken));

		bool CancelTickerById(Guid tickerId);

		Task DeleteCronTickerOccurrenceByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<(string, int)> GetTickerRequestByIdAsync(Guid tickerId, TickerType tickerType, CancellationToken cancellationToken = default(CancellationToken));

		IEnumerable<(string, (string, string, TickerTaskPriority))> GetTickerFunctions();

		Task<IList<(int, int)>> GetLastWeekJobStatusesAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<(TickerStatus, int)>> GetOverallJobStatusesAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<IList<(string, int)>> GetMachineJobsAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerExceptionHandler
	{
		Task HandleExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType);

		Task HandleCanceledExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType);
	}
	public interface ITickerExecutionTaskHandler
	{
		Task ExecuteTaskAsync(InternalFunctionContext context, bool isDue, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerFunctionBase
	{
	}
	public interface ITickerFunction : ITickerFunctionBase
	{
		Task ExecuteAsync(TickerFunctionContext context, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerFunction<TRequest> : ITickerFunctionBase
	{
		Task ExecuteAsync(TickerFunctionContext<TRequest> context, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerFunctionConcurrencyGate
	{
		SemaphoreSlim GetSemaphoreOrNull(string functionName, int maxConcurrency);
	}
	internal interface ITickerOptionsBuilder
	{
		internal TimeSpan TimeOutChecker { get; }
	}
	public interface ITickerPersistenceProvider<TTimeTicker, TCronTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		IAsyncEnumerable<TimeTickerEntity> QueueTimeTickers(TimeTickerEntity[] timeTickers, CancellationToken cancellationToken = default(CancellationToken));

		IAsyncEnumerable<TimeTickerEntity> QueueTimedOutTimeTickers(CancellationToken cancellationToken = default(CancellationToken));

		Task ReleaseAcquiredTimeTickers(Guid[] timeTickerIds, CancellationToken cancellationToken = default(CancellationToken));

		Task<TimeTickerEntity[]> GetEarliestTimeTickers(CancellationToken cancellationToken = default(CancellationToken));

		Task<int> UpdateTimeTicker(InternalFunctionContext functionContext, CancellationToken cancellationToken = default(CancellationToken));

		Task<byte[]> GetTimeTickerRequest(Guid id, CancellationToken cancellationToken);

		Task UpdateTimeTickersWithUnifiedContext(Guid[] timeTickerIds, InternalFunctionContext functionContext, CancellationToken cancellationToken = default(CancellationToken));

		Task<TimeTickerEntity[]> AcquireImmediateTimeTickersAsync(Guid[] ids, CancellationToken cancellationToken = default(CancellationToken));

		Task MigrateDefinedCronTickers((string Function, string Expression)[] cronTickers, CancellationToken cancellationToken = default(CancellationToken));

		Task<CronTickerEntity[]> GetAllCronTickerExpressions(CancellationToken cancellationToken);

		Task ReleaseDeadNodeTimeTickerResources(string instanceIdentifier, CancellationToken cancellationToken = default(CancellationToken));

		Task<CronTickerOccurrenceEntity<TCronTicker>> GetEarliestAvailableCronOccurrence(Guid[] ids, CancellationToken cancellationToken = default(CancellationToken));

		IAsyncEnumerable<CronTickerOccurrenceEntity<TCronTicker>> QueueCronTickerOccurrences((DateTime Key, InternalManagerContext[] Items) cronTickerOccurrences, CancellationToken cancellationToken = default(CancellationToken));

		IAsyncEnumerable<CronTickerOccurrenceEntity<TCronTicker>> QueueTimedOutCronTickerOccurrences(CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateCronTickerOccurrence(InternalFunctionContext functionContext, CancellationToken cancellationToken = default(CancellationToken));

		Task ReleaseAcquiredCronTickerOccurrences(Guid[] occurrenceIds, CancellationToken cancellationToken = default(CancellationToken));

		Task<byte[]> GetCronTickerOccurrenceRequest(Guid tickerId, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateCronTickerOccurrencesWithUnifiedContext(Guid[] timeTickerIds, InternalFunctionContext functionContext, CancellationToken cancellationToken = default(CancellationToken));

		Task ReleaseDeadNodeOccurrenceResources(string instanceIdentifier, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> SkipStaleCronOccurrencesAsync(TimeSpan staleThreshold, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(0);
		}

		ITickerQueryable<TTimeTicker> TimeTickersQuery();

		ITickerQueryable<TCronTicker> CronTickersQuery();

		ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> CronTickerOccurrencesQuery();

		Task<TTimeTicker> GetTimeTickerById(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<TTimeTicker[]> GetTimeTickers(Expression<Func<TTimeTicker, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<TTimeTicker>> GetTimeTickersPaginated(Expression<Func<TTimeTicker, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> AddTimeTickers(TTimeTicker[] tickers, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> UpdateTimeTickers(TTimeTicker[] tickers, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> RemoveTimeTickers(Guid[] tickerIds, CancellationToken cancellationToken = default(CancellationToken));

		Task<TCronTicker> GetCronTickerById(Guid id, CancellationToken cancellationToken);

		Task<TCronTicker[]> GetCronTickers(Expression<Func<TCronTicker, bool>> predicate, CancellationToken cancellationToken);

		Task<PaginationResult<TCronTicker>> GetCronTickersPaginated(Expression<Func<TCronTicker, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> InsertCronTickers(TCronTicker[] tickers, CancellationToken cancellationToken);

		Task<int> UpdateCronTickers(TCronTicker[] cronTicker, CancellationToken cancellationToken);

		Task<int> RemoveCronTickers(Guid[] cronTickerIds, CancellationToken cancellationToken);

		Task<CronTickerOccurrenceEntity<TCronTicker>[]> GetAllCronTickerOccurrences(Expression<Func<CronTickerOccurrenceEntity<TCronTicker>, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<CronTickerOccurrenceEntity<TCronTicker>>> GetAllCronTickerOccurrencesPaginated(Expression<Func<CronTickerOccurrenceEntity<TCronTicker>, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> InsertCronTickerOccurrences(CronTickerOccurrenceEntity<TCronTicker>[] cronTickerOccurrences, CancellationToken cancellationToken);

		Task<int> RemoveCronTickerOccurrences(Guid[] cronTickerOccurrences, CancellationToken cancellationToken);

		Task<CronTickerOccurrenceEntity<TCronTicker>[]> AcquireImmediateCronOccurrencesAsync(Guid[] occurrenceIds, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerQDispatcher
	{
		bool IsEnabled { get; }

		Task DispatchAsync(InternalFunctionContext[] contexts, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITickerQHostScheduler
	{
		bool IsRunning { get; }

		Task StartAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task StopAsync(CancellationToken cancellationToken = default(CancellationToken));

		void RestartIfNeeded(DateTime? dateTime);

		void Restart();
	}
	internal interface ITickerQNotificationHubSender
	{
		Task AddCronTickerNotifyAsync(object cronTicker);

		Task UpdateCronTickerNotifyAsync(object cronTicker);

		Task RemoveCronTickerNotifyAsync(Guid id);

		Task AddTimeTickerNotifyAsync(Guid id);

		Task AddTimeTickersBatchNotifyAsync();

		Task UpdateTimeTickerNotifyAsync(object timeTicker);

		Task RemoveTimeTickerNotifyAsync(Guid id);

		void UpdateActiveThreads(string activeThreads);

		void UpdateNextOccurrence(DateTime? nextOccurrence);

		void UpdateHostStatus(bool active);

		void UpdateHostException(string exceptionMessage);

		Task UpdateNodeHeartBeatAsync(JsonElement nodeHeartBeat);

		Task AddCronOccurrenceAsync(Guid groupId, object occurrence);

		Task UpdateCronOccurrenceAsync(Guid groupId, object occurrence);

		Task UpdateTimeTickerFromInternalFunctionContext<TTimeTickerEntity>(InternalFunctionContext internalFunctionContext) where TTimeTickerEntity : TimeTickerEntity<TTimeTickerEntity>, new();

		Task UpdateCronOccurrenceFromInternalFunctionContext<TCronTickerEntity>(InternalFunctionContext internalFunctionContext) where TCronTickerEntity : CronTickerEntity, new();

		Task CanceledTickerNotifyAsync(Guid id);
	}
	internal interface ITickerQRedisContext
	{
		IDistributedCache DistributedCache { get; }

		bool HasRedisConnection { get; }

		Task<TResult[]> GetOrSetArrayAsync<TResult>(string cacheKey, Func<CancellationToken, Task<TResult[]>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;

		Task<string[]> GetDeadNodesAsync();

		Task NotifyNodeAliveAsync();
	}
	public interface ITickerQTaskScheduler
	{
		bool IsFrozen { get; }

		int ActiveWorkers { get; }

		int TotalQueuedTasks { get; }

		bool IsDisposed { get; }

		ValueTask QueueAsync(Func<CancellationToken, Task> work, TickerTaskPriority priority, CancellationToken cancellationToken = default(CancellationToken));

		void Freeze();

		void Resume();

		string GetDiagnostics();

		Task<bool> WaitForRunningTasksAsync(TimeSpan? timeout = null);
	}
	public interface ITickerQueryable<TEntity>
	{
		ITickerQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

		ITickerQueryable<TEntity> WithRelated(params TickerRelation[] relations);

		ITickerQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);

		ITickerQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector);

		ITickerQueryable<TEntity> Skip(int count);

		ITickerQueryable<TEntity> Take(int count);

		ITickerQueryable<TEntity> AsNoTracking();

		Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<PaginationResult<TEntity>> ToPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken));
	}
	public static class TickerQueryableExtensions
	{
		public static Task<TEntity> FindAsync<TEntity>(this ITickerQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			return queryable.Where(predicate).FirstOrDefaultAsync(cancellationToken);
		}

		public static Task<int> CountAsync<TEntity>(this ITickerQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			return queryable.Where(predicate).CountAsync(cancellationToken);
		}

		public static async Task<bool> AnyAsync<TEntity>(this ITickerQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await queryable.Where(predicate).CountAsync(cancellationToken) > 0;
		}
	}
}
namespace TickerQ.Utilities.Interfaces.Managers
{
	public interface ICronTickerManager<TCronTicker> where TCronTicker : CronTickerEntity
	{
		Task<TickerResult<TCronTicker>> AddAsync(TCronTicker entity, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TCronTicker>> UpdateAsync(TCronTicker cronTicker, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TCronTicker>> DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<List<TCronTicker>>> AddBatchAsync(List<TCronTicker> entities, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<List<TCronTicker>>> UpdateBatchAsync(List<TCronTicker> cronTickers, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TCronTicker>> DeleteBatchAsync(List<Guid> ids, CancellationToken cancellationToken = default(CancellationToken));
	}
	internal interface IInternalTickerManager
	{
		Task<(TimeSpan TimeRemaining, InternalFunctionContext[] Functions)> GetNextTickers(CancellationToken cancellationToken = default(CancellationToken));

		Task ReleaseAcquiredResources(InternalFunctionContext[] context, CancellationToken cancellationToken = default(CancellationToken));

		Task SetTickersInProgress(InternalFunctionContext[] context, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateTickerAsync(InternalFunctionContext context, CancellationToken cancellationToken = default(CancellationToken));

		Task<T> GetRequestAsync<T>(Guid tickerId, TickerType type, CancellationToken cancellationToken = default(CancellationToken));

		Task<T> GetRequestAsync<T>(Guid tickerId, TickerType type, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default(CancellationToken));

		Task<InternalFunctionContext[]> RunTimedOutTickers(CancellationToken cancellationToken = default(CancellationToken));

		Task MigrateDefinedCronTickers((string, string)[] cronExpressions, CancellationToken cancellationToken = default(CancellationToken));

		Task DeleteTicker(Guid tickerId, TickerType type, CancellationToken cancellationToken = default(CancellationToken));

		Task ReleaseDeadNodeResources(string instanceIdentifier, CancellationToken cancellationToken = default(CancellationToken));

		Task UpdateSkipTimeTickersWithUnifiedContextAsync(InternalFunctionContext[] context, CancellationToken cancellationToken = default(CancellationToken));

		Task<int> SkipStaleCronOccurrencesAsync(TimeSpan staleThreshold, CancellationToken cancellationToken = default(CancellationToken));
	}
	public interface ITimeTickerManager<TTimeTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>
	{
		Task<TickerResult<TTimeTicker>> AddAsync(TTimeTicker entity, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TTimeTicker>> UpdateAsync(TTimeTicker timeTicker, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TTimeTicker>> DeleteAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<List<TTimeTicker>>> AddBatchAsync(List<TTimeTicker> entities, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<List<TTimeTicker>>> UpdateBatchAsync(List<TTimeTicker> timeTickers, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TTimeTicker>> DeleteBatchAsync(List<Guid> ids, CancellationToken cancellationToken = default(CancellationToken));

		Task<TickerResult<TTimeTicker>> AddAsync<TFunction>(DateTime? executionTime = null, CancellationToken cancellationToken = default(CancellationToken)) where TFunction : class, ITickerFunction;

		Task<TickerResult<TTimeTicker>> AddAsync<TFunction, TRequest>(DateTime? executionTime, TRequest request, CancellationToken cancellationToken = default(CancellationToken)) where TFunction : class, ITickerFunction<TRequest>;
	}
}
namespace TickerQ.Utilities.Instrumentation
{
	public abstract class TickerQBaseLoggerInstrumentation
	{
		private readonly ILogger _logger;

		protected readonly string InstanceIdentifier;

		protected TickerQBaseLoggerInstrumentation(ILogger logger, string instanceIdentifier)
		{
			_logger = logger;
			InstanceIdentifier = instanceIdentifier;
		}

		public abstract Activity StartJobActivity(string activityName, InternalFunctionContext context);

		public virtual void LogJobEnqueued(string jobType, string functionName, Guid jobId, string enqueuedFrom = null)
		{
			_logger.LogInformation("TickerQ Job enqueued: {JobType} - {Function} ({JobId}) from {EnqueuedFrom}", jobType, functionName, jobId, enqueuedFrom ?? "Unknown");
		}

		public virtual void LogJobStarted(Guid jobId, string functionName, string jobType)
		{
			_logger.LogInformation("TickerQ Job started: {JobType} - {Function} ({JobId})", jobType, functionName, jobId);
		}

		public virtual void LogJobCompleted(Guid jobId, string functionName, long executionTimeMs, bool success)
		{
			_logger.LogInformation("TickerQ Job completed: {Function} ({JobId}) in {ExecutionTime}ms - Success: {Success}", functionName, jobId, executionTimeMs, success);
		}

		public virtual void LogJobFailed(Guid jobId, string functionName, Exception exception, int retryCount)
		{
			_logger.LogError(exception, "TickerQ Job failed: {Function} ({JobId}) - Retry {RetryCount} - {Error}", functionName, jobId, retryCount, exception.Message);
		}

		public virtual void LogJobAttemptFailed(Guid jobId, string functionName, int attempt, int maxRetries, long elapsedMs, Exception exception)
		{
			if (attempt == 0)
			{
				_logger.LogError(exception, "Attempt of {Function} failed in {ElapsedMs}ms — {Error}", functionName, elapsedMs, exception.Message);
			}
			else
			{
				_logger.LogError(exception, "Retry {Attempt} of {MaxRetries} for {Function} failed in {ElapsedMs}ms — {Error}", attempt, maxRetries, functionName, elapsedMs, exception.Message);
			}
		}

		public virtual void LogJobRetryScheduled(Guid jobId, string functionName, int nextAttempt, int maxRetries, int intervalSeconds)
		{
			_logger.LogWarning("Retrying in {IntervalSeconds}s (retry {NextAttempt} of {MaxRetries})…", intervalSeconds, nextAttempt, maxRetries);
		}

		public virtual void LogJobCancelled(Guid jobId, string functionName, string reason)
		{
			_logger.LogWarning("TickerQ Job cancelled: {Function} ({JobId}) - {Reason}", functionName, jobId, reason);
		}

		public virtual void LogJobSkipped(Guid jobId, string functionName, string reason)
		{
			_logger.LogInformation("TickerQ Job skipped: {Function} ({JobId}) - {Reason}", functionName, jobId, reason);
		}

		public virtual void LogSeedingDataStarted(string seedingDataType)
		{
			_logger.LogInformation("TickerQ start seeding data: {TickerType} ({EnvironmentName})", seedingDataType, InstanceIdentifier);
		}

		public virtual void LogSeedingDataCompleted(string seedingDataType)
		{
			_logger.LogInformation("TickerQ completed seeding data: {TickerType} ({EnvironmentName})", seedingDataType, InstanceIdentifier);
		}

		public virtual void LogRequestDeserializationFailure(string requestType, string functionName, Guid tickerId, TickerType type, Exception exception)
		{
			_logger.LogError("Failed to deserialize request to {RequestType} - {TickerId} - {TickerType}: {Exception}", requestType, tickerId, type, exception);
		}
	}
	internal interface ITickerQInstrumentation
	{
		Activity? StartJobActivity(string activityName, InternalFunctionContext context);

		void LogJobEnqueued(string jobType, string functionName, Guid jobId, string? enqueuedFrom = null);

		void LogJobCompleted(Guid jobId, string functionName, long executionTimeMs, bool success);

		void LogJobFailed(Guid jobId, string functionName, Exception exception, int retryCount);

		void LogJobAttemptFailed(Guid jobId, string functionName, int attempt, int maxRetries, long elapsedMs, Exception exception);

		void LogJobRetryScheduled(Guid jobId, string functionName, int nextAttempt, int maxRetries, int intervalSeconds);

		void LogJobCancelled(Guid jobId, string functionName, string reason);

		void LogJobSkipped(Guid jobId, string functionName, string reason);

		void LogSeedingDataStarted(string seedingDataType);

		void LogSeedingDataCompleted(string seedingDataType);

		void LogRequestDeserializationFailure(string requestType, string functionName, Guid tickerId, TickerType type, Exception exception);
	}
	public sealed class LoggerInstrumentation : TickerQBaseLoggerInstrumentation, ITickerQInstrumentation
	{
		public LoggerInstrumentation(ILogger<LoggerInstrumentation> logger, SchedulerOptionsBuilder optionsBuilder)
			: base(logger, optionsBuilder.NodeIdentifier)
		{
		}

		public override Activity StartJobActivity(string activityName, InternalFunctionContext context)
		{
			return null;
		}
	}
}
namespace TickerQ.Utilities.Infrastructure
{
	public sealed class InMemoryTickerQueryable<TEntity> : ITickerQueryable<TEntity>
	{
		private readonly Func<CancellationToken, Task<List<TEntity>>> _loader;

		private readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> _pipeline;

		public InMemoryTickerQueryable(Func<CancellationToken, Task<List<TEntity>>> loader)
			: this(loader, (Func<IQueryable<TEntity>, IQueryable<TEntity>>)((IQueryable<TEntity> q) => q))
		{
		}

		private InMemoryTickerQueryable(Func<CancellationToken, Task<List<TEntity>>> loader, Func<IQueryable<TEntity>, IQueryable<TEntity>> pipeline)
		{
			_loader = loader;
			_pipeline = pipeline;
		}

		private InMemoryTickerQueryable<TEntity> Append(Func<IQueryable<TEntity>, IQueryable<TEntity>> step)
		{
			Func<IQueryable<TEntity>, IQueryable<TEntity>> currentPipeline = _pipeline;
			return new InMemoryTickerQueryable<TEntity>(_loader, (IQueryable<TEntity> q) => step(currentPipeline(q)));
		}

		public ITickerQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
		{
			return Append((IQueryable<TEntity> q) => q.Where(predicate));
		}

		public ITickerQueryable<TEntity> WithRelated(params TickerRelation[] relations)
		{
			return this;
		}

		public ITickerQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
		{
			return Append((IQueryable<TEntity> q) => q.OrderBy(keySelector));
		}

		public ITickerQueryable<TEntity> OrderByDescending<TKey>(Expression<Func<TEntity, TKey>> keySelector)
		{
			return Append((IQueryable<TEntity> q) => q.OrderByDescending(keySelector));
		}

		public ITickerQueryable<TEntity> Skip(int count)
		{
			return Append((IQueryable<TEntity> q) => q.Skip(count));
		}

		public ITickerQueryable<TEntity> Take(int count)
		{
			return Append((IQueryable<TEntity> q) => q.Take(count));
		}

		public ITickerQueryable<TEntity> AsNoTracking()
		{
			return this;
		}

		public async Task<TEntity[]> ToArrayAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TEntity> source = await _loader(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return _pipeline(source.AsQueryable()).ToArray();
		}

		public async Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TEntity> source = await _loader(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return _pipeline(source.AsQueryable()).FirstOrDefault();
		}

		public async Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			List<TEntity> source = await _loader(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return _pipeline(source.AsQueryable()).Count();
		}

		public async Task<PaginationResult<TEntity>> ToPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
		{
			pageNumber = Math.Max(1, pageNumber);
			pageSize = Math.Clamp(pageSize, 1, 1000);
			List<TEntity> source = await _loader(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			IQueryable<TEntity> source2 = _pipeline(source.AsQueryable());
			return new PaginationResult<TEntity>(totalCount: source2.Count(), items: source2.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray(), pageNumber: pageNumber, pageSize: pageSize);
		}
	}
	public sealed class TickerDashboardDataService<TTimeTicker, TCronTicker> : ITickerDashboardDataService<TTimeTicker, TCronTicker> where TTimeTicker : TimeTickerEntity<TTimeTicker>, new() where TCronTicker : CronTickerEntity, new()
	{
		private readonly ITickerPersistenceProvider<TTimeTicker, TCronTicker> _persistence;

		private readonly ITickerQHostScheduler _hostScheduler;

		private readonly ITickerQTaskScheduler _taskScheduler;

		private readonly SchedulerOptionsBuilder _schedulerOptions;

		public TickerDashboardDataService(ITickerPersistenceProvider<TTimeTicker, TCronTicker> persistence, ITickerQHostScheduler hostScheduler = null, ITickerQTaskScheduler taskScheduler = null, SchedulerOptionsBuilder schedulerOptions = null)
		{
			_persistence = persistence ?? throw new ArgumentNullException("persistence");
			_hostScheduler = hostScheduler;
			_taskScheduler = taskScheduler;
			_schedulerOptions = schedulerOptions;
		}

		public async Task<PaginationResult<TimeTickerFlatDto>> GetTimeTickersFlatAsync(TimeTickerQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (filter == null)
			{
				filter = new TimeTickerQueryFilter();
			}
			ITickerQueryable<TTimeTicker> tickerQueryable = _persistence.TimeTickersQuery().WithRelated(default(TickerRelation));
			if (!string.IsNullOrWhiteSpace(filter.FunctionName))
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.Function.Contains(filter.FunctionName));
			}
			TickerStatus[] statuses = filter.Statuses;
			if (statuses != null && statuses.Length > 0)
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => filter.Statuses.Contains(x.Status, null));
			}
			if (filter.ScheduledFrom.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.ExecutionTime >= filter.ScheduledFrom);
			}
			if (filter.ScheduledTo.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.ExecutionTime <= filter.ScheduledTo);
			}
			if (filter.ExecutedFrom.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.ExecutedAt >= filter.ExecutedFrom);
			}
			if (filter.ExecutedTo.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.ExecutedAt <= filter.ExecutedTo);
			}
			tickerQueryable = tickerQueryable.Where((TTimeTicker x) => x.ParentId == null);
			if (filter.HasChildren.HasValue)
			{
				tickerQueryable = ((!filter.HasChildren.Value) ? tickerQueryable.Where((TTimeTicker x) => !x.Children.Any()) : tickerQueryable.Where((TTimeTicker x) => x.Children.Any()));
			}
			if (!string.IsNullOrWhiteSpace(filter.Search))
			{
				string search = filter.Search;
				tickerQueryable = tickerQueryable.Where((TTimeTicker x) => (x.Function != null && x.Function.Contains(search)) || (x.Description != null && x.Description.Contains(search)));
			}
			tickerQueryable = ApplyTimeTickerSort(tickerQueryable, filter.SortBy, filter.SortDescending);
			PaginationResult<TTimeTicker> paginationResult = await tickerQueryable.ToPaginatedAsync(filter.PageNumber, filter.PageSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return new PaginationResult<TimeTickerFlatDto>(paginationResult.Items.Select(MapTimeTicker).ToArray(), paginationResult.TotalCount, paginationResult.PageNumber, paginationResult.PageSize);
		}

		public async Task<TimeTickerFlatDto> GetTimeTickerByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			TTimeTicker val = await _persistence.GetTimeTickerById(id, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return (val == null) ? null : MapTimeTicker(val);
		}

		public async Task<IList<TimeTickerFlatDto>> GetTimeTickerChildrenAsync(Guid parentId, CancellationToken cancellationToken = default(CancellationToken))
		{
			return (await (from x in _persistence.TimeTickersQuery()
				where x.ParentId == parentId
				orderby x.ExecutionTime
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Select(MapTimeTicker).ToList();
		}

		public async Task<PaginationResult<CronTickerFlatDto>> GetCronTickersFlatAsync(CronTickerQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (filter == null)
			{
				filter = new CronTickerQueryFilter();
			}
			ITickerQueryable<TCronTicker> tickerQueryable = _persistence.CronTickersQuery();
			if (!string.IsNullOrWhiteSpace(filter.FunctionName))
			{
				tickerQueryable = tickerQueryable.Where((TCronTicker x) => x.Function.Contains(filter.FunctionName));
			}
			if (!string.IsNullOrWhiteSpace(filter.Expression))
			{
				tickerQueryable = tickerQueryable.Where((TCronTicker x) => x.Expression.Contains(filter.Expression));
			}
			if (!string.IsNullOrWhiteSpace(filter.Search))
			{
				string search = filter.Search;
				tickerQueryable = tickerQueryable.Where((TCronTicker x) => (x.Function != null && x.Function.Contains(search)) || (x.Expression != null && x.Expression.Contains(search)) || (x.Description != null && x.Description.Contains(search)));
			}
			tickerQueryable = ApplyCronTickerSort(tickerQueryable, filter.SortBy, filter.SortDescending);
			PaginationResult<TCronTicker> result = await tickerQueryable.ToPaginatedAsync(filter.PageNumber, filter.PageSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			List<CronTickerFlatDto> items = new List<CronTickerFlatDto>(result.Items.Count());
			foreach (TCronTicker item in result.Items)
			{
				List<CronTickerFlatDto> list = items;
				list.Add(await MapCronTickerWithLastRunAsync(item, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
			}
			TickerStatus[] lastRunStatuses = filter.LastRunStatuses;
			if (lastRunStatuses != null && lastRunStatuses.Length > 0)
			{
				items = items.Where((CronTickerFlatDto x) => x.LastRunStatus.HasValue && filter.LastRunStatuses.Contains(x.LastRunStatus.Value, null)).ToList();
			}
			return new PaginationResult<CronTickerFlatDto>(items.ToArray(), result.TotalCount, result.PageNumber, result.PageSize);
		}

		public async Task<CronTickerFlatDto> GetCronTickerByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			TCronTicker val = await _persistence.GetCronTickerById(id, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return (val != null) ? (await MapCronTickerWithLastRunAsync(val, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) : null;
		}

		public async Task<PaginationResult<CronOccurrenceFlatDto>> GetCronOccurrencesFlatAsync(Guid cronTickerId, CronOccurrenceQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (filter == null)
			{
				filter = new CronOccurrenceQueryFilter();
			}
			ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> tickerQueryable = from x in _persistence.CronTickerOccurrencesQuery()
				where x.CronTickerId == cronTickerId
				select x;
			TickerStatus[] statuses = filter.Statuses;
			if (statuses != null && statuses.Length > 0)
			{
				tickerQueryable = tickerQueryable.Where((CronTickerOccurrenceEntity<TCronTicker> x) => filter.Statuses.Contains(x.Status, null));
			}
			if (filter.ScheduledFrom.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime >= filter.ScheduledFrom);
			}
			if (filter.ScheduledTo.HasValue)
			{
				tickerQueryable = tickerQueryable.Where((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime <= filter.ScheduledTo);
			}
			tickerQueryable = ApplyOccurrenceSort(tickerQueryable, filter.SortBy, filter.SortDescending);
			PaginationResult<CronTickerOccurrenceEntity<TCronTicker>> result = await tickerQueryable.ToPaginatedAsync(filter.PageNumber, filter.PageSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			string parentFunction = (await _persistence.GetCronTickerById(cronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))?.Function;
			return new PaginationResult<CronOccurrenceFlatDto>(result.Items.Select((CronTickerOccurrenceEntity<TCronTicker> x) => MapOccurrence(x, parentFunction)).ToArray(), result.TotalCount, result.PageNumber, result.PageSize);
		}

		public async Task<CronOccurrenceFlatDto> GetCronOccurrenceByIdAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			CronTickerOccurrenceEntity<TCronTicker> entity = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.Id == id
				select x).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (entity == null)
			{
				return null;
			}
			return MapOccurrence(entity, (await _persistence.GetCronTickerById(entity.CronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))?.Function);
		}

		public async Task<PaginationResult<ExecutionFlatDto>> GetExecutionsFlatAsync(ExecutionQueryFilter filter, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (filter == null)
			{
				filter = new ExecutionQueryFilter();
			}
			bool num = filter.Types == null || filter.Types.Length == 0 || filter.Types.Contains(ExecutionType.TimeTicker, null);
			bool includeCron = filter.Types == null || filter.Types.Length == 0 || filter.Types.Contains(ExecutionType.CronOccurrence, null);
			List<ExecutionFlatDto> all = new List<ExecutionFlatDto>();
			if (num)
			{
				all.AddRange((await ApplyExecutionTimeFilters(from x in _persistence.TimeTickersQuery().WithRelated(default(TickerRelation))
					where x.ExecutedAt != null && x.ParentId == null
					select x, filter).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Select(MapTimeTickerToExecution));
			}
			if (includeCron)
			{
				CronTickerOccurrenceEntity<TCronTicker>[] array = await ApplyExecutionOccurrenceFilters(from x in _persistence.CronTickerOccurrencesQuery()
					where x.ExecutedAt != null
					select x, filter).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				foreach (CronTickerOccurrenceEntity<TCronTicker> item in array)
				{
					TCronTicker val = await _persistence.GetCronTickerById(item.CronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					all.Add(MapOccurrenceToExecution(item, val?.Function, val?.Retries ?? 0));
				}
			}
			TickerStatus[] statuses = filter.Statuses;
			if (statuses != null && statuses.Length > 0)
			{
				all = all.Where((ExecutionFlatDto x) => filter.Statuses.Contains(x.Status, null)).ToList();
			}
			if (!string.IsNullOrWhiteSpace(filter.FunctionName))
			{
				all = all.Where((ExecutionFlatDto x) => x.FunctionName != null && x.FunctionName.Contains(filter.FunctionName)).ToList();
			}
			if (!string.IsNullOrWhiteSpace(filter.Search))
			{
				string search = filter.Search;
				all = all.Where((ExecutionFlatDto x) => (x.FunctionName != null && x.FunctionName.Contains(search)) || (x.ExceptionMessage != null && x.ExceptionMessage.Contains(search))).ToList();
			}
			List<ExecutionFlatDto> list = ApplyExecutionSort(all, filter.SortBy, filter.SortDescending);
			return new PaginationResult<ExecutionFlatDto>(totalCount: list.Count, items: list.Skip((Math.Max(1, filter.PageNumber) - 1) * filter.PageSize).Take(filter.PageSize).ToArray(), pageNumber: filter.PageNumber, pageSize: filter.PageSize);
		}

		public Task<byte[]> GetTimeTickerRequestAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _persistence.GetTimeTickerRequest(id, cancellationToken);
		}

		public Task<byte[]> GetCronOccurrenceRequestAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _persistence.GetCronTickerOccurrenceRequest(id, cancellationToken);
		}

		public async Task<IList<(TickerStatus Status, int Count)>> GetOverallStatusesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			TTimeTicker[] timeTickers = await _persistence.TimeTickersQuery().ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			CronTickerOccurrenceEntity<TCronTicker>[] source = await _persistence.CronTickerOccurrencesQuery().ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			IEnumerable<TickerStatus> source2 = timeTickers.Select((TTimeTicker x) => x.Status).Concat(source.Select((CronTickerOccurrenceEntity<TCronTicker> x) => x.Status));
			TickerStatus[] values = Enum.GetValues<TickerStatus>();
			Dictionary<TickerStatus, int> counts = (from x in source2
				group x by x).ToDictionary((IGrouping<TickerStatus, TickerStatus> g) => g.Key, (IGrouping<TickerStatus, TickerStatus> g) => g.Count());
			return values.Select((TickerStatus s) => (s: s, counts.GetValueOrDefault(s, 0))).ToList();
		}

		public Task<IList<(string NodeName, int JobCount)>> GetNodeJobsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult((IList<(string, int)>)Array.Empty<(string, int)>());
		}

		public async Task<IList<TimeTickerFlatDto>> GetUpcomingTickersAsync(int count, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (count <= 0)
			{
				count = 10;
			}
			DateTime now = DateTime.UtcNow;
			TTimeTicker[] upcomingTime = await (from x in _persistence.TimeTickersQuery()
				where x.ExecutionTime > now && ((int)x.Status == 0 || (int)x.Status == 1)
				orderby x.ExecutionTime
				select x).Take(count).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			CronTickerOccurrenceEntity<TCronTicker>[] array = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.ExecutionTime > now && ((int)x.Status == 0 || (int)x.Status == 1)
				orderby x.ExecutionTime
				select x).Take(count).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			List<TimeTickerFlatDto> combined = new List<TimeTickerFlatDto>(upcomingTime.Length + array.Length);
			combined.AddRange(upcomingTime.Select(MapTimeTicker));
			CronTickerOccurrenceEntity<TCronTicker>[] array2 = array;
			foreach (CronTickerOccurrenceEntity<TCronTicker> occ in array2)
			{
				combined.Add(MapCronOccurrenceToTimeTickerDto(occ, await _persistence.GetCronTickerById(occ.CronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)));
			}
			return combined.OrderBy((TimeTickerFlatDto x) => x.ScheduledFor).Take(count).ToList();
		}

		private static TimeTickerFlatDto MapCronOccurrenceToTimeTickerDto(CronTickerOccurrenceEntity<TCronTicker> occ, TCronTicker parent)
		{
			string text = parent?.Function ?? string.Empty;
			TickerTaskPriority priority = TickerTaskPriority.Normal;
			if (TickerFunctionProvider.TickerFunctions.TryGetValue(text, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
			{
				priority = value.Item2;
			}
			return new TimeTickerFlatDto
			{
				Id = occ.Id,
				FunctionName = text,
				Status = occ.Status,
				ScheduledFor = occ.ExecutionTime,
				ElapsedTime = occ.ElapsedTime,
				Retries = (parent?.Retries ?? 0),
				RetryCount = occ.RetryCount,
				Priority = priority,
				CreatedAt = occ.CreatedAt,
				ChildCount = 0,
				ExceptionMessage = occ.ExceptionMessage,
				SkippedReason = null,
				ParentId = null,
				RunCondition = null,
				ExecutedAt = occ.ExecutedAt,
				Description = ((parent != null) ? ("Cron: " + parent.Expression) : null)
			};
		}

		public async Task<IList<ExecutionFlatDto>> GetRecentActivityAsync(int count, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (count <= 0)
			{
				count = 10;
			}
			TTimeTicker[] recentTime = await (from x in _persistence.TimeTickersQuery()
				where x.ExecutedAt != null
				orderby x.ExecutedAt descending
				select x).Take(count).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			CronTickerOccurrenceEntity<TCronTicker>[] array = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.ExecutedAt != null
				orderby x.ExecutedAt descending
				select x).Take(count).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			List<ExecutionFlatDto> combined = new List<ExecutionFlatDto>(recentTime.Length + array.Length);
			combined.AddRange(recentTime.Select(MapTimeTickerToExecution));
			CronTickerOccurrenceEntity<TCronTicker>[] array2 = array;
			foreach (CronTickerOccurrenceEntity<TCronTicker> occ in array2)
			{
				TCronTicker val = await _persistence.GetCronTickerById(occ.CronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				combined.Add(MapOccurrenceToExecution(occ, val?.Function, val?.Retries ?? 0));
			}
			return combined.OrderByDescending((ExecutionFlatDto x) => x.ExecutedAt).Take(count).ToList();
		}

		private static TimeTickerFlatDto MapTimeTicker(TTimeTicker e)
		{
			TickerTaskPriority priority = TickerTaskPriority.Normal;
			int childCount = e.Children?.Count ?? 0;
			if (TickerFunctionProvider.TickerFunctions.TryGetValue(e.Function, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
			{
				priority = value.Item2;
			}
			return new TimeTickerFlatDto
			{
				Id = e.Id,
				FunctionName = e.Function,
				Status = e.Status,
				ScheduledFor = e.ExecutionTime,
				ElapsedTime = e.ElapsedTime,
				Retries = e.Retries,
				RetryCount = e.RetryCount,
				Priority = priority,
				CreatedAt = e.CreatedAt,
				ChildCount = childCount,
				ExceptionMessage = e.ExceptionMessage,
				SkippedReason = e.SkippedReason,
				ParentId = e.ParentId,
				RunCondition = e.RunCondition,
				ExecutedAt = e.ExecutedAt,
				Description = e.Description
			};
		}

		private async Task<CronTickerFlatDto> MapCronTickerWithLastRunAsync(TCronTicker c, CancellationToken cancellationToken)
		{
			TickerTaskPriority priority = TickerTaskPriority.Normal;
			if (TickerFunctionProvider.TickerFunctions.TryGetValue(c.Function, out (string, TickerTaskPriority, TickerFunctionDelegate, int) value))
			{
				priority = value.Item2;
			}
			CronTickerOccurrenceEntity<TCronTicker>[] array = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.CronTickerId == c.Id
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			CronTickerOccurrenceEntity<TCronTicker> cronTickerOccurrenceEntity = (from x in array
				where x.ExecutedAt.HasValue
				orderby x.ExecutedAt descending
				select x).FirstOrDefault();
			return new CronTickerFlatDto
			{
				Id = c.Id,
				FunctionName = c.Function,
				Expression = c.Expression,
				Description = c.Description,
				Retries = c.Retries,
				Priority = priority,
				CreatedAt = c.CreatedAt,
				OccurrenceCount = array.Length,
				LastRunStatus = cronTickerOccurrenceEntity?.Status,
				LastRunAt = cronTickerOccurrenceEntity?.ExecutedAt,
				IsEnabled = c.IsEnabled,
				IsSystemPaused = c.IsSystemPaused
			};
		}

		private static CronOccurrenceFlatDto MapOccurrence(CronTickerOccurrenceEntity<TCronTicker> e, string parentFunction)
		{
			return new CronOccurrenceFlatDto
			{
				Id = e.Id,
				CronTickerId = e.CronTickerId,
				FunctionName = parentFunction,
				Status = e.Status,
				ScheduledFor = e.ExecutionTime,
				ElapsedTime = e.ElapsedTime,
				RetryCount = e.RetryCount,
				ExceptionMessage = e.ExceptionMessage,
				SkippedReason = e.SkippedReason,
				ExecutedAt = e.ExecutedAt,
				CreatedAt = e.CreatedAt
			};
		}

		private static ExecutionFlatDto MapTimeTickerToExecution(TTimeTicker e)
		{
			return new ExecutionFlatDto
			{
				Id = e.Id,
				Type = ExecutionType.TimeTicker,
				FunctionName = e.Function,
				Status = e.Status,
				ScheduledFor = e.ExecutionTime.GetValueOrDefault(),
				ElapsedTime = e.ElapsedTime,
				RetryCount = e.RetryCount,
				Retries = e.Retries,
				ExceptionMessage = e.ExceptionMessage,
				SkippedReason = e.SkippedReason,
				ExecutedAt = e.ExecutedAt,
				ChildCount = (e.Children?.Count ?? 0)
			};
		}

		private static ExecutionFlatDto MapOccurrenceToExecution(CronTickerOccurrenceEntity<TCronTicker> e, string functionName, int parentRetries)
		{
			return new ExecutionFlatDto
			{
				Id = e.Id,
				Type = ExecutionType.CronOccurrence,
				FunctionName = functionName,
				Status = e.Status,
				ScheduledFor = e.ExecutionTime,
				ElapsedTime = e.ElapsedTime,
				RetryCount = e.RetryCount,
				Retries = parentRetries,
				ExceptionMessage = e.ExceptionMessage,
				SkippedReason = e.SkippedReason,
				ExecutedAt = e.ExecutedAt
			};
		}

		private static ITickerQueryable<TTimeTicker> ApplyTimeTickerSort(ITickerQueryable<TTimeTicker> query, string sortBy, bool descending)
		{
			sortBy = (sortBy ?? "createdat").ToLowerInvariant();
			return sortBy switch
			{
				"status" => descending ? query.OrderByDescending((TTimeTicker x) => x.Status) : query.OrderBy((TTimeTicker x) => x.Status), 
				"functionname" => descending ? query.OrderByDescending((TTimeTicker x) => x.Function) : query.OrderBy((TTimeTicker x) => x.Function), 
				"executedat" => descending ? query.OrderByDescending((TTimeTicker x) => x.ExecutedAt) : query.OrderBy((TTimeTicker x) => x.ExecutedAt), 
				"executiontime" => descending ? query.OrderByDescending((TTimeTicker x) => x.ElapsedTime) : query.OrderBy((TTimeTicker x) => x.ElapsedTime), 
				"scheduledfor" => descending ? query.OrderByDescending((TTimeTicker x) => x.ExecutionTime) : query.OrderBy((TTimeTicker x) => x.ExecutionTime), 
				_ => descending ? query.OrderByDescending((TTimeTicker x) => x.CreatedAt) : query.OrderBy((TTimeTicker x) => x.CreatedAt), 
			};
		}

		private static ITickerQueryable<TCronTicker> ApplyCronTickerSort(ITickerQueryable<TCronTicker> query, string sortBy, bool descending)
		{
			sortBy = (sortBy ?? "createdat").ToLowerInvariant();
			if (!(sortBy == "functionname"))
			{
				if (sortBy == "expression")
				{
					return descending ? query.OrderByDescending((TCronTicker x) => x.Expression) : query.OrderBy((TCronTicker x) => x.Expression);
				}
				return descending ? query.OrderByDescending((TCronTicker x) => x.CreatedAt) : query.OrderBy((TCronTicker x) => x.CreatedAt);
			}
			return descending ? query.OrderByDescending((TCronTicker x) => x.Function) : query.OrderBy((TCronTicker x) => x.Function);
		}

		private static ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> ApplyOccurrenceSort(ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> query, string sortBy, bool descending)
		{
			sortBy = (sortBy ?? "scheduledfor").ToLowerInvariant();
			return sortBy switch
			{
				"status" => descending ? query.OrderByDescending((CronTickerOccurrenceEntity<TCronTicker> x) => x.Status) : query.OrderBy((CronTickerOccurrenceEntity<TCronTicker> x) => x.Status), 
				"executedat" => descending ? query.OrderByDescending((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutedAt) : query.OrderBy((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutedAt), 
				"executiontime" => descending ? query.OrderByDescending((CronTickerOccurrenceEntity<TCronTicker> x) => x.ElapsedTime) : query.OrderBy((CronTickerOccurrenceEntity<TCronTicker> x) => x.ElapsedTime), 
				"createdat" => descending ? query.OrderByDescending((CronTickerOccurrenceEntity<TCronTicker> x) => x.CreatedAt) : query.OrderBy((CronTickerOccurrenceEntity<TCronTicker> x) => x.CreatedAt), 
				_ => descending ? query.OrderByDescending((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime) : query.OrderBy((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime), 
			};
		}

		private static List<ExecutionFlatDto> ApplyExecutionSort(List<ExecutionFlatDto> items, string sortBy, bool descending)
		{
			sortBy = (sortBy ?? "executedat").ToLowerInvariant();
			return (sortBy switch
			{
				"status" => (IEnumerable<ExecutionFlatDto>)(descending ? items.OrderByDescending((ExecutionFlatDto x) => x.Status) : items.OrderBy((ExecutionFlatDto x) => x.Status)), 
				"functionname" => descending ? items.OrderByDescending((ExecutionFlatDto x) => x.FunctionName) : items.OrderBy((ExecutionFlatDto x) => x.FunctionName), 
				"duration" => descending ? items.OrderByDescending((ExecutionFlatDto x) => x.ElapsedTime) : items.OrderBy((ExecutionFlatDto x) => x.ElapsedTime), 
				"scheduledfor" => descending ? items.OrderByDescending((ExecutionFlatDto x) => x.ScheduledFor) : items.OrderBy((ExecutionFlatDto x) => x.ScheduledFor), 
				_ => descending ? items.OrderByDescending((ExecutionFlatDto x) => x.ExecutedAt) : items.OrderBy((ExecutionFlatDto x) => x.ExecutedAt), 
			}).ToList();
		}

		private static ITickerQueryable<TTimeTicker> ApplyExecutionTimeFilters(ITickerQueryable<TTimeTicker> query, ExecutionQueryFilter filter)
		{
			if (filter.ExecutedFrom.HasValue)
			{
				query = query.Where((TTimeTicker x) => x.ExecutedAt >= filter.ExecutedFrom);
			}
			if (filter.ExecutedTo.HasValue)
			{
				query = query.Where((TTimeTicker x) => x.ExecutedAt <= filter.ExecutedTo);
			}
			return query;
		}

		private static ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> ApplyExecutionOccurrenceFilters(ITickerQueryable<CronTickerOccurrenceEntity<TCronTicker>> query, ExecutionQueryFilter filter)
		{
			if (filter.ExecutedFrom.HasValue)
			{
				query = query.Where((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutedAt >= filter.ExecutedFrom);
			}
			if (filter.ExecutedTo.HasValue)
			{
				query = query.Where((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutedAt <= filter.ExecutedTo);
			}
			return query;
		}

		public Task<IList<NodeDto>> GetNodesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult((IList<NodeDto>)Array.Empty<NodeDto>());
		}

		private static NodeHealthStatus ComputeHealth(DateTime? lastHeartbeat, DateTime now)
		{
			if (!lastHeartbeat.HasValue)
			{
				return NodeHealthStatus.Down;
			}
			TimeSpan timeSpan = now - lastHeartbeat.Value;
			if (timeSpan <= TimeSpan.FromMinutes(5L))
			{
				return NodeHealthStatus.Healthy;
			}
			if (timeSpan <= TimeSpan.FromMinutes(30L))
			{
				return NodeHealthStatus.Degraded;
			}
			return NodeHealthStatus.Down;
		}

		public Task<IList<FunctionInfoDto>> GetNodeFunctionsAsync(string nodeName, CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetAllFunctionsAsync(cancellationToken);
		}

		public Task<IList<FunctionInfoDto>> GetAllFunctionsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			FrozenDictionary<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> tickerFunctions = TickerFunctionProvider.TickerFunctions;
			FrozenDictionary<string, (string RequestType, string RequestExampleJson)> infos = TickerFunctionProvider.TickerFunctionRequestInfos;
			return Task.FromResult((IList<FunctionInfoDto>)tickerFunctions.Select((KeyValuePair<string, (string cronExpression, TickerTaskPriority Priority, TickerFunctionDelegate Delegate, int MaxConcurrency)> kvp) =>
			{
				string key = kvp.Key;
				string requestType = null;
				string requestExample = null;
				if (infos != null && infos.TryGetValue(key, out (string, string) value))
				{
					(requestType, requestExample) = value;
				}
				return new FunctionInfoDto
				{
					FunctionName = key,
					RequestType = requestType,
					RequestExample = requestExample,
					Priority = kvp.Value.Priority,
					CronExpression = kvp.Value.cronExpression
				};
			}).ToList());
		}

		public Task<HostStatusDto> GetHostStatusAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return Task.FromResult(new HostStatusDto
			{
				IsRunning = (_hostScheduler?.IsRunning ?? false),
				ActiveThreads = (_taskScheduler?.ActiveWorkers ?? 0),
				MaxConcurrency = (_schedulerOptions?.MaxConcurrency ?? 0)
			});
		}

		public async Task<NextTickerDto> GetNextTickerAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime now = DateTime.UtcNow;
			TTimeTicker[] nextTime = await (from x in _persistence.TimeTickersQuery()
				where x.ExecutionTime > now && ((int)x.Status == 0 || (int)x.Status == 1)
				orderby x.ExecutionTime
				select x).Take(1).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			CronTickerOccurrenceEntity<TCronTicker>[] source = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.ExecutionTime > now && ((int)x.Status == 0 || (int)x.Status == 1)
				orderby x.ExecutionTime
				select x).Take(1).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			TTimeTicker val = nextTime.FirstOrDefault();
			CronTickerOccurrenceEntity<TCronTicker> co = source.FirstOrDefault();
			if (val == null && co == null)
			{
				return new NextTickerDto();
			}
			if (val != null && (co == null || val.ExecutionTime <= co.ExecutionTime))
			{
				return new NextTickerDto
				{
					Id = val.Id,
					FunctionName = val.Function,
					ScheduledFor = val.ExecutionTime,
					Type = ExecutionType.TimeTicker
				};
			}
			TCronTicker val2 = ((co != null) ? (await _persistence.GetCronTickerById(co.CronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) : null);
			TCronTicker val3 = val2;
			return new NextTickerDto
			{
				Id = co.Id,
				FunctionName = val3?.Function,
				ScheduledFor = co.ExecutionTime,
				Type = ExecutionType.CronOccurrence
			};
		}

		public async Task<IList<GraphBucketDto>> GetTimeTickersGraphAsync(int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime date = DateTime.UtcNow.Date;
			DateTime from = date.AddDays(-Math.Abs(pastDays));
			DateTime to = date.AddDays(Math.Abs(futureDays)).AddDays(1.0);
			TTimeTicker[] source = await (from x in _persistence.TimeTickersQuery()
				where (x.ExecutedAt != null && x.ExecutedAt >= @from && x.ExecutedAt < to) || (x.ExecutionTime != null && x.ExecutionTime >= @from && x.ExecutionTime < to)
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return BuildBuckets(from, to, from x in source
				select (bucketDate: (x.ExecutedAt ?? x.ExecutionTime.GetValueOrDefault()).Date, Status: x.Status) into t
				where t.bucketDate >= @from && t.bucketDate < to
				select t);
		}

		public async Task<IList<GraphBucketDto>> GetCronTickersGraphAsync(int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime date = DateTime.UtcNow.Date;
			DateTime from = date.AddDays(-Math.Abs(pastDays));
			DateTime to = date.AddDays(Math.Abs(futureDays)).AddDays(1.0);
			CronTickerOccurrenceEntity<TCronTicker>[] source = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.ExecutionTime >= @from && x.ExecutionTime < to
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return BuildBuckets(from, to, source.Select((CronTickerOccurrenceEntity<TCronTicker> x) => (Date: x.ExecutionTime.Date, Status: x.Status)));
		}

		public async Task<IList<GraphBucketDto>> GetCronTickerGraphByIdAsync(Guid cronTickerId, int pastDays, int futureDays, CancellationToken cancellationToken = default(CancellationToken))
		{
			DateTime date = DateTime.UtcNow.Date;
			DateTime from = date.AddDays(-Math.Abs(pastDays));
			DateTime to = date.AddDays(Math.Abs(futureDays)).AddDays(1.0);
			CronTickerOccurrenceEntity<TCronTicker>[] source = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.CronTickerId == cronTickerId && x.ExecutionTime >= @from && x.ExecutionTime < to
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			return BuildBuckets(from, to, source.Select((CronTickerOccurrenceEntity<TCronTicker> x) => (Date: x.ExecutionTime.Date, Status: x.Status)));
		}

		public async Task<IList<GraphBucketDto>> GetCronOccurrencesGraphAsync(Guid cronTickerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			CronTickerOccurrenceEntity<TCronTicker>[] array = await (from x in _persistence.CronTickerOccurrencesQuery()
				where x.CronTickerId == cronTickerId
				select x).ToArrayAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (array.Length == 0)
			{
				return new List<GraphBucketDto>();
			}
			DateTime date = array.Min((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime).Date;
			DateTime to = array.Max((CronTickerOccurrenceEntity<TCronTicker> x) => x.ExecutionTime).Date.AddDays(1.0);
			return BuildBuckets(date, to, array.Select((CronTickerOccurrenceEntity<TCronTicker> x) => (Date: x.ExecutionTime.Date, Status: x.Status)));
		}

		public async Task<bool> ToggleCronTickerAsync(Guid cronTickerId, bool isEnabled, CancellationToken cancellationToken = default(CancellationToken))
		{
			TCronTicker val = await _persistence.GetCronTickerById(cronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (val == null)
			{
				return false;
			}
			val.IsEnabled = isEnabled;
			val.UpdatedAt = DateTime.UtcNow;
			await _persistence.UpdateCronTickers(new TCronTicker[1] { val }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_hostScheduler != null)
			{
				_hostScheduler.Restart();
			}
			return true;
		}

		public async Task RunCronTickerOnDemandAsync(Guid cronTickerId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (await _persistence.GetCronTickerById(cronTickerId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false) == null)
			{
				throw new InvalidOperationException($"Cron ticker {cronTickerId} not found");
			}
			CronTickerOccurrenceEntity<TCronTicker> cronTickerOccurrenceEntity = new CronTickerOccurrenceEntity<TCronTicker>
			{
				Id = Guid.NewGuid(),
				CronTickerId = cronTickerId,
				Status = TickerStatus.Idle,
				ExecutionTime = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};
			await _persistence.InsertCronTickerOccurrences(new CronTickerOccurrenceEntity<TCronTicker>[1] { cronTickerOccurrenceEntity }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			if (_hostScheduler != null)
			{
				_hostScheduler.Restart();
			}
		}

		public bool CancelTicker(Guid tickerId)
		{
			return TickerCancellationTokenManager.RequestTickerCancellationById(tickerId);
		}

		public async Task StartHostAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_hostScheduler != null)
			{
				await _hostScheduler.StartAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public async Task StopHostAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_hostScheduler != null)
			{
				await _hostScheduler.StopAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		public void RestartHost()
		{
			_hostScheduler?.Restart();
		}

		private static IList<GraphBucketDto> BuildBuckets(DateTime from, DateTime to, IEnumerable<(DateTime date, TickerStatus status)> points)
		{
			TickerStatus[] values = Enum.GetValues<TickerStatus>();
			Dictionary<DateTime, Dictionary<TickerStatus, int>> dictionary = (from p in points
				group p by p.date).ToDictionary((IGrouping<DateTime, (DateTime date, TickerStatus status)> g) => g.Key, (IGrouping<DateTime, (DateTime date, TickerStatus status)> g) => (from x in g
				group x by x.status).ToDictionary((IGrouping<TickerStatus, (DateTime date, TickerStatus status)> sg) => sg.Key, (IGrouping<TickerStatus, (DateTime date, TickerStatus status)> sg) => sg.Count()));
			List<GraphBucketDto> list = new List<GraphBucketDto>();
			DateTime dateTime = from;
			while (dateTime < to)
			{
				dictionary.TryGetValue(dateTime, out var statusCounts);
				list.Add(new GraphBucketDto
				{
					Date = dateTime,
					Counts = values.Select((TickerStatus s) => (s: s, (statusCounts != null && statusCounts.TryGetValue(s, out var value)) ? value : 0)).ToArray()
				});
				dateTime = dateTime.AddDays(1.0);
			}
			return list;
		}
	}
}
namespace TickerQ.Utilities.Exceptions
{
	public sealed class SdkOfflineSkipException : Exception
	{
		public SdkOfflineSkipException(string message)
			: base(message)
		{
		}
	}
	public class TickerValidatorException : Exception
	{
		public TickerValidatorException(string message)
			: base(message)
		{
		}
	}
}
namespace TickerQ.Utilities.Enums
{
	internal enum CoreNotifyActionType
	{
		NotifyNextOccurence,
		NotifyHostStatus,
		NotifyHostExceptionMessage,
		NotifyThreadCount
	}
	public enum RunCondition
	{
		OnSuccess,
		OnFailure,
		OnCancelled,
		OnFailureOrCancelled,
		OnAnyCompletedStatus,
		InProgress
	}
	public enum TickerQStartMode
	{
		Immediate,
		Manual
	}
	public enum TickerRelation
	{
		Children,
		ChildrenDeep,
		CronTicker
	}
	public enum TickerStatus
	{
		Idle,
		Queued,
		InProgress,
		Done,
		DueDone,
		Failed,
		Cancelled,
		Skipped
	}
	public enum TickerTaskPriority
	{
		LongRunning,
		High,
		Normal,
		Low
	}
	public enum TickerType
	{
		CronTickerOccurrence,
		TimeTicker
	}
}
namespace TickerQ.Utilities.Entities
{
	public class CronTickerEntity : BaseTickerEntity
	{
		public virtual string Expression { get; set; }

		public virtual byte[] Request { get; set; }

		public virtual int Retries { get; set; }

		public virtual int[] RetryIntervals { get; set; }

		public virtual bool IsEnabled { get; set; } = true;

		public virtual bool IsSystemPaused { get; set; }
	}
	public class CronTickerOccurrenceEntity<TCronTicker> where TCronTicker : CronTickerEntity
	{
		public virtual Guid Id { get; set; }

		public virtual TickerStatus Status { get; set; }

		public virtual string LockHolder { get; set; }

		public virtual DateTime ExecutionTime { get; set; }

		public virtual Guid CronTickerId { get; set; }

		public virtual DateTime? LockedAt { get; set; }

		public virtual DateTime? ExecutedAt { get; set; }

		public virtual TCronTicker CronTicker { get; set; }

		public virtual string ExceptionMessage { get; set; }

		public virtual string SkippedReason { get; set; }

		public virtual long ElapsedTime { get; set; }

		public virtual int RetryCount { get; set; }

		public virtual DateTime CreatedAt { get; set; }

		public virtual DateTime UpdatedAt { get; set; }
	}
	public class TimeTickerEntity : TimeTickerEntity<TimeTickerEntity>
	{
	}
	public class TimeTickerEntity<TTicker> : BaseTickerEntity where TTicker : TimeTickerEntity<TTicker>
	{
		[JsonInclude]
		public virtual TickerStatus Status { get; internal set; }

		[JsonInclude]
		public virtual string LockHolder { get; internal set; }

		public virtual byte[] Request { get; set; }

		public virtual DateTime? ExecutionTime { get; set; }

		[JsonInclude]
		public virtual DateTime? LockedAt { get; internal set; }

		[JsonInclude]
		public virtual DateTime? ExecutedAt { get; internal set; }

		[JsonInclude]
		public virtual string ExceptionMessage { get; internal set; }

		[JsonInclude]
		public virtual string SkippedReason { get; internal set; }

		[JsonInclude]
		public virtual long ElapsedTime { get; internal set; }

		public virtual int Retries { get; set; }

		[JsonInclude]
		public virtual int RetryCount { get; internal set; }

		public virtual int[] RetryIntervals { get; set; }

		[JsonInclude]
		public virtual Guid? ParentId { get; internal set; }

		[JsonIgnore]
		public virtual TTicker Parent { get; internal set; }

		public virtual ICollection<TTicker> Children { get; set; } = new List<TTicker>();

		public virtual RunCondition? RunCondition { get; set; }
	}
}
namespace TickerQ.Utilities.Entities.BaseEntity
{
	public class BaseTickerEntity
	{
		public virtual Guid Id { get; set; } = Guid.NewGuid();

		public virtual string Function { get; set; }

		public virtual string Description { get; set; }

		[JsonInclude]
		public virtual string InitIdentifier { get; internal set; }

		[JsonInclude]
		public virtual DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;

		[JsonInclude]
		public virtual DateTime UpdatedAt { get; internal set; } = DateTime.UtcNow;
	}
}
namespace TickerQ.Utilities.DashboardDtos
{
	public class CronOccurrenceFlatDto
	{
		public Guid Id { get; set; }

		public Guid CronTickerId { get; set; }

		public string FunctionName { get; set; }

		public TickerStatus Status { get; set; }

		public DateTime ScheduledFor { get; set; }

		public long ElapsedTime { get; set; }

		public int RetryCount { get; set; }

		public string ExceptionMessage { get; set; }

		public string SkippedReason { get; set; }

		public DateTime? ExecutedAt { get; set; }

		public DateTime CreatedAt { get; set; }
	}
	public class CronOccurrenceQueryFilter
	{
		public TickerStatus[] Statuses { get; set; }

		public DateTime? ScheduledFrom { get; set; }

		public DateTime? ScheduledTo { get; set; }

		public string SortBy { get; set; }

		public bool SortDescending { get; set; } = true;

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;
	}
	public class CronOccurrenceTickerGraphData
	{
		public DateTime Date { get; set; }

		public Tuple<int, int>[] Results { get; set; }
	}
	public class CronTickerFlatDto
	{
		public Guid Id { get; set; }

		public string FunctionName { get; set; }

		public string Expression { get; set; }

		public string Description { get; set; }

		public int Retries { get; set; }

		public TickerTaskPriority Priority { get; set; }

		public DateTime CreatedAt { get; set; }

		public int OccurrenceCount { get; set; }

		public TickerStatus? LastRunStatus { get; set; }

		public DateTime? LastRunAt { get; set; }

		public bool IsEnabled { get; set; }

		public bool IsSystemPaused { get; set; }
	}
	public class CronTickerQueryFilter
	{
		public string FunctionName { get; set; }

		public TickerStatus[] LastRunStatuses { get; set; }

		public string Expression { get; set; }

		public string Search { get; set; }

		public string SortBy { get; set; }

		public bool SortDescending { get; set; }

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;
	}
	public enum ExecutionType
	{
		TimeTicker,
		CronOccurrence
	}
	public class ExecutionFlatDto
	{
		public Guid Id { get; set; }

		public ExecutionType Type { get; set; }

		public string FunctionName { get; set; }

		public TickerStatus Status { get; set; }

		public DateTime ScheduledFor { get; set; }

		public long ElapsedTime { get; set; }

		public int RetryCount { get; set; }

		public int Retries { get; set; }

		public string ExceptionMessage { get; set; }

		public string SkippedReason { get; set; }

		public DateTime? ExecutedAt { get; set; }

		public int ChildCount { get; set; }
	}
	public class ExecutionQueryFilter
	{
		public ExecutionType[] Types { get; set; }

		public TickerStatus[] Statuses { get; set; }

		public string FunctionName { get; set; }

		public DateTime? ExecutedFrom { get; set; }

		public DateTime? ExecutedTo { get; set; }

		public string Search { get; set; }

		public string SortBy { get; set; }

		public bool SortDescending { get; set; } = true;

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;
	}
	public class FunctionInfoDto
	{
		public string FunctionName { get; set; }

		public string RequestType { get; set; }

		public string RequestExample { get; set; }

		public TickerTaskPriority Priority { get; set; }

		public string CronExpression { get; set; }
	}
	public class HostStatusDto
	{
		public bool IsRunning { get; set; }

		public int ActiveThreads { get; set; }

		public int MaxConcurrency { get; set; }
	}
	public class NextTickerDto
	{
		public Guid? Id { get; set; }

		public string FunctionName { get; set; }

		public DateTime? ScheduledFor { get; set; }

		public ExecutionType Type { get; set; }
	}
	public class GraphBucketDto
	{
		public DateTime Date { get; set; }

		public (TickerStatus Status, int Count)[] Counts { get; set; }
	}
	public enum NodeHealthStatus
	{
		Healthy,
		Degraded,
		Down
	}
	public class NodeDto
	{
		public string NodeName { get; set; }

		public DateTime? LastHeartbeat { get; set; }

		public NodeHealthStatus HealthStatus { get; set; }

		public int FunctionCount { get; set; }

		public int ActiveJobs { get; set; }
	}
	public class TickerGraphData
	{
		public DateTime Date { get; set; }

		public Tuple<int, int>[] Results { get; set; }
	}
	public class TimeTickerFlatDto
	{
		public Guid Id { get; set; }

		public string FunctionName { get; set; }

		public TickerStatus Status { get; set; }

		public DateTime? ScheduledFor { get; set; }

		public long ElapsedTime { get; set; }

		public int Retries { get; set; }

		public int RetryCount { get; set; }

		public TickerTaskPriority Priority { get; set; }

		public DateTime CreatedAt { get; set; }

		public int ChildCount { get; set; }

		public string ExceptionMessage { get; set; }

		public string SkippedReason { get; set; }

		public Guid? ParentId { get; set; }

		public RunCondition? RunCondition { get; set; }

		public DateTime? ExecutedAt { get; set; }

		public string Description { get; set; }
	}
	public class TimeTickerQueryFilter
	{
		public string FunctionName { get; set; }

		public TickerStatus[] Statuses { get; set; }

		public DateTime? ScheduledFrom { get; set; }

		public DateTime? ScheduledTo { get; set; }

		public DateTime? ExecutedFrom { get; set; }

		public DateTime? ExecutedTo { get; set; }

		public bool? ParentOnly { get; set; }

		public bool? HasChildren { get; set; }

		public string Search { get; set; }

		public string SortBy { get; set; }

		public bool SortDescending { get; set; } = true;

		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 20;
	}
	public class UpdateCronTickerRequest
	{
		public string Function { get; set; } = string.Empty;

		public string Expression { get; set; } = string.Empty;

		public string Request { get; set; }

		public int? Retries { get; set; }

		public string Description { get; set; }

		public int[] Intervals { get; set; }
	}
}
namespace TickerQ.Utilities.Base
{
	[AttributeUsage(AttributeTargets.Method)]
	public class TickerFunctionAttribute : Attribute
	{
		public TickerFunctionAttribute(string functionName, string cronExpression = null, TickerTaskPriority taskPriority = TickerTaskPriority.Normal, int maxConcurrency = 0)
		{
		}

		public TickerFunctionAttribute(string functionName, TickerTaskPriority taskPriority, int maxConcurrency = 0)
		{
		}
	}
	public class TickerFunctionContext<TRequest> : TickerFunctionContext
	{
		public readonly TRequest Request;

		public TickerFunctionContext(TickerFunctionContext tickerFunctionContext, TRequest request)
		{
			Request = request;
			Id = tickerFunctionContext.Id;
			ParentId = tickerFunctionContext.ParentId;
			Type = tickerFunctionContext.Type;
			RetryCount = tickerFunctionContext.RetryCount;
			IsDue = tickerFunctionContext.IsDue;
			ScheduledFor = tickerFunctionContext.ScheduledFor;
			RequestCancelOperationAction = tickerFunctionContext.RequestCancelOperationAction;
			CronOccurrenceOperations = tickerFunctionContext.CronOccurrenceOperations;
			FunctionName = tickerFunctionContext.FunctionName;
		}
	}
	public class TickerFunctionContext
	{
		internal AsyncServiceScope ServiceScope { get; set; }

		internal Action RequestCancelOperationAction { get; set; }

		public Guid Id { get; internal set; }

		public Guid? ParentId { get; internal set; }

		public TickerType Type { get; internal set; }

		public int RetryCount { get; internal set; }

		internal int Retries { get; set; }

		internal int[] RetryIntervals { get; set; }

		public bool IsDue { get; internal set; }

		public DateTime ScheduledFor { get; internal set; }

		public string FunctionName { get; internal set; }

		public CronOccurrenceOperations CronOccurrenceOperations { get; internal set; }

		public void RequestCancellation()
		{
			RequestCancelOperationAction();
		}

		internal void SetServiceScope(AsyncServiceScope serviceScope)
		{
			ServiceScope = serviceScope;
		}
	}
	public class CronOccurrenceOperations
	{
		internal Action SkipIfAlreadyRunningAction { get; set; }

		public void SkipIfAlreadyRunning()
		{
			SkipIfAlreadyRunningAction();
		}
	}
	[AttributeUsage(AttributeTargets.Constructor)]
	public class TickerQConstructorAttribute : Attribute
	{
	}
}
