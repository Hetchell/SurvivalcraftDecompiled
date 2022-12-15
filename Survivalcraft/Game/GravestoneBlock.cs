using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200007A RID: 122
	public class GravestoneBlock : Block
	{
		// Token: 0x06000296 RID: 662 RVA: 0x0000F6AC File Offset: 0x0000D8AC
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Graves");
			for (int i = 0; i < 16; i++)
			{
				int variant = GravestoneBlock.GetVariant(i);
				float radians = (GravestoneBlock.GetRotation(i) == 0) ? 0f : 1.5707964f;
				string name = "Grave" + (variant % 4 + 1).ToString(CultureInfo.InvariantCulture);
				bool flag = variant >= 4;
				Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(name, true).ParentBone);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
				this.m_standaloneBlockMeshes[i] = new BlockMesh();
				this.m_standaloneBlockMeshes[i].AppendModelMeshPart(model.FindMesh(name, true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
				if (flag)
				{
					Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Plinth", true).ParentBone);
					this.m_blockMeshes[i].AppendModelMeshPart(model.FindMesh("Plinth", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateRotationY(radians) * Matrix.CreateTranslation(0.5f, 0f, 0.5f), false, false, false, false, Color.White);
					this.m_standaloneBlockMeshes[i].AppendModelMeshPart(model.FindMesh("Plinth", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
				}
				this.m_collisionBoxes[i] = new BoundingBox[]
				{
					this.m_blockMeshes[i].CalculateBoundingBox()
				};
			}
			base.Initialize();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000F8D0 File Offset: 0x0000DAD0
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshes.Length)
			{
				int num2 = Terrain.ExtractContents((y > 0) ? generator.Terrain.GetCellValueFast(x, y - 1, z) : 0);
				bool flag = BlocksManager.Blocks[num2].DigMethod != BlockDigMethod.Shovel;
				bool flag2 = num2 == 7 || num2 == 4 || num2 == 52;
				int num3 = (int)(MathUtils.Hash((uint)(x + 172 * y + 18271 * z)) & 65535U);
				Matrix value2 = Matrix.Identity;
				if (!flag)
				{
					float radians = 0.2f * ((float)(num3 % 16) / 7.5f - 1f);
					float radians2 = 0.1f * ((float)((num3 >> 4) % 16) / 7.5f - 1f);
					value2 = ((GravestoneBlock.GetRotation(num) != 0) ? (Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationZ(radians) * Matrix.CreateRotationY(radians2) * Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateTranslation(-0.5f, 0f, -0.5f) * Matrix.CreateRotationX(radians) * Matrix.CreateRotationY(radians2) * Matrix.CreateTranslation(0.5f, 0f, 0.5f)));
				}
				float f = flag ? 0f : MathUtils.Sqr((float)((num3 >> 8) % 16) / 15f);
				Color color = (!flag2) ? Color.Lerp(Color.White, new Color(255, 233, 199), f) : Color.Lerp(new Color(217, 206, 123), new Color(229, 206, 123), f);
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[num], color, new Matrix?(value2), geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000FAC0 File Offset: 0x0000DCC0
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_blockMeshes.Length)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshes[num], color, size, ref matrix, environmentData);
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000FAF4 File Offset: 0x0000DCF4
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num < this.m_collisionBoxes.Length)
			{
				return this.m_collisionBoxes[num];
			}
			return base.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000FB24 File Offset: 0x0000DD24
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = Terrain.ExtractData(value);
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = MathUtils.Abs(Vector3.Dot(forward, Vector3.UnitX));
			if (MathUtils.Abs(Vector3.Dot(forward, Vector3.UnitZ)) > num)
			{
				return new BlockPlacementData
				{
					Value = Terrain.MakeBlockValue(189, 0, GravestoneBlock.SetRotation(data, 0)),
					CellFace = raycastResult.CellFace
				};
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(189, 0, GravestoneBlock.SetRotation(data, 1)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000FBD9 File Offset: 0x0000DDD9
		public override IEnumerable<int> GetCreativeValues()
		{
			int num;
			for (int i = 0; i < 8; i = num)
			{
				int data = GravestoneBlock.SetVariant(0, i);
				yield return Terrain.MakeBlockValue(189, 0, data);
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(189, 0, Terrain.ExtractData(oldValue)),
				Count = 1
			});
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000FC25 File Offset: 0x0000DE25
		public static int GetRotation(int data)
		{
			return (data & 8) >> 3;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000FC2C File Offset: 0x0000DE2C
		public static int SetRotation(int data, int rotation)
		{
			return (data & -9) | (rotation << 3 & 8);
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000FC38 File Offset: 0x0000DE38
		public static int GetVariant(int data)
		{
			return data & 7;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000FC3D File Offset: 0x0000DE3D
		public static int SetVariant(int data, int variant)
		{
			return (data & -8) | (variant & 7);
		}

		// Token: 0x04000129 RID: 297
		public const int Index = 189;

		// Token: 0x0400012A RID: 298
		public BlockMesh[] m_standaloneBlockMeshes = new BlockMesh[16];

		// Token: 0x0400012B RID: 299
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x0400012C RID: 300
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];
	}
}
