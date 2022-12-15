using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Engine;
using Engine.Graphics;
using Engine.Input;

namespace Game
{
	// Token: 0x020002D7 RID: 727
	public static class Program
	{
		// Token: 0x17000314 RID: 788
		// (get) Token: 0x0600148F RID: 5263 RVA: 0x0009FF08 File Offset: 0x0009E108
		// (set) Token: 0x06001490 RID: 5264 RVA: 0x0009FF0F File Offset: 0x0009E10F
		public static float LastFrameTime { get; set; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06001491 RID: 5265 RVA: 0x0009FF17 File Offset: 0x0009E117
		// (set) Token: 0x06001492 RID: 5266 RVA: 0x0009FF1E File Offset: 0x0009E11E
		public static float LastCpuFrameTime { get; set; }

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06001493 RID: 5267 RVA: 0x0009FF28 File Offset: 0x0009E128
		// (remove) Token: 0x06001494 RID: 5268 RVA: 0x0009FF5C File Offset: 0x0009E15C
		public static event Action<Uri> HandleUri;

		// Token: 0x06001495 RID: 5269 RVA: 0x0009FF90 File Offset: 0x0009E190
		[STAThread]
		public static void Main()
		{
			/*
			 * Currently, there are a quite few issues, but overall it is now playable in terms of the 'exploration' side of things.
			 * -> Do not use magma on sand, the magma behaviour is now broken. Bug report:
			 *		>> Magma will destroy sand<unintended>
			 * -> Falling blocks: When blocks are placed, the player can suddenly have the ability to no-clip with zero damage taken. 
			 *		>> This is applied to not only the player, but all living entities<breaks game>
			 *		>> Only fixes itself when program is terminated and rerun. 
			 * -> Vines are generated in wrong places(i.e. no trees to complement)
			 * -> Random tree dissection
			 * -> Unable to light explosive barrels(incendiary or normal). 
			 *		>> But they can be lit on fire by other fire sources(i.e. burning bushes)
			 */
			Console.WriteLine("hello world");
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			Log.RemoveAllLogSinks();
			Log.AddLogSink(new GameLogSink());
			Window.HandleUri += Program.HandleUriHandler;
			Window.Deactivated += Program.DeactivatedHandler;
			Window.Frame += Program.FrameHandler;
			Window.Closed += ModsManager.SaveSettings;
			Window.UnhandledException += delegate (UnhandledExceptionInfo e)
			{
				ExceptionManager.ReportExceptionToUser("Unhandled exception.", e.Exception);
				e.IsHandled = true;
			};
			//Window.Run(1080, 780, Converter(WindowMode.Resizable), "生存战争2.2插件版V1.33正式版");
			Window.Run(1080, 780, Engine.WindowMode.Resizable, "Modded SurvivalCraft(2.2)[Decompiled Ver]");
        }

        private static Engine.WindowMode Converter(Game.WindowMode windowmode)
        {
            switch (windowmode)
            {
                case WindowMode.Resizable:
                    return Engine.WindowMode.Resizable;
                case WindowMode.Borderless:
                    return Engine.WindowMode.Fullscreen;
                default:
                    return Engine.WindowMode.Resizable;
            }
        }

        // Token: 0x06001496 RID: 5270 RVA: 0x000A003D File Offset: 0x0009E23D
        public static void HandleUriHandler(Uri uri)
		{
			Program.m_urisToHandle.Add(uri);
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x000A004A File Offset: 0x0009E24A
		public static void DeactivatedHandler()
		{
			GC.Collect();
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x000A0054 File Offset: 0x0009E254
		public static void FrameHandler()
		{
			if (Time.FrameIndex < 0)
			{
				Display.Clear(new Vector4?(Vector4.Zero), new float?(1f), null);
				return;
			}
			if (Time.FrameIndex == 0)
			{
				Program.Initialize();
				return;
			}
			Program.Run();
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x000A00A0 File Offset: 0x0009E2A0
		public static void Initialize()
		{
			Log.Information(string.Format("Survivalcraft starting up at {0}, Version={1}, BuildConfiguration={2}, Platform={3}, Storage.AvailableFreeSpace={4}MB, ApproximateScreenDpi={5:0.0}, ApproxScreenInches={6:0.0}, ScreenResolution={7}, ProcessorsCount={8}, RAM={9}MB, 64bit={10}", new object[]
			{
				DateTime.Now,
				VersionsManager.Version,
				VersionsManager.BuildConfiguration,
				VersionsManager.Platform,
				Storage.FreeSpace / 1024L / 1024L,
				ScreenResolutionManager.ApproximateScreenDpi,
				ScreenResolutionManager.ApproximateScreenInches,
				Window.Size,
				Environment.ProcessorCount,
				Utilities.GetTotalAvailableMemory() / 1024 / 1024,
				Marshal.SizeOf<IntPtr>() == 8
			}));
			MarketplaceManager.Initialize();
			SettingsManager.Initialize();
			AnalyticsManager.Initialize();
			VersionsManager.Initialize();
			ExternalContentManager.Initialize();
			ContentManager.Initialize();
			ScreensManager.Initialize();
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x000A0190 File Offset: 0x0009E390
		public static void Run()
		{
			VrManager.WaitGetPoses();
			double realTime = Time.RealTime;
			Program.LastFrameTime = (float)(realTime - Program.m_frameBeginTime);
			Program.LastCpuFrameTime = (float)(Program.m_cpuEndTime - Program.m_frameBeginTime);
			Program.m_frameBeginTime = realTime;
			if (Keyboard.IsKeyDown(Key.F10))
			{
				if (SettingsManager.WindowMode == WindowMode.Borderless)
				{
					SettingsManager.WindowMode = WindowMode.Resizable;
				}
				else
				{
					SettingsManager.WindowMode = WindowMode.Borderless;
				}
			}
			Window.PresentationInterval = ((!VrManager.IsVrStarted) ? SettingsManager.PresentationInterval : 0);
			try
			{
				if (ExceptionManager.Error == null)
				{
					while (Program.m_urisToHandle.Count > 0)
					{
						Uri obj = Program.m_urisToHandle[0];
						Program.m_urisToHandle.RemoveAt(0);
						Action<Uri> handleUri = Program.HandleUri;
						if (handleUri != null)
						{
							handleUri(obj);
						}
					}
					PerformanceManager.Update();
					MotdManager.Update();
					MusicManager.Update();
					ScreensManager.Update();
					DialogsManager.Update();
				}
				else
				{
					ExceptionManager.UpdateExceptionScreen();
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser(null, e);
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
			try
			{
				Display.RenderTarget = null;
				if (ExceptionManager.Error == null)
				{
					ScreensManager.Draw();
					PerformanceManager.Draw();
					ScreenCaptureManager.Run();
				}
				else
				{
					ExceptionManager.DrawExceptionScreen();
				}
				Program.m_cpuEndTime = Time.RealTime;
			}
			catch (Exception e2)
			{
				ExceptionManager.ReportExceptionToUser(null, e2);
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x04000E97 RID: 3735
		public static double m_frameBeginTime;

		// Token: 0x04000E98 RID: 3736
		public static double m_cpuEndTime;

		// Token: 0x04000E99 RID: 3737
		public static List<Uri> m_urisToHandle = new List<Uri>();
	}
}
