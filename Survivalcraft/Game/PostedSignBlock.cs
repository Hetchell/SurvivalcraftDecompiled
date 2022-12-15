using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000CA RID: 202
	public abstract class PostedSignBlock : SignBlock, IElectricElementBlock, IPaintableBlock
	{
		// Token: 0x060003F1 RID: 1009 RVA: 0x0001550C File Offset: 0x0001370C
		public PostedSignBlock(string modelName, int coloredTextureSlot, int attachedSignBlockIndex)
		{
			this.m_modelName = modelName;
			this.m_coloredTextureSlot = coloredTextureSlot;
			this.m_attachedSignBlockIndex = attachedSignBlockIndex;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00015598 File Offset: 0x00013798
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>(this.m_modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Sign", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Surface", true).ParentBone);
			for (int i = 0; i < 16; i++)
			{
				bool hanging = PostedSignBlock.GetHanging(i);
				Matrix matrix = Matrix.CreateRotationY((float)PostedSignBlock.GetDirection(i) * 3.1415927f / 4f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f);
				if (hanging)
				{
					matrix *= Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, 1f, 0f);
				}
				this.m_directions[i] = matrix.Forward;
				BlockMesh blockMesh = new BlockMesh();
				blockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * matrix, false, hanging, false, false, Color.White);
				BlockMesh blockMesh2 = new BlockMesh();
				blockMesh2.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform2 * matrix, false, hanging, false, false, Color.White);
				this.m_blockMeshes[i] = new BlockMesh();
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_blockMeshes[i].AppendBlockMesh(blockMesh2);
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(this.m_blockMeshes[i]);
				this.m_blockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_collisionBoxes[i] = new BoundingBox[2];
				this.m_collisionBoxes[i][0] = blockMesh.CalculateBoundingBox();
				this.m_collisionBoxes[i][1] = blockMesh2.CalculateBoundingBox();
				this.m_surfaceMeshes[i] = new BlockMesh();
				this.m_surfaceMeshes[i].AppendModelMeshPart(model.FindMesh("Surface", true).MeshParts[0], boneAbsoluteTransform3 * matrix, false, hanging, false, false, Color.White);
				this.m_surfaceNormals[i] = -matrix.Forward;
				if (hanging)
				{
					for (int j = 0; j < this.m_surfaceMeshes[i].Vertices.Count; j++)
					{
						Vector2 textureCoordinates = this.m_surfaceMeshes[i].Vertices.Array[j].TextureCoordinates;
						textureCoordinates.Y = 1f - textureCoordinates.Y;
						this.m_surfaceMeshes[i].Vertices.Array[j].TextureCoordinates = textureCoordinates;
					}
				}
			}
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Sign", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Post", true).MeshParts[0], boneAbsoluteTransform2 * Matrix.CreateTranslation(0f, -0.6f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.AppendBlockMesh(this.m_standaloneBlockMesh);
			this.m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			base.Initialize();
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00015A04 File Offset: 0x00013C04
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00015A2C File Offset: 0x00013C2C
		public override string GetCategory(int value)
		{
			if (PostedSignBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00015A65 File Offset: 0x00013C65
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PostedSignBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, PostedSignBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00015A78 File Offset: 0x00013C78
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(oldValue));
			int data = PostedSignBlock.SetColor(0, color);
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data),
				Count = 1
			});
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00015ACC File Offset: 0x00013CCC
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.DefaultTextureSlot);
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00015B28 File Offset: 0x00013D28
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int variant = PostedSignBlock.GetVariant(data);
			int? color = PostedSignBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[variant], SubsystemPalette.GetColor(generator, color), null, geometry.SubsetOpaque);
			}
			else
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshes[variant], Color.White, null, geometry.SubsetOpaque);
			}
			generator.GenerateWireVertices(value, x, y, z, PostedSignBlock.GetHanging(data) ? 5 : 4, 0.01f, Vector2.Zero, geometry.SubsetOpaque);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00015BD4 File Offset: 0x00013DD4
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), 1.25f * size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMesh, color, 1.25f * size, ref matrix, environmentData);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00015C36 File Offset: 0x00013E36
		public int? GetPaintColor(int value)
		{
			return PostedSignBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00015C44 File Offset: 0x00013E44
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, PostedSignBlock.SetColor(data, color));
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00015C68 File Offset: 0x00013E68
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int variant = PostedSignBlock.GetVariant(Terrain.ExtractData(value));
			return this.m_collisionBoxes[variant];
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00015C8C File Offset: 0x00013E8C
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int? color = PostedSignBlock.GetColor(Terrain.ExtractData(value));
			if (raycastResult.CellFace.Face < 4)
			{
				int data = AttachedSignBlock.SetFace(AttachedSignBlock.SetColor(0, color), raycastResult.CellFace.Face);
				return new BlockPlacementData
				{
					Value = Terrain.MakeBlockValue(this.m_attachedSignBlockIndex, 0, data),
					CellFace = raycastResult.CellFace
				};
			}
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = float.MinValue;
			int direction = 0;
			for (int i = 0; i < 8; i++)
			{
				float num2 = Vector3.Dot(forward, this.m_directions[i]);
				if (num2 > num)
				{
					num = num2;
					direction = i;
				}
			}
			int data2 = PostedSignBlock.SetHanging(PostedSignBlock.SetDirection(PostedSignBlock.SetColor(0, color), direction), raycastResult.CellFace.Face == 5);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, data2),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00015DA4 File Offset: 0x00013FA4
		public override BlockMesh GetSignSurfaceBlockMesh(int data)
		{
			return this.m_surfaceMeshes[PostedSignBlock.GetVariant(data)];
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00015DB3 File Offset: 0x00013FB3
		public override Vector3 GetSignSurfaceNormal(int data)
		{
			return this.m_surfaceNormals[PostedSignBlock.GetVariant(data)];
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00015DC8 File Offset: 0x00013FC8
		public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			return new SignElectricElement(subsystemElectricity, new CellFace(x, y, z, PostedSignBlock.GetHanging(data) ? 5 : 4));
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00015DF8 File Offset: 0x00013FF8
		public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (PostedSignBlock.GetHanging(Terrain.ExtractData(value)))
			{
				if (face != 5 || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
				{
					return null;
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
			else
			{
				if (face != 4 || SubsystemElectricity.GetConnectorDirection(face, 0, connectorFace) == null)
				{
					return null;
				}
				return new ElectricConnectorType?(ElectricConnectorType.Input);
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00015E61 File Offset: 0x00014061
		public int GetConnectionMask(int value)
		{
			return int.MaxValue;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00015E68 File Offset: 0x00014068
		public static int GetDirection(int data)
		{
			return data & 7;
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00015E6D File Offset: 0x0001406D
		public static int SetDirection(int data, int direction)
		{
			return (data & -8) | (direction & 7);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00015E77 File Offset: 0x00014077
		public static bool GetHanging(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x00015E7F File Offset: 0x0001407F
		public static int SetHanging(int data, bool hanging)
		{
			if (!hanging)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x00015E8C File Offset: 0x0001408C
		public static int GetVariant(int data)
		{
			return data & 15;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x00015E92 File Offset: 0x00014092
		public static int SetVariant(int data, int variant)
		{
			return (data & -16) | (variant & 15);
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00015EA0 File Offset: 0x000140A0
		public static int? GetColor(int data)
		{
			if ((data & 16) != 0)
			{
				return new int?(data >> 5 & 15);
			}
			return null;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00015EC8 File Offset: 0x000140C8
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -497) | 16 | (color.Value & 15) << 5;
			}
			return data & -497;
		}

		// Token: 0x040001BD RID: 445
		public string m_modelName;

		// Token: 0x040001BE RID: 446
		public int m_coloredTextureSlot;

		// Token: 0x040001BF RID: 447
		public int m_attachedSignBlockIndex;

		// Token: 0x040001C0 RID: 448
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		// Token: 0x040001C1 RID: 449
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x040001C2 RID: 450
		public BlockMesh[] m_blockMeshes = new BlockMesh[16];

		// Token: 0x040001C3 RID: 451
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[16];

		// Token: 0x040001C4 RID: 452
		public BlockMesh[] m_surfaceMeshes = new BlockMesh[16];

		// Token: 0x040001C5 RID: 453
		public Vector3[] m_surfaceNormals = new Vector3[16];

		// Token: 0x040001C6 RID: 454
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[16][];

		// Token: 0x040001C7 RID: 455
		public Vector3[] m_directions = new Vector3[16];
	}
}
