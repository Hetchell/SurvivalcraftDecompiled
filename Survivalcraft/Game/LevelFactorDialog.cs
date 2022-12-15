using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x020002A2 RID: 674
	public class LevelFactorDialog : Dialog
	{
		// Token: 0x06001386 RID: 4998 RVA: 0x000976C0 File Offset: 0x000958C0
		public LevelFactorDialog(string title, string description, IEnumerable<ComponentLevel.Factor> factors, float total)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/LevelFactorDialog");
			base.LoadContents(this, node);
			this.m_titleWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Title", true);
			this.m_descriptionWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Description", true);
			this.m_namesWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Names", true);
			this.m_valuesWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.Values", true);
			this.m_totalNameWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.TotalName", true);
			this.m_totalValueWidget = this.Children.Find<LabelWidget>("LevelFactorDialog.TotalValue", true);
			this.m_okWidget = this.Children.Find<ButtonWidget>("LevelFactorDialog.OK", true);
			this.m_titleWidget.Text = title;
			this.m_descriptionWidget.Text = description;
			this.m_namesWidget.Text = string.Empty;
			this.m_valuesWidget.Text = string.Empty;
			foreach (ComponentLevel.Factor factor in factors)
			{
				LabelWidget namesWidget = this.m_namesWidget;
				namesWidget.Text += string.Format("{0,24}\n", factor.Description);
				LabelWidget valuesWidget = this.m_valuesWidget;
				valuesWidget.Text += string.Format(CultureInfo.InvariantCulture, "x {0:0.00}\n", factor.Value);
			}
			this.m_namesWidget.Text = this.m_namesWidget.Text.TrimEnd(Array.Empty<char>());
			this.m_valuesWidget.Text = this.m_valuesWidget.Text.TrimEnd(Array.Empty<char>());
			this.m_totalNameWidget.Text = string.Format("{0,24}", "TOTAL");
			this.m_totalValueWidget.Text = string.Format(CultureInfo.InvariantCulture, "x {0:0.00}", total);
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x000978C4 File Offset: 0x00095AC4
		public override void Update()
		{
			if (base.Input.Cancel || this.m_okWidget.IsClicked)
			{
				DialogsManager.HideDialog(this);
			}
		}

		// Token: 0x04000D58 RID: 3416
		public LabelWidget m_titleWidget;

		// Token: 0x04000D59 RID: 3417
		public LabelWidget m_descriptionWidget;

		// Token: 0x04000D5A RID: 3418
		public LabelWidget m_namesWidget;

		// Token: 0x04000D5B RID: 3419
		public LabelWidget m_valuesWidget;

		// Token: 0x04000D5C RID: 3420
		public LabelWidget m_totalNameWidget;

		// Token: 0x04000D5D RID: 3421
		public LabelWidget m_totalValueWidget;

		// Token: 0x04000D5E RID: 3422
		public ButtonWidget m_okWidget;
	}
}
