using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000165 RID: 357
	public class SubsystemBowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x0002C7F6 File Offset: 0x0002A9F6
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0002C7FE File Offset: 0x0002A9FE
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = ((componentPlayer.ComponentGui.ModalPanelWidget == null) ? new BowWidget(inventory, slotIndex) : null);
			return true;
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0002C824 File Offset: 0x0002AA24
		public override bool OnAim(Ray3 aim, ComponentMiner componentMiner, AimState state)
		{
			IInventory inventory = componentMiner.Inventory;
			if (inventory != null)
			{
				int activeSlotIndex = inventory.ActiveSlotIndex;
				if (activeSlotIndex >= 0)
				{
					int slotValue = inventory.GetSlotValue(activeSlotIndex);
					int slotCount = inventory.GetSlotCount(activeSlotIndex);
					int num = Terrain.ExtractContents(slotValue);
					int data = Terrain.ExtractData(slotValue);
					if (num == 191 && slotCount > 0)
					{
						double gameTime;
						if (!this.m_aimStartTimes.TryGetValue(componentMiner, out gameTime))
						{
							gameTime = this.m_subsystemTime.GameTime;
							this.m_aimStartTimes[componentMiner] = gameTime;
						}
						float num2 = (float)(this.m_subsystemTime.GameTime - gameTime);
						float num3 = (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0);
						Vector3 v = ((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.02f : 0.04f) + 0.25f * MathUtils.Saturate((num2 - 2.1f) / 5f)) * new Vector3
						{
							X = SimplexNoise.OctavedNoise(num3, 2f, 3, 2f, 0.5f, false),
							Y = SimplexNoise.OctavedNoise(num3 + 100f, 2f, 3, 2f, 0.5f, false),
							Z = SimplexNoise.OctavedNoise(num3 + 200f, 2f, 3, 2f, 0.5f, false)
						};
						aim.Direction = Vector3.Normalize(aim.Direction + v);
						switch (state)
						{
						case AimState.InProgress:
						{
							if (num2 >= 9f)
							{
								componentMiner.ComponentCreature.ComponentCreatureSounds.PlayMoanSound();
								return true;
							}
							ComponentFirstPersonModel componentFirstPersonModel = componentMiner.Entity.FindComponent<ComponentFirstPersonModel>();
							if (componentFirstPersonModel != null)
							{
								ComponentPlayer componentPlayer = componentMiner.ComponentPlayer;
								if (componentPlayer != null)
								{
									componentPlayer.ComponentAimingSights.ShowAimingSights(aim.Position, aim.Direction);
								}
								componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.1f, 0.15f, 0f);
								componentFirstPersonModel.ItemRotationOrder = new Vector3(0f, -0.7f, 0f);
							}
							componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.2f;
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(0f, 0f, 0f);
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(0f, -0.2f, 0f);
							if (this.m_subsystemTime.PeriodicGameTimeEvent(0.10000000149011612, 0.0))
							{
								int draw = MathUtils.Min(BowBlock.GetDraw(data) + 1, 15);
								inventory.RemoveSlotItems(activeSlotIndex, 1);
								inventory.AddSlotItems(activeSlotIndex, Terrain.MakeBlockValue(num, 0, BowBlock.SetDraw(data, draw)), 1);
							}
							break;
						}
						case AimState.Cancelled:
							inventory.RemoveSlotItems(activeSlotIndex, 1);
							inventory.AddSlotItems(activeSlotIndex, Terrain.MakeBlockValue(num, 0, BowBlock.SetDraw(data, 0)), 1);
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						case AimState.Completed:
						{
							int draw2 = BowBlock.GetDraw(data);
							ArrowBlock.ArrowType? arrowType = BowBlock.GetArrowType(data);
							if (arrowType != null)
							{
								Vector3 vector = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition + componentMiner.ComponentCreature.ComponentBody.Matrix.Right * 0.3f - componentMiner.ComponentCreature.ComponentBody.Matrix.Up * 0.2f;
								Vector3 vector2 = Vector3.Normalize(vector + aim.Direction * 10f - vector);
								float num4 = MathUtils.Lerp(0f, 28f, MathUtils.Pow((float)draw2 / 15f, 0.75f));
								if (componentMiner.ComponentPlayer != null)
								{
									num4 *= 0.5f * (componentMiner.ComponentPlayer.ComponentLevel.StrengthFactor - 1f) + 1f;
								}
								Vector3 zero = Vector3.Zero;
								ArrowBlock.ArrowType? arrowType2 = arrowType;
								ArrowBlock.ArrowType arrowType3 = ArrowBlock.ArrowType.WoodenArrow;
								if (arrowType2.GetValueOrDefault() == arrowType3 & arrowType2 != null)
								{
									zero = new Vector3(0.025f, 0.025f, 0.025f);
								}
								arrowType2 = arrowType;
								arrowType3 = ArrowBlock.ArrowType.StoneArrow;
								if (arrowType2.GetValueOrDefault() == arrowType3 & arrowType2 != null)
								{
									zero = new Vector3(0.01f, 0.01f, 0.01f);
								}
								int value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
								Vector3 vector3 = Vector3.Normalize(Vector3.Cross(vector2, Vector3.UnitY));
								Vector3 v2 = Vector3.Normalize(Vector3.Cross(vector2, vector3));
								Vector3 v3 = this.m_random.Float(0f - zero.X, zero.X) * vector3 + this.m_random.Float(0f - zero.Y, zero.Y) * v2 + this.m_random.Float(0f - zero.Z, zero.Z) * vector2;
								if (this.m_subsystemProjectiles.FireProjectile(value, vector, (vector2 + v3) * num4, Vector3.Zero, componentMiner.ComponentCreature) != null)
								{
									data = BowBlock.SetArrowType(data, null);
									this.m_subsystemAudio.PlaySound("Audio/Bow", 1f, this.m_random.Float(-0.1f, 0.1f), vector, 3f, true);
								}
							}
							else
							{
								ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
								if (componentPlayer2 != null)
								{
									componentPlayer2.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemBowBlockBehavior.fName, 0), Color.White, true, false);
								}
							}
							inventory.RemoveSlotItems(activeSlotIndex, 1);
							int value2 = Terrain.MakeBlockValue(num, 0, BowBlock.SetDraw(data, 0));
							inventory.AddSlotItems(activeSlotIndex, value2, 1);
							int damageCount = 0;
							if (draw2 >= 15)
							{
								damageCount = 2;
							}
							else if (draw2 >= 4)
							{
								damageCount = 1;
							}
							componentMiner.DamageActiveTool(damageCount);
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0002CE34 File Offset: 0x0002B034
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			int num = Terrain.ExtractContents(value);
			ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (num != 192 || !this.m_supportedArrowTypes.Contains(arrowType))
			{
				return 0;
			}
			if (BowBlock.GetArrowType(Terrain.ExtractData(inventory.GetSlotValue(slotIndex))) == null)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0002CE88 File Offset: 0x0002B088
		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			if (processCount == 1)
			{
				ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
				int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);
				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(191, 0, BowBlock.SetArrowType(data, new ArrowBlock.ArrowType?(arrowType))), 1);
				return;
			}
			processedValue = value;
			processedCount = count;
		}

        // Token: 0x06000710 RID: 1808 RVA: 0x0002CEED File Offset: 0x0002B0ED
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x040003E6 RID: 998
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003E7 RID: 999
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x040003E8 RID: 1000
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040003E9 RID: 1001
		public static string fName = "SubsystemBowBlockBehavior";

		// Token: 0x040003EA RID: 1002
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003EB RID: 1003
		public Dictionary<ComponentMiner, double> m_aimStartTimes = new Dictionary<ComponentMiner, double>();

		// Token: 0x040003EC RID: 1004
		public ArrowBlock.ArrowType[] m_supportedArrowTypes = new ArrowBlock.ArrowType[]
		{
			ArrowBlock.ArrowType.WoodenArrow,
			ArrowBlock.ArrowType.StoneArrow,
			ArrowBlock.ArrowType.CopperArrow,
			ArrowBlock.ArrowType.IronArrow,
			ArrowBlock.ArrowType.DiamondArrow,
			ArrowBlock.ArrowType.FireArrow
		};
	}
}
