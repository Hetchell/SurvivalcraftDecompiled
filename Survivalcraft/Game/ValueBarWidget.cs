using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020003A2 RID: 930
	public class ValueBarWidget : Widget
	{
		// Token: 0x170004AF RID: 1199
		// (get) Token: 0x06001B5A RID: 7002 RVA: 0x000D5860 File Offset: 0x000D3A60
		// (set) Token: 0x06001B5B RID: 7003 RVA: 0x000D5868 File Offset: 0x000D3A68
		public float Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170004B0 RID: 1200
		// (get) Token: 0x06001B5C RID: 7004 RVA: 0x000D5876 File Offset: 0x000D3A76
		// (set) Token: 0x06001B5D RID: 7005 RVA: 0x000D587E File Offset: 0x000D3A7E
		public int BarsCount
		{
			get
			{
				return this.m_barsCount;
			}
			set
			{
				this.m_barsCount = MathUtils.Clamp(value, 1, 1000);
			}
		}

		// Token: 0x170004B1 RID: 1201
		// (get) Token: 0x06001B5E RID: 7006 RVA: 0x000D5892 File Offset: 0x000D3A92
		// (set) Token: 0x06001B5F RID: 7007 RVA: 0x000D589A File Offset: 0x000D3A9A
		public bool FlipDirection { get; set; }

		// Token: 0x170004B2 RID: 1202
		// (get) Token: 0x06001B60 RID: 7008 RVA: 0x000D58A3 File Offset: 0x000D3AA3
		// (set) Token: 0x06001B61 RID: 7009 RVA: 0x000D58AB File Offset: 0x000D3AAB
		public Vector2 BarSize { get; set; }

		// Token: 0x170004B3 RID: 1203
		// (get) Token: 0x06001B62 RID: 7010 RVA: 0x000D58B4 File Offset: 0x000D3AB4
		// (set) Token: 0x06001B63 RID: 7011 RVA: 0x000D58BC File Offset: 0x000D3ABC
		public float Spacing { get; set; }

		// Token: 0x170004B4 RID: 1204
		// (get) Token: 0x06001B64 RID: 7012 RVA: 0x000D58C5 File Offset: 0x000D3AC5
		// (set) Token: 0x06001B65 RID: 7013 RVA: 0x000D58CD File Offset: 0x000D3ACD
		public Color LitBarColor
		{
			get
			{
				return this.m_litBarColor;
			}
			set
			{
				this.m_litBarColor = value;
			}
		}

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06001B66 RID: 7014 RVA: 0x000D58D6 File Offset: 0x000D3AD6
		// (set) Token: 0x06001B67 RID: 7015 RVA: 0x000D58DE File Offset: 0x000D3ADE
		public Color LitBarColor2
		{
			get
			{
				return this.m_litBarColor2;
			}
			set
			{
				this.m_litBarColor2 = value;
			}
		}

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001B68 RID: 7016 RVA: 0x000D58E7 File Offset: 0x000D3AE7
		// (set) Token: 0x06001B69 RID: 7017 RVA: 0x000D58EF File Offset: 0x000D3AEF
		public Color UnlitBarColor
		{
			get
			{
				return this.m_unlitBarColor;
			}
			set
			{
				this.m_unlitBarColor = value;
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001B6A RID: 7018 RVA: 0x000D58F8 File Offset: 0x000D3AF8
		// (set) Token: 0x06001B6B RID: 7019 RVA: 0x000D5900 File Offset: 0x000D3B00
		public bool BarBlending { get; set; }

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06001B6C RID: 7020 RVA: 0x000D5909 File Offset: 0x000D3B09
		// (set) Token: 0x06001B6D RID: 7021 RVA: 0x000D5911 File Offset: 0x000D3B11
		public bool HalfBars { get; set; }

		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x06001B6E RID: 7022 RVA: 0x000D591A File Offset: 0x000D3B1A
		// (set) Token: 0x06001B6F RID: 7023 RVA: 0x000D5922 File Offset: 0x000D3B22
		public Subtexture BarSubtexture
		{
			get
			{
				return this.m_barSubtexture;
			}
			set
			{
				if (value != this.m_barSubtexture)
				{
					this.m_barSubtexture = value;
				}
			}
		}

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x06001B70 RID: 7024 RVA: 0x000D5934 File Offset: 0x000D3B34
		// (set) Token: 0x06001B71 RID: 7025 RVA: 0x000D593C File Offset: 0x000D3B3C
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

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x06001B72 RID: 7026 RVA: 0x000D594E File Offset: 0x000D3B4E
		// (set) Token: 0x06001B73 RID: 7027 RVA: 0x000D5956 File Offset: 0x000D3B56
		public LayoutDirection LayoutDirection
		{
			get
			{
				return this.m_layoutDirection;
			}
			set
			{
				this.m_layoutDirection = value;
			}
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x000D5960 File Offset: 0x000D3B60
		public ValueBarWidget()
		{
			this.IsHitTestVisible = false;
			this.BarSize = new Vector2(24f);
			this.BarBlending = true;
			this.TextureLinearFilter = true;
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x000D59CE File Offset: 0x000D3BCE
		public void Flash(int count)
		{
			this.m_flashCount = MathUtils.Max(this.m_flashCount, (float)count);
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x000D59E4 File Offset: 0x000D3BE4
		public override void Draw(Widget.DrawContext dc)
		{
            BaseBatch baseBatch = this.BarSubtexture == null ? (BaseBatch)dc.PrimitivesRenderer2D.FlatBatch(depthStencilState: DepthStencilState.None) : (BaseBatch)dc.PrimitivesRenderer2D.TexturedBatch(this.BarSubtexture.Texture, depthStencilState: DepthStencilState.None, samplerState: (this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp)); int start = 0;
			int count;
			if (baseBatch is TexturedBatch2D)
			{
				count = ((TexturedBatch2D)baseBatch).TriangleVertices.Count;
			}
			else
			{
				start = ((FlatBatch2D)baseBatch).LineVertices.Count;
				count = ((FlatBatch2D)baseBatch).TriangleVertices.Count;
			}
			Vector2 zero = Vector2.Zero;
			if (this.m_layoutDirection == LayoutDirection.Horizontal)
			{
				zero.X += this.Spacing / 2f;
			}
			else
			{
				zero.Y += this.Spacing / 2f;
			}
			int num = this.HalfBars ? 1 : 2;
			for (int i = 0; i < 2 * this.BarsCount; i += num)
			{
				bool flag = i % 2 == 0;
				float num2 = 0.5f * (float)i;
				float num3 = (!this.FlipDirection) ? MathUtils.Clamp((this.Value - num2 / (float)this.BarsCount) * (float)this.BarsCount, 0f, 1f) : MathUtils.Clamp((this.Value - ((float)this.BarsCount - num2 - 1f) / (float)this.BarsCount) * (float)this.BarsCount, 0f, 1f);
				if (!this.BarBlending)
				{
					num3 = MathUtils.Ceiling(num3);
				}
				float s = (this.m_flashCount > 0f) ? (1f - MathUtils.Abs(MathUtils.Sin(this.m_flashCount * 3.1415927f))) : 1f;
				Color c = this.LitBarColor;
				if (this.LitBarColor2 != Color.Transparent && this.BarsCount > 1)
				{
					c = Color.Lerp(this.LitBarColor, this.LitBarColor2, num2 / (float)(this.BarsCount - 1));
				}
				Color color = Color.Lerp(this.UnlitBarColor, c, num3) * s * base.GlobalColorTransform;
				if (this.HalfBars)
				{
					if (flag)
					{
						Vector2 zero2 = Vector2.Zero;
						Vector2 vector = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 1f) : new Vector2(1f, 0.5f);
						if (baseBatch is TexturedBatch2D)
						{
							Vector2 topLeft = this.BarSubtexture.TopLeft;
							Vector2 texCoord = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector.Y));
							((TexturedBatch2D)baseBatch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, topLeft, texCoord, color);
						}
						else
						{
							((FlatBatch2D)baseBatch).QueueQuad(zero + zero2 * this.BarSize, zero + vector * this.BarSize, 0f, color);
						}
					}
					else
					{
						Vector2 vector2 = (this.m_layoutDirection == LayoutDirection.Horizontal) ? new Vector2(0.5f, 0f) : new Vector2(0f, 0.5f);
						Vector2 one = Vector2.One;
						if (baseBatch is TexturedBatch2D)
						{
							Vector2 texCoord2 = new Vector2(MathUtils.Lerp(this.BarSubtexture.TopLeft.X, this.BarSubtexture.BottomRight.X, vector2.X), MathUtils.Lerp(this.BarSubtexture.TopLeft.Y, this.BarSubtexture.BottomRight.Y, vector2.Y));
							Vector2 bottomRight = this.BarSubtexture.BottomRight;
							((TexturedBatch2D)baseBatch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, texCoord2, bottomRight, color);
						}
						else
						{
							((FlatBatch2D)baseBatch).QueueQuad(zero + vector2 * this.BarSize, zero + one * this.BarSize, 0f, color);
						}
					}
				}
				else
				{
					Vector2 zero3 = Vector2.Zero;
					Vector2 one2 = Vector2.One;
					if (baseBatch is TexturedBatch2D)
					{
						Vector2 topLeft2 = this.BarSubtexture.TopLeft;
						Vector2 bottomRight2 = this.BarSubtexture.BottomRight;
						((TexturedBatch2D)baseBatch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, topLeft2, bottomRight2, color);
					}
					else
					{
						((FlatBatch2D)baseBatch).QueueQuad(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, color);
						((FlatBatch2D)baseBatch).QueueRectangle(zero + zero3 * this.BarSize, zero + one2 * this.BarSize, 0f, Color.MultiplyColorOnly(color, 0.75f));
					}
				}
				if (!flag || !this.HalfBars)
				{
					if (this.m_layoutDirection == LayoutDirection.Horizontal)
					{
						zero.X += this.BarSize.X + this.Spacing;
					}
					else
					{
						zero.Y += this.BarSize.Y + this.Spacing;
					}
				}
			}
			if (baseBatch is TexturedBatch2D)
			{
				((TexturedBatch2D)baseBatch).TransformTriangles(base.GlobalTransform, count, -1);
			}
			else
			{
				((FlatBatch2D)baseBatch).TransformLines(base.GlobalTransform, start, -1);
				((FlatBatch2D)baseBatch).TransformTriangles(base.GlobalTransform, count, -1);
			}
			this.m_flashCount = MathUtils.Max(this.m_flashCount - 4f * Time.FrameDuration, 0f);
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x000D6018 File Offset: 0x000D4218
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			if (this.m_layoutDirection == LayoutDirection.Horizontal)
			{
				base.DesiredSize = new Vector2((this.BarSize.X + this.Spacing) * (float)this.BarsCount, this.BarSize.Y);
				return;
			}
			base.DesiredSize = new Vector2(this.BarSize.X, (this.BarSize.Y + this.Spacing) * (float)this.BarsCount);
		}

		// Token: 0x040012F8 RID: 4856
		public float m_value;

		// Token: 0x040012F9 RID: 4857
		public int m_barsCount = 8;

		// Token: 0x040012FA RID: 4858
		public Color m_litBarColor = new Color(16, 140, 0);

		// Token: 0x040012FB RID: 4859
		public Color m_litBarColor2 = Color.Transparent;

		// Token: 0x040012FC RID: 4860
		public Color m_unlitBarColor = new Color(48, 48, 48);

		// Token: 0x040012FD RID: 4861
		public Subtexture m_barSubtexture;

		// Token: 0x040012FE RID: 4862
		public LayoutDirection m_layoutDirection;

		// Token: 0x040012FF RID: 4863
		public float m_flashCount;

		// Token: 0x04001300 RID: 4864
		public bool m_textureLinearFilter;
	}
}
