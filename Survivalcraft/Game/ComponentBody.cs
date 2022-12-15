using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C6 RID: 454
	public class ComponentBody : ComponentFrame, IUpdateable
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000B75 RID: 2933 RVA: 0x000563A7 File Offset: 0x000545A7
		// (set) Token: 0x06000B76 RID: 2934 RVA: 0x000563AF File Offset: 0x000545AF
		public Vector3 BoxSize { get; set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000B77 RID: 2935 RVA: 0x000563B8 File Offset: 0x000545B8
		// (set) Token: 0x06000B78 RID: 2936 RVA: 0x000563C0 File Offset: 0x000545C0
		public float Mass { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000B79 RID: 2937 RVA: 0x000563C9 File Offset: 0x000545C9
		// (set) Token: 0x06000B7A RID: 2938 RVA: 0x000563D1 File Offset: 0x000545D1
		public float Density { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000B7B RID: 2939 RVA: 0x000563DA File Offset: 0x000545DA
		// (set) Token: 0x06000B7C RID: 2940 RVA: 0x000563E2 File Offset: 0x000545E2
		public Vector2 AirDrag { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000B7D RID: 2941 RVA: 0x000563EB File Offset: 0x000545EB
		// (set) Token: 0x06000B7E RID: 2942 RVA: 0x000563F3 File Offset: 0x000545F3
		public Vector2 WaterDrag { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000B7F RID: 2943 RVA: 0x000563FC File Offset: 0x000545FC
		// (set) Token: 0x06000B80 RID: 2944 RVA: 0x00056404 File Offset: 0x00054604
		public float WaterSwayAngle { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000B81 RID: 2945 RVA: 0x0005640D File Offset: 0x0005460D
		// (set) Token: 0x06000B82 RID: 2946 RVA: 0x00056415 File Offset: 0x00054615
		public float WaterTurnSpeed { get; set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000B83 RID: 2947 RVA: 0x0005641E File Offset: 0x0005461E
		// (set) Token: 0x06000B84 RID: 2948 RVA: 0x00056426 File Offset: 0x00054626
		public float ImmersionDepth { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x0005642F File Offset: 0x0005462F
		// (set) Token: 0x06000B86 RID: 2950 RVA: 0x00056437 File Offset: 0x00054637
		public float ImmersionFactor { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000B87 RID: 2951 RVA: 0x00056440 File Offset: 0x00054640
		// (set) Token: 0x06000B88 RID: 2952 RVA: 0x00056448 File Offset: 0x00054648
		public FluidBlock ImmersionFluidBlock { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000B89 RID: 2953 RVA: 0x00056451 File Offset: 0x00054651
		// (set) Token: 0x06000B8A RID: 2954 RVA: 0x00056459 File Offset: 0x00054659
		public int? StandingOnValue { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000B8B RID: 2955 RVA: 0x00056462 File Offset: 0x00054662
		// (set) Token: 0x06000B8C RID: 2956 RVA: 0x0005646A File Offset: 0x0005466A
		public ComponentBody StandingOnBody { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x00056473 File Offset: 0x00054673
		// (set) Token: 0x06000B8E RID: 2958 RVA: 0x0005647B File Offset: 0x0005467B
		public Vector3 StandingOnVelocity { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000B8F RID: 2959 RVA: 0x00056484 File Offset: 0x00054684
		// (set) Token: 0x06000B90 RID: 2960 RVA: 0x0005648C File Offset: 0x0005468C
		public Vector3 Velocity
		{
			get
			{
				return this.m_velocity;
			}
			set
			{
				if (value.LengthSquared() > 625f)
				{
					this.m_velocity = 25f * Vector3.Normalize(value);
					return;
				}
				this.m_velocity = value;
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x000564BA File Offset: 0x000546BA
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x000564C4 File Offset: 0x000546C4
		public bool IsSneaking
		{
			get
			{
				return this.m_isSneaking;
			}
			set
			{
				if (this.StandingOnValue == null)
				{
					value = false;
				}
				this.m_isSneaking = value;
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000B93 RID: 2963 RVA: 0x000564EB File Offset: 0x000546EB
		// (set) Token: 0x06000B94 RID: 2964 RVA: 0x000564F3 File Offset: 0x000546F3
		public bool IsGravityEnabled { get; set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000B95 RID: 2965 RVA: 0x000564FC File Offset: 0x000546FC
		// (set) Token: 0x06000B96 RID: 2966 RVA: 0x00056504 File Offset: 0x00054704
		public bool IsGroundDragEnabled { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000B97 RID: 2967 RVA: 0x0005650D File Offset: 0x0005470D
		// (set) Token: 0x06000B98 RID: 2968 RVA: 0x00056515 File Offset: 0x00054715
		public bool IsWaterDragEnabled { get; set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000B99 RID: 2969 RVA: 0x0005651E File Offset: 0x0005471E
		// (set) Token: 0x06000B9A RID: 2970 RVA: 0x00056526 File Offset: 0x00054726
		public bool IsSmoothRiseEnabled { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000B9B RID: 2971 RVA: 0x0005652F File Offset: 0x0005472F
		// (set) Token: 0x06000B9C RID: 2972 RVA: 0x00056537 File Offset: 0x00054737
		public float MaxSmoothRiseHeight { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000B9D RID: 2973 RVA: 0x00056540 File Offset: 0x00054740
		// (set) Token: 0x06000B9E RID: 2974 RVA: 0x00056548 File Offset: 0x00054748
		public Vector3 CollisionVelocityChange { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000B9F RID: 2975 RVA: 0x00056554 File Offset: 0x00054754
		public BoundingBox BoundingBox
		{
			get
			{
				Vector3 boxSize = this.BoxSize;
				Vector3 position = base.Position;
				return new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000BA0 RID: 2976 RVA: 0x000565C5 File Offset: 0x000547C5
		public ReadOnlyList<ComponentBody> ChildBodies
		{
			get
			{
				return new ReadOnlyList<ComponentBody>(this.m_childBodies);
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x000565D2 File Offset: 0x000547D2
		// (set) Token: 0x06000BA2 RID: 2978 RVA: 0x000565DC File Offset: 0x000547DC
		public ComponentBody ParentBody
		{
			get
			{
				return this.m_parentBody;
			}
			set
			{
				if (value != this.m_parentBody)
				{
					if (this.m_parentBody != null)
					{
						this.m_parentBody.m_childBodies.Remove(this);
					}
					this.m_parentBody = value;
					if (this.m_parentBody != null)
					{
						this.m_parentBody.m_childBodies.Add(this);
					}
				}
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x0005662C File Offset: 0x0005482C
		// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x00056634 File Offset: 0x00054834
		public Vector3 ParentBodyPositionOffset { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x0005663D File Offset: 0x0005483D
		// (set) Token: 0x06000BA6 RID: 2982 RVA: 0x00056645 File Offset: 0x00054845
		public Quaternion ParentBodyRotationOffset { get; set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x0005664E File Offset: 0x0005484E
		public UpdateOrder UpdateOrder
		{
			get
			{
				if (this.m_parentBody == null)
				{
					return UpdateOrder.Body;
				}
				return this.m_parentBody.UpdateOrder + 1;
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000BA8 RID: 2984 RVA: 0x00056668 File Offset: 0x00054868
		// (remove) Token: 0x06000BA9 RID: 2985 RVA: 0x000566A0 File Offset: 0x000548A0
		public event Action<ComponentBody> CollidedWithBody;

		// Token: 0x06000BAA RID: 2986 RVA: 0x000566D8 File Offset: 0x000548D8
		static ComponentBody()
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = -2; i <= 2; i++)
			{
				for (int j = -2; j <= 2; j++)
				{
					for (int k = -2; k <= 2; k++)
					{
						Vector3 item = new Vector3(0.25f * (float)i, 0.25f * (float)j, 0.25f * (float)k);
						list.Add(item);
					}
				}
			}
			list.Sort((Vector3 o1, Vector3 o2) => Comparer<float>.Default.Compare(o1.LengthSquared(), o2.LengthSquared()));
			ComponentBody.m_freeSpaceOffsets = list.ToArray();
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0005675A File Offset: 0x0005495A
		public void ApplyImpulse(Vector3 impulse)
		{
			this.m_totalImpulse += impulse;
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0005676E File Offset: 0x0005496E
		public void ApplyDirectMove(Vector3 directMove)
		{
			this.m_directMove += directMove;
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x00056782 File Offset: 0x00054982
		public bool IsChildOfBody(ComponentBody componentBody)
		{
			return this.ParentBody == componentBody || (this.ParentBody != null && this.ParentBody.IsChildOfBody(componentBody));
		}

        // Token: 0x06000BAE RID: 2990 RVA: 0x000567A8 File Offset: 0x000549A8
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemMovingBlocks = base.Project.FindSubsystem<SubsystemMovingBlocks>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFluidBlockBehavior = base.Project.FindSubsystem<SubsystemFluidBlockBehavior>(true);
			this.BoxSize = valuesDictionary.GetValue<Vector3>("BoxSize");
			this.Mass = valuesDictionary.GetValue<float>("Mass");
			this.Density = valuesDictionary.GetValue<float>("Density");
			this.AirDrag = valuesDictionary.GetValue<Vector2>("AirDrag");
			this.WaterDrag = valuesDictionary.GetValue<Vector2>("WaterDrag");
			this.WaterSwayAngle = valuesDictionary.GetValue<float>("WaterSwayAngle");
			this.WaterTurnSpeed = valuesDictionary.GetValue<float>("WaterTurnSpeed");
			this.Velocity = valuesDictionary.GetValue<Vector3>("Velocity");
			this.MaxSmoothRiseHeight = valuesDictionary.GetValue<float>("MaxSmoothRiseHeight");
			this.ParentBody = valuesDictionary.GetValue<EntityReference>("ParentBody").GetComponent<ComponentBody>(base.Entity, idToEntityMap, false);
			this.ParentBodyPositionOffset = valuesDictionary.GetValue<Vector3>("ParentBodyPositionOffset");
			this.ParentBodyRotationOffset = valuesDictionary.GetValue<Quaternion>("ParentBodyRotationOffset");
			this.IsSmoothRiseEnabled = true;
			this.IsGravityEnabled = true;
			this.IsGroundDragEnabled = true;
			this.IsWaterDragEnabled = true;
		}

        // Token: 0x06000BAF RID: 2991 RVA: 0x00056948 File Offset: 0x00054B48
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			if (this.Velocity != Vector3.Zero)
			{
				valuesDictionary.SetValue<Vector3>("Velocity", this.Velocity);
			}
			EntityReference value = EntityReference.FromId(this.ParentBody, entityToIdMap);
			if (!value.IsNullOrEmpty())
			{
				valuesDictionary.SetValue<EntityReference>("ParentBody", value);
				valuesDictionary.SetValue<Vector3>("ParentBodyPositionOffset", this.ParentBodyPositionOffset);
				valuesDictionary.SetValue<Quaternion>("ParentBodyRotationOffset", this.ParentBodyRotationOffset);
			}
		}

        // Token: 0x06000BB0 RID: 2992 RVA: 0x000569C4 File Offset: 0x00054BC4
        public override void OnEntityRemoved()
		{
			this.ParentBody = null;
			ComponentBody[] array = this.ChildBodies.ToArray<ComponentBody>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ParentBody = null;
			}
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00056A00 File Offset: 0x00054C00
		public void Update(float dt)
		{
			this.CollisionVelocityChange = Vector3.Zero;
			this.Velocity += this.m_totalImpulse;
			this.m_totalImpulse = Vector3.Zero;
			if (this.m_parentBody != null || this.m_velocity.LengthSquared() > 9.9999994E-11f || this.m_directMove != Vector3.Zero)
			{
				this.m_stoppedTime = 0f;
			}
			else
			{
				this.m_stoppedTime += dt;
				if (this.m_stoppedTime > 0.5f && !Time.PeriodicEvent(0.25, 0.0))
				{
					return;
				}
			}
			Vector3 position = base.Position;
			TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(Terrain.ToCell(position.X), Terrain.ToCell(position.Z));
			if (chunkAtCell == null || chunkAtCell.State <= TerrainChunkState.InvalidContents4)
			{
				this.Velocity = Vector3.Zero;
				return;
			}
			this.m_bodiesCollisionBoxes.Clear();
			this.FindBodiesCollisionBoxes(position, this.m_bodiesCollisionBoxes);
			this.m_movingBlocksCollisionBoxes.Clear();
			this.FindMovingBlocksCollisionBoxes(position, this.m_movingBlocksCollisionBoxes);
			if (!this.MoveToFreeSpace())
			{
				ComponentHealth componentHealth = base.Entity.FindComponent<ComponentHealth>();
				if (componentHealth != null)
				{
					componentHealth.Injure(1f, null, true, "Crushed");
					return;
				}
				base.Project.RemoveEntity(base.Entity, true);
				return;
			}
			else
			{
				if (this.IsGravityEnabled)
				{
					this.m_velocity.Y = this.m_velocity.Y - 10f * dt;
					if (this.ImmersionFactor > 0f)
					{
						float num = this.ImmersionFactor * (1f + 0.03f * MathUtils.Sin((float)MathUtils.Remainder(2.0 * this.m_subsystemTime.GameTime, 6.2831854820251465)));
						this.m_velocity.Y = this.m_velocity.Y + 10f * (1f / this.Density * num) * dt;
					}
				}
				float num2 = MathUtils.Saturate(this.AirDrag.X * dt);
				float num3 = MathUtils.Saturate(this.AirDrag.Y * dt);
				this.m_velocity.X = this.m_velocity.X * (1f - num2);
				this.m_velocity.Y = this.m_velocity.Y * (1f - num3);
				this.m_velocity.Z = this.m_velocity.Z * (1f - num2);
				if (this.IsWaterDragEnabled && this.ImmersionFactor > 0f && this.ImmersionFluidBlock != null)
				{
					Vector2? vector = this.m_subsystemFluidBlockBehavior.CalculateFlowSpeed(Terrain.ToCell(position.X), Terrain.ToCell(position.Y), Terrain.ToCell(position.Z));
					Vector3 vector2 = (vector != null) ? new Vector3(vector.Value.X, 0f, vector.Value.Y) : Vector3.Zero;
					float num4 = 1f;
					if (this.ImmersionFluidBlock.FrictionFactor != 1f)
					{
						num4 = ((SimplexNoise.Noise((float)MathUtils.Remainder(6.0 * Time.FrameStartTime + (double)(this.GetHashCode() % 1000), 1000.0)) > 0.5f) ? this.ImmersionFluidBlock.FrictionFactor : 1f);
					}
					float f = MathUtils.Saturate(this.WaterDrag.X * num4 * this.ImmersionFactor * dt);
					float f2 = MathUtils.Saturate(this.WaterDrag.Y * num4 * dt);
					this.m_velocity.X = MathUtils.Lerp(this.m_velocity.X, vector2.X, f);
					this.m_velocity.Y = MathUtils.Lerp(this.m_velocity.Y, vector2.Y, f2);
					this.m_velocity.Z = MathUtils.Lerp(this.m_velocity.Z, vector2.Z, f);
					if (this.m_parentBody == null && vector != null && this.StandingOnValue == null)
					{
						if (this.WaterTurnSpeed > 0f)
						{
							float s = MathUtils.Saturate(MathUtils.Lerp(1f, 0f, this.m_velocity.Length()));
							Vector2 vector3 = Vector2.Normalize(vector.Value) * s;
							base.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.WaterTurnSpeed * (-1f * vector3.X + 0.71f * vector3.Y) * dt);
						}
						if (this.WaterSwayAngle > 0f)
						{
							base.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, this.WaterSwayAngle * (float)MathUtils.Sin((double)(200f / this.Mass) * this.m_subsystemTime.GameTime));
						}
					}
				}
				if (this.m_parentBody != null)
				{
					Vector3 v = Vector3.Transform(this.ParentBodyPositionOffset, this.m_parentBody.Rotation) + this.m_parentBody.Position - position;
					this.m_velocity = ((dt > 0f) ? (v / dt) : Vector3.Zero);
					base.Rotation = this.ParentBodyRotationOffset * this.m_parentBody.Rotation;
				}
				this.StandingOnValue = null;
				this.StandingOnBody = null;
				this.StandingOnVelocity = Vector3.Zero;
				Vector3 velocity = this.m_velocity;
				float num5 = this.m_velocity.Length();
				if (num5 > 0f)
				{
					float x = 0.45f * MathUtils.Min(this.BoxSize.X, this.BoxSize.Y, this.BoxSize.Z) / num5;
					float num7;
					for (float num6 = dt; num6 > 0f; num6 -= num7)
					{
						num7 = MathUtils.Min(num6, x);
						this.MoveWithCollision(num7, this.m_velocity * num7 + this.m_directMove);
						this.m_directMove = Vector3.Zero;
					}
				}
				this.CollisionVelocityChange = this.m_velocity - velocity;
				if (this.IsGroundDragEnabled && this.StandingOnValue != null)
				{
					this.m_velocity = Vector3.Lerp(this.m_velocity, this.StandingOnVelocity, 6f * dt);
				}
				if (this.StandingOnValue == null)
				{
					this.IsSneaking = false;
				}
				this.UpdateImmersionData();
				if (this.ImmersionFluidBlock is WaterBlock && this.ImmersionDepth > 0.3f && !this.m_fluidEffectsPlayed)
				{
					this.m_fluidEffectsPlayed = true;
					this.m_subsystemAudio.PlayRandomSound("Audio/WaterFallIn", this.m_random.Float(0.75f, 1f), this.m_random.Float(-0.3f, 0f), position, 4f, true);
					this.m_subsystemParticles.AddParticleSystem(new WaterSplashParticleSystem(this.m_subsystemTerrain, position, (this.BoundingBox.Max - this.BoundingBox.Min).Length() > 0.8f));
					return;
				}
				if (this.ImmersionFluidBlock is MagmaBlock && this.ImmersionDepth > 0f && !this.m_fluidEffectsPlayed)
				{
					this.m_fluidEffectsPlayed = true;
					this.m_subsystemAudio.PlaySound("Audio/SizzleLong", 1f, 0f, position, 4f, true);
					this.m_subsystemParticles.AddParticleSystem(new MagmaSplashParticleSystem(this.m_subsystemTerrain, position, (this.BoundingBox.Max - this.BoundingBox.Min).Length() > 0.8f));
					return;
				}
				if (this.ImmersionFluidBlock == null)
				{
					this.m_fluidEffectsPlayed = false;
				}
				return;
			}
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x000571CC File Offset: 0x000553CC
		public void UpdateImmersionData()
		{
			Vector3 position = base.Position;
			int x = Terrain.ToCell(position.X);
			int y = Terrain.ToCell(position.Y + 0.01f);
			int z = Terrain.ToCell(position.Z);
			FluidBlock fluidBlock;
			float? surfaceHeight = this.m_subsystemFluidBlockBehavior.GetSurfaceHeight(x, y, z, out fluidBlock);
			if (surfaceHeight != null)
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
				this.ImmersionDepth = MathUtils.Max(surfaceHeight.Value - position.Y, 0f);
				this.ImmersionFactor = MathUtils.Saturate(MathUtils.Pow(this.ImmersionDepth / this.BoxSize.Y, 0.7f));
				this.ImmersionFluidBlock = BlocksManager.FluidBlocks[Terrain.ExtractContents(cellValue)];
				return;
			}
			this.ImmersionDepth = 0f;
			this.ImmersionFactor = 0f;
			this.ImmersionFluidBlock = null;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x000572B0 File Offset: 0x000554B0
		public bool MoveToFreeSpace()
		{
			Vector3 boxSize = this.BoxSize;
			Vector3 position = base.Position;
			for (int i = 0; i < ComponentBody.m_freeSpaceOffsets.Length; i++)
			{
				Vector3? vector = null;
				Vector3 vector2 = position + ComponentBody.m_freeSpaceOffsets[i];
				if (!(Terrain.ToCell(vector2) != Terrain.ToCell(position)))
				{
					BoundingBox box = new BoundingBox(vector2 - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), vector2 + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
					box.Min += new Vector3(0.01f, this.MaxSmoothRiseHeight + 0.01f, 0.01f);
					box.Max -= new Vector3(0.01f);
					this.m_collisionBoxes.Clear();
					this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
					this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
					this.m_collisionBoxes.AddRange(this.m_bodiesCollisionBoxes);
					if (!this.IsColliding(box, this.m_collisionBoxes))
					{
						vector = new Vector3?(vector2);
					}
					else
					{
						this.m_stoppedTime = 0f;
						ComponentBody.CollisionBox collisionBox;
						float num = this.CalculatePushBack(box, 0, this.m_collisionBoxes, out collisionBox);
						ComponentBody.CollisionBox collisionBox2;
						float num2 = this.CalculatePushBack(box, 1, this.m_collisionBoxes, out collisionBox2);
						ComponentBody.CollisionBox collisionBox3;
						float num3 = this.CalculatePushBack(box, 2, this.m_collisionBoxes, out collisionBox3);
						float num4 = num * num;
						float num5 = num2 * num2;
						float num6 = num3 * num3;
						List<Vector3> list = new List<Vector3>();
						if (num4 <= num5 && num4 <= num6)
						{
							list.Add(vector2 + new Vector3(num, 0f, 0f));
							if (num5 <= num6)
							{
								list.Add(vector2 + new Vector3(0f, num2, 0f));
								list.Add(vector2 + new Vector3(0f, 0f, num3));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, 0f, num3));
								list.Add(vector2 + new Vector3(0f, num2, 0f));
							}
						}
						else if (num5 <= num4 && num5 <= num6)
						{
							list.Add(vector2 + new Vector3(0f, num2, 0f));
							if (num4 <= num6)
							{
								list.Add(vector2 + new Vector3(num, 0f, 0f));
								list.Add(vector2 + new Vector3(0f, 0f, num3));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, 0f, num3));
								list.Add(vector2 + new Vector3(num, 0f, 0f));
							}
						}
						else
						{
							list.Add(vector2 + new Vector3(0f, 0f, num3));
							if (num4 <= num5)
							{
								list.Add(vector2 + new Vector3(num, 0f, 0f));
								list.Add(vector2 + new Vector3(0f, num2, 0f));
							}
							else
							{
								list.Add(vector2 + new Vector3(0f, num2, 0f));
								list.Add(vector2 + new Vector3(num, 0f, 0f));
							}
						}
						foreach (Vector3 vector3 in list)
						{
							box = new BoundingBox(vector3 - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), vector3 + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
							box.Min += new Vector3(0.02f, this.MaxSmoothRiseHeight + 0.02f, 0.02f);
							box.Max -= new Vector3(0.02f);
							this.m_collisionBoxes.Clear();
							this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
							this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
							this.m_collisionBoxes.AddRange(this.m_bodiesCollisionBoxes);
							if (!this.IsColliding(box, this.m_collisionBoxes))
							{
								vector = new Vector3?(vector3);
								break;
							}
						}
					}
					if (vector != null)
					{
						base.Position = vector.Value;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x00057804 File Offset: 0x00055A04
		public void MoveWithCollision(float dt, Vector3 move)
		{
			Vector3 position = base.Position;
			bool isSmoothRising = this.IsSmoothRiseEnabled && this.MaxSmoothRiseHeight > 0f && this.HandleSmoothRise(ref move, position, dt);
			this.HandleAxisCollision(1, move.Y, ref position, isSmoothRising);
			this.HandleAxisCollision(0, move.X, ref position, isSmoothRising);
			this.HandleAxisCollision(2, move.Z, ref position, isSmoothRising);
			base.Position = position;
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x00057874 File Offset: 0x00055A74
		public bool HandleSmoothRise(ref Vector3 move, Vector3 position, float dt)
		{
			Vector3 boxSize = this.BoxSize;
			BoundingBox box = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			box.Min += new Vector3(0.04f, 0f, 0.04f);
			box.Max -= new Vector3(0.04f, 0f, 0.04f);
			this.m_collisionBoxes.Clear();
			this.FindTerrainCollisionBoxes(box, this.m_collisionBoxes);
			this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
			ComponentBody.CollisionBox collisionBox;
			float num = MathUtils.Max(this.CalculatePushBack(box, 1, this.m_collisionBoxes, out collisionBox), 0f);
			if (!BlocksManager.Blocks[Terrain.ExtractContents(collisionBox.BlockValue)].NoSmoothRise && num > 0.04f)
			{
				float x = MathUtils.Min(4.5f * dt, num);
				move.Y = MathUtils.Max(move.Y, x);
				this.m_velocity.Y = MathUtils.Max(this.m_velocity.Y, 0f);
				this.StandingOnValue = new int?(collisionBox.BlockValue);
				this.StandingOnBody = collisionBox.ComponentBody;
				this.m_stoppedTime = 0f;
				return true;
			}
			return false;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x00057A08 File Offset: 0x00055C08
		public void HandleAxisCollision(int axis, float move, ref Vector3 position, bool isSmoothRising)
		{
			Vector3 boxSize = this.BoxSize;
			this.m_collisionBoxes.Clear();
			if (this.IsSneaking && axis != 1)
			{
				this.FindSneakCollisionBoxes(position, new Vector2(boxSize.X - 0.08f, boxSize.Z - 0.08f), this.m_collisionBoxes);
			}
			Vector3 v;
			if (axis != 0)
			{
				if (axis != 1)
				{
					position.Z += move;
					v = new Vector3(0.04f, 0.04f, 0f);
				}
				else
				{
					position.Y += move;
					v = new Vector3(0.04f, 0f, 0.04f);
				}
			}
			else
			{
				position.X += move;
				v = new Vector3(0f, 0.04f, 0.04f);
			}
			BoundingBox boundingBox = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f) + v, position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f) - v);
			this.FindTerrainCollisionBoxes(boundingBox, this.m_collisionBoxes);
			this.m_collisionBoxes.AddRange(this.m_movingBlocksCollisionBoxes);
			ComponentBody.CollisionBox collisionBox;
			float num;
			if (axis != 1 || isSmoothRising)
			{
				BoundingBox smoothRiseBox = boundingBox;
				smoothRiseBox.Min.Y = smoothRiseBox.Min.Y + this.MaxSmoothRiseHeight;
				num = this.CalculateSmoothRisePushBack(boundingBox, smoothRiseBox, axis, this.m_collisionBoxes, out collisionBox);
			}
			else
			{
				num = this.CalculatePushBack(boundingBox, axis, this.m_collisionBoxes, out collisionBox);
			}
			BoundingBox box = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f) + v, position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f) - v);
			ComponentBody.CollisionBox collisionBox2;
			float num2 = this.CalculatePushBack(box, axis, this.m_bodiesCollisionBoxes, out collisionBox2);
			if (MathUtils.Abs(num) > MathUtils.Abs(num2))
			{
				if (num == 0f)
				{
					return;
				}
				int num3 = Terrain.ExtractContents(collisionBox.BlockValue);
				if (BlocksManager.Blocks[num3].HasCollisionBehavior)
				{
					SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num3);
					for (int i = 0; i < blockBehaviors.Length; i++)
					{
						Vector3 vector = (collisionBox.Box.Min + collisionBox.Box.Max) / 2f;
						CellFace cellFace = CellFace.FromAxisAndDirection(Terrain.ToCell(vector.X), Terrain.ToCell(vector.Y), Terrain.ToCell(vector.Z), axis, 0f - ComponentBody.GetVectorComponent(this.m_velocity, axis));
						blockBehaviors[i].OnCollide(cellFace, ComponentBody.GetVectorComponent(this.m_velocity, axis), this);
					}
				}
				if (axis == 0)
				{
					position.X += num;
					this.m_velocity.X = collisionBox.BlockVelocity.X;
					return;
				}
				if (axis != 1)
				{
					position.Z += num;
					this.m_velocity.Z = collisionBox.BlockVelocity.Z;
					return;
				}
				position.Y += num;
				this.m_velocity.Y = collisionBox.BlockVelocity.Y;
				if (move < 0f)
				{
					this.StandingOnValue = new int?(collisionBox.BlockValue);
					this.StandingOnBody = collisionBox.ComponentBody;
					this.StandingOnVelocity = collisionBox.BlockVelocity;
					return;
				}
			}
			else
			{
				if (num2 == 0f)
				{
					return;
				}
				ComponentBody componentBody = collisionBox2.ComponentBody;
				if (axis != 0)
				{
					if (axis != 1)
					{
						ComponentBody.InelasticCollision(this.m_velocity.Z, componentBody.m_velocity.Z, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.Z, out componentBody.m_velocity.Z);
						position.Z += num2;
					}
					else
					{
						ComponentBody.InelasticCollision(this.m_velocity.Y, componentBody.m_velocity.Y, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.Y, out componentBody.m_velocity.Y);
						position.Y += num2;
						if (move < 0f)
						{
							this.StandingOnValue = new int?(collisionBox2.BlockValue);
							this.StandingOnBody = collisionBox2.ComponentBody;
							this.StandingOnVelocity = new Vector3(componentBody.m_velocity.X, 0f, componentBody.m_velocity.Z);
						}
					}
				}
				else
				{
					ComponentBody.InelasticCollision(this.m_velocity.X, componentBody.m_velocity.X, this.Mass, componentBody.Mass, 0.5f, out this.m_velocity.X, out componentBody.m_velocity.X);
					position.X += num2;
				}
				if (this.CollidedWithBody != null)
				{
					this.CollidedWithBody(componentBody);
				}
				if (componentBody.CollidedWithBody != null)
				{
					componentBody.CollidedWithBody(this);
				}
			}
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x00057F4C File Offset: 0x0005614C
		public void FindBodiesCollisionBoxes(Vector3 position, DynamicArray<ComponentBody.CollisionBox> result)
		{
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), 4f, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (componentBody != this && componentBody != this.m_parentBody && componentBody.m_parentBody != this)
				{
					result.Add(new ComponentBody.CollisionBox
					{
						Box = componentBody.BoundingBox,
						ComponentBody = componentBody
					});
				}
			}
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00057FEC File Offset: 0x000561EC
		public void FindMovingBlocksCollisionBoxes(Vector3 position, DynamicArray<ComponentBody.CollisionBox> result)
		{
			Vector3 boxSize = this.BoxSize;
			BoundingBox boundingBox = new BoundingBox(position - new Vector3(boxSize.X / 2f, 0f, boxSize.Z / 2f), position + new Vector3(boxSize.X / 2f, boxSize.Y, boxSize.Z / 2f));
			boundingBox.Min -= new Vector3(1f);
			boundingBox.Max += new Vector3(1f);
			this.m_movingBlockSets.Clear();
			this.m_subsystemMovingBlocks.FindMovingBlocks(boundingBox, false, this.m_movingBlockSets);
			for (int i = 0; i < this.m_movingBlockSets.Count; i++)
			{
				IMovingBlockSet movingBlockSet = this.m_movingBlockSets.Array[i];
				for (int j = 0; j < movingBlockSet.Blocks.Count; j++)
				{
					MovingBlock movingBlock = movingBlockSet.Blocks[j];
					int num = Terrain.ExtractContents(movingBlock.Value);
					Block block = BlocksManager.Blocks[num];
					if (block.IsCollidable)
					{
						BoundingBox[] customCollisionBoxes = block.GetCustomCollisionBoxes(this.m_subsystemTerrain, movingBlock.Value);
						Vector3 v = new Vector3(movingBlock.Offset) + movingBlockSet.Position;
						for (int k = 0; k < customCollisionBoxes.Length; k++)
						{
							result.Add(new ComponentBody.CollisionBox
							{
								Box = new BoundingBox(v + customCollisionBoxes[k].Min, v + customCollisionBoxes[k].Max),
								BlockValue = movingBlock.Value,
								BlockVelocity = movingBlockSet.CurrentVelocity
							});
						}
					}
				}
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x000581E0 File Offset: 0x000563E0
		public void FindTerrainCollisionBoxes(BoundingBox box, DynamicArray<ComponentBody.CollisionBox> result)
		{
			Point3 point = Terrain.ToCell(box.Min);
			Point3 point2 = Terrain.ToCell(box.Max);
			point.Y = MathUtils.Max(point.Y, 0);
			point2.Y = MathUtils.Min(point2.Y, 255);
			if (point.Y > point2.Y)
			{
				return;
			}
			for (int i = point.X; i <= point2.X; i++)
			{
				for (int j = point.Z; j <= point2.Z; j++)
				{
					TerrainChunk chunkAtCell = this.m_subsystemTerrain.Terrain.GetChunkAtCell(i, j);
					if (chunkAtCell != null)
					{
						int num = TerrainChunk.CalculateCellIndex(i & 15, point.Y, j & 15);
						int k = point.Y;
						while (k <= point2.Y)
						{
							int cellValueFast = chunkAtCell.GetCellValueFast(num);
							int num2 = Terrain.ExtractContents(cellValueFast);
							if (num2 != 0)
							{
								Block block = BlocksManager.Blocks[num2];
								if (block.IsCollidable)
								{
									BoundingBox[] customCollisionBoxes = block.GetCustomCollisionBoxes(this.m_subsystemTerrain, cellValueFast);
									Vector3 v = new Vector3((float)i, (float)k, (float)j);
									for (int l = 0; l < customCollisionBoxes.Length; l++)
									{
										result.Add(new ComponentBody.CollisionBox
										{
											Box = new BoundingBox(v + customCollisionBoxes[l].Min, v + customCollisionBoxes[l].Max),
											BlockValue = cellValueFast
										});
									}
								}
							}
							k++;
							num++;
						}
					}
				}
			}
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x00058380 File Offset: 0x00056580
		public void FindSneakCollisionBoxes(Vector3 position, Vector2 overhang, DynamicArray<ComponentBody.CollisionBox> result)
		{
			int num = Terrain.ToCell(position.X);
			int num2 = Terrain.ToCell(position.Y);
			int num3 = Terrain.ToCell(position.Z);
			if (BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num, num2 - 1, num3)].IsCollidable)
			{
				return;
			}
			bool flag = position.X < (float)num + 0.5f;
			bool flag2 = position.Z < (float)num3 + 0.5f;
			if (flag)
			{
				if (flag2)
				{
					bool isCollidable = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num, num2 - 1, num3 - 1)].IsCollidable;
					bool isCollidable2 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num - 1, num2 - 1, num3)].IsCollidable;
					bool isCollidable3 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num - 1, num2 - 1, num3 - 1)].IsCollidable;
					if ((isCollidable && !isCollidable2) || (!isCollidable && !isCollidable2 && isCollidable3))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if ((!isCollidable && isCollidable2) || (!isCollidable && !isCollidable2 && isCollidable3))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if (isCollidable && isCollidable2)
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
						return;
					}
				}
				else
				{
					bool isCollidable4 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num, num2 - 1, num3 + 1)].IsCollidable;
					bool isCollidable5 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num - 1, num2 - 1, num3)].IsCollidable;
					bool isCollidable6 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num - 1, num2 - 1, num3 + 1)].IsCollidable;
					if ((isCollidable4 && !isCollidable5) || (!isCollidable4 && !isCollidable5 && isCollidable6))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
							BlockValue = 0
						};
						result.Add(item);
					}
					if ((!isCollidable4 && isCollidable5) || (!isCollidable4 && !isCollidable5 && isCollidable6))
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
							BlockValue = 0
						};
						result.Add(item);
					}
					if (isCollidable4 && isCollidable5)
					{
						ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
						{
							Box = new BoundingBox(new Vector3((float)num + overhang.X, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
							BlockValue = 0
						};
						result.Add(item);
						return;
					}
				}
			}
			else if (flag2)
			{
				bool isCollidable7 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num, num2 - 1, num3 - 1)].IsCollidable;
				bool isCollidable8 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num + 1, num2 - 1, num3)].IsCollidable;
				bool isCollidable9 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num + 1, num2 - 1, num3 - 1)].IsCollidable;
				if ((isCollidable7 && !isCollidable8) || (!isCollidable7 && !isCollidable8 && isCollidable9))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if ((!isCollidable7 && isCollidable8) || (!isCollidable7 && !isCollidable8 && isCollidable9))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if (isCollidable7 && isCollidable8)
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3 + overhang.Y), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
					return;
				}
			}
			else
			{
				bool isCollidable10 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num, num2 - 1, num3 + 1)].IsCollidable;
				bool isCollidable11 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num + 1, num2 - 1, num3)].IsCollidable;
				bool isCollidable12 = BlocksManager.Blocks[this.m_subsystemTerrain.Terrain.GetCellContents(num + 1, num2 - 1, num3 + 1)].IsCollidable;
				if ((isCollidable10 && !isCollidable11) || (!isCollidable10 && !isCollidable11 && isCollidable12))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1), (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
						BlockValue = 0
					};
					result.Add(item);
				}
				if ((!isCollidable10 && isCollidable11) || (!isCollidable10 && !isCollidable11 && isCollidable12))
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1))),
						BlockValue = 0
					};
					result.Add(item);
				}
				if (isCollidable10 && isCollidable11)
				{
					ComponentBody.CollisionBox item = new ComponentBody.CollisionBox
					{
						Box = new BoundingBox(new Vector3((float)num, (float)num2, (float)num3), new Vector3((float)(num + 1) - overhang.X, (float)(num2 + 1), (float)(num3 + 1) - overhang.Y)),
						BlockValue = 0
					};
					result.Add(item);
				}
			}
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x00058A58 File Offset: 0x00056C58
		public bool IsColliding(BoundingBox box, DynamicArray<ComponentBody.CollisionBox> collisionBoxes)
		{
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				if (box.Intersection(collisionBoxes.Array[i].Box))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x00058A94 File Offset: 0x00056C94
		public float CalculatePushBack(BoundingBox box, int axis, DynamicArray<ComponentBody.CollisionBox> collisionBoxes, out ComponentBody.CollisionBox pushingCollisionBox)
		{
			pushingCollisionBox = default(ComponentBody.CollisionBox);
			float num = 0f;
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				float num2 = ComponentBody.CalculateBoxBoxOverlap(ref box, ref collisionBoxes.Array[i].Box, axis);
				if (MathUtils.Abs(num2) > MathUtils.Abs(num))
				{
					num = num2;
					pushingCollisionBox = collisionBoxes.Array[i];
				}
			}
			return num;
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00058B00 File Offset: 0x00056D00
		public float CalculateSmoothRisePushBack(BoundingBox normalBox, BoundingBox smoothRiseBox, int axis, DynamicArray<ComponentBody.CollisionBox> collisionBoxes, out ComponentBody.CollisionBox pushingCollisionBox)
		{
			pushingCollisionBox = default(ComponentBody.CollisionBox);
			float num = 0f;
			for (int i = 0; i < collisionBoxes.Count; i++)
			{
				float num2 = (!BlocksManager.Blocks[Terrain.ExtractContents(collisionBoxes.Array[i].BlockValue)].NoSmoothRise) ? ComponentBody.CalculateBoxBoxOverlap(ref smoothRiseBox, ref collisionBoxes.Array[i].Box, axis) : ComponentBody.CalculateBoxBoxOverlap(ref normalBox, ref collisionBoxes.Array[i].Box, axis);
				if (MathUtils.Abs(num2) > MathUtils.Abs(num))
				{
					num = num2;
					pushingCollisionBox = collisionBoxes.Array[i];
				}
			}
			return num;
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x00058BB4 File Offset: 0x00056DB4
		public static float CalculateBoxBoxOverlap(ref BoundingBox b1, ref BoundingBox b2, int axis)
		{
			if (b1.Max.X <= b2.Min.X || b1.Min.X >= b2.Max.X || b1.Max.Y <= b2.Min.Y || b1.Min.Y >= b2.Max.Y || b1.Max.Z <= b2.Min.Z || b1.Min.Z >= b2.Max.Z)
			{
				return 0f;
			}
			if (axis == 0)
			{
				float num = b1.Min.X + b1.Max.X;
				float num2 = b2.Min.X + b2.Max.X;
				float num3 = b1.Max.X - b1.Min.X;
				float num4 = b2.Max.X - b2.Min.X;
				float num5 = num2 - num;
				float num6 = num3 + num4;
				return 0.5f * ((num5 > 0f) ? (num5 - num6) : (num5 + num6));
			}
			if (axis != 1)
			{
				float num7 = b1.Min.Z + b1.Max.Z;
				float num8 = b2.Min.Z + b2.Max.Z;
				float num9 = b1.Max.Z - b1.Min.Z;
				float num10 = b2.Max.Z - b2.Min.Z;
				float num11 = num8 - num7;
				float num12 = num9 + num10;
				return 0.5f * ((num11 > 0f) ? (num11 - num12) : (num11 + num12));
			}
			float num13 = b1.Min.Y + b1.Max.Y;
			float num14 = b2.Min.Y + b2.Max.Y;
			float num15 = b1.Max.Y - b1.Min.Y;
			float num16 = b2.Max.Y - b2.Min.Y;
			float num17 = num14 - num13;
			float num18 = num15 + num16;
			return 0.5f * ((num17 > 0f) ? (num17 - num18) : (num17 + num18));
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x00058DF9 File Offset: 0x00056FF9
		public static float GetVectorComponent(Vector3 v, int axis)
		{
			if (axis == 0)
			{
				return v.X;
			}
			if (axis != 1)
			{
				return v.Z;
			}
			return v.Y;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00058E18 File Offset: 0x00057018
		public static void InelasticCollision(float v1, float v2, float m1, float m2, float cr, out float result1, out float result2)
		{
			float num = 1f / (m1 + m2);
			result1 = (cr * m2 * (v2 - v1) + m1 * v1 + m2 * v2) * num;
			result2 = (cr * m1 * (v1 - v2) + m1 * v1 + m2 * v2) * num;
		}

		// Token: 0x0400066A RID: 1642
		public SubsystemTime m_subsystemTime;

		// Token: 0x0400066B RID: 1643
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x0400066C RID: 1644
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x0400066D RID: 1645
		public SubsystemMovingBlocks m_subsystemMovingBlocks;

		// Token: 0x0400066E RID: 1646
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x0400066F RID: 1647
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000670 RID: 1648
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000671 RID: 1649
		public SubsystemFluidBlockBehavior m_subsystemFluidBlockBehavior;

		// Token: 0x04000672 RID: 1650
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000673 RID: 1651
		public DynamicArray<ComponentBody.CollisionBox> m_collisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x04000674 RID: 1652
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x04000675 RID: 1653
		public DynamicArray<IMovingBlockSet> m_movingBlockSets = new DynamicArray<IMovingBlockSet>();

		// Token: 0x04000676 RID: 1654
		public DynamicArray<ComponentBody.CollisionBox> m_bodiesCollisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x04000677 RID: 1655
		public DynamicArray<ComponentBody.CollisionBox> m_movingBlocksCollisionBoxes = new DynamicArray<ComponentBody.CollisionBox>();

		// Token: 0x04000678 RID: 1656
		public ComponentBody m_parentBody;

		// Token: 0x04000679 RID: 1657
		public List<ComponentBody> m_childBodies = new List<ComponentBody>();

		// Token: 0x0400067A RID: 1658
		public Vector3 m_velocity;

		// Token: 0x0400067B RID: 1659
		public bool m_isSneaking;

		// Token: 0x0400067C RID: 1660
		public Vector3 m_totalImpulse;

		// Token: 0x0400067D RID: 1661
		public Vector3 m_directMove;

		// Token: 0x0400067E RID: 1662
		public bool m_fluidEffectsPlayed;

		// Token: 0x0400067F RID: 1663
		public float m_stoppedTime;

		// Token: 0x04000680 RID: 1664
		public static Vector3[] m_freeSpaceOffsets;

		// Token: 0x04000681 RID: 1665
		public static bool DrawBodiesBounds;

		// Token: 0x04000682 RID: 1666
		public const float SleepThresholdSpeed = 1E-05f;

		// Token: 0x04000683 RID: 1667
		public const float MaxSpeed = 25f;

		// Token: 0x02000449 RID: 1097
		public struct CollisionBox
		{
			// Token: 0x04001624 RID: 5668
			public int BlockValue;

			// Token: 0x04001625 RID: 5669
			public Vector3 BlockVelocity;

			// Token: 0x04001626 RID: 5670
			public ComponentBody ComponentBody;

			// Token: 0x04001627 RID: 5671
			public BoundingBox Box;
		}
	}
}
