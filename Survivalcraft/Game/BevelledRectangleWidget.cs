using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200036A RID: 874
	public class BevelledRectangleWidget : Widget
	{
		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x060018CA RID: 6346 RVA: 0x000C4B80 File Offset: 0x000C2D80
		// (set) Token: 0x060018CB RID: 6347 RVA: 0x000C4B88 File Offset: 0x000C2D88
		public Vector2 Size { get; set; }

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x060018CC RID: 6348 RVA: 0x000C4B91 File Offset: 0x000C2D91
		// (set) Token: 0x060018CD RID: 6349 RVA: 0x000C4B99 File Offset: 0x000C2D99
		public float BevelSize { get; set; }

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x060018CE RID: 6350 RVA: 0x000C4BA2 File Offset: 0x000C2DA2
		// (set) Token: 0x060018CF RID: 6351 RVA: 0x000C4BAA File Offset: 0x000C2DAA
		public float DirectionalLight { get; set; }

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x060018D0 RID: 6352 RVA: 0x000C4BB3 File Offset: 0x000C2DB3
		// (set) Token: 0x060018D1 RID: 6353 RVA: 0x000C4BBB File Offset: 0x000C2DBB
		public float AmbientLight { get; set; }

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x060018D2 RID: 6354 RVA: 0x000C4BC4 File Offset: 0x000C2DC4
		// (set) Token: 0x060018D3 RID: 6355 RVA: 0x000C4BCC File Offset: 0x000C2DCC
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
			set
			{
				if (value != this.m_texture)
				{
					this.m_texture = value;
				}
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x060018D4 RID: 6356 RVA: 0x000C4BDE File Offset: 0x000C2DDE
		// (set) Token: 0x060018D5 RID: 6357 RVA: 0x000C4BE6 File Offset: 0x000C2DE6
		public float TextureScale { get; set; }

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x060018D6 RID: 6358 RVA: 0x000C4BEF File Offset: 0x000C2DEF
		// (set) Token: 0x060018D7 RID: 6359 RVA: 0x000C4BF7 File Offset: 0x000C2DF7
		public bool TextureLinearFilter
		{
			get
			{
				return this.m_textureLinearFilter;
			}
			set
			{
				if (value != this.m_textureLinearFilter)
				{
					this.m_textureLinearFilter = value;
				}
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x060018D8 RID: 6360 RVA: 0x000C4C09 File Offset: 0x000C2E09
		// (set) Token: 0x060018D9 RID: 6361 RVA: 0x000C4C11 File Offset: 0x000C2E11
		public Color CenterColor { get; set; }

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x060018DA RID: 6362 RVA: 0x000C4C1A File Offset: 0x000C2E1A
		// (set) Token: 0x060018DB RID: 6363 RVA: 0x000C4C22 File Offset: 0x000C2E22
		public Color BevelColor { get; set; }

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x060018DC RID: 6364 RVA: 0x000C4C2B File Offset: 0x000C2E2B
		// (set) Token: 0x060018DD RID: 6365 RVA: 0x000C4C33 File Offset: 0x000C2E33
		public Color ShadowColor { get; set; }

		// Token: 0x060018DE RID: 6366 RVA: 0x000C4C3C File Offset: 0x000C2E3C
		public BevelledRectangleWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.BevelSize = 2f;
			this.AmbientLight = 0.6f;
			this.DirectionalLight = 0.4f;
			this.TextureScale = 1f;
			this.TextureLinearFilter = false;
			this.CenterColor = new Color(181, 172, 154);
			this.BevelColor = new Color(181, 172, 154);
			this.ShadowColor = new Color(0, 0, 0, 80);
			this.IsHitTestVisible = false;
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x000C4CE0 File Offset: 0x000C2EE0
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Texture != null)
			{
				SamplerState samplerState = this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap;
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
				TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.Texture, false, 0, DepthStencilState.None, null, null, samplerState);
				int count = flatBatch2D.TriangleVertices.Count;
				int count2 = texturedBatch2D.TriangleVertices.Count;
				BevelledRectangleWidget.QueueBevelledRectangle(texturedBatch2D, flatBatch2D, Vector2.Zero, base.ActualSize, 0f, this.BevelSize, this.CenterColor * base.GlobalColorTransform, this.BevelColor * base.GlobalColorTransform, this.ShadowColor * base.GlobalColorTransform, this.AmbientLight, this.DirectionalLight, this.TextureScale);
				flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				texturedBatch2D.TransformTriangles(base.GlobalTransform, count2, -1);
				return;
			}
			FlatBatch2D flatBatch2D2 = dc.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
			int count3 = flatBatch2D2.TriangleVertices.Count;
			BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D2, Vector2.Zero, base.ActualSize, 0f, this.BevelSize, this.CenterColor * base.GlobalColorTransform, this.BevelColor * base.GlobalColorTransform, this.ShadowColor * base.GlobalColorTransform, this.AmbientLight, this.DirectionalLight, 0f);
			flatBatch2D2.TransformTriangles(base.GlobalTransform, count3, -1);
		}

		// Token: 0x060018E0 RID: 6368 RVA: 0x000C4E68 File Offset: 0x000C3068
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.BevelColor.A != 0 || this.CenterColor.A > 0);
			base.DesiredSize = this.Size;
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x000C4EAC File Offset: 0x000C30AC
		public static void QueueBevelledRectangle(TexturedBatch2D texturedBatch, FlatBatch2D flatBatch, Vector2 c1, Vector2 c2, float depth, float bevelSize, Color color, Color bevelColor, Color shadowColor, float ambientLight, float directionalLight, float textureScale)
		{
			float num = MathUtils.Abs(bevelSize);
			Vector2 vector = c1 + new Vector2(num);
			Vector2 vector2 = c2 - new Vector2(num);
			Vector2 vector3 = c2 + new Vector2(1.5f * num);
			float x = c1.X;
			float x2 = vector.X;
			float x3 = vector2.X;
			float x4 = c2.X;
			float x5 = vector3.X;
			float y = c1.Y;
			float y2 = vector.Y;
			float y3 = vector2.Y;
			float y4 = c2.Y;
			float y5 = vector3.Y;
			float num2 = MathUtils.Saturate(((bevelSize > 0f) ? 1f : -0.75f) * directionalLight + ambientLight);
			float num3 = MathUtils.Saturate(((bevelSize > 0f) ? -0.75f : 1f) * directionalLight + ambientLight);
			float num4 = MathUtils.Saturate(((bevelSize > 0f) ? -0.375f : 0.5f) * directionalLight + ambientLight);
			float num5 = MathUtils.Saturate(((bevelSize > 0f) ? 0.5f : -0.375f) * directionalLight + ambientLight);
			float num6 = MathUtils.Saturate(0f * directionalLight + ambientLight);
			Color color2 = new Color((byte)(num4 * (float)bevelColor.R), (byte)(num4 * (float)bevelColor.G), (byte)(num4 * (float)bevelColor.B), bevelColor.A);
			Color color3 = new Color((byte)(num5 * (float)bevelColor.R), (byte)(num5 * (float)bevelColor.G), (byte)(num5 * (float)bevelColor.B), bevelColor.A);
			Color color4 = new Color((byte)(num2 * (float)bevelColor.R), (byte)(num2 * (float)bevelColor.G), (byte)(num2 * (float)bevelColor.B), bevelColor.A);
			Color color5 = new Color((byte)(num3 * (float)bevelColor.R), (byte)(num3 * (float)bevelColor.G), (byte)(num3 * (float)bevelColor.B), bevelColor.A);
			Color color6 = new Color((byte)(num6 * (float)color.R), (byte)(num6 * (float)color.G), (byte)(num6 * (float)color.B), color.A);
			if (texturedBatch != null)
			{
				float num7 = textureScale / (float)texturedBatch.Texture.Width;
				float num8 = textureScale / (float)texturedBatch.Texture.Height;
				float num9 = x * num7;
				float num10 = y * num8;
				float x6 = num9;
				float x7 = (x2 - x) * num7 + num9;
				float x8 = (x3 - x) * num7 + num9;
				float x9 = (x4 - x) * num7 + num9;
				float y6 = num10;
				float y7 = (y2 - y) * num8 + num10;
				float y8 = (y3 - y) * num8 + num10;
				float y9 = (y4 - y) * num8 + num10;
				if (bevelColor.A > 0)
				{
					texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, new Vector2(x6, y6), new Vector2(x7, y7), new Vector2(x8, y7), new Vector2(x9, y6), color4);
					texturedBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, new Vector2(x8, y7), new Vector2(x8, y8), new Vector2(x9, y9), new Vector2(x9, y6), color3);
					texturedBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, new Vector2(x6, y9), new Vector2(x9, y9), new Vector2(x8, y8), new Vector2(x7, y8), color5);
					texturedBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, new Vector2(x6, y6), new Vector2(x6, y9), new Vector2(x7, y8), new Vector2(x7, y7), color2);
				}
				if (color6.A > 0)
				{
					texturedBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, new Vector2(x7, y7), new Vector2(x8, y8), color6);
				}
			}
			else if (flatBatch != null)
			{
				if (bevelColor.A > 0)
				{
					flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), new Vector2(x3, y2), new Vector2(x4, y), depth, color4);
					flatBatch.QueueQuad(new Vector2(x3, y2), new Vector2(x3, y3), new Vector2(x4, y4), new Vector2(x4, y), depth, color3);
					flatBatch.QueueQuad(new Vector2(x, y4), new Vector2(x4, y4), new Vector2(x3, y3), new Vector2(x2, y3), depth, color5);
					flatBatch.QueueQuad(new Vector2(x, y), new Vector2(x, y4), new Vector2(x2, y3), new Vector2(x2, y2), depth, color2);
				}
				if (color6.A > 0)
				{
					flatBatch.QueueQuad(new Vector2(x2, y2), new Vector2(x3, y3), depth, color6);
				}
			}
			if (bevelSize > 0f && flatBatch != null && shadowColor.A > 0)
			{
				Color color7 = shadowColor;
				Color color8 = new Color(0, 0, 0, 0);
				flatBatch.QueueTriangle(new Vector2(x, y4), new Vector2(x2, y5), new Vector2(x2, y4), depth, color8, color8, color7);
				flatBatch.QueueTriangle(new Vector2(x4, y), new Vector2(x4, y2), new Vector2(x5, y2), depth, color8, color7, color8);
				flatBatch.QueueTriangle(new Vector2(x4, y4), new Vector2(x4, y5), new Vector2(x5, y4), depth, color7, color8, color8);
				flatBatch.QueueQuad(new Vector2(x2, y4), new Vector2(x2, y5), new Vector2(x4, y5), new Vector2(x4, y4), depth, color7, color8, color8, color7);
				flatBatch.QueueQuad(new Vector2(x4, y2), new Vector2(x4, y4), new Vector2(x5, y4), new Vector2(x5, y2), depth, color7, color7, color8, color8);
			}
		}

		// Token: 0x04001176 RID: 4470
		public Texture2D m_texture;

		// Token: 0x04001177 RID: 4471
		public bool m_textureLinearFilter;
	}
}
