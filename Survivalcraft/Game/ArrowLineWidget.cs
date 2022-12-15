using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000368 RID: 872
	public class ArrowLineWidget : Widget
	{
		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x060018A1 RID: 6305 RVA: 0x000C4277 File Offset: 0x000C2477
		// (set) Token: 0x060018A2 RID: 6306 RVA: 0x000C427F File Offset: 0x000C247F
		public float Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x060018A3 RID: 6307 RVA: 0x000C428F File Offset: 0x000C248F
		// (set) Token: 0x060018A4 RID: 6308 RVA: 0x000C4297 File Offset: 0x000C2497
		public float ArrowWidth
		{
			get
			{
				return this.m_arrowWidth;
			}
			set
			{
				this.m_arrowWidth = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x060018A5 RID: 6309 RVA: 0x000C42A7 File Offset: 0x000C24A7
		// (set) Token: 0x060018A6 RID: 6310 RVA: 0x000C42AF File Offset: 0x000C24AF
		public Color Color { get; set; }

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x060018A7 RID: 6311 RVA: 0x000C42B8 File Offset: 0x000C24B8
		// (set) Token: 0x060018A8 RID: 6312 RVA: 0x000C42C0 File Offset: 0x000C24C0
		public string PointsString
		{
			get
			{
				return this.m_pointsString;
			}
			set
			{
				this.m_pointsString = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x060018A9 RID: 6313 RVA: 0x000C42D0 File Offset: 0x000C24D0
		// (set) Token: 0x060018AA RID: 6314 RVA: 0x000C42D8 File Offset: 0x000C24D8
		public bool AbsoluteCoordinates
		{
			get
			{
				return this.m_absoluteCoordinates;
			}
			set
			{
				this.m_absoluteCoordinates = value;
				this.m_parsingPending = true;
			}
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x000C42E8 File Offset: 0x000C24E8
		public ArrowLineWidget()
		{
			this.Width = 6f;
			this.ArrowWidth = 0f;
			this.Color = Color.White;
			this.IsHitTestVisible = false;
			this.PointsString = "0, 0; 50, 0";
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x000C433C File Offset: 0x000C253C
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.m_parsingPending)
			{
				this.ParsePoints();
			}
			Color color = this.Color * base.GlobalColorTransform;
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, null);
			int count = flatBatch2D.TriangleVertices.Count;
			for (int i = 0; i < this.m_vertices.Count; i += 3)
			{
				Vector2 p = this.m_startOffset + this.m_vertices[i];
				Vector2 p2 = this.m_startOffset + this.m_vertices[i + 1];
				Vector2 p3 = this.m_startOffset + this.m_vertices[i + 2];
				flatBatch2D.QueueTriangle(p, p2, p3, 0f, color);
			}
			flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x000C4410 File Offset: 0x000C2610
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			if (this.m_parsingPending)
			{
				this.ParsePoints();
			}
			base.IsDrawRequired = (this.Color.A > 0 && this.Width > 0f);
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x000C4454 File Offset: 0x000C2654
		public void ParsePoints()
		{
			this.m_parsingPending = false;
			List<Vector2> list = new List<Vector2>();
			foreach (string data in this.m_pointsString.Split(new string[]
			{
				";"
			}, StringSplitOptions.None))
			{
				list.Add(HumanReadableConverter.ConvertFromString<Vector2>(data));
			}
			this.m_vertices.Clear();
			for (int j = 0; j < list.Count; j++)
			{
				if (j >= 1)
				{
					Vector2 vector = list[j - 1];
					Vector2 vector2 = list[j];
					Vector2 vector3 = Vector2.Normalize(vector2 - vector);
					Vector2 vector4 = vector3;
					Vector2 v = vector3;
					if (j >= 2)
					{
						vector4 = Vector2.Normalize(vector - list[j - 2]);
					}
					if (j <= list.Count - 2)
					{
						v = Vector2.Normalize(list[j + 1] - vector2);
					}
					Vector2 v2 = Vector2.Perpendicular(vector4);
					Vector2 v3 = Vector2.Perpendicular(vector3);
					float num = 3.1415927f - Vector2.Angle(vector4, vector3);
					float s = 0.5f * this.Width / MathUtils.Tan(num / 2f);
					Vector2 v4 = 0.5f * v2 * this.Width - vector4 * s;
					float num2 = 3.1415927f - Vector2.Angle(vector3, v);
					float s2 = 0.5f * this.Width / MathUtils.Tan(num2 / 2f);
					Vector2 v5 = 0.5f * v3 * this.Width - vector3 * s2;
					this.m_vertices.Add(vector + v4);
					this.m_vertices.Add(vector - v4);
					this.m_vertices.Add(vector2 - v5);
					this.m_vertices.Add(vector2 - v5);
					this.m_vertices.Add(vector2 + v5);
					this.m_vertices.Add(vector + v4);
					if (j == list.Count - 1)
					{
						this.m_vertices.Add(vector2 - 0.5f * this.ArrowWidth * v3);
						this.m_vertices.Add(vector2 + 0.5f * this.ArrowWidth * v3);
						this.m_vertices.Add(vector2 + 0.5f * this.ArrowWidth * vector3);
					}
				}
			}
			if (this.m_vertices.Count <= 0)
			{
				base.DesiredSize = Vector2.Zero;
				this.m_startOffset = Vector2.Zero;
				return;
			}
			float? num3 = null;
			float? num4 = null;
			float? num5 = null;
			float? num6 = null;
			int k = 0;
			while (k < this.m_vertices.Count)
			{
				if (num3 == null)
				{
					goto IL_2FB;
				}
				float x = this.m_vertices[k].X;
				float? num7 = num3;
				if (x < num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_2FB;
				}
				IL_314:
				if (num4 == null)
				{
					goto IL_346;
				}
				float y = this.m_vertices[k].Y;
				num7 = num4;
				if (y < num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_346;
				}
				IL_35F:
				if (num5 == null)
				{
					goto IL_391;
				}
				float x2 = this.m_vertices[k].X;
				num7 = num5;
				if (x2 > num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_391;
				}
				IL_3AA:
				if (num6 == null)
				{
					goto IL_3DC;
				}
				float y2 = this.m_vertices[k].Y;
				num7 = num6;
				if (y2 > num7.GetValueOrDefault() & num7 != null)
				{
					goto IL_3DC;
				}
				IL_3F5:
				k++;
				continue;
				IL_3DC:
				num6 = new float?(this.m_vertices[k].Y);
				goto IL_3F5;
				IL_391:
				num5 = new float?(this.m_vertices[k].X);
				goto IL_3AA;
				IL_346:
				num4 = new float?(this.m_vertices[k].Y);
				goto IL_35F;
				IL_2FB:
				num3 = new float?(this.m_vertices[k].X);
				goto IL_314;
			}
			if (this.AbsoluteCoordinates)
			{
				base.DesiredSize = new Vector2(num5.Value, num6.Value);
				this.m_startOffset = Vector2.Zero;
				return;
			}
			base.DesiredSize = new Vector2(num5.Value - num3.Value, num6.Value - num4.Value);
			this.m_startOffset = -new Vector2(num3.Value, num4.Value);
		}

		// Token: 0x04001168 RID: 4456
		public string m_pointsString;

		// Token: 0x04001169 RID: 4457
		public float m_width;

		// Token: 0x0400116A RID: 4458
		public float m_arrowWidth;

		// Token: 0x0400116B RID: 4459
		public bool m_absoluteCoordinates;

		// Token: 0x0400116C RID: 4460
		public List<Vector2> m_vertices = new List<Vector2>();

		// Token: 0x0400116D RID: 4461
		public bool m_parsingPending;

		// Token: 0x0400116E RID: 4462
		public Vector2 m_startOffset;
	}
}
