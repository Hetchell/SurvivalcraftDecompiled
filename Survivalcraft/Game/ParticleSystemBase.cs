using System;

namespace Game
{
	// Token: 0x020002C4 RID: 708
	public abstract class ParticleSystemBase
	{
		// Token: 0x06001411 RID: 5137
		public abstract void Draw(Camera camera);

		// Token: 0x06001412 RID: 5138
		public abstract bool Simulate(float dt);

		// Token: 0x06001413 RID: 5139 RVA: 0x0009B909 File Offset: 0x00099B09
		public virtual void OnAdded()
		{
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x0009B90B File Offset: 0x00099B0B
		public virtual void OnRemoved()
		{
		}

		// Token: 0x04000DE9 RID: 3561
		public SubsystemParticles SubsystemParticles;
	}
}
