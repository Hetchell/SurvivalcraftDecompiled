using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BC RID: 444
	public class ComponentAimingSights : Component, IUpdateable, IDrawable
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000B1E RID: 2846 RVA: 0x00052EFF File Offset: 0x000510FF
		// (set) Token: 0x06000B1F RID: 2847 RVA: 0x00052F07 File Offset: 0x00051107
		public bool IsSightsVisible { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000B20 RID: 2848 RVA: 0x00052F10 File Offset: 0x00051110
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Reset;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000B21 RID: 2849 RVA: 0x00052F14 File Offset: 0x00051114
		public int[] DrawOrders
		{
			get
			{
				return ComponentAimingSights.m_drawOrders;
			}
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x00052F1B File Offset: 0x0005111B
		public void ShowAimingSights(Vector3 position, Vector3 direction)
		{
			this.IsSightsVisible = true;
			this.m_sightsPosition = position;
			this.m_sightsDirection = direction;
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x00052F32 File Offset: 0x00051132
		public void Update(float dt)
		{
			this.IsSightsVisible = false;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x00052F3C File Offset: 0x0005113C
		public void Draw(Camera camera, int drawOrder)
		{
			if (camera.GameWidget != this.m_componentPlayer.GameWidget)
			{
				return;
			}
			if (this.m_componentPlayer.ComponentHealth.Health > 0f && this.m_componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible)
			{
				if (this.IsSightsVisible)
				{
					Texture2D texture = ContentManager.Get<Texture2D>("Textures/Gui/Sights");
					float s = (camera.Eye == null) ? 8f : 2.5f;
					Vector3 v = this.m_sightsPosition + this.m_sightsDirection * 50f;
					Vector3 vector = Vector3.Normalize(Vector3.Cross(this.m_sightsDirection, Vector3.UnitY));
					Vector3 v2 = Vector3.Normalize(Vector3.Cross(this.m_sightsDirection, vector));
					Vector3 p = v + s * (-vector - v2);
					Vector3 p2 = v + s * (vector - v2);
					Vector3 p3 = v + s * (vector + v2);
					Vector3 p4 = v + s * (-vector + v2);
					TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(texture, false, 0, DepthStencilState.None, null, null, null);
					int count = texturedBatch3D.TriangleVertices.Count;
					texturedBatch3D.QueueQuad(p, p2, p3, p4, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 1f), Color.White);
					texturedBatch3D.TransformTriangles(camera.ViewMatrix, count, -1);
				}
				if (camera.Eye == null && !camera.UsesMovementControls && !this.IsSightsVisible && (SettingsManager.LookControlMode == LookControlMode.SplitTouch || !this.m_componentPlayer.ComponentInput.IsControlledByTouch))
				{
					Subtexture subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Crosshair");
					float s2 = 1.25f;
					Vector3 v3 = camera.ViewPosition + camera.ViewDirection * 50f;
					Vector3 vector2 = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY));
					Vector3 v4 = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, vector2));
					Vector3 p5 = v3 + s2 * (-vector2 - v4);
					Vector3 p6 = v3 + s2 * (vector2 - v4);
					Vector3 p7 = v3 + s2 * (vector2 + v4);
					Vector3 p8 = v3 + s2 * (-vector2 + v4);
					TexturedBatch3D texturedBatch3D2 = this.m_primitivesRenderer3D.TexturedBatch(subtexture.Texture, false, 0, DepthStencilState.None, null, null, null);
					int count2 = texturedBatch3D2.TriangleVertices.Count;
					texturedBatch3D2.QueueQuad(p5, p6, p7, p8, new Vector2(subtexture.TopLeft.X, subtexture.TopLeft.Y), new Vector2(subtexture.BottomRight.X, subtexture.TopLeft.Y), new Vector2(subtexture.BottomRight.X, subtexture.BottomRight.Y), new Vector2(subtexture.TopLeft.X, subtexture.BottomRight.Y), Color.White);
					texturedBatch3D2.TransformTriangles(camera.ViewMatrix, count2, -1);
				}
			}
			this.m_primitivesRenderer2D.Flush(true, int.MaxValue);
			this.m_primitivesRenderer3D.Flush(camera.ProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x000532D4 File Offset: 0x000514D4
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
		}

		// Token: 0x04000618 RID: 1560
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000619 RID: 1561
		public readonly PrimitivesRenderer2D m_primitivesRenderer2D = new PrimitivesRenderer2D();

		// Token: 0x0400061A RID: 1562
		public readonly PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x0400061B RID: 1563
		public Vector3 m_sightsPosition;

		// Token: 0x0400061C RID: 1564
		public Vector3 m_sightsDirection;

		// Token: 0x0400061D RID: 1565
		public static int[] m_drawOrders = new int[]
		{
			2000
		};
	}
}
