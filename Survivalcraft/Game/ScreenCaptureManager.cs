using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000147 RID: 327
	public static class ScreenCaptureManager
	{
		// Token: 0x06000627 RID: 1575 RVA: 0x00024D8C File Offset: 0x00022F8C
		public static void Run()
		{
			if (ScreenCaptureManager.m_captureRequested)
			{
				try
				{
					ScreenshotSize screenshotSize = SettingsManager.ScreenshotSize;
					int num;
					int num2;
					if (screenshotSize != ScreenshotSize.ScreenSize)
					{
						if (screenshotSize != ScreenshotSize.FullHD)
						{
							num = 1920;
							num2 = 1080;
						}
						else
						{
							num = 1920;
							num2 = 1080;
						}
					}
					else
					{
						num = MathUtils.Max(Window.ScreenSize.X, Window.ScreenSize.Y);
						num2 = MathUtils.Min(Window.ScreenSize.X, Window.ScreenSize.Y);
						float num3 = (float)num / (float)num2;
						num = MathUtils.Min(num, 2048);
						num2 = (int)MathUtils.Round((float)num / num3);
					}
					DateTime now = DateTime.Now;
					ScreenCaptureManager.Capture(num, num2, string.Format("Survivalcraft {0:D4}-{1:D2}-{2:D2} {3:D2}-{4:D2}-{5:D2}.jpg", new object[]
					{
						now.Year,
						now.Month,
						now.Day,
						now.Hour,
						now.Minute,
						now.Second
					}));
					Action successHandler = ScreenCaptureManager.m_successHandler;
					if (successHandler != null)
					{
						successHandler();
					}
					GC.Collect();
				}
				catch (Exception ex)
				{
					Log.Error("Error capturing screen. Reason: " + ex.Message);
					Action<Exception> failureHandler = ScreenCaptureManager.m_failureHandler;
					if (failureHandler != null)
					{
						failureHandler(ex);
					}
				}
				finally
				{
					ScreenCaptureManager.m_captureRequested = false;
					ScreenCaptureManager.m_successHandler = null;
					ScreenCaptureManager.m_failureHandler = null;
				}
			}
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00024F24 File Offset: 0x00023124
		public static void CapturePhoto(Action success, Action<Exception> failure)
		{
			if (!ScreenCaptureManager.m_captureRequested)
			{
				ScreenCaptureManager.m_captureRequested = true;
				ScreenCaptureManager.m_successHandler = success;
				ScreenCaptureManager.m_failureHandler = failure;
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00024F40 File Offset: 0x00023140
		public static void Capture(int width, int height, string filename)
		{
			if (GameManager.Project != null)
			{
				using (RenderTarget2D renderTarget2D = new RenderTarget2D(width, height, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8))
				{
					RenderTarget2D renderTarget = Display.RenderTarget;
					Dictionary<ComponentGui, bool> dictionary = new Dictionary<ComponentGui, bool>();
					ResolutionMode resolutionMode = ResolutionMode.High;
					try
					{
						if (!SettingsManager.ShowGuiInScreenshots)
						{
							foreach (ComponentPlayer componentPlayer in GameManager.Project.FindSubsystem<SubsystemPlayers>(true).ComponentPlayers)
							{
								dictionary[componentPlayer.ComponentGui] = componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible;
								componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible = false;
							}
						}
						resolutionMode = SettingsManager.ResolutionMode;
						SettingsManager.ResolutionMode = ResolutionMode.High;
						Display.RenderTarget = renderTarget2D;
						ScreensManager.Draw();
						if (SettingsManager.ShowLogoInScreenshots)
						{
							PrimitivesRenderer2D primitivesRenderer2D = new PrimitivesRenderer2D();
							Texture2D texture2D = ContentManager.Get<Texture2D>("Textures/Gui/ScreenCaptureOverlay");
							Vector2 vector = new Vector2((float)((width - texture2D.Width) / 2), 0f);
							Vector2 corner = vector + new Vector2((float)texture2D.Width, (float)texture2D.Height);
							primitivesRenderer2D.TexturedBatch(texture2D, false, 0, DepthStencilState.None, null, null, null).QueueQuad(vector, corner, 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), Color.White);
							primitivesRenderer2D.Flush(true, int.MaxValue);
						}
					}
					finally
					{
						Display.RenderTarget = renderTarget;
						foreach (KeyValuePair<ComponentGui, bool> keyValuePair in dictionary)
						{
							keyValuePair.Key.ControlsContainerWidget.IsVisible = keyValuePair.Value;
						}
						SettingsManager.ResolutionMode = resolutionMode;
					}
					Image image = new Image(renderTarget2D.Width, renderTarget2D.Height);
					renderTarget2D.GetData<Color>(image.Pixels, 0, new Rectangle(0, 0, renderTarget2D.Width, renderTarget2D.Height));
					if (!Storage.DirectoryExists(ScreenCaptureManager.ScreenshotDir))
					{
						Storage.CreateDirectory(ScreenCaptureManager.ScreenshotDir);
					}
					using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
					{
						ScreenCaptureManager.ScreenshotDir,
						filename
					}), OpenFileMode.CreateOrOpen))
					{
						Image.Save(image, stream, ImageFileFormat.Jpg, false);
					}
				}
			}
		}

		// Token: 0x04000313 RID: 787
		public static readonly string ScreenshotDir = "app:/ScreenCapture";

		// Token: 0x04000314 RID: 788
		public static bool m_captureRequested;

		// Token: 0x04000315 RID: 789
		public static Action m_successHandler;

		// Token: 0x04000316 RID: 790
		public static Action<Exception> m_failureHandler;
	}
}
