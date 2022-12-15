using System;
using System.Xml.Linq;

namespace Game
{
	// Token: 0x0200025E RID: 606
	public class EditPistonDialog : Dialog
	{
		// Token: 0x0600122C RID: 4652 RVA: 0x0008CB0C File Offset: 0x0008AD0C
		public EditPistonDialog(int data, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditPistonDialog");
			base.LoadContents(this, node);
			this.m_title = this.Children.Find<LabelWidget>("EditPistonDialog.Title", true);
			this.m_slider1 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider1", true);
			this.m_panel2 = this.Children.Find<ContainerWidget>("EditPistonDialog.Panel2", true);
			this.m_slider2 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider2", true);
			this.m_slider3 = this.Children.Find<SliderWidget>("EditPistonDialog.Slider3", true);
			this.m_okButton = this.Children.Find<ButtonWidget>("EditPistonDialog.OK", true);
			this.m_cancelButton = this.Children.Find<ButtonWidget>("EditPistonDialog.Cancel", true);
			this.m_handler = handler;
			this.m_data = data;
			this.m_mode = PistonBlock.GetMode(data);
			this.m_maxExtension = PistonBlock.GetMaxExtension(data);
			this.m_pullCount = PistonBlock.GetPullCount(data);
			this.m_speed = PistonBlock.GetSpeed(data);
			this.m_title.Text = "Edit " + BlocksManager.Blocks[237].GetDisplayName(null, Terrain.MakeBlockValue(237, 0, data));
			this.m_slider1.Granularity = 1f;
			this.m_slider1.MinValue = 1f;
			this.m_slider1.MaxValue = 8f;
			this.m_slider2.Granularity = 1f;
			this.m_slider2.MinValue = 1f;
			this.m_slider2.MaxValue = 8f;
			this.m_slider3.Granularity = 1f;
			this.m_slider3.MinValue = 0f;
			this.m_slider3.MaxValue = 3f;
			this.m_panel2.IsVisible = (this.m_mode > PistonMode.Pushing);
			this.UpdateControls();
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x0008CCF0 File Offset: 0x0008AEF0
		public override void Update()
		{
			if (this.m_slider1.IsSliding)
			{
				this.m_maxExtension = (int)this.m_slider1.Value - 1;
			}
			if (this.m_slider2.IsSliding)
			{
				this.m_pullCount = (int)this.m_slider2.Value - 1;
			}
			if (this.m_slider3.IsSliding)
			{
				this.m_speed = (int)this.m_slider3.Value;
			}
			if (this.m_okButton.IsClicked)
			{
				int value = PistonBlock.SetMaxExtension(PistonBlock.SetPullCount(PistonBlock.SetSpeed(this.m_data, this.m_speed), this.m_pullCount), this.m_maxExtension);
				this.Dismiss(new int?(value));
			}
			if (base.Input.Cancel || this.m_cancelButton.IsClicked)
			{
				this.Dismiss(null);
			}
			this.UpdateControls();
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x0008CDD0 File Offset: 0x0008AFD0
		public void UpdateControls()
		{
			this.m_slider1.Value = (float)(this.m_maxExtension + 1);
			this.m_slider1.Text = string.Format("{0} blocks", this.m_maxExtension + 1);
			this.m_slider2.Value = (float)(this.m_pullCount + 1);
			this.m_slider2.Text = string.Format("{0} blocks", this.m_pullCount + 1);
			this.m_slider3.Value = (float)this.m_speed;
			this.m_slider3.Text = EditPistonDialog.m_speedNames[this.m_speed];
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x0008CE72 File Offset: 0x0008B072
		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (this.m_handler != null && result != null)
			{
				this.m_handler(result.Value);
			}
		}

		// Token: 0x04000C4F RID: 3151
		public LabelWidget m_title;

		// Token: 0x04000C50 RID: 3152
		public SliderWidget m_slider1;

		// Token: 0x04000C51 RID: 3153
		public SliderWidget m_slider2;

		// Token: 0x04000C52 RID: 3154
		public ContainerWidget m_panel2;

		// Token: 0x04000C53 RID: 3155
		public SliderWidget m_slider3;

		// Token: 0x04000C54 RID: 3156
		public ButtonWidget m_okButton;

		// Token: 0x04000C55 RID: 3157
		public ButtonWidget m_cancelButton;

		// Token: 0x04000C56 RID: 3158
		public Action<int> m_handler;

		// Token: 0x04000C57 RID: 3159
		public int m_data;

		// Token: 0x04000C58 RID: 3160
		public PistonMode m_mode;

		// Token: 0x04000C59 RID: 3161
		public int m_maxExtension;

		// Token: 0x04000C5A RID: 3162
		public int m_pullCount;

		// Token: 0x04000C5B RID: 3163
		public int m_speed;

		// Token: 0x04000C5C RID: 3164
		public static string[] m_speedNames = new string[]
		{
			"Fast",
			"Medium",
			"Slow",
			"Very Slow"
		};
	}
}
