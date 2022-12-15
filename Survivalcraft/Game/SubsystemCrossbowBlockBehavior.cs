using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200016F RID: 367
	public class SubsystemCrossbowBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x00033351 File Offset: 0x00031551
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00033359 File Offset: 0x00031559
		public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
		{
			componentPlayer.ComponentGui.ModalPanelWidget = ((componentPlayer.ComponentGui.ModalPanelWidget == null) ? new CrossbowWidget(inventory, slotIndex) : null);
			return true;
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x00033380 File Offset: 0x00031580
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
					if (num == 200 && slotCount > 0)
					{
						int draw = CrossbowBlock.GetDraw(data);
						double gameTime;
						if (!this.m_aimStartTimes.TryGetValue(componentMiner, out gameTime))
						{
							gameTime = this.m_subsystemTime.GameTime;
							this.m_aimStartTimes[componentMiner] = gameTime;
						}
						float num2 = (float)(this.m_subsystemTime.GameTime - gameTime);
						float num3 = (float)MathUtils.Remainder(this.m_subsystemTime.GameTime, 1000.0);
						Vector3 v = ((componentMiner.ComponentCreature.ComponentBody.IsSneaking ? 0.01f : 0.03f) + 0.15f * MathUtils.Saturate((num2 - 2.5f) / 6f)) * new Vector3
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
							if (num2 >= 10f)
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
								componentFirstPersonModel.ItemOffsetOrder = new Vector3(-0.22f, 0.15f, 0.1f);
								componentFirstPersonModel.ItemRotationOrder = new Vector3(-0.7f, 0f, 0f);
							}
							componentMiner.ComponentCreature.ComponentCreatureModel.AimHandAngleOrder = 1.3f;
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemOffsetOrder = new Vector3(-0.08f, -0.1f, 0.07f);
							componentMiner.ComponentCreature.ComponentCreatureModel.InHandItemRotationOrder = new Vector3(-1.55f, 0f, 0f);
							break;
						}
						case AimState.Cancelled:
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						case AimState.Completed:
						{
							ArrowBlock.ArrowType? arrowType = CrossbowBlock.GetArrowType(data);
							if (draw != 15)
							{
								ComponentPlayer componentPlayer2 = componentMiner.ComponentPlayer;
								if (componentPlayer2 != null)
								{
									componentPlayer2.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemCrossbowBlockBehavior.fName, 0), Color.White, true, false);
								}
							}
							else if (arrowType == null)
							{
								ComponentPlayer componentPlayer3 = componentMiner.ComponentPlayer;
								if (componentPlayer3 != null)
								{
									componentPlayer3.ComponentGui.DisplaySmallMessage(LanguageControl.Get(SubsystemCrossbowBlockBehavior.fName, 1), Color.White, true, false);
								}
							}
							else
							{
								Vector3 vector = componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition + componentMiner.ComponentCreature.ComponentBody.Matrix.Right * 0.3f - componentMiner.ComponentCreature.ComponentBody.Matrix.Up * 0.2f;
								Vector3 v2 = Vector3.Normalize(vector + aim.Direction * 10f - vector);
								int value = Terrain.MakeBlockValue(192, 0, ArrowBlock.SetArrowType(0, arrowType.Value));
								float s = 38f;
								if (this.m_subsystemProjectiles.FireProjectile(value, vector, s * v2, Vector3.Zero, componentMiner.ComponentCreature) != null)
								{
									data = CrossbowBlock.SetArrowType(data, null);
									this.m_subsystemAudio.PlaySound("Audio/Bow", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, 0.05f);
								}
							}
							inventory.RemoveSlotItems(activeSlotIndex, 1);
							int value2 = Terrain.MakeBlockValue(num, 0, CrossbowBlock.SetDraw(data, 0));
							inventory.AddSlotItems(activeSlotIndex, value2, 1);
							if (draw > 0)
							{
								componentMiner.DamageActiveTool(1);
								this.m_subsystemAudio.PlaySound("Audio/CrossbowBoing", 1f, this.m_random.Float(-0.1f, 0.1f), componentMiner.ComponentCreature.ComponentCreatureModel.EyePosition, 3f, 0f);
							}
							this.m_aimStartTimes.Remove(componentMiner);
							break;
						}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x00033838 File Offset: 0x00031A38
		public override int GetProcessInventoryItemCapacity(IInventory inventory, int slotIndex, int value)
		{
			int num = Terrain.ExtractContents(value);
			ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
			if (num != 192 || !this.m_supportedArrowTypes.Contains(arrowType))
			{
				return 0;
			}
			int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
			ArrowBlock.ArrowType? arrowType2 = CrossbowBlock.GetArrowType(data);
			int draw = CrossbowBlock.GetDraw(data);
			if (arrowType2 == null && draw == 15)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x00033898 File Offset: 0x00031A98
		public override void ProcessInventoryItem(IInventory inventory, int slotIndex, int value, int count, int processCount, out int processedValue, out int processedCount)
		{
			if (processCount == 1)
			{
				ArrowBlock.ArrowType arrowType = ArrowBlock.GetArrowType(Terrain.ExtractData(value));
				int data = Terrain.ExtractData(inventory.GetSlotValue(slotIndex));
				processedValue = 0;
				processedCount = 0;
				inventory.RemoveSlotItems(slotIndex, 1);
				inventory.AddSlotItems(slotIndex, Terrain.MakeBlockValue(200, 0, CrossbowBlock.SetArrowType(data, new ArrowBlock.ArrowType?(arrowType))), 1);
				return;
			}
			processedValue = value;
			processedCount = count;
		}

        // Token: 0x060007DE RID: 2014 RVA: 0x000338FD File Offset: 0x00031AFD
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			base.Load(valuesDictionary);
		}

		// Token: 0x0400041E RID: 1054
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400041F RID: 1055
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x04000420 RID: 1056
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000421 RID: 1057
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000422 RID: 1058
		public static string fName = "SubsystemCrossbowBlockBehavior";

		// Token: 0x04000423 RID: 1059
		public Dictionary<ComponentMiner, double> m_aimStartTimes = new Dictionary<ComponentMiner, double>();

		// Token: 0x04000424 RID: 1060
		public ArrowBlock.ArrowType[] m_supportedArrowTypes = new ArrowBlock.ArrowType[]
		{
			ArrowBlock.ArrowType.IronBolt,
			ArrowBlock.ArrowType.DiamondBolt,
			ArrowBlock.ArrowType.ExplosiveBolt
		};
	}
}
