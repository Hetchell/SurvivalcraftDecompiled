using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200014B RID: 331
	public static class ScreensManager
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x000252A9 File Offset: 0x000234A9
		// (set) Token: 0x06000630 RID: 1584 RVA: 0x000252B0 File Offset: 0x000234B0
		public static ContainerWidget RootWidget { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000631 RID: 1585 RVA: 0x000252B8 File Offset: 0x000234B8
		public static bool IsAnimating
		{
			get
			{
				return ScreensManager.m_animationData != null;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000632 RID: 1586 RVA: 0x000252C2 File Offset: 0x000234C2
		// (set) Token: 0x06000633 RID: 1587 RVA: 0x000252C9 File Offset: 0x000234C9
		public static Screen CurrentScreen { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000634 RID: 1588 RVA: 0x000252D1 File Offset: 0x000234D1
		// (set) Token: 0x06000635 RID: 1589 RVA: 0x000252D8 File Offset: 0x000234D8
		public static Screen PreviousScreen { get; set; }

		// Token: 0x06000636 RID: 1590 RVA: 0x000252E0 File Offset: 0x000234E0
		public static T FindScreen<T>(string name) where T : Screen
		{
			Screen screen;
			ScreensManager.m_screens.TryGetValue(name, out screen);
			return (T)((object)screen);
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00025301 File Offset: 0x00023501
		public static void AddScreen(string name, Screen screen)
		{
			ScreensManager.m_screens.Add(name, screen);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0002530F File Offset: 0x0002350F
		public static void SwitchScreen(string name, params object[] parameters)
		{
			ScreensManager.SwitchScreen(string.IsNullOrEmpty(name) ? null : ScreensManager.FindScreen<Screen>(name), parameters);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00025328 File Offset: 0x00023528
		public static void SwitchScreen(Screen screen, params object[] parameters)
		{
			if (ScreensManager.m_animationData != null)
			{
				ScreensManager.EndAnimation();
			}
			ScreensManager.m_animationData = new ScreensManager.AnimationData()
			{
				NewScreen = screen,
				OldScreen = ScreensManager.CurrentScreen,
				Parameters = parameters,
				Speed = ((ScreensManager.CurrentScreen == null) ? float.MaxValue : 4f)
			};
			if (ScreensManager.CurrentScreen != null)
			{
				ScreensManager.RootWidget.IsUpdateEnabled = false;
				ScreensManager.CurrentScreen.Input.Clear();
			}
			ScreensManager.PreviousScreen = ScreensManager.CurrentScreen;
			ScreensManager.CurrentScreen = screen;
			ScreensManager.UpdateAnimation();
			if (ScreensManager.CurrentScreen != null)
			{
				Log.Verbose("Entered screen \"" + ScreensManager.GetScreenName(ScreensManager.CurrentScreen) + "\"");
				AnalyticsManager.LogEvent("[" + ScreensManager.GetScreenName(ScreensManager.CurrentScreen) + "] Entered screen", new AnalyticsParameter[]
				{
					new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff"))
				});
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00025420 File Offset: 0x00023620
		public static void Initialize()
		{
			if (ScreensManager.Initialize1 != null)
			{
				ScreensManager.Initialize1();
				return;
			}
			ScreensManager.RootWidget = new CanvasWidget();
			ScreensManager.RootWidget.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.All);
			LoadingScreen loadingScreen = new LoadingScreen();
			ScreensManager.AddScreen("Loading", loadingScreen);
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Nag", new NagScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("MainMenu", new MainMenuScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Recipaedia", new RecipaediaScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("RecipaediaRecipes", new RecipaediaRecipesScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("RecipaediaDescription", new RecipaediaDescriptionScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Bestiary", new BestiaryScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("BestiaryDescription", new BestiaryDescriptionScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Help", new HelpScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("HelpTopic", new HelpTopicScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Settings", new SettingsScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsPerformance", new SettingsPerformanceScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsGraphics", new SettingsGraphicsScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsUi", new SettingsUiScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsCompatibility", new SettingsCompatibilityScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsAudio", new SettingsAudioScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("SettingsControls", new SettingsControlsScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Play", new PlayScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("NewWorld", new NewWorldScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("ModifyWorld", new ModifyWorldScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("WorldOptions", new WorldOptionsScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("GameLoading", new GameLoadingScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Game", new GameScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("TrialEnded", new TrialEndedScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("ExternalContent", new ExternalContentScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("CommunityContent", new CommunityContentScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Content", new ContentScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("ManageContent", new ManageContentScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Players", new PlayersScreen());
			});
			loadingScreen.AddLoadAction(delegate
			{
				ScreensManager.AddScreen("Player", new PlayerScreen());
			});
			ScreensManager.SwitchScreen("Loading", Array.Empty<object>());
			Action initialized = ScreensManager.Initialized;
			if (initialized == null)
			{
				return;
			}
			initialized();
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x000258BD File Offset: 0x00023ABD
		public static void Update()
		{
			if (ScreensManager.m_animationData != null)
			{
				ScreensManager.UpdateAnimation();
			}
			if (VrManager.IsVrStarted)
			{
				ScreensManager.AnimateVrQuad();
			}
			Widget.UpdateWidgetsHierarchy(ScreensManager.RootWidget);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000258E4 File Offset: 0x00023AE4
		public static void Draw()
		{
			if (VrManager.IsVrStarted)
			{
				Point2 point = new Point2(Display.Viewport.Width, Display.Viewport.Height);
				if (MathUtils.Max(point.X, point.Y) == 0)
				{
					point = new Point2(1500, 1000);
				}
				while (MathUtils.Max(point.X, point.Y) < 1024)
				{
					point *= 2;
				}
				if (ScreensManager.m_uiRenderTarget == null || ScreensManager.m_uiRenderTarget.Width != point.X || ScreensManager.m_uiRenderTarget.Height != point.Y)
				{
					Utilities.Dispose<RenderTarget2D>(ref ScreensManager.m_uiRenderTarget);
					ScreensManager.m_uiRenderTarget = new RenderTarget2D(point.X, point.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
				}
				RenderTarget2D renderTarget = Display.RenderTarget;
				try
				{
					Display.RenderTarget = ScreensManager.m_uiRenderTarget;
					ScreensManager.LayoutAndDrawWidgets();
					Display.RenderTarget = VrManager.VrRenderTarget;
					for (VrEye vrEye = VrEye.Left; vrEye <= VrEye.Right; vrEye++)
					{
						Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
						ScreensManager.DrawVrBackground();
						ScreensManager.DrawVrQuad();
						Matrix hmdMatrix = VrManager.HmdMatrix;
						Matrix m = Matrix.Invert(VrManager.GetEyeToHeadTransform(vrEye));
						Matrix m2 = Matrix.Invert(hmdMatrix);
						Matrix projectionMatrix = VrManager.GetProjectionMatrix(vrEye, 0.1f, 1024f);
						ScreensManager.m_pr3.Flush(m2 * m * projectionMatrix, true, int.MaxValue);
						VrManager.SubmitEyeTexture(vrEye, VrManager.VrRenderTarget);
					}
				}
				finally
				{
					Display.RenderTarget = renderTarget;
				}
				ScreensManager.m_pr2.TexturedBatch(ScreensManager.m_uiRenderTarget, false, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.Opaque, SamplerState.PointClamp).QueueQuad(new Vector2(0f, 0f), new Vector2((float)ScreensManager.m_uiRenderTarget.Width, (float)ScreensManager.m_uiRenderTarget.Height), 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), Color.White);
				ScreensManager.m_pr2.Flush(true, int.MaxValue);
				return;
			}
			Utilities.Dispose<RenderTarget2D>(ref ScreensManager.m_uiRenderTarget);
			ScreensManager.LayoutAndDrawWidgets();
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00025B10 File Offset: 0x00023D10
		public static void UpdateAnimation()
		{
			float num = MathUtils.Min(Time.FrameDuration, 0.1f);
			float factor = ScreensManager.m_animationData.Factor;
			ScreensManager.m_animationData.Factor = MathUtils.Min(ScreensManager.m_animationData.Factor + ScreensManager.m_animationData.Speed * num, 1f);
			if (ScreensManager.m_animationData.Factor < 0.5f)
			{
				if (ScreensManager.m_animationData.OldScreen != null)
				{
					float num2 = 2f * (0.5f - ScreensManager.m_animationData.Factor);
					float scale = 1f;
					ScreensManager.m_animationData.OldScreen.ColorTransform = new Color(num2, num2, num2, num2);
					ScreensManager.m_animationData.OldScreen.RenderTransform = Matrix.CreateTranslation((0f - ScreensManager.m_animationData.OldScreen.ActualSize.X) / 2f, (0f - ScreensManager.m_animationData.OldScreen.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(ScreensManager.m_animationData.OldScreen.ActualSize.X / 2f, ScreensManager.m_animationData.OldScreen.ActualSize.Y / 2f, 0f);
				}
			}
			else if (factor < 0.5f)
			{
				if (ScreensManager.m_animationData.OldScreen != null)
				{
					ScreensManager.m_animationData.OldScreen.Leave();
					ScreensManager.RootWidget.Children.Remove(ScreensManager.m_animationData.OldScreen);
				}
				if (ScreensManager.m_animationData.NewScreen != null)
				{
					ScreensManager.RootWidget.Children.Insert(0, ScreensManager.m_animationData.NewScreen);
					ScreensManager.m_animationData.NewScreen.Enter(ScreensManager.m_animationData.Parameters);
					ScreensManager.m_animationData.NewScreen.ColorTransform = Color.Transparent;
					ScreensManager.RootWidget.IsUpdateEnabled = true;
				}
			}
			else if (ScreensManager.m_animationData.NewScreen != null)
			{
				float num3 = 2f * (ScreensManager.m_animationData.Factor - 0.5f);
				float scale2 = 1f;
				ScreensManager.m_animationData.NewScreen.ColorTransform = new Color(num3, num3, num3, num3);
				ScreensManager.m_animationData.NewScreen.RenderTransform = Matrix.CreateTranslation((0f - ScreensManager.m_animationData.NewScreen.ActualSize.X) / 2f, (0f - ScreensManager.m_animationData.NewScreen.ActualSize.Y) / 2f, 0f) * Matrix.CreateScale(scale2) * Matrix.CreateTranslation(ScreensManager.m_animationData.NewScreen.ActualSize.X / 2f, ScreensManager.m_animationData.NewScreen.ActualSize.Y / 2f, 0f);
			}
			if (ScreensManager.m_animationData.Factor >= 1f)
			{
				ScreensManager.EndAnimation();
			}
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00025E14 File Offset: 0x00024014
		public static void EndAnimation()
		{
			if (ScreensManager.m_animationData.NewScreen != null)
			{
				ScreensManager.m_animationData.NewScreen.ColorTransform = Color.White;
				ScreensManager.m_animationData.NewScreen.RenderTransform = Matrix.CreateScale(1f);
			}
			ScreensManager.m_animationData = null;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00025E60 File Offset: 0x00024060
		public static string GetScreenName(Screen screen)
		{
			string key = ScreensManager.m_screens.FirstOrDefault(kvp => kvp.Value == screen).Key;
			if (key == null)
			{
				return string.Empty;
			}
			return key;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00025EA4 File Offset: 0x000240A4
		public static void AnimateVrQuad()
		{
			if (Time.FrameIndex >= 5)
			{
				float num = 6f;
				Matrix hmdMatrix = VrManager.HmdMatrix;
				Vector3 vector = hmdMatrix.Translation + num * (Vector3.Normalize(hmdMatrix.Forward * new Vector3(1f, 0f, 1f)) + new Vector3(0f, 0.1f, 0f));
				if (ScreensManager.m_vrQuadPosition == Vector3.Zero)
				{
					ScreensManager.m_vrQuadPosition = vector;
				}
				if (Vector3.Distance(ScreensManager.m_vrQuadPosition, vector) > 0f)
				{
					Vector3 v = vector * new Vector3(1f, 0f, 1f) - ScreensManager.m_vrQuadPosition * new Vector3(1f, 0f, 1f);
					Vector3 v2 = vector * new Vector3(0f, 1f, 0f) - ScreensManager.m_vrQuadPosition * new Vector3(0f, 1f, 0f);
					float num2 = v.Length();
					float num3 = v2.Length();
					ScreensManager.m_vrQuadPosition += v * MathUtils.Min(0.75f * MathUtils.Pow(MathUtils.Max(num2 - 0.15f * num, 0f), 0.33f) * Time.FrameDuration, 1f);
					ScreensManager.m_vrQuadPosition += v2 * MathUtils.Min(1.5f * MathUtils.Pow(MathUtils.Max(num3 - 0.05f * num, 0f), 0.33f) * Time.FrameDuration, 1f);
				}
				Vector2 vector2 = new Vector2((float)ScreensManager.m_uiRenderTarget.Width / (float)ScreensManager.m_uiRenderTarget.Height, 1f);
				vector2 /= MathUtils.Max(vector2.X, vector2.Y);
				vector2 *= 7.5f;
				ScreensManager.m_vrQuadMatrix.Forward = Vector3.Normalize(hmdMatrix.Translation - ScreensManager.m_vrQuadPosition);
				ScreensManager.m_vrQuadMatrix.Right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, ScreensManager.m_vrQuadMatrix.Forward)) * vector2.X;
				ScreensManager.m_vrQuadMatrix.Up = Vector3.Normalize(Vector3.Cross(ScreensManager.m_vrQuadMatrix.Forward, ScreensManager.m_vrQuadMatrix.Right)) * vector2.Y;
				ScreensManager.m_vrQuadMatrix.Translation = ScreensManager.m_vrQuadPosition - 0.5f * (ScreensManager.m_vrQuadMatrix.Right + ScreensManager.m_vrQuadMatrix.Up);
				ScreensManager.RootWidget.WidgetsHierarchyInput.VrQuadMatrix = new Matrix?(ScreensManager.m_vrQuadMatrix);
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00026184 File Offset: 0x00024384
		public static void DrawVrQuad()
		{
			ScreensManager.QueueQuad(ScreensManager.m_pr3.TexturedBatch(ScreensManager.m_uiRenderTarget, false, 0, DepthStencilState.Default, RasterizerState.CullNoneScissor, BlendState.Opaque, SamplerState.LinearClamp), ScreensManager.m_vrQuadMatrix.Translation, ScreensManager.m_vrQuadMatrix.Right, ScreensManager.m_vrQuadMatrix.Up, Color.White);
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000261E0 File Offset: 0x000243E0
		public static void DrawVrBackground()
		{
			Matrix hmdMatrix = VrManager.HmdMatrix;
			TexturedBatch3D batch = ScreensManager.m_pr3.TexturedBatch(ContentManager.Get<Texture2D>("Textures/Star"), false, 0, null, null, null, null);
			ScreensManager.Random.Seed(0);
			for (int i = 0; i < 1500; i++)
			{
				float f = MathUtils.Pow(ScreensManager.Random.Float(0f, 1f), 6f);
				Color rgb = (MathUtils.Lerp(0.05f, 0.4f, f) * Color.White).RGB;
				int num = 6;
				Vector3 vector = ScreensManager.Random.Vector3(500f);
				Vector3 vector2 = Vector3.Normalize(Vector3.Cross(vector, Vector3.UnitY)) * (float)num;
				Vector3 up = Vector3.Normalize(Vector3.Cross(vector2, vector)) * (float)num;
				ScreensManager.QueueQuad(batch, vector + hmdMatrix.Translation, vector2, up, rgb);
			}
			TexturedBatch3D batch2 = ScreensManager.m_pr3.TexturedBatch(ContentManager.Get<Texture2D>("Textures/Blocks"), true, 1, null, null, null, SamplerState.PointClamp);
			for (int j = -8; j <= 8; j++)
			{
				for (int k = -8; k <= 8; k++)
				{
					float num2 = 1f;
					float num3 = 1f;
					Vector3 vector3 = new Vector3(((float)j - 0.5f) * num2, 0f, ((float)k - 0.5f) * num2) + new Vector3(MathUtils.Round(hmdMatrix.Translation.X), 0f, MathUtils.Round(hmdMatrix.Translation.Z));
					float num4 = Vector3.Distance(vector3, hmdMatrix.Translation);
					float num5 = MathUtils.Lerp(1f, 0f, MathUtils.Saturate(num4 / 7f));
					if (num5 > 0f)
					{
						ScreensManager.QueueQuad(batch2, vector3, new Vector3(num3, 0f, 0f), new Vector3(0f, 0f, num3), Color.Gray * num5, new Vector2(0.1875f, 0.25f), new Vector2(0.25f, 0.3125f));
					}
				}
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00026414 File Offset: 0x00024614
		public static void LayoutAndDrawWidgets()
		{
			if (ScreensManager.m_animationData != null)
			{
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
			}
			float num;
			switch (SettingsManager.GuiSize)
			{
			case GuiSize.Smallest:
				num = 1120f;
				break;
			case GuiSize.Smaller:
				num = 960f;
				break;
			case GuiSize.Normal:
				num = 850f;
				break;
			default:
				num = 850f;
				break;
			}
			num *= ScreensManager.DebugUiScale;
			Vector2 vector = new Vector2((float)Display.Viewport.Width, (float)Display.Viewport.Height);
			float num2 = vector.X / num;
			Vector2 availableSize = new Vector2(num, num / vector.X * vector.Y);
			float num3 = num * 9f / 16f;
			if (vector.Y / num2 < num3)
			{
				num2 = vector.Y / num3;
				availableSize = new Vector2(num3 / vector.Y * vector.X, num3);
			}
			ScreensManager.RootWidget.LayoutTransform = Matrix.CreateScale(num2, num2, 1f);
			if (SettingsManager.UpsideDownLayout)
			{
				ScreensManager.RootWidget.LayoutTransform *= new Matrix(-1f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
			}
			Widget.LayoutWidgetsHierarchy(ScreensManager.RootWidget, availableSize);
			Widget.DrawWidgetsHierarchy(ScreensManager.RootWidget);
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x000265A0 File Offset: 0x000247A0
		public static void QueueQuad(FlatBatch3D batch, Vector3 corner, Vector3 right, Vector3 up, Color color)
		{
			Vector3 p = corner + right;
			Vector3 p2 = corner + right + up;
			Vector3 p3 = corner + up;
			batch.QueueQuad(corner, p, p2, p3, color);
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x000265D7 File Offset: 0x000247D7
		public static void QueueQuad(TexturedBatch3D batch, Vector3 center, Vector3 right, Vector3 up, Color color)
		{
			ScreensManager.QueueQuad(batch, center, right, up, color, new Vector2(0f, 0f), new Vector2(1f, 1f));
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00026604 File Offset: 0x00024804
		public static void QueueQuad(TexturedBatch3D batch, Vector3 corner, Vector3 right, Vector3 up, Color color, Vector2 tc1, Vector2 tc2)
		{
			Vector3 p = corner + right;
			Vector3 p2 = corner + right + up;
			Vector3 p3 = corner + up;
			batch.QueueQuad(corner, p, p2, p3, new Vector2(tc1.X, tc2.Y), new Vector2(tc2.X, tc2.Y), new Vector2(tc2.X, tc1.Y), new Vector2(tc1.X, tc1.Y), color);
		}

		// Token: 0x04000326 RID: 806
		public static Dictionary<string, Screen> m_screens = new Dictionary<string, Screen>();

		// Token: 0x04000327 RID: 807
		public static ScreensManager.AnimationData m_animationData;

		// Token: 0x04000328 RID: 808
		public static PrimitivesRenderer2D m_pr2 = new PrimitivesRenderer2D();

		// Token: 0x04000329 RID: 809
		public static PrimitivesRenderer3D m_pr3 = new PrimitivesRenderer3D();

		// Token: 0x0400032A RID: 810
		public static Game.Random Random = new Game.Random(0);

		// Token: 0x0400032B RID: 811
		public static RenderTarget2D m_uiRenderTarget;

		// Token: 0x0400032C RID: 812
		public static Vector3 m_vrQuadPosition;

		// Token: 0x0400032D RID: 813
		public static Matrix m_vrQuadMatrix;

		// Token: 0x0400032E RID: 814
		public static float DebugUiScale = 1f;

		// Token: 0x04000332 RID: 818
		public static Action Initialize1;

		// Token: 0x04000333 RID: 819
		public static Action Initialized;

		// Token: 0x02000409 RID: 1033
		public class AnimationData
		{
			// Token: 0x040014FD RID: 5373
			public Screen OldScreen;

			// Token: 0x040014FE RID: 5374
			public Screen NewScreen;

			// Token: 0x040014FF RID: 5375
			public float Factor;

			// Token: 0x04001500 RID: 5376
			public float Speed;

			// Token: 0x04001501 RID: 5377
			public object[] Parameters;
		}
	}
}
