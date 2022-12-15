using System;
using Engine;

namespace Game
{
	// Token: 0x020002DA RID: 730
	public class Projectile : WorldItem
	{
		// Token: 0x04000EA1 RID: 3745
		public Vector3 Rotation;

		// Token: 0x04000EA2 RID: 3746
		public Vector3 AngularVelocity;

		// Token: 0x04000EA3 RID: 3747
		public bool IsInWater;

		// Token: 0x04000EA4 RID: 3748
		public double LastNoiseTime;

		// Token: 0x04000EA5 RID: 3749
		public ComponentCreature Owner;

		// Token: 0x04000EA6 RID: 3750
		public ProjectileStoppedAction ProjectileStoppedAction;

		// Token: 0x04000EA7 RID: 3751
		public ITrailParticleSystem TrailParticleSystem;

		// Token: 0x04000EA8 RID: 3752
		public Vector3 TrailOffset;

		// Token: 0x04000EA9 RID: 3753
		public bool NoChunk;

		// Token: 0x04000EAA RID: 3754
		public bool IsIncendiary;
	}
}
