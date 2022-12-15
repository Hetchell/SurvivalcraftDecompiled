using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000137 RID: 311
	public class GameLoadingScreen : Screen
	{
		// Token: 0x060005C6 RID: 1478 RVA: 0x00020A94 File Offset: 0x0001EC94
		public GameLoadingScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/GameLoadingScreen");
			base.LoadContents(this, node);
			this.m_stateMachine.AddState("WaitingForFadeIn", null, delegate
			{
				if (!ScreensManager.IsAnimating)
				{
					if (string.IsNullOrEmpty(this.m_worldSnapshotName))
					{
						this.m_stateMachine.TransitionTo("Loading");
						return;
					}
					this.m_stateMachine.TransitionTo("RestoringSnapshot");
				}
			}, null);
			this.m_stateMachine.AddState("Loading", null, delegate
			{
				ContainerWidget gamesWidget = ScreensManager.FindScreen<GameScreen>("Game").Children.Find<ContainerWidget>("GamesWidget", true);
				GameManager.LoadProject(this.m_worldInfo, gamesWidget);
				ScreensManager.SwitchScreen("Game", Array.Empty<object>());
			}, null);
			this.m_stateMachine.AddState("RestoringSnapshot", null, delegate
			{
				GameManager.DisposeProject();
				WorldsManager.RestoreWorldFromSnapshot(this.m_worldInfo.DirectoryName, this.m_worldSnapshotName);
				this.m_stateMachine.TransitionTo("Loading");
			}, null);
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00020B20 File Offset: 0x0001ED20
		public override void Update()
		{
			try
			{
				this.m_stateMachine.Update();
			}
			catch (Exception e)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
				DialogsManager.ShowDialog(null, new MessageDialog(LanguageControl.Get(GameLoadingScreen.fName, 1), ExceptionManager.MakeFullErrorMessage(e), LanguageControl.Get("Usual", "ok"), null, null));
			}
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00020B8C File Offset: 0x0001ED8C
		public override void Enter(object[] parameters)
		{
			this.m_worldInfo = (WorldInfo)parameters[0];
			this.m_worldSnapshotName = (string)parameters[1];
			this.m_stateMachine.TransitionTo("WaitingForFadeIn");
			ProgressManager.UpdateProgress("Loading World", 0f);
		}

		// Token: 0x040002AB RID: 683
		public WorldInfo m_worldInfo;

		// Token: 0x040002AC RID: 684
		public string m_worldSnapshotName;

		// Token: 0x040002AD RID: 685
		public StateMachine m_stateMachine = new StateMachine();

		// Token: 0x040002AE RID: 686
		public static string fName = "GameLoadingScreen";
	}
}
