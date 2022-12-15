using System;
using Engine;

namespace Game
{
	// Token: 0x02000380 RID: 896
	public class FireWidget : CanvasWidget
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x060019A4 RID: 6564 RVA: 0x000C944B File Offset: 0x000C764B
		// (set) Token: 0x060019A5 RID: 6565 RVA: 0x000C9458 File Offset: 0x000C7658
		public float ParticlesPerSecond
		{
			get
			{
				return this.m_fireRenderer.ParticlesPerSecond;
			}
			set
			{
				this.m_fireRenderer.ParticlesPerSecond = value;
			}
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x000C9466 File Offset: 0x000C7666
		public FireWidget()
		{
			base.ClampToBounds = true;
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x000C9482 File Offset: 0x000C7682
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x000C9492 File Offset: 0x000C7692
		public override void Draw(Widget.DrawContext dc)
		{
			this.m_fireRenderer.Draw(dc.PrimitivesRenderer2D, 0f, base.GlobalTransform, base.GlobalColorTransform);
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x000C94B8 File Offset: 0x000C76B8
		public override void Update()
		{
			float dt = MathUtils.Clamp(Time.FrameDuration, 0f, 0.1f);
			this.m_fireRenderer.Origin = new Vector2(0f, base.ActualSize.Y);
			this.m_fireRenderer.CutoffPosition = float.NegativeInfinity;
			this.m_fireRenderer.ParticleSize = 32f;
			this.m_fireRenderer.ParticleSpeed = 32f;
			this.m_fireRenderer.Width = base.ActualSize.X;
			this.m_fireRenderer.MinTimeToLive = 0.5f;
			this.m_fireRenderer.MaxTimeToLive = 2f;
			this.m_fireRenderer.ParticleAnimationPeriod = 1.25f;
			this.m_fireRenderer.Update(dt);
		}

		// Token: 0x040011FE RID: 4606
		public ScreenSpaceFireRenderer m_fireRenderer = new ScreenSpaceFireRenderer(100);
	}
}
