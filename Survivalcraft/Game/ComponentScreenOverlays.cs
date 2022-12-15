using System;
using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000201 RID: 513
	public class ComponentScreenOverlays : Component, IDrawable, IUpdateable
	{
		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x000765FF File Offset: 0x000747FF
		// (set) Token: 0x06000F84 RID: 3972 RVA: 0x00076607 File Offset: 0x00074807
		public float BlackoutFactor { get; set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000F85 RID: 3973 RVA: 0x00076610 File Offset: 0x00074810
		// (set) Token: 0x06000F86 RID: 3974 RVA: 0x00076618 File Offset: 0x00074818
		public float RedoutFactor { get; set; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000F87 RID: 3975 RVA: 0x00076621 File Offset: 0x00074821
		// (set) Token: 0x06000F88 RID: 3976 RVA: 0x00076629 File Offset: 0x00074829
		public float GreenoutFactor { get; set; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000F89 RID: 3977 RVA: 0x00076632 File Offset: 0x00074832
		// (set) Token: 0x06000F8A RID: 3978 RVA: 0x0007663A File Offset: 0x0007483A
		public string FloatingMessage { get; set; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000F8B RID: 3979 RVA: 0x00076643 File Offset: 0x00074843
		// (set) Token: 0x06000F8C RID: 3980 RVA: 0x0007664B File Offset: 0x0007484B
		public float FloatingMessageFactor { get; set; }

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000F8D RID: 3981 RVA: 0x00076654 File Offset: 0x00074854
		// (set) Token: 0x06000F8E RID: 3982 RVA: 0x0007665C File Offset: 0x0007485C
		public string Message { get; set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000F8F RID: 3983 RVA: 0x00076665 File Offset: 0x00074865
		// (set) Token: 0x06000F90 RID: 3984 RVA: 0x0007666D File Offset: 0x0007486D
		public float MessageFactor { get; set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00076676 File Offset: 0x00074876
		// (set) Token: 0x06000F92 RID: 3986 RVA: 0x0007667E File Offset: 0x0007487E
		public float IceFactor { get; set; }

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x00076687 File Offset: 0x00074887
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Reset;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x0007668B File Offset: 0x0007488B
		public int[] DrawOrders
		{
			get
			{
				return ComponentScreenOverlays.m_drawOrders;
			}
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x00076694 File Offset: 0x00074894
		public void Update(float dt)
		{
			bool flag = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
			if (flag != this.m_isUnderWater)
			{
				this.m_isUnderWater = flag;
				this.m_waterSurfaceCrossTime = new double?(this.m_subsystemTime.GameTime);
			}
			this.BlackoutFactor = 0f;
			this.RedoutFactor = 0f;
			this.GreenoutFactor = 0f;
			this.IceFactor = 0f;
			this.FloatingMessage = null;
			this.FloatingMessageFactor = 0f;
			this.Message = null;
			this.MessageFactor = 0f;
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0007672C File Offset: 0x0007492C
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentPlayer.GameWidget != camera.GameWidget)
			{
				return;
			}
			if (this.m_waterSurfaceCrossTime != null)
			{
				float num = (float)(this.m_subsystemTime.GameTime - this.m_waterSurfaceCrossTime.Value);
				float num2 = 0.66f * MathUtils.Sqr(MathUtils.Saturate(1f - 0.75f * num));
				if (num2 > 0.01f)
				{
					Matrix matrix = default(Matrix);
					matrix.Translation = Vector3.Zero;
					matrix.Forward = camera.ViewDirection;
					matrix.Right = Vector3.Normalize(Vector3.Cross(camera.ViewUp, matrix.Forward));
					matrix.Up = Vector3.Normalize(Vector3.Cross(matrix.Right, matrix.Forward));
					Vector3 vector = matrix.ToYawPitchRoll();
					Vector2 zero = Vector2.Zero;
					zero.X -= 2f * vector.X / 3.1415927f + 0.05f * MathUtils.Sin(5f * num);
					zero.Y += 2f * vector.Y / 3.1415927f + (this.m_isUnderWater ? (0.75f * num) : (-0.75f * num));
					Texture2D texture = ContentManager.Get<Texture2D>("Textures/SplashOverlay");
					this.DrawTexturedOverlay(camera, texture, new Color(156, 206, 210), num2, num2, zero);
				}
			}
			if (this.IceFactor > 0f)
			{
				this.DrawIceOverlay(camera, this.IceFactor);
			}
			if (this.RedoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, new Color(255, 64, 0), MathUtils.Saturate(2f * (this.RedoutFactor - 0.5f)), this.RedoutFactor);
			}
			if (this.BlackoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, Color.Black, MathUtils.Saturate(2f * (this.BlackoutFactor - 0.5f)), this.BlackoutFactor);
			}
			if (this.GreenoutFactor > 0.01f)
			{
				this.DrawOverlay(camera, new Color(166, 175, 103), this.GreenoutFactor, MathUtils.Saturate(2f * this.GreenoutFactor));
			}
			if (!string.IsNullOrEmpty(this.FloatingMessage) && this.FloatingMessageFactor > 0.01f)
			{
				this.DrawFloatingMessage(camera, this.FloatingMessage, this.FloatingMessageFactor);
			}
			if (!string.IsNullOrEmpty(this.Message) && this.MessageFactor > 0.01f)
			{
				this.DrawMessage(camera, this.Message, this.MessageFactor);
			}
		}

        // Token: 0x06000F97 RID: 3991 RVA: 0x000769C4 File Offset: 0x00074BC4
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x00076A2C File Offset: 0x00074C2C
		public void DrawOverlay(Camera camera, Color color, float innerFactor, float outerFactor)
		{
			Vector2 viewportSize = camera.ViewportSize;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(viewportSize.X, 0f);
			Vector2 vector3 = new Vector2(viewportSize.X, viewportSize.Y);
			Vector2 vector4 = new Vector2(0f, viewportSize.Y);
			Vector2 p = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
			Color color2 = color * outerFactor;
			Color color3 = color * innerFactor;
			FlatBatch2D flatBatch2D = this.m_primitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, BlendState.AlphaBlend);
			int count = flatBatch2D.TriangleVertices.Count;
			flatBatch2D.QueueTriangle(vector, vector2, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector2, vector3, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector3, vector4, p, 0f, color2, color2, color3);
			flatBatch2D.QueueTriangle(vector4, vector, p, 0f, color2, color2, color3);
			flatBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			flatBatch2D.Flush(true);
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00076B4C File Offset: 0x00074D4C
		public void DrawTexturedOverlay(Camera camera, Texture2D texture, Color color, float innerFactor, float outerFactor, Vector2 offset)
		{
			Vector2 viewportSize = camera.ViewportSize;
			float num = viewportSize.X / viewportSize.Y;
			Vector2 vector = new Vector2(0f, 0f);
			Vector2 vector2 = new Vector2(viewportSize.X, 0f);
			Vector2 vector3 = new Vector2(viewportSize.X, viewportSize.Y);
			Vector2 vector4 = new Vector2(0f, viewportSize.Y);
			Vector2 p = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
			offset.X = MathUtils.Remainder(offset.X, 1f);
			offset.Y = MathUtils.Remainder(offset.Y, 1f);
			Vector2 vector5 = new Vector2(0f, 0f) + offset;
			Vector2 vector6 = new Vector2(num, 0f) + offset;
			Vector2 vector7 = new Vector2(num, 1f) + offset;
			Vector2 vector8 = new Vector2(0f, 1f) + offset;
			Vector2 texCoord = new Vector2(num / 2f, 0.5f) + offset;
			Color color2 = color * outerFactor;
			Color color3 = color * innerFactor;
			TexturedBatch2D texturedBatch2D = this.m_primitivesRenderer2D.TexturedBatch(texture, false, 0, DepthStencilState.None, null, BlendState.Additive, SamplerState.PointWrap);
			int count = texturedBatch2D.TriangleVertices.Count;
			texturedBatch2D.QueueTriangle(vector, vector2, p, 0f, vector5, vector6, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector2, vector3, p, 0f, vector6, vector7, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector3, vector4, p, 0f, vector7, vector8, texCoord, color2, color2, color3);
			texturedBatch2D.QueueTriangle(vector4, vector, p, 0f, vector8, vector5, texCoord, color2, color2, color3);
			texturedBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
			texturedBatch2D.Flush(true);
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x00076D38 File Offset: 0x00074F38
		public void DrawIceOverlay(Camera camera, float factor)
		{
			Vector2 viewportSize = camera.ViewportSize;
			float s = (camera.Eye != null) ? 1.3f : 1f;
			float num = (camera.Eye != null) ? MathUtils.Pow(factor, 0.4f) : factor;
			Vector2 v = (camera.Eye != null) ? viewportSize : new Vector2(1f);
			float num2 = v.Length();
			Point2 point = new Point2((int)MathUtils.Round(12f * viewportSize.X / viewportSize.Y), (int)MathUtils.Round(12f));
			if (this.m_iceVertices == null || this.m_cellsCount != point)
			{
				this.m_cellsCount = point;
				this.m_random.Seed(0);
				this.m_iceVertices = new Vector2[(point.X + 1) * (point.Y + 1)];
				for (int i = 0; i <= point.X; i++)
				{
					for (int j = 0; j <= point.Y; j++)
					{
						float num3 = (float)i;
						float num4 = (float)j;
						if (i != 0 && i != point.X)
						{
							num3 += this.m_random.Float(-0.4f, 0.4f);
						}
						if (j != 0 && j != point.Y)
						{
							num4 += this.m_random.Float(-0.4f, 0.4f);
						}
						float x = num3 / (float)point.X;
						float y = num4 / (float)point.Y;
						this.m_iceVertices[i + j * (point.X + 1)] = new Vector2(x, y);
					}
				}
			}
			Vector3 vector = Vector3.UnitX / camera.ProjectionMatrix.M11 * 2f * 0.2f * s;
			Vector3 vector2 = Vector3.UnitY / camera.ProjectionMatrix.M22 * 2f * 0.2f * s;
			Vector3 v2 = -0.2f * Vector3.UnitZ - 0.5f * (vector + vector2);
			if (this.m_light == null || Time.PeriodicEvent(0.05000000074505806, 0.0))
			{
				this.m_light = new float?(LightingManager.CalculateSmoothLight(this.m_subsystemTerrain, camera.ViewPosition) ?? (this.m_light ?? 1f));
			}
			Color color = Color.MultiplyColorOnly(Color.White, this.m_light.Value);
			this.m_random.Seed(0);
			Texture2D texture = ContentManager.Get<Texture2D>("Textures/IceOverlay");
			TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(texture, false, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.PointWrap);
			Vector2 v3 = new Vector2(viewportSize.X / viewportSize.Y, 1f);
			Vector2 vector3 = new Vector2((float)(point.X - 1), (float)(point.Y - 1));
			for (int k = 0; k < point.X; k++)
			{
				for (int l = 0; l < point.Y; l++)
				{
					float num5 = (new Vector2((float)(2 * k) / vector3.X - 1f, (float)(2 * l) / vector3.Y - 1f) * v).Length() / num2;
					if (1f - num5 + this.m_random.Float(0f, 0.05f) < num)
					{
						Vector2 vector4 = this.m_iceVertices[k + l * (point.X + 1)];
						Vector2 vector5 = this.m_iceVertices[k + 1 + l * (point.X + 1)];
						Vector2 vector6 = this.m_iceVertices[k + 1 + (l + 1) * (point.X + 1)];
						Vector2 vector7 = this.m_iceVertices[k + (l + 1) * (point.X + 1)];
						Vector3 vector8 = v2 + vector4.X * vector + vector4.Y * vector2;
						Vector3 p = v2 + vector5.X * vector + vector5.Y * vector2;
						Vector3 vector9 = v2 + vector6.X * vector + vector6.Y * vector2;
						Vector3 p2 = v2 + vector7.X * vector + vector7.Y * vector2;
						Vector2 vector10 = vector4 * v3;
						Vector2 texCoord = vector5 * v3;
						Vector2 vector11 = vector6 * v3;
						Vector2 texCoord2 = vector7 * v3;
						texturedBatch3D.QueueTriangle(vector8, p, vector9, vector10, texCoord, vector11, color);
						texturedBatch3D.QueueTriangle(vector9, p2, vector8, vector11, texCoord2, vector10, color);
					}
				}
			}
			texturedBatch3D.Flush(camera.ProjectionMatrix, true);
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x000772A4 File Offset: 0x000754A4
		public void DrawFloatingMessage(Camera camera, string message, float factor)
		{
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			if (camera.Eye == null)
			{
				Vector2 position = camera.ViewportSize / 2f;
				position.X += 0.07f * camera.ViewportSize.X * (float)MathUtils.Sin(1.7300000190734863 * Time.FrameStartTime);
				position.Y += 0.07f * camera.ViewportSize.Y * (float)MathUtils.Cos(1.1200000047683716 * Time.FrameStartTime);
				FontBatch2D fontBatch2D = this.m_primitivesRenderer2D.FontBatch(font, 1, DepthStencilState.None, null, BlendState.AlphaBlend, null);
				int count = fontBatch2D.TriangleVertices.Count;
				fontBatch2D.QueueText(message, position, 0f, Color.White * factor, TextAnchor.HorizontalCenter, Vector2.One * camera.GameWidget.GlobalScale, Vector2.Zero, 0f);
				fontBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
				fontBatch2D.Flush(true);
				return;
			}
			Vector3 position2 = -4f * Vector3.UnitZ;
			position2.X += 0.28f * (float)MathUtils.Sin(1.7300000190734863 * Time.FrameStartTime);
			position2.Y += 0.28f * (float)MathUtils.Cos(1.1200000047683716 * Time.FrameStartTime);
			FontBatch3D fontBatch3D = this.m_primitivesRenderer3D.FontBatch(font, 1, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, null);
			fontBatch3D.QueueText(message, position2, 0.008f * Vector3.UnitX, -0.008f * Vector3.UnitY, Color.White * factor, TextAnchor.HorizontalCenter, Vector2.Zero);
			fontBatch3D.Flush(camera.ProjectionMatrix, true);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x00077474 File Offset: 0x00075674
		public void DrawMessage(Camera camera, string message, float factor)
		{
			BitmapFont font = ContentManager.Get<BitmapFont>("Fonts/Pericles");
			if (camera.Eye == null)
			{
				Vector2 position = new Vector2(camera.ViewportSize.X / 2f, camera.ViewportSize.Y - 25f);
				FontBatch2D fontBatch2D = this.m_primitivesRenderer2D.FontBatch(font, 0, DepthStencilState.None, null, BlendState.AlphaBlend, null);
				int count = fontBatch2D.TriangleVertices.Count;
				fontBatch2D.QueueText(message, position, 0f, Color.Gray * factor, TextAnchor.HorizontalCenter | TextAnchor.Bottom, Vector2.One * camera.GameWidget.GlobalScale, Vector2.Zero, 0f);
				fontBatch2D.TransformTriangles(camera.ViewportMatrix, count, -1);
				fontBatch2D.Flush(true);
				return;
			}
			Vector3 position2 = -4f * Vector3.UnitZ + -0.24f / camera.ProjectionMatrix.M22 * 2f * Vector3.UnitY;
			FontBatch3D fontBatch3D = this.m_primitivesRenderer3D.FontBatch(font, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, null);
			fontBatch3D.QueueText(message, position2, 0.0104f * Vector3.UnitX, -0.0104f * Vector3.UnitY, Color.White * factor, TextAnchor.HorizontalCenter, Vector2.Zero);
			fontBatch3D.Flush(camera.ProjectionMatrix, true);
		}

		// Token: 0x040009F4 RID: 2548
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009F5 RID: 2549
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009F6 RID: 2550
		public SubsystemSky m_subsystemSky;

		// Token: 0x040009F7 RID: 2551
		public ComponentGui m_componentGui;

		// Token: 0x040009F8 RID: 2552
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040009F9 RID: 2553
		public PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x040009FA RID: 2554
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x040009FB RID: 2555
		public Game.Random m_random = new Game.Random(0);

		// Token: 0x040009FC RID: 2556
		public Vector2[] m_iceVertices;

		// Token: 0x040009FD RID: 2557
		public Point2 m_cellsCount;

		// Token: 0x040009FE RID: 2558
		public float? m_light;

		// Token: 0x040009FF RID: 2559
		public double? m_waterSurfaceCrossTime;

		// Token: 0x04000A00 RID: 2560
		public bool m_isUnderWater;

		// Token: 0x04000A01 RID: 2561
		public static int[] m_drawOrders = new int[]
		{
			1101
		};
	}
}
