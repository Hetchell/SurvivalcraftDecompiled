using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Engine;

namespace Game
{
	// Token: 0x020002D6 RID: 726
	public struct Profiler : IDisposable
	{
		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06001483 RID: 5251 RVA: 0x0009F9CE File Offset: 0x0009DBCE
		public static int MaxNameLength
		{
			get
			{
				return Profiler.m_maxNameLength;
			}
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06001484 RID: 5252 RVA: 0x0009F9D8 File Offset: 0x0009DBD8
		public static ReadOnlyList<Profiler.Metric> Metrics
		{
			get
			{
				if (Profiler.m_sortNeeded)
				{
					Profiler.m_sortedMetrics.Sort((Profiler.Metric x, Profiler.Metric y) => string.CompareOrdinal(x.Name, y.Name));
					Profiler.m_sortNeeded = false;
				}
				return new ReadOnlyList<Profiler.Metric>(Profiler.m_sortedMetrics);
			}
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x0009FA28 File Offset: 0x0009DC28
		public Profiler(string name)
		{
			if (Profiler.Enabled)
			{
				if (!Profiler.m_metrics.TryGetValue(name, out this.m_metric))
				{
					this.m_metric = new Profiler.Metric();
					this.m_metric.Name = name;
					Profiler.m_maxNameLength = MathUtils.Max(Profiler.m_maxNameLength, name.Length);
					Profiler.m_metrics.Add(name, this.m_metric);
					Profiler.m_sortedMetrics.Add(this.m_metric);
					Profiler.m_sortNeeded = true;
				}
				this.m_startTicks = Stopwatch.GetTimestamp();
				return;
			}
			this.m_startTicks = 0L;
			this.m_metric = null;
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x0009FAC0 File Offset: 0x0009DCC0
		public void Dispose()
		{
			if (this.m_metric != null)
			{
				long num = Stopwatch.GetTimestamp() - this.m_startTicks;
				this.m_metric.TotalTicks += num;
				this.m_metric.MaxTicks = MathUtils.Max(this.m_metric.MaxTicks, num);
				this.m_metric.HitCount++;
				this.m_metric = null;
				return;
			}
			throw new InvalidOperationException("Profiler.Dispose called without a matching constructor.");
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x0009FB38 File Offset: 0x0009DD38
		public static void Sample()
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				float sample = (float)metric.TotalTicks / (float)Stopwatch.Frequency;
				metric.AverageHitCount.AddSample((float)metric.HitCount);
				metric.AverageTime.AddSample(sample);
				metric.HitCount = 0;
				metric.TotalTicks = 0L;
				metric.MaxTicks = 0L;
			}
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0009FBCC File Offset: 0x0009DDCC
		public static void ReportAverage(Profiler.Metric metric, StringBuilder text)
		{
			int num = Profiler.m_maxNameLength + 2;
			int length = text.Length;
			text.Append(metric.Name);
			text.Append('.', Math.Max(1, num - text.Length + length));
			text.AppendNumber(metric.AverageHitCount.Value, 2);
			text.Append("x");
			text.Append('.', Math.Max(1, num + 9 - text.Length + length));
			Profiler.FormatTimeSimple(text, metric.AverageTime.Value);
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x0009FC5C File Offset: 0x0009DE5C
		public static void ReportFrame(Profiler.Metric metric, StringBuilder text)
		{
			int num = Profiler.m_maxNameLength + 2;
			int length = text.Length;
			text.Append(metric.Name);
			text.Append('.', Math.Max(1, num - text.Length + length));
			Profiler.FormatTimeSimple(text, (float)metric.TotalTicks / (float)Stopwatch.Frequency);
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x0009FCB4 File Offset: 0x0009DEB4
		public static void ReportAverage(StringBuilder text)
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				Profiler.ReportAverage(metric, text);
				text.Append("\n");
			}
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x0009FD14 File Offset: 0x0009DF14
		public static void ReportFrame(StringBuilder text)
		{
			foreach (Profiler.Metric metric in Profiler.Metrics)
			{
				Profiler.ReportFrame(metric, text);
				text.Append("\n");
			}
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x0009FD74 File Offset: 0x0009DF74
		public static void FormatTimeSimple(StringBuilder text, float time)
		{
			text.AppendNumber(time * 1000f, 3);
			text.Append("ms");
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x0009FD90 File Offset: 0x0009DF90
		public static void FormatTime(StringBuilder text, float time)
		{
			if (time >= 1f)
			{
				text.AppendNumber(time, 2);
				text.Append("s");
				return;
			}
			if (time >= 0.1f)
			{
				text.AppendNumber(time * 1000f, 0);
				text.Append("ms");
				return;
			}
			if (time >= 0.01f)
			{
				text.AppendNumber(time * 1000f, 1);
				text.Append("ms");
				return;
			}
			if (time >= 0.001f)
			{
				text.AppendNumber(time * 1000f, 2);
				text.Append("ms");
				return;
			}
			if (time >= 0.0001f)
			{
				text.AppendNumber(time * 1000000f, 0);
				text.Append("us");
				return;
			}
			if (time >= 1E-05f)
			{
				text.AppendNumber(time * 1000000f, 1);
				text.Append("us");
				return;
			}
			if (time >= 1E-06f)
			{
				text.AppendNumber(time * 1000000f, 2);
				text.Append("us");
				return;
			}
			if (time >= 1E-07f)
			{
				text.AppendNumber(time * 1E+09f, 0);
				text.Append("ns");
				return;
			}
			if (time >= 1E-08f)
			{
				text.AppendNumber(time * 1E+09f, 1);
				text.Append("ns");
				return;
			}
			text.AppendNumber(time * 1E+09f, 2);
			text.Append("ns");
		}

		// Token: 0x04000E90 RID: 3728
		public static Dictionary<string, Profiler.Metric> m_metrics = new Dictionary<string, Profiler.Metric>();

		// Token: 0x04000E91 RID: 3729
		public static List<Profiler.Metric> m_sortedMetrics = new List<Profiler.Metric>();

		// Token: 0x04000E92 RID: 3730
		public static int m_maxNameLength;

		// Token: 0x04000E93 RID: 3731
		public static bool m_sortNeeded;

		// Token: 0x04000E94 RID: 3732
		public long m_startTicks;

		// Token: 0x04000E95 RID: 3733
		public Profiler.Metric m_metric;

		// Token: 0x04000E96 RID: 3734
		public static bool Enabled = true;

		// Token: 0x020004CF RID: 1231
		public class Metric
		{
			// Token: 0x040017B4 RID: 6068
			public string Name;

			// Token: 0x040017B5 RID: 6069
			public int HitCount;

			// Token: 0x040017B6 RID: 6070
			public long TotalTicks;

			// Token: 0x040017B7 RID: 6071
			public long MaxTicks;

			// Token: 0x040017B8 RID: 6072
			public readonly RunningAverage AverageHitCount = new RunningAverage(5f);

			// Token: 0x040017B9 RID: 6073
			public readonly RunningAverage AverageTime = new RunningAverage(5f);
		}
	}
}
