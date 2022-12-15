using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AC RID: 428
	public class SubsystemSpikesBlockBehavior : SubsystemBlockBehavior, IUpdateable
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000A79 RID: 2681 RVA: 0x0004E068 File Offset: 0x0004C268
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					86
				};
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0004E075 File Offset: 0x0004C275
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000A7B RID: 2683 RVA: 0x0004E078 File Offset: 0x0004C278
		public void Update(float dt)
		{
			if (this.m_closestSoundToPlay != null)
			{
				this.m_subsystemAudio.PlaySound("Audio/Spikes", 0.7f, SubsystemSpikesBlockBehavior.m_random.Float(-0.1f, 0.1f), this.m_closestSoundToPlay.Value, 4f, true);
				this.m_closestSoundToPlay = null;
			}
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x0004E0D8 File Offset: 0x0004C2D8
		public bool RetractExtendSpikes(int x, int y, int z, bool extend)
		{
			int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			int num = Terrain.ExtractContents(cellValue);
			if (BlocksManager.Blocks[num] is SpikedPlankBlock)
			{
				int data = SpikedPlankBlock.SetSpikesState(Terrain.ExtractData(cellValue), extend);
				int value = Terrain.ReplaceData(cellValue, data);
				base.SubsystemTerrain.ChangeCell(x, y, z, value, true);
				Vector3 vector = new Vector3((float)x, (float)y, (float)z);
				float num2 = this.m_subsystemAudio.CalculateListenerDistance(vector);
				if (this.m_closestSoundToPlay == null || num2 < this.m_subsystemAudio.CalculateListenerDistance(this.m_closestSoundToPlay.Value))
				{
					this.m_closestSoundToPlay = new Vector3?(vector);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x0004E188 File Offset: 0x0004C388
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int data = Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			if (!SpikedPlankBlock.GetSpikesState(data))
			{
				return;
			}
			int mountingFace = SpikedPlankBlock.GetMountingFace(data);
			if (cellFace.Face != mountingFace)
			{
				return;
			}
			ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
			if (componentCreature != null)
			{
				double num;
				this.m_lastInjuryTimes.TryGetValue(componentCreature, out num);
				if (this.m_subsystemTime.GameTime - num > 1.0)
				{
					this.m_lastInjuryTimes[componentCreature] = this.m_subsystemTime.GameTime;
					componentCreature.ComponentHealth.Injure(0.1f, null, false, "Spiked by a trap");
				}
			}
		}

        // Token: 0x06000A7E RID: 2686 RVA: 0x0004E23B File Offset: 0x0004C43B
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

        // Token: 0x06000A7F RID: 2687 RVA: 0x0004E268 File Offset: 0x0004C468
        public override void OnEntityRemoved(Entity entity)
		{
			ComponentCreature componentCreature = entity.FindComponent<ComponentCreature>();
			if (componentCreature != null)
			{
				this.m_lastInjuryTimes.Remove(componentCreature);
			}
		}

		// Token: 0x040005BF RID: 1471
		public static Game.Random m_random = new Game.Random();

		// Token: 0x040005C0 RID: 1472
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005C1 RID: 1473
		public SubsystemTime m_subsystemTime;

		// Token: 0x040005C2 RID: 1474
		public Vector3? m_closestSoundToPlay;

		// Token: 0x040005C3 RID: 1475
		public Dictionary<ComponentCreature, double> m_lastInjuryTimes = new Dictionary<ComponentCreature, double>();
	}
}
