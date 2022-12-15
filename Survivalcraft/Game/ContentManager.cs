using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Engine;
using Engine.Content;
using OpenTK;

namespace Game
{
	// Token: 0x02000363 RID: 867
	public static class ContentManager
	{
		// Token: 0x06001874 RID: 6260 RVA: 0x000C2958 File Offset: 0x000C0B58
		public static void Initialize()
		{
			ModsManager.Initialize();
			ContentCache.AddPackage("app:/Content.pak", Encoding.UTF8.GetBytes(ContentManager.Pad()), new byte[]
			{
				63
			});
			using (List<FileEntry>.Enumerator enumerator = ModsManager.GetEntries(".pak").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FileEntry fileEntry = enumerator.Current;
					ContentCache.AddPackage(() => fileEntry.Stream, Encoding.UTF8.GetBytes(ContentManager.Pad()), new byte[]
					{
						63
					});
				}
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x000C2A08 File Offset: 0x000C0C08
		public static object Get(string name)
		{
			return ContentCache.Get(name, true);
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x000C2A14 File Offset: 0x000C0C14
		public static object Get(Type type, string name)
		{
			if (type == typeof(Subtexture))
			{
				return TextureAtlasManager.GetSubtexture(name);
			}
			if (type == typeof(string) && name.StartsWith("Strings/"))
			{
				return StringsManager.GetString(name.Substring(8));
			}
			object obj = ContentManager.Get(name);
			if (!type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
			{
				throw new InvalidOperationException(string.Format(LanguageControl.Get("ContentManager", "1"), name, obj.GetType().FullName, type.FullName));
			}
			return obj;
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x000C2AB2 File Offset: 0x000C0CB2
		public static T Get<T>(string name)
		{
			return (T)((object)ContentManager.Get(typeof(T), name));
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x000C2AC9 File Offset: 0x000C0CC9
		public static void Dispose(string name)
		{
			ContentCache.Dispose(name);
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000C2AD1 File Offset: 0x000C0CD1
		public static bool IsContent(object content)
		{
			return ContentCache.IsContent(content);
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000C2AD9 File Offset: 0x000C0CD9
		public static ReadOnlyList<ContentInfo> List()
		{
			return ContentCache.List("");
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x000C2AE5 File Offset: 0x000C0CE5
		public static ReadOnlyList<ContentInfo> List(string directory)
		{
			return ContentCache.List(directory);
		}

		// Token: 0x0600187C RID: 6268 RVA: 0x000C2AF0 File Offset: 0x000C0CF0
		public static string Pad()
		{
			string text = string.Empty;
			string text2 = "0123456789abdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
			Game.Random random = new Game.Random(171);
			int p;
			for (int i = 0; i < 229; i++)
			{
				p = random.Int(text2.Length);
				p = MathUtils.Abs(p);
				text += text2[p].ToString();
			}
			return text;
		}
	}
}
