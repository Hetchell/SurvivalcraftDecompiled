using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using Engine.Media;

namespace Game
{
	// Token: 0x02000132 RID: 306
	public class BestiaryDescriptionScreen : Screen
	{
		// Token: 0x060005A0 RID: 1440 RVA: 0x0001E7E4 File Offset: 0x0001C9E4
		public BestiaryDescriptionScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/BestiaryDescriptionScreen");
			base.LoadContents(this, node);
			this.m_modelWidget = this.Children.Find<ModelWidget>("Model", true);
			this.m_nameWidget = this.Children.Find<LabelWidget>("Name", true);
			this.m_leftButtonWidget = this.Children.Find<ButtonWidget>("Left", true);
			this.m_rightButtonWidget = this.Children.Find<ButtonWidget>("Right", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("Description", true);
			this.m_propertyNames1Widget = this.Children.Find<LabelWidget>("PropertyNames1", true);
			this.m_propertyValues1Widget = this.Children.Find<LabelWidget>("PropertyValues1", true);
			this.m_propertyNames2Widget = this.Children.Find<LabelWidget>("PropertyNames2", true);
			this.m_propertyValues2Widget = this.Children.Find<LabelWidget>("PropertyValues2", true);
			this.m_dropsPanel = this.Children.Find<ContainerWidget>("Drops", true);
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001E8F0 File Offset: 0x0001CAF0
		public override void Enter(object[] parameters)
		{
			BestiaryCreatureInfo item = (BestiaryCreatureInfo)parameters[0];
			this.m_infoList = (IList<BestiaryCreatureInfo>)parameters[1];
			this.m_index = this.m_infoList.IndexOf(item);
			this.UpdateCreatureProperties();
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0001E92C File Offset: 0x0001CB2C
		public override void Update()
		{
			this.m_leftButtonWidget.IsEnabled = (this.m_index > 0);
			this.m_rightButtonWidget.IsEnabled = (this.m_index < this.m_infoList.Count - 1);
			if (this.m_leftButtonWidget.IsClicked || base.Input.Left)
			{
				this.m_index = MathUtils.Max(this.m_index - 1, 0);
				this.UpdateCreatureProperties();
			}
			if (this.m_rightButtonWidget.IsClicked || base.Input.Right)
			{
				this.m_index = MathUtils.Min(this.m_index + 1, this.m_infoList.Count - 1);
				this.UpdateCreatureProperties();
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0001EA24 File Offset: 0x0001CC24
		public void UpdateCreatureProperties()
		{
			if (this.m_index >= 0 && this.m_index < this.m_infoList.Count)
			{
				BestiaryCreatureInfo bestiaryCreatureInfo = this.m_infoList[this.m_index];
				this.m_modelWidget.AutoRotationVector = new Vector3(0f, 1f, 0f);
				BestiaryScreen.SetupBestiaryModelWidget(bestiaryCreatureInfo, this.m_modelWidget, new Vector3(-1f, 0f, -1f), true, true);
				this.m_nameWidget.Text = bestiaryCreatureInfo.DisplayName;
				this.m_descriptionWidget.Text = bestiaryCreatureInfo.Description;
				this.m_propertyNames1Widget.Text = string.Empty;
				this.m_propertyValues1Widget.Text = string.Empty;
				LabelWidget propertyNames1Widget = this.m_propertyNames1Widget;
				propertyNames1Widget.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "resilience");
				LabelWidget propertyValues1Widget = this.m_propertyValues1Widget;
				propertyValues1Widget.Text = propertyValues1Widget.Text + bestiaryCreatureInfo.AttackResilience.ToString() + "\n";
				LabelWidget propertyNames1Widget2 = this.m_propertyNames1Widget;
				propertyNames1Widget2.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "attack");
				LabelWidget propertyValues1Widget2 = this.m_propertyValues1Widget;
				propertyValues1Widget2.Text = propertyValues1Widget2.Text + ((bestiaryCreatureInfo.AttackPower > 0f) ? bestiaryCreatureInfo.AttackPower.ToString("0.0") : LanguageControl.Get("Usual", "none")) + "\n";
				LabelWidget propertyNames1Widget3 = this.m_propertyNames1Widget;
				propertyNames1Widget3.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "herding");
				LabelWidget propertyValues1Widget3 = this.m_propertyValues1Widget;
				propertyValues1Widget3.Text = propertyValues1Widget3.Text + (bestiaryCreatureInfo.IsHerding ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no")) + "\n";
				LabelWidget propertyNames1Widget4 = this.m_propertyNames1Widget;
				propertyNames1Widget4.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, 1);
				LabelWidget propertyValues1Widget4 = this.m_propertyValues1Widget;
				propertyValues1Widget4.Text = propertyValues1Widget4.Text + (bestiaryCreatureInfo.CanBeRidden ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no")) + "\n";
				this.m_propertyNames1Widget.Text = this.m_propertyNames1Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyValues1Widget.Text = this.m_propertyValues1Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyNames2Widget.Text = string.Empty;
				this.m_propertyValues2Widget.Text = string.Empty;
				LabelWidget propertyNames2Widget = this.m_propertyNames2Widget;
				propertyNames2Widget.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "speed");
				LabelWidget propertyValues2Widget = this.m_propertyValues2Widget;
				propertyValues2Widget.Text = propertyValues2Widget.Text + ((double)bestiaryCreatureInfo.MovementSpeed * 3.6).ToString("0") + LanguageControl.Get(BestiaryDescriptionScreen.fName, "speed unit");
				LabelWidget propertyNames2Widget2 = this.m_propertyNames2Widget;
				propertyNames2Widget2.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "jump height");
				LabelWidget propertyValues2Widget2 = this.m_propertyValues2Widget;
				propertyValues2Widget2.Text = propertyValues2Widget2.Text + bestiaryCreatureInfo.JumpHeight.ToString("0.0") + LanguageControl.Get(BestiaryDescriptionScreen.fName, "length unit");
				LabelWidget propertyNames2Widget3 = this.m_propertyNames2Widget;
				propertyNames2Widget3.Text += LanguageControl.Get(BestiaryDescriptionScreen.fName, "weight");
				LabelWidget propertyValues2Widget3 = this.m_propertyValues2Widget;
				propertyValues2Widget3.Text = propertyValues2Widget3.Text + bestiaryCreatureInfo.Mass.ToString() + LanguageControl.Get(BestiaryDescriptionScreen.fName, "weight unit");
				LabelWidget propertyNames2Widget4 = this.m_propertyNames2Widget;
				propertyNames2Widget4.Text += LanguageControl.Get("BlocksManager", "Spawner Eggs");
				LabelWidget propertyValues2Widget4 = this.m_propertyValues2Widget;
				propertyValues2Widget4.Text = propertyValues2Widget4.Text + (bestiaryCreatureInfo.HasSpawnerEgg ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no")) + "\n";
				this.m_propertyNames2Widget.Text = this.m_propertyNames2Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_propertyValues2Widget.Text = this.m_propertyValues2Widget.Text.TrimEnd(Array.Empty<char>());
				this.m_dropsPanel.Children.Clear();
				if (bestiaryCreatureInfo.Loot.Count > 0)
				{
					using (List<ComponentLoot.Loot>.Enumerator enumerator = bestiaryCreatureInfo.Loot.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ComponentLoot.Loot loot = enumerator.Current;
							string text = (loot.MinCount >= loot.MaxCount) ? string.Format("{0}", loot.MinCount) : string.Format(LanguageControl.Get(BestiaryDescriptionScreen.fName, "range"), loot.MinCount, loot.MaxCount);
							if (loot.Probability < 1f)
							{
								text += string.Format(LanguageControl.Get(BestiaryDescriptionScreen.fName, 2), string.Format("{0:0}", loot.Probability * 100f));
							}
							this.m_dropsPanel.Children.Add(new StackPanelWidget
							{
								Margin = new Vector2(20f, 0f),
								Children = 
								{
									new BlockIconWidget
									{
										Size = new Vector2(32f),
										Scale = 1.2f,
										VerticalAlignment = WidgetAlignment.Center,
										Value = loot.Value
									},
									new CanvasWidget
									{
										Size = new Vector2(10f, 0f)
									},
									new LabelWidget
									{
										VerticalAlignment = WidgetAlignment.Center,
										Text = text
									}
								}
							});
						}
						return;
					}
				}
				this.m_dropsPanel.Children.Add(new LabelWidget
				{
					Margin = new Vector2(20f, 0f),
					Font = ContentManager.Get<BitmapFont>("Fonts/Pericles"),
					Text = LanguageControl.Get("Usual", "nothing")
				});
			}
		}

		// Token: 0x0400027D RID: 637
		public ModelWidget m_modelWidget;

		// Token: 0x0400027E RID: 638
		public LabelWidget m_nameWidget;

		// Token: 0x0400027F RID: 639
		public ButtonWidget m_leftButtonWidget;

		// Token: 0x04000280 RID: 640
		public ButtonWidget m_rightButtonWidget;

		// Token: 0x04000281 RID: 641
		public LabelWidget m_descriptionWidget;

		// Token: 0x04000282 RID: 642
		public LabelWidget m_propertyNames1Widget;

		// Token: 0x04000283 RID: 643
		public LabelWidget m_propertyValues1Widget;

		// Token: 0x04000284 RID: 644
		public LabelWidget m_propertyNames2Widget;

		// Token: 0x04000285 RID: 645
		public LabelWidget m_propertyValues2Widget;

		// Token: 0x04000286 RID: 646
		public ContainerWidget m_dropsPanel;

		// Token: 0x04000287 RID: 647
		public static string fName = "BestiaryDescriptionScreen";

		// Token: 0x04000288 RID: 648
		public int m_index;

		// Token: 0x04000289 RID: 649
		public IList<BestiaryCreatureInfo> m_infoList;
	}
}
