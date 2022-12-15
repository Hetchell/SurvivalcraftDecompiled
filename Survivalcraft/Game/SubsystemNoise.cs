using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000195 RID: 405
	public class SubsystemNoise : Subsystem
	{
		// Token: 0x0600096C RID: 2412 RVA: 0x000422BF File Offset: 0x000404BF
		public void MakeNoise(Vector3 position, float loudness, float range)
		{
			this.MakeNoisepublic(null, position, loudness, range);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x000422CB File Offset: 0x000404CB
		public void MakeNoise(ComponentBody sourceBody, float loudness, float range)
		{
			this.MakeNoisepublic(sourceBody, sourceBody.Position, loudness, range);
		}

        // Token: 0x0600096E RID: 2414 RVA: 0x000422DC File Offset: 0x000404DC
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemBodies = base.Project.FindSubsystem<SubsystemBodies>(true);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x000422F0 File Offset: 0x000404F0
		public void MakeNoisepublic(ComponentBody sourceBody, Vector3 position, float loudness, float range)
		{
			float num = range * range;
			this.m_componentBodies.Clear();
			this.m_subsystemBodies.FindBodiesAroundPoint(new Vector2(position.X, position.Z), range, this.m_componentBodies);
			for (int i = 0; i < this.m_componentBodies.Count; i++)
			{
				ComponentBody componentBody = this.m_componentBodies.Array[i];
				if (componentBody != sourceBody && Vector3.DistanceSquared(componentBody.Position, position) < num)
				{
					foreach (INoiseListener noiseListener in componentBody.Entity.FindComponents<INoiseListener>())
					{
						noiseListener.HearNoise(sourceBody, position, loudness);
					}
				}
			}
		}

		// Token: 0x040004F9 RID: 1273
		public SubsystemBodies m_subsystemBodies;

		// Token: 0x040004FA RID: 1274
		public DynamicArray<ComponentBody> m_componentBodies = new DynamicArray<ComponentBody>();
	}
}
