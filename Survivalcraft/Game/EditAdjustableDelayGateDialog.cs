using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
	// Token: 0x02000259 RID: 601
	public class EditAdjustableDelayGateDialog : Dialog
	{
		// Token: 0x06001217 RID: 4631 RVA: 0x0008B6FC File Offset: 0x000898FC
		public EditAdjustableDelayGateDialog(int delay, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditAdjustableDelayGateDialog");
			base.LoadContents(this, node);
			this.m_delaySlider = this.Children.Find<SliderWidget>("EditAdjustableDelayGateDialog.DelaySlider", true);
			this.m_plusButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.PlusButton", true);
			this.m_minusButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.MinusButton", true);
			this.m_delayLabel = this.Children.Find<LabelWidget>("EditAdjustableDelayGateDialog.Label", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel", true);
			this.m_handler = handler;
			this.m_delay = delay;
			this.UpdateControls();
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x0008B7C0 File Offset: 0x000899C0
		public override void Update()
		{
			if (this.m_delaySlider.IsSliding)
			{
				this.m_delay = (int)this.m_delaySlider.Value;
			}
			if (this.m_minusButton.IsClicked)
			{
				this.m_delay = MathUtils.Max(this.m_delay - 1, (int)this.m_delaySlider.MinValue);
			}
			if (this.m_plusButton.IsClicked)
			{
				this.m_delay = MathUtils.Min(this.m_delay + 1, (int)this.m_delaySlider.MaxValue);
			}
			if (this.m_okButton.IsClicked)
			{
				this.Dismiss(new int?(this.m_delay));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0008B894 File Offset: 0x00089A94
		public void UpdateControls()
		{
			this.m_delaySlider.Value = (float)this.m_delay;
			this.m_minusButton.IsEnabled = ((float)this.m_delay > this.m_delaySlider.MinValue);
			this.m_plusButton.IsEnabled = ((float)this.m_delay < this.m_delaySlider.MaxValue);
			this.m_delayLabel.Text = string.Format("{0:0.00} seconds", (float)(this.m_delay + 1) * 0.01f);
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0008B91A File Offset: 0x00089B1A
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000C26 RID: 3110
		public Action<int> m_handler;

		// Token: 0x04000C27 RID: 3111
		public SliderWidget m_delaySlider;

		// Token: 0x04000C28 RID: 3112
		public ButtonWidget m_plusButton;

		// Token: 0x04000C29 RID: 3113
		public ButtonWidget m_minusButton;

		// Token: 0x04000C2A RID: 3114
		public LabelWidget m_delayLabel;

		// Token: 0x04000C2B RID: 3115
		public ButtonWidget m_okButton;

		// Token: 0x04000C2C RID: 3116
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C2D RID: 3117
		public int m_delay;
	}
}
