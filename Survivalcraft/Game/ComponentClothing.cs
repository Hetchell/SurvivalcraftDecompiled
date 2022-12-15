using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001CB RID: 459
	public class ComponentClothing : Component, IUpdateable, IInventory
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000BF1 RID: 3057 RVA: 0x0005AA67 File Offset: 0x00058C67
		public Texture2D InnerClothedTexture
		{
			get
			{
				return this.m_innerClothedTexture;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x0005AA6F File Offset: 0x00058C6F
		public Texture2D OuterClothedTexture
		{
			get
			{
				return this.m_outerClothedTexture;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000BF3 RID: 3059 RVA: 0x0005AA77 File Offset: 0x00058C77
		// (set) Token: 0x06000BF4 RID: 3060 RVA: 0x0005AA7F File Offset: 0x00058C7F
		public float Insulation { get; set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000BF5 RID: 3061 RVA: 0x0005AA88 File Offset: 0x00058C88
		// (set) Token: 0x06000BF6 RID: 3062 RVA: 0x0005AA90 File Offset: 0x00058C90
		public ClothingSlot LeastInsulatedSlot { get; set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000BF7 RID: 3063 RVA: 0x0005AA99 File Offset: 0x00058C99
		// (set) Token: 0x06000BF8 RID: 3064 RVA: 0x0005AAA1 File Offset: 0x00058CA1
		public float SteedMovementSpeedFactor { get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000BF9 RID: 3065 RVA: 0x0005AAAA File Offset: 0x00058CAA
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x0005AAAD File Offset: 0x00058CAD
		Project IInventory.Project
		{
			get
			{
				return base.Project;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000BFB RID: 3067 RVA: 0x0005AAB5 File Offset: 0x00058CB5
		public int SlotsCount
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x0005AAB8 File Offset: 0x00058CB8
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x0005AAC0 File Offset: 0x00058CC0
		public int VisibleSlotsCount
		{
			get
			{
				return this.SlotsCount;
			}
			set
			{
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0005AAC2 File Offset: 0x00058CC2
		// (set) Token: 0x06000BFF RID: 3071 RVA: 0x0005AAC5 File Offset: 0x00058CC5
		public int ActiveSlotIndex
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x0005AAC7 File Offset: 0x00058CC7
		public ReadOnlyList<int> GetClothes(ClothingSlot slot)
		{
			return new ReadOnlyList<int>(this.m_clothes[slot]);
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x0005AADC File Offset: 0x00058CDC
		public void SetClothes(ClothingSlot slot, IEnumerable<int> clothes)
		{
			if (!this.m_clothes[slot].SequenceEqual(clothes))
			{
				this.m_clothes[slot].Clear();
				this.m_clothes[slot].AddRange(clothes);
				this.m_clothedTexturesValid = false;
				float num = 0f;
				foreach (KeyValuePair<ClothingSlot, List<int>> keyValuePair in this.m_clothes)
				{
					foreach (int value in keyValuePair.Value)
					{
						ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
						num += clothingData.DensityModifier;
					}
				}
				float num2 = num - this.m_densityModifierApplied;
				this.m_densityModifierApplied += num2;
				this.m_componentBody.Density += num2;
				this.SteedMovementSpeedFactor = 1f;
				float num3 = 2f;
				float num4 = 0.2f;
				float num5 = 0.4f;
				float num6 = 2f;
				foreach (int value2 in this.GetClothes(ClothingSlot.Head))
				{
					ClothingData clothingData2 = ClothingBlock.GetClothingData(Terrain.ExtractData(value2));
					num3 += clothingData2.Insulation;
					this.SteedMovementSpeedFactor *= clothingData2.SteedMovementSpeedFactor;
				}
				foreach (int value3 in this.GetClothes(ClothingSlot.Torso))
				{
					ClothingData clothingData3 = ClothingBlock.GetClothingData(Terrain.ExtractData(value3));
					num4 += clothingData3.Insulation;
					this.SteedMovementSpeedFactor *= clothingData3.SteedMovementSpeedFactor;
				}
				foreach (int value4 in this.GetClothes(ClothingSlot.Legs))
				{
					ClothingData clothingData4 = ClothingBlock.GetClothingData(Terrain.ExtractData(value4));
					num5 += clothingData4.Insulation;
					this.SteedMovementSpeedFactor *= clothingData4.SteedMovementSpeedFactor;
				}
				foreach (int value5 in this.GetClothes(ClothingSlot.Feet))
				{
					ClothingData clothingData5 = ClothingBlock.GetClothingData(Terrain.ExtractData(value5));
					num6 += clothingData5.Insulation;
					this.SteedMovementSpeedFactor *= clothingData5.SteedMovementSpeedFactor;
				}
				this.Insulation = 1f / (1f / num3 + 1f / num4 + 1f / num5 + 1f / num6);
				float num7 = MathUtils.Min(num3, num4, num5, num6);
				if (num3 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Head;
					return;
				}
				if (num4 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Torso;
					return;
				}
				if (num5 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Legs;
					return;
				}
				if (num6 == num7)
				{
					this.LeastInsulatedSlot = ClothingSlot.Feet;
				}
			}
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x0005AE30 File Offset: 0x00059030
		public float ApplyArmorProtection(float attackPower)
		{
			float num = this.m_random.Float(0f, 1f);
			ClothingSlot slot = (num < 0.1f) ? ClothingSlot.Feet : ((num < 0.3f) ? ClothingSlot.Legs : ((num < 0.9f) ? ClothingSlot.Torso : ClothingSlot.Head));
			float num2 = (float)(((ClothingBlock)BlocksManager.Blocks[203]).Durability + 1);
			List<int> list = new List<int>(this.GetClothes(slot));
			for (int i = 0; i < list.Count; i++)
			{
				int value = list[i];
				ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
				float x = (num2 - (float)BlocksManager.Blocks[203].GetDamage(value)) / num2 * clothingData.Sturdiness;
				float num3 = MathUtils.Min(attackPower * MathUtils.Saturate(clothingData.ArmorProtection), x);
				if (num3 > 0f)
				{
					attackPower -= num3;
					if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative)
					{
						float x2 = num3 / clothingData.Sturdiness * num2 + 0.001f;
						int damageCount = (int)(MathUtils.Floor(x2) + (float)(this.m_random.Bool(MathUtils.Remainder(x2, 1f)) ? 1 : 0));
						list[i] = BlocksManager.DamageItem(value, damageCount);
					}
					if (!string.IsNullOrEmpty(clothingData.ImpactSoundsFolder))
					{
						this.m_subsystemAudio.PlayRandomSound(clothingData.ImpactSoundsFolder, 1f, this.m_random.Float(-0.3f, 0.3f), this.m_componentBody.Position, 4f, 0.15f);
					}
				}
			}
			int j = 0;
			while (j < list.Count)
			{
				if (Terrain.ExtractContents(list[j]) != 203)
				{
					list.RemoveAt(j);
					this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, this.m_componentBody.Position + this.m_componentBody.BoxSize / 2f, 1f, 1f, Color.White, 0));
				}
				else
				{
					j++;
				}
			}
			this.SetClothes(slot, list);
			return MathUtils.Max(attackPower, 0f);
		}

        // Token: 0x06000C03 RID: 3075 RVA: 0x0005B060 File Offset: 0x00059260
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_componentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.m_componentHumanModel = base.Entity.FindComponent<ComponentHumanModel>(true);
			this.m_componentBody = base.Entity.FindComponent<ComponentBody>(true);
			this.m_componentOuterClothingModel = base.Entity.FindComponent<ComponentOuterClothingModel>(true);
			this.m_componentVitalStats = base.Entity.FindComponent<ComponentVitalStats>(true);
			this.m_componentLocomotion = base.Entity.FindComponent<ComponentLocomotion>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>(true);
			this.SteedMovementSpeedFactor = 1f;
			this.Insulation = 0f;
			this.LeastInsulatedSlot = ClothingSlot.Feet;
			this.m_clothes[ClothingSlot.Head] = new List<int>();
			this.m_clothes[ClothingSlot.Torso] = new List<int>();
			this.m_clothes[ClothingSlot.Legs] = new List<int>();
			this.m_clothes[ClothingSlot.Feet] = new List<int>();
			ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("Clothes");
			this.SetClothes(ClothingSlot.Head, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Head")));
			this.SetClothes(ClothingSlot.Torso, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Torso")));
			this.SetClothes(ClothingSlot.Legs, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Legs")));
			this.SetClothes(ClothingSlot.Feet, HumanReadableConverter.ValuesListFromString<int>(';', value.GetValue<string>("Feet")));
			Display.DeviceReset += this.Display_DeviceReset;
		}

        // Token: 0x06000C04 RID: 3076 RVA: 0x0005B23C File Offset: 0x0005943C
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Clothes", valuesDictionary2);
			valuesDictionary2.SetValue<string>("Head", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Head].ToArray()));
			valuesDictionary2.SetValue<string>("Torso", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Torso].ToArray()));
			valuesDictionary2.SetValue<string>("Legs", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Legs].ToArray()));
			valuesDictionary2.SetValue<string>("Feet", HumanReadableConverter.ValuesListToString<int>(';', this.m_clothes[ClothingSlot.Feet].ToArray()));
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x0005B2E8 File Offset: 0x000594E8
		public override void Dispose()
		{
			base.Dispose();
			if (this.m_skinTexture != null && !ContentManager.IsContent(this.m_skinTexture))
			{
				this.m_skinTexture.Dispose();
				this.m_skinTexture = null;
			}
			if (this.m_innerClothedTexture != null)
			{
				this.m_innerClothedTexture.Dispose();
				this.m_innerClothedTexture = null;
			}
			if (this.m_outerClothedTexture != null)
			{
				this.m_outerClothedTexture.Dispose();
				this.m_outerClothedTexture = null;
			}
			Display.DeviceReset -= this.Display_DeviceReset;
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x0005B368 File Offset: 0x00059568
		public void Update(float dt)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled && this.m_subsystemTime.PeriodicGameTimeEvent(0.5, 0.0))
			{
				foreach (int slot in EnumUtils.GetEnumValues(typeof(ClothingSlot)))
				{
					bool flag = false;
					this.m_clothesList.Clear();
					this.m_clothesList.AddRange(this.GetClothes((ClothingSlot)slot));
					int i = 0;
					while (i < this.m_clothesList.Count)
					{
						int value = this.m_clothesList[i];
						ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
						if ((float)clothingData.PlayerLevelRequired > this.m_componentPlayer.PlayerData.Level)
						{
							this.m_componentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentClothing.fName, 1), clothingData.PlayerLevelRequired, clothingData.DisplayName), Color.White, true, true);
							this.m_subsystemPickables.AddPickable(value, 1, this.m_componentBody.Position, null, null);
							this.m_clothesList.RemoveAt(i);
							flag = true;
						}
						else
						{
							i++;
						}
					}
					if (flag)
					{
						this.SetClothes((ClothingSlot)slot, this.m_clothesList);
					}
				}
			}
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled && this.m_subsystemTime.PeriodicGameTimeEvent(2.0, 0.0) && ((this.m_componentLocomotion.LastWalkOrder != null && this.m_componentLocomotion.LastWalkOrder.Value != Vector2.Zero) || (this.m_componentLocomotion.LastSwimOrder != null && this.m_componentLocomotion.LastSwimOrder.Value != Vector3.Zero) || this.m_componentLocomotion.LastJumpOrder != 0f))
			{
				if (this.m_lastTotalElapsedGameTime != null)
				{
					foreach (int slot2 in EnumUtils.GetEnumValues(typeof(ClothingSlot)))
					{
						bool flag2 = false;
						this.m_clothesList.Clear();
						this.m_clothesList.AddRange(this.GetClothes((ClothingSlot)slot2));
						for (int j = 0; j < this.m_clothesList.Count; j++)
						{
							int value2 = this.m_clothesList[j];
							ClothingData clothingData2 = ClothingBlock.GetClothingData(Terrain.ExtractData(value2));
							float num = (this.m_componentVitalStats.Wetness > 0f) ? (10f * clothingData2.Sturdiness) : (20f * clothingData2.Sturdiness);
							double num2 = MathUtils.Floor(this.m_lastTotalElapsedGameTime.Value / (double)num);
							if (MathUtils.Floor(this.m_subsystemGameInfo.TotalElapsedGameTime / (double)num) > num2 && this.m_random.Float(0f, 1f) < 0.75f)
							{
								this.m_clothesList[j] = BlocksManager.DamageItem(value2, 1);
								flag2 = true;
							}
						}
						int k = 0;
						while (k < this.m_clothesList.Count)
						{
							if (Terrain.ExtractContents(this.m_clothesList[k]) != 203)
							{
								this.m_clothesList.RemoveAt(k);
								this.m_subsystemParticles.AddParticleSystem(new BlockDebrisParticleSystem(this.m_subsystemTerrain, this.m_componentBody.Position + this.m_componentBody.BoxSize / 2f, 1f, 1f, Color.White, 0));
								this.m_componentGui.DisplaySmallMessage(LanguageControl.Get(ComponentClothing.fName, 2), Color.White, true, true);
							}
							else
							{
								k++;
							}
						}
						if (flag2)
						{
							this.SetClothes((ClothingSlot)slot2, this.m_clothesList);
						}
					}
				}
				this.m_lastTotalElapsedGameTime = new double?(this.m_subsystemGameInfo.TotalElapsedGameTime);
			}
			this.UpdateRenderTargets();
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x0005B810 File Offset: 0x00059A10
		public int GetSlotValue(int slotIndex)
		{
			return this.GetClothes((ClothingSlot)slotIndex).LastOrDefault<int>();
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0005B823 File Offset: 0x00059A23
		public int GetSlotCount(int slotIndex)
		{
			if ((this.GetClothes((ClothingSlot)slotIndex)).Count <= 0)
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x0005B83C File Offset: 0x00059A3C
		public int GetSlotCapacity(int slotIndex, int value)
		{
			return 0;
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x0005B840 File Offset: 0x00059A40
		public int GetSlotProcessCapacity(int slotIndex, int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			if (block.GetNutritionalValue(value) > 0f)
			{
				return 1;
			}
			if (block is ClothingBlock && this.CanWearClothing(value))
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x0005B87E File Offset: 0x00059A7E
		public void AddSlotItems(int slotIndex, int value, int count)
		{
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x0005B880 File Offset: 0x00059A80
		public void ProcessSlotItems(int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			processedCount = 0;
			processedValue = 0;
			if (processCount != 1)
			{
				return;
			}
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			if (block.GetNutritionalValue(value) > 0f)
			{
				if (block is BucketBlock)
				{
					processedValue = Terrain.MakeBlockValue(90, 0, Terrain.ExtractData(value));
					processedCount = 1;
				}
				if (count > 1 && processedCount > 0 && processedValue != value)
				{
					processedValue = value;
					processedCount = processCount;
				}
				else if (!this.m_componentVitalStats.Eat(value))
				{
					processedValue = value;
					processedCount = processCount;
				}
			}
			if (block is ClothingBlock)
			{
				ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
				List<int> list = new List<int>(this.GetClothes(clothingData.Slot));
				list.Add(value);
				this.SetClothes(clothingData.Slot, list);
			}
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x0005B944 File Offset: 0x00059B44
		public int RemoveSlotItems(int slotIndex, int count)
		{
			if (count == 1)
			{
				List<int> list = new List<int>(this.GetClothes((ClothingSlot)slotIndex));
				if (list.Count > 0)
				{
					list.RemoveAt(list.Count - 1);
					this.SetClothes((ClothingSlot)slotIndex, list);
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x0005B98C File Offset: 0x00059B8C
		public void DropAllItems(Vector3 position)
		{
			Game.Random random = new Game.Random();
			SubsystemPickables subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			for (int i = 0; i < this.SlotsCount; i++)
			{
				int slotCount = this.GetSlotCount(i);
				if (slotCount > 0)
				{
					int slotValue = this.GetSlotValue(i);
					int count = this.RemoveSlotItems(i, slotCount);
					Vector3 value = random.Float(5f, 10f) * Vector3.Normalize(new Vector3(random.Float(-1f, 1f), random.Float(1f, 2f), random.Float(-1f, 1f)));
					subsystemPickables.AddPickable(slotValue, count, position, new Vector3?(value), null);
				}
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x0005BA53 File Offset: 0x00059C53
		public void Display_DeviceReset()
		{
			this.m_clothedTexturesValid = false;
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x0005BA5C File Offset: 0x00059C5C
		public bool CanWearClothing(int value)
		{
			ClothingData clothingData = ClothingBlock.GetClothingData(Terrain.ExtractData(value));
			ReadOnlyList<int> list = this.GetClothes(clothingData.Slot);
			if (list.Count == 0)
			{
				return true;
			}
			ClothingData clothingData2 = ClothingBlock.GetClothingData(Terrain.ExtractData(list[list.Count - 1]));
			return clothingData.Layer > clothingData2.Layer;
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x0005BAB8 File Offset: 0x00059CB8
		public void UpdateRenderTargets()
		{
			if (this.m_skinTexture == null || this.m_componentPlayer.PlayerData.CharacterSkinName != this.m_skinTextureName)
			{
				this.m_skinTexture = CharacterSkinsManager.LoadTexture(this.m_componentPlayer.PlayerData.CharacterSkinName);
				this.m_skinTextureName = this.m_componentPlayer.PlayerData.CharacterSkinName;
				Utilities.Dispose<RenderTarget2D>(ref this.m_innerClothedTexture);
				Utilities.Dispose<RenderTarget2D>(ref this.m_outerClothedTexture);
			}
			if (this.m_innerClothedTexture == null || this.m_innerClothedTexture.Width != this.m_skinTexture.Width || this.m_innerClothedTexture.Height != this.m_skinTexture.Height)
			{
				this.m_innerClothedTexture = new RenderTarget2D(this.m_skinTexture.Width, this.m_skinTexture.Height, 1, ColorFormat.Rgba8888, DepthFormat.None);
				this.m_componentHumanModel.TextureOverride = this.m_innerClothedTexture;
				this.m_clothedTexturesValid = false;
			}
			if (this.m_outerClothedTexture == null || this.m_outerClothedTexture.Width != this.m_skinTexture.Width || this.m_outerClothedTexture.Height != this.m_skinTexture.Height)
			{
				this.m_outerClothedTexture = new RenderTarget2D(this.m_skinTexture.Width, this.m_skinTexture.Height, 1, ColorFormat.Rgba8888, DepthFormat.None);
				this.m_componentOuterClothingModel.TextureOverride = this.m_outerClothedTexture;
				this.m_clothedTexturesValid = false;
			}
			if (ComponentClothing.DrawClothedTexture && !this.m_clothedTexturesValid)
			{
				this.m_clothedTexturesValid = true;
				Rectangle scissorRectangle = Display.ScissorRectangle;
				RenderTarget2D renderTarget = Display.RenderTarget;
				try
				{
					Display.RenderTarget = this.m_innerClothedTexture;
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					int num = 0;
					TexturedBatch2D texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(this.m_skinTexture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
					texturedBatch2D.QueueQuad(Vector2.Zero, new Vector2((float)this.m_innerClothedTexture.Width, (float)this.m_innerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, Color.White);
					foreach (ClothingSlot slot in ComponentClothing.m_innerSlotsOrder)
					{
						foreach (int value in this.GetClothes(slot))
						{
							int data = Terrain.ExtractData(value);
							ClothingData clothingData = ClothingBlock.GetClothingData(data);
							Color fabricColor = SubsystemPalette.GetFabricColor(this.m_subsystemTerrain, new int?(ClothingBlock.GetClothingColor(data)));
							texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(clothingData.Texture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
							if (!clothingData.IsOuter)
							{
								texturedBatch2D.QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_innerClothedTexture.Width, (float)this.m_innerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, fabricColor);
							}
						}
					}
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
					Display.RenderTarget = this.m_outerClothedTexture;
					Display.Clear(new Vector4?(new Vector4(Color.Transparent)), null, null);
					num = 0;
					foreach (ClothingSlot slot2 in ComponentClothing.m_outerSlotsOrder)
					{
						foreach (int value2 in this.GetClothes(slot2))
						{
							int data2 = Terrain.ExtractData(value2);
							ClothingData clothingData2 = ClothingBlock.GetClothingData(data2);
							Color fabricColor2 = SubsystemPalette.GetFabricColor(this.m_subsystemTerrain, new int?(ClothingBlock.GetClothingColor(data2)));
							texturedBatch2D = this.m_primitivesRenderer.TexturedBatch(clothingData2.Texture, false, num++, DepthStencilState.None, null, BlendState.NonPremultiplied, SamplerState.PointClamp);
							if (clothingData2.IsOuter)
							{
								texturedBatch2D.QueueQuad(new Vector2(0f, 0f), new Vector2((float)this.m_outerClothedTexture.Width, (float)this.m_outerClothedTexture.Height), 0f, Vector2.Zero, Vector2.One, fabricColor2);
							}
						}
					}
					this.m_primitivesRenderer.Flush(true, int.MaxValue);
				}
				finally
				{
					Display.RenderTarget = renderTarget;
					Display.ScissorRectangle = scissorRectangle;
				}
			}
		}

		// Token: 0x040006CC RID: 1740
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040006CD RID: 1741
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040006CE RID: 1742
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040006CF RID: 1743
		public SubsystemTime m_subsystemTime;

		// Token: 0x040006D0 RID: 1744
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040006D1 RID: 1745
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040006D2 RID: 1746
		public ComponentGui m_componentGui;

		// Token: 0x040006D3 RID: 1747
		public ComponentHumanModel m_componentHumanModel;

		// Token: 0x040006D4 RID: 1748
		public ComponentBody m_componentBody;

		// Token: 0x040006D5 RID: 1749
		public ComponentOuterClothingModel m_componentOuterClothingModel;

		// Token: 0x040006D6 RID: 1750
		public ComponentVitalStats m_componentVitalStats;

		// Token: 0x040006D7 RID: 1751
		public ComponentLocomotion m_componentLocomotion;

		// Token: 0x040006D8 RID: 1752
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040006D9 RID: 1753
		public Texture2D m_skinTexture;

		// Token: 0x040006DA RID: 1754
		public string m_skinTextureName;

		// Token: 0x040006DB RID: 1755
		public RenderTarget2D m_innerClothedTexture;

		// Token: 0x040006DC RID: 1756
		public RenderTarget2D m_outerClothedTexture;

		// Token: 0x040006DD RID: 1757
		public PrimitivesRenderer2D m_primitivesRenderer = new PrimitivesRenderer2D();

		// Token: 0x040006DE RID: 1758
		public Game.Random m_random = new Game.Random();

		// Token: 0x040006DF RID: 1759
		public float m_densityModifierApplied;

		// Token: 0x040006E0 RID: 1760
		public double? m_lastTotalElapsedGameTime;

		// Token: 0x040006E1 RID: 1761
		public bool m_clothedTexturesValid;

		// Token: 0x040006E2 RID: 1762
		public static string fName = "ComponentClothing";

		// Token: 0x040006E3 RID: 1763
		public List<int> m_clothesList = new List<int>();

		// Token: 0x040006E4 RID: 1764
		public Dictionary<ClothingSlot, List<int>> m_clothes = new Dictionary<ClothingSlot, List<int>>();

		// Token: 0x040006E5 RID: 1765
		public static ClothingSlot[] m_innerSlotsOrder = new ClothingSlot[]
		{
			ClothingSlot.Head,
			ClothingSlot.Torso,
			ClothingSlot.Feet,
			ClothingSlot.Legs
		};

		// Token: 0x040006E6 RID: 1766
		public static ClothingSlot[] m_outerSlotsOrder = new ClothingSlot[]
		{
			ClothingSlot.Head,
			ClothingSlot.Torso,
			ClothingSlot.Legs,
			ClothingSlot.Feet
		};

		// Token: 0x040006E7 RID: 1767
		public static bool ShowClothedTexture = false;

		// Token: 0x040006E8 RID: 1768
		public static bool DrawClothedTexture = true;
	}
}
