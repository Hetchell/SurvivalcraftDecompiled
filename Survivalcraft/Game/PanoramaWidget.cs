using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000394 RID: 916
	public class PanoramaWidget : Widget
	{
		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06001A97 RID: 6807 RVA: 0x000D156A File Offset: 0x000CF76A
		// (set) Token: 0x06001A98 RID: 6808 RVA: 0x000D1572 File Offset: 0x000CF772
		public Texture2D Texture { get; set; }

		// Token: 0x06001A99 RID: 6809 RVA: 0x000D157B File Offset: 0x000CF77B
		public PanoramaWidget()
		{
			this.Texture = ContentManager.Get<Texture2D>("Textures/Gui/Panorama");
			this.m_timeOffset = new Game.Random().Float(0f, 1000f);
		}

		// Token: 0x06001A9A RID: 6810 RVA: 0x000D15B0 File Offset: 0x000CF7B0
		public void DrawImage(Widget.DrawContext dc)
		{
			float num = (float)MathUtils.Remainder(Time.FrameStartTime + (double)this.m_timeOffset, 10000.0);
			float x = 2f * SimplexNoise.OctavedNoise(num, 0.02f, 4, 2f, 0.5f, false) - 1f;
			float y = 2f * SimplexNoise.OctavedNoise(num + 100f, 0.02f, 4, 2f, 0.5f, false) - 1f;
			this.m_position += 0.03f * new Vector2(x, y) * MathUtils.Min(Time.FrameDuration, 0.1f);
			this.m_position.X = MathUtils.Remainder(this.m_position.X, 1f);
			this.m_position.Y = MathUtils.Remainder(this.m_position.Y, 1f);
			float f = 0.5f * MathUtils.PowSign(MathUtils.Sin(0.21f * num + 2f), 2f) + 0.5f;
			float num2 = MathUtils.Lerp(0.13f, 0.3f, f);
			float num3 = num2 / (float)this.Texture.Height * (float)this.Texture.Width / base.ActualSize.X * base.ActualSize.Y;
			float x2 = this.m_position.X;
			float y2 = this.m_position.Y;
			Vector2 zero = Vector2.Zero;
			Vector2 actualSize = base.ActualSize;
			Vector2 texCoord = new Vector2(x2 - num2, y2 - num3);
			Vector2 texCoord2 = new Vector2(x2 + num2, y2 + num3);
			TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.Texture, false, 0, DepthStencilState.DepthWrite, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
			int count = texturedBatch2D.TriangleVertices.Count;
			texturedBatch2D.QueueQuad(zero, actualSize, 1f, texCoord, texCoord2, base.GlobalColorTransform);
			texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x06001A9B RID: 6811 RVA: 0x000D17B4 File Offset: 0x000CF9B4
		public void DrawSquares(Widget.DrawContext dc)
		{
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, BlendState.AlphaBlend);
			int count = flatBatch2D.LineVertices.Count;
			int count2 = flatBatch2D.TriangleVertices.Count;
			float num = (float)MathUtils.Remainder(Time.FrameStartTime + (double)this.m_timeOffset, 10000.0);
			float num2 = base.ActualSize.X / 12f;
			float num3 = (float)base.GlobalColorTransform.A / 255f;
			for (float num4 = 0f; num4 < base.ActualSize.X; num4 += num2)
			{
				for (float num5 = 0f; num5 < base.ActualSize.Y; num5 += num2)
				{
					float num6 = 0.35f * MathUtils.Pow(MathUtils.Saturate(SimplexNoise.OctavedNoise(num4 + 1000f, num5, 0.7f * num, 0.5f, 1, 2f, 1f, false) - 0.1f), 1f) * num3;
					float num7 = 0.7f * MathUtils.Pow(SimplexNoise.OctavedNoise(num4, num5, 0.5f * num, 0.5f, 1, 2f, 1f, false), 3f) * num3;
					Vector2 corner = new Vector2(num4, num5);
					Vector2 corner2 = new Vector2(num4 + num2, num5 + num2);
					if (num6 > 0.01f)
					{
						flatBatch2D.QueueRectangle(corner, corner2, 0f, new Color(0f, 0f, 0f, num6));
					}
					if (num7 > 0.01f)
					{
						flatBatch2D.QueueQuad(corner, corner2, 0f, new Color(0f, 0f, 0f, num7));
					}
				}
			}
			flatBatch2D.TransformLines(base.GlobalTransform, count, -1);
			flatBatch2D.TransformTriangles(base.GlobalTransform, count2, -1);
		}

		// Token: 0x06001A9C RID: 6812 RVA: 0x000D1994 File Offset: 0x000CFB94
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x000D199D File Offset: 0x000CFB9D
		public override void Draw(Widget.DrawContext dc)
		{
			this.DrawImage(dc);
			this.DrawSquares(dc);
		}

		// Token: 0x0400128D RID: 4749
		public Vector2 m_position;

		// Token: 0x0400128E RID: 4750
		public float m_timeOffset;
	}
}
