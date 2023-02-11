using System;
using System.Diagnostics;
using System.Linq;
using Engine;
using GameEntitySystem;
using Survivalcraft.Game.ModificationHolder;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F0 RID: 496
	public class ComponentLocomotion : Component, IUpdateable
	{
		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000E29 RID: 3625 RVA: 0x0006DE07 File Offset: 0x0006C007
		// (set) Token: 0x06000E2A RID: 3626 RVA: 0x0006DE0F File Offset: 0x0006C00F
		public float AccelerationFactor { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000E2B RID: 3627 RVA: 0x0006DE18 File Offset: 0x0006C018
		// (set) Token: 0x06000E2C RID: 3628 RVA: 0x0006DE20 File Offset: 0x0006C020
		public float WalkSpeed { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000E2D RID: 3629 RVA: 0x0006DE29 File Offset: 0x0006C029
		// (set) Token: 0x06000E2E RID: 3630 RVA: 0x0006DE31 File Offset: 0x0006C031
		public float LadderSpeed { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000E2F RID: 3631 RVA: 0x0006DE3A File Offset: 0x0006C03A
		// (set) Token: 0x06000E30 RID: 3632 RVA: 0x0006DE42 File Offset: 0x0006C042
		public float JumpSpeed { get; set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000E31 RID: 3633 RVA: 0x0006DE4B File Offset: 0x0006C04B
		// (set) Token: 0x06000E32 RID: 3634 RVA: 0x0006DE53 File Offset: 0x0006C053
		public float FlySpeed { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x0006DE5C File Offset: 0x0006C05C
		// (set) Token: 0x06000E34 RID: 3636 RVA: 0x0006DE64 File Offset: 0x0006C064
		public float CreativeFlySpeed { get; set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000E35 RID: 3637 RVA: 0x0006DE6D File Offset: 0x0006C06D
		// (set) Token: 0x06000E36 RID: 3638 RVA: 0x0006DE75 File Offset: 0x0006C075
		public float SwimSpeed { get; set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000E37 RID: 3639 RVA: 0x0006DE7E File Offset: 0x0006C07E
		// (set) Token: 0x06000E38 RID: 3640 RVA: 0x0006DE86 File Offset: 0x0006C086
		public float TurnSpeed { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000E39 RID: 3641 RVA: 0x0006DE8F File Offset: 0x0006C08F
		// (set) Token: 0x06000E3A RID: 3642 RVA: 0x0006DE97 File Offset: 0x0006C097
		public float LookSpeed { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000E3B RID: 3643 RVA: 0x0006DEA0 File Offset: 0x0006C0A0
		// (set) Token: 0x06000E3C RID: 3644 RVA: 0x0006DEA8 File Offset: 0x0006C0A8
		public float InAirWalkFactor { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000E3D RID: 3645 RVA: 0x0006DEB1 File Offset: 0x0006C0B1
		// (set) Token: 0x06000E3E RID: 3646 RVA: 0x0006DEB9 File Offset: 0x0006C0B9
		public float? SlipSpeed { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000E3F RID: 3647 RVA: 0x0006DEC2 File Offset: 0x0006C0C2
		// (set) Token: 0x06000E40 RID: 3648 RVA: 0x0006DECC File Offset: 0x0006C0CC
		public Vector2 LookAngles
		{
			get
			{
				return this.m_lookAngles;
			}
			set
			{
				value.X = MathUtils.Clamp(value.X, 0f - MathUtils.DegToRad(140f), MathUtils.DegToRad(140f));
				value.Y = MathUtils.Clamp(value.Y, 0f - MathUtils.DegToRad(82f), MathUtils.DegToRad(82f));
				this.m_lookAngles = value;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0006DF38 File Offset: 0x0006C138
		// (set) Token: 0x06000E42 RID: 3650 RVA: 0x0006DF40 File Offset: 0x0006C140
		public int? LadderValue { get; set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000E43 RID: 3651 RVA: 0x0006DF49 File Offset: 0x0006C149
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x0006DF54 File Offset: 0x0006C154
		public Vector2? WalkOrder
		{
			get
			{
				return this.m_walkOrder;
			}
			set
			{
				this.m_walkOrder = value;
				if (this.m_walkOrder != null)
				{
					float num = this.m_walkOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_walkOrder = new Vector2?(this.m_walkOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000E45 RID: 3653 RVA: 0x0006DFB2 File Offset: 0x0006C1B2
		// (set) Token: 0x06000E46 RID: 3654 RVA: 0x0006DFBC File Offset: 0x0006C1BC
		public Vector3? FlyOrder
		{
			get
			{
				return this.m_flyOrder;
			}
			set
			{
				this.m_flyOrder = value;
				if (this.m_flyOrder != null)
				{
					float num = this.m_flyOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_flyOrder = new Vector3?(this.m_flyOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000E47 RID: 3655 RVA: 0x0006E01A File Offset: 0x0006C21A
		// (set) Token: 0x06000E48 RID: 3656 RVA: 0x0006E024 File Offset: 0x0006C224
		public Vector3? SwimOrder
		{
			get
			{
				return this.m_swimOrder;
			}
			set
			{
				this.m_swimOrder = value;
				if (this.m_swimOrder != null)
				{
					float num = this.m_swimOrder.Value.LengthSquared();
					if (num > 1f)
					{
						this.m_swimOrder = new Vector3?(this.m_swimOrder.Value / MathUtils.Sqrt(num));
					}
				}
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000E49 RID: 3657 RVA: 0x0006E082 File Offset: 0x0006C282
		// (set) Token: 0x06000E4A RID: 3658 RVA: 0x0006E08A File Offset: 0x0006C28A
		public Vector2 TurnOrder
		{
			get
			{
				return this.m_turnOrder;
			}
			set
			{
				this.m_turnOrder = value;
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x0006E093 File Offset: 0x0006C293
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x0006E09B File Offset: 0x0006C29B
		public Vector2 LookOrder
		{
			get
			{
				return this.m_lookOrder;
			}
			set
			{
				this.m_lookOrder = value;
			}
		}

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x0006E0A4 File Offset: 0x0006C2A4
		// (set) Token: 0x06000E4E RID: 3662 RVA: 0x0006E0AC File Offset: 0x0006C2AC
		public float JumpOrder
		{
			get
			{
				return this.m_jumpOrder;
			}
			set
			{
				this.m_jumpOrder = MathUtils.Saturate(value);
			}
		}

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0006E0BA File Offset: 0x0006C2BA
		// (set) Token: 0x06000E50 RID: 3664 RVA: 0x0006E0C2 File Offset: 0x0006C2C2
		public Vector3? VrMoveOrder { get; set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x0006E0CB File Offset: 0x0006C2CB
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x0006E0D3 File Offset: 0x0006C2D3
		public Vector2? VrLookOrder { get; set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000E53 RID: 3667 RVA: 0x0006E0DC File Offset: 0x0006C2DC
		// (set) Token: 0x06000E54 RID: 3668 RVA: 0x0006E0E4 File Offset: 0x0006C2E4
		public float StunTime { get; set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0006E0ED File Offset: 0x0006C2ED
		// (set) Token: 0x06000E56 RID: 3670 RVA: 0x0006E0F5 File Offset: 0x0006C2F5
		public Vector2? LastWalkOrder { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000E57 RID: 3671 RVA: 0x0006E0FE File Offset: 0x0006C2FE
		// (set) Token: 0x06000E58 RID: 3672 RVA: 0x0006E106 File Offset: 0x0006C306
		public float LastJumpOrder { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000E59 RID: 3673 RVA: 0x0006E10F File Offset: 0x0006C30F
		// (set) Token: 0x06000E5A RID: 3674 RVA: 0x0006E117 File Offset: 0x0006C317
		public Vector3? LastFlyOrder { get; set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000E5B RID: 3675 RVA: 0x0006E120 File Offset: 0x0006C320
		// (set) Token: 0x06000E5C RID: 3676 RVA: 0x0006E128 File Offset: 0x0006C328
		public Vector3? LastSwimOrder { get; set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000E5D RID: 3677 RVA: 0x0006E131 File Offset: 0x0006C331
		// (set) Token: 0x06000E5E RID: 3678 RVA: 0x0006E139 File Offset: 0x0006C339
		public Vector2 LastTurnOrder { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000E5F RID: 3679 RVA: 0x0006E142 File Offset: 0x0006C342
		// (set) Token: 0x06000E60 RID: 3680 RVA: 0x0006E14A File Offset: 0x0006C34A
		public bool IsCreativeFlyEnabled { get; set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000E61 RID: 3681 RVA: 0x0006E153 File Offset: 0x0006C353
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Locomotion;
			}
		}

		// Token: 0x06000E62 RID: 3682 RVA: 0x0006E158 File Offset: 0x0006C358
		public virtual void Update(float dt)
		{
			this.SlipSpeed = null;
			if (this.m_subsystemGameInfo.WorldSettings.GameMode != GameMode.Creative)
			{
				this.IsCreativeFlyEnabled = false;
			}
			this.StunTime = MathUtils.Max(this.StunTime - dt, 0f);
			if (this.m_componentCreature.ComponentHealth.Health > 0f && this.StunTime <= 0f)
			{
				Vector3 position = this.m_componentCreature.ComponentBody.Position;
				PlayerStats playerStats = this.m_componentCreature.PlayerStats;
				if (playerStats != null)
				{
					float num = (this.m_lastPosition != null) ? Vector3.Distance(position, this.m_lastPosition.Value) : 0f;
					num = MathUtils.Min(num, 25f * this.m_subsystemTime.PreviousGameTimeDelta);
					playerStats.DistanceTravelled += (double)num;
					if (this.m_componentRider != null && this.m_componentRider.Mount != null)
					{
						playerStats.DistanceRidden += (double)num;
					}
					else
					{
						if (this.m_walking)
						{
							playerStats.DistanceWalked += (double)num;
							this.m_walking = false;
						}
						if (this.m_falling)
						{
							playerStats.DistanceFallen += (double)num;
							this.m_falling = false;
						}
						if (this.m_climbing)
						{
							playerStats.DistanceClimbed += (double)num;
							this.m_climbing = false;
						}
						if (this.m_jumping)
						{
							playerStats.Jumps += 1L;
							this.m_jumping = false;
						}
						if (this.m_swimming)
						{
							playerStats.DistanceSwam += (double)num;
							this.m_swimming = false;
						}
						if (this.m_flying)
						{
							playerStats.DistanceFlown += (double)num;
							this.m_flying = false;
						}
					}
					playerStats.DeepestDive = MathUtils.Max(playerStats.DeepestDive, (double)this.m_componentCreature.ComponentBody.ImmersionDepth);
					playerStats.LowestAltitude = MathUtils.Min(playerStats.LowestAltitude, (double)position.Y);
					playerStats.HighestAltitude = MathUtils.Max(playerStats.HighestAltitude, (double)position.Y);
					playerStats.EasiestModeUsed = (GameMode)MathUtils.Min((int)this.m_subsystemGameInfo.WorldSettings.GameMode, (int)playerStats.EasiestModeUsed);
				}
				if(this.m_componentCreature is ComponentPlayer)
				{
					if(ComponentInput.speed > 3 && ComponentInput.state)
						this.m_componentCreature.ComponentBody.Position = new Vector3(position.X + ComponentInput.step, position.Y, position.Z);
				}
				this.m_lastPosition = new Vector3?(position);
				this.m_swimBurstRemaining = MathUtils.Saturate(0.1f * this.m_swimBurstRemaining + dt);
				int x = Terrain.ToCell(position.X);
				int y = Terrain.ToCell(position.Y + 0.2f);
				int z = Terrain.ToCell(position.Z);
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
				int num2 = Terrain.ExtractContents(cellValue);
				Block block = BlocksManager.Blocks[num2];
				if (this.LadderSpeed > 0f && this.LadderValue == null && block is LadderBlock && this.m_subsystemTime.GameTime >= this.m_ladderActivationTime && !this.IsCreativeFlyEnabled && this.m_componentCreature.ComponentBody.ParentBody == null)
				{
					int face = LadderBlock.GetFace(Terrain.ExtractData(cellValue));
					if ((face == 0 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z > 0f) || (face == 1 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.X > 0f) || (face == 2 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z < 0f) || (face == 3 && this.m_componentCreature.ComponentBody.CollisionVelocityChange.X < 0f) || this.m_componentCreature.ComponentBody.StandingOnValue == null)
					{
						this.LadderValue = new int?(cellValue);
						this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
						this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
					}
				}
				Quaternion rotation = this.m_componentCreature.ComponentBody.Rotation;
				float num3 = MathUtils.Atan2(2f * rotation.Y * rotation.W - 2f * rotation.X * rotation.Z, 1f - 2f * rotation.Y * rotation.Y - 2f * rotation.Z * rotation.Z);
				num3 += (0f - this.TurnSpeed) * this.TurnOrder.X * dt;
				if (this.VrLookOrder != null)
				{
					num3 += this.VrLookOrder.Value.X;
				}
				this.m_componentCreature.ComponentBody.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, num3);
				this.LookAngles += this.LookSpeed * this.LookOrder * dt;
				if (this.VrLookOrder != null)
				{
					this.LookAngles = new Vector2(this.LookAngles.X, this.VrLookOrder.Value.Y);
				}
				if (this.VrMoveOrder != null)
				{
					this.m_componentCreature.ComponentBody.ApplyDirectMove(this.VrMoveOrder.Value);
				}
				if (this.LadderValue != null)
				{
					this.LadderMovement(dt, cellValue);
				}
				else
				{
					this.NormalMovement(dt);
				}
			}
			else
			{
				this.m_componentCreature.ComponentBody.IsGravityEnabled = true;
				this.m_componentCreature.ComponentBody.IsGroundDragEnabled = true;
				this.m_componentCreature.ComponentBody.IsWaterDragEnabled = true;
			}
			this.LastWalkOrder = this.WalkOrder;
			this.LastFlyOrder = this.FlyOrder;
			this.LastSwimOrder = this.SwimOrder;
			this.LastTurnOrder = this.TurnOrder;
			this.LastJumpOrder = this.JumpOrder;
			this.WalkOrder = null;
			this.FlyOrder = null;
			this.SwimOrder = null;
			this.TurnOrder = Vector2.Zero;
			this.JumpOrder = 0f;
			this.VrMoveOrder = null;
			this.VrLookOrder = null;
			this.LookOrder = new Vector2(this.m_lookAutoLevelX ? (-10f * this.LookAngles.X / this.LookSpeed) : 0f, this.m_lookAutoLevelY ? (-10f * this.LookAngles.Y / this.LookSpeed) : 0f);
		}

        // Token: 0x06000E63 RID: 3683 RVA: 0x0006E824 File Offset: 0x0006CA24
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_componentPlayer = base.Entity.FindComponent<ComponentPlayer>();
			this.m_componentLevel = base.Entity.FindComponent<ComponentLevel>();
			this.m_componentClothing = base.Entity.FindComponent<ComponentClothing>();
			this.m_componentMount = base.Entity.FindComponent<ComponentMount>();
			this.m_componentRider = base.Entity.FindComponent<ComponentRider>();
			this.IsCreativeFlyEnabled = valuesDictionary.GetValue<bool>("IsCreativeFlyEnabled");
			this.AccelerationFactor = valuesDictionary.GetValue<float>("AccelerationFactor");
			this.WalkSpeed = valuesDictionary.GetValue<float>("WalkSpeed");
			this.LadderSpeed = valuesDictionary.GetValue<float>("LadderSpeed");
			this.JumpSpeed = valuesDictionary.GetValue<float>("JumpSpeed");
			this.CreativeFlySpeed = valuesDictionary.GetValue<float>("CreativeFlySpeed");
			this.FlySpeed = valuesDictionary.GetValue<float>("FlySpeed");
			this.SwimSpeed = valuesDictionary.GetValue<float>("SwimSpeed");
			this.TurnSpeed = valuesDictionary.GetValue<float>("TurnSpeed");
			this.LookSpeed = valuesDictionary.GetValue<float>("LookSpeed");
			this.InAirWalkFactor = valuesDictionary.GetValue<float>("InAirWalkFactor");
			this.m_walkSpeedWhenTurning = valuesDictionary.GetValue<float>("WalkSpeedWhenTurning");
			this.m_minFrictionFactor = valuesDictionary.GetValue<float>("MinFrictionFactor");
			this.m_lookAutoLevelX = valuesDictionary.GetValue<bool>("LookAutoLevelX");
			this.m_lookAutoLevelY = valuesDictionary.GetValue<bool>("LookAutoLevelY");
			if (base.Entity.FindComponent<ComponentPlayer>() == null)
			{
				this.WalkSpeed *= this.m_random.Float(0.85f, 1f);
				this.FlySpeed *= this.m_random.Float(0.85f, 1f);
				this.SwimSpeed *= this.m_random.Float(0.85f, 1f);
			}
		}

        // Token: 0x06000E64 RID: 3684 RVA: 0x0006EA52 File Offset: 0x0006CC52
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<bool>("IsCreativeFlyEnabled", this.IsCreativeFlyEnabled);
		}

		private Boolean FlyingLogicAnimal(String[] animalSet)
		{
			Boolean result = false;
			for (int i = 0; i < animalSet.Length; i++)
			{
				result = result || this.m_componentCreature.DisplayName.Contains(animalSet[i]);
			}
			return result;
		}

		// Token: 0x06000E65 RID: 3685 RVA: 0x0006EA68 File Offset: 0x0006CC68
		public void NormalMovement(float dt)
		{
			Boolean enabler = this.IsCreativeFlyEnabled;
			this.m_componentCreature.ComponentBody.IsGravityEnabled = true;
			this.m_componentCreature.ComponentBody.IsGroundDragEnabled = true;
			this.m_componentCreature.ComponentBody.IsWaterDragEnabled = true;
			Vector3 vector = this.m_componentCreature.ComponentBody.getVectorSpeed();
			Vector3 right = this.m_componentCreature.ComponentBody.Matrix.Right;
			Vector3 vector2 = Vector3.Transform(this.m_componentCreature.ComponentBody.Matrix.Forward, Quaternion.CreateFromAxisAngle(right, this.LookAngles.Y));
			if (this.WalkSpeed > 0f && this.WalkOrder != null)
			{
				enabler = ModificationsHolder.allowFlyingAnimal ? enabler || this.FlyingLogicAnimal(ModificationsHolder.animalTypes) : enabler;
                if (enabler)
				{
                    
                    Vector3 vector3 = new Vector3(this.WalkOrder.Value.X, 0f, this.WalkOrder.Value.Y);
					if (this.FlyOrder != null)
					{
						vector3 += this.FlyOrder.Value;
					}
					Vector3 v = (!SettingsManager.HorizontalCreativeFlight || this.m_componentPlayer == null || this.m_componentPlayer.ComponentInput.IsControlledByTouch) ? Vector3.Normalize(vector2 + 0.1f * Vector3.UnitY) : Vector3.Normalize(vector2 * new Vector3(1f, 0f, 1f));
					Vector3 v2 = this.CreativeFlySpeed *  (right * vector3.X + Vector3.UnitY * vector3.Y + v * vector3.Z);
					float num = (vector3 == Vector3.Zero) ? 5f : 3f;
					vector += MathUtils.Saturate(num * dt) * (v2 - vector);
					vector += (v2 * ComponentInput.speed * 0.05f);
					this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
					this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
					this.m_flying = true;
				}
				else
				{
					Vector2 value = this.WalkOrder.Value;
					if (this.m_walkSpeedWhenTurning > 0f && MathUtils.Abs(this.TurnOrder.X) > 0.02f)
					{
						value.Y = MathUtils.Max(value.Y, MathUtils.Lerp(0f, this.m_walkSpeedWhenTurning, MathUtils.Saturate(2f * MathUtils.Abs(this.TurnOrder.X))));
					}
					float num2 = this.WalkSpeed;
					if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0.2f)
					{
						num2 *= 0.66f;
					}
					if (value.Y < 0f)
					{
						num2 *= 0.6f;
					}
					if (this.m_componentLevel != null)
					{
						num2 *= this.m_componentLevel.SpeedFactor;
					}
					if (this.m_componentMount != null)
					{
						ComponentRider rider = this.m_componentMount.Rider;
						if (rider != null)
						{
							ComponentClothing componentClothing = rider.Entity.FindComponent<ComponentClothing>();
							if (componentClothing != null)
							{
								num2 *= componentClothing.SteedMovementSpeedFactor;
							}
						}
					}
					Vector3 v3 = value.X * Vector3.Normalize(new Vector3(right.X, 0f, right.Z)) + value.Y * Vector3.Normalize(new Vector3(vector2.X, 0f, vector2.Z));
					Vector3 vector4 = num2 * v3 + this.m_componentCreature.ComponentBody.StandingOnVelocity;
					float num4;
					if (this.m_componentCreature.ComponentBody.StandingOnValue != null)
					{
						float num3 = MathUtils.Max(BlocksManager.Blocks[Terrain.ExtractContents(this.m_componentCreature.ComponentBody.StandingOnValue.Value)].FrictionFactor, this.m_minFrictionFactor);
						num4 = MathUtils.Saturate(dt * 6f * this.AccelerationFactor * num3);
						if (num3 < 0.25f)
						{
							this.SlipSpeed = new float?(num2 * value.Length());
						}
						this.m_walking = true;
					}
					else
					{
						num4 = MathUtils.Saturate(dt * 6f * this.AccelerationFactor * this.InAirWalkFactor);
						if (this.m_componentCreature.ComponentBody.ImmersionFactor > 0f)
						{
							this.m_swimming = true;
						}
						else
						{
							this.m_falling = true;
						}
					}
					vector.X += num4 * (vector4.X - vector.X);
					vector.Z += num4 * (vector4.Z - vector.Z);
					Vector3 vector5 = value.X * right + value.Y * vector2;
					if (this.m_componentLevel != null)
					{
						vector5 *= this.m_componentLevel.SpeedFactor;
					}
					vector.Y += 10f * this.AccelerationFactor * vector5.Y * this.m_componentCreature.ComponentBody.ImmersionFactor * dt;
					this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
					if (this.m_componentPlayer != null && Time.PeriodicEvent(10.0, 0.0) && (this.m_shoesWarningTime == 0.0 || Time.FrameStartTime - this.m_shoesWarningTime > 300.0) && this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_componentCreature.ComponentBody.ImmersionFactor < 0.1f)
					{
						bool flag = false;
						int value2 = this.m_componentPlayer.ComponentClothing.GetClothes(ClothingSlot.Feet).LastOrDefault<int>();
						if (Terrain.ExtractContents(value2) == 203)
						{
							flag = (ClothingBlock.GetClothingData(Terrain.ExtractData(value2)).MovementSpeedFactor > 1f);
						}
						if (!flag && vector4.LengthSquared() / vector.LengthSquared() > 0.99f && this.WalkOrder.Value.LengthSquared() > 0.99f)
						{
							this.m_componentPlayer.ComponentGui.DisplaySmallMessage(LanguageControl.Get(base.GetType().Name, 1), Color.White, true, true);
							this.m_shoesWarningTime = Time.FrameStartTime;
						}
					}
				}
			}
			if (this.FlySpeed > 0f && this.FlyOrder != null)
			{
				Vector3 value3 = this.FlyOrder.Value;
				Vector3 v4 = this.FlySpeed * value3;
				vector += MathUtils.Saturate(2f * this.AccelerationFactor * dt) * (v4 - vector);
				this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
				this.m_flying = true;
			}
			if (this.SwimSpeed > 0f && this.SwimOrder != null && this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f)
			{
				Vector3 value4 = this.SwimOrder.Value;
				Vector3 vector6 = this.SwimSpeed * value4;
				float num5 = 2f;
				if (value4.LengthSquared() >= 0.99f)
				{
					vector6 *= MathUtils.Lerp(1f, 2f, this.m_swimBurstRemaining);
					num5 *= MathUtils.Lerp(1f, 4f, this.m_swimBurstRemaining);
					this.m_swimBurstRemaining -= dt;
				}
				vector += MathUtils.Saturate(num5 * this.AccelerationFactor * dt) * (vector6 - vector);
				this.m_componentCreature.ComponentBody.IsGravityEnabled = (MathUtils.Abs(value4.Y) <= 0.07f);
				this.m_componentCreature.ComponentBody.IsWaterDragEnabled = false;
				this.m_componentCreature.ComponentBody.IsGroundDragEnabled = false;
				this.m_swimming = true;
			}
			if (this.JumpOrder > 0f && (this.m_componentCreature.ComponentBody.StandingOnValue != null || this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f) && !this.m_componentCreature.ComponentBody.IsSneaking)
			{
				float num6 = this.JumpSpeed;
				if (this.m_componentLevel != null)
				{
					num6 *= 0.25f * (this.m_componentLevel.SpeedFactor - 1f) + 1f;
				}
				vector.Y = MathUtils.Min(vector.Y + MathUtils.Saturate(this.JumpOrder) * num6, num6);
				this.m_jumping = true;
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 10f);
			}
			if (MathUtils.Abs(this.m_componentCreature.ComponentBody.CollisionVelocityChange.Y) > 3f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				this.m_subsystemNoise.MakeNoise(this.m_componentCreature.ComponentBody, 0.25f, 10f);
			}
			//this.m_componentCreature.ComponentBody.Velocity = vector;
			this.m_componentCreature.ComponentBody.setVectorSpeed(vector, this.m_componentCreature, this, enabler);
        }

		// Token: 0x06000E66 RID: 3686 RVA: 0x0006F3D4 File Offset: 0x0006D5D4
		public void LadderMovement(float dt, int value)
		{
			this.m_componentCreature.ComponentBody.IsGravityEnabled = false;
			Vector3 position = this.m_componentCreature.ComponentBody.Position;
			Vector3 vector = this.m_componentCreature.ComponentBody.getVectorSpeed();
			int num = Terrain.ExtractContents(value);
			if (BlocksManager.Blocks[num] is LadderBlock)
			{
				this.LadderValue = new int?(value);
				if (this.WalkOrder != null)
				{
					Vector2 value2 = this.WalkOrder.Value;
					float num2 = this.LadderSpeed * value2.Y;
					vector.X = 5f * (MathUtils.Floor(position.X) + 0.5f - position.X);
					vector.Z = 5f * (MathUtils.Floor(position.Z) + 0.5f - position.Z);
					vector.Y += MathUtils.Saturate(20f * dt) * (num2 - vector.Y);
					this.m_climbing = true;
				}
				if (this.m_componentCreature.ComponentBody.StandingOnValue != null && this.m_subsystemTime.GameTime >= this.m_ladderActivationTime)
				{
					this.LadderValue = null;
					this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
				}
			}
			else
			{
				this.LadderValue = null;
				this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.20000000298023224;
			}
			if (this.JumpOrder > 0f)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(2f);
				vector += this.JumpSpeed * this.m_componentCreature.ComponentBody.Matrix.Forward;
				this.m_ladderActivationTime = this.m_subsystemTime.GameTime + 0.33000001311302185;
				this.LadderValue = null;
				this.m_jumping = true;
			}
			if (this.IsCreativeFlyEnabled)
			{
				this.m_componentCreature.ComponentCreatureSounds.PlayFootstepSound(1f);
				this.LadderValue = null;
			}
			if (this.m_componentCreature.ComponentBody.ParentBody != null)
			{
				this.LadderValue = null;
			}
			this.m_componentCreature.ComponentBody.setVectorSpeed(vector);
		}

		// Token: 0x040008ED RID: 2285
		public SubsystemTime m_subsystemTime;

		// Token: 0x040008EE RID: 2286
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x040008EF RID: 2287
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040008F0 RID: 2288
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040008F1 RID: 2289
		public ComponentCreature m_componentCreature;

		// Token: 0x040008F2 RID: 2290
		public ComponentPlayer m_componentPlayer;

		// Token: 0x040008F3 RID: 2291
		public ComponentLevel m_componentLevel;

		// Token: 0x040008F4 RID: 2292
		public ComponentClothing m_componentClothing;

		// Token: 0x040008F5 RID: 2293
		public ComponentMount m_componentMount;

		// Token: 0x040008F6 RID: 2294
		public ComponentRider m_componentRider;

		// Token: 0x040008F7 RID: 2295
		public Game.Random m_random = new Game.Random();

		// Token: 0x040008F8 RID: 2296
		public Vector2? m_walkOrder;

		// Token: 0x040008F9 RID: 2297
		public Vector3? m_flyOrder;

		// Token: 0x040008FA RID: 2298
		public Vector3? m_swimOrder;

		// Token: 0x040008FB RID: 2299
		public Vector2 m_turnOrder;

		// Token: 0x040008FC RID: 2300
		public Vector2 m_lookOrder;

		// Token: 0x040008FD RID: 2301
		public float m_jumpOrder;

		// Token: 0x040008FE RID: 2302
		public bool m_lookAutoLevelX;

		// Token: 0x040008FF RID: 2303
		public bool m_lookAutoLevelY;

		// Token: 0x04000900 RID: 2304
		public double m_shoesWarningTime;

		// Token: 0x04000901 RID: 2305
		public float m_walkSpeedWhenTurning;

		// Token: 0x04000902 RID: 2306
		public float m_minFrictionFactor;

		// Token: 0x04000903 RID: 2307
		public double m_ladderActivationTime;

		// Token: 0x04000904 RID: 2308
		public float m_swimBurstRemaining;

		// Token: 0x04000905 RID: 2309
		public Vector2 m_lookAngles;

		// Token: 0x04000906 RID: 2310
		public Vector3? m_lastPosition;

		// Token: 0x04000907 RID: 2311
		public bool m_walking;

		// Token: 0x04000908 RID: 2312
		public bool m_falling;

		// Token: 0x04000909 RID: 2313
		public bool m_climbing;

		// Token: 0x0400090A RID: 2314
		public bool m_jumping;

		// Token: 0x0400090B RID: 2315
		public bool m_swimming;

		// Token: 0x0400090C RID: 2316
		public bool m_flying;
	}
}
