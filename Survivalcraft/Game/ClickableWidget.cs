using System;

namespace Game
{
	// Token: 0x02000374 RID: 884
	public class ClickableWidget : Widget
	{
		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x0600194E RID: 6478 RVA: 0x000C66AD File Offset: 0x000C48AD
		// (set) Token: 0x0600194F RID: 6479 RVA: 0x000C66B5 File Offset: 0x000C48B5
		public string SoundName { get; set; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06001950 RID: 6480 RVA: 0x000C66BE File Offset: 0x000C48BE
		// (set) Token: 0x06001951 RID: 6481 RVA: 0x000C66C6 File Offset: 0x000C48C6
		public bool IsPressed { get; set; }

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06001952 RID: 6482 RVA: 0x000C66CF File Offset: 0x000C48CF
		// (set) Token: 0x06001953 RID: 6483 RVA: 0x000C66D7 File Offset: 0x000C48D7
		public bool IsClicked { get; set; }

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06001954 RID: 6484 RVA: 0x000C66E0 File Offset: 0x000C48E0
		// (set) Token: 0x06001955 RID: 6485 RVA: 0x000C66E8 File Offset: 0x000C48E8
		public bool IsTapped { get; set; }

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06001956 RID: 6486 RVA: 0x000C66F1 File Offset: 0x000C48F1
		// (set) Token: 0x06001957 RID: 6487 RVA: 0x000C66F9 File Offset: 0x000C48F9
		public bool IsChecked { get; set; }

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06001958 RID: 6488 RVA: 0x000C6702 File Offset: 0x000C4902
		// (set) Token: 0x06001959 RID: 6489 RVA: 0x000C670A File Offset: 0x000C490A
		public bool IsAutoCheckingEnabled { get; set; }

		// Token: 0x0600195A RID: 6490 RVA: 0x000C6713 File Offset: 0x000C4913
		public override void UpdateCeases()
		{
			base.UpdateCeases();
			this.IsPressed = false;
			this.IsClicked = false;
			this.IsTapped = false;
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x000C6730 File Offset: 0x000C4930
		public override void Update()
		{
			WidgetInput input = base.Input;
			this.IsPressed = false;
			this.IsTapped = false;
			this.IsClicked = false;
			if (input.Press != null && base.HitTestGlobal(input.Press.Value, null) == this)
			{
				this.IsPressed = true;
			}
			if (input.Tap != null && base.HitTestGlobal(input.Tap.Value, null) == this)
			{
				this.IsTapped = true;
			}
			if (input.Click != null && base.HitTestGlobal(input.Click.Value.Start, null) == this && base.HitTestGlobal(input.Click.Value.End, null) == this)
			{
				this.IsClicked = true;
				if (this.IsAutoCheckingEnabled)
				{
					this.IsChecked = !this.IsChecked;
				}
				if (!string.IsNullOrEmpty(this.SoundName))
				{
					AudioManager.PlaySound(this.SoundName, 1f, 0f, 0f);
				}
			}
		}
	}
}
