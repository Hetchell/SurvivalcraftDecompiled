using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x02000139 RID: 313
	public class HelpScreen : Screen
	{
		// Token: 0x060005D3 RID: 1491 RVA: 0x00020E20 File Offset: 0x0001F020
		public HelpScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/HelpScreen");
			base.LoadContents(this, node);
			this.m_topicsList = this.Children.Find<ListPanelWidget>("TopicsList", true);
			this.m_recipaediaButton = this.Children.Find<ButtonWidget>("RecipaediaButton", true);
			this.m_bestiaryButton = this.Children.Find<ButtonWidget>("BestiaryButton", true);
			this.m_topicsList.ItemWidgetFactory = delegate(object item)
			{
				HelpTopic helpTopic2 = (HelpTopic)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/HelpTopicItem");
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				containerWidget.Children.Find<LabelWidget>("HelpTopicItem.Title", true).Text = helpTopic2.Title;
				return containerWidget;
			};
			this.m_topicsList.ItemClicked += delegate(object item)
			{
				HelpTopic helpTopic2 = item as HelpTopic;
				if (helpTopic2 != null)
				{
					this.ShowTopic(helpTopic2);
				}
			};
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in LanguageControl.items2["Help"])
			{
				if (keyValuePair.Value.ContainsKey("DisabledPlatforms"))
				{
					string text;
					keyValuePair.Value.TryGetValue("DisabledPlatforms", out text);
					if (text.Split(new string[]
					{
						","
					}, StringSplitOptions.None).FirstOrDefault((string s) => s.Trim().ToLower() == VersionsManager.Platform.ToString().ToLower()) == null)
					{
						continue;
					}
				}
				string empty;
				keyValuePair.Value.TryGetValue("Title", out empty);
				string empty2;
				keyValuePair.Value.TryGetValue("Name", out empty2);
				string empty3;
				keyValuePair.Value.TryGetValue("value", out empty3);
				if (string.IsNullOrEmpty(empty))
				{
					empty = string.Empty;
				}
				if (string.IsNullOrEmpty(empty2))
				{
					empty2 = string.Empty;
				}
				if (string.IsNullOrEmpty(empty3))
				{
					empty3 = string.Empty;
				}
				string name = empty2;
				string title = empty;
				string text2 = string.Empty;
				foreach (string text3 in empty3.Split(new string[]
				{
					"\n"
				}, StringSplitOptions.None))
				{
					text2 = text2 + text3.Trim() + " ";
				}
				text2 = text2.Replace("\r", "");
				text2 = text2.Replace("’", "'");
				text2 = text2.Replace("\\n", "\n");
				HelpTopic helpTopic = new HelpTopic
				{
					Name = name,
					Title = title,
					Text = text2
				};
				if (!string.IsNullOrEmpty(helpTopic.Name))
				{
					this.m_topics.Add(helpTopic.Name, helpTopic);
				}
				this.m_topicsList.AddItem(helpTopic);
			}
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x000210CC File Offset: 0x0001F2CC
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("HelpTopic") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("Recipaedia") && ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("Bestiary"))
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0002110C File Offset: 0x0001F30C
		public override void Leave()
		{
			this.m_topicsList.SelectedItem = null;
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x0002111C File Offset: 0x0001F31C
		public override void Update()
		{
			if (this.m_recipaediaButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Recipaedia", Array.Empty<object>());
			}
			if (this.m_bestiaryButton.IsClicked)
			{
				ScreensManager.SwitchScreen("Bestiary", Array.Empty<object>());
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x000211A3 File Offset: 0x0001F3A3
		public HelpTopic GetTopic(string name)
		{
			return this.m_topics[name];
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x000211B4 File Offset: 0x0001F3B4
		public void ShowTopic(HelpTopic helpTopic)
		{
			if (helpTopic.Name == "Keyboard")
			{
				DialogsManager.ShowDialog(null, new KeyboardHelpDialog());
				return;
			}
			if (helpTopic.Name == "Gamepad")
			{
				DialogsManager.ShowDialog(null, new GamepadHelpDialog());
				return;
			}
			ScreensManager.SwitchScreen("HelpTopic", new object[]
			{
				helpTopic
			});
		}

		// Token: 0x040002B0 RID: 688
		public ListPanelWidget m_topicsList;

		// Token: 0x040002B1 RID: 689
		public ButtonWidget m_recipaediaButton;

		// Token: 0x040002B2 RID: 690
		public ButtonWidget m_bestiaryButton;

		// Token: 0x040002B3 RID: 691
		public Screen m_previousScreen;

		// Token: 0x040002B4 RID: 692
		public Dictionary<string, HelpTopic> m_topics = new Dictionary<string, HelpTopic>();
	}
}
