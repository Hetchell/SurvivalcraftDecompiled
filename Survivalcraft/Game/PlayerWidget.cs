using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000396 RID: 918
	public class PlayerWidget : CanvasWidget
	{
		// Token: 0x06001AB4 RID: 6836 RVA: 0x000D20EC File Offset: 0x000D02EC
		public PlayerWidget(PlayerData playerData, CharacterSkinsCache characterSkinsCache)
		{
			XElement node = ContentManager.Get<XElement>("Widgets/PlayerWidget");
			base.LoadContents(this, node);
			this.m_playerModel = this.Children.Find<PlayerModelWidget>("PlayerModel", true);
			this.m_nameLabel = this.Children.Find<LabelWidget>("Name", true);
			this.m_detailsLabel = this.Children.Find<LabelWidget>("Details", true);
			this.m_editButton = this.Children.Find<ButtonWidget>("EditButton", true);
			this.m_playerModel.CharacterSkinsCache = characterSkinsCache;
			this.m_playerData = playerData;
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x000D2184 File Offset: 0x000D0384
		public override void Update()
		{
			SubsystemGameInfo subsystemGameInfo = this.m_playerData.SubsystemPlayers.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_playerModel.PlayerClass = this.m_playerData.PlayerClass;
			this.m_playerModel.CharacterSkinName = this.m_playerData.CharacterSkinName;
			this.m_nameLabel.Text = this.m_playerData.Name;
			this.m_detailsLabel.Text = this.m_playerData.PlayerClass.ToString();
			LabelWidget detailsLabel = this.m_detailsLabel;
			detailsLabel.Text += "\n";
			LabelWidget detailsLabel2 = this.m_detailsLabel;
			detailsLabel2.Text = detailsLabel2.Text + "通过" + PlayerScreen.GetDeviceDisplayName(this.m_playerData.InputDevice) + "控制";
			LabelWidget detailsLabel3 = this.m_detailsLabel;
			detailsLabel3.Text += "\n";
			LabelWidget detailsLabel4 = this.m_detailsLabel;
			detailsLabel4.Text += ((this.m_playerData.LastSpawnTime >= 0.0) ? string.Format("In game for {0:N1} days", (subsystemGameInfo.TotalElapsedGameTime - this.m_playerData.LastSpawnTime) / 1200.0) : "Never spawned yet");
			if (this.m_editButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Player", new object[]
				{
					PlayerScreen.Mode.Edit,
					this.m_playerData
				});
			}
		}

		// Token: 0x0400129D RID: 4765
		public PlayerData m_playerData;

		// Token: 0x0400129E RID: 4766
		public PlayerModelWidget m_playerModel;

		// Token: 0x0400129F RID: 4767
		public LabelWidget m_nameLabel;

		// Token: 0x040012A0 RID: 4768
		public LabelWidget m_detailsLabel;

		// Token: 0x040012A1 RID: 4769
		public ButtonWidget m_editButton;
	}
}
