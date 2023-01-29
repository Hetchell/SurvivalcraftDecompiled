using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018C RID: 396
	public class SubsystemMagmaBlockBehavior : SubsystemFluidBlockBehavior, IUpdateable
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600090C RID: 2316 RVA: 0x0003E352 File Offset: 0x0003C552
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					92
				};
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x0003E35F File Offset: 0x0003C55F
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0003E362 File Offset: 0x0003C562
		public SubsystemMagmaBlockBehavior() : base(BlocksManager.FluidBlocks[92], false)
		{
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0003E380 File Offset: 0x0003C580
		public void Update(float dt)
		{
			if (base.SubsystemTime.PeriodicGameTimeEvent(2.0, 0.0))
			{
				base.SpreadFluid();
			}
			if (base.SubsystemTime.PeriodicGameTimeEvent(1.0, 0.75))
			{
				float num = float.MaxValue;
				foreach (Vector3 p in base.SubsystemAudio.ListenerPositions)
				{
					float? num2 = base.CalculateDistanceToFluid(p, 8, false);
					if (num2 != null && num2.Value < num)
					{
						num = num2.Value;
					}
				}
				this.m_soundVolume = base.SubsystemAudio.CalculateVolume(num, 2f, 3.5f);
			}
			base.SubsystemAmbientSounds.MagmaSoundVolume = MathUtils.Max(base.SubsystemAmbientSounds.MagmaSoundVolume, this.m_soundVolume);
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x0003E484 File Offset: 0x0003C684
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			base.OnBlockAdded(value, oldValue, x, y, z);
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					for (int k = -1; k <= 1; k++)
					{
						this.ApplyMagmaNeighborhoodEffect(x + i, y + j, z + k);
					}
				}
			}
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x0003E4D3 File Offset: 0x0003C6D3
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
			this.ApplyMagmaNeighborhoodEffect(neighborX, neighborY, neighborZ);
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x0003E4F0 File Offset: 0x0003C6F0
		public override bool OnFluidInteract(int interactValue, int x, int y, int z, int fluidValue)
		{
			if (BlocksManager.Blocks[Terrain.ExtractContents(interactValue)] is WaterBlock)
			{
				base.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 5f, true);
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				base.Set(x, y, z, 67);
				return true;
			}
			return base.OnFluidInteract(interactValue, x, y, z, fluidValue);
		}

        // Token: 0x06000913 RID: 2323 RVA: 0x0003E57A File Offset: 0x0003C77A
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemParticles = base.Project.FindSubsystem<SubsystemParticles>(true);
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0003E5A8 File Offset: 0x0003C7A8
		public void ApplyMagmaNeighborhoodEffect(int x, int y, int z)
		{
			this.m_subsystemFireBlockBehavior.SetCellOnFire(x, y, z, 1f);
			int cellContents = base.SubsystemTerrain.Terrain.GetCellContents(x, y, z);
			//if (cellContents != 8)
			//{
			//	if (cellContents - 61 <= 1)
			//	{
			//		//base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
			//		//this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, new Vector3((float)x + 0.5f, (float)(y + 1), (float)z + 0.5f)));
			//		//return;
			//	}
			//}
			//else
			//{
			//	base.SubsystemTerrain.ChangeCell(x, y, z, 2, true);
			//	this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, new Vector3((float)x + 0.5f, (float)(y + 1), (float)z + 0.5f)));
			//}
			if (cellContents == 8)
			{
				base.SubsystemTerrain.ChangeCell(x, y, z, 2, true);
				this.m_subsystemParticles.AddParticleSystem(new BurntDebrisParticleSystem(base.SubsystemTerrain, new Vector3((float)x + 0.5f, (float)(y + 1), (float)z + 0.5f)));
			}
		}

		// Token: 0x040004BA RID: 1210
		public Game.Random m_random = new Game.Random();

		// Token: 0x040004BB RID: 1211
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x040004BC RID: 1212
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x040004BD RID: 1213
		public float m_soundVolume;
	}
}
