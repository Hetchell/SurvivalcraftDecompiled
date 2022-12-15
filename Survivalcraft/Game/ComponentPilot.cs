using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001FB RID: 507
	public class ComponentPilot : Component, IUpdateable
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x00072DA5 File Offset: 0x00070FA5
		// (set) Token: 0x06000F0B RID: 3851 RVA: 0x00072DAD File Offset: 0x00070FAD
		public Vector3? Destination { get; set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000F0C RID: 3852 RVA: 0x00072DB6 File Offset: 0x00070FB6
		// (set) Token: 0x06000F0D RID: 3853 RVA: 0x00072DBE File Offset: 0x00070FBE
		public float Speed { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000F0E RID: 3854 RVA: 0x00072DC7 File Offset: 0x00070FC7
		// (set) Token: 0x06000F0F RID: 3855 RVA: 0x00072DCF File Offset: 0x00070FCF
		public float Range { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000F10 RID: 3856 RVA: 0x00072DD8 File Offset: 0x00070FD8
		// (set) Token: 0x06000F11 RID: 3857 RVA: 0x00072DE0 File Offset: 0x00070FE0
		public bool IgnoreHeightDifference { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000F12 RID: 3858 RVA: 0x00072DE9 File Offset: 0x00070FE9
		// (set) Token: 0x06000F13 RID: 3859 RVA: 0x00072DF1 File Offset: 0x00070FF1
		public bool RaycastDestination { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000F14 RID: 3860 RVA: 0x00072DFA File Offset: 0x00070FFA
		// (set) Token: 0x06000F15 RID: 3861 RVA: 0x00072E02 File Offset: 0x00071002
		public bool TakeRisks { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x00072E0B File Offset: 0x0007100B
		// (set) Token: 0x06000F17 RID: 3863 RVA: 0x00072E13 File Offset: 0x00071013
		public ComponentBody DoNotAvoidBody { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000F18 RID: 3864 RVA: 0x00072E1C File Offset: 0x0007101C
		// (set) Token: 0x06000F19 RID: 3865 RVA: 0x00072E24 File Offset: 0x00071024
		public bool IsStuck { get; set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000F1A RID: 3866 RVA: 0x00072E2D File Offset: 0x0007102D
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00072E30 File Offset: 0x00071030
		public void SetDestination(Vector3? destination, float speed, float range, bool ignoreHeightDifference, bool raycastDestination, bool takeRisks, ComponentBody doNotAvoidBody)
		{
			bool flag = true;
			if (this.Destination != null && destination != null)
			{
				Vector3 v = Vector3.Normalize(this.Destination.Value - this.m_componentCreature.ComponentBody.Position);
				if (Vector3.Dot(Vector3.Normalize(destination.Value - this.m_componentCreature.ComponentBody.Position), v) > 0.5f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.IsStuck = false;
				this.m_lastStuckCheckPosition = null;
				this.m_aboveBelowTime = null;
			}
			this.Destination = destination;
			this.Speed = speed;
			this.Range = range;
			this.IgnoreHeightDifference = ignoreHeightDifference;
			this.RaycastDestination = raycastDestination;
			this.TakeRisks = takeRisks;
			this.DoNotAvoidBody = doNotAvoidBody;
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x00072F08 File Offset: 0x00071108
		public void Stop()
		{
			this.SetDestination(null, 0f, 0f, false, false, false, null);
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x00072F34 File Offset: 0x00071134
		public void Update(float dt)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextUpdateTime)
			{
				this.m_nextUpdateTime = this.m_subsystemTime.GameTime + (double)this.m_random.Float(0.09f, 0.11f);
				this.m_walkOrder = null;
				this.m_flyOrder = null;
				this.m_swimOrder = null;
				this.m_turnOrder = Vector2.Zero;
				this.m_jumpOrder = 0f;
				if (this.Destination != null)
				{
					Vector3 position = this.m_componentCreature.ComponentBody.Position;
					Vector3 forward = this.m_componentCreature.ComponentBody.Matrix.Forward;
					Vector3 vector = this.AvoidNearestBody(position, this.Destination.Value);
					Vector3 vector2 = vector - position;
					float num = vector2.LengthSquared();
					Vector2 vector3 = new Vector2(vector.X, vector.Z) - new Vector2(position.X, position.Z);
					float num2 = vector3.LengthSquared();
					float x = Vector2.Angle(forward.XZ, vector2.XZ);
					float num3 = ((this.m_componentCreature.ComponentBody.CollisionVelocityChange * new Vector3(1f, 0f, 1f)).LengthSquared() > 0f && this.m_componentCreature.ComponentBody.StandingOnValue != null) ? 0.15f : 0.4f;
					if (this.m_subsystemTime.GameTime >= this.m_lastStuckCheckTime + (double)num3 || this.m_lastStuckCheckPosition == null)
					{
						this.m_lastStuckCheckTime = this.m_subsystemTime.GameTime;
						if (MathUtils.Abs(x) > MathUtils.DegToRad(20f) || this.m_lastStuckCheckPosition == null || Vector3.Dot(position - this.m_lastStuckCheckPosition.Value, Vector3.Normalize(vector2)) > 0.2f)
						{
							this.m_lastStuckCheckPosition = new Vector3?(position);
							this.m_stuckCount = 0;
						}
						else
						{
							this.m_stuckCount++;
						}
						this.IsStuck = (this.m_stuckCount >= 4);
					}
					if (this.m_componentCreature.ComponentLocomotion.FlySpeed > 0f && (num > 9f || vector2.Y > 0.5f || vector2.Y < -1.5f || (this.m_componentCreature.ComponentBody.StandingOnValue == null && this.m_componentCreature.ComponentBody.ImmersionFactor == 0f)) && this.m_componentCreature.ComponentBody.ImmersionFactor < 1f)
					{
						float y = MathUtils.Min(0.08f * vector3.LengthSquared(), 12f);
						Vector3 v = vector + new Vector3(0f, y, 0f);
						Vector3 vector4 = this.Speed * Vector3.Normalize(v - position);
						vector4.Y = MathUtils.Max(vector4.Y, -0.5f);
						this.m_flyOrder = new Vector3?(vector4);
						this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
					}
					else if (this.m_componentCreature.ComponentLocomotion.SwimSpeed > 0f && this.m_componentCreature.ComponentBody.ImmersionFactor > 0.5f)
					{
						Vector3 vector5 = this.Speed * Vector3.Normalize(vector - position);
						vector5.Y = MathUtils.Clamp(vector5.Y, -0.5f, 0.5f);
						this.m_swimOrder = new Vector3?(vector5);
						this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
					}
					else if (this.m_componentCreature.ComponentLocomotion.WalkSpeed > 0f)
					{
						if (this.IsTerrainSafeToGo(position, vector2))
						{
							this.m_turnOrder = new Vector2(MathUtils.Clamp(x, -1f, 1f), 0f);
							if (num2 > 1f)
							{
								this.m_walkOrder = new Vector2?(new Vector2(0f, MathUtils.Lerp(this.Speed, 0f, MathUtils.Saturate((MathUtils.Abs(x) - 0.33f) / 0.66f))));
								if (this.Speed >= 1f && this.m_componentCreature.ComponentLocomotion.InAirWalkFactor >= 1f && num > 1f && this.m_random.Float(0f, 1f) < 0.05f)
								{
									this.m_jumpOrder = 1f;
								}
							}
							else
							{
								float x2 = this.Speed * MathUtils.Min(1f * MathUtils.Sqrt(num2), 1f);
								this.m_walkOrder = new Vector2?(new Vector2(0f, MathUtils.Lerp(x2, 0f, MathUtils.Saturate(2f * MathUtils.Abs(x)))));
							}
						}
						else
						{
							this.IsStuck = true;
						}
						if (num2 < 1f && vector2.Y < -0.1f)
						{
							this.m_componentCreature.ComponentBody.IsSmoothRiseEnabled = false;
						}
						else
						{
							this.m_componentCreature.ComponentBody.IsSmoothRiseEnabled = true;
						}
						if (num2 < 1f && (vector2.Y < -0.5f || vector2.Y > 1f))
						{
							if (vector2.Y > 0f && this.m_random.Float(0f, 1f) < 0.05f)
							{
								this.m_jumpOrder = 1f;
							}
							if (this.m_aboveBelowTime == null)
							{
								this.m_aboveBelowTime = new double?(this.m_subsystemTime.GameTime);
							}
							else if (this.m_subsystemTime.GameTime - this.m_aboveBelowTime.Value > 2.0 && this.m_componentCreature.ComponentBody.StandingOnValue != null)
							{
								this.IsStuck = true;
							}
						}
						else
						{
							this.m_aboveBelowTime = null;
						}
					}
					if ((!this.IgnoreHeightDifference) ? (num <= this.Range * this.Range) : (num2 <= this.Range * this.Range))
					{
						if (this.RaycastDestination)
						{
							if (this.m_subsystemTerrain.Raycast(position + new Vector3(0f, 0.5f, 0f), vector + new Vector3(0f, 0.5f, 0f), false, true, (int value, float distance) => BlocksManager.Blocks[Terrain.ExtractContents(value)].IsCollidable) == null)
							{
								this.Destination = null;
							}
						}
						else
						{
							this.Destination = null;
						}
					}
				}
				if (this.Destination == null && this.m_componentCreature.ComponentLocomotion.FlySpeed > 0f && this.m_componentCreature.ComponentBody.StandingOnValue == null && this.m_componentCreature.ComponentBody.ImmersionFactor == 0f)
				{
					this.m_turnOrder = Vector2.Zero;
					this.m_walkOrder = null;
					this.m_swimOrder = null;
					this.m_flyOrder = new Vector3?(new Vector3(0f, -0.5f, 0f));
				}
			}
			this.m_componentCreature.ComponentLocomotion.WalkOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.WalkOrder, this.m_walkOrder);
			this.m_componentCreature.ComponentLocomotion.SwimOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.SwimOrder, this.m_swimOrder);
			this.m_componentCreature.ComponentLocomotion.TurnOrder += this.m_turnOrder;
			this.m_componentCreature.ComponentLocomotion.FlyOrder = ComponentPilot.CombineNullables(this.m_componentCreature.ComponentLocomotion.FlyOrder, this.m_flyOrder);
			this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_jumpOrder, this.m_componentCreature.ComponentLocomotion.JumpOrder);
			this.m_jumpOrder = 0f;
		}

        // Token: 0x06000F1E RID: 3870 RVA: 0x000737DC File Offset: 0x000719DC
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00073834 File Offset: 0x00071A34
		public bool IsTerrainSafeToGo(Vector3 position, Vector3 direction)
		{
			Vector3 vector = position + new Vector3(0f, 0.1f, 0f) + ((direction.LengthSquared() < 1.2f) ? new Vector3(direction.X, 0f, direction.Z) : (1.2f * Vector3.Normalize(new Vector3(direction.X, 0f, direction.Z))));
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (Vector3.Dot(direction, new Vector3((float)i, 0f, (float)j)) > 0f)
					{
						for (int k = 0; k >= -2; k--)
						{
							int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector.X) + i, Terrain.ToCell(vector.Y) + k, Terrain.ToCell(vector.Z) + j);
							Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
							if (block.ShouldAvoid(cellValue))
							{
								return false;
							}
							if (block.IsCollidable)
							{
								break;
							}
						}
					}
				}
			}
			Vector3 vector2 = position + new Vector3(0f, 0.1f, 0f) + ((direction.LengthSquared() < 1f) ? new Vector3(direction.X, 0f, direction.Z) : (1f * Vector3.Normalize(new Vector3(direction.X, 0f, direction.Z))));
			bool flag = true;
			int num = this.TakeRisks ? 7 : 5;
			for (int l = 0; l >= -num; l--)
			{
				int cellValue2 = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(vector2.X), Terrain.ToCell(vector2.Y) + l, Terrain.ToCell(vector2.Z));
				Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)];
				if ((block2.IsCollidable || block2.BlockIndex == 18) && !block2.ShouldAvoid(cellValue2))
				{
					flag = false;
					break;
				}
			}
			return !flag;
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00073A60 File Offset: 0x00071C60
		public ComponentBody FindNearestBodyInFront(Vector3 position, Vector2 direction)
		{
			if (this.m_subsystemTime.GameTime >= this.m_nextBodiesUpdateTime)
			{
				this.m_nextBodiesUpdateTime = this.m_subsystemTime.GameTime + 0.5;
				this.m_nearbyBodies.Clear();
				this.m_subsystemBodies.FindBodiesAroundPoint(this.m_componentCreature.ComponentBody.Position.XZ, 4f, this.m_nearbyBodies);
			}
			ComponentBody result = null;
			float num = float.MaxValue;
			foreach (ComponentBody componentBody in this.m_nearbyBodies)
			{
				if (componentBody != this.m_componentCreature.ComponentBody && MathUtils.Abs(componentBody.Position.Y - this.m_componentCreature.ComponentBody.Position.Y) <= 1.1f && Vector2.Dot(componentBody.Position.XZ - position.XZ, direction) > 0f)
				{
					float num2 = Vector2.DistanceSquared(componentBody.Position.XZ, position.XZ);
					if (num2 < num)
					{
						num = num2;
						result = componentBody;
					}
				}
			}
			return result;
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x00073BB0 File Offset: 0x00071DB0
		public Vector3 AvoidNearestBody(Vector3 position, Vector3 destination)
		{
			Vector2 v = destination.XZ - position.XZ;
			ComponentBody componentBody = this.FindNearestBodyInFront(position, Vector2.Normalize(v));
			if (componentBody != null && componentBody != this.DoNotAvoidBody)
			{
				float num = 0.72f * (componentBody.BoxSize.X + this.m_componentCreature.ComponentBody.BoxSize.X) + 0.5f;
				Vector2 xz = componentBody.Position.XZ;
				Vector2 v2 = Segment2.NearestPoint(new Segment2(position.XZ, destination.XZ), xz) - xz;
				if (v2.LengthSquared() < num * num)
				{
					float num2 = v.Length();
					Vector2 vector = Vector2.Normalize(xz + Vector2.Normalize(v2) * num - position.XZ);
					if (Vector2.Dot(v / num2, vector) > 0.5f)
					{
						return new Vector3(position.X + vector.X * num2, destination.Y, position.Z + vector.Y * num2);
					}
				}
			}
			return destination;
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x00073CD0 File Offset: 0x00071ED0
		public static Vector2? CombineNullables(Vector2? v1, Vector2? v2)
		{
			if (v1 == null)
			{
				return v2;
			}
			if (v2 == null)
			{
				return v1;
			}
			return new Vector2?(v1.Value + v2.Value);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x00073D00 File Offset: 0x00071F00
		public static Vector3? CombineNullables(Vector3? v1, Vector3? v2)
		{
			if (v1 == null)
			{
				return v2;
			}
			if (v2 == null)
			{
				return v1;
			}
			return new Vector3?(v1.Value + v2.Value);
		}

		// Token: 0x04000992 RID: 2450
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000993 RID: 2451
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000994 RID: 2452
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000995 RID: 2453
		public ComponentCreature m_componentCreature;

		// Token: 0x04000996 RID: 2454
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000997 RID: 2455
		public Vector2? m_walkOrder;

		// Token: 0x04000998 RID: 2456
		public Vector3? m_flyOrder;

		// Token: 0x04000999 RID: 2457
		public Vector3? m_swimOrder;

		// Token: 0x0400099A RID: 2458
		public Vector2 m_turnOrder;

		// Token: 0x0400099B RID: 2459
		public float m_jumpOrder;

		// Token: 0x0400099C RID: 2460
		public double m_nextUpdateTime;

		// Token: 0x0400099D RID: 2461
		public double m_lastStuckCheckTime;

		// Token: 0x0400099E RID: 2462
		public int m_stuckCount;

		// Token: 0x0400099F RID: 2463
		public double? m_aboveBelowTime;

		// Token: 0x040009A0 RID: 2464
		public Vector3? m_lastStuckCheckPosition;

		// Token: 0x040009A1 RID: 2465
		public DynamicArray<ComponentBody> m_nearbyBodies = new DynamicArray<ComponentBody>();

		// Token: 0x040009A2 RID: 2466
		public double m_nextBodiesUpdateTime;

		// Token: 0x040009A3 RID: 2467
		public static bool DrawPilotDestination;
	}
}
