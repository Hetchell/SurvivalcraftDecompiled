using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Engine;
using Engine.Audio;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000178 RID: 376
	public class SubsystemExplosivesBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x00037E6F File Offset: 0x0003606F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600084A RID: 2122 RVA: 0x00037E72 File Offset: 0x00036072
		public override int[] HandledBlocks
		{
			get
			{
				return new int[0];
			}
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00037E7C File Offset: 0x0003607C
		public bool IgniteFuse(int x, int y, int z)
		{
			int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(x, y, z);
			if (BlocksManager.Blocks[cellContents] is GunpowderKegBlock)
			{
				this.AddExplosive(new Point3(x, y, z), this.m_random.Float(4f, 5f));
				return true;
			}
			if (BlocksManager.Blocks[cellContents] is DetonatorBlock)
			{
				this.AddExplosive(new Point3(x, y, z), this.m_random.Float(0.8f, 1.2f));
				return true;
			}
			return false;
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x00037F04 File Offset: 0x00036104
		public void Update(float dt)
		{
			float num = float.MaxValue;
			if (this.m_explosiveDataByPoint.Count > 0)
			{
				foreach (SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData in this.m_explosiveDataByPoint.Values.ToArray<SubsystemExplosivesBlockBehavior.ExplosiveData>())
				{
					Point3 point = explosiveData.Point;
					int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(point.X, point.Y, point.Z);
					int num2 = Terrain.ExtractContents(cellValue);
					Block block = BlocksManager.Blocks[num2];
					if (explosiveData.FuseParticleSystem == null)
					{
						GunpowderKegBlock gunpowderKegBlock = block as GunpowderKegBlock;
						if (gunpowderKegBlock != null)
						{
							explosiveData.FuseParticleSystem = new FuseParticleSystem(new Vector3((float)point.X, (float)point.Y, (float)point.Z) + gunpowderKegBlock.FuseOffset);
							this.m_subsystemParticles.AddParticleSystem(explosiveData.FuseParticleSystem);
						}
					}
					explosiveData.TimeToExplosion -= dt;
					if (explosiveData.TimeToExplosion <= 0f)
					{
						this.m_subsystemExplosions.TryExplodeBlock(explosiveData.Point.X, explosiveData.Point.Y, explosiveData.Point.Z, cellValue);
					}
					float x = this.m_subsystemAudio.CalculateListenerDistance(new Vector3((float)point.X, (float)point.Y, (float)point.Z) + new Vector3(0.5f));
					num = MathUtils.Min(num, x);
				}
			}
			this.m_fuseSound.Volume = SettingsManager.SoundsVolume * this.m_subsystemAudio.CalculateVolume(num, 2f, 2f);
			if (this.m_fuseSound.Volume > AudioManager.MinAudibleVolume)
			{
				this.m_fuseSound.Play();
				return;
			}
			this.m_fuseSound.Pause();
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x000380C9 File Offset: 0x000362C9
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (this.m_subsystemFireBlockBehavior.IsCellOnFire(x, y, z))
			{
				this.IgniteFuse(x, y, z);
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x000380E8 File Offset: 0x000362E8
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			Point3 point = new Point3(x, y, z);
			this.RemoveExplosive(point);
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x00038108 File Offset: 0x00036308
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			List<Point3> list = new List<Point3>();
			foreach (Point3 point in this.m_explosiveDataByPoint.Keys)
			{
				if (point.X >= chunk.Origin.X && point.X < chunk.Origin.X + 16 && point.Z >= chunk.Origin.Y && point.Z < chunk.Origin.Y + 16)
				{
					list.Add(point);
				}
			}
			foreach (Point3 point2 in list)
			{
				this.RemoveExplosive(point2);
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x000381F8 File Offset: 0x000363F8
		public override void OnExplosion(int value, int x, int y, int z, float damage)
		{
			int num = Terrain.ExtractContents(value);
			Block block = BlocksManager.Blocks[num];
			if (block.GetExplosionPressure(value) > 0f && MathUtils.Saturate(damage / block.ExplosionResilience) > 0.01f && this.m_random.Float(0f, 1f) < 0.5f)
			{
				this.IgniteFuse(x, y, z);
			}
		}

        // Token: 0x06000851 RID: 2129 RVA: 0x00038260 File Offset: 0x00036460
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
			this.m_subsystemExplosions = base.Project.FindSubsystem<SubsystemExplosions>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_fuseSound = this.m_subsystemAudio.CreateSound("Audio/Fuse");
			this.m_fuseSound.IsLooped = true;
			foreach (object obj in valuesDictionary.GetValue<ValuesDictionary>("Explosives"))
			{
				ValuesDictionary valuesDictionary2 = (ValuesDictionary)obj;
				Point3 value = valuesDictionary2.GetValue<Point3>("Point");
				float value2 = valuesDictionary2.GetValue<float>("TimeToExplosion");
				this.AddExplosive(value, value2);
			}
		}

        // Token: 0x06000852 RID: 2130 RVA: 0x00038358 File Offset: 0x00036558
        public override void Save(ValuesDictionary valuesDictionary)
		{
			base.Save(valuesDictionary);
			int num = 0;
			ValuesDictionary valuesDictionary2 = new ValuesDictionary();
			valuesDictionary.SetValue<ValuesDictionary>("Explosives", valuesDictionary2);
			foreach (SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData in this.m_explosiveDataByPoint.Values)
			{
				ValuesDictionary valuesDictionary3 = new ValuesDictionary();
				valuesDictionary2.SetValue<ValuesDictionary>(num++.ToString(CultureInfo.InvariantCulture), valuesDictionary3);
				valuesDictionary3.SetValue<Point3>("Point", explosiveData.Point);
				valuesDictionary3.SetValue<float>("TimeToExplosion", explosiveData.TimeToExplosion);
			}
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x0003840C File Offset: 0x0003660C
		public override void Dispose()
		{
			Utilities.Dispose<Sound>(ref this.m_fuseSound);
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x0003841C File Offset: 0x0003661C
		public void AddExplosive(Point3 point, float timeToExplosion)
		{
			if (!this.m_explosiveDataByPoint.ContainsKey(point))
			{
				SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData = new SubsystemExplosivesBlockBehavior.ExplosiveData();
				explosiveData.Point = point;
				explosiveData.TimeToExplosion = timeToExplosion;
				this.m_explosiveDataByPoint.Add(point, explosiveData);
			}
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00038458 File Offset: 0x00036658
		public void RemoveExplosive(Point3 point)
		{
			SubsystemExplosivesBlockBehavior.ExplosiveData explosiveData;
			if (this.m_explosiveDataByPoint.TryGetValue(point, out explosiveData))
			{
				this.m_explosiveDataByPoint.Remove(point);
				if (explosiveData.FuseParticleSystem != null)
				{
					this.m_subsystemParticles.RemoveParticleSystem(explosiveData.FuseParticleSystem);
				}
			}
		}

		// Token: 0x04000460 RID: 1120
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000461 RID: 1121
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000462 RID: 1122
		public SubsystemExplosions m_subsystemExplosions;

		// Token: 0x04000463 RID: 1123
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x04000464 RID: 1124
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x04000465 RID: 1125
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000466 RID: 1126
		public Dictionary<Point3, SubsystemExplosivesBlockBehavior.ExplosiveData> m_explosiveDataByPoint = new Dictionary<Point3, SubsystemExplosivesBlockBehavior.ExplosiveData>();

		// Token: 0x04000467 RID: 1127
		public Sound m_fuseSound;

		// Token: 0x02000421 RID: 1057
		public class ExplosiveData
		{
			// Token: 0x04001579 RID: 5497
			public Point3 Point;

			// Token: 0x0400157A RID: 5498
			public float TimeToExplosion;

			// Token: 0x0400157B RID: 5499
			public FuseParticleSystem FuseParticleSystem;
		}
	}
}
