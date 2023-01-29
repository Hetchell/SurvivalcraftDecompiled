using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FC RID: 508
	public class ComponentPlayer : ComponentCreature, IUpdateable
	{
		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x00073D4E File Offset: 0x00071F4E
		// (set) Token: 0x06000F26 RID: 3878 RVA: 0x00073D56 File Offset: 0x00071F56
		public PlayerData PlayerData { get; set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000F27 RID: 3879 RVA: 0x00073D5F File Offset: 0x00071F5F
		public GameWidget GameWidget
		{
			get
			{
				return this.PlayerData.GameWidget;
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000F28 RID: 3880 RVA: 0x00073D6C File Offset: 0x00071F6C
		public ContainerWidget GuiWidget
		{
			get
			{
				return this.PlayerData.GameWidget.GuiWidget;
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x06000F29 RID: 3881 RVA: 0x00073D7E File Offset: 0x00071F7E
		public ViewWidget ViewWidget
		{
			get
			{
				return this.PlayerData.GameWidget.ViewWidget;
			}
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x06000F2A RID: 3882 RVA: 0x00073D90 File Offset: 0x00071F90
		// (set) Token: 0x06000F2B RID: 3883 RVA: 0x00073D98 File Offset: 0x00071F98
		public ComponentGui ComponentGui { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x00073DA1 File Offset: 0x00071FA1
		// (set) Token: 0x06000F2D RID: 3885 RVA: 0x00073DA9 File Offset: 0x00071FA9
		public ComponentInput ComponentInput { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x00073DB2 File Offset: 0x00071FB2
		// (set) Token: 0x06000F2F RID: 3887 RVA: 0x00073DBA File Offset: 0x00071FBA
		public ComponentBlockHighlight ComponentBlockHighlight { get; set; }

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x06000F30 RID: 3888 RVA: 0x00073DC3 File Offset: 0x00071FC3
		// (set) Token: 0x06000F31 RID: 3889 RVA: 0x00073DCB File Offset: 0x00071FCB
		public ComponentScreenOverlays ComponentScreenOverlays { get; set; }

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x00073DD4 File Offset: 0x00071FD4
		// (set) Token: 0x06000F33 RID: 3891 RVA: 0x00073DDC File Offset: 0x00071FDC
		public ComponentAimingSights ComponentAimingSights { get; set; }

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06000F34 RID: 3892 RVA: 0x00073DE5 File Offset: 0x00071FE5
		// (set) Token: 0x06000F35 RID: 3893 RVA: 0x00073DED File Offset: 0x00071FED
		public ComponentMiner ComponentMiner { get; set; }

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000F36 RID: 3894 RVA: 0x00073DF6 File Offset: 0x00071FF6
		// (set) Token: 0x06000F37 RID: 3895 RVA: 0x00073DFE File Offset: 0x00071FFE
		public ComponentRider ComponentRider { get; set; }

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000F38 RID: 3896 RVA: 0x00073E07 File Offset: 0x00072007
		// (set) Token: 0x06000F39 RID: 3897 RVA: 0x00073E0F File Offset: 0x0007200F
		public ComponentSleep ComponentSleep { get; set; }

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000F3A RID: 3898 RVA: 0x00073E18 File Offset: 0x00072018
		// (set) Token: 0x06000F3B RID: 3899 RVA: 0x00073E20 File Offset: 0x00072020
		public ComponentVitalStats ComponentVitalStats { get; set; }

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x06000F3C RID: 3900 RVA: 0x00073E29 File Offset: 0x00072029
		// (set) Token: 0x06000F3D RID: 3901 RVA: 0x00073E31 File Offset: 0x00072031
		public ComponentSickness ComponentSickness { get; set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x06000F3E RID: 3902 RVA: 0x00073E3A File Offset: 0x0007203A
		// (set) Token: 0x06000F3F RID: 3903 RVA: 0x00073E42 File Offset: 0x00072042
		public ComponentFlu ComponentFlu { get; set; }

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06000F40 RID: 3904 RVA: 0x00073E4B File Offset: 0x0007204B
		// (set) Token: 0x06000F41 RID: 3905 RVA: 0x00073E53 File Offset: 0x00072053
		public ComponentLevel ComponentLevel { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06000F42 RID: 3906 RVA: 0x00073E5C File Offset: 0x0007205C
		// (set) Token: 0x06000F43 RID: 3907 RVA: 0x00073E64 File Offset: 0x00072064
		public ComponentClothing ComponentClothing { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000F44 RID: 3908 RVA: 0x00073E6D File Offset: 0x0007206D
		// (set) Token: 0x06000F45 RID: 3909 RVA: 0x00073E75 File Offset: 0x00072075
		public ComponentOuterClothingModel ComponentOuterClothingModel { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000F46 RID: 3910 RVA: 0x00073E7E File Offset: 0x0007207E
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x00073E84 File Offset: 0x00072084
		public void Update(float dt)
		{
            if (!ComponentInput.noclipState)
			{
				Block.m_defaultCollisionBoxes = new BoundingBox[]
					{
						new BoundingBox(Vector3.Zero, Vector3.One)
					};
            }
            PlayerInput playerInput = this.ComponentInput.PlayerInput;
			if (this.ComponentInput.IsControlledByTouch && this.m_aim != null)
			{
				playerInput.Look = Vector2.Zero;
			}
			if (this.ComponentMiner.Inventory != null)
			{
				this.ComponentMiner.Inventory.ActiveSlotIndex += playerInput.ScrollInventory;
				if (playerInput.SelectInventorySlot != null)
				{
					this.ComponentMiner.Inventory.ActiveSlotIndex = MathUtils.Clamp(playerInput.SelectInventorySlot.Value, 0, 9);
				}
			}
			ComponentSteedBehavior componentSteedBehavior = null;
			ComponentBoat componentBoat = null;
			ComponentMount mount = this.ComponentRider.Mount;
			if (mount != null)
			{
				componentSteedBehavior = mount.Entity.FindComponent<ComponentSteedBehavior>();
				componentBoat = mount.Entity.FindComponent<ComponentBoat>();
			}
			if (componentSteedBehavior != null)
			{
				if (playerInput.Move.Z > 0.5f && !this.m_speedOrderBlocked)
				{
					if (this.PlayerData.PlayerClass == PlayerClass.Male)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellFast", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					else
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellFast", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = 1;
					this.m_speedOrderBlocked = true;
				}
				else if (playerInput.Move.Z < -0.5f && !this.m_speedOrderBlocked)
				{
					if (this.PlayerData.PlayerClass == PlayerClass.Male)
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/MaleYellSlow", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					else
					{
						this.m_subsystemAudio.PlayRandomSound("Audio/Creatures/FemaleYellSlow", 0.75f, 0f, base.ComponentBody.Position, 2f, false);
					}
					componentSteedBehavior.SpeedOrder = -1;
					this.m_speedOrderBlocked = true;
				}
				else if (MathUtils.Abs(playerInput.Move.Z) <= 0.25f)
				{
					this.m_speedOrderBlocked = false;
				}
				componentSteedBehavior.TurnOrder = playerInput.Move.X;
				componentSteedBehavior.JumpOrder = (float)(playerInput.Jump ? 1 : 0);
				base.ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
			}
			else if (componentBoat != null)
			{
				componentBoat.TurnOrder = playerInput.Move.X;
				componentBoat.MoveOrder = playerInput.Move.Z;
				base.ComponentLocomotion.LookOrder = new Vector2(playerInput.Look.X, 0f);
				base.ComponentCreatureModel.RowLeftOrder = (playerInput.Move.X < -0.2f || playerInput.Move.Z > 0.2f);
				base.ComponentCreatureModel.RowRightOrder = (playerInput.Move.X > 0.2f || playerInput.Move.Z > 0.2f);
			}
			else
			{
				base.ComponentLocomotion.WalkOrder = new Vector2?(base.ComponentBody.IsSneaking ? (0.66f * new Vector2(playerInput.SneakMove.X, playerInput.SneakMove.Z)) : new Vector2(playerInput.Move.X, playerInput.Move.Z));
				base.ComponentLocomotion.FlyOrder = new Vector3?(new Vector3(0f, playerInput.Move.Y, 0f));
				base.ComponentLocomotion.TurnOrder = playerInput.Look * new Vector2(1f, 0f);
				base.ComponentLocomotion.JumpOrder = MathUtils.Max((float)(playerInput.Jump ? 1 : 0), base.ComponentLocomotion.JumpOrder);
			}
			base.ComponentLocomotion.LookOrder += playerInput.Look * (SettingsManager.FlipVerticalAxis ? new Vector2(0f, -1f) : new Vector2(0f, 1f));
			base.ComponentLocomotion.VrLookOrder = playerInput.VrLook;
			base.ComponentLocomotion.VrMoveOrder = playerInput.VrMove;
			int num = Terrain.ExtractContents(this.ComponentMiner.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			bool flag = false;
			if (playerInput.Interact != null && !flag && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				if (!this.ComponentMiner.Use(playerInput.Interact.Value))
				{
					TerrainRaycastResult? terrainRaycastResult = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.Interact.Value, RaycastMode.Interaction, true, true, true);
					if (terrainRaycastResult != null)
					{
						if (!this.ComponentMiner.Interact(terrainRaycastResult.Value))
						{
							//this thing here will cause the clipping bug. The place method. 
							if (this.ComponentMiner.Place(terrainRaycastResult.Value))
							//if(true)
							{
								this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
								flag = true;
								this.m_isAimBlocked = true;
							}
						}
						else
						{
							this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
							flag = true;
							this.m_isAimBlocked = true;
						}
					}
				}
				else
				{
					this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
					flag = true;
					this.m_isAimBlocked = true;
				}
			}
			float num2 = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? 0.1f : 1.4f;
			if (playerInput.Aim != null && block.IsAimable && this.m_subsystemTime.GameTime - this.m_lastActionTime > (double)num2)
			{
				if (!this.m_isAimBlocked)
				{
					Ray3 value = playerInput.Aim.Value;
					Vector3 vector = this.GameWidget.ActiveCamera.WorldToScreen(value.Position + value.Direction, Matrix.Identity);
					Point2 size = Window.Size;
					if (this.ComponentInput.IsControlledByVr || (vector.X >= (float)size.X * 0.02f && vector.X < (float)size.X * 0.98f && vector.Y >= (float)size.Y * 0.02f && vector.Y < (float)size.Y * 0.98f))
					{
						this.m_aim = new Ray3?(value);
						if (this.ComponentMiner.Aim(value, AimState.InProgress))
						{
							this.ComponentMiner.Aim(this.m_aim.Value, AimState.Cancelled);
							this.m_aim = null;
							this.m_isAimBlocked = true;
						}
						else if (!this.m_aimHintIssued && Time.PeriodicEvent(1.0, 0.0))
						{
							Time.QueueTimeDelayedExecution(Time.RealTime + 3.0, delegate
							{
								if (!this.m_aimHintIssued && this.m_aim != null && !base.ComponentBody.IsSneaking)
								{
									this.m_aimHintIssued = true;
									this.ComponentGui.DisplaySmallMessage(LanguageControl.Get(ComponentPlayer.fName, 1), Color.White, true, true);
								}
							});
						}
					}
					else if (this.m_aim != null)
					{
						this.ComponentMiner.Aim(this.m_aim.Value, AimState.Cancelled);
						this.m_aim = null;
						this.m_isAimBlocked = true;
					}
				}
			}
			else
			{
				this.m_isAimBlocked = false;
				if (this.m_aim != null)
				{
					this.ComponentMiner.Aim(this.m_aim.Value, AimState.Completed);
					this.m_aim = null;
					this.m_lastActionTime = this.m_subsystemTime.GameTime;
				}
			}
			flag |= (this.m_aim != null);
			if (playerInput.Hit != null && !flag && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				BodyRaycastResult? bodyRaycastResult = this.ComponentMiner.Raycast<BodyRaycastResult>(playerInput.Hit.Value, RaycastMode.Interaction, true, true, true);
				if (bodyRaycastResult != null)
				{
					flag = true;
					this.m_isDigBlocked = true;
					if (Vector3.Distance(bodyRaycastResult.Value.HitPoint(), base.ComponentCreatureModel.EyePosition) <= 2f)
					{
						this.ComponentMiner.Hit(bodyRaycastResult.Value.ComponentBody, bodyRaycastResult.Value.HitPoint(), playerInput.Hit.Value.Direction);
					}
				}
			}
			if (playerInput.Dig != null && !flag && !this.m_isDigBlocked && this.m_subsystemTime.GameTime - this.m_lastActionTime > 0.33000001311302185)
			{
				TerrainRaycastResult? terrainRaycastResult2 = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.Dig.Value, RaycastMode.Digging, true, true, true);
				if (terrainRaycastResult2 != null && this.ComponentMiner.Dig(terrainRaycastResult2.Value))
				{
					this.m_lastActionTime = this.m_subsystemTime.GameTime;
					this.m_subsystemTerrain.TerrainUpdater.RequestSynchronousUpdate();
				}
			}
			if (playerInput.Dig == null)
			{
				this.m_isDigBlocked = false;
			}
			if (playerInput.Drop && this.ComponentMiner.Inventory != null)
			{
				IInventory inventory = this.ComponentMiner.Inventory;
				int slotValue = inventory.GetSlotValue(inventory.ActiveSlotIndex);
				int slotCount = inventory.GetSlotCount(inventory.ActiveSlotIndex);
				int num3 = inventory.RemoveSlotItems(inventory.ActiveSlotIndex, slotCount);
				if (slotValue != 0 && num3 != 0)
				{
					Vector3 position = base.ComponentBody.Position + new Vector3(0f, base.ComponentBody.BoxSize.Y * 0.66f, 0f) + 0.25f * base.ComponentBody.Matrix.Forward;
					Vector3 value2 = 8f * Matrix.CreateFromQuaternion(base.ComponentCreatureModel.EyeRotation).Forward;
					this.m_subsystemPickables.AddPickable(slotValue, num3, position, new Vector3?(value2), null);
				}
			}
			if (playerInput.PickBlockType == null || flag)
			{
				return;
			}
			ComponentCreativeInventory componentCreativeInventory = this.ComponentMiner.Inventory as ComponentCreativeInventory;
			if (componentCreativeInventory == null)
			{
				return;
			}
			TerrainRaycastResult? terrainRaycastResult3 = this.ComponentMiner.Raycast<TerrainRaycastResult>(playerInput.PickBlockType.Value, RaycastMode.Digging, true, false, false);
			if (terrainRaycastResult3 == null)
			{
				return;
			}
			int num4 = terrainRaycastResult3.Value.Value;
			num4 = Terrain.ReplaceLight(num4, 0);
			int num5 = Terrain.ExtractContents(num4);
			Block block2 = BlocksManager.Blocks[num5];
			int num6 = 0;
			IEnumerable<int> creativeValues = block2.GetCreativeValues();
			if (block2.GetCreativeValues().Contains(num4))
			{
				num6 = num4;
			}
			if (num6 == 0 && !block2.IsNonDuplicable)
			{
				List<BlockDropValue> list = new List<BlockDropValue>();
				bool flag2;
				block2.GetDropValues(this.m_subsystemTerrain, num4, 0, int.MaxValue, list, out flag2);
				if (list.Count > 0 && list[0].Count > 0)
				{
					num6 = list[0].Value;
				}
			}
			if (num6 == 0)
			{
				num6 = creativeValues.FirstOrDefault<int>();
			}
			if (num6 == 0)
			{
				return;
			}
			int num7 = -1;
			for (int i = 0; i < 10; i++)
			{
				if (componentCreativeInventory.GetSlotCapacity(i, num6) > 0 && componentCreativeInventory.GetSlotCount(i) > 0 && componentCreativeInventory.GetSlotValue(i) == num6)
				{
					num7 = i;
					break;
				}
			}
			if (num7 < 0)
			{
				for (int j = 0; j < 10; j++)
				{
					if (componentCreativeInventory.GetSlotCapacity(j, num6) > 0 && (componentCreativeInventory.GetSlotCount(j) == 0 || componentCreativeInventory.GetSlotValue(j) == 0))
					{
						num7 = j;
						break;
					}
				}
			}
			if (num7 < 0)
			{
				num7 = componentCreativeInventory.ActiveSlotIndex;
			}
			componentCreativeInventory.RemoveSlotItems(num7, int.MaxValue);
			componentCreativeInventory.AddSlotItems(num7, num6, 1);
			componentCreativeInventory.ActiveSlotIndex = num7;
			this.ComponentGui.DisplaySmallMessage(block2.GetDisplayName(this.m_subsystemTerrain, num4), Color.White, false, false);
			this.m_subsystemAudio.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f, 0f);
		}

        // Token: 0x06000F48 RID: 3912 RVA: 0x00074A94 File Offset: 0x00072C94
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.ComponentGui = base.Entity.FindComponent<ComponentGui>(true);
			this.ComponentInput = base.Entity.FindComponent<ComponentInput>(true);
			this.ComponentScreenOverlays = base.Entity.FindComponent<ComponentScreenOverlays>(true);
			this.ComponentBlockHighlight = base.Entity.FindComponent<ComponentBlockHighlight>(true);
			this.ComponentAimingSights = base.Entity.FindComponent<ComponentAimingSights>(true);
			this.ComponentMiner = base.Entity.FindComponent<ComponentMiner>(true);
			this.ComponentRider = base.Entity.FindComponent<ComponentRider>(true);
			this.ComponentSleep = base.Entity.FindComponent<ComponentSleep>(true);
			this.ComponentVitalStats = base.Entity.FindComponent<ComponentVitalStats>(true);
			this.ComponentSickness = base.Entity.FindComponent<ComponentSickness>(true);
			this.ComponentFlu = base.Entity.FindComponent<ComponentFlu>(true);
			this.ComponentLevel = base.Entity.FindComponent<ComponentLevel>(true);
			this.ComponentClothing = base.Entity.FindComponent<ComponentClothing>(true);
			this.ComponentOuterClothingModel = base.Entity.FindComponent<ComponentOuterClothingModel>(true);
			int playerIndex = valuesDictionary.GetValue<int>("PlayerIndex");
			this.PlayerData = base.Project.FindSubsystem<SubsystemPlayers>(true).PlayersData.First((PlayerData d) => d.PlayerIndex == playerIndex);
		}

        // Token: 0x06000F49 RID: 3913 RVA: 0x00074C43 File Offset: 0x00072E43
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			valuesDictionary.SetValue<int>("PlayerIndex", this.PlayerData.PlayerIndex);
		}

		// Token: 0x040009AC RID: 2476
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040009AD RID: 2477
		public SubsystemTime m_subsystemTime;

		// Token: 0x040009AE RID: 2478
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040009AF RID: 2479
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x040009B0 RID: 2480
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009B1 RID: 2481
		public bool m_aimHintIssued;

		// Token: 0x040009B2 RID: 2482
		public static string fName = "ComponentPlayer";

		// Token: 0x040009B3 RID: 2483
		public double m_lastActionTime;

		// Token: 0x040009B4 RID: 2484
		public bool m_speedOrderBlocked;

		// Token: 0x040009B5 RID: 2485
		public Ray3? m_aim;

		// Token: 0x040009B6 RID: 2486
		public bool m_isAimBlocked;

		// Token: 0x040009B7 RID: 2487
		public bool m_isDigBlocked;
	}
}
