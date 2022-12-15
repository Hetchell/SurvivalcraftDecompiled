using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000204 RID: 516
	public class ComponentSimpleModel : ComponentModel
	{
		// Token: 0x06000FB2 RID: 4018 RVA: 0x00078044 File Offset: 0x00076244
		public override void Animate()
		{
			if (this.m_componentSpawn != null)
			{
				base.Opacity = new float?((this.m_componentSpawn.SpawnDuration > 0f) ? ((float)MathUtils.Saturate((this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.SpawnTime) / (double)this.m_componentSpawn.SpawnDuration)) : 1f);
				if (this.m_componentSpawn.DespawnTime != null)
				{
					base.Opacity = new float?(MathUtils.Min(base.Opacity.Value, (float)MathUtils.Saturate(1.0 - (this.m_subsystemGameInfo.TotalElapsedGameTime - this.m_componentSpawn.DespawnTime.Value) / (double)this.m_componentSpawn.DespawnDuration)));
				}
			}
			base.SetBoneTransform(base.Model.RootBone.Index, new Matrix?(this.m_componentFrame.Matrix));
			base.Animate();
		}

        // Token: 0x06000FB3 RID: 4019 RVA: 0x00078145 File Offset: 0x00076345
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.m_componentSpawn = base.Entity.FindComponent<ComponentSpawn>();
			base.Load(valuesDictionary, idToEntityMap);
		}

		// Token: 0x04000A25 RID: 2597
		public SubsystemGameInfo m_subsystemGameInfo;

		// Token: 0x04000A26 RID: 2598
		public ComponentSpawn m_componentSpawn;
	}
}
