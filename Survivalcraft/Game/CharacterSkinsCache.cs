using System;
using System.Collections.Generic;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000239 RID: 569
	public class CharacterSkinsCache
	{
		// Token: 0x06001189 RID: 4489 RVA: 0x00087B8D File Offset: 0x00085D8D
		public bool ContainsTexture(Texture2D texture)
		{
			return this.m_textures.ContainsValue(texture);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00087B9C File Offset: 0x00085D9C
		public Texture2D GetTexture(string name)
		{
			Texture2D texture2D;
			if (!this.m_textures.TryGetValue(name, out texture2D))
			{
				texture2D = CharacterSkinsManager.LoadTexture(name);
				this.m_textures.Add(name, texture2D);
			}
			return texture2D;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00087BD0 File Offset: 0x00085DD0
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

		// Token: 0x04000BAD RID: 2989
		public Dictionary<string, Texture2D> m_textures = new Dictionary<string, Texture2D>();
	}
}
