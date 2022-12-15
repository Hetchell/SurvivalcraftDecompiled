using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000306 RID: 774
	public class Subtexture
	{
		// Token: 0x060015D1 RID: 5585 RVA: 0x000A6301 File Offset: 0x000A4501
		public Subtexture(Texture2D texture, Vector2 topLeft, Vector2 bottomRight)
		{
			this.Texture = texture;
			this.TopLeft = topLeft;
			this.BottomRight = bottomRight;
		}

		// Token: 0x04000F7F RID: 3967
		public readonly Texture2D Texture;

		// Token: 0x04000F80 RID: 3968
		public readonly Vector2 TopLeft;

		// Token: 0x04000F81 RID: 3969
		public readonly Vector2 BottomRight;
	}
}
