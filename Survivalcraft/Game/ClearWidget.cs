using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000373 RID: 883
	public class ClearWidget : Widget
	{
		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x0600193F RID: 6463 RVA: 0x000C657E File Offset: 0x000C477E
		// (set) Token: 0x06001940 RID: 6464 RVA: 0x000C6586 File Offset: 0x000C4786
		public Color Color { get; set; }

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06001941 RID: 6465 RVA: 0x000C658F File Offset: 0x000C478F
		// (set) Token: 0x06001942 RID: 6466 RVA: 0x000C6597 File Offset: 0x000C4797
		public float Depth { get; set; }

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06001943 RID: 6467 RVA: 0x000C65A0 File Offset: 0x000C47A0
		// (set) Token: 0x06001944 RID: 6468 RVA: 0x000C65A8 File Offset: 0x000C47A8
		public int Stencil { get; set; }

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06001945 RID: 6469 RVA: 0x000C65B1 File Offset: 0x000C47B1
		// (set) Token: 0x06001946 RID: 6470 RVA: 0x000C65B9 File Offset: 0x000C47B9
		public bool ClearColor { get; set; }

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x000C65C2 File Offset: 0x000C47C2
		// (set) Token: 0x06001948 RID: 6472 RVA: 0x000C65CA File Offset: 0x000C47CA
		public bool ClearDepth { get; set; }

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x000C65D3 File Offset: 0x000C47D3
		// (set) Token: 0x0600194A RID: 6474 RVA: 0x000C65DB File Offset: 0x000C47DB
		public bool ClearStencil { get; set; }

		// Token: 0x0600194B RID: 6475 RVA: 0x000C65E4 File Offset: 0x000C47E4
		public ClearWidget()
		{
			this.ClearColor = true;
			this.ClearDepth = true;
			this.ClearStencil = true;
			this.Color = Color.Black;
			this.Depth = 1f;
			this.Stencil = 0;
			this.IsHitTestVisible = false;
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x000C6630 File Offset: 0x000C4830
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000C663C File Offset: 0x000C483C
		public override void Draw(Widget.DrawContext dc)
		{
			Display.Clear(this.ClearColor ? new Vector4?(new Vector4(this.Color)) : null, this.ClearDepth ? new float?(this.Depth) : null, this.ClearStencil ? new int?(this.Stencil) : null);
		}
	}
}
