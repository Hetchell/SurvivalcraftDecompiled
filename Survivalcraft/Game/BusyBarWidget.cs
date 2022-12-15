using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200036E RID: 878
	public class BusyBarWidget : Widget
	{
		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x0600190D RID: 6413 RVA: 0x000C5C71 File Offset: 0x000C3E71
		// (set) Token: 0x0600190E RID: 6414 RVA: 0x000C5C79 File Offset: 0x000C3E79
		public Color LitBarColor { get; set; }

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x0600190F RID: 6415 RVA: 0x000C5C82 File Offset: 0x000C3E82
		// (set) Token: 0x06001910 RID: 6416 RVA: 0x000C5C8A File Offset: 0x000C3E8A
		public Color UnlitBarColor { get; set; }

		// Token: 0x06001911 RID: 6417 RVA: 0x000C5C93 File Offset: 0x000C3E93
		public BusyBarWidget()
		{
			this.IsHitTestVisible = false;
			this.LitBarColor = new Color(16, 140, 0);
			this.UnlitBarColor = new Color(48, 48, 48);
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x000C5CC6 File Offset: 0x000C3EC6
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = new Vector2(120f, 12f);
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x000C5CE4 File Offset: 0x000C3EE4
		public override void Draw(Widget.DrawContext dc)
		{
			if (Time.RealTime - this.m_lastBoxesStepTime > 0.25)
			{
				this.m_boxIndex++;
				this.m_lastBoxesStepTime = Time.RealTime;
			}
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, null, null, null);
			int count = flatBatch2D.TriangleVertices.Count;
			for (int i = 0; i < 5; i++)
			{
				Vector2 v = new Vector2(((float)i + 0.5f) * 24f, 6f);
				Color c = (i == this.m_boxIndex % 5) ? this.LitBarColor : this.UnlitBarColor;
				float v2 = (i == this.m_boxIndex % 5) ? 12f : 8f;
				flatBatch2D.QueueQuad(v - new Vector2(v2) / 2f, v + new Vector2(v2) / 2f, 0f, c * base.GlobalColorTransform);
			}
			flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x04001195 RID: 4501
		public const int m_barsCount = 5;

		// Token: 0x04001196 RID: 4502
		public const float m_barSize = 8f;

		// Token: 0x04001197 RID: 4503
		public const float m_barsSpacing = 24f;

		// Token: 0x04001198 RID: 4504
		public int m_boxIndex;

		// Token: 0x04001199 RID: 4505
		public double m_lastBoxesStepTime;
	}
}
