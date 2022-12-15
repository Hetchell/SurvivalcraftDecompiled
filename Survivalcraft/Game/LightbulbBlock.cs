using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000A1 RID: 161
	public class LightbulbBlock : MountedElectricElementBlock, IPaintableBlock
	{
		// Token: 0x0600031D RID: 797 RVA: 0x00011FB8 File Offset: 0x000101B8
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Lightbulbs");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Top", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sides", true).ParentBone);
			for (int i = 0; i < 6; i++)
			{
				Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX(3.1415927f) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(1.5707964f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY((float)i * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				this.m_bulbBlockMeshes[i] = new BlockMesh();
				this.m_bulbBlockMeshes[i].AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m, false, false, false, false, Color.White);
				this.m_bulbBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.1875f, 0.25f, 0f), -1);
				this.m_bulbBlockMeshesLit[i] = new BlockMesh();
				this.m_bulbBlockMeshesLit[i].AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m, true, false, false, false, new Color(255, 255, 230));
				this.m_bulbBlockMeshesLit[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f), -1);
				this.m_sidesBlockMeshes[i] = new BlockMesh();
				this.m_sidesBlockMeshes[i].AppendModelMeshPart(model.FindMesh("Sides", true).MeshParts[0], boneAbsoluteTransform2 * m, false, false, true, false, Color.White);
				this.m_sidesBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[]
				{
					this.m_sidesBlockMeshes[i].CalculateBoundingBox()
				};
			}
			Matrix m2 = Matrix.CreateRotationY(-1.5707964f) * Matrix.CreateRotationZ(1.5707964f);
			this.m_standaloneBulbBlockMesh.AppendModelMeshPart(model.FindMesh("Top", true).MeshParts[0], boneAbsoluteTransform * m2, false, false, true, false, Color.White);
			this.m_standaloneBulbBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.1875f, 0.25f, 0f), -1);
			this.m_standaloneSidesBlockMesh.AppendModelMeshPart(model.FindMesh("Sides", true).MeshParts[0], boneAbsoluteTransform2 * m2, false, false, true, false, Color.White);
			this.m_standaloneSidesBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0.1875f, 0f), -1);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x000122EA File Offset: 0x000104EA
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(139);
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x000122F4 File Offset: 0x000104F4
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = LightbulbBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, LanguageControl.Get(LightbulbBlock.fName, 1));
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00012320 File Offset: 0x00010520
		public override string GetCategory(int value)
		{
			if (LightbulbBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00012359 File Offset: 0x00010559
		public override int GetFace(int value)
		{
			return LightbulbBlock.GetMountingFace(Terrain.ExtractData(value));
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00012366 File Offset: 0x00010566
		public override int GetEmittedLightAmount(int value)
		{
			return LightbulbBlock.GetLightIntensity(Terrain.ExtractData(value));
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00012374 File Offset: 0x00010574
		public override int GetShadowStrength(int value)
		{
			int lightIntensity = LightbulbBlock.GetLightIntensity(Terrain.ExtractData(value));
			return this.DefaultShadowStrength - 10 * lightIntensity;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00012398 File Offset: 0x00010598
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x06000325 RID: 805 RVA: 0x000123E8 File Offset: 0x000105E8
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int? color = LightbulbBlock.GetColor(Terrain.ExtractData(oldValue));
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(139, 0, LightbulbBlock.SetColor(0, color)),
				Count = 1
			});
			showDebris = true;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00012438 File Offset: 0x00010638
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int mountingFace = LightbulbBlock.GetMountingFace(Terrain.ExtractData(value));
			if (mountingFace >= this.m_collisionBoxes.Length)
			{
				return null;
			}
			return this.m_collisionBoxes[mountingFace];
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00012468 File Offset: 0x00010668
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int mountingFace = LightbulbBlock.GetMountingFace(data);
			int lightIntensity = LightbulbBlock.GetLightIntensity(data);
			int? color = LightbulbBlock.GetColor(data);
			Color color2 = (color != null) ? SubsystemPalette.GetColor(generator, color) : this.m_copperColor;
			if (mountingFace < this.m_bulbBlockMeshes.Length)
			{
				if (lightIntensity <= 0)
				{
					generator.GenerateMeshVertices(this, x, y, z, this.m_bulbBlockMeshes[mountingFace], Color.White, null, geometry.SubsetAlphaTest);
				}
				else
				{
					byte r = (byte)(195 + lightIntensity * 4);
					byte g = (byte)(180 + lightIntensity * 5);
					byte b = (byte)(165 + lightIntensity * 6);
					generator.GenerateMeshVertices(this, x, y, z, this.m_bulbBlockMeshesLit[mountingFace], new Color(r, g, b), null, geometry.SubsetOpaque);
				}
				generator.GenerateMeshVertices(this, x, y, z, this.m_sidesBlockMeshes[mountingFace], color2, null, geometry.SubsetOpaque);
				generator.GenerateWireVertices(value, x, y, z, mountingFace, 0.875f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00012580 File Offset: 0x00010780
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = LightbulbBlock.GetColor(Terrain.ExtractData(value));
			Color c = (color2 != null) ? SubsystemPalette.GetColor(environmentData, color2) : this.m_copperColor;
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneSidesBlockMesh, color * c, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBulbBlockMesh, color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x000125EB File Offset: 0x000107EB
		public int? GetPaintColor(int value)
		{
			return LightbulbBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x0600032A RID: 810 RVA: 0x000125F8 File Offset: 0x000107F8
		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, LightbulbBlock.SetColor(data, color));
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00012619 File Offset: 0x00010819
		public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new LightBulbElectricElement(subsystemElectricity, new CellFace(x, y, z, this.GetFace(value)), value);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00012634 File Offset: 0x00010834
		public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = this.GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace) != null)
			{
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			return null;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00012670 File Offset: 0x00010870
		public static int GetMountingFace(int data)
		{
			return data & 7;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00012675 File Offset: 0x00010875
		public static int SetMountingFace(int data, int face)
		{
			return (data & -8) | (face & 7);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001267F File Offset: 0x0001087F
		public static int GetLightIntensity(int data)
		{
			return data >> 3 & 15;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00012687 File Offset: 0x00010887
		public static int SetLightIntensity(int data, int intensity)
		{
			return (data & -121) | (intensity & 15) << 3;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00012694 File Offset: 0x00010894
		public static int? GetColor(int data)
		{
			if ((data & 128) != 0)
			{
				return new int?(data >> 8 & 15);
			}
			return null;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000126BF File Offset: 0x000108BF
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -3969) | 128 | (color.Value & 15) << 8;
			}
			return data & -3969;
		}

		// Token: 0x0400016C RID: 364
		public const int Index = 139;

		// Token: 0x0400016D RID: 365
		public BlockMesh m_standaloneBulbBlockMesh = new BlockMesh();

		// Token: 0x0400016E RID: 366
		public BlockMesh m_standaloneSidesBlockMesh = new BlockMesh();

		// Token: 0x0400016F RID: 367
		public BlockMesh[] m_bulbBlockMeshes = new BlockMesh[6];

		// Token: 0x04000170 RID: 368
		public BlockMesh[] m_bulbBlockMeshesLit = new BlockMesh[6];

		// Token: 0x04000171 RID: 369
		public BlockMesh[] m_sidesBlockMeshes = new BlockMesh[6];

		// Token: 0x04000172 RID: 370
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[6][];

		// Token: 0x04000173 RID: 371
		public new static string fName = "LightbulbBlock";

		// Token: 0x04000174 RID: 372
		public Color m_copperColor = new Color(118, 56, 32);
	}
}
