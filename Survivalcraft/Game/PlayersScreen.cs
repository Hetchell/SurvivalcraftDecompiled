using System;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000141 RID: 321
	public class PlayersScreen : Screen
	{
		// Token: 0x06000605 RID: 1541 RVA: 0x000230C4 File Offset: 0x000212C4
		public PlayersScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/PlayersScreen");
			base.LoadContents(this, node);
			this.m_playersPanel = this.Children.Find<StackPanelWidget>("PlayersPanel", true);
			this.m_addPlayerButton = this.Children.Find<ButtonWidget>("AddPlayerButton", true);
			this.m_screenLayoutButton = this.Children.Find<ButtonWidget>("ScreenLayoutButton", true);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0002313C File Offset: 0x0002133C
		public override void Enter(object[] parameters)
		{
			this.m_subsystemPlayers = (SubsystemPlayers)parameters[0];
			this.m_subsystemPlayers.PlayerAdded += this.PlayersChanged;
			this.m_subsystemPlayers.PlayerRemoved += this.PlayersChanged;
			this.UpdatePlayersPanel();
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0002318C File Offset: 0x0002138C
		public override void Leave()
		{
			this.m_subsystemPlayers.PlayerAdded -= this.PlayersChanged;
			this.m_subsystemPlayers.PlayerRemoved -= this.PlayersChanged;
			this.m_subsystemPlayers = null;
			this.m_characterSkinsCache.Clear();
			this.m_playersPanel.Children.Clear();
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x000231EC File Offset: 0x000213EC
		public override void Update()
		{
			if (this.m_addPlayerButton.IsClicked)
			{
				SubsystemGameInfo subsystemGameInfo = this.m_subsystemPlayers.Project.FindSubsystem<SubsystemGameInfo>(true);
				if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Cruel)
				{
					DialogsManager.ShowDialog(null, new MessageDialog("Unavailable", "Cannot add players in cruel mode.", "确定", null, null));
				}
				else if (subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
				{
					DialogsManager.ShowDialog(null, new MessageDialog("Unavailable", "Cannot add players in adventure mode.", "确定", null, null));
				}
				else if (this.m_subsystemPlayers.PlayersData.Count >= 4)
				{
					DialogsManager.ShowDialog(null, new MessageDialog("Unavailable", string.Format("A maximum of {0} players are allowed.", 4), "确定", null, null));
				}
				else
				{
					ScreensManager.SwitchScreen("Player", new object[]
					{
						PlayerScreen.Mode.Add,
						this.m_subsystemPlayers.Project
					});
				}
			}
			if (this.m_screenLayoutButton.IsClicked)
			{
				ScreenLayout[] array = null;
				if (this.m_subsystemPlayers.PlayersData.Count == 1)
				{
					array = new ScreenLayout[1];
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 2)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.DoubleVertical,
						ScreenLayout.DoubleHorizontal,
						ScreenLayout.DoubleOpposite
					};
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 3)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.TripleVertical,
						ScreenLayout.TripleHorizontal,
						ScreenLayout.TripleEven,
						ScreenLayout.TripleOpposite
					};
				}
				else if (this.m_subsystemPlayers.PlayersData.Count == 4)
				{
					array = new ScreenLayout[]
					{
						ScreenLayout.Quadruple,
						ScreenLayout.QuadrupleOpposite
					};
				}
				if (array != null)
				{
					DialogsManager.ShowDialog(null, new ListSelectionDialog("Select Screen Layout", array, 80f, delegate(object o)
					{
						string str = o.ToString();
						string name = "Textures/Atlas/ScreenLayout" + str;
						return new StackPanelWidget
						{
							Direction = LayoutDirection.Horizontal,
							VerticalAlignment = WidgetAlignment.Center,
							Children = 
							{
								new RectangleWidget
								{
									Size = new Vector2(98f, 56f),
									Subtexture = ContentManager.Get<Subtexture>(name),
									FillColor = Color.White,
									OutlineColor = Color.Transparent,
									Margin = new Vector2(10f, 0f)
								},
								new StackPanelWidget
								{
									Direction = LayoutDirection.Vertical,
									VerticalAlignment = WidgetAlignment.Center,
									Margin = new Vector2(10f, 0f),
									Children = 
									{
										new LabelWidget
										{
											Text = StringsManager.GetString("ScreenLayout." + str + ".Name"),
											Font = ContentManager.Get<BitmapFont>("Fonts/Pericles")
										},
										new LabelWidget
										{
											Text = StringsManager.GetString("ScreenLayout." + str + ".Description"),
											Font = ContentManager.Get<BitmapFont>("Fonts/Pericles"),
											Color = Color.Gray
										}
									}
								}
							}
						};
					}, delegate(object o)
					{
						if (o != null)
						{
							if (this.m_subsystemPlayers.PlayersData.Count == 1)
							{
								SettingsManager.ScreenLayout1 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 2)
							{
								SettingsManager.ScreenLayout2 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 3)
							{
								SettingsManager.ScreenLayout3 = (ScreenLayout)o;
							}
							if (this.m_subsystemPlayers.PlayersData.Count == 4)
							{
								SettingsManager.ScreenLayout4 = (ScreenLayout)o;
							}
						}
					}));
				}
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen("Game", Array.Empty<object>());
			}
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0002340C File Offset: 0x0002160C
		public void UpdatePlayersPanel()
		{
			this.m_playersPanel.Children.Clear();
			foreach (PlayerData playerData in this.m_subsystemPlayers.PlayersData)
			{
				this.m_playersPanel.Children.Add(new PlayerWidget(playerData, this.m_characterSkinsCache));
			}
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x0002348C File Offset: 0x0002168C
		public void PlayersChanged(PlayerData playerData)
		{
			this.UpdatePlayersPanel();
		}

		// Token: 0x040002EC RID: 748
		public StackPanelWidget m_playersPanel;

		// Token: 0x040002ED RID: 749
		public ButtonWidget m_addPlayerButton;

		// Token: 0x040002EE RID: 750
		public ButtonWidget m_screenLayoutButton;

		// Token: 0x040002EF RID: 751
		public SubsystemPlayers m_subsystemPlayers;

		// Token: 0x040002F0 RID: 752
		public CharacterSkinsCache m_characterSkinsCache = new CharacterSkinsCache();
	}
}
