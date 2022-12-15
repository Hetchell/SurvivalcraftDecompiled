using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000182 RID: 386
	public class SubsystemGlow : Subsystem, IDrawable
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060008D6 RID: 2262 RVA: 0x0003CF18 File Offset: 0x0003B118
		public int[] DrawOrders
		{
			get
			{
				return SubsystemGlow.m_drawOrders;
			}
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0003CF20 File Offset: 0x0003B120
		public GlowPoint AddGlowPoint()
		{
			GlowPoint glowPoint = new GlowPoint();
			this.m_glowPoints.Add(glowPoint, true);
			return glowPoint;
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0003CF41 File Offset: 0x0003B141
		public void RemoveGlowPoint(GlowPoint glowPoint)
		{
			this.m_glowPoints.Remove(glowPoint);
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x0003CF50 File Offset: 0x0003B150
		public void Draw(Camera camera, int drawOrder)
		{
			foreach (GlowPoint glowPoint in this.m_glowPoints.Keys)
			{
				if (glowPoint.Color.A > 0)
				{
					Vector3 vector = glowPoint.Position - camera.ViewPosition;
					float num = Vector3.Dot(vector, camera.ViewDirection);
					if (num > 0.01f)
					{
						float num2 = vector.Length();
						if (num2 < this.m_subsystemSky.ViewFogRange.Y)
						{
							float num3 = glowPoint.Size;
							if (glowPoint.FarDistance > 0f)
							{
								num3 += (glowPoint.FarSize - glowPoint.Size) * MathUtils.Saturate(num2 / glowPoint.FarDistance);
							}
							Vector3 v = (0f - (0.01f + 0.02f * num)) / num2 * vector;
							Vector3 p = glowPoint.Position + num3 * (-glowPoint.Right - glowPoint.Up) + v;
							Vector3 p2 = glowPoint.Position + num3 * (glowPoint.Right - glowPoint.Up) + v;
							Vector3 p3 = glowPoint.Position + num3 * (glowPoint.Right + glowPoint.Up) + v;
							Vector3 p4 = glowPoint.Position + num3 * (-glowPoint.Right + glowPoint.Up) + v;
							this.m_batchesByType[(int)glowPoint.Type].QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), glowPoint.Color);
						}
					}
				}
			}
			this.m_primitivesRenderer.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

        // Token: 0x060008DA RID: 2266 RVA: 0x0003D190 File Offset: 0x0003B390
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_batchesByType[0] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/RoundGlow"), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[1] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/SquareGlow"), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[2] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/HorizontalRectGlow"), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
			this.m_batchesByType[3] = this.m_primitivesRenderer.TexturedBatch(ContentManager.Get<Texture2D>("Textures/VerticalRectGlow"), false, 0, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwiseScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
		}

		// Token: 0x040004A6 RID: 1190
		public SubsystemSky m_subsystemSky;

		// Token: 0x040004A7 RID: 1191
		public Dictionary<GlowPoint, bool> m_glowPoints = new Dictionary<GlowPoint, bool>();

		// Token: 0x040004A8 RID: 1192
		public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

		// Token: 0x040004A9 RID: 1193
		public TexturedBatch3D[] m_batchesByType = new TexturedBatch3D[4];

		// Token: 0x040004AA RID: 1194
		public static int[] m_drawOrders = new int[]
		{
			110
		};
	}
}
