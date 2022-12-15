using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000320 RID: 800
	public static class TextureAtlasManager
	{
		// Token: 0x06001702 RID: 5890 RVA: 0x000B80EF File Offset: 0x000B62EF
		public static void LoadAtlases()
		{
			TextureAtlasManager.LoadTextureAtlas(ContentManager.Get<Texture2D>("Atlases/AtlasTexture"), ContentManager.Get<string>("Atlases/Atlas"), "Textures/Atlas/");
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x000B8110 File Offset: 0x000B6310
		public static Subtexture GetSubtexture(string name)
		{
			Subtexture subtexture;
			if (!TextureAtlasManager.m_subtextures.TryGetValue(name, out subtexture))
			{
				try
				{
					subtexture = new Subtexture(ContentManager.Get<Texture2D>(name), Vector2.Zero, Vector2.One);
					TextureAtlasManager.m_subtextures.Add(name, subtexture);
					return subtexture;
				}
				catch (Exception innerException)
				{
					throw new InvalidOperationException("Required subtexture " + name + " not found in TextureAtlasManager.", innerException);
				}
			}
			return subtexture;
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x000B8180 File Offset: 0x000B6380
		public static void LoadTextureAtlas(Texture2D texture, string atlasDefinition, string prefix)
		{
			string[] array = atlasDefinition.Split(new char[]
			{
				'\n',
				'\r'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length < 5)
				{
					throw new InvalidOperationException("Invalid texture atlas definition.");
				}
				string key = prefix + array2[0];
				int num = int.Parse(array2[1], CultureInfo.InvariantCulture);
				int num2 = int.Parse(array2[2], CultureInfo.InvariantCulture);
				int num3 = int.Parse(array2[3], CultureInfo.InvariantCulture);
				int num4 = int.Parse(array2[4], CultureInfo.InvariantCulture);
				Vector2 topLeft = new Vector2((float)num / (float)texture.Width, (float)num2 / (float)texture.Height);
				Vector2 bottomRight = new Vector2((float)(num + num3) / (float)texture.Width, (float)(num2 + num4) / (float)texture.Height);
				Subtexture value = new Subtexture(texture, topLeft, bottomRight);
				TextureAtlasManager.m_subtextures.Add(key, value);
			}
		}

		// Token: 0x040010AF RID: 4271
		public static Dictionary<string, Subtexture> m_subtextures = new Dictionary<string, Subtexture>();
	}
}
