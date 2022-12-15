using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000177 RID: 375
	public class SubsystemExplosions : Subsystem, IUpdateable
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600083E RID: 2110 RVA: 0x00036678 File Offset: 0x00034878
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0003667C File Offset: 0x0003487C
		public bool TryExplodeBlock(int x, int y, int z, int value)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			float explosionPressure = block.GetExplosionPressure(value);
			bool explosionIncendiary = block.GetExplosionIncendiary(value);
			if (explosionPressure > 0f)
			{
				this.AddExplosion(x, y, z, explosionPressure, explosionIncendiary, false);
				return true;
			}
			return false;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x000366C0 File Offset: 0x000348C0
		public void AddExplosion(int x, int y, int z, float pressure, bool isIncendiary, bool noExplosionSound)
		{
			if (pressure > 0f)
			{
				this.m_queuedExplosions.Add(new SubsystemExplosions.ExplosionData
				{
					X = x,
					Y = y,
					Z = z,
					Pressure = pressure,
					IsIncendiary = isIncendiary,
					NoExplosionSound = noExplosionSound
				});
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00036720 File Offset: 0x00034920
		public void Update(float dt)
		{
			if (this.m_queuedExplosions.Count <= 0)
			{
				return;
			}
			int x = this.m_queuedExplosions[0].X;
			int y = this.m_queuedExplosions[0].Y;
			int z = this.m_queuedExplosions[0].Z;
			this.m_pressureByPoint = new SubsystemExplosions.SparseSpatialArray<float>(x, y, z, 0f);
			this.m_surroundingPressureByPoint = new SubsystemExplosions.SparseSpatialArray<SubsystemExplosions.SurroundingPressurePoint>(x, y, z, new SubsystemExplosions.SurroundingPressurePoint
			{
				IsIncendiary = false,
				Pressure = 0f
			});
			this.m_projectilesCount = 0;
			this.m_generatedProjectiles.Clear();
			bool flag = false;
			int i = 0;
			while (i < this.m_queuedExplosions.Count)
			{
				SubsystemExplosions.ExplosionData explosionData = this.m_queuedExplosions[i];
				if (MathUtils.Abs(explosionData.X - x) <= 4 && MathUtils.Abs(explosionData.Y - y) <= 4 && MathUtils.Abs(explosionData.Z - z) <= 4)
				{
					this.m_queuedExplosions.RemoveAt(i);
					this.SimulateExplosion(explosionData.X, explosionData.Y, explosionData.Z, explosionData.Pressure, explosionData.IsIncendiary);
					flag |= !explosionData.NoExplosionSound;
				}
				else
				{
					i++;
				}
			}
			this.PostprocessExplosions(flag);
			if (!this.ShowExplosionPressure)
			{
				this.m_pressureByPoint = null;
				this.m_surroundingPressureByPoint = null;
			}
		}

        // Token: 0x06000842 RID: 2114 RVA: 0x0003688C File Offset: 0x00034A8C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemNoise = base.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
			this.m_subsystemPickables = base.Project.FindSubsystem<SubsystemPickables>(true);
			this.m_subsystemProjectiles = base.Project.FindSubsystem<SubsystemProjectiles>(true);
			this.m_subsystemBlockBehaviors = base.Project.FindSubsystem<SubsystemBlockBehaviors>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_explosionParticleSystem = new ExplosionParticleSystem();
			this.m_subsystemParticles.AddParticleSystem(this.m_explosionParticleSystem);
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x00036958 File Offset: 0x00034B58
		public void SimulateExplosion(int x, int y, int z, float pressure, bool isIncendiary)
		{
			float num = MathUtils.Max(0.13f * MathUtils.Pow(pressure, 0.5f), 1f);
			this.m_subsystemTerrain.ChangeCell(x, y, z, Terrain.MakeBlockValue(0), true);
			SubsystemExplosions.SparseSpatialArray<bool> processed = new SubsystemExplosions.SparseSpatialArray<bool>(x, y, z, true);
			List<SubsystemExplosions.ProcessPoint> list = new List<SubsystemExplosions.ProcessPoint>();
			List<SubsystemExplosions.ProcessPoint> list2 = new List<SubsystemExplosions.ProcessPoint>();
			List<SubsystemExplosions.ProcessPoint> list3 = new List<SubsystemExplosions.ProcessPoint>();
			this.TryAddPoint(x, y, z, -1, pressure, isIncendiary, list, processed);
			int num2 = 0;
			int num3 = 0;
			while (list.Count > 0 || list2.Count > 0)
			{
				num2 += list.Count;
				num3++;
				float num4 = 5f * (float)MathUtils.Max(num3 - 7, 0);
				float num5 = pressure / (MathUtils.Pow((float)num2, 0.66f) + num4);
				if (num5 >= num)
				{
					foreach (SubsystemExplosions.ProcessPoint processPoint in list)
					{
						float num6 = this.m_pressureByPoint.Get(processPoint.X, processPoint.Y, processPoint.Z);
						float num7 = num5 + num6;
						this.m_pressureByPoint.Set(processPoint.X, processPoint.Y, processPoint.Z, num7);
						if (processPoint.Axis == 0)
						{
							this.TryAddPoint(processPoint.X - 1, processPoint.Y, processPoint.Z, 0, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X + 1, processPoint.Y, processPoint.Z, 0, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y - 1, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y + 1, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z - 1, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z + 1, -1, num7, isIncendiary, list2, processed);
						}
						else if (processPoint.Axis == 1)
						{
							this.TryAddPoint(processPoint.X - 1, processPoint.Y, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X + 1, processPoint.Y, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y - 1, processPoint.Z, 1, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y + 1, processPoint.Z, 1, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z - 1, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z + 1, -1, num7, isIncendiary, list2, processed);
						}
						else if (processPoint.Axis == 2)
						{
							this.TryAddPoint(processPoint.X - 1, processPoint.Y, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X + 1, processPoint.Y, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y - 1, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y + 1, processPoint.Z, -1, num7, isIncendiary, list2, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z - 1, 2, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z + 1, 2, num7, isIncendiary, list3, processed);
						}
						else
						{
							this.TryAddPoint(processPoint.X - 1, processPoint.Y, processPoint.Z, 0, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X + 1, processPoint.Y, processPoint.Z, 0, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y - 1, processPoint.Z, 1, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y + 1, processPoint.Z, 1, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z - 1, 2, num7, isIncendiary, list3, processed);
							this.TryAddPoint(processPoint.X, processPoint.Y, processPoint.Z + 1, 2, num7, isIncendiary, list3, processed);
						}
					}
				}
				List<SubsystemExplosions.ProcessPoint> list4 = list;
				list4.Clear();
				list = list2;
				list2 = list3;
				list3 = list4;
			}
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00036E70 File Offset: 0x00035070
		public void TryAddPoint(int x, int y, int z, int axis, float currentPressure, bool isIncendiary, List<SubsystemExplosions.ProcessPoint> toProcess, SubsystemExplosions.SparseSpatialArray<bool> processed)
		{
			if (processed.Get(x, y, z))
			{
				return;
			}
			int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (num != 0)
			{
				int num2 = (int)(MathUtils.Hash((uint)(x + 913 * y + 217546 * z)) % 100U);
				float num3 = MathUtils.Lerp(1f, 2f, (float)num2 / 100f);
				if (num2 % 8 == 0)
				{
					num3 *= 3f;
				}
				Block block = BlocksManager.Blocks[num];
				float num4 = this.m_pressureByPoint.Get(x - 1, y, z) + this.m_pressureByPoint.Get(x + 1, y, z) + this.m_pressureByPoint.Get(x, y - 1, z) + this.m_pressureByPoint.Get(x, y + 1, z) + this.m_pressureByPoint.Get(x, y, z - 1) + this.m_pressureByPoint.Get(x, y, z + 1);
				float num5 = MathUtils.Max(block.ExplosionResilience * num3, 1f);
				float num6 = num4 / num5;
				if (num6 > 1f)
				{
					int newValue = Terrain.MakeBlockValue(0);
					this.m_subsystemTerrain.DestroyCell(0, x, y, z, newValue, true, true);
					bool flag = false;
					float probability = (num6 > 5f) ? 0.95f : 0.75f;
					if (this.m_random.Bool(probability))
					{
						flag = this.TryExplodeBlock(x, y, z, cellValue);
					}
					if (flag)
					{
						goto IL_4A2;
					}
					Vector3 v;
					float num7;
					this.CalculateImpulseAndDamage(new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f), 60f, new float?(2f * num4), out v, out num7);
					bool flag2 = false;
					List<BlockDropValue> list = new List<BlockDropValue>();
					bool flag3;
					block.GetDropValues(this.m_subsystemTerrain, cellValue, newValue, 0, list, out flag3);
					if (list.Count == 0)
					{
						list.Add(new BlockDropValue
						{
							Value = cellValue,
							Count = 1
						});
						flag2 = true;
					}
					using (List<BlockDropValue>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BlockDropValue blockDropValue = enumerator.Current;
							int num8 = Terrain.ExtractContents(blockDropValue.Value);
							if (!(BlocksManager.Blocks[num8] is FluidBlock))
							{
								float num9 = (this.m_projectilesCount < 40) ? 1f : ((this.m_projectilesCount < 60) ? 0.5f : ((this.m_projectilesCount >= 80) ? 0.125f : 0.25f));
								if (this.m_random.Float(0f, 1f) < num9)
								{
									Vector3 vector = v + this.m_random.Vector3(0.05f * v.Length());
									if (this.m_projectilesCount >= 1)
									{
										vector *= this.m_random.Float(0.5f, 1f);
										vector += this.m_random.Vector3(0.2f * vector.Length());
									}
									float num10 = flag2 ? 0f : MathUtils.Lerp(1f, 0f, (float)this.m_projectilesCount / 20f);
									Projectile projectile = this.m_subsystemProjectiles.AddProjectile(blockDropValue.Value, new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f), vector, this.m_random.Vector3(0f, 20f), null);
									projectile.ProjectileStoppedAction = ((this.m_random.Float(0f, 1f) >= num10) ? ProjectileStoppedAction.Disappear : ProjectileStoppedAction.TurnIntoPickable);
									if (this.m_random.Float(0f, 1f) < 0.5f && this.m_projectilesCount < 35)
									{
										float num11 = (num4 > 60f) ? this.m_random.Float(3f, 7f) : this.m_random.Float(1f, 3f);
										if (isIncendiary)
										{
											num11 += 10f;
										}
										this.m_subsystemProjectiles.AddTrail(projectile, Vector3.Zero, new SmokeTrailParticleSystem(15, this.m_random.Float(0.75f, 1.5f), num11, isIncendiary ? new Color(255, 140, 192) : Color.White));
										projectile.IsIncendiary = isIncendiary;
									}
									this.m_generatedProjectiles.Add(projectile, true);
									this.m_projectilesCount++;
								}
							}
						}
						goto IL_4A2;
					}
				}
				this.m_surroundingPressureByPoint.Set(x, y, z, new SubsystemExplosions.SurroundingPressurePoint
				{
					Pressure = num4,
					IsIncendiary = isIncendiary
				});
				if (block.IsCollidable)
				{
					return;
				}
			}
			IL_4A2:
			toProcess.Add(new SubsystemExplosions.ProcessPoint
			{
				X = x,
				Y = y,
				Z = z,
				Axis = axis
			});
			processed.Set(x, y, z, true);
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00037378 File Offset: 0x00035578
		public void PostprocessExplosions(bool playExplosionSound)
		{
			Point3 point = Point3.Zero;
			float num = float.MaxValue;
			float num2 = 0f;
			foreach (KeyValuePair<Point3, float> keyValuePair in this.m_pressureByPoint.ToDictionary())
			{
				num2 += keyValuePair.Value;
				float num3 = this.m_subsystemAudio.CalculateListenerDistance(new Vector3(keyValuePair.Key));
				if (num3 < num)
				{
					num = num3;
					point = keyValuePair.Key;
				}
				float num4 = 0.001f * MathUtils.Pow(num2, 0.5f);
				float num5 = MathUtils.Saturate(keyValuePair.Value / 15f - num4) * this.m_random.Float(0.2f, 1f);
				if (num5 > 0.1f)
				{
					this.m_explosionParticleSystem.SetExplosionCell(keyValuePair.Key, num5);
				}
			}
			foreach (KeyValuePair<Point3, SubsystemExplosions.SurroundingPressurePoint> keyValuePair2 in this.m_surroundingPressureByPoint.ToDictionary())
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(keyValuePair2.Key.X, keyValuePair2.Key.Y, keyValuePair2.Key.Z);
				int num6 = Terrain.ExtractContents(cellValue);
				SubsystemBlockBehavior[] blockBehaviors = this.m_subsystemBlockBehaviors.GetBlockBehaviors(num6);
				if (blockBehaviors.Length != 0)
				{
					for (int i = 0; i < blockBehaviors.Length; i++)
					{
						blockBehaviors[i].OnExplosion(cellValue, keyValuePair2.Key.X, keyValuePair2.Key.Y, keyValuePair2.Key.Z, keyValuePair2.Value.Pressure);
					}
				}
				float probability = keyValuePair2.Value.IsIncendiary ? 0.5f : 0.2f;
				Block block = BlocksManager.Blocks[num6];
				if (block.FireDuration > 0f && keyValuePair2.Value.Pressure / block.ExplosionResilience > 0.2f && this.m_random.Bool(probability))
				{
					this.m_subsystemFireBlockBehavior.SetCellOnFire(keyValuePair2.Key.X, keyValuePair2.Key.Y, keyValuePair2.Key.Z, keyValuePair2.Value.IsIncendiary ? 1f : 0.3f);
				}
			}
			foreach (ComponentBody componentBody in this.m_subsystemBodies.Bodies)
			{
				Vector3 vector;
				float num7;
				this.CalculateImpulseAndDamage(componentBody, null, out vector, out num7);
				vector *= this.m_random.Float(0.5f, 1.5f);
				num7 *= this.m_random.Float(0.5f, 1.5f);
				componentBody.ApplyImpulse(vector);
				ComponentHealth componentHealth = componentBody.Entity.FindComponent<ComponentHealth>();
				if (componentHealth != null)
				{
					componentHealth.Injure(num7, null, false, "Blasted by explosion");
				}
				ComponentDamage componentDamage = componentBody.Entity.FindComponent<ComponentDamage>();
				if (componentDamage != null)
				{
					componentDamage.Damage(num7);
				}
				ComponentOnFire componentOnFire = componentBody.Entity.FindComponent<ComponentOnFire>();
				if (componentOnFire != null && this.m_random.Float(0f, 1f) < MathUtils.Min(num7 - 0.1f, 0.5f))
				{
					componentOnFire.SetOnFire(null, this.m_random.Float(6f, 8f));
				}
			}
			foreach (Pickable pickable in this.m_subsystemPickables.Pickables)
			{
				Block block2 = BlocksManager.Blocks[Terrain.ExtractContents(pickable.Value)];
				Vector3 v;
				float num8;
				this.CalculateImpulseAndDamage(pickable.Position + new Vector3(0f, 0.5f, 0f), 20f, null, out v, out num8);
				if (num8 / block2.ExplosionResilience > 0.1f)
				{
					this.TryExplodeBlock(Terrain.ToCell(pickable.Position.X), Terrain.ToCell(pickable.Position.Y), Terrain.ToCell(pickable.Position.Z), pickable.Value);
					pickable.ToRemove = true;
				}
				else
				{
					Vector3 v2 = (v + new Vector3(0f, 0.1f * v.Length(), 0f)) * this.m_random.Float(0.75f, 1f);
					if (v2.Length() > 10f)
					{
						Projectile projectile = this.m_subsystemProjectiles.AddProjectile(pickable.Value, pickable.Position, pickable.Velocity + v2, this.m_random.Vector3(0f, 20f), null);
						if (this.m_random.Float(0f, 1f) < 0.33f)
						{
							this.m_subsystemProjectiles.AddTrail(projectile, Vector3.Zero, new SmokeTrailParticleSystem(15, this.m_random.Float(0.75f, 1.5f), this.m_random.Float(1f, 6f), Color.White));
						}
						pickable.ToRemove = true;
					}
					else
					{
						pickable.Velocity += v2;
					}
				}
			}
			foreach (Projectile projectile2 in this.m_subsystemProjectiles.Projectiles)
			{
				if (!this.m_generatedProjectiles.ContainsKey(projectile2))
				{
					Vector3 v3;
					float num9;
					this.CalculateImpulseAndDamage(projectile2.Position + new Vector3(0f, 0.5f, 0f), 20f, null, out v3, out num9);
					projectile2.Velocity += (v3 + new Vector3(0f, 0.1f * v3.Length(), 0f)) * this.m_random.Float(0.75f, 1f);
				}
			}
			Vector3 position = new Vector3((float)point.X, (float)point.Y, (float)point.Z);
			float delay = this.m_subsystemAudio.CalculateDelay(num);
			if (num2 > 1000000f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionEnormous", 1f, this.m_random.Float(-0.1f, 0.1f), position, 40f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 100f);
				return;
			}
			if (num2 > 100000f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionHuge", 1f, this.m_random.Float(-0.2f, 0.2f), position, 30f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 70f);
				return;
			}
			if (num2 > 20000f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionLarge", 1f, this.m_random.Float(-0.2f, 0.2f), position, 26f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 50f);
				return;
			}
			if (num2 > 4000f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionMedium", 1f, this.m_random.Float(-0.2f, 0.2f), position, 24f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 40f);
				return;
			}
			if (num2 > 100f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionSmall", 1f, this.m_random.Float(-0.2f, 0.2f), position, 22f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 35f);
				return;
			}
			if (num2 > 0f)
			{
				if (playExplosionSound)
				{
					this.m_subsystemAudio.PlaySound("Audio/ExplosionTiny", 1f, this.m_random.Float(-0.2f, 0.2f), position, 20f, delay);
				}
				this.m_subsystemNoise.MakeNoise(position, 1f, 30f);
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x00037C84 File Offset: 0x00035E84
		public void CalculateImpulseAndDamage(ComponentBody componentBody, float? obstaclePressure, out Vector3 impulse, out float damage)
		{
			this.CalculateImpulseAndDamage(0.5f * (componentBody.BoundingBox.Min + componentBody.BoundingBox.Max), componentBody.Mass, obstaclePressure, out impulse, out damage);
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x00037CBC File Offset: 0x00035EBC
		public void CalculateImpulseAndDamage(Vector3 position, float mass, float? obstaclePressure, out Vector3 impulse, out float damage)
		{
			Point3 point = Terrain.ToCell(position);
			if (obstaclePressure == null)
			{
				obstaclePressure = new float?(this.m_pressureByPoint.Get(point.X, point.Y, point.Z));
			}
			float num = 0f;
			Vector3 vector = Vector3.Zero;
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						int num2 = point.X + i;
						int num3 = point.Y + j;
						int num4 = point.Z + k;
						float num5 = (this.m_subsystemTerrain.Terrain.GetCellContents(num2, num3, num4) != 0) ? obstaclePressure.Value : this.m_pressureByPoint.Get(num2, num3, num4);
						if (i != 0 || j != 0 || k != 0)
						{
							vector += num5 * Vector3.Normalize(new Vector3((float)(point.X - num2), (float)(point.Y - num3), (float)(point.Z - num4)));
						}
						num += num5;
					}
				}
			}
			float num6 = MathUtils.Max(MathUtils.Pow(mass, 0.5f), 1f);
			impulse = 9.259259f * Vector3.Normalize(vector) * MathUtils.Pow(vector.Length(), 0.5f) / num6;
			damage = 2.5925925f * MathUtils.Pow(num, 0.5f) / num6;
		}

		// Token: 0x0400044F RID: 1103
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000450 RID: 1104
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000451 RID: 1105
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000452 RID: 1106
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x04000453 RID: 1107
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x04000454 RID: 1108
		public SubsystemPickables m_subsystemPickables;

		// Token: 0x04000455 RID: 1109
		public SubsystemProjectiles m_subsystemProjectiles;

		// Token: 0x04000456 RID: 1110
		public SubsystemBlockBehaviors m_subsystemBlockBehaviors;

		// Token: 0x04000457 RID: 1111
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x04000458 RID: 1112
		public List<SubsystemExplosions.ExplosionData> m_queuedExplosions = new List<SubsystemExplosions.ExplosionData>();

		// Token: 0x04000459 RID: 1113
		public SubsystemExplosions.SparseSpatialArray<float> m_pressureByPoint;

		// Token: 0x0400045A RID: 1114
		public SubsystemExplosions.SparseSpatialArray<SubsystemExplosions.SurroundingPressurePoint> m_surroundingPressureByPoint;

		// Token: 0x0400045B RID: 1115
		public int m_projectilesCount;

		// Token: 0x0400045C RID: 1116
		public Dictionary<Projectile, bool> m_generatedProjectiles = new Dictionary<Projectile, bool>();

		// Token: 0x0400045D RID: 1117
		public Game.Random m_random = new Game.Random();

		// Token: 0x0400045E RID: 1118
		public ExplosionParticleSystem m_explosionParticleSystem;

		// Token: 0x0400045F RID: 1119
		public bool ShowExplosionPressure;

		// Token: 0x0200041D RID: 1053
		public class SparseSpatialArray<T>
		{
			// Token: 0x06001E4A RID: 7754 RVA: 0x000DE5F0 File Offset: 0x000DC7F0
			public SparseSpatialArray(int centerX, int centerY, int centerZ, T outside)
			{
				this.m_data = new T[4096][];
				this.m_originX = centerX - 128;
				this.m_originY = centerY - 128;
				this.m_originZ = centerZ - 128;
				this.m_outside = outside;
			}

			// Token: 0x06001E4B RID: 7755 RVA: 0x000DE644 File Offset: 0x000DC844
			public T Get(int x, int y, int z)
			{
				x -= this.m_originX;
				y -= this.m_originY;
				z -= this.m_originZ;
				if (x < 0 || x >= 256 || y < 0 || y >= 256 || z < 0 || z >= 256)
				{
					return this.m_outside;
				}
				int num = x >> 4;
				int num2 = y >> 4;
				int num3 = z >> 4;
				int num4 = num + (num2 << 4) + (num3 << 4 << 4);
				T[] array = this.m_data[num4];
				if (array != null)
				{
					int num5 = x & 15;
					int num6 = y & 15;
					int num7 = z & 15;
					int num8 = num5 + (num6 << 4) + (num7 << 4 << 4);
					return array[num8];
				}
				return default(T);
			}

			// Token: 0x06001E4C RID: 7756 RVA: 0x000DE6F0 File Offset: 0x000DC8F0
			public void Set(int x, int y, int z, T value)
			{
				x -= this.m_originX;
				y -= this.m_originY;
				z -= this.m_originZ;
				if (x >= 0 && x < 256 && y >= 0 && y < 256 && z >= 0 && z < 256)
				{
					int num = x >> 4;
					int num2 = y >> 4;
					int num3 = z >> 4;
					int num4 = num + (num2 << 4) + (num3 << 4 << 4);
					T[] array = this.m_data[num4];
					if (array == null)
					{
						array = new T[4096];
						this.m_data[num4] = array;
					}
					int num5 = x & 15;
					int num6 = y & 15;
					int num7 = z & 15;
					int num8 = num5 + (num6 << 4) + (num7 << 4 << 4);
					array[num8] = value;
				}
			}

			// Token: 0x06001E4D RID: 7757 RVA: 0x000DE7A0 File Offset: 0x000DC9A0
			public void Clear()
			{
				for (int i = 0; i < this.m_data.Length; i++)
				{
					this.m_data[i] = null;
				}
			}

			// Token: 0x06001E4E RID: 7758 RVA: 0x000DE7CC File Offset: 0x000DC9CC
			public Dictionary<Point3, T> ToDictionary()
			{
				Dictionary<Point3, T> dictionary = new Dictionary<Point3, T>();
				for (int i = 0; i < this.m_data.Length; i++)
				{
					T[] array = this.m_data[i];
					if (array != null)
					{
						int num = this.m_originX + ((i & 15) << 4);
						int num2 = this.m_originY + ((i >> 4 & 15) << 4);
						int num3 = this.m_originZ + ((i >> 8 & 15) << 4);
						for (int j = 0; j < array.Length; j++)
						{
							if (!object.Equals(array[j], default(T)))
							{
								int num4 = j & 15;
								int num5 = j >> 4 & 15;
								int num6 = j >> 8 & 15;
								dictionary.Add(new Point3(num + num4, num2 + num5, num3 + num6), array[j]);
							}
						}
					}
				}
				return dictionary;
			}

			// Token: 0x04001563 RID: 5475
			public const int m_bits1 = 4;

			// Token: 0x04001564 RID: 5476
			public const int m_bits2 = 4;

			// Token: 0x04001565 RID: 5477
			public const int m_mask1 = 15;

			// Token: 0x04001566 RID: 5478
			public const int m_mask2 = 15;

			// Token: 0x04001567 RID: 5479
			public const int m_diameter = 256;

			// Token: 0x04001568 RID: 5480
			public int m_originX;

			// Token: 0x04001569 RID: 5481
			public int m_originY;

			// Token: 0x0400156A RID: 5482
			public int m_originZ;

			// Token: 0x0400156B RID: 5483
			public T[][] m_data;

			// Token: 0x0400156C RID: 5484
			public T m_outside;
		}

		// Token: 0x0200041E RID: 1054
		public struct ExplosionData
		{
			// Token: 0x0400156D RID: 5485
			public int X;

			// Token: 0x0400156E RID: 5486
			public int Y;

			// Token: 0x0400156F RID: 5487
			public int Z;

			// Token: 0x04001570 RID: 5488
			public float Pressure;

			// Token: 0x04001571 RID: 5489
			public bool IsIncendiary;

			// Token: 0x04001572 RID: 5490
			public bool NoExplosionSound;
		}

		// Token: 0x0200041F RID: 1055
		public struct ProcessPoint
		{
			// Token: 0x04001573 RID: 5491
			public int X;

			// Token: 0x04001574 RID: 5492
			public int Y;

			// Token: 0x04001575 RID: 5493
			public int Z;

			// Token: 0x04001576 RID: 5494
			public int Axis;
		}

		// Token: 0x02000420 RID: 1056
		public struct SurroundingPressurePoint
		{
			// Token: 0x04001577 RID: 5495
			public float Pressure;

			// Token: 0x04001578 RID: 5496
			public bool IsIncendiary;
		}
	}
}
