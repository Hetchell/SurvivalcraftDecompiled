using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F3 RID: 499
	public class ComponentMiner : Component, IUpdateable
	{
		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000E78 RID: 3704 RVA: 0x0006FBDA File Offset: 0x0006DDDA
		// (set) Token: 0x06000E79 RID: 3705 RVA: 0x0006FBE2 File Offset: 0x0006DDE2
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x0006FBEB File Offset: 0x0006DDEB
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x0006FBF3 File Offset: 0x0006DDF3
		public ComponentPlayer ComponentPlayer { get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x0006FBFC File Offset: 0x0006DDFC
		// (set) Token: 0x06000E7D RID: 3709 RVA: 0x0006FC04 File Offset: 0x0006DE04
		public IInventory Inventory { get; set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x0006FC0D File Offset: 0x0006DE0D
		public int ActiveBlockValue
		{
			get
			{
				if (this.Inventory == null)
				{
					return 0;
				}
				return this.Inventory.GetSlotValue(this.Inventory.ActiveSlotIndex);
			}
		}

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000E7F RID: 3711 RVA: 0x0006FC2F File Offset: 0x0006DE2F
		// (set) Token: 0x06000E80 RID: 3712 RVA: 0x0006FC37 File Offset: 0x0006DE37
		public float AttackPower { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000E81 RID: 3713 RVA: 0x0006FC40 File Offset: 0x0006DE40
		// (set) Token: 0x06000E82 RID: 3714 RVA: 0x0006FC48 File Offset: 0x0006DE48
		public float PokingPhase { get; set; }

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000E83 RID: 3715 RVA: 0x0006FC51 File Offset: 0x0006DE51
		// (set) Token: 0x06000E84 RID: 3716 RVA: 0x0006FC59 File Offset: 0x0006DE59
		public CellFace? DigCellFace { get; set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000E85 RID: 3717 RVA: 0x0006FC64 File Offset: 0x0006DE64
		public float DigTime
		{
			get
			{
				if (this.DigCellFace == null)
				{
					return 0f;
				}
				return (float)(this.m_subsystemTime.GameTime - this.m_digStartTime);
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0006FC9C File Offset: 0x0006DE9C
		public float DigProgress
		{
			get
			{
				if (this.DigCellFace == null)
				{
					return 0f;
				}
				return this.m_digProgress;
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000E87 RID: 3719 RVA: 0x0006FCC5 File Offset: 0x0006DEC5
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x0006FCC8 File Offset: 0x0006DEC8
		public void Poke(bool forceRestart)
		{
			if (ComponentMiner.Poke1 != null)
			{
				ComponentMiner.Poke1(forceRestart);
				return;
			}
			if (forceRestart)
			{
				this.PokingPhase = 0.0001f;
				return;
			}
			this.PokingPhase = MathUtils.Max(0.0001f, this.PokingPhase);
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x0006FD04 File Offset: 0x0006DF04
		public bool Dig(TerrainRaycastResult raycastResult)
		{
			if (ComponentMiner.Dig1 != null)
			{
				return ComponentMiner.Dig1(raycastResult);
			}
			bool result = false;
			this.m_lastDigFrameIndex = Time.FrameIndex;
			CellFace cellFace = raycastResult.CellFace;
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int activeBlockValue = this.ActiveBlockValue;
			int num2 = Terrain.ExtractContents(activeBlockValue);
			Block block2 = BlocksManager.Blocks[num2];
			if (this.DigCellFace == null || this.DigCellFace.Value.X != cellFace.X || this.DigCellFace.Value.Y != cellFace.Y || this.DigCellFace.Value.Z != cellFace.Z)
			{
				this.m_digStartTime = this.m_subsystemTime.GameTime;
				this.DigCellFace = new CellFace?(cellFace);
			}
			float num3 = this.CalculateDigTime(cellValue, num2);
			this.m_digProgress = ((num3 > 0f) ? MathUtils.Saturate((float)(this.m_subsystemTime.GameTime - this.m_digStartTime) / num3) : 1f);
			if (!this.CanUseTool(activeBlockValue))
			{
				this.m_digProgress = 0f;
				if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, this.m_digStartTime + 1.0))
				{
					ComponentPlayer componentPlayer = this.ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block2.PlayerLevelRequired, block2.GetDisplayName(this.m_subsystemTerrain, activeBlockValue)), Color.White, true, true);
					}
				}
			}
			bool flag = this.ComponentPlayer != null && !this.ComponentPlayer.ComponentInput.IsControlledByTouch && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative;
			if (flag || (this.m_lastPokingPhase <= 0.5f && this.PokingPhase > 0.5f))
			{
				if (this.m_digProgress >= 1f)
				{
					this.DigCellFace = null;
					if (flag)
					{
						this.Poke(true);
					}
					BlockPlacementData digValue = block.GetDigValue(this.m_subsystemTerrain, this, cellValue, activeBlockValue, raycastResult);
					this.m_subsystemTerrain.DestroyCell(block2.ToolLevel, digValue.CellFace.X, digValue.CellFace.Y, digValue.CellFace.Z, digValue.Value, false, false);
					this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 2f);
					this.DamageActiveTool(1);
					if (this.ComponentCreature.PlayerStats != null)
					{
						this.ComponentCreature.PlayerStats.BlocksDug += 1L;
					}
					result = true;
				}
				else
				{
					this.m_subsystemSoundMaterials.PlayImpactSound(cellValue, new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z), 1f);
					BlockDebrisParticleSystem particleSystem = block.CreateDebrisParticleSystem(this.m_subsystemTerrain, raycastResult.HitPoint(0.1f), cellValue, 0.35f);
					base.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
				}
			}
			return result;
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x00070058 File Offset: 0x0006E258
		/*
		 * Must find out why this method will cause the game to break. Solid blocks will become transparent suddenly, for all entities. 
		 */
		public bool Place(TerrainRaycastResult raycastResult)
		{
			return true;
			if (ComponentMiner.Place1 != null)
			{
				return ComponentMiner.Place1(raycastResult);
			}
			if (this.Place(raycastResult, this.ActiveBlockValue))
			{
				if (this.Inventory != null)
				{
					this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, 1);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x000700AC File Offset: 0x0006E2AC
		public bool Place(TerrainRaycastResult raycastResult, int value)
		{
			if (ComponentMiner.Place2 != null)
			{
				return ComponentMiner.Place2(raycastResult, value);
			}
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num].IsPlaceable)
			{
				Block block = BlocksManager.Blocks[num];
				BlockPlacementData placementValue = block.GetPlacementValue(this.m_subsystemTerrain, this, value, raycastResult);
				if (placementValue.Value != 0)
				{
					Point3 point = CellFace.FaceToPoint3(placementValue.CellFace.Face);
					int num2 = placementValue.CellFace.X + point.X;
					int num3 = placementValue.CellFace.Y + point.Y;
					int num4 = placementValue.CellFace.Z + point.Z;
					if (num3 > 0 && num3 < 255 && (ComponentMiner.IsBlockPlacingAllowed(this.ComponentCreature.ComponentBody) || this.m_subsystemGameInfo.WorldSettings.GameMode <= GameMode.Harmless))
					{
						bool flag = false;
						if (block.IsCollidable)
						{
							BoundingBox boundingBox = this.ComponentCreature.ComponentBody.BoundingBox;
							boundingBox.Min += new Vector3(0.2f);
							boundingBox.Max -= new Vector3(0.2f);
							BoundingBox[] box = block.GetCustomCollisionBoxes(this.m_subsystemTerrain, placementValue.Value);
							for (int i = 0; i < box.Length; i++)
							{
								box[i].Min += new Vector3((float)num2, (float)num3, (float)num4);
								box[i].Max += new Vector3((float)num2, (float)num3, (float)num4);
								if (boundingBox.Intersection(box[i]))
								{
									flag = true;
									break;
								}
							}
							//foreach (BoundingBox box1 in box)
							//{
							//	box1.Min += new Vector3((float)num2, (float)num3, (float)num4);
							//	box1.Max += new Vector3((float)num2, (float)num3, (float)num4);
							//	if (boundingBox.Intersection(box1))
							//	{
							//		flag = true;
							//		break;
							//	}
							//}
						}
						if (!flag)
						{
							SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(Terrain.ExtractContents(placementValue.Value));
							for (int j = 0; j < blockBehaviors.Length; j++)
							{
								blockBehaviors[j].OnItemPlaced(num2, num3, num4, ref placementValue, value);
							}
							//this.m_subsystemTerrain.DestroyCell(0, num2, num3, num4, placementValue.Value, false, false);
							this.m_subsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, new Vector3((float)placementValue.CellFace.X, (float)placementValue.CellFace.Y, (float)placementValue.CellFace.Z), 5f, false);
							this.Poke(false);
							if (this.ComponentCreature.PlayerStats != null)
							{
								this.ComponentCreature.PlayerStats.BlocksPlaced += 1L;
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x00070358 File Offset: 0x0006E558
		public bool Use(Ray3 ray)
		{
			if (ComponentMiner.Use1 != null)
			{
				return ComponentMiner.Use1(ray);
			}
			int num = Terrain.ExtractContents(this.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			if (!this.CanUseTool(this.ActiveBlockValue))
			{
				ComponentPlayer componentPlayer = this.ComponentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
				}
				this.Poke(false);
				return false;
			}
			SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num);
			for (int i = 0; i < blockBehaviors.Length; i++)
			{
				if (blockBehaviors[i].OnUse(ray, this))
				{
					this.Poke(false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00070420 File Offset: 0x0006E620
		public bool Interact(TerrainRaycastResult raycastResult)
		{
			if (ComponentMiner.Interact1 != null)
			{
				return ComponentMiner.Interact1(raycastResult);
			}
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z);
			SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(cellContents);
			for (int i = 0; i < blockBehaviors.Length; i++)
			{
				if (blockBehaviors[i].OnInteract(raycastResult, this))
				{
					if (this.ComponentCreature.PlayerStats != null)
					{
						this.ComponentCreature.PlayerStats.BlocksInteracted += 1L;
					}
					this.Poke(false);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x000704CC File Offset: 0x0006E6CC
		public void Hit(ComponentBody componentBody, Vector3 hitPoint, Vector3 hitDirection)
		{
			if (ComponentMiner.Hit1 != null)
			{
				ComponentMiner.Hit1(componentBody, hitPoint, hitDirection);
				return;
			}
			if (this.m_subsystemTime.GameTime - this.m_lastHitTime <= 0.6600000262260437)
			{
				return;
			}
			this.m_lastHitTime = this.m_subsystemTime.GameTime;
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(this.ActiveBlockValue)];
			if (!this.CanUseTool(this.ActiveBlockValue))
			{
				ComponentPlayer componentPlayer = this.ComponentPlayer;
				if (componentPlayer != null)
				{
					componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
				}
				this.Poke(false);
				return;
			}
			float num;
			float probability;
			if (this.ActiveBlockValue != 0)
			{
				num = block.GetMeleePower(this.ActiveBlockValue) * this.AttackPower * this.m_random.Float(0.8f, 1.2f);
				probability = block.GetMeleeHitProbability(this.ActiveBlockValue);
			}
			else
			{
				num = this.AttackPower * this.m_random.Float(0.8f, 1.2f);
				probability = 0.66f;
			}
			bool flag;
			if (this.ComponentPlayer != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/Swoosh", 1f, this.m_random.Float(-0.2f, 0.2f), componentBody.Position, 3f, false);
				flag = this.m_random.Bool(probability);
				num *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				ComponentMiner.AttackBody(componentBody, this.ComponentCreature, hitPoint, hitDirection, num, true);
				this.DamageActiveTool(1);
			}
			else if (this.ComponentCreature is ComponentPlayer)
			{
				HitValueParticleSystem particleSystem = new HitValueParticleSystem(hitPoint + 0.75f * hitDirection, 1f * hitDirection + this.ComponentCreature.ComponentBody.Velocity, Color.White, LanguageControl.Get(ComponentMiner.fName, 2));
				base.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
			}
			if (this.ComponentCreature.PlayerStats != null)
			{
				this.ComponentCreature.PlayerStats.MeleeAttacks += 1L;
				if (flag)
				{
					this.ComponentCreature.PlayerStats.MeleeHits += 1L;
				}
			}
			this.Poke(false);
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00070730 File Offset: 0x0006E930
		public bool Aim(Ray3 aim, AimState state)
		{
			int num = Terrain.ExtractContents(this.ActiveBlockValue);
			Block block = BlocksManager.Blocks[num];
			if (block.IsAimable)
			{
				if (!this.CanUseTool(this.ActiveBlockValue))
				{
					ComponentPlayer componentPlayer = this.ComponentPlayer;
					if (componentPlayer != null)
					{
						componentPlayer.ComponentGui.DisplaySmallMessage(string.Format(LanguageControl.Get(ComponentMiner.fName, 1), block.PlayerLevelRequired, block.GetDisplayName(this.m_subsystemTerrain, this.ActiveBlockValue)), Color.White, true, true);
					}
					this.Poke(false);
					return true;
				}
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num);
				for (int i = 0; i < blockBehaviors.Length; i++)
				{
					if (blockBehaviors[i].OnAim(aim, this, state))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x000707E8 File Offset: 0x0006E9E8
		public object Raycast(Ray3 ray, RaycastMode mode, bool raycastTerrain = true, bool raycastBodies = true, bool raycastMovingBlocks = true)
		{
			float reach = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? SettingsManager.CreativeReach : 5f;
			Vector3 creaturePosition = this.ComponentCreature.ComponentCreatureModel.EyePosition;
			Vector3 start = ray.Position;
			Vector3 direction = Vector3.Normalize(ray.Direction);
			Vector3 end = ray.Position + direction * 15f;
			Point3 startCell = Terrain.ToCell(start);
			BodyRaycastResult? bodyRaycastResult = this.m_subsystemBodies.Raycast(start, end, 0.35f, (ComponentBody body, float distance) => Vector3.DistanceSquared(start + distance * direction, creaturePosition) <= reach * reach && body.Entity != this.Entity && !body.IsChildOfBody(this.ComponentCreature.ComponentBody) && !this.ComponentCreature.ComponentBody.IsChildOfBody(body) && Vector3.Dot(Vector3.Normalize(body.BoundingBox.Center() - start), direction) > 0.7f);
			MovingBlocksRaycastResult? movingBlocksRaycastResult = this.m_subsystemMovingBlocks.Raycast(start, end, true);
			TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(start, end, true, true, delegate(int value, float distance)
			{
				if (Vector3.DistanceSquared(start + distance * direction, creaturePosition) <= reach * reach)
				{
					Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
					if (distance == 0f && block is CrossBlock && Vector3.Dot(direction, new Vector3(startCell) + new Vector3(0.5f) - start) < 0f)
					{
						return false;
					}
					if (mode == RaycastMode.Digging)
					{
						return !block.IsDiggingTransparent;
					}
					if (mode == RaycastMode.Interaction)
					{
						return !block.IsPlacementTransparent || block.IsInteractive(this.m_subsystemTerrain, value);
					}
					if (mode == RaycastMode.Gathering)
					{
						return block.IsGatherable;
					}
				}
				return false;
			});
			float num = (bodyRaycastResult != null) ? bodyRaycastResult.Value.Distance : float.PositiveInfinity;
			float num2 = (movingBlocksRaycastResult != null) ? movingBlocksRaycastResult.Value.Distance : float.PositiveInfinity;
			float num3 = (terrainRaycastResult != null) ? terrainRaycastResult.Value.Distance : float.PositiveInfinity;
			if (num < num2 && num < num3)
			{
				return bodyRaycastResult.Value;
			}
			if (num2 < num && num2 < num3)
			{
				return movingBlocksRaycastResult.Value;
			}
			if (num3 < num && num3 < num2)
			{
				return terrainRaycastResult.Value;
			}
			return new Ray3(start, direction);
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x000709A4 File Offset: 0x0006EBA4
		public T? Raycast<T>(Ray3 ray, RaycastMode mode, bool raycastTerrain = true, bool raycastBodies = true, bool raycastMovingBlocks = true) where T : struct
		{
			object obj = this.Raycast(ray, mode, raycastTerrain, raycastBodies, raycastMovingBlocks);
			if (!(obj is T))
			{
				return null;
			}
			return new T?((T)((object)obj));
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x000709DC File Offset: 0x0006EBDC
		public void RemoveActiveTool(int removeCount)
		{
			if (this.Inventory != null)
			{
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, removeCount);
			}
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00070A00 File Offset: 0x0006EC00
		public void DamageActiveTool(int damageCount)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative || this.Inventory == null)
			{
				return;
			}
			int num = BlocksManager.DamageItem(this.ActiveBlockValue, damageCount);
			if (num != 0)
			{
				int slotCount = this.Inventory.GetSlotCount(this.Inventory.ActiveSlotIndex);
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, slotCount);
				if (this.Inventory.GetSlotCount(this.Inventory.ActiveSlotIndex) == 0)
				{
					this.Inventory.AddSlotItems(this.Inventory.ActiveSlotIndex, num, slotCount);
					return;
				}
			}
			else
			{
				this.Inventory.RemoveSlotItems(this.Inventory.ActiveSlotIndex, 1);
			}
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x00070AB0 File Offset: 0x0006ECB0
		public static void AttackBody(ComponentBody target, ComponentCreature attacker, Vector3 hitPoint, Vector3 hitDirection, float attackPower, bool isMeleeAttack)
		{
			if (attacker != null && attacker is ComponentPlayer && target.Entity.FindComponent<ComponentPlayer>() != null && !target.Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings.IsFriendlyFireEnabled)
			{
				attacker.Entity.FindComponent<ComponentGui>(true).DisplaySmallMessage(LanguageControl.Get(ComponentMiner.fName, 3), Color.White, true, true);
				return;
			}
			if (attackPower > 0f)
			{
				ComponentClothing componentClothing = target.Entity.FindComponent<ComponentClothing>();
				if (componentClothing != null)
				{
					attackPower = componentClothing.ApplyArmorProtection(attackPower);
				}
				ComponentLevel componentLevel = target.Entity.FindComponent<ComponentLevel>();
				if (componentLevel != null)
				{
					attackPower /= componentLevel.ResilienceFactor;
				}
				ComponentHealth componentHealth = target.Entity.FindComponent<ComponentHealth>();
				if (componentHealth != null)
				{
					float num = attackPower / componentHealth.AttackResilience;
					string cause;
					if (attacker != null)
					{
						string key = attacker.KillVerbs[ComponentMiner.s_random.Int(0, attacker.KillVerbs.Count - 1)];
						string displayName = attacker.DisplayName;
						cause = string.Format(LanguageControl.Get(ComponentMiner.fName, 4), displayName, LanguageControl.Get(ComponentMiner.fName, key));
					}
					else
					{
						switch (ComponentMiner.s_random.Int(0, 5))
						{
						case 0:
							cause = LanguageControl.Get(ComponentMiner.fName, 5);
							break;
						case 1:
							cause = LanguageControl.Get(ComponentMiner.fName, 6);
							break;
						case 2:
							cause = LanguageControl.Get(ComponentMiner.fName, 7);
							break;
						case 3:
							cause = LanguageControl.Get(ComponentMiner.fName, 8);
							break;
						case 4:
							cause = LanguageControl.Get(ComponentMiner.fName, 9);
							break;
						default:
							cause = LanguageControl.Get(ComponentMiner.fName, 10);
							break;
						}
					}
					float health = componentHealth.Health;
					componentHealth.Injure(num, attacker, false, cause);
					if (num > 0f)
					{
						target.Project.FindSubsystem<SubsystemAudio>(true).PlayRandomSound("Audio/Impacts/Body", 1f, ComponentMiner.s_random.Float(-0.3f, 0.3f), target.Position, 4f, false);
						float num2 = (health - componentHealth.Health) * componentHealth.AttackResilience;
						if (attacker is ComponentPlayer && num2 > 0f)
						{
							string text = (0f - num2).ToString("0", CultureInfo.InvariantCulture);
							HitValueParticleSystem particleSystem = new HitValueParticleSystem(hitPoint + 0.75f * hitDirection, 1f * hitDirection + attacker.ComponentBody.Velocity, Color.White, text);
							target.Project.FindSubsystem<SubsystemParticles>(true).AddParticleSystem(particleSystem);
						}
					}
				}
				ComponentDamage componentDamage = target.Entity.FindComponent<ComponentDamage>();
				if (componentDamage != null)
				{
					float num3 = attackPower / componentDamage.AttackResilience;
					componentDamage.Damage(num3);
					if (num3 > 0f)
					{
						target.Project.FindSubsystem<SubsystemAudio>(true).PlayRandomSound(componentDamage.DamageSoundName, 1f, ComponentMiner.s_random.Float(-0.3f, 0.3f), target.Position, 4f, false);
					}
				}
			}
			float num4 = 0f;
			float x = 0f;
			if (isMeleeAttack && attacker != null)
			{
				float num5 = (attackPower >= 2f) ? 1.25f : 1f;
				float num6 = MathUtils.Pow(attacker.ComponentBody.Mass / target.Mass, 0.5f);
				float x2 = num5 * num6;
				num4 = 5.5f * MathUtils.Saturate(x2);
				x = 0.25f * MathUtils.Saturate(x2);
			}
			else if (attackPower > 0f)
			{
				num4 = 2f;
				x = 0.2f;
			}
			if (num4 > 0f)
			{
				target.ApplyImpulse(num4 * Vector3.Normalize(hitDirection + ComponentMiner.s_random.Vector3(0.1f) + 0.2f * Vector3.UnitY));
				ComponentLocomotion componentLocomotion = target.Entity.FindComponent<ComponentLocomotion>();
				if (componentLocomotion != null)
				{
					componentLocomotion.StunTime = MathUtils.Max(componentLocomotion.StunTime, x);
				}
			}
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x00070E90 File Offset: 0x0006F090
		public void Update(float dt)
		{
			float num = (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative) ? (0.5f / SettingsManager.CreativeDigTime) : 4f;
			this.m_lastPokingPhase = this.PokingPhase;
			if (this.DigCellFace != null || this.PokingPhase > 0f)
			{
				this.PokingPhase += num * this.m_subsystemTime.GameTimeDelta;
				if (this.PokingPhase > 1f)
				{
					if (this.DigCellFace != null)
					{
						this.PokingPhase = MathUtils.Remainder(this.PokingPhase, 1f);
					}
					else
					{
						this.PokingPhase = 0f;
					}
				}
			}
			if (this.DigCellFace != null && Time.FrameIndex - this.m_lastDigFrameIndex > 1)
			{
				this.DigCellFace = null;
			}
		}

        // Token: 0x06000E96 RID: 3734 RVA: 0x00070F74 File Offset: 0x0006F174
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemSoundMaterials = base.Project.FindSubsystem<SubsystemSoundMaterials>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.ComponentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			if (this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative && this.ComponentPlayer != null)
			{
				this.Inventory = base.Entity.FindComponent<ComponentCreativeInventory>();
			}
			else
			{
				this.Inventory = base.Entity.FindComponent<ComponentInventory>();
			}
			this.AttackPower = valuesDictionary.GetValue<float>("AttackPower");
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x00071084 File Offset: 0x0006F284
		public static bool IsBlockPlacingAllowed(ComponentBody componentBody)
		{
			if (componentBody.StandingOnBody != null || componentBody.StandingOnValue != null)
			{
				return true;
			}
			if (componentBody.ImmersionFactor > 0.01f)
			{
				return true;
			}
			if (componentBody.ParentBody != null && ComponentMiner.IsBlockPlacingAllowed(componentBody.ParentBody))
			{
				return true;
			}
			ComponentLocomotion componentLocomotion = componentBody.Entity.FindComponent<ComponentLocomotion>();
			return componentLocomotion != null && componentLocomotion.LadderValue != null;
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x000710F4 File Offset: 0x0006F2F4
		public float CalculateDigTime(int digValue, int toolContents)
		{
			Block block = BlocksManager.Blocks[toolContents];
			Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(digValue)];
			float digResilience = block2.DigResilience;
			if (this.ComponentPlayer != null && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Creative)
			{
				if (digResilience < float.PositiveInfinity)
				{
					return 0f;
				}
				return float.PositiveInfinity;
			}
			else if (this.ComponentPlayer != null && this.m_subsystemGameInfo.WorldSettings.GameMode == GameMode.Adventure)
			{
				float num = 0f;
				if (block2.DigMethod == BlockDigMethod.Shovel && block.ShovelPower >= 2f)
				{
					num = block.ShovelPower;
				}
				else if (block2.DigMethod == BlockDigMethod.Quarry && block.QuarryPower >= 2f)
				{
					num = block.QuarryPower;
				}
				else if (block2.DigMethod == BlockDigMethod.Hack && block.HackPower >= 2f)
				{
					num = block.HackPower;
				}
				num *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
				if (num <= 0f)
				{
					return float.PositiveInfinity;
				}
				return MathUtils.Max(digResilience / num, 0f);
			}
			else
			{
				float num2 = 0f;
				if (block2.DigMethod == BlockDigMethod.Shovel)
				{
					num2 = block.ShovelPower;
				}
				else if (block2.DigMethod == BlockDigMethod.Quarry)
				{
					num2 = block.QuarryPower;
				}
				else if (block2.DigMethod == BlockDigMethod.Hack)
				{
					num2 = block.HackPower;
				}
				if (this.ComponentPlayer != null)
				{
					num2 *= this.ComponentPlayer.ComponentLevel.StrengthFactor;
				}
				if (num2 <= 0f)
				{
					return float.PositiveInfinity;
				}
				return MathUtils.Max(digResilience / num2, 0f);
			}
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00071274 File Offset: 0x0006F474
		public bool CanUseTool(int toolValue)
		{
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative && this.m_subsystemGameInfo.WorldSettings.AreAdventureSurvivalMechanicsEnabled)
			{
				Block block = BlocksManager.Blocks[Terrain.ExtractContents(toolValue)];
				if (this.ComponentPlayer != null && this.ComponentPlayer.PlayerData.Level < (float)block.PlayerLevelRequired)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400092F RID: 2351
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000930 RID: 2352
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000931 RID: 2353
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x04000932 RID: 2354
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000933 RID: 2355
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000934 RID: 2356
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000935 RID: 2357
		public SubsystemSoundMaterials m_subsystemSoundMaterials;

		// Token: 0x04000936 RID: 2358
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000937 RID: 2359
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000938 RID: 2360
		public static Game.Random s_random = new Game.Random();

		// Token: 0x04000939 RID: 2361
		public double m_digStartTime;

		// Token: 0x0400093A RID: 2362
		public float m_digProgress;

		// Token: 0x0400093B RID: 2363
		public double m_lastHitTime;

		// Token: 0x0400093C RID: 2364
		public static string fName = "ComponentMiner";

		// Token: 0x0400093D RID: 2365
		public int m_lastDigFrameIndex;

		// Token: 0x0400093E RID: 2366
		public float m_lastPokingPhase;

		// Token: 0x04000945 RID: 2373
		public static Func<TerrainRaycastResult, bool> Dig1;

		// Token: 0x04000946 RID: 2374
		public static Action<bool> Poke1;

		// Token: 0x04000947 RID: 2375
		public static Func<TerrainRaycastResult, bool> Place1;

		// Token: 0x04000948 RID: 2376
		public static Func<TerrainRaycastResult, int, bool> Place2;

		// Token: 0x04000949 RID: 2377
		public static Func<Ray3, bool> Use1;

		// Token: 0x0400094A RID: 2378
		public static Func<TerrainRaycastResult, bool> Interact1;

		// Token: 0x0400094B RID: 2379
		public static Action<ComponentBody, Vector3, Vector3> Hit1;
	}
}
