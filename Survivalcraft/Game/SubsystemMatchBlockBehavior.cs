using System;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200018E RID: 398
	public class SubsystemMatchBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600091E RID: 2334 RVA: 0x0003E892 File Offset: 0x0003CA92
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					108
				};
			}
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0003E8A0 File Offset: 0x0003CAA0
		public override bool OnUse(Ray3 ray, ComponentMiner componentMiner)
		{
			object obj = componentMiner.Raycast(ray, RaycastMode.Digging, true, true, true);
			if (obj is TerrainRaycastResult)
			{
				CellFace cellFace = ((TerrainRaycastResult)obj).CellFace;
				if (this.m_subsystemExplosivesBlockBehavior.IgniteFuse(cellFace.X, cellFace.Y, cellFace.Z))
				{
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
				if (this.m_subsystemFireBlockBehavior.SetCellOnFire(cellFace.X, cellFace.Y, cellFace.Z, 1f))
				{
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
			}
			else if (obj is BodyRaycastResult)
			{
				ComponentOnFire componentOnFire = ((BodyRaycastResult)obj).ComponentBody.Entity.FindComponent<ComponentOnFire>();
				if (componentOnFire != null)
				{
					if (this.m_subsystemGameInfo.WorldSettings.GameMode < GameMode.Challenging || this.m_random.Float(0f, 1f) < 0.33f)
					{
						componentOnFire.SetOnFire(componentMiner.ComponentCreature, this.m_random.Float(6f, 8f));
					}
					this.m_subsystemAudio.PlaySound("Audio/Match", 1f, this.m_random.Float(-0.1f, 0.1f), ray.Position, 1f, true);
					componentMiner.RemoveActiveTool(1);
					return true;
				}
			}
			return false;
		}

        // Token: 0x06000920 RID: 2336 RVA: 0x0003EA4C File Offset: 0x0003CC4C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_subsystemFireBlockBehavior = base.Project.FindSubsystem<SubsystemFireBlockBehavior>(true);
			this.m_subsystemExplosivesBlockBehavior = base.Project.FindSubsystem<SubsystemExplosivesBlockBehavior>(true);
		}

		// Token: 0x040004C1 RID: 1217
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040004C2 RID: 1218
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x040004C3 RID: 1219
		public SubsystemFireBlockBehavior m_subsystemFireBlockBehavior;

		// Token: 0x040004C4 RID: 1220
		public SubsystemExplosivesBlockBehavior m_subsystemExplosivesBlockBehavior;

		// Token: 0x040004C5 RID: 1221
		public Game.Random m_random = new Game.Random();
	}
}
