using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000019 RID: 25
	public abstract class BasePumpkinBlock : Block
	{
		// Token: 0x060000BE RID: 190 RVA: 0x00006731 File Offset: 0x00004931
		public BasePumpkinBlock(bool isRotten)
		{
			this.m_isRotten = isRotten;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006764 File Offset: 0x00004964
		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Pumpkins");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Pumpkin", true).ParentBone);
			for (int i = 0; i < 8; i++)
			{
				float num = MathUtils.Lerp(0.2f, 1f, (float)i / 7f);
				float num2 = MathUtils.Min(0.3f * num, 0.7f * (1f - num));
				Color color;
				if (this.m_isRotten)
				{
					color = Color.White;
				}
				else
				{
					color = Color.Lerp(new Color(0, 128, 128), new Color(80, 255, 255), (float)i / 7f);
					if (i == 7)
					{
						color.R = byte.MaxValue;
					}
				}
				this.m_blockMeshesBySize[i] = new BlockMesh();
				if (i >= 1)
				{
					this.m_blockMeshesBySize[i].AppendModelMeshPart(model.FindMesh("Pumpkin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(num) * Matrix.CreateTranslation(0.5f + num2, 0f, 0.5f + num2), false, false, false, false, color);
				}
				if (this.m_isRotten)
				{
					this.m_blockMeshesBySize[i].TransformTextureCoordinates(Matrix.CreateTranslation(-0.375f, 0.25f, 0f), -1);
				}
				this.m_standaloneBlockMeshesBySize[i] = new BlockMesh();
				this.m_standaloneBlockMeshesBySize[i].AppendModelMeshPart(model.FindMesh("Pumpkin", true).MeshParts[0], boneAbsoluteTransform * Matrix.CreateScale(num) * Matrix.CreateTranslation(0f, -0.23f, 0f), false, false, false, false, color);
				if (this.m_isRotten)
				{
					this.m_standaloneBlockMeshesBySize[i].TransformTextureCoordinates(Matrix.CreateTranslation(-0.375f, 0.25f, 0f), -1);
				}
			}
			for (int j = 0; j < 8; j++)
			{
				BoundingBox boundingBox = (this.m_blockMeshesBySize[j].Vertices.Count > 0) ? this.m_blockMeshesBySize[j].CalculateBoundingBox() : new BoundingBox(new Vector3(0.5f, 0f, 0.5f), new Vector3(0.5f, 0f, 0.5f));
				float num3 = boundingBox.Max.X - boundingBox.Min.X;
				if (num3 < 0.8f)
				{
					float num4 = (0.8f - num3) / 2f;
					boundingBox.Min.X = boundingBox.Min.X - num4;
					boundingBox.Min.Z = boundingBox.Min.Z - num4;
					boundingBox.Max.X = boundingBox.Max.X + num4;
					boundingBox.Max.Y = 0.4f;
					boundingBox.Max.Z = boundingBox.Max.Z + num4;
				}
				this.m_collisionBoxesBySize[j] = new BoundingBox[]
				{
					boundingBox
				};
			}
			base.Initialize();
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006A60 File Offset: 0x00004C60
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			return this.m_collisionBoxesBySize[size];
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006A84 File Offset: 0x00004C84
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			int size = BasePumpkinBlock.GetSize(data);
			bool isDead = BasePumpkinBlock.GetIsDead(data);
			if (size >= 1)
			{
				generator.GenerateMeshVertices(this, x, y, z, this.m_blockMeshesBySize[size], Color.White, null, geometry.SubsetOpaque);
			}
			if (size == 0)
			{
				generator.GenerateCrossfaceVertices(this, value, x, y, z, new Color(160, 160, 160), 11, geometry.SubsetAlphaTest);
				return;
			}
			if (size < 7 && !isDead)
			{
				generator.GenerateCrossfaceVertices(this, value, x, y, z, Color.White, 28, geometry.SubsetAlphaTest);
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006B24 File Offset: 0x00004D24
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int size2 = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, this.m_standaloneBlockMeshesBySize[size2], color, 2f * size, ref matrix, environmentData);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006B58 File Offset: 0x00004D58
		public override int GetShadowStrength(int value)
		{
			return BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006B65 File Offset: 0x00004D65
		public override float GetNutritionalValue(int value)
		{
			if (BasePumpkinBlock.GetSize(Terrain.ExtractData(value)) != 7)
			{
				return 0f;
			}
			return base.GetNutritionalValue(value);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006B84 File Offset: 0x00004D84
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(oldValue));
			if (size >= 1)
			{
				int value = this.SetDamage(Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), size)), this.GetDamage(oldValue));
				dropValues.Add(new BlockDropValue
				{
					Value = value,
					Count = 1
				});
			}
			showDebris = true;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006BEC File Offset: 0x00004DEC
		public override BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			float num = MathUtils.Lerp(0.2f, 1f, (float)size / 7f);
			Color color = (size == 7) ? Color.White : new Color(0, 128, 128);
			return new BlockDebrisParticleSystem(subsystemTerrain, position, 1.5f * strength, this.DestructionDebrisScale * num, color, this.DefaultTextureSlot);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006C58 File Offset: 0x00004E58
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int size = BasePumpkinBlock.GetSize(Terrain.ExtractData(value));
			if (this.m_isRotten)
			{
				if (size >= 7)
				{
					return "腐烂的南瓜";
				}
				return "腐烂未成熟的南瓜";
			}
			else
			{
				if (size >= 7)
				{
					return "南瓜";
				}
				return "未成熟的南瓜";
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006C98 File Offset: 0x00004E98
		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 1));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 3));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 5));
			yield return Terrain.MakeBlockValue(this.BlockIndex, 0, BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, true), 7));
			yield break;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006CA8 File Offset: 0x00004EA8
		public static int GetSize(int data)
		{
			return 7 - (data & 7);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006CAF File Offset: 0x00004EAF
		public static int SetSize(int data, int size)
		{
			return (data & -8) | 7 - (size & 7);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00006CBB File Offset: 0x00004EBB
		public static bool GetIsDead(int data)
		{
			return (data & 8) != 0;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00006CC3 File Offset: 0x00004EC3
		public static int SetIsDead(int data, bool isDead)
		{
			if (!isDead)
			{
				return data & -9;
			}
			return data | 8;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00006CD0 File Offset: 0x00004ED0
		public override int GetDamage(int value)
		{
			return (Terrain.ExtractData(value) & 16) >> 4;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00006CE0 File Offset: 0x00004EE0
		public override int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, (num & -17) | (damage & 1) << 4);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006D04 File Offset: 0x00004F04
		public override int GetDamageDestructionValue(int value)
		{
			if (this.m_isRotten)
			{
				return 0;
			}
			int data = Terrain.ExtractData(value);
			return this.SetDamage(Terrain.MakeBlockValue(244, 0, data), 0);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006D35 File Offset: 0x00004F35
		public override int GetRotPeriod(int value)
		{
			if (!BasePumpkinBlock.GetIsDead(Terrain.ExtractData(value)))
			{
				return 0;
			}
			return this.DefaultRotPeriod;
		}

		// Token: 0x0400006E RID: 110
		public BlockMesh[] m_blockMeshesBySize = new BlockMesh[8];

		// Token: 0x0400006F RID: 111
		public BlockMesh[] m_standaloneBlockMeshesBySize = new BlockMesh[8];

		// Token: 0x04000070 RID: 112
		public BoundingBox[][] m_collisionBoxesBySize = new BoundingBox[8][];

		// Token: 0x04000071 RID: 113
		public bool m_isRotten;
	}
}
