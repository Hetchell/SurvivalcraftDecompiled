using System;
using Engine;

namespace Game
{
	// Token: 0x020001B7 RID: 439
	public class SubsystemWaterBlockBehavior : SubsystemFluidBlockBehavior, IUpdateable
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x000515F9 File Offset: 0x0004F7F9
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					18
				};
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x00051606 File Offset: 0x0004F806
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000AEA RID: 2794 RVA: 0x00051609 File Offset: 0x0004F809
		public SubsystemWaterBlockBehavior() : base(BlocksManager.FluidBlocks[18], true)
		{
		}

		// Token: 0x06000AEB RID: 2795 RVA: 0x00051628 File Offset: 0x0004F828
		public void Update(float dt)
		{
			if (base.SubsystemTime.PeriodicGameTimeEvent(0.25, 0.0))
			{
				base.SpreadFluid();
			}
			if (base.SubsystemTime.PeriodicGameTimeEvent(1.0, 0.25))
			{
				float num = float.MaxValue;
				foreach (Vector3 p in base.SubsystemAudio.ListenerPositions)
				{
					float? num2 = base.CalculateDistanceToFluid(p, 8, true);
					if (num2 != null && num2.Value < num)
					{
						num = num2.Value;
					}
				}
				this.m_soundVolume = 0.5f * base.SubsystemAudio.CalculateVolume(num, 2f, 3.5f);
			}
			base.SubsystemAmbientSounds.WaterSoundVolume = MathUtils.Max(base.SubsystemAmbientSounds.WaterSoundVolume, this.m_soundVolume);
		}

		// Token: 0x06000AEC RID: 2796 RVA: 0x00051734 File Offset: 0x0004F934
		public override bool OnFluidInteract(int interactValue, int x, int y, int z, int fluidValue)
		{
			if (BlocksManager.Blocks[Terrain.ExtractContents(interactValue)] is MagmaBlock)
			{
				base.SubsystemAudio.PlayRandomSound("Audio/Sizzles", 1f, this.m_random.Float(-0.1f, 0.1f), new Vector3((float)x, (float)y, (float)z), 5f, true);
				base.SubsystemTerrain.DestroyCell(0, x, y, z, 0, false, false);
				base.Set(x, y, z, 3);
				return true;
			}
			return base.OnFluidInteract(interactValue, x, y, z, fluidValue);
		}

		// Token: 0x06000AED RID: 2797 RVA: 0x000517BD File Offset: 0x0004F9BD
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			if (y > 80 && SubsystemWeather.IsPlaceFrozen(base.SubsystemTerrain.Terrain.GetSeasonalTemperature(x, z), y))
			{
				dropValue.Value = Terrain.MakeBlockValue(62);
				return;
			}
			base.OnItemHarvested(x, y, z, blockValue, ref dropValue, ref newBlockValue);
		}

		// Token: 0x040005F5 RID: 1525
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005F6 RID: 1526
		public float m_soundVolume;
	}
}
