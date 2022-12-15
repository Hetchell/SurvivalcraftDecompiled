using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x020002C6 RID: 710
	public static class PerformanceManager
	{
		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06001417 RID: 5143 RVA: 0x0009B928 File Offset: 0x00099B28
		public static float? LongTermAverageFrameTime
		{
			get
			{
				return PerformanceManager.m_longTermAverageFrameTime;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06001418 RID: 5144 RVA: 0x0009B92F File Offset: 0x00099B2F
		public static float AverageFrameTime
		{
			get
			{
				return PerformanceManager.m_averageFrameTime.Value;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06001419 RID: 5145 RVA: 0x0009B93B File Offset: 0x00099B3B
		public static float AverageCpuFrameTime
		{
			get
			{
				return PerformanceManager.m_averageCpuFrameTime.Value;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600141A RID: 5146 RVA: 0x0009B947 File Offset: 0x00099B47
		public static long TotalMemoryUsed
		{
			get
			{
				return PerformanceManager.m_totalMemoryUsed;
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x0600141B RID: 5147 RVA: 0x0009B94E File Offset: 0x00099B4E
		public static long TotalGpuMemoryUsed
		{
			get
			{
				return PerformanceManager.m_totalGpuMemoryUsed;
			}
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x0009B958 File Offset: 0x00099B58
		static PerformanceManager()
		{
			PerformanceManager.m_stateMachine.AddState("PreMeasure", delegate
			{
				PerformanceManager.m_totalGameTime = 0.0;
			}, delegate
			{
				if (GameManager.Project != null)
				{
					PerformanceManager.m_totalGameTime += (double)Time.FrameDuration;
					if (PerformanceManager.m_totalGameTime > 60.0)
					{
						PerformanceManager.m_stateMachine.TransitionTo("Measuring");
					}
				}
			}, null);
			PerformanceManager.m_stateMachine.AddState("Measuring", delegate
			{
				PerformanceManager.m_totalFrameTime = 0.0;
				PerformanceManager.m_totalCpuFrameTime = 0.0;
				PerformanceManager.m_frameCount = 0;
			}, delegate
			{
				if (GameManager.Project != null)
				{
					if (ScreensManager.CurrentScreen != null && ScreensManager.CurrentScreen.GetType() == typeof(GameScreen))
					{
						//Console.Beep();
						float lastFrameTime = Program.LastFrameTime;
						float lastCpuFrameTime = Program.LastCpuFrameTime;
						if (lastFrameTime > 0f && lastFrameTime < 1f && lastCpuFrameTime > 0f && lastCpuFrameTime < 1f)
						{
							PerformanceManager.m_totalFrameTime += (double)lastFrameTime;
							PerformanceManager.m_totalCpuFrameTime += (double)lastCpuFrameTime;
							PerformanceManager.m_frameCount++;
						}
						if (PerformanceManager.m_totalFrameTime > 180.0)
						{
							PerformanceManager.m_stateMachine.TransitionTo("PostMeasure");
							return;
						}
					}
				}
				else
				{
					PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
				}
			}, null);
			PerformanceManager.m_stateMachine.AddState("PostMeasure", delegate
			{
				if (PerformanceManager.m_frameCount > 0)
				{
					PerformanceManager.m_longTermAverageFrameTime = new float?((float)(PerformanceManager.m_totalFrameTime / (double)PerformanceManager.m_frameCount));
					float num = (float)((int)MathUtils.Round(MathUtils.Round(PerformanceManager.m_totalFrameTime / (double)PerformanceManager.m_frameCount / 0.004999999888241291) * 0.004999999888241291 * 1000.0));
					float num2 = (float)((int)MathUtils.Round(MathUtils.Round(PerformanceManager.m_totalCpuFrameTime / (double)PerformanceManager.m_frameCount / 0.004999999888241291) * 0.004999999888241291 * 1000.0));
					AnalyticsManager.LogEvent("[PerformanceManager] Measurement", new AnalyticsParameter[]
					{
						new AnalyticsParameter("FrameCount", PerformanceManager.m_frameCount.ToString()),
						new AnalyticsParameter("AverageFrameTime", num.ToString() + "ms"),
						new AnalyticsParameter("AverageFrameCpuTime", num2.ToString() + "ms")
					});
					Log.Information(string.Concat(new string[]
					{
						"PerformanceManager Measurement: frames=",
						PerformanceManager.m_frameCount.ToString(),
						", avgFrameTime=",
						num.ToString(),
						"ms, avgFrameCpuTime=",
						num2.ToString(),
						"ms"
					}));
				}
			}, delegate
			{
				if (GameManager.Project == null)
				{
					PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
				}
			}, null);
			PerformanceManager.m_stateMachine.TransitionTo("PreMeasure");
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0009BA40 File Offset: 0x00099C40
		public static void Update()
		{
			PerformanceManager.m_averageFrameTime.AddSample(Program.LastFrameTime);
			PerformanceManager.m_averageCpuFrameTime.AddSample(Program.LastCpuFrameTime);
			if (Time.PeriodicEvent(1.0, 0.0))
			{
				PerformanceManager.m_totalMemoryUsed = GC.GetTotalMemory(false);
				PerformanceManager.m_totalGpuMemoryUsed = Display.GetGpuMemoryUsage();
			}
			PerformanceManager.m_stateMachine.Update();
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0009BAA4 File Offset: 0x00099CA4
		public static void Draw()
		{
			Vector2 vector = new Vector2(MathUtils.Round(MathUtils.Clamp(ScreensManager.RootWidget.GlobalScale, 1f, 4f)));
			Viewport viewport = Display.Viewport;
			if (SettingsManager.DisplayFpsCounter)
			{
				if (Time.PeriodicEvent(1.0, 0.0))
				{
					PerformanceManager.m_statsString = string.Format("CPUMEM {0:0}MB, GPUMEM {1:0}MB, CPU {2:0}%, FPS {3:0.0}", new object[]
					{
						(float)PerformanceManager.TotalMemoryUsed / 1024f / 1024f,
						(float)PerformanceManager.TotalGpuMemoryUsed / 1024f / 1024f,
						PerformanceManager.AverageCpuFrameTime / PerformanceManager.AverageFrameTime * 100f,
						1f / PerformanceManager.AverageFrameTime
					});
				}
				PerformanceManager.m_primitivesRenderer.FontBatch(BitmapFont.DebugFont, 0, null, null, null, SamplerState.PointClamp).QueueText(PerformanceManager.m_statsString, new Vector2((float)viewport.Width, 0f), 0f, Color.White, TextAnchor.Right, vector, Vector2.Zero, 0f);
			}
			if (SettingsManager.DisplayFpsRibbon)
			{
				float num = ((float)viewport.Width / vector.X > 480f) ? (vector.X * 2f) : vector.X;
				float num2 = (float)viewport.Height / -0.1f;
				float num3 = (float)(viewport.Height - 1);
				float s = 0.5f;
				int num4 = MathUtils.Max((int)((float)viewport.Width / num), 1);
				if (PerformanceManager.m_frameData == null || PerformanceManager.m_frameData.Length != num4)
				{
					PerformanceManager.m_frameData = new PerformanceManager.FrameData[num4];
					PerformanceManager.m_frameDataIndex = 0;
				}
				PerformanceManager.m_frameData[PerformanceManager.m_frameDataIndex] = new PerformanceManager.FrameData
				{
					CpuTime = Program.LastCpuFrameTime,
					TotalTime = Program.LastFrameTime
				};
				PerformanceManager.m_frameDataIndex = (PerformanceManager.m_frameDataIndex + 1) % PerformanceManager.m_frameData.Length;
				FlatBatch2D flatBatch2D = PerformanceManager.m_primitivesRenderer.FlatBatch(0, null, null, null);
				Color color = Color.Orange * s;
				Color color2 = Color.Red * s;
				for (int i = PerformanceManager.m_frameData.Length - 1; i >= 0; i--)
				{
					int num5 = (i - PerformanceManager.m_frameData.Length + 1 + PerformanceManager.m_frameDataIndex + PerformanceManager.m_frameData.Length) % PerformanceManager.m_frameData.Length;
					PerformanceManager.FrameData frameData = PerformanceManager.m_frameData[num5];
					float x = (float)i * num;
					float x2 = (float)(i + 1) * num;
					flatBatch2D.QueueQuad(new Vector2(x, num3), new Vector2(x2, num3 + frameData.CpuTime * num2), 0f, color);
					flatBatch2D.QueueQuad(new Vector2(x, num3 + frameData.CpuTime * num2), new Vector2(x2, num3 + frameData.TotalTime * num2), 0f, color2);
				}
				flatBatch2D.QueueLine(new Vector2(0f, num3 + 0.016666668f * num2), new Vector2((float)viewport.Width, num3 + 0.016666668f * num2), 0f, Color.Green);
			}
			else
			{
				PerformanceManager.m_frameData = null;
			}
			PerformanceManager.m_primitivesRenderer.Flush(true, int.MaxValue);
		}

		// Token: 0x04000DEF RID: 3567
		public static PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x04000DF0 RID: 3568
		public static RunningAverage m_averageFrameTime = new RunningAverage(1f);

		// Token: 0x04000DF1 RID: 3569
		public static RunningAverage m_averageCpuFrameTime = new RunningAverage(1f);

		// Token: 0x04000DF2 RID: 3570
		public static float? m_longTermAverageFrameTime;

		// Token: 0x04000DF3 RID: 3571
		public static long m_totalMemoryUsed;

		// Token: 0x04000DF4 RID: 3572
		public static long m_totalGpuMemoryUsed;

		// Token: 0x04000DF5 RID: 3573
		public static StateMachine m_stateMachine = new StateMachine();

		// Token: 0x04000DF6 RID: 3574
		public static double m_totalGameTime;

		// Token: 0x04000DF7 RID: 3575
		public static double m_totalFrameTime;

		// Token: 0x04000DF8 RID: 3576
		public static double m_totalCpuFrameTime;

		// Token: 0x04000DF9 RID: 3577
		public static int m_frameCount;

		// Token: 0x04000DFA RID: 3578
		public static string m_statsString = string.Empty;

		// Token: 0x04000DFB RID: 3579
		public static PerformanceManager.FrameData[] m_frameData;

		// Token: 0x04000DFC RID: 3580
		public static int m_frameDataIndex;

		// Token: 0x020004BF RID: 1215
		public struct FrameData
		{
			// Token: 0x0400178A RID: 6026
			public float CpuTime;

			// Token: 0x0400178B RID: 6027
			public float TotalTime;
		}
	}
}
