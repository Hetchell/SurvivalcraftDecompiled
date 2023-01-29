using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000222 RID: 546
	public abstract class Block
	{
		// Token: 0x060010B6 RID: 4278 RVA: 0x0007F0DA File Offset: 0x0007D2DA
		public virtual void Initialize()
		{
			if (this.Durability < -1 || this.Durability > 65535)
			{
				throw new InvalidOperationException(string.Format(LanguageControl.Get(Block.fName, 1), this.DefaultDisplayName));
			}
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x0007F110 File Offset: 0x0007D310
		public virtual string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int num = Terrain.ExtractData(value);
			string block = LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "DisplayName");
			if (string.IsNullOrEmpty(block))
			{
				return this.DefaultDisplayName;
			}
			return block;
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0007F15C File Offset: 0x0007D35C
		public virtual string GetDescription(int value)
		{
			int num = Terrain.ExtractData(value);
			string block = LanguageControl.GetBlock(string.Format("{0}:{1}", base.GetType().Name, num), "Description");
			if (string.IsNullOrEmpty(block))
			{
				return this.DefaultDescription;
			}
			return block;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0007F1A6 File Offset: 0x0007D3A6
		public virtual string GetCategory(int value)
		{
			return LanguageControl.Get("BlocksManager", this.DefaultCategory);
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x0007F1B8 File Offset: 0x0007D3B8
		public virtual IEnumerable<int> GetCreativeValues()
		{
			if (this.DefaultCreativeData >= 0)
			{
				yield return Terrain.ReplaceContents(Terrain.ReplaceData(0, this.DefaultCreativeData), this.BlockIndex);
			}
			yield break;
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0007F1C8 File Offset: 0x0007D3C8
		public virtual bool IsInteractive(SubsystemTerrain subsystemTerrain, int value)
		{
			return this.DefaultIsInteractive;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x0007F1D0 File Offset: 0x0007D3D0
		public virtual IEnumerable<CraftingRecipe> GetProceduralCraftingRecipes()
		{
			yield break;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0007F1D9 File Offset: 0x0007D3D9
		public virtual CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel, float playerLevel)
		{
			return null;
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0007F1DC File Offset: 0x0007D3DC
		public virtual bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return this.IsTransparent;
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x0007F1E4 File Offset: 0x0007D3E4
		public virtual bool ShouldGenerateFace(SubsystemTerrain subsystemTerrain, int face, int value, int neighborValue)
		{
			int num = Terrain.ExtractContents(neighborValue);
			return BlocksManager.Blocks[num].IsFaceTransparent(subsystemTerrain, CellFace.OppositeFace(face), neighborValue);
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0007F20E File Offset: 0x0007D40E
		public virtual int GetShadowStrength(int value)
		{
			return this.DefaultShadowStrength;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x0007F216 File Offset: 0x0007D416
		public virtual int GetFaceTextureSlot(int face, int value)
		{
			return this.DefaultTextureSlot;
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x0007F21E File Offset: 0x0007D41E
		public virtual string GetSoundMaterialName(SubsystemTerrain subsystemTerrain, int value)
		{
			return this.DefaultSoundMaterialName;
		}

		// Token: 0x060010C3 RID: 4291
		public abstract void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z);

		// Token: 0x060010C4 RID: 4292
		public abstract void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData);

		// Token: 0x060010C5 RID: 4293 RVA: 0x0007F228 File Offset: 0x0007D428
		public virtual BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0007F254 File Offset: 0x0007D454
		public virtual BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = 0,
				CellFace = raycastResult.CellFace
			};
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0007F280 File Offset: 0x0007D480
		public virtual void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = (this.DestructionDebrisScale > 0f);
			if (toolLevel < this.RequiredToolLevel)
			{
				return;
			}
			if (this.DefaultDropContent != 0)
			{
				int num = (int)this.DefaultDropCount;
				if (this.Random.Bool(this.DefaultDropCount - (float)num))
				{
					num++;
				}
				for (int i = 0; i < num; i++)
				{
					BlockDropValue item = new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(this.DefaultDropContent),
						Count = 1
					};
					dropValues.Add(item);
				}
			}
			int num2 = (int)this.DefaultExperienceCount;
			if (this.Random.Bool(this.DefaultExperienceCount - (float)num2))
			{
				num2++;
			}
			for (int j = 0; j < num2; j++)
			{
				BlockDropValue item = new BlockDropValue
				{
					Value = Terrain.MakeBlockValue(248),
					Count = 1
				};
				dropValues.Add(item);
			}
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0007F369 File Offset: 0x0007D569
		public virtual int GetDamage(int value)
		{
			return Terrain.ExtractData(value) >> 4 & 4095;
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0007F37C File Offset: 0x0007D57C
		public virtual int SetDamage(int value, int damage)
		{
			int num = Terrain.ExtractData(value);
			num &= 15;
			num |= MathUtils.Clamp(damage, 0, 4095) << 4;
			return Terrain.ReplaceData(value, num);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0007F3AD File Offset: 0x0007D5AD
		public virtual int GetDamageDestructionValue(int value)
		{
			return 0;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0007F3B0 File Offset: 0x0007D5B0
		public virtual int GetRotPeriod(int value)
		{
			return this.DefaultRotPeriod;
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0007F3B8 File Offset: 0x0007D5B8
		public virtual float GetSicknessProbability(int value)
		{
			return this.DefaultSicknessProbability;
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0007F3C0 File Offset: 0x0007D5C0
		public virtual float GetMeleePower(int value)
		{
			return this.DefaultMeleePower;
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0007F3C8 File Offset: 0x0007D5C8
		public virtual float GetMeleeHitProbability(int value)
		{
			return this.DefaultMeleeHitProbability;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0007F3D0 File Offset: 0x0007D5D0
		public virtual float GetProjectilePower(int value)
		{
			return this.DefaultProjectilePower;
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0007F3D8 File Offset: 0x0007D5D8
		public virtual float GetHeat(int value)
		{
			return this.DefaultHeat;
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0007F3E0 File Offset: 0x0007D5E0
		public virtual float GetExplosionPressure(int value)
		{
			return this.DefaultExplosionPressure;
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0007F3E8 File Offset: 0x0007D5E8
		public virtual bool GetExplosionIncendiary(int value)
		{
			return this.DefaultExplosionIncendiary;
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0007F3F0 File Offset: 0x0007D5F0
		public virtual Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconBlockOffset;
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0007F3F8 File Offset: 0x0007D5F8
		public virtual Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconViewOffset;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0007F400 File Offset: 0x0007D600
		public virtual float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
		{
			return this.DefaultIconViewScale;
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0007F408 File Offset: 0x0007D608
		public virtual BlockDebrisParticleSystem CreateDebrisParticleSystem(SubsystemTerrain subsystemTerrain, Vector3 position, int value, float strength)
		{
			return new BlockDebrisParticleSystem(subsystemTerrain, position, strength, this.DestructionDebrisScale, Color.White, this.GetFaceTextureSlot(4, value));
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0007F426 File Offset: 0x0007D626
		public virtual BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return Block.m_defaultCollisionBoxes;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0007F42D File Offset: 0x0007D62D
		public virtual BoundingBox[] GetCustomInteractionBoxes(SubsystemTerrain terrain, int value)
		{
			return this.GetCustomCollisionBoxes(terrain, value);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0007F437 File Offset: 0x0007D637
		public virtual int GetEmittedLightAmount(int value)
		{
			return this.DefaultEmittedLightAmount;
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0007F43F File Offset: 0x0007D63F
		public virtual float GetNutritionalValue(int value)
		{
			return this.DefaultNutritionalValue;
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x0007F447 File Offset: 0x0007D647
		public virtual bool ShouldAvoid(int value)
		{
			return false;
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x0007F44A File Offset: 0x0007D64A
		public virtual bool IsSwapAnimationNeeded(int oldValue, int newValue)
		{
			return true;
		}

		public void Setter_m_defaultCollisionBoxesBackToOriginal()
		{
			Block.m_defaultCollisionBoxes = new BoundingBox[]
					{
						 new BoundingBox(Vector3.Zero, Vector3.One)
					};
		}

        // Token: 0x060010DD RID: 4317 RVA: 0x0007F44D File Offset: 0x0007D64D
        public virtual bool IsHeatBlocker(int value)
		{
			return this.IsCollidable;
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0007F458 File Offset: 0x0007D658
		public float? Raycast(Ray3 ray, SubsystemTerrain subsystemTerrain, int value, bool useInteractionBoxes, out int nearestBoxIndex, out BoundingBox nearestBox)
		{
			float? result = null;
			nearestBoxIndex = 0;
			nearestBox = default(BoundingBox);
			BoundingBox[] array = useInteractionBoxes ? this.GetCustomInteractionBoxes(subsystemTerrain, value) : this.GetCustomCollisionBoxes(subsystemTerrain, value);
			for (int i = 0; i < array.Length; i++)
			{
				float? num = ray.Intersection(array[i]);
				if (num != null && (result == null || num.Value < result.Value))
				{
					nearestBoxIndex = i;
					result = num;
				}
			}
			nearestBox = array[nearestBoxIndex];
			return result;
		}

		// Token: 0x04000AF6 RID: 2806
		public int BlockIndex;

		// Token: 0x04000AF7 RID: 2807
		public string DefaultDisplayName = string.Empty;

		// Token: 0x04000AF8 RID: 2808
		public string DefaultDescription = string.Empty;

		// Token: 0x04000AF9 RID: 2809
		public string DefaultCategory = string.Empty;

		// Token: 0x04000AFA RID: 2810
		public int DisplayOrder;

		// Token: 0x04000AFB RID: 2811
		public Vector3 DefaultIconBlockOffset = Vector3.Zero;

		// Token: 0x04000AFC RID: 2812
		public Vector3 DefaultIconViewOffset = new Vector3(1f);

		// Token: 0x04000AFD RID: 2813
		public float DefaultIconViewScale = 1f;

		// Token: 0x04000AFE RID: 2814
		public float FirstPersonScale = 1f;

		// Token: 0x04000AFF RID: 2815
		public Vector3 FirstPersonOffset = Vector3.Zero;

		// Token: 0x04000B00 RID: 2816
		public Vector3 FirstPersonRotation = Vector3.Zero;

		// Token: 0x04000B01 RID: 2817
		public float InHandScale = 1f;

		// Token: 0x04000B02 RID: 2818
		public Vector3 InHandOffset = Vector3.Zero;

		// Token: 0x04000B03 RID: 2819
		public Vector3 InHandRotation = Vector3.Zero;

		// Token: 0x04000B04 RID: 2820
		public string Behaviors = string.Empty;

		// Token: 0x04000B05 RID: 2821
		public string CraftingId = string.Empty;

		// Token: 0x04000B06 RID: 2822
		public int DefaultCreativeData;

		// Token: 0x04000B07 RID: 2823
		public bool IsCollidable = true;

		// Token: 0x04000B08 RID: 2824
		public bool IsPlaceable = true;

		// Token: 0x04000B09 RID: 2825
		public bool IsDiggingTransparent;

		// Token: 0x04000B0A RID: 2826
		public bool IsPlacementTransparent;

		// Token: 0x04000B0B RID: 2827
		public bool DefaultIsInteractive;

		// Token: 0x04000B0C RID: 2828
		public bool IsEditable;

		// Token: 0x04000B0D RID: 2829
		public bool IsNonDuplicable;

		// Token: 0x04000B0E RID: 2830
		public bool IsGatherable;

		// Token: 0x04000B0F RID: 2831
		public bool HasCollisionBehavior;

		// Token: 0x04000B10 RID: 2832
		public bool KillsWhenStuck;

		// Token: 0x04000B11 RID: 2833
		public bool IsFluidBlocker = true;

		// Token: 0x04000B12 RID: 2834
		public bool IsTransparent;

		// Token: 0x04000B13 RID: 2835
		public int DefaultShadowStrength;

		// Token: 0x04000B14 RID: 2836
		public int LightAttenuation;

		// Token: 0x04000B15 RID: 2837
		public int DefaultEmittedLightAmount;

		// Token: 0x04000B16 RID: 2838
		public float ObjectShadowStrength;

		// Token: 0x04000B17 RID: 2839
		public int DefaultDropContent;

		// Token: 0x04000B18 RID: 2840
		public float DefaultDropCount = 1f;

		// Token: 0x04000B19 RID: 2841
		public float DefaultExperienceCount;

		// Token: 0x04000B1A RID: 2842
		public int RequiredToolLevel;

		// Token: 0x04000B1B RID: 2843
		public int MaxStacking = 40;

		// Token: 0x04000B1C RID: 2844
		public float SleepSuitability;

		// Token: 0x04000B1D RID: 2845
		public float FrictionFactor = 1f;

		// Token: 0x04000B1E RID: 2846
		public float Density = 4f;

		// Token: 0x04000B1F RID: 2847
		public bool NoAutoJump;

		// Token: 0x04000B20 RID: 2848
		public bool NoSmoothRise;

		// Token: 0x04000B21 RID: 2849
		public int DefaultTextureSlot;

		// Token: 0x04000B22 RID: 2850
		public float DestructionDebrisScale = 1f;

		// Token: 0x04000B23 RID: 2851
		public float FuelHeatLevel;

		// Token: 0x04000B24 RID: 2852
		public float FuelFireDuration;

		// Token: 0x04000B25 RID: 2853
		public string DefaultSoundMaterialName;

		// Token: 0x04000B26 RID: 2854
		public float ShovelPower = 1f;

		// Token: 0x04000B27 RID: 2855
		public float QuarryPower = 1f;

		// Token: 0x04000B28 RID: 2856
		public float HackPower = 1f;

		// Token: 0x04000B29 RID: 2857
		public float DefaultMeleePower = 1f;

		// Token: 0x04000B2A RID: 2858
		public float DefaultMeleeHitProbability = 0.66f;

		// Token: 0x04000B2B RID: 2859
		public float DefaultProjectilePower = 1f;

		// Token: 0x04000B2C RID: 2860
		public int ToolLevel;

		// Token: 0x04000B2D RID: 2861
		public int PlayerLevelRequired = 1;

		// Token: 0x04000B2E RID: 2862
		public int Durability = -1;

		// Token: 0x04000B2F RID: 2863
		public BlockDigMethod DigMethod;

		// Token: 0x04000B30 RID: 2864
		public float DigResilience = 1f;

		// Token: 0x04000B31 RID: 2865
		public float ProjectileResilience = 1f;

		// Token: 0x04000B32 RID: 2866
		public bool IsAimable;

		// Token: 0x04000B33 RID: 2867
		public bool IsStickable;

		// Token: 0x04000B34 RID: 2868
		public bool AlignToVelocity;

		// Token: 0x04000B35 RID: 2869
		public float ProjectileSpeed = 15f;

		// Token: 0x04000B36 RID: 2870
		public float ProjectileDamping = 0.8f;

		// Token: 0x04000B37 RID: 2871
		public float ProjectileTipOffset;

		// Token: 0x04000B38 RID: 2872
		public bool DisintegratesOnHit;

		// Token: 0x04000B39 RID: 2873
		public float ProjectileStickProbability;

		// Token: 0x04000B3A RID: 2874
		public float DefaultHeat;

		// Token: 0x04000B3B RID: 2875
		public float FireDuration;

		// Token: 0x04000B3C RID: 2876
		public float ExplosionResilience;

		// Token: 0x04000B3D RID: 2877
		public float DefaultExplosionPressure;

		// Token: 0x04000B3E RID: 2878
		public bool DefaultExplosionIncendiary;

		// Token: 0x04000B3F RID: 2879
		public bool IsExplosionTransparent;

		// Token: 0x04000B40 RID: 2880
		public float DefaultNutritionalValue;

		// Token: 0x04000B41 RID: 2881
		public FoodType FoodType;

		// Token: 0x04000B42 RID: 2882
		public int DefaultRotPeriod;

		// Token: 0x04000B43 RID: 2883
		public float DefaultSicknessProbability;

		// Token: 0x04000B44 RID: 2884
		public static string fName = "Block";

		// Token: 0x04000B45 RID: 2885
		protected Game.Random Random = new Game.Random();

		// Token: 0x04000B46 RID: 2886
		public static BoundingBox[] m_defaultCollisionBoxes = new BoundingBox[]
		{
			new BoundingBox(Vector3.Zero, Vector3.One)
		};
	}
}
