using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Serialization;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200035C RID: 860
	public class WorldPalette
	{
		// Token: 0x06001822 RID: 6178 RVA: 0x000BEA40 File Offset: 0x000BCC40
		public WorldPalette()
		{
			this.Colors = WorldPalette.DefaultColors.ToArray<Color>();
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in LanguageControl.items[base.GetType().Name].ToArray<KeyValuePair<string, string>>())
			{
				list.Add(keyValuePair.Value);
			}
			this.Names = list.ToArray();
		}

		// Token: 0x06001823 RID: 6179 RVA: 0x000BEAB4 File Offset: 0x000BCCB4
		public WorldPalette(ValuesDictionary valuesDictionary)
		{
			string[] array = valuesDictionary.GetValue<string>("Colors", new string(';', 15)).Split(new char[]
			{
				';'
			});
			if (array.Length != 16)
			{
				throw new InvalidOperationException("Invalid colors.");
			}
			this.Colors = array.Select(delegate(string s, int i)
			{
				if (string.IsNullOrEmpty(s))
				{
					return WorldPalette.DefaultColors[i];
				}
				return HumanReadableConverter.ConvertFromString<Color>(s);
			}).ToArray<Color>();
			string[] array2 = valuesDictionary.GetValue<string>("Names", new string(';', 15)).Split(new char[]
			{
				';'
			});
			if (array2.Length != 16)
			{
				throw new InvalidOperationException("Invalid color names.");
			}
			this.Names = array2.Select(delegate(string s, int i)
			{
				if (string.IsNullOrEmpty(s))
				{
					return LanguageControl.Get(base.GetType().Name, i);
				}
				return s;
			}).ToArray<string>();
			string[] names = this.Names;
			for (int j = 0; j < names.Length; j++)
			{
				if (!WorldPalette.VerifyColorName(names[j]))
				{
					throw new InvalidOperationException("Invalid color name.");
				}
			}
		}

		// Token: 0x06001824 RID: 6180 RVA: 0x000BEBAC File Offset: 0x000BCDAC
		public ValuesDictionary Save()
		{
			ValuesDictionary valuesDictionary = new ValuesDictionary();
			string value = string.Join(";", this.Colors.Select(delegate(Color c, int i)
			{
				if (c == WorldPalette.DefaultColors[i])
				{
					return string.Empty;
				}
				return HumanReadableConverter.ConvertToString(c);
			}));
			string value2 = string.Join(";", this.Names.Select(delegate(string n, int i)
			{
				if (n == LanguageControl.Get(base.GetType().Name, i))
				{
					return string.Empty;
				}
				return n;
			}));
			valuesDictionary.SetValue<string>("Colors", value);
			valuesDictionary.SetValue<string>("Names", value2);
			return valuesDictionary;
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x000BEC2D File Offset: 0x000BCE2D
		public void CopyTo(WorldPalette palette)
		{
			palette.Colors = this.Colors.ToArray<Color>();
			palette.Names = this.Names.ToArray<string>();
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x000BEC54 File Offset: 0x000BCE54
		public static bool VerifyColorName(string name)
		{
			if (name.Length < 1 || name.Length > 16)
			{
				return false;
			}
			foreach (char c in name)
			{
				if (!char.IsLetterOrDigit(c) && c != '-' && c != ' ')
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0400111A RID: 4378
		public const int MaxColors = 16;

		// Token: 0x0400111B RID: 4379
		public const int MaxNameLength = 16;

		// Token: 0x0400111C RID: 4380
		public static readonly Color[] DefaultColors = new Color[]
		{
			new Color(255, 255, 255),
			new Color(181, 255, 255),
			new Color(255, 181, 255),
			new Color(160, 181, 255),
			new Color(255, 240, 160),
			new Color(181, 255, 181),
			new Color(255, 181, 160),
			new Color(181, 181, 181),
			new Color(112, 112, 112),
			new Color(32, 112, 112),
			new Color(112, 32, 112),
			new Color(26, 52, 128),
			new Color(87, 54, 31),
			new Color(24, 116, 24),
			new Color(136, 32, 32),
			new Color(24, 24, 24)
		};

		// Token: 0x0400111D RID: 4381
		public Color[] Colors;

		// Token: 0x0400111E RID: 4382
		public string[] Names;
	}
}
