using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200025A RID: 602
	public class EditBatteryDialog : Dialog
	{
		// Token: 0x0600121B RID: 4635 RVA: 0x0008B948 File Offset: 0x00089B48
		public EditBatteryDialog(int voltageLevel, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditBatteryDialog");
			base.LoadContents(this, node);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditBatteryDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditBatteryDialog.Cancel", true);
			this.m_voltageSlider = this.Children.Find<SliderWidget>("EditBatteryDialog.VoltageSlider", true);
			this.m_handler = handler;
			this.m_voltageLevel = voltageLevel;
			this.UpdateControls();
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0008B9C8 File Offset: 0x00089BC8
		public override void Update()
		{
			if (this.m_voltageSlider.IsSliding)
			{
				this.m_voltageLevel = (int)this.m_voltageSlider.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new int?(this.m_voltageLevel));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0008BA44 File Offset: 0x00089C44
		public void UpdateControls()
		{
			this.m_voltageSlider.Text = string.Format("{0:0.0}V ({1})", 1.5f * (float)this.m_voltageLevel / 15f, (this.m_voltageLevel < 8) ? "Low" : "High");
			this.m_voltageSlider.Value = (float)this.m_voltageLevel;
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0008BAA5 File Offset: 0x00089CA5
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000C2E RID: 3118
		public Action<int> m_handler;

		// Token: 0x04000C2F RID: 3119
		public ButtonWidget m_okButton;

		// Token: 0x04000C30 RID: 3120
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C31 RID: 3121
		public SliderWidget m_voltageSlider;

		// Token: 0x04000C32 RID: 3122
		public int m_voltageLevel;
	}
}
