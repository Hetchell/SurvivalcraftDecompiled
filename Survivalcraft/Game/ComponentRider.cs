using System;
using System.Linq;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FF RID: 511
	public class ComponentRider : Component, IUpdateable
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000F69 RID: 3945 RVA: 0x00075818 File Offset: 0x00073A18
		// (set) Token: 0x06000F6A RID: 3946 RVA: 0x00075820 File Offset: 0x00073A20
		public ComponentCreature ComponentCreature { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000F6B RID: 3947 RVA: 0x00075829 File Offset: 0x00073A29
		public ComponentMount Mount
		{
			get
			{
				if (this.ComponentCreature.ComponentBody.ParentBody != null)
				{
					return this.ComponentCreature.ComponentBody.ParentBody.Entity.FindComponent<ComponentMount>();
				}
				return null;
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x00075859 File Offset: 0x00073A59
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0007585C File Offset: 0x00073A5C
		public ComponentMount FindNearestMount()
		{
			Vector2 point = new Vector2(this.ComponentCreature.ComponentBody.Position.X, this.ComponentCreature.ComponentBody.Position.Z);
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(point, 2.5f, this.m_componentBodies);
			float num = 0f;
			ComponentMount result = null;
			foreach (ComponentMount componentMount in from b in this.m_componentBodies
			select b.Entity.FindComponent<ComponentMount>() into m
			where m != null && m.Entity != base.Entity
			select m)
			{
				float num2 = this.ScoreMount(componentMount, 2.5f);
				if (num2 > num)
				{
					num = num2;
					result = componentMount;
				}
			}
			return result;
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x00075950 File Offset: 0x00073B50
		public void StartMounting(ComponentMount componentMount)
		{
			if (!this.m_isAnimating && this.Mount == null)
			{
				this.m_isAnimating = true;
				this.m_animationTime = 0f;
				this.m_isDismounting = false;
				this.ComponentCreature.ComponentBody.ParentBody = componentMount.ComponentBody;
				this.ComponentCreature.ComponentBody.ParentBodyPositionOffset = Vector3.Transform(this.ComponentCreature.ComponentBody.Position - componentMount.ComponentBody.Position, Quaternion.Conjugate(componentMount.ComponentBody.Rotation));
				this.ComponentCreature.ComponentBody.ParentBodyRotationOffset = Quaternion.Conjugate(componentMount.ComponentBody.Rotation) * this.ComponentCreature.ComponentBody.Rotation;
				this.m_targetPositionOffset = componentMount.MountOffset + this.m_riderOffset;
				this.m_targetRotationOffset = Quaternion.Identity;
				this.ComponentCreature.ComponentLocomotion.IsCreativeFlyEnabled = false;
			}
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x00075A50 File Offset: 0x00073C50
		public void StartDismounting()
		{
			if (!this.m_isAnimating && this.Mount != null)
			{
				float x = 0f;
				if (this.Mount.DismountOffset.X > 0f)
				{
					float s = this.Mount.DismountOffset.X + 0.5f;
					Vector3 vector = 0.5f * (this.ComponentCreature.ComponentBody.BoundingBox.Min + this.ComponentCreature.ComponentBody.BoundingBox.Max);
					TerrainRaycastResult? terrainRaycastResult = this.m_subsystemTerrain.Raycast(vector, vector - s * this.ComponentCreature.ComponentBody.Matrix.Right, false, true, null);
					TerrainRaycastResult? terrainRaycastResult2 = this.m_subsystemTerrain.Raycast(vector, vector + s * this.ComponentCreature.ComponentBody.Matrix.Right, false, true, null);
					x = ((terrainRaycastResult == null) ? (0f - this.Mount.DismountOffset.X) : ((terrainRaycastResult2 == null) ? this.Mount.DismountOffset.X : ((terrainRaycastResult.Value.Distance <= terrainRaycastResult2.Value.Distance) ? MathUtils.Min(terrainRaycastResult2.Value.Distance, this.Mount.DismountOffset.X) : (0f - MathUtils.Min(terrainRaycastResult.Value.Distance, this.Mount.DismountOffset.X)))));
				}
				this.m_isAnimating = true;
				this.m_animationTime = 0f;
				this.m_isDismounting = true;
				this.m_targetPositionOffset = this.Mount.MountOffset + this.m_riderOffset + new Vector3(x, this.Mount.DismountOffset.Y, this.Mount.DismountOffset.Z);
				this.m_targetRotationOffset = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathUtils.Sign(x) * MathUtils.DegToRad(60f));
			}
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x00075C74 File Offset: 0x00073E74
		public void Update(float dt)
		{
			if (this.m_isAnimating)
			{
				float f = 8f * dt;
				ComponentBody componentBody = this.ComponentCreature.ComponentBody;
				componentBody.ParentBodyPositionOffset = Vector3.Lerp(componentBody.ParentBodyPositionOffset, this.m_targetPositionOffset, f);
				componentBody.ParentBodyRotationOffset = Quaternion.Slerp(componentBody.ParentBodyRotationOffset, this.m_targetRotationOffset, f);
				this.m_animationTime += dt;
				if (Vector3.DistanceSquared(componentBody.ParentBodyPositionOffset, this.m_targetPositionOffset) < 0.010000001f || this.m_animationTime > 0.75f)
				{
					this.m_isAnimating = false;
					if (this.m_isDismounting)
					{
						if (componentBody.ParentBody != null)
						{
							componentBody.Velocity = componentBody.ParentBody.Velocity;
							componentBody.ParentBody = null;
						}
					}
					else
					{
						componentBody.ParentBodyPositionOffset = this.m_targetPositionOffset;
						componentBody.ParentBodyRotationOffset = this.m_targetRotationOffset;
						this.m_outOfMountTime = 0f;
					}
				}
			}
			ComponentMount mount = this.Mount;
			if (mount != null && !this.m_isAnimating)
			{
				ComponentBody componentBody2 = this.ComponentCreature.ComponentBody;
				ComponentBody parentBody = this.ComponentCreature.ComponentBody.ParentBody;
				if (Vector3.DistanceSquared(parentBody.Position + Vector3.Transform(componentBody2.ParentBodyPositionOffset, parentBody.Rotation), componentBody2.Position) > 0.16000001f)
				{
					this.m_outOfMountTime += dt;
				}
				else
				{
					this.m_outOfMountTime = 0f;
				}
				ComponentHealth componentHealth = mount.Entity.FindComponent<ComponentHealth>();
				if (this.m_outOfMountTime > 0.1f || (componentHealth != null && componentHealth.Health <= 0f) || this.ComponentCreature.ComponentHealth.Health <= 0f)
				{
					this.StartDismounting();
				}
				this.ComponentCreature.ComponentBody.ParentBodyPositionOffset = mount.MountOffset + this.m_riderOffset;
				this.ComponentCreature.ComponentBody.ParentBodyRotationOffset = Quaternion.Identity;
			}
		}

        // Token: 0x06000F71 RID: 3953 RVA: 0x00075E58 File Offset: 0x00074058
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.ComponentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_riderOffset = valuesDictionary.GetValue<Vector3>("RiderOffset");
		}

		// Token: 0x06000F72 RID: 3954 RVA: 0x00075EAC File Offset: 0x000740AC
		public float ScoreMount(ComponentMount componentMount, float maxDistance)
		{
			if (componentMount.ComponentBody.Velocity.LengthSquared() < 1f)
			{
				Vector3 v = componentMount.ComponentBody.Position + Vector3.Transform(componentMount.MountOffset, componentMount.ComponentBody.Rotation) - this.ComponentCreature.ComponentCreatureModel.EyePosition;
				if (v.Length() < maxDistance)
				{
					Vector3 forward = Matrix.CreateFromQuaternion(this.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
					if (Vector3.Dot(Vector3.Normalize(v), forward) > 0.33f)
					{
						return maxDistance - v.Length();
					}
				}
			}
			return 0f;
		}

		// Token: 0x040009DC RID: 2524
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040009DD RID: 2525
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040009DE RID: 2526
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040009DF RID: 2527
		public Vector3 m_riderOffset;

		// Token: 0x040009E0 RID: 2528
		public float m_animationTime;

		// Token: 0x040009E1 RID: 2529
		public bool m_isAnimating;

		// Token: 0x040009E2 RID: 2530
		public bool m_isDismounting;

		// Token: 0x040009E3 RID: 2531
		public Vector3 m_targetPositionOffset;

		// Token: 0x040009E4 RID: 2532
		public Quaternion m_targetRotationOffset;

		// Token: 0x040009E5 RID: 2533
		public float m_outOfMountTime;
	}
}
