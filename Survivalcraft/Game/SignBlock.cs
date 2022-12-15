using System;
using Engine;

namespace Game
{
	// Token: 0x020000F0 RID: 240
	public abstract class SignBlock : Block
	{
		// Token: 0x06000490 RID: 1168
		public abstract BlockMesh GetSignSurfaceBlockMesh(int data);

		// Token: 0x06000491 RID: 1169
		public abstract Vector3 GetSignSurfaceNormal(int data);
	}
}
