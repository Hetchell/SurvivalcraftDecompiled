using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C4 RID: 452
	public class ComponentBlockHighlight : Component, IDrawable, IUpdateable
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000B5B RID: 2907 RVA: 0x00054F80 File Offset: 0x00053180
		// (set) Token: 0x06000B5C RID: 2908 RVA: 0x00054F88 File Offset: 0x00053188
		public Point3? NearbyEditableCell { get; set; }

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000B5D RID: 2909 RVA: 0x00054F91 File Offset: 0x00053191
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.BlockHighlight;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000B5E RID: 2910 RVA: 0x00054F98 File Offset: 0x00053198
		public int[] DrawOrders
		{
			get
			{
				return ComponentBlockHighlight.m_drawOrders;
			}
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x00054FA0 File Offset: 0x000531A0
		public void Update(float dt)
		{
			Camera activeCamera = this.m_componentPlayer.GameWidget.ActiveCamera;
			Ray3? ray = this.m_componentPlayer.ComponentInput.IsControlledByVr ? this.m_componentPlayer.ComponentInput.CalculateVrHandRay() : new Ray3?(new Ray3(activeCamera.ViewPosition, activeCamera.ViewDirection));
			this.NearbyEditableCell = null;
			if (ray != null)
			{
				this.m_highlightRaycastResult = this.m_componentPlayer.ComponentMiner.Raycast(ray.Value, RaycastMode.Digging, true, true, true);
				if (!(this.m_highlightRaycastResult is TerrainRaycastResult))
				{
					return;
				}
				TerrainRaycastResult terrainRaycastResult = (TerrainRaycastResult)this.m_highlightRaycastResult;
				if (terrainRaycastResult.Distance < 3f)
				{
					Point3 point = terrainRaycastResult.CellFace.Point;
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
					if (block is CrossBlock)
					{
						terrainRaycastResult.Distance = MathUtils.Max(terrainRaycastResult.Distance, 0.1f);
						this.m_highlightRaycastResult = terrainRaycastResult;
					}
					if (block.IsEditable)
					{
						this.NearbyEditableCell = new Point3?(terrainRaycastResult.CellFace.Point);
						return;
					}
				}
			}
			else
			{
				this.m_highlightRaycastResult = null;
			}
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x000550F4 File Offset: 0x000532F4
		public void Draw(Camera camera, int drawOrder)
		{
			if (camera.GameWidget.PlayerData == this.m_componentPlayer.PlayerData)
			{
				if (drawOrder == ComponentBlockHighlight.m_drawOrders[0])
				{
					this.DrawFillHighlight(camera);
					this.DrawOutlineHighlight(camera);
					this.DrawReticleHighlight(camera);
					return;
				}
				this.DrawRayHighlight(camera);
			}
		}

        // Token: 0x06000B61 RID: 2913 RVA: 0x00055140 File Offset: 0x00053340
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAnimatedTextures = base.Project.FindSubsystem<SubsystemAnimatedTextures>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.m_shader = ContentManager.Get<Shader>("Shaders/Highlight");
		}

		// Token: 0x06000B62 RID: 2914 RVA: 0x000551A8 File Offset: 0x000533A8
		public void DrawRayHighlight(Camera camera)
		{
			if (camera.Eye == null)
			{
				return;
			}
			Ray3 ray = default(Ray3);
			float num;
			if (this.m_highlightRaycastResult is TerrainRaycastResult)
			{
				TerrainRaycastResult terrainRaycastResult = (TerrainRaycastResult)this.m_highlightRaycastResult;
				ray = terrainRaycastResult.Ray;
				num = MathUtils.Min(terrainRaycastResult.Distance, 2f);
			}
			else if (this.m_highlightRaycastResult is BodyRaycastResult)
			{
				BodyRaycastResult bodyRaycastResult = (BodyRaycastResult)this.m_highlightRaycastResult;
				ray = bodyRaycastResult.Ray;
				num = MathUtils.Min(bodyRaycastResult.Distance, 2f);
			}
			else if (this.m_highlightRaycastResult is MovingBlocksRaycastResult)
			{
				MovingBlocksRaycastResult movingBlocksRaycastResult = (MovingBlocksRaycastResult)this.m_highlightRaycastResult;
				ray = movingBlocksRaycastResult.Ray;
				num = MathUtils.Min(movingBlocksRaycastResult.Distance, 2f);
			}
			else
			{
				if (!(this.m_highlightRaycastResult is Ray3))
				{
					return;
				}
				ray = (Ray3)this.m_highlightRaycastResult;
				num = 2f;
			}
			Color color = Color.White * 0.5f;
			Color color2 = Color.Lerp(color, Color.Transparent, MathUtils.Saturate(num / 2f));
			FlatBatch3D flatBatch3D = this.m_primitivesRenderer3D.FlatBatch(0, null, null, null);
			flatBatch3D.QueueLine(ray.Position, ray.Position + ray.Direction * num, color, color2);
			flatBatch3D.Flush(camera.ViewProjectionMatrix, true);
		}

		// Token: 0x06000B63 RID: 2915 RVA: 0x000552F0 File Offset: 0x000534F0
		public void DrawReticleHighlight(Camera camera)
		{
			if (camera.Eye != null && this.m_highlightRaycastResult is TerrainRaycastResult)
			{
				TerrainRaycastResult terrainRaycastResult = (TerrainRaycastResult)this.m_highlightRaycastResult;
				Vector3 vector = terrainRaycastResult.HitPoint(0f);
				Vector3 vector2 = (!(BlocksManager.Blocks[Terrain.ExtractContents(terrainRaycastResult.Value)] is CrossBlock)) ? CellFace.FaceToVector3(terrainRaycastResult.CellFace.Face) : (-terrainRaycastResult.Ray.Direction);
				float num = Vector3.Distance(camera.ViewPosition, vector);
				float s = 0.03f + MathUtils.Min(0.008f * num, 0.04f);
				float s2 = 0.01f * num;
				Vector3 v = (MathUtils.Abs(Vector3.Dot(vector2, Vector3.UnitY)) < 0.5f) ? Vector3.UnitY : Vector3.UnitX;
				Vector3 vector3 = Vector3.Normalize(Vector3.Cross(vector2, v));
				Vector3 v2 = Vector3.Normalize(Vector3.Cross(vector2, vector3));
				Subtexture subtexture = ContentManager.Get<Subtexture>("Textures/Atlas/Reticle");
				TexturedBatch3D texturedBatch3D = this.m_primitivesRenderer3D.TexturedBatch(subtexture.Texture, false, 0, DepthStencilState.DepthRead, null, null, SamplerState.LinearClamp);
				Vector3 p = vector + s * (-vector3 + v2) + s2 * vector2;
				Vector3 p2 = vector + s * (vector3 + v2) + s2 * vector2;
				Vector3 p3 = vector + s * (vector3 - v2) + s2 * vector2;
				Vector3 p4 = vector + s * (-vector3 - v2) + s2 * vector2;
				Vector2 texCoord = new Vector2(subtexture.TopLeft.X, subtexture.TopLeft.Y);
				Vector2 texCoord2 = new Vector2(subtexture.BottomRight.X, subtexture.TopLeft.Y);
				Vector2 texCoord3 = new Vector2(subtexture.BottomRight.X, subtexture.BottomRight.Y);
				Vector2 texCoord4 = new Vector2(subtexture.TopLeft.X, subtexture.BottomRight.Y);
				texturedBatch3D.QueueQuad(p, p2, p3, p4, texCoord, texCoord2, texCoord3, texCoord4, Color.White);
				texturedBatch3D.Flush(camera.ViewProjectionMatrix, true);
			}
		}

		// Token: 0x06000B64 RID: 2916 RVA: 0x00055558 File Offset: 0x00053758
		public void DrawFillHighlight(Camera camera)
		{
			if (camera.Eye != null && this.m_highlightRaycastResult is TerrainRaycastResult)
			{
				CellFace cellFace = ((TerrainRaycastResult)this.m_highlightRaycastResult).CellFace;
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
				int num = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num];
				if (this.m_geometry == null || cellValue != this.m_value || cellFace != this.m_cellFace)
				{
					this.m_geometry = new ComponentBlockHighlight.Geometry();
					block.GenerateTerrainVertices(this.m_subsystemTerrain.BlockGeometryGenerator, this.m_geometry, cellValue, cellFace.X, cellFace.Y, cellFace.Z);
					this.m_cellFace = cellFace;
					this.m_value = cellValue;
				}
				DynamicArray<TerrainVertex> vertices = this.m_geometry.SubsetOpaque.Vertices;
				DynamicArray<ushort> indices = this.m_geometry.SubsetOpaque.Indices;
				float x = this.m_subsystemSky.ViewFogRange.X;
				float y = this.m_subsystemSky.ViewFogRange.Y;
				Vector3 viewPosition = camera.ViewPosition;
				Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
				Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
				Display.BlendState = BlendState.NonPremultiplied;
				Display.DepthStencilState = DepthStencilState.Default;
				Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
				this.m_shader.GetParameter("u_origin", false).SetValue(v.XZ);
				this.m_shader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
				this.m_shader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
				this.m_shader.GetParameter("u_texture", false).SetValue(this.m_subsystemAnimatedTextures.AnimatedBlocksTexture);
				this.m_shader.GetParameter("u_samplerState", false).SetValue(SamplerState.PointWrap);
				this.m_shader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
				this.m_shader.GetParameter("u_fogStartInvLength", false).SetValue(new Vector2(x, 1f / (y - x)));
				Display.DrawUserIndexed<TerrainVertex>(PrimitiveType.TriangleList, this.m_shader, TerrainVertex.VertexDeclaration, vertices.Array, 0, vertices.Count, indices.Array, 0, indices.Count);
			}
		}

		// Token: 0x06000B65 RID: 2917 RVA: 0x000557F8 File Offset: 0x000539F8
		public void DrawOutlineHighlight(Camera camera)
		{
			if (camera.UsesMovementControls || this.m_componentPlayer.ComponentHealth.Health <= 0f || !this.m_componentPlayer.ComponentGui.ControlsContainerWidget.IsVisible)
			{
				return;
			}
			if (this.m_componentPlayer.ComponentMiner.DigCellFace != null)
			{
				CellFace value = this.m_componentPlayer.ComponentMiner.DigCellFace.Value;
				BoundingBox cellFaceBoundingBox = this.GetCellFaceBoundingBox(value.Point);
				ComponentBlockHighlight.DrawBoundingBoxFace(this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null), value.Face, cellFaceBoundingBox.Min, cellFaceBoundingBox.Max, Color.Black);
			}
			else
			{
				if (!this.m_componentPlayer.ComponentAimingSights.IsSightsVisible && (SettingsManager.LookControlMode == LookControlMode.SplitTouch || !this.m_componentPlayer.ComponentInput.IsControlledByTouch) && this.m_highlightRaycastResult is TerrainRaycastResult)
				{
					CellFace cellFace = ((TerrainRaycastResult)this.m_highlightRaycastResult).CellFace;
					BoundingBox cellFaceBoundingBox2 = this.GetCellFaceBoundingBox(cellFace.Point);
					ComponentBlockHighlight.DrawBoundingBoxFace(this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null), cellFace.Face, cellFaceBoundingBox2.Min, cellFaceBoundingBox2.Max, Color.Black);
				}
				if (this.NearbyEditableCell != null)
				{
					BoundingBox cellFaceBoundingBox3 = this.GetCellFaceBoundingBox(this.NearbyEditableCell.Value);
					this.m_primitivesRenderer3D.FlatBatch(0, DepthStencilState.None, null, null).QueueBoundingBox(cellFaceBoundingBox3, Color.Black);
				}
			}
			this.m_primitivesRenderer3D.Flush(camera.ViewProjectionMatrix, true, int.MaxValue);
		}

		// Token: 0x06000B66 RID: 2918 RVA: 0x0005599C File Offset: 0x00053B9C
		public static void DrawBoundingBoxFace(FlatBatch3D batch, int face, Vector3 c1, Vector3 c2, Color color)
		{
			switch (face)
			{
			case 0:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c2.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c2.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c2.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c2.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				return;
			case 1:
				batch.QueueLine(new Vector3(c2.X, c1.Y, c2.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				return;
			case 2:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c1.Y, c1.Z), color);
				return;
			case 3:
				batch.QueueLine(new Vector3(c1.X, c2.Y, c2.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				return;
			case 4:
				batch.QueueLine(new Vector3(c2.X, c2.Y, c2.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c2.Y, c1.Z), new Vector3(c1.X, c2.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c2.Y, c1.Z), new Vector3(c2.X, c2.Y, c2.Z), color);
				return;
			case 5:
				batch.QueueLine(new Vector3(c1.X, c1.Y, c2.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c1.Z), color);
				batch.QueueLine(new Vector3(c1.X, c1.Y, c1.Z), new Vector3(c1.X, c1.Y, c2.Z), color);
				batch.QueueLine(new Vector3(c2.X, c1.Y, c1.Z), new Vector3(c2.X, c1.Y, c2.Z), color);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000B67 RID: 2919 RVA: 0x00055EE0 File Offset: 0x000540E0
		public BoundingBox GetCellFaceBoundingBox(Point3 point)
		{
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			BoundingBox[] customCollisionBoxes = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)].GetCustomCollisionBoxes(this.m_subsystemTerrain, cellValue);
			Vector3 vector = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
			if (customCollisionBoxes.Length != 0)
			{
				BoundingBox? boundingBox = null;
				for (int i = 0; i < customCollisionBoxes.Length; i++)
				{
					if (customCollisionBoxes[i] != default(BoundingBox))
					{
						boundingBox = new BoundingBox?((boundingBox != null) ? BoundingBox.Union(boundingBox.Value, customCollisionBoxes[i]) : customCollisionBoxes[i]);
					}
				}
				if (boundingBox == null)
				{
					boundingBox = new BoundingBox?(new BoundingBox(Vector3.Zero, Vector3.One));
				}
				return new BoundingBox(boundingBox.Value.Min + vector, boundingBox.Value.Max + vector);
			}
			return new BoundingBox(vector, vector + Vector3.One);
		}

		// Token: 0x04000655 RID: 1621
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000656 RID: 1622
		public SubsystemAnimatedTextures m_subsystemAnimatedTextures;

		// Token: 0x04000657 RID: 1623
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000658 RID: 1624
		public ComponentPlayer m_componentPlayer;

		// Token: 0x04000659 RID: 1625
		public PrimitivesRenderer3D m_primitivesRenderer3D = new PrimitivesRenderer3D();

		// Token: 0x0400065A RID: 1626
		public Shader m_shader;

		// Token: 0x0400065B RID: 1627
		public ComponentBlockHighlight.Geometry m_geometry;

		// Token: 0x0400065C RID: 1628
		public CellFace m_cellFace;

		// Token: 0x0400065D RID: 1629
		public int m_value;

		// Token: 0x0400065E RID: 1630
		public object m_highlightRaycastResult;

		// Token: 0x0400065F RID: 1631
		public static int[] m_drawOrders = new int[]
		{
			1,
			2000
		};

		// Token: 0x02000448 RID: 1096
		public class Geometry : TerrainGeometry
		{
			// Token: 0x06001EBE RID: 7870 RVA: 0x000DFFC0 File Offset: 0x000DE1C0
			public Geometry()
			{
				TerrainGeometrySubset terrainGeometrySubset = new TerrainGeometrySubset();
				TerrainGeometrySubset[] array = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.SubsetOpaque = terrainGeometrySubset;
				this.SubsetAlphaTest = terrainGeometrySubset;
				this.SubsetTransparent = terrainGeometrySubset;
				this.OpaqueSubsetsByFace = array;
				this.AlphaTestSubsetsByFace = array;
				this.TransparentSubsetsByFace = array;
			}
		}
	}
}
