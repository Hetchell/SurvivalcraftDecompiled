using System;
using Engine;

namespace Game
{
	// Token: 0x020002C1 RID: 705
	public class Particle
	{
		// Token: 0x04000DD3 RID: 3539
		public bool IsActive;

		// Token: 0x04000DD4 RID: 3540
		public Vector3 Position;

		// Token: 0x04000DD5 RID: 3541
		public Vector2 Size;

		// Token: 0x04000DD6 RID: 3542
		public float Rotation;

		// Token: 0x04000DD7 RID: 3543
		public Color Color;

		// Token: 0x04000DD8 RID: 3544
		public int TextureSlot;

		// Token: 0x04000DD9 RID: 3545
		public bool UseAdditiveBlending;

		// Token: 0x04000DDA RID: 3546
		public bool FlipX;

		// Token: 0x04000DDB RID: 3547
		public bool FlipY;

		// Token: 0x04000DDC RID: 3548
		public ParticleBillboardingMode BillboardingMode;
	}
}
