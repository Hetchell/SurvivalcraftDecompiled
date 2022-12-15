using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020000FD RID: 253
	public abstract class StairsBlock : Block, IPaintableBlock
	{
		// Token: 0x060004D8 RID: 1240 RVA: 0x000199C0 File Offset: 0x00017BC0
		public StairsBlock(int coloredTextureSlot)
		{
			this.m_coloredTextureSlot = coloredTextureSlot;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00019A18 File Offset: 0x00017C18
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Stairs");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Stairs", true).ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StairsOuterCorner", true).ParentBone);
			Matrix boneAbsoluteTransform3 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("StairsInnerCorner", true).ParentBone);
			for (int i = 0; i < 24; i++)
			{
				int rotation = StairsBlock.GetRotation(i);
				bool isUpsideDown = StairsBlock.GetIsUpsideDown(i);
				StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(i);
				Matrix m = (!isUpsideDown) ? (Matrix.CreateRotationY((float)rotation * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationY((float)rotation * 3.1415927f / 2f) * Matrix.CreateTranslation(0.5f, -0.5f, 0.5f) * Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0f, 0.5f, 0f));
				BlockMesh blockMesh = new BlockMesh();
				switch (cornerType)
				{
				case StairsBlock.CornerType.None:
					blockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * m, false, isUpsideDown, false, false, Color.White);
					break;
				case StairsBlock.CornerType.OneQuarter:
					blockMesh.AppendModelMeshPart(model.FindMesh("StairsOuterCorner", true).MeshParts[0], boneAbsoluteTransform2 * m, false, isUpsideDown, false, false, Color.White);
					break;
				case StairsBlock.CornerType.ThreeQuarters:
					blockMesh.AppendModelMeshPart(model.FindMesh("StairsInnerCorner", true).MeshParts[0], boneAbsoluteTransform3 * m, false, isUpsideDown, false, false, Color.White);
					break;
				}
				float num = (float)(isUpsideDown ? rotation : (-(float)rotation));
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateRotationZ(num * 3.1415927f / 2f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), 16);
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateRotationZ((0f - num) * 3.1415927f / 2f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), 32);
				if (isUpsideDown)
				{
					blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-0.03125f, -0.03125f, 0f) * Matrix.CreateScale(1f, -1f, 1f) * Matrix.CreateTranslation(0.03125f, 0.03125f, 0f), -1);
				}
				this.m_coloredBlockMeshes[i] = new BlockMesh();
				this.m_coloredBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_coloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
				this.m_coloredBlockMeshes[i].GenerateSidesData();
				this.m_uncoloredBlockMeshes[i] = new BlockMesh();
				this.m_uncoloredBlockMeshes[i].AppendBlockMesh(blockMesh);
				this.m_uncoloredBlockMeshes[i].TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
				this.m_uncoloredBlockMeshes[i].GenerateSidesData();
			}
			this.m_standaloneUncoloredBlockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneUncoloredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.DefaultTextureSlot % 16) / 16f, (float)(this.DefaultTextureSlot / 16) / 16f, 0f), -1);
			this.m_standaloneColoredBlockMesh.AppendModelMeshPart(model.FindMesh("Stairs", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), false, false, false, false, Color.White);
			this.m_standaloneColoredBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(this.m_coloredTextureSlot % 16) / 16f, (float)(this.m_coloredTextureSlot / 16) / 16f, 0f), -1);
			this.m_collisionBoxes[0] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[1] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[2] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[3] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[4] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[5] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[6] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[7] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[8] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f))
			};
			this.m_collisionBoxes[9] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 0.5f))
			};
			this.m_collisionBoxes[10] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f))
			};
			this.m_collisionBoxes[11] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[12] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[13] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[14] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(0.5f, 0.5f, 1f))
			};
			this.m_collisionBoxes[15] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[16] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f))
			};
			this.m_collisionBoxes[17] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(0.5f, 1f, 1f))
			};
			this.m_collisionBoxes[18] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(0.5f, 1f, 0.5f))
			};
			this.m_collisionBoxes[19] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0.5f, 0.5f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0.5f, 0f), new Vector3(1f, 1f, 0.5f))
			};
			this.m_collisionBoxes[20] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f))
			};
			this.m_collisionBoxes[21] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(0.5f, 0.5f, 1f))
			};
			this.m_collisionBoxes[22] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0.5f, 0.5f, 0.5f))
			};
			this.m_collisionBoxes[23] = new BoundingBox[]
			{
				new BoundingBox(new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f)),
				new BoundingBox(new Vector3(0f, 0f, 0.5f), new Vector3(1f, 0.5f, 1f)),
				new BoundingBox(new Vector3(0.5f, 0f, 0f), new Vector3(1f, 0.5f, 0.5f))
			};
			base.Initialize();
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001AB89 File Offset: 0x00018D89
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(0, null));
			int num;
			for (int i = 0; i < 16; i = num)
			{
				yield return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(0, new int?(i)));
				num = i + 1;
			}
			yield break;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001AB9C File Offset: 0x00018D9C
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			int data = Terrain.ExtractData(value);
			bool isUpsideDown = StairsBlock.GetIsUpsideDown(data);
			if (face == 4)
			{
				return !isUpsideDown;
			}
			if (face == 5)
			{
				return isUpsideDown;
			}
			StairsBlock.CornerType cornerType = StairsBlock.GetCornerType(data);
			if (cornerType == StairsBlock.CornerType.None)
			{
				int rotation = StairsBlock.GetRotation(data);
				return face != (rotation + 2 & 3);
			}
			if (cornerType != StairsBlock.CornerType.OneQuarter)
			{
				int rotation2 = StairsBlock.GetRotation(data);
				return face != (rotation2 + 1 & 3) && face != (rotation2 + 2 & 3);
			}
			return true;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001AC0C File Offset: 0x00018E0C
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int? color = StairsBlock.GetColor(data);
			if (color != null)
			{
				generator.GenerateShadedMeshVertices(this, x, y, z, this.m_coloredBlockMeshes[StairsBlock.GetVariant(data)], SubsystemPalette.GetColor(generator, color), null, null, geometry.SubsetOpaque);
				return;
			}
			generator.GenerateShadedMeshVertices(this, x, y, z, this.m_uncoloredBlockMeshes[StairsBlock.GetVariant(data)], Color.White, null, null, geometry.SubsetOpaque);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001AC94 File Offset: 0x00018E94
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int rotation = 0;
			if (num == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 2;
			}
			else if (num2 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 3;
			}
			else if (num3 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 0;
			}
			else if (num4 == MathUtils.Max(num, num2, num3, num4))
			{
				rotation = 1;
			}
			bool isUpsideDown = raycastResult.CellFace.Face == 5;
			int data = Terrain.ExtractData(value);
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetIsUpsideDown(StairsBlock.SetRotation(data, rotation), isUpsideDown)),
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001AD94 File Offset: 0x00018F94
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int data = Terrain.ExtractData(value);
			return this.m_collisionBoxes[StairsBlock.GetVariant(data)];
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001ADB8 File Offset: 0x00018FB8
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? color2 = StairsBlock.GetColor(Terrain.ExtractData(value));
			if (color2 != null)
			{
				BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneColoredBlockMesh, color * SubsystemPalette.GetColor(environmentData, color2), size, ref matrix, environmentData);
				return;
			}
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneUncoloredBlockMesh, color, size, ref matrix, environmentData);
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0001AE10 File Offset: 0x00019010
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? color = StairsBlock.GetColor(Terrain.ExtractData(value));
			return SubsystemPalette.GetName(subsystemTerrain, color, base.GetDisplayName(subsystemTerrain, value));
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0001AE38 File Offset: 0x00019038
		public override string GetCategory(int value)
		{
			if (StairsBlock.GetColor(Terrain.ExtractData(value)) == null)
			{
				return base.GetCategory(value);
			}
			return LanguageControl.Get("BlocksManager", "Painted");
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0001AE74 File Offset: 0x00019074
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			int data = Terrain.ExtractData(oldValue);
			int data2 = StairsBlock.SetColor(0, StairsBlock.GetColor(data));
			int value = Terrain.MakeBlockValue(this.BlockIndex, 0, data2);
			dropValues.Add(new BlockDropValue
			{
				Value = value,
				Count = 1
			});
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0001AEC8 File Offset: 0x000190C8
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int? color = StairsBlock.GetColor(Terrain.ExtractData(value));
			if (color != null)
			{
				return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, SubsystemPalette.GetColor(subsystemTerrain, color), this.m_coloredTextureSlot);
			}
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, base.GetFaceTextureSlot(0, value));
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0001AF23 File Offset: 0x00019123
		public int? GetPaintColor(int value)
		{
			return StairsBlock.GetColor(Terrain.ExtractData(value));
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0001AF30 File Offset: 0x00019130
		public int Paint(SubsystemTerrain terrain, int value, int? color)
		{
			return Terrain.MakeBlockValue(this.BlockIndex, 0, StairsBlock.SetColor(Terrain.ExtractData(value), color));
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0001AF4A File Offset: 0x0001914A
		public static Point3 RotationToDirection(int rotation)
		{
			return CellFace.FaceToPoint3((rotation + 2) % 4);
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001AF56 File Offset: 0x00019156
		public static int GetRotation(int data)
		{
			return data & 3;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001AF5B File Offset: 0x0001915B
		public static int SetRotation(int data, int rotation)
		{
			return (data & -4) | (rotation & 3);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001AF65 File Offset: 0x00019165
		public static bool GetIsUpsideDown(int data)
		{
			return (data & 4) != 0;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001AF6D File Offset: 0x0001916D
		public static int SetIsUpsideDown(int data, bool isUpsideDown)
		{
			if (isUpsideDown)
			{
				return data | 4;
			}
			return data & -5;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001AF7A File Offset: 0x0001917A
		public static StairsBlock.CornerType GetCornerType(int data)
		{
			return (StairsBlock.CornerType)(data >> 3 & 3);
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001AF81 File Offset: 0x00019181
		public static int SetCornerType(int data, StairsBlock.CornerType cornerType)
		{
			return (data & -25) | (int)((int)(cornerType & (StairsBlock.CornerType)3) << 3);
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001AF90 File Offset: 0x00019190
		public static int? GetColor(int data)
		{
			if ((data & 32) != 0)
			{
				return new int?(data >> 6 & 15);
			}
			return null;
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001AFB8 File Offset: 0x000191B8
		public static int SetColor(int data, int? color)
		{
			if (color != null)
			{
				return (data & -993) | 32 | (color.Value & 15) << 6;
			}
			return data & -993;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001AFE2 File Offset: 0x000191E2
		public static int GetVariant(int data)
		{
			return data & 31;
		}

		// Token: 0x04000224 RID: 548
		public BlockMesh m_standaloneUncoloredBlockMesh = new BlockMesh();

		// Token: 0x04000225 RID: 549
		public BlockMesh m_standaloneColoredBlockMesh = new BlockMesh();

		// Token: 0x04000226 RID: 550
		public BlockMesh[] m_uncoloredBlockMeshes = new BlockMesh[24];

		// Token: 0x04000227 RID: 551
		public BlockMesh[] m_coloredBlockMeshes = new BlockMesh[24];

		// Token: 0x04000228 RID: 552
		public BoundingBox[][] m_collisionBoxes = new BoundingBox[24][];

		// Token: 0x04000229 RID: 553
		public int m_coloredTextureSlot;

		// Token: 0x020003E8 RID: 1000
		public enum CornerType
		{
			// Token: 0x040014A8 RID: 5288
			None,
			// Token: 0x040014A9 RID: 5289
			OneQuarter,
			// Token: 0x040014AA RID: 5290
			ThreeQuarters
		}
	}
}
