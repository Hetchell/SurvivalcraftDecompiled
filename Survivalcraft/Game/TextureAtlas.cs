using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200031F RID: 799
	public class TextureAtlas
	{
		// Token: 0x1700036F RID: 879
		// (get) Token: 0x060016FE RID: 5886 RVA: 0x000B7F48 File Offset: 0x000B6148
		public Texture2D Texture
		{
			get
			{
				return this.m_texture;
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x000B7F50 File Offset: 0x000B6150
		public TextureAtlas(Texture2D texture, string atlasDefinition, string prefix)
		{
			this.m_texture = texture;
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
				Rectangle value = new Rectangle
				{
					Left = int.Parse(array2[1], CultureInfo.InvariantCulture),
					Top = int.Parse(array2[2], CultureInfo.InvariantCulture),
					Width = int.Parse(array2[3], CultureInfo.InvariantCulture),
					Height = int.Parse(array2[4], CultureInfo.InvariantCulture)
				};
				this.m_rectangles.Add(key, value);
			}
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x000B803B File Offset: 0x000B623B
		public bool ContainsTexture(string textureName)
		{
			return this.m_rectangles.ContainsKey(textureName);
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000B804C File Offset: 0x000B624C
		public Vector4? GetTextureCoordinates(string textureName)
		{
			Rectangle rectangle;
			if (this.m_rectangles.TryGetValue(textureName, out rectangle))
			{
				return new Vector4?(new Vector4
				{
					X = (float)rectangle.Left / (float)this.m_texture.Width,
					Y = (float)rectangle.Top / (float)this.m_texture.Height,
					Z = (float)rectangle.Right / (float)this.m_texture.Width,
					W = (float)rectangle.Bottom / (float)this.m_texture.Height
				});
			}
			return null;
		}

		// Token: 0x040010AD RID: 4269
		public Texture2D m_texture;

		// Token: 0x040010AE RID: 4270
		public Dictionary<string, Rectangle> m_rectangles = new Dictionary<string, Rectangle>();
	}
}
