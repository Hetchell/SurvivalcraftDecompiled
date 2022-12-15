using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200039D RID: 925
	public class StarRatingWidget : Widget
	{
		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06001B0C RID: 6924 RVA: 0x000D4190 File Offset: 0x000D2390
		// (set) Token: 0x06001B0D RID: 6925 RVA: 0x000D4198 File Offset: 0x000D2398
		public float StarSize { get; set; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06001B0E RID: 6926 RVA: 0x000D41A1 File Offset: 0x000D23A1
		// (set) Token: 0x06001B0F RID: 6927 RVA: 0x000D41A9 File Offset: 0x000D23A9
		public Color ForeColor { get; set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x06001B10 RID: 6928 RVA: 0x000D41B2 File Offset: 0x000D23B2
		// (set) Token: 0x06001B11 RID: 6929 RVA: 0x000D41BA File Offset: 0x000D23BA
		public Color BackColor { get; set; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x06001B12 RID: 6930 RVA: 0x000D41C3 File Offset: 0x000D23C3
		// (set) Token: 0x06001B13 RID: 6931 RVA: 0x000D41CB File Offset: 0x000D23CB
		public float Rating
		{
			get
			{
				return this.m_rating;
			}
			set
			{
				this.m_rating = MathUtils.Clamp(value, 0f, 5f);
			}
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x000D41E4 File Offset: 0x000D23E4
		public StarRatingWidget()
		{
			this.m_texture = ContentManager.Get<Texture2D>("Textures/Gui/RatingStar");
			this.ForeColor = new Color(255, 192, 0);
			this.BackColor = new Color(96, 96, 96);
			this.StarSize = 64f;
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x000D423C File Offset: 0x000D243C
		public override void Update()
		{
			if (base.Input.Press != null && base.HitTestGlobal(base.Input.Press.Value, null) == this)
			{
				Vector2 vector = base.ScreenToWidget(base.Input.Press.Value);
				this.Rating = (float)((int)MathUtils.Floor(5f * vector.X / base.ActualSize.X + 1f));
			}
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x000D42C0 File Offset: 0x000D24C0
		public override void Draw(Widget.DrawContext dc)
		{
			TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.m_texture, false, 0, DepthStencilState.None, null, null, SamplerState.LinearWrap);
			float x = 0f;
			float x2 = base.ActualSize.X * this.Rating / 5f;
			float x3 = base.ActualSize.X;
			float y = 0f;
			float y2 = base.ActualSize.Y;
			int count = texturedBatch2D.TriangleVertices.Count;
			texturedBatch2D.QueueQuad(new Vector2(x, y), new Vector2(x2, y2), 0f, new Vector2(0f, 0f), new Vector2(this.Rating, 1f), this.ForeColor * base.GlobalColorTransform);
			texturedBatch2D.QueueQuad(new Vector2(x2, y), new Vector2(x3, y2), 0f, new Vector2(this.Rating, 0f), new Vector2(5f, 1f), this.BackColor * base.GlobalColorTransform);
			texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x000D43D9 File Offset: 0x000D25D9
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.DesiredSize = new Vector2(5f * this.StarSize, this.StarSize);
		}

		// Token: 0x040012D0 RID: 4816
		public Texture2D m_texture;

		// Token: 0x040012D1 RID: 4817
		public float m_rating;
	}
}
