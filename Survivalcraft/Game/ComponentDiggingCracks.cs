using System;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001D2 RID: 466
	public class ComponentDiggingCracks : Component, IDrawable
	{
		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06000C8D RID: 3213 RVA: 0x0005DCE5 File Offset: 0x0005BEE5
		public int[] DrawOrders
		{
			get
			{
				return ComponentDiggingCracks.m_drawOrders;
			}
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0005DCEC File Offset: 0x0005BEEC
		public void Draw(Camera camera, int drawOrder)
		{
			if (this.m_componentMiner.DigCellFace == null || this.m_componentMiner.DigProgress <= 0f || this.m_componentMiner.DigTime <= 0.2f)
			{
				return;
			}
			Point3 point = this.m_componentMiner.DigCellFace.Value.Point;
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			if (this.m_geometry == null || cellValue != this.m_value || point != this.m_point)
			{
				this.m_geometry = new ComponentDiggingCracks.Geometry();
				block.GenerateTerrainVertices(this.m_subsystemTerrain.BlockGeometryGenerator, this.m_geometry, cellValue, point.X, point.Y, point.Z);
				this.m_point = point;
				this.m_value = cellValue;
				this.m_vertices.Clear();
				ComponentDiggingCracks.CracksVertex item = default(ComponentDiggingCracks.CracksVertex);
				for (int i = 0; i < this.m_geometry.SubsetOpaque.Vertices.Count; i++)
				{
					TerrainVertex terrainVertex = this.m_geometry.SubsetOpaque.Vertices.Array[i];
					int b = (terrainVertex.Color.R + terrainVertex.Color.G + terrainVertex.Color.B) / 3;
					item.X = terrainVertex.X;
					item.Y = terrainVertex.Y;
					item.Z = terrainVertex.Z;
					item.Tx = (float)terrainVertex.Tx / 32767f * 16f;
					item.Ty = (float)terrainVertex.Ty / 32767f * 16f;
					item.Color = new Color(b, b, b, 128);
					this.m_vertices.Add(item);
				}
			}
			Vector3 viewPosition = camera.ViewPosition;
			Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
			Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
			DynamicArray<ushort> indices = this.m_geometry.SubsetOpaque.Indices;
			float x = this.m_subsystemSky.ViewFogRange.X;
			float y = this.m_subsystemSky.ViewFogRange.Y;
			int num2 = MathUtils.Clamp((int)(this.m_componentMiner.DigProgress * 8f), 0, 7);
			Display.BlendState = BlendState.NonPremultiplied;
			Display.DepthStencilState = DepthStencilState.Default;
			Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
			this.m_shader.GetParameter("u_origin", false).SetValue(v.XZ);
			this.m_shader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
			this.m_shader.GetParameter("u_viewPosition", false).SetValue(camera.ViewPosition);
			this.m_shader.GetParameter("u_texture", false).SetValue(this.m_textures[num2]);
			this.m_shader.GetParameter("u_samplerState", false).SetValue(SamplerState.PointWrap);
			this.m_shader.GetParameter("u_fogColor", false).SetValue(new Vector3(this.m_subsystemSky.ViewFogColor));
			this.m_shader.GetParameter("u_fogStartInvLength", false).SetValue(new Vector2(x, 1f / (y - x)));
			Display.DrawUserIndexed<ComponentDiggingCracks.CracksVertex>(PrimitiveType.TriangleList, this.m_shader, ComponentDiggingCracks.CracksVertex.VertexDeclaration, this.m_vertices.Array, 0, this.m_vertices.Count, indices.Array, 0, indices.Count);
		}

        // Token: 0x06000C8F RID: 3215 RVA: 0x0005E0D0 File Offset: 0x0005C2D0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.m_shader = ContentManager.Get<Shader>("Shaders/AlphaTested");
			this.m_textures = new Texture2D[8];
			for (int i = 0; i < 8; i++)
			{
				this.m_textures[i] = ContentManager.Get<Texture2D>(string.Format("Textures/Cracks{0}", i + 1));
			}
		}

		// Token: 0x0400073E RID: 1854
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400073F RID: 1855
		public SubsystemSky m_subsystemSky;

		// Token: 0x04000740 RID: 1856
		public ComponentMiner m_componentMiner;

		// Token: 0x04000741 RID: 1857
		public Texture2D[] m_textures;

		// Token: 0x04000742 RID: 1858
		public Shader m_shader;

		// Token: 0x04000743 RID: 1859
		public ComponentDiggingCracks.Geometry m_geometry;

		// Token: 0x04000744 RID: 1860
		public DynamicArray<ComponentDiggingCracks.CracksVertex> m_vertices = new DynamicArray<ComponentDiggingCracks.CracksVertex>();

		// Token: 0x04000745 RID: 1861
		public Point3 m_point;

		// Token: 0x04000746 RID: 1862
		public int m_value;

		// Token: 0x04000747 RID: 1863
		public static int[] m_drawOrders = new int[]
		{
			1
		};

		// Token: 0x0200044E RID: 1102
		public class Geometry : TerrainGeometry
		{
			// Token: 0x06001ECB RID: 7883 RVA: 0x000E00A8 File Offset: 0x000DE2A8
			public Geometry()
			{
				TerrainGeometrySubset terrainGeometrySubset = this.SubsetTransparent = (this.SubsetAlphaTest = (this.SubsetOpaque = new TerrainGeometrySubset()));
				this.OpaqueSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.AlphaTestSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
				this.TransparentSubsetsByFace = new TerrainGeometrySubset[]
				{
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset,
					terrainGeometrySubset
				};
			}
		}

		// Token: 0x0200044F RID: 1103
		public struct CracksVertex
		{
			// Token: 0x0400162F RID: 5679
			public float X;

			// Token: 0x04001630 RID: 5680
			public float Y;

			// Token: 0x04001631 RID: 5681
			public float Z;

			// Token: 0x04001632 RID: 5682
			public float Tx;

			// Token: 0x04001633 RID: 5683
			public float Ty;

			// Token: 0x04001634 RID: 5684
			public Color Color;

			// Token: 0x04001635 RID: 5685
			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement[]
			{
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementSemantic.Position),
				new VertexElement(12, VertexElementFormat.Vector2, VertexElementSemantic.TextureCoordinate),
				new VertexElement(20, VertexElementFormat.NormalizedByte4, VertexElementSemantic.Color)
			});
		}
	}
}
