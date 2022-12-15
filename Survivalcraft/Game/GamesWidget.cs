using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000386 RID: 902
	public class GamesWidget : ContainerWidget
	{
		// Token: 0x060019C4 RID: 6596 RVA: 0x000CA808 File Offset: 0x000C8A08
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.MeasureOverride(parentAvailableSize);
			base.IsOverdrawRequired = (this.Children.Count > 1);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x000CA828 File Offset: 0x000C8A28
		public override void ArrangeOverride()
		{
			if (this.Children.Count == 1)
			{
				ContainerWidget.ArrangeChildWidgetInCell(Vector2.Zero, base.ActualSize, this.Children[0]);
				if (SettingsManager.ScreenLayout1 == ScreenLayout.Single)
				{
					this.Children[0].LayoutTransform = Matrix.Identity;
					return;
				}
			}
			else if (this.Children.Count == 2)
			{
				if (SettingsManager.ScreenLayout2 == ScreenLayout.DoubleVertical)
				{
					this.m_spacing = 12f;
					this.m_bevel = 3f;
					float x = 0f;
					float y = 0f;
					float x2 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y2 = 0f;
					float x3 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y3 = base.ActualSize.Y;
					float num = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x, y), new Vector2(x, y) + new Vector2(x3, y3) / num, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num, num, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x2, y2), new Vector2(x2, y2) + new Vector2(x3, y3) / num, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num, num, 1f);
				}
				if (SettingsManager.ScreenLayout2 == ScreenLayout.DoubleHorizontal)
				{
					this.m_spacing = 12f;
					this.m_bevel = 3f;
					float x4 = 0f;
					float y4 = 0f;
					float x5 = 0f;
					float y5 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x6 = base.ActualSize.X;
					float y6 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num2 = 0.48f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x4, y4), new Vector2(x4, y4) + new Vector2(x6, y6) / num2, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num2, num2, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x5, y5), new Vector2(x5, y5) + new Vector2(x6, y6) / num2, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num2, num2, 1f);
				}
				if (SettingsManager.ScreenLayout2 == ScreenLayout.DoubleOpposite)
				{
					this.m_spacing = 20f;
					this.m_bevel = 4f;
					float x7 = 0f;
					float y7 = 0f;
					float x8 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y8 = 0f;
					float x9 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y9 = base.ActualSize.Y;
					float num3 = (float)Window.Size.Y / (float)Window.Size.X;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x7, y7), new Vector2(x7, y7) + new Vector2(x9, y9) / num3, this.Children[0]);
					this.Children[0].LayoutTransform = new Matrix(0f, num3, 0f, 0f, 0f - num3, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x8, y8), new Vector2(x8, y8) + new Vector2(x9, y9) / num3, this.Children[1]);
					this.Children[1].LayoutTransform = new Matrix(0f, 0f - num3, 0f, 0f, num3, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					return;
				}
			}
			else if (this.Children.Count == 3)
			{
				this.m_spacing = 12f;
				this.m_bevel = 3f;
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleVertical)
				{
					float x10 = 0f;
					float y10 = 0f;
					float x11 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y11 = 0f;
					float x12 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y12 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x13 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y13 = base.ActualSize.Y;
					float y14 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num4 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x10, y10), new Vector2(x10, y10) + new Vector2(x13, y13) / num4, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num4, num4, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x11, y11), new Vector2(x11, y11) + new Vector2(x13, y14) / num4, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num4, num4, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x12, y12), new Vector2(x12, y12) + new Vector2(x13, y14) / num4, this.Children[2]);
					this.Children[2].LayoutTransform = Matrix.CreateScale(num4, num4, 1f);
				}
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleHorizontal)
				{
					float x14 = 0f;
					float y15 = 0f;
					float x15 = 0f;
					float y16 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x16 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y17 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x17 = base.ActualSize.X;
					float x18 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y18 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num5 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x14, y15), new Vector2(x14, y15) + new Vector2(x17, y18) / num5, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num5, num5, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x15, y16), new Vector2(x15, y16) + new Vector2(x18, y18) / num5, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num5, num5, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x16, y17), new Vector2(x16, y17) + new Vector2(x18, y18) / num5, this.Children[2]);
					this.Children[2].LayoutTransform = Matrix.CreateScale(num5, num5, 1f);
				}
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleEven)
				{
					float x19 = 0f;
					float y19 = 0f;
					float x20 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y20 = 0f;
					float x21 = base.ActualSize.X / 4f + this.m_spacing / 4f;
					float y21 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x22 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y22 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num6 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x19, y19), new Vector2(x19, y19) + new Vector2(x22, y22) / num6, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num6, num6, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x20, y20), new Vector2(x20, y20) + new Vector2(x22, y22) / num6, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num6, num6, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x21, y21), new Vector2(x21, y21) + new Vector2(x22, y22) / num6, this.Children[2]);
					this.Children[2].LayoutTransform = Matrix.CreateScale(num6, num6, 1f);
				}
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleOpposite)
				{
					float x23 = 0f;
					float y23 = 0f;
					float x24 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y24 = 0f;
					float x25 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y25 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x26 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y26 = base.ActualSize.Y;
					float y27 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num7 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x23, y23), new Vector2(x23, y23) + new Vector2(x26, y26) / num7, this.Children[0]);
					this.Children[0].LayoutTransform = new Matrix(0f, num7, 0f, 0f, 0f - num7, 0f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x24, y24), new Vector2(x24, y24) + new Vector2(x26, y27) / num7, this.Children[1]);
					this.Children[1].LayoutTransform = new Matrix(0f - num7, 0f, 0f, 0f, 0f, 0f - num7, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x25, y25), new Vector2(x25, y25) + new Vector2(x26, y27) / num7, this.Children[2]);
					this.Children[2].LayoutTransform = new Matrix(num7, 0f, 0f, 0f, 0f, num7, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					return;
				}
			}
			else if (this.Children.Count == 4)
			{
				if (SettingsManager.ScreenLayout4 == ScreenLayout.Quadruple)
				{
					this.m_spacing = 12f;
					this.m_bevel = 3f;
					float x27 = 0f;
					float y28 = 0f;
					float x28 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y29 = 0f;
					float x29 = 0f;
					float y30 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x30 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y31 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x31 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y32 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num8 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x27, y28), new Vector2(x27, y28) + new Vector2(x31, y32) / num8, this.Children[0]);
					this.Children[0].LayoutTransform = Matrix.CreateScale(num8, num8, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x28, y29), new Vector2(x28, y29) + new Vector2(x31, y32) / num8, this.Children[1]);
					this.Children[1].LayoutTransform = Matrix.CreateScale(num8, num8, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x29, y30), new Vector2(x29, y30) + new Vector2(x31, y32) / num8, this.Children[2]);
					this.Children[2].LayoutTransform = Matrix.CreateScale(num8, num8, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x30, y31), new Vector2(x30, y31) + new Vector2(x31, y32) / num8, this.Children[3]);
					this.Children[3].LayoutTransform = Matrix.CreateScale(num8, num8, 1f);
				}
				if (SettingsManager.ScreenLayout4 == ScreenLayout.QuadrupleOpposite)
				{
					this.m_spacing = 12f;
					this.m_bevel = 3f;
					float x32 = 0f;
					float y33 = 0f;
					float x33 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y34 = 0f;
					float x34 = 0f;
					float y35 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x35 = base.ActualSize.X / 2f + this.m_spacing / 2f;
					float y36 = base.ActualSize.Y / 2f + this.m_spacing / 2f;
					float x36 = base.ActualSize.X / 2f - this.m_spacing / 2f;
					float y37 = base.ActualSize.Y / 2f - this.m_spacing / 2f;
					float num9 = 0.5f;
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x32, y33), new Vector2(x32, y33) + new Vector2(x36, y37) / num9, this.Children[0]);
					this.Children[0].LayoutTransform = new Matrix(0f - num9, 0f, 0f, 0f, 0f, 0f - num9, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x33, y34), new Vector2(x33, y34) + new Vector2(x36, y37) / num9, this.Children[1]);
					this.Children[1].LayoutTransform = new Matrix(0f - num9, 0f, 0f, 0f, 0f, 0f - num9, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x34, y35), new Vector2(x34, y35) + new Vector2(x36, y37) / num9, this.Children[2]);
					this.Children[2].LayoutTransform = new Matrix(num9, 0f, 0f, 0f, 0f, num9, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					ContainerWidget.ArrangeChildWidgetInCell(new Vector2(x35, y36), new Vector2(x35, y36) + new Vector2(x36, y37) / num9, this.Children[3]);
					this.Children[3].LayoutTransform = new Matrix(num9, 0f, 0f, 0f, 0f, num9, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
					return;
				}
			}
			else if (this.Children.Count > 4)
			{
				throw new InvalidOperationException("Too many GameWidgets.");
			}
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x000CBB2C File Offset: 0x000C9D2C
		public override void Overdraw(Widget.DrawContext dc)
		{
			Color color = new Color(181, 172, 154) * base.GlobalColorTransform;
			float num = 0.6f;
			float directionalLight = 0.4f;
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(0, null, null, null);
			int count = flatBatch2D.TriangleVertices.Count;
			if (this.Children.Count == 2)
			{
				if (SettingsManager.ScreenLayout2 == ScreenLayout.DoubleVertical || SettingsManager.ScreenLayout2 == ScreenLayout.DoubleOpposite)
				{
					Vector2 c = new Vector2(base.ActualSize.X / 2f - this.m_spacing / 2f, -100f);
					Vector2 c2 = new Vector2(base.ActualSize.X / 2f + this.m_spacing / 2f, base.ActualSize.Y + 100f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, c, c2, 0f, this.m_bevel, color, color, Color.Transparent, num, directionalLight, 0f);
				}
				if (SettingsManager.ScreenLayout2 == ScreenLayout.DoubleHorizontal)
				{
					Vector2 c3 = new Vector2(-100f, base.ActualSize.Y / 2f - this.m_spacing / 2f);
					Vector2 c4 = new Vector2(base.ActualSize.X + 100f, base.ActualSize.Y / 2f + this.m_spacing / 2f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, c3, c4, 0f, this.m_bevel, color, color, Color.Transparent, num, directionalLight, 0f);
				}
			}
			else if (this.Children.Count == 3)
			{
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleVertical || SettingsManager.ScreenLayout3 == ScreenLayout.TripleOpposite)
				{
					float x = -100f;
					float x2 = base.ActualSize.X / 2f - this.m_spacing / 2f + this.m_bevel;
					float x3 = base.ActualSize.X / 2f + this.m_spacing / 2f - this.m_bevel;
					float x4 = base.ActualSize.X + 100f;
					float y = -100f;
					float y2 = base.ActualSize.Y / 2f - this.m_spacing / 2f + this.m_bevel;
					float y3 = base.ActualSize.Y / 2f + this.m_spacing / 2f - this.m_bevel;
					float y4 = base.ActualSize.Y + 100f;
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x, y), new Vector2(x2, y4), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x3, y), new Vector2(x4, y2), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x3, y3), new Vector2(x4, y4), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					Color color2 = color * new Color(num, num, num, 1f);
					flatBatch2D.QueueQuad(new Vector2(x2, y), new Vector2(x3, y4), 0f, color2);
					flatBatch2D.QueueQuad(new Vector2(x3, y2), new Vector2(x4, y3), 0f, color2);
				}
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleHorizontal)
				{
					float x5 = -100f;
					float x6 = base.ActualSize.X / 2f - this.m_spacing / 2f + this.m_bevel;
					float x7 = base.ActualSize.X / 2f + this.m_spacing / 2f - this.m_bevel;
					float x8 = base.ActualSize.X + 100f;
					float y5 = -100f;
					float y6 = base.ActualSize.Y / 2f - this.m_spacing / 2f + this.m_bevel;
					float y7 = base.ActualSize.Y / 2f + this.m_spacing / 2f - this.m_bevel;
					float y8 = base.ActualSize.Y + 100f;
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x5, y5), new Vector2(x8, y6), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x5, y7), new Vector2(x6, y8), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x7, y7), new Vector2(x8, y8), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					Color color3 = color * new Color(num, num, num, 1f);
					flatBatch2D.QueueQuad(new Vector2(x5, y6), new Vector2(x8, y7), 0f, color3);
					flatBatch2D.QueueQuad(new Vector2(x6, y7), new Vector2(x7, y8), 0f, color3);
				}
				if (SettingsManager.ScreenLayout3 == ScreenLayout.TripleEven)
				{
					float x9 = -100f;
					float x10 = base.ActualSize.X / 2f - this.m_spacing / 2f + this.m_bevel;
					float x11 = base.ActualSize.X / 2f + this.m_spacing / 2f - this.m_bevel;
					float x12 = base.ActualSize.X + 100f;
					float x13 = base.ActualSize.X / 4f;
					float x14 = base.ActualSize.X * 3f / 4f;
					float y9 = -100f;
					float y10 = base.ActualSize.Y / 2f - this.m_spacing / 2f + this.m_bevel;
					float y11 = base.ActualSize.Y / 2f + this.m_spacing / 2f - this.m_bevel;
					float y12 = base.ActualSize.Y + 100f;
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x9, y9), new Vector2(x10, y10), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x11, y9), new Vector2(x12, y10), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x13, y11), new Vector2(x14, y12), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
					Color color4 = color * new Color(num, num, num, 1f);
					flatBatch2D.QueueQuad(new Vector2(x10, y9), new Vector2(x11, y10), 0f, color4);
					flatBatch2D.QueueQuad(new Vector2(x9, y10), new Vector2(x12, y11), 0f, color4);
					flatBatch2D.QueueQuad(new Vector2(x9, y11), new Vector2(x13, y12), 0f, color4);
					flatBatch2D.QueueQuad(new Vector2(x14, y11), new Vector2(x12, y12), 0f, color4);
				}
			}
			else if (this.Children.Count == 4)
			{
				float x15 = -100f;
				float x16 = base.ActualSize.X / 2f - this.m_spacing / 2f + this.m_bevel;
				float x17 = base.ActualSize.X / 2f + this.m_spacing / 2f - this.m_bevel;
				float x18 = base.ActualSize.X + 100f;
				float y13 = -100f;
				float y14 = base.ActualSize.Y / 2f - this.m_spacing / 2f + this.m_bevel;
				float y15 = base.ActualSize.Y / 2f + this.m_spacing / 2f - this.m_bevel;
				float y16 = base.ActualSize.Y + 100f;
				BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x15, y13), new Vector2(x16, y14), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
				BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x17, y13), new Vector2(x18, y14), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
				BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x15, y15), new Vector2(x16, y16), 0f, 0f - this.m_bevel, Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
				BevelledRectangleWidget.QueueBevelledRectangle(null, flatBatch2D, new Vector2(x17, y15), new Vector2(x18, y16), 0f, 0f - this.m_bevel, (this.Children.Count == 3) ? color : Color.Transparent, color, Color.Transparent, num, directionalLight, 0f);
				Color color5 = color * new Color(num, num, num, 1f);
				flatBatch2D.QueueQuad(new Vector2(x16, y13), new Vector2(x17, y16), 0f, color5);
				flatBatch2D.QueueQuad(new Vector2(x15, y14), new Vector2(x18, y15), 0f, color5);
			}
			else if (this.Children.Count > 4)
			{
				throw new InvalidOperationException("Too many GameWidgets.");
			}
			flatBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
		}

		// Token: 0x04001218 RID: 4632
		public float m_spacing;

		// Token: 0x04001219 RID: 4633
		public float m_bevel;
	}
}
