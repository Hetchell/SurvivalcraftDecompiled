using System;
using System.Collections.Generic;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200022B RID: 555
	public class BlocksTexturesCache
	{
		// Token: 0x06001116 RID: 4374 RVA: 0x00085B98 File Offset: 0x00083D98
		public Texture2D GetTexture(string name)
		{
			Texture2D texture2D;
			if (!this.m_textures.TryGetValue(name, out texture2D))
			{
				texture2D = BlocksTexturesManager.LoadTexture(name);
				this.m_textures.Add(name, texture2D);
			}
			return texture2D;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00085BCC File Offset: 0x00083DCC
		public void Clear()
		{
			foreach (Texture2D texture2D in this.m_textures.Values)
			{
				if (!ContentManager.IsContent(texture2D))
				{
					texture2D.Dispose();
				}
			}
			this.m_textures.Clear();
		}

		// Token: 0x04000B70 RID: 2928
		public Dictionary<string, Texture2D> m_textures = new Dictionary<string, Texture2D>();
	}
}
