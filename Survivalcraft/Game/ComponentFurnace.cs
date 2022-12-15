using System;
using System.Globalization;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001E1 RID: 481
	public class ComponentFurnace : ComponentInventoryBase, IUpdateable
	{
		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000D25 RID: 3365 RVA: 0x00064391 File Offset: 0x00062591
		public int RemainsSlotIndex
		{
			get
			{
				return this.SlotsCount - 1;
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000D26 RID: 3366 RVA: 0x0006439B File Offset: 0x0006259B
		public int ResultSlotIndex
		{
			get
			{
				return this.SlotsCount - 2;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000D27 RID: 3367 RVA: 0x000643A5 File Offset: 0x000625A5
		public int FuelSlotIndex
		{
			get
			{
				return this.SlotsCount - 3;
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06000D28 RID: 3368 RVA: 0x000643AF File Offset: 0x000625AF
		public float HeatLevel
		{
			get
			{
				return this.m_heatLevel;
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06000D29 RID: 3369 RVA: 0x000643B7 File Offset: 0x000625B7
		public float SmeltingProgress
		{
			get
			{
				return this.m_smeltingProgress;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06000D2A RID: 3370 RVA: 0x000643BF File Offset: 0x000625BF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000D2B RID: 3371 RVA: 0x000643C2 File Offset: 0x000625C2
		public override int GetSlotCapacity(int slotIndex, int value)
		{
			if (slotIndex != this.FuelSlotIndex)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			if (BlocksManager.Blocks[Terrain.ExtractContents(value)].FuelHeatLevel > 0f)
			{
				return base.GetSlotCapacity(slotIndex, value);
			}
			return 0;
		}

		// Token: 0x06000D2C RID: 3372 RVA: 0x000643F8 File Offset: 0x000625F8
		public override void AddSlotItems(int slotIndex, int value, int count)
		{
			this.m_updateSmeltingRecipe = true;
			base.AddSlotItems(slotIndex, value, count);
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x0006440A File Offset: 0x0006260A
		public override int RemoveSlotItems(int slotIndex, int count)
		{
			this.m_updateSmeltingRecipe = true;
			return base.RemoveSlotItems(slotIndex, count);
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x0006441C File Offset: 0x0006261C
		public void Update(float dt)
		{
			Point3 coordinates = this.m_componentBlockEntity.Coordinates;
			if (this.m_heatLevel > 0f)
			{
				this.m_fireTimeRemaining = MathUtils.Max(0f, this.m_fireTimeRemaining - dt);
				if (this.m_fireTimeRemaining == 0f)
				{
					this.m_heatLevel = 0f;
				}
			}
			if (this.m_updateSmeltingRecipe)
			{
				this.m_updateSmeltingRecipe = false;
				float heatLevel = 0f;
				if (this.m_heatLevel > 0f)
				{
					heatLevel = this.m_heatLevel;
				}
				else
				{
					ComponentInventoryBase.Slot slot = this.m_slots[this.FuelSlotIndex];
					if (slot.Count > 0)
					{
						int num = Terrain.ExtractContents(slot.Value);
						heatLevel = BlocksManager.Blocks[num].FuelHeatLevel;
					}
				}
				CraftingRecipe craftingRecipe = this.FindSmeltingRecipe(heatLevel);
				if (craftingRecipe != this.m_smeltingRecipe)
				{
					this.m_smeltingRecipe = ((craftingRecipe != null && craftingRecipe.ResultValue != 0) ? craftingRecipe : null);
					this.m_smeltingProgress = 0f;
				}
			}
			if (this.m_smeltingRecipe == null)
			{
				this.m_heatLevel = 0f;
				this.m_fireTimeRemaining = 0f;
			}
			if (this.m_smeltingRecipe != null && this.m_fireTimeRemaining <= 0f)
			{
				ComponentInventoryBase.Slot slot2 = this.m_slots[this.FuelSlotIndex];
				if (slot2.Count > 0)
				{
					int num2 = Terrain.ExtractContents(slot2.Value);
					Block block = BlocksManager.Blocks[num2];
					if (block.GetExplosionPressure(slot2.Value) > 0f)
					{
						slot2.Count = 0;
						this.m_subsystemExplosions.TryExplodeBlock(coordinates.X, coordinates.Y, coordinates.Z, slot2.Value);
					}
					else if (block.FuelHeatLevel > 0f)
					{
						slot2.Count--;
						this.m_fireTimeRemaining = block.FuelFireDuration;
						this.m_heatLevel = block.FuelHeatLevel;
					}
				}
			}
			if (this.m_fireTimeRemaining <= 0f)
			{
				this.m_smeltingRecipe = null;
				this.m_smeltingProgress = 0f;
			}
			if (this.m_smeltingRecipe != null)
			{
				this.m_smeltingProgress = MathUtils.Min(this.m_smeltingProgress + 0.15f * dt, 1f);
				if (this.m_smeltingProgress >= 1f)
				{
					for (int i = 0; i < this.m_furnaceSize; i++)
					{
						if (this.m_slots[i].Count > 0)
						{
							this.m_slots[i].Count--;
						}
					}
					this.m_slots[this.ResultSlotIndex].Value = this.m_smeltingRecipe.ResultValue;
					this.m_slots[this.ResultSlotIndex].Count += this.m_smeltingRecipe.ResultCount;
					if (this.m_smeltingRecipe.RemainsValue != 0 && this.m_smeltingRecipe.RemainsCount > 0)
					{
						this.m_slots[this.RemainsSlotIndex].Value = this.m_smeltingRecipe.RemainsValue;
						this.m_slots[this.RemainsSlotIndex].Count += this.m_smeltingRecipe.RemainsCount;
					}
					this.m_smeltingRecipe = null;
					this.m_smeltingProgress = 0f;
					this.m_updateSmeltingRecipe = true;
				}
			}
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(coordinates.X, coordinates.Z);
			if (chunkAtCell != null && chunkAtCell.State == TerrainChunkState.Valid)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
				this.m_subsystemTerrain.ChangeCell(coordinates.X, coordinates.Y, coordinates.Z, Terrain.ReplaceContents(cellValue, (this.m_heatLevel > 0f) ? 65 : 64), true);
			}
		}

        // Token: 0x06000D2F RID: 3375 RVA: 0x000647E0 File Offset: 0x000629E0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_componentBlockEntity = base.Entity.FindComponent<ComponentBlockEntity>(true);
			this.m_furnaceSize = this.SlotsCount - 3;
			if (this.m_furnaceSize < 1 || this.m_furnaceSize > 3)
			{
				throw new InvalidOperationException("Invalid furnace size.");
			}
			this.m_fireTimeRemaining = valuesDictionary.GetValue<float>("FireTimeRemaining");
			this.m_heatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			this.m_updateSmeltingRecipe = true;
		}

        // Token: 0x06000D30 RID: 3376 RVA: 0x0006487F File Offset: 0x00062A7F
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<float>("FireTimeRemaining", this.m_fireTimeRemaining);
			valuesDictionary.SetValue<float>("HeatLevel", this.m_heatLevel);
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x000648AC File Offset: 0x00062AAC
		public CraftingRecipe FindSmeltingRecipe(float heatLevel)
		{
			if (heatLevel > 0f)
			{
				for (int i = 0; i < this.m_furnaceSize; i++)
				{
					int slotValue = this.GetSlotValue(i);
					int num = Terrain.ExtractContents(slotValue);
					int num2 = Terrain.ExtractData(slotValue);
					if (this.GetSlotCount(i) > 0)
					{
						Block block = BlocksManager.Blocks[num];
						this.m_matchedIngredients[i] = block.CraftingId + ":" + num2.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this.m_matchedIngredients[i] = null;
					}
				}
				ComponentPlayer componentPlayer = base.FindInteractingPlayer();
				float playerLevel = (componentPlayer != null) ? componentPlayer.PlayerData.Level : 1f;
				CraftingRecipe craftingRecipe = CraftingRecipesManager.FindMatchingRecipe(this.m_subsystemTerrain, this.m_matchedIngredients, heatLevel, playerLevel);
				if (craftingRecipe != null && craftingRecipe.ResultValue != 0)
				{
					if (craftingRecipe.RequiredHeatLevel <= 0f)
					{
						craftingRecipe = null;
					}
					if (craftingRecipe != null)
					{
						ComponentInventoryBase.Slot slot = this.m_slots[this.ResultSlotIndex];
						int num3 = Terrain.ExtractContents(craftingRecipe.ResultValue);
						if (slot.Count != 0 && (craftingRecipe.ResultValue != slot.Value || craftingRecipe.ResultCount + slot.Count > BlocksManager.Blocks[num3].MaxStacking))
						{
							craftingRecipe = null;
						}
					}
					if (craftingRecipe != null && craftingRecipe.RemainsValue != 0 && craftingRecipe.RemainsCount > 0)
					{
						if (this.m_slots[this.RemainsSlotIndex].Count == 0 || this.m_slots[this.RemainsSlotIndex].Value == craftingRecipe.RemainsValue)
						{
							if (BlocksManager.Blocks[Terrain.ExtractContents(craftingRecipe.RemainsValue)].MaxStacking - this.m_slots[this.RemainsSlotIndex].Count < craftingRecipe.RemainsCount)
							{
								craftingRecipe = null;
							}
						}
						else
						{
							craftingRecipe = null;
						}
					}
				}
				if (craftingRecipe != null && !string.IsNullOrEmpty(craftingRecipe.Message) && componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(craftingRecipe.Message, Color.White, true, true);
				}
				return craftingRecipe;
			}
			return null;
		}

		// Token: 0x0400080D RID: 2061
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400080E RID: 2062
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x0400080F RID: 2063
		public ComponentBlockEntity m_componentBlockEntity;

		// Token: 0x04000810 RID: 2064
		public int m_furnaceSize;

		// Token: 0x04000811 RID: 2065
		public string[] m_matchedIngredients = new string[9];

		// Token: 0x04000812 RID: 2066
		public float m_fireTimeRemaining;

		// Token: 0x04000813 RID: 2067
		public float m_heatLevel;

		// Token: 0x04000814 RID: 2068
		public bool m_updateSmeltingRecipe;

		// Token: 0x04000815 RID: 2069
		public CraftingRecipe m_smeltingRecipe;

		// Token: 0x04000816 RID: 2070
		public float m_smeltingProgress;
	}
}
