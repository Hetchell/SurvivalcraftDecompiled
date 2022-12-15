using System;
using System.Reflection;
using Engine;

namespace Game
{
	// Token: 0x0200006B RID: 107
	public abstract class FluidBlock : CubeBlock
	{
		// Token: 0x06000232 RID: 562 RVA: 0x0000D60C File Offset: 0x0000B80C
		public FluidBlock(int maxLevel)
		{
			this.MaxLevel = maxLevel;
			for (int i = 0; i < 16; i++)
			{
				float num = 0.875f * MathUtils.Saturate(1f - (float)i / (float)this.MaxLevel);
				this.m_heightByLevel[i] = num;
				this.m_boundingBoxesByLevel[i] = new BoundingBox[]
				{
					new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, num, 1f))
				};
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000D6B0 File Offset: 0x0000B8B0
		public override void Initialize()
		{
			base.Initialize();
			TypeInfo typeInfo = null;
			TypeInfo typeInfo2 = base.GetType().GetTypeInfo();
			while (typeInfo2 != null)
			{
				if (typeInfo2.BaseType == typeof(FluidBlock))
				{
					typeInfo = typeInfo2;
					break;
				}
				typeInfo2 = typeInfo2.BaseType.GetTypeInfo();
			}
			if (typeInfo == null)
			{
				throw new InvalidOperationException("Fluid type not found.");
			}
			this.m_theSameFluidsByIndex = new bool[BlocksManager.Blocks.Length];
			for (int i = 0; i < BlocksManager.Blocks.Length; i++)
			{
				Block block = BlocksManager.Blocks[i];
				this.m_theSameFluidsByIndex[i] = (block.GetType().GetTypeInfo() == typeInfo || block.GetType().GetTypeInfo().IsSubclassOf(typeInfo.AsType()));
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000D777 File Offset: 0x0000B977
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.m_boundingBoxesByLevel[FluidBlock.GetLevel(Terrain.ExtractData(value))];
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000D78B File Offset: 0x0000B98B
		public bool IsTheSameFluid(int contents)
		{
			return this.m_theSameFluidsByIndex[contents];
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000D795 File Offset: 0x0000B995
		public float GetLevelHeight(int level)
		{
			return this.m_heightByLevel[level];
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000D7A0 File Offset: 0x0000B9A0
		public void GenerateFluidTerrainVertices(BlockGeometryGenerator generator, int value, int x, int y, int z, Color sideColor, Color topColor, TerrainGeometrySubset[] subset)
		{
			int data = Terrain.ExtractData(value);
			if (FluidBlock.GetIsTop(data))
			{
				Terrain terrain = generator.Terrain;
				int cellValueFast = terrain.GetCellValueFast(x - 1, y, z - 1);
				int cellValueFast2 = terrain.GetCellValueFast(x, y, z - 1);
				int cellValueFast3 = terrain.GetCellValueFast(x + 1, y, z - 1);
				int cellValueFast4 = terrain.GetCellValueFast(x - 1, y, z);
				int cellValueFast5 = terrain.GetCellValueFast(x + 1, y, z);
				int cellValueFast6 = terrain.GetCellValueFast(x - 1, y, z + 1);
				int cellValueFast7 = terrain.GetCellValueFast(x, y, z + 1);
				int cellValueFast8 = terrain.GetCellValueFast(x + 1, y, z + 1);
				float h = this.CalculateNeighborHeight(cellValueFast);
				float num = this.CalculateNeighborHeight(cellValueFast2);
				float h2 = this.CalculateNeighborHeight(cellValueFast3);
				float num2 = this.CalculateNeighborHeight(cellValueFast4);
				float num3 = this.CalculateNeighborHeight(cellValueFast5);
				float h3 = this.CalculateNeighborHeight(cellValueFast6);
				float num4 = this.CalculateNeighborHeight(cellValueFast7);
				float h4 = this.CalculateNeighborHeight(cellValueFast8);
				float levelHeight = this.GetLevelHeight(FluidBlock.GetLevel(data));
				float height = FluidBlock.CalculateFluidVertexHeight(h, num, num2, levelHeight);
				float height2 = FluidBlock.CalculateFluidVertexHeight(num, h2, levelHeight, num3);
				float height3 = FluidBlock.CalculateFluidVertexHeight(levelHeight, num3, num4, h4);
				float height4 = FluidBlock.CalculateFluidVertexHeight(num2, levelHeight, h3, num4);
				float x2 = FluidBlock.ZeroSubst(num3, levelHeight) - FluidBlock.ZeroSubst(num2, levelHeight);
				float x3 = FluidBlock.ZeroSubst(num4, levelHeight) - FluidBlock.ZeroSubst(num, levelHeight);
				int overrideTopTextureSlot = this.DefaultTextureSlot - (int)MathUtils.Sign(x2) - 16 * (int)MathUtils.Sign(x3);
				generator.GenerateCubeVertices(this, value, x, y, z, height, height2, height3, height4, sideColor, topColor, topColor, topColor, topColor, overrideTopTextureSlot, subset);
				return;
			}
			generator.GenerateCubeVertices(this, value, x, y, z, sideColor, subset);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000D950 File Offset: 0x0000BB50
		public static float ZeroSubst(float v, float subst)
		{
			if (v != 0f)
			{
				return v;
			}
			return subst;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000D960 File Offset: 0x0000BB60
		public static float CalculateFluidVertexHeight(float h1, float h2, float h3, float h4)
		{
			float num = MathUtils.Max(h1, h2, h3, h4);
			if (num >= 1f)
			{
				return 1f;
			}
			if (h1 == 0.01f || h2 == 0.01f || h3 == 0.01f || h4 == 0.01f)
			{
				return 0f;
			}
			return num;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000D9AC File Offset: 0x0000BBAC
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.ReplaceData(Terrain.ReplaceContents(0, this.BlockIndex), 0),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000D9E9 File Offset: 0x0000BBE9
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face >= 4)
			{
				return this.DefaultTextureSlot;
			}
			return this.DefaultTextureSlot + 16;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000DA00 File Offset: 0x0000BC00
		public override bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			int contents = Terrain.ExtractContents(neighborValue);
			return !this.IsTheSameFluid(contents) && base.ShouldGenerateFace(subsystemTerrain, face, value, neighborValue);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000DA2C File Offset: 0x0000BC2C
		public float CalculateNeighborHeight(int value)
		{
			int num = Terrain.ExtractContents(value);
			if (this.IsTheSameFluid(num))
			{
				int data = Terrain.ExtractData(value);
				if (FluidBlock.GetIsTop(data))
				{
					return this.GetLevelHeight(FluidBlock.GetLevel(data));
				}
				return 1f;
			}
			else
			{
				if (num == 0)
				{
					return 0.01f;
				}
				return 0f;
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000DA79 File Offset: 0x0000BC79
		public override bool IsHeatBlocker(int value)
		{
			return true;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000DA7C File Offset: 0x0000BC7C
		public static int GetLevel(int data)
		{
			return data & 15;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000DA82 File Offset: 0x0000BC82
		public static int SetLevel(int data, int level)
		{
			return (data & -16) | (level & 15);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000DA8D File Offset: 0x0000BC8D
		public static bool GetIsTop(int data)
		{
			return (data & 16) != 0;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000DA96 File Offset: 0x0000BC96
		public static int SetIsTop(int data, bool isTop)
		{
			if (!isTop)
			{
				return data & -17;
			}
			return data | 16;
		}

		// Token: 0x04000106 RID: 262
		public float[] m_heightByLevel = new float[16];

		// Token: 0x04000107 RID: 263
		public BoundingBox[][] m_boundingBoxesByLevel = new BoundingBox[16][];

		// Token: 0x04000108 RID: 264
		public bool[] m_theSameFluidsByIndex;

		// Token: 0x04000109 RID: 265
		public readonly int MaxLevel;
	}
}
