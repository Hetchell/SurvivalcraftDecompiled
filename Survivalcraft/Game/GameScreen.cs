using System;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Survivalcraft.Game.ModificationHolder;

namespace Game
{
	// Token: 0x02000138 RID: 312
	public class GameScreen : Screen
	{
		// Token: 0x060005CD RID: 1485 RVA: 0x00020C7C File Offset: 0x0001EE7C
		public GameScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/GameScreen");
            ModificationsHolder.nodeForMainScreen = node;
            base.LoadContents(this, node);
			base.IsDrawRequired = true;
			Window.Deactivated += delegate()
			{
				GameManager.SaveProject(true, false); //some issue here. 
			};
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x00020CCD File Offset: 0x0001EECD
		public override void Enter(object[] parameters)
		{
			if (GameManager.Project != null)
			{
				GameManager.Project.FindSubsystem<SubsystemAudio>(true).Unmute();
			}
			MusicManager.CurrentMix = MusicManager.Mix.None;
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00020CEC File Offset: 0x0001EEEC
		public override void Leave()
		{
			if (GameManager.Project != null)
			{
				GameManager.Project.FindSubsystem<SubsystemAudio>(true).Mute();
				GameManager.SaveProject(true, true);
			}
			this.ShowHideCursors(true);
			MusicManager.CurrentMix = MusicManager.Mix.Menu;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00020D1C File Offset: 0x0001EF1C
		public override void Update()
		{
			if (GameManager.Project != null)
			{
				double realTime = Time.RealTime;
				if (realTime - this.m_lastAutosaveTime > 120.0)
				{
					this.m_lastAutosaveTime = realTime;
					GameManager.SaveProject(false, true);
				}
				if (MarketplaceManager.IsTrialMode && GameManager.Project.FindSubsystem<SubsystemGameInfo>(true).TotalElapsedGameTime > 1140.0)
				{
					GameManager.SaveProject(true, false);
					GameManager.DisposeProject();
					ScreensManager.SwitchScreen("TrialEnded", Array.Empty<object>());
				}
				GameManager.UpdateProject();
			}
			this.ShowHideCursors(GameManager.Project == null || DialogsManager.HasDialogs(this) || DialogsManager.HasDialogs(base.RootWidget) || ScreensManager.CurrentScreen != this);
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00020DC9 File Offset: 0x0001EFC9
		public override void Draw(Widget.DrawContext dc)
		{
			if (!ScreensManager.IsAnimating && SettingsManager.ResolutionMode == ResolutionMode.High)
			{
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
			}
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x00020DF9 File Offset: 0x0001EFF9
		public void ShowHideCursors(bool show)
		{
			base.Input.IsMouseCursorVisible = show;
			base.Input.IsPadCursorVisible = show;
			base.Input.IsVrCursorVisible = show;
		}

		// Token: 0x040002AF RID: 687
		public double m_lastAutosaveTime;
	}
}
