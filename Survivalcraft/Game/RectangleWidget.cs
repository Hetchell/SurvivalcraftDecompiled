using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000397 RID: 919
	public class RectangleWidget : Widget
	{
		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001AB6 RID: 6838 RVA: 0x000D2301 File Offset: 0x000D0501
		// (set) Token: 0x06001AB7 RID: 6839 RVA: 0x000D2309 File Offset: 0x000D0509
		public Vector2 Size { get; set; }

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06001AB8 RID: 6840 RVA: 0x000D2312 File Offset: 0x000D0512
		// (set) Token: 0x06001AB9 RID: 6841 RVA: 0x000D231A File Offset: 0x000D051A
		public float Depth { get; set; }

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06001ABA RID: 6842 RVA: 0x000D2323 File Offset: 0x000D0523
		// (set) Token: 0x06001ABB RID: 6843 RVA: 0x000D232B File Offset: 0x000D052B
		public bool DepthWriteEnabled
		{
			get
			{
				return this.m_depthWriteEnabled;
			}
			set
			{
				if (value != this.m_depthWriteEnabled)
				{
					this.m_depthWriteEnabled = value;
				}
			}
		}

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x06001ABC RID: 6844 RVA: 0x000D233D File Offset: 0x000D053D
		// (set) Token: 0x06001ABD RID: 6845 RVA: 0x000D2345 File Offset: 0x000D0545
		public Subtexture Subtexture
		{
			get
			{
				return this.m_subtexture;
			}
			set
			{
				if (value != this.m_subtexture)
				{
					this.m_subtexture = value;
				}
			}
		}

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x06001ABE RID: 6846 RVA: 0x000D2357 File Offset: 0x000D0557
		// (set) Token: 0x06001ABF RID: 6847 RVA: 0x000D235F File Offset: 0x000D055F
		public bool TextureWrap
		{
			get
			{
				return this.m_textureWrap;
			}
			set
			{
				if (value != this.m_textureWrap)
				{
					this.m_textureWrap = value;
				}
			}
		}

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x06001AC0 RID: 6848 RVA: 0x000D2371 File Offset: 0x000D0571
		// (set) Token: 0x06001AC1 RID: 6849 RVA: 0x000D2379 File Offset: 0x000D0579
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

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x06001AC2 RID: 6850 RVA: 0x000D238B File Offset: 0x000D058B
		// (set) Token: 0x06001AC3 RID: 6851 RVA: 0x000D2393 File Offset: 0x000D0593
		public bool FlipHorizontal { get; set; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x06001AC4 RID: 6852 RVA: 0x000D239C File Offset: 0x000D059C
		// (set) Token: 0x06001AC5 RID: 6853 RVA: 0x000D23A4 File Offset: 0x000D05A4
		public bool FlipVertical { get; set; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x06001AC6 RID: 6854 RVA: 0x000D23AD File Offset: 0x000D05AD
		// (set) Token: 0x06001AC7 RID: 6855 RVA: 0x000D23B5 File Offset: 0x000D05B5
		public Color FillColor { get; set; }

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x000D23BE File Offset: 0x000D05BE
		// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x000D23C6 File Offset: 0x000D05C6
		public Color OutlineColor { get; set; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x06001ACA RID: 6858 RVA: 0x000D23CF File Offset: 0x000D05CF
		// (set) Token: 0x06001ACB RID: 6859 RVA: 0x000D23D7 File Offset: 0x000D05D7
		public float OutlineThickness { get; set; }

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x06001ACC RID: 6860 RVA: 0x000D23E0 File Offset: 0x000D05E0
		// (set) Token: 0x06001ACD RID: 6861 RVA: 0x000D23E8 File Offset: 0x000D05E8
		public Vector2 Texcoord1 { get; set; }

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000D23F1 File Offset: 0x000D05F1
		// (set) Token: 0x06001ACF RID: 6863 RVA: 0x000D23F9 File Offset: 0x000D05F9
		public Vector2 Texcoord2 { get; set; }

		// Token: 0x06001AD0 RID: 6864 RVA: 0x000D2404 File Offset: 0x000D0604
		public RectangleWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.TextureLinearFilter = true;
			this.FillColor = Color.Black;
			this.OutlineColor = Color.White;
			this.OutlineThickness = 1f;
			this.IsHitTestVisible = false;
			this.Texcoord1 = Vector2.Zero;
			this.Texcoord2 = Vector2.One;
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x000D246C File Offset: 0x000D066C
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.FillColor.A == 0 && (this.OutlineColor.A == 0 || this.OutlineThickness <= 0f))
			{
				return;
			}
			DepthStencilState depthStencilState = this.DepthWriteEnabled ? DepthStencilState.DepthWrite : DepthStencilState.None;
			Matrix globalTransform = base.GlobalTransform;
			Vector2 zero = Vector2.Zero;
			Vector2 vector = new Vector2(base.ActualSize.X, 0f);
			Vector2 actualSize = base.ActualSize;
			Vector2 vector2 = new Vector2(0f, base.ActualSize.Y);
			Vector2 vector3;
			Vector2.Transform(ref zero, ref globalTransform, out vector3);
			Vector2 vector4;
			Vector2.Transform(ref vector, ref globalTransform, out vector4);
			Vector2 vector5;
			Vector2.Transform(ref actualSize, ref globalTransform, out vector5);
			Vector2 vector6;
			Vector2.Transform(ref vector2, ref globalTransform, out vector6);
			Color color = this.FillColor * base.GlobalColorTransform;
			if (color.A != 0)
			{
				if (this.Subtexture != null)
				{
					SamplerState samplerState = (!this.TextureWrap) ? (this.TextureLinearFilter ? SamplerState.LinearClamp : SamplerState.PointClamp) : (this.TextureLinearFilter ? SamplerState.LinearWrap : SamplerState.PointWrap);
					TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.Subtexture.Texture, false, 0, depthStencilState, null, null, samplerState);
					Vector2 vector7 = default(Vector2);
					Vector2 vector8 = default(Vector2);
					Vector2 texCoord;
					Vector2 texCoord2;
					if (this.TextureWrap)
					{
						vector7 = Vector2.Zero;
						texCoord = new Vector2(base.ActualSize.X / (float)this.Subtexture.Texture.Width, 0f);
						vector8 = new Vector2(base.ActualSize.X / (float)this.Subtexture.Texture.Width, base.ActualSize.Y / (float)this.Subtexture.Texture.Height);
						texCoord2 = new Vector2(0f, base.ActualSize.Y / (float)this.Subtexture.Texture.Height);
					}
					else
					{
						vector7.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord1.X);
						vector7.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord1.Y);
						vector8.X = MathUtils.Lerp(this.Subtexture.TopLeft.X, this.Subtexture.BottomRight.X, this.Texcoord2.X);
						vector8.Y = MathUtils.Lerp(this.Subtexture.TopLeft.Y, this.Subtexture.BottomRight.Y, this.Texcoord2.Y);
						texCoord = new Vector2(vector8.X, vector7.Y);
						texCoord2 = new Vector2(vector7.X, vector8.Y);
					}
					if (this.FlipHorizontal)
					{
						Utilities.Swap<float>(ref vector7.X, ref texCoord.X);
						Utilities.Swap<float>(ref vector8.X, ref texCoord2.X);
					}
					if (this.FlipVertical)
					{
						Utilities.Swap<float>(ref vector7.Y, ref vector8.Y);
						Utilities.Swap<float>(ref texCoord.Y, ref texCoord2.Y);
					}
					texturedBatch2D.QueueQuad(vector3, vector4, vector5, vector6, this.Depth, vector7, texCoord, vector8, texCoord2, color);
				}
				else
				{
					dc.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null).QueueQuad(vector3, vector4, vector5, vector6, this.Depth, color);
				}
			}
			Color color2 = this.OutlineColor * base.GlobalColorTransform;
			if (color2.A != 0 && this.OutlineThickness > 0f)
			{
				FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, depthStencilState, null, null);
				Vector2 vector9 = Vector2.Normalize(base.GlobalTransform.Right.XY);
				Vector2 v = -Vector2.Normalize(base.GlobalTransform.Up.XY);
				int num = (int)MathUtils.Max(MathUtils.Round(this.OutlineThickness * base.GlobalTransform.Right.Length()), 1f);
				for (int i = 0; i < num; i++)
				{
					flatBatch2D.QueueLine(vector3, vector4, this.Depth, color2);
					flatBatch2D.QueueLine(vector4, vector5, this.Depth, color2);
					flatBatch2D.QueueLine(vector5, vector6, this.Depth, color2);
					flatBatch2D.QueueLine(vector6, vector3, this.Depth, color2);
					vector3 += vector9 - v;
					vector4 += -vector9 - v;
					vector5 += -vector9 + v;
					vector6 += vector9 + v;
				}
			}
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x000D2970 File Offset: 0x000D0B70
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.FillColor.A != 0 || (this.OutlineColor.A != 0 && this.OutlineThickness > 0f));
			base.DesiredSize = this.Size;
		}

		// Token: 0x040012A2 RID: 4770
		public Subtexture m_subtexture;

		// Token: 0x040012A3 RID: 4771
		public bool m_textureWrap;

		// Token: 0x040012A4 RID: 4772
		public bool m_textureLinearFilter;

		// Token: 0x040012A5 RID: 4773
		public bool m_depthWriteEnabled;
	}
}
