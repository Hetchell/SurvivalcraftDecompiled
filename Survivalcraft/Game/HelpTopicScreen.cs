using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200013A RID: 314
	public class HelpTopicScreen : Screen
	{
		// Token: 0x060005DB RID: 1499 RVA: 0x0002127C File Offset: 0x0001F47C
		public HelpTopicScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/HelpTopicScreen");
			base.LoadContents(this, node);
			this.m_titleLabel = this.Children.Find<LabelWidget>("Title", true);
			this.m_textLabel = this.Children.Find<LabelWidget>("Text", true);
			this.m_scrollPanel = this.Children.Find<ScrollPanelWidget>("ScrollPanel", true);
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000212E8 File Offset: 0x0001F4E8
		public override void Enter(object[] parameters)
		{
			HelpTopic helpTopic = (HelpTopic)parameters[0];
			this.m_titleLabel.Text = helpTopic.Title;
			this.m_textLabel.Text = helpTopic.Text;
			this.m_scrollPanel.ScrollPosition = 0f;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00021330 File Offset: 0x0001F530
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x040002B5 RID: 693
		public LabelWidget m_titleLabel;

		// Token: 0x040002B6 RID: 694
		public LabelWidget m_textLabel;

		// Token: 0x040002B7 RID: 695
		public ScrollPanelWidget m_scrollPanel;
	}
}
