using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000143 RID: 323
	public class RecipaediaDescriptionScreen : Screen
	{
		// Token: 0x06000614 RID: 1556 RVA: 0x00023C74 File Offset: 0x00021E74
		public RecipaediaDescriptionScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/RecipaediaDescriptionScreen");
			base.LoadContents(this, node);
			this.m_blockIconWidget = this.Children.Find<BlockIconWidget>("Icon", true);
			this.m_nameWidget = this.Children.Find<LabelWidget>("Name", true);
			this.m_leftButtonWidget = this.Children.Find<ButtonWidget>("Left", true);
			this.m_rightButtonWidget = this.Children.Find<ButtonWidget>("Right", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("Description", true);
			this.m_propertyNames1Widget = this.Children.Find<LabelWidget>("PropertyNames1", true);
			this.m_propertyValues1Widget = this.Children.Find<LabelWidget>("PropertyValues1", true);
			this.m_propertyNames2Widget = this.Children.Find<LabelWidget>("PropertyNames2", true);
			this.m_propertyValues2Widget = this.Children.Find<LabelWidget>("PropertyValues2", true);
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00023D6C File Offset: 0x00021F6C
		public override void Enter(object[] parameters)
		{
			int item = (int)parameters[0];
			this.m_valuesList = (IList<int>)parameters[1];
			this.m_index = this.m_valuesList.IndexOf(item);
			this.UpdateBlockProperties();
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x00023DA8 File Offset: 0x00021FA8
		public override void Update()
		{
			this.m_leftButtonWidget.IsEnabled = (this.m_index > 0);
			this.m_rightButtonWidget.IsEnabled = (this.m_index < this.m_valuesList.Count - 1);
			if (this.m_leftButtonWidget.IsClicked || base.Input.Left)
			{
				this.m_index = MathUtils.Max(this.m_index - 1, 0);
				this.UpdateBlockProperties();
			}
			if (this.m_rightButtonWidget.IsClicked || base.Input.Right)
			{
				this.m_index = MathUtils.Min(this.m_index + 1, this.m_valuesList.Count - 1);
				this.UpdateBlockProperties();
			}
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00023EA0 File Offset: 0x000220A0
		public Dictionary<string, string> GetBlockProperties(int value)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.DefaultEmittedLightAmount > 0)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 1), block.DefaultEmittedLightAmount.ToString());
			}
			if (block.FuelFireDuration > 0f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 2), block.FuelFireDuration.ToString());
			}
			dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 3), (block.MaxStacking > 1) ? string.Format(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 4), block.MaxStacking.ToString()) : LanguageControl.Get("Usual", "no"));
			dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 5), (block.FireDuration > 0f) ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
			if (block.GetNutritionalValue(value) > 0f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 6), block.GetNutritionalValue(value).ToString());
			}
			if (block.GetRotPeriod(value) > 0)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 7), string.Format(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 25), string.Format("{0:0.0}", (float)(2 * block.GetRotPeriod(value)) * 60f / 1200f)));
			}
			if (block.DigMethod != BlockDigMethod.None)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 8), LanguageControl.Get("DigMethod", block.DigMethod.ToString()));
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 9), block.DigResilience.ToString());
			}
			if (block.ExplosionResilience > 0f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 10), block.ExplosionResilience.ToString());
			}
			if (block.GetExplosionPressure(value) > 0f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 11), block.GetExplosionPressure(value).ToString());
			}
			bool flag = false;
			if (block.GetMeleePower(value) > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 12), block.GetMeleePower(value).ToString());
				flag = true;
			}
			if (block.GetMeleePower(value) > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 13), string.Format("{0:0}%", 100f * block.GetMeleeHitProbability(value)));
				flag = true;
			}
			if (block.GetProjectilePower(value) > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 14), block.GetProjectilePower(value).ToString());
				flag = true;
			}
			if (block.ShovelPower > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 15), block.ShovelPower.ToString());
				flag = true;
			}
			if (block.HackPower > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 16), block.HackPower.ToString());
				flag = true;
			}
			if (block.QuarryPower > 1f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 17), block.QuarryPower.ToString());
				flag = true;
			}
			if (flag && block.Durability > 0)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 18), block.Durability.ToString());
			}
			if (block.DefaultExperienceCount > 0f)
			{
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 19), block.DefaultExperienceCount.ToString());
			}
			if (block is ClothingBlock)
			{
				ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 20), clothingData.CanBeDyed ? LanguageControl.Get("Usual", "yes") : LanguageControl.Get("Usual", "no"));
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 21), string.Format("{0}%", (int)(clothingData.ArmorProtection * 100f)));
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 22), clothingData.Sturdiness.ToString());
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 23), string.Format("{0:0.0} clo", clothingData.Insulation));
				dictionary.Add(LanguageControl.Get(RecipaediaDescriptionScreen.fName, 24), string.Format("{0:0}%", clothingData.MovementSpeedFactor * 100f));
			}
			return dictionary;
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00024334 File Offset: 0x00022534
		public void UpdateBlockProperties()
		{
			if (this.m_index >= 0 && this.m_index < this.m_valuesList.Count)
			{
				int value = this.m_valuesList[this.m_index];
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				this.m_blockIconWidget.Value = value;
				this.m_nameWidget.Text = block.GetDisplayName(null, value);
				this.m_descriptionWidget.Text = block.GetDescription(value);
				this.m_propertyNames1Widget.Text = string.Empty;
				this.m_propertyValues1Widget.Text = string.Empty;
				this.m_propertyNames2Widget.Text = string.Empty;
				this.m_propertyValues2Widget.Text = string.Empty;
				Dictionary<string, string> blockProperties = this.GetBlockProperties(value);
				int num2 = 0;
				foreach (KeyValuePair<string, string> keyValuePair in blockProperties)
				{
					if (num2 < blockProperties.Count - blockProperties.Count / 2)
					{
						LabelWidget propertyNames1Widget = this.m_propertyNames1Widget;
						propertyNames1Widget.Text = propertyNames1Widget.Text + keyValuePair.Key + ":\n";
						LabelWidget propertyValues1Widget = this.m_propertyValues1Widget;
						propertyValues1Widget.Text = propertyValues1Widget.Text + keyValuePair.Value + "\n";
					}
					else
					{
						LabelWidget propertyNames2Widget = this.m_propertyNames2Widget;
						propertyNames2Widget.Text = propertyNames2Widget.Text + keyValuePair.Key + ":\n";
						LabelWidget propertyValues2Widget = this.m_propertyValues2Widget;
						propertyValues2Widget.Text = propertyValues2Widget.Text + keyValuePair.Value + "\n";
					}
					num2++;
				}
			}
		}

		// Token: 0x040002F5 RID: 757
		public BlockIconWidget m_blockIconWidget;

		// Token: 0x040002F6 RID: 758
		public LabelWidget m_nameWidget;

		// Token: 0x040002F7 RID: 759
		public ButtonWidget m_leftButtonWidget;

		// Token: 0x040002F8 RID: 760
		public ButtonWidget m_rightButtonWidget;

		// Token: 0x040002F9 RID: 761
		public LabelWidget m_descriptionWidget;

		// Token: 0x040002FA RID: 762
		public LabelWidget m_propertyNames1Widget;

		// Token: 0x040002FB RID: 763
		public LabelWidget m_propertyValues1Widget;

		// Token: 0x040002FC RID: 764
		public LabelWidget m_propertyNames2Widget;

		// Token: 0x040002FD RID: 765
		public LabelWidget m_propertyValues2Widget;

		// Token: 0x040002FE RID: 766
		public int m_index;

		// Token: 0x040002FF RID: 767
		public IList<int> m_valuesList;

		// Token: 0x04000300 RID: 768
		public static string fName = "RecipaediaDescriptionScreen";
	}
}
