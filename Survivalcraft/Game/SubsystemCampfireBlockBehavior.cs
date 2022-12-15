using System;
using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000169 RID: 361
	public class SubsystemCampfireBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000723 RID: 1827 RVA: 0x0002D8B2 File Offset: 0x0002BAB2
		public Dictionary<Point3, FireParticleSystem>.KeyCollection Campfires
		{
			get
			{
				return this.m_particleSystemsByCell.Keys;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000724 RID: 1828 RVA: 0x0002D8BF File Offset: 0x0002BABF
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000725 RID: 1829 RVA: 0x0002D8C2 File Offset: 0x0002BAC2
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0002D8CC File Offset: 0x0002BACC
		public void Update(float dt)
		{
			if (this.m_subsystemTime.PeriodicGameTimeEvent(5.0, 0.0))
			{
				this.m_updateIndex++;
				foreach (Point3 point in this.m_particleSystemsByCell.Keys)
				{
					PrecipitationShaftInfo precipitationShaftInfo = this.m_subsystemWeather.GetPrecipitationShaftInfo(point.X, point.Z);
					if ((precipitationShaftInfo.Intensity > 0f && point.Y >= precipitationShaftInfo.YLimit - 1) || this.m_updateIndex % 5 == 0)
					{
						this.m_toReduce.Add(point);
					}
				}
				foreach (Point3 point2 in this.m_toReduce)
				{
					this.ResizeCampfire(point2.X, point2.Y, point2.Z, -1, true);
				}
				this.m_toReduce.Clear();
			}
			if (Time.PeriodicEvent(0.5, 0.0))
			{
				float num = float.MaxValue;
				foreach (Point3 point3 in this.m_particleSystemsByCell.Keys)
				{
					float x = this.m_subsystemAmbientSounds.SubsystemAudio.CalculateListenerDistanceSquared(new Vector3((float)point3.X, (float)point3.Y, (float)point3.Z));
					num = MathUtils.Min(num, x);
				}
				this.m_fireSoundVolume = this.m_subsystemAmbientSounds.SubsystemAudio.CalculateVolume(MathUtils.Sqrt(num), 2f, 2f);
			}
			this.m_subsystemAmbientSounds.FireSoundVolume = MathUtils.Max(this.m_subsystemAmbientSounds.FireSoundVolume, this.m_fireSoundVolume);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0002DAE4 File Offset: 0x0002BCE4
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y - 1, z);
			if (BlocksManager.Blocks[cellContents].IsTransparent)
			{
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			}
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0002DB27 File Offset: 0x0002BD27
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0002DB35 File Offset: 0x0002BD35
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.RemoveCampfireParticleSystem(x, y, z);
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0002DB42 File Offset: 0x0002BD42
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.RemoveCampfireParticleSystem(x, y, z);
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x0002DB5B File Offset: 0x0002BD5B
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.AddCampfireParticleSystem(value, x, y, z);
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0002DB68 File Offset: 0x0002BD68
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_particleSystemsByCell.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.ResizeCampfire(point2.X, point2.Y, point2.Z, -15, false);
				this.RemoveCampfireParticleSystem(point2.X, point2.Y, point2.Z);
			}
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x0002DC8C File Offset: 0x0002BE8C
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!worldItem.ToRemove)
			{
				int x = cellFace.X;
				int y = cellFace.Y;
				int z = cellFace.Z;
				int value = worldItem.Value;
				Pickable pickable = worldItem as Pickable;
				if (this.AddFuel(x, y, z, value, (pickable != null) ? pickable.Count : 1))
				{
					worldItem.ToRemove = true;
				}
			}
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x0002DCDA File Offset: 0x0002BEDA
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			if (this.AddFuel(raycastResult.CellFace.X, raycastResult.CellFace.Y, raycastResult.CellFace.Z, componentMiner.ActiveBlockValue, 1))
			{
				componentMiner.RemoveActiveTool(1);
			}
			return true;
		}

        // Token: 0x0600072F RID: 1839 RVA: 0x0002DD14 File Offset: 0x0002BF14
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemWeather = base.Project.FindSubsystem<SubsystemWeather>(true);
			this.m_subsystemAmbientSounds = base.Project.FindSubsystem<SubsystemAmbientSounds>(true);
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x0002DD70 File Offset: 0x0002BF70
		public void AddCampfireParticleSystem(int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num > 0)
			{
				Vector3 v = new Vector3(0.5f, 0.15f, 0.5f);
				float size = MathUtils.Lerp(0.2f, 0.5f, (float)num / 15f);
				FireParticleSystem fireParticleSystem = new FireParticleSystem(new Vector3((float)x, (float)y, (float)z) + v, size, 256f);
				this.m_subsystemParticles.AddParticleSystem(fireParticleSystem);
				this.m_particleSystemsByCell[new Point3(x, y, z)] = fireParticleSystem;
			}
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x0002DDF8 File Offset: 0x0002BFF8
		public void RemoveCampfireParticleSystem(int x, int y, int z)
		{
			Point3 key = new Point3(x, y, z);
			FireParticleSystem fireParticleSystem;
			if (this.m_particleSystemsByCell.TryGetValue(key, out fireParticleSystem))
			{
				fireParticleSystem.IsStopped = true;
				this.m_particleSystemsByCell.Remove(key);
			}
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x0002DE34 File Offset: 0x0002C034
		public bool AddFuel(int x, int y, int z, int value, int count)
		{
			if (Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(x, y, z)) > 0)
			{
				int num = Terrain.ExtractContents(value);
				Block block = BlocksManager.Blocks[num];
				if (base.Project.FindSubsystem<SubsystemExplosions>(true).TryExplodeBlock(x, y, z, value))
				{
					return true;
				}
				if (block is SnowBlock || block is SnowballBlock || block is IceBlock)
				{
					return this.ResizeCampfire(x, y, z, -1, true);
				}
				if (block.FuelHeatLevel > 0f)
				{
					float num2 = (float)count * MathUtils.Min(block.FuelFireDuration, 20f) / 5f;
					int num3 = (int)num2;
					float num4 = num2 - (float)num3;
					if (this.m_random.Float(0f, 1f) < num4)
					{
						num3++;
					}
					return num3 <= 0 || this.ResizeCampfire(x, y, z, num3, true);
				}
			}
			return false;
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x0002DF0C File Offset: 0x0002C10C
		public bool ResizeCampfire(int x, int y, int z, int steps, bool playSound)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractData(cellValue);
			if (num > 0)
			{
				int num2 = MathUtils.Clamp(num + steps, 0, 15);
				if (num2 != num)
				{
					int value = Terrain.ReplaceData(cellValue, num2);
					base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
					if (playSound)
					{
						if (steps >= 0)
						{
							this.m_subsystemAmbientSounds.SubsystemAudio.PlaySound("Audio/BlockPlaced", 1f, 0f, new Vector3((float)x, (float)y, (float)z), 3f, false);
						}
						else
						{
							this.m_subsystemAmbientSounds.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, 0f, new Vector3((float)x, (float)y, (float)z), 3f, true);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x040003F8 RID: 1016
		public SubsystemTime m_subsystemTime;

		// Token: 0x040003F9 RID: 1017
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040003FA RID: 1018
		public SubsystemWeather m_subsystemWeather;

		// Token: 0x040003FB RID: 1019
		public SubsystemAmbientSounds m_subsystemAmbientSounds;

		// Token: 0x040003FC RID: 1020
		public Dictionary<Point3, FireParticleSystem> m_particleSystemsByCell = new Dictionary<Point3, FireParticleSystem>();

		// Token: 0x040003FD RID: 1021
		public float m_fireSoundVolume;

		// Token: 0x040003FE RID: 1022
		public Game.Random m_random = new Game.Random();

		// Token: 0x040003FF RID: 1023
		public int m_updateIndex;

		// Token: 0x04000400 RID: 1024
		public List<Point3> m_toReduce = new List<Point3>();
	}
}
