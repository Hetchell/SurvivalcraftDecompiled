using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
	// Token: 0x02000384 RID: 900
	public class FurnitureDesignWidget : Widget
	{
		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x000C9AC0 File Offset: 0x000C7CC0
		// (set) Token: 0x060019B1 RID: 6577 RVA: 0x000C9AC8 File Offset: 0x000C7CC8
		public Vector2 Size { get; set; }

		// Token: 0x17000429 RID: 1065
		// (get) Token: 0x060019B2 RID: 6578 RVA: 0x000C9AD1 File Offset: 0x000C7CD1
		// (set) Token: 0x060019B3 RID: 6579 RVA: 0x000C9AD9 File Offset: 0x000C7CD9
		public FurnitureDesignWidget.ViewMode Mode { get; set; }

		// Token: 0x1700042A RID: 1066
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x000C9AE2 File Offset: 0x000C7CE2
		// (set) Token: 0x060019B5 RID: 6581 RVA: 0x000C9AEA File Offset: 0x000C7CEA
		public FurnitureDesign Design { get; set; }

		// Token: 0x060019B6 RID: 6582 RVA: 0x000C9AF4 File Offset: 0x000C7CF4
		public FurnitureDesignWidget()
		{
			base.ClampToBounds = true;
			this.Size = new Vector2(float.PositiveInfinity);
			this.Mode = FurnitureDesignWidget.ViewMode.Perspective;
			this.m_direction = Vector3.Normalize(new Vector3(1f, -0.5f, -1f));
			this.m_rotationSpeed = new Vector2(2f, 0.5f);
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x000C9B70 File Offset: 0x000C7D70
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Design == null)
			{
				return;
			}
			Matrix matrix;
			if (this.Mode == FurnitureDesignWidget.ViewMode.Perspective)
			{
				Viewport viewport = Display.Viewport;
				Vector3 vector = new Vector3(0.5f, 0.5f, 0.5f);
				Matrix m = Matrix.CreateLookAt(2.65f * this.m_direction + vector, vector, Vector3.UnitY);
				Matrix m2 = Matrix.CreatePerspectiveFieldOfView(1.2f, base.ActualSize.X / base.ActualSize.Y, 0.4f, 4f);
				Matrix m3 = MatrixUtils.CreateScaleTranslation(base.ActualSize.X, 0f - base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
				matrix = m * m2 * m3;
				FlatBatch3D flatBatch3D = this.m_primitivesRenderer3d.FlatBatch(1, DepthStencilState.DepthRead, null, null);
				for (int i = 0; i <= this.Design.Resolution; i++)
				{
					float num = (float)i / (float)this.Design.Resolution;
					Color color = (i % 2 == 0) ? new Color(56, 56, 56, 56) : new Color(28, 28, 28, 28);
					color *= base.GlobalColorTransform;
					flatBatch3D.QueueLine(new Vector3(num, 0f, 0f), new Vector3(num, 0f, 1f), color);
					flatBatch3D.QueueLine(new Vector3(0f, 0f, num), new Vector3(1f, 0f, num), color);
					flatBatch3D.QueueLine(new Vector3(0f, num, 0f), new Vector3(0f, num, 1f), color);
					flatBatch3D.QueueLine(new Vector3(0f, 0f, num), new Vector3(0f, 1f, num), color);
					flatBatch3D.QueueLine(new Vector3(0f, num, 1f), new Vector3(1f, num, 1f), color);
					flatBatch3D.QueueLine(new Vector3(num, 0f, 1f), new Vector3(num, 1f, 1f), color);
				}
				Color color2 = new Color(64, 64, 64, 255) * base.GlobalColorTransform;
				FontBatch3D fontBatch3D = this.m_primitivesRenderer3d.FontBatch(ContentManager.Get<BitmapFont>("Fonts/Pericles"), 1, null, null, null, null);
				fontBatch3D.QueueText("Front", new Vector3(0.5f, 0f, 0f), 0.004f * new Vector3(-1f, 0f, 0f), 0.004f * new Vector3(0f, 0f, -1f), color2, TextAnchor.HorizontalCenter);
				fontBatch3D.QueueText("Side", new Vector3(1f, 0f, 0.5f), 0.004f * new Vector3(0f, 0f, -1f), 0.004f * new Vector3(1f, 0f, 0f), color2, TextAnchor.HorizontalCenter);
				if (FurnitureDesignWidget.DrawDebugFurniture)
				{
					this.DebugDraw();
				}
			}
			else
			{
				Vector3 position;
				Vector3 up;
				if (this.Mode == FurnitureDesignWidget.ViewMode.Side)
				{
					position = new Vector3(1f, 0f, 0f);
					up = new Vector3(0f, 1f, 0f);
				}
				else if (this.Mode != FurnitureDesignWidget.ViewMode.Top)
				{
					position = new Vector3(0f, 0f, -10f);
					up = new Vector3(0f, 1f, 0f);
				}
				else
				{
					position = new Vector3(0f, 1f, 0f);
					up = new Vector3(0f, 0f, 1f);
				}
				Viewport viewport2 = Display.Viewport;
				float num2 = MathUtils.Min(base.ActualSize.X, base.ActualSize.Y);
				Matrix m4 = Matrix.CreateLookAt(position, new Vector3(0f, 0f, 0f), up);
				Matrix m5 = Matrix.CreateOrthographic(2f, 2f, -10f, 10f);
				Matrix m6 = MatrixUtils.CreateScaleTranslation(num2, 0f - num2, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport2.Width, -2f / (float)viewport2.Height, -1f, 1f);
				matrix = Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) * m4 * m5 * m6;
				FlatBatch2D flatBatch2D = this.m_primitivesRenderer2d.FlatBatch(0, null, null, null);
				Matrix globalTransform = base.GlobalTransform;
				for (int j = 1; j < this.Design.Resolution; j++)
				{
					float num3 = (float)j / (float)this.Design.Resolution;
					Vector2 vector2 = new Vector2(base.ActualSize.X * num3, 0f);
					Vector2 vector3 = new Vector2(base.ActualSize.X * num3, base.ActualSize.Y);
					Vector2 vector4 = new Vector2(0f, base.ActualSize.Y * num3);
					Vector2 vector5 = new Vector2(base.ActualSize.X, base.ActualSize.Y * num3);
					Vector2.Transform(ref vector2, ref globalTransform, out vector2);
					Vector2.Transform(ref vector3, ref globalTransform, out vector3);
					Vector2.Transform(ref vector4, ref globalTransform, out vector4);
					Vector2.Transform(ref vector5, ref globalTransform, out vector5);
					Color color3 = (j % 2 == 0) ? new Color(0, 0, 0, 56) : new Color(0, 0, 0, 28);
					Color color4 = (j % 2 == 0) ? new Color(56, 56, 56, 56) : new Color(28, 28, 28, 28);
					color3 *= base.GlobalColorTransform;
					color4 *= base.GlobalColorTransform;
					flatBatch2D.QueueLine(vector2, vector3, 0f, (j % 2 == 0) ? color3 : (color3 * 0.75f));
					flatBatch2D.QueueLine(vector2 + new Vector2(1f, 0f), vector3 + new Vector2(1f, 0f), 0f, color4);
					flatBatch2D.QueueLine(vector4, vector5, 0f, color3);
					flatBatch2D.QueueLine(vector4 + new Vector2(0f, 1f), vector5 + new Vector2(0f, 1f), 0f, color4);
				}
			}
			Matrix identity = Matrix.Identity;
			FurnitureGeometry geometry = this.Design.Geometry;
			for (int k = 0; k < 6; k++)
			{
				Color color5 = base.GlobalColorTransform;
				if (this.Mode == FurnitureDesignWidget.ViewMode.Perspective)
				{
					float num4 = LightingManager.LightIntensityByLightValueAndFace[15 + 16 * CellFace.OppositeFace(k)];
					color5 *= new Color(num4, num4, num4);
				}
				if (geometry.SubsetOpaqueByFace[k] != null)
				{
					BlocksManager.DrawMeshBlock(this.m_primitivesRenderer3d, geometry.SubsetOpaqueByFace[k], color5, 1f, ref identity, null);
				}
				if (geometry.SubsetAlphaTestByFace[k] != null)
				{
					BlocksManager.DrawMeshBlock(this.m_primitivesRenderer3d, geometry.SubsetAlphaTestByFace[k], color5, 1f, ref identity, null);
				}
			}
			this.m_primitivesRenderer3d.Flush(matrix, true, int.MaxValue);
			this.m_primitivesRenderer2d.Flush(true, int.MaxValue);
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x000CA36C File Offset: 0x000C856C
		public override void Update()
		{
			if (this.Mode != FurnitureDesignWidget.ViewMode.Perspective)
			{
				return;
			}
			if (base.Input.Tap != null && base.HitTestGlobal(base.Input.Tap.Value, null) == this)
			{
				this.m_dragStartPoint = base.Input.Tap;
			}
			if (base.Input.Press != null)
			{
				if (this.m_dragStartPoint != null)
				{
					Vector2 vector = base.ScreenToWidget(base.Input.Press.Value) - base.ScreenToWidget(this.m_dragStartPoint.Value);
					Vector2 vector2 = default(Vector2);
					vector2.Y = -0.01f * vector.X;
					vector2.X = 0.01f * vector.Y;
					if (Time.FrameDuration > 0f)
					{
						this.m_rotationSpeed = vector2 / Time.FrameDuration;
					}
					this.Rotate(vector2);
					this.m_dragStartPoint = base.Input.Press;
					return;
				}
			}
			else
			{
				this.m_dragStartPoint = null;
				this.Rotate(this.m_rotationSpeed * Time.FrameDuration);
				this.m_rotationSpeed *= MathUtils.Pow(0.1f, Time.FrameDuration);
			}
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x000CA4C4 File Offset: 0x000C86C4
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.Design != null);
			base.DesiredSize = this.Size;
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x000CA4E4 File Offset: 0x000C86E4
		public void Rotate(Vector2 angles)
		{
			float num = MathUtils.DegToRad(1f);
			Vector3 axis = Vector3.Normalize(Vector3.Cross(this.m_direction, Vector3.UnitY));
			this.m_direction = Vector3.TransformNormal(this.m_direction, Matrix.CreateRotationY(angles.Y));
			float num2 = MathUtils.Acos(Vector3.Dot(this.m_direction, Vector3.UnitY));
			float num3 = MathUtils.Acos(Vector3.Dot(this.m_direction, -Vector3.UnitY));
			angles.X = MathUtils.Min(angles.X, num2 - num);
			angles.X = MathUtils.Max(angles.X, 0f - (num3 - num));
			this.m_direction = Vector3.TransformNormal(this.m_direction, Matrix.CreateFromAxisAngle(axis, angles.X));
			this.m_direction = Vector3.Normalize(this.m_direction);
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x000CA5BD File Offset: 0x000C87BD
		public void DebugDraw()
		{
		}

		// Token: 0x0400120B RID: 4619
		public static string fName = "FurnitureDesignWidget";

		// Token: 0x0400120C RID: 4620
		public PrimitivesRenderer2D m_primitivesRenderer2d = new PrimitivesRenderer2D();

		// Token: 0x0400120D RID: 4621
		public PrimitivesRenderer3D m_primitivesRenderer3d = new PrimitivesRenderer3D();

		// Token: 0x0400120E RID: 4622
		public Vector2? m_dragStartPoint;

		// Token: 0x0400120F RID: 4623
		public Vector3 m_direction;

		// Token: 0x04001210 RID: 4624
		public Vector2 m_rotationSpeed;

		// Token: 0x04001211 RID: 4625
		public static bool DrawDebugFurniture;

		// Token: 0x0200051E RID: 1310
		public enum ViewMode
		{
			// Token: 0x040018E8 RID: 6376
			Side,
			// Token: 0x040018E9 RID: 6377
			Top,
			// Token: 0x040018EA RID: 6378
			Front,
			// Token: 0x040018EB RID: 6379
			Perspective
		}
	}
}
