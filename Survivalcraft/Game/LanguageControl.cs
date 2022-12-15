using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000305 RID: 773
	public static class LanguageControl
	{
		// Token: 0x060015C7 RID: 5575 RVA: 0x000A5CB4 File Offset: 0x000A3EB4
		public static void init(LanguageControl.LanguageType languageType)
		{
			LanguageControl.items = new Dictionary<string, Dictionary<string, string>>();
			LanguageControl.items2 = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
			string path = "app:lang/" + languageType.ToString() + ".json";
			List<FileEntry> entries = ModsManager.GetEntries(".json");
			try
			{
				LanguageControl.loadJson(Storage.OpenFile(path, OpenFileMode.Read));
				foreach (FileEntry fileEntry in entries)
				{
					if (Storage.GetFileName(fileEntry.Filename).StartsWith(languageType.ToString()))
					{
						LanguageControl.loadJson(fileEntry.Stream);
						break;
					}
				}
			}
			catch
			{
				LanguageControl.loadJson(Storage.OpenFile("app:lang/" + LanguageControl.LanguageType.zh_CN.ToString() + ".json", OpenFileMode.Read));
				foreach (FileEntry fileEntry2 in entries)
				{
					if (Storage.GetFileName(fileEntry2.Filename).StartsWith(LanguageControl.LanguageType.zh_CN.ToString()))
					{
						LanguageControl.loadJson(fileEntry2.Stream);
						break;
					}
				}
			}
		}

		// Token: 0x060015C8 RID: 5576 RVA: 0x000A5E18 File Offset: 0x000A4018
		public static void loadJson(Stream stream)
		{
			string text = new StreamReader(stream).ReadToEnd();
			if (text.Length > 0)
			{
				foreach (KeyValuePair<string, object> keyValuePair in ((JsonObject)SimpleJson.SimpleJson.DeserializeObject(text)))
				{
					JsonObject jsonObject = (JsonObject)keyValuePair.Value;
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
					foreach (KeyValuePair<string, object> keyValuePair2 in jsonObject)
					{
						JsonObject jsonObject2 = keyValuePair2.Value as JsonObject;
						if (jsonObject2 != null)
						{
							Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
							foreach (KeyValuePair<string, object> keyValuePair3 in jsonObject2)
							{
								if (dictionary3.ContainsKey(keyValuePair3.Key))
								{
									dictionary3[keyValuePair3.Key] = keyValuePair3.Value.ToString();
								}
								else
								{
									dictionary3.Add(keyValuePair3.Key, keyValuePair3.Value.ToString());
								}
								Dictionary<string, Dictionary<string, string>> dictionary4;
								Dictionary<string, string> dictionary5;
								string text2;
								if (LanguageControl.items2.TryGetValue(keyValuePair.Key, out dictionary4) && dictionary4.TryGetValue(keyValuePair2.Key, out dictionary5) && dictionary5.TryGetValue(keyValuePair3.Key, out text2))
								{
									dictionary5[keyValuePair3.Key] = keyValuePair3.Value.ToString();
								}
							}
							if (!dictionary2.ContainsKey(keyValuePair2.Key))
							{
								dictionary2.Add(keyValuePair2.Key, dictionary3);
							}
						}
						else
						{
							if (!dictionary.ContainsKey(keyValuePair2.Key))
							{
								dictionary.Add(keyValuePair2.Key, keyValuePair2.Value.ToString());
							}
							Dictionary<string, string> dictionary6;
							string text3;
							if (LanguageControl.items.TryGetValue(keyValuePair.Key, out dictionary6) && dictionary6.TryGetValue(keyValuePair2.Key, out text3))
							{
								dictionary6[keyValuePair2.Key] = keyValuePair2.Value.ToString();
							}
						}
					}
					if (!LanguageControl.items.ContainsKey(keyValuePair.Key))
					{
						LanguageControl.items.Add(keyValuePair.Key, dictionary);
					}
					if (dictionary2.Count > 0)
					{
						if (!LanguageControl.items2.ContainsKey(keyValuePair.Key))
						{
							LanguageControl.items2.Add(keyValuePair.Key, dictionary2);
						}
						Dictionary<string, Dictionary<string, string>> dictionary7;
						if (LanguageControl.items2.TryGetValue(keyValuePair.Key, out dictionary7))
						{
							foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair4 in dictionary2)
							{
								Dictionary<string, string> dictionary8;
								if (!dictionary7.TryGetValue(keyValuePair4.Key, out dictionary8))
								{
									dictionary7.Add(keyValuePair4.Key, keyValuePair4.Value);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060015C9 RID: 5577 RVA: 0x000A6150 File Offset: 0x000A4350
		public static string LName()
		{
			return ModsManager.modSettings.languageType.ToString();
		}

		// Token: 0x060015CA RID: 5578 RVA: 0x000A6167 File Offset: 0x000A4367
		public static string Get(string className, int key)
		{
			return LanguageControl.Get(className, key.ToString());
		}

		// Token: 0x060015CB RID: 5579 RVA: 0x000A6178 File Offset: 0x000A4378
		public static string Get(string className, string key)
		{
			Dictionary<string, string> dictionary;
			string text;
			if (!LanguageControl.items.TryGetValue(className, out dictionary) || !dictionary.TryGetValue(key, out text))
			{
				return key;
			}
			if (string.IsNullOrEmpty(text))
			{
				return key;
			}
			return text;
		}

		// Token: 0x060015CC RID: 5580 RVA: 0x000A61AC File Offset: 0x000A43AC
		public static string GetBlock(string name, string prop)
		{
			string[] array = name.Split(new char[]
			{
				':'
			});
			Dictionary<string, Dictionary<string, string>> dictionary;
			if (LanguageControl.items2.TryGetValue("Blocks", out dictionary))
			{
				Dictionary<string, string> dictionary2;
				Dictionary<string, string> dictionary3;
				if (dictionary.TryGetValue(name, out dictionary2))
				{
					string result;
					if (dictionary2.TryGetValue(prop, out result))
					{
						return result;
					}
				}
				else if (dictionary.TryGetValue(array[0] + ":0", out dictionary3))
				{
					if (array[0] == "ClothingBlock")
					{
						return "";
					}
					string result2;
					if (dictionary3.TryGetValue(prop, out result2))
					{
						return result2;
					}
				}
			}
			return "";
		}

		// Token: 0x060015CD RID: 5581 RVA: 0x000A6238 File Offset: 0x000A4438
		public static string GetContentWidgets(string name, string prop)
		{
			Dictionary<string, Dictionary<string, string>> dictionary;
			Dictionary<string, string> dictionary2;
			string result;
			if (LanguageControl.items2.TryGetValue("ContentWidgets", out dictionary) && dictionary.TryGetValue(name, out dictionary2) && dictionary2.TryGetValue(prop, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x060015CE RID: 5582 RVA: 0x000A6275 File Offset: 0x000A4475
		public static string GetContentWidgets(string name, int pos)
		{
			return LanguageControl.GetContentWidgets(name, pos.ToString());
		}

		// Token: 0x060015CF RID: 5583 RVA: 0x000A6284 File Offset: 0x000A4484
		public static string GetDatabase(string name, string prop)
		{
			Dictionary<string, Dictionary<string, string>> dictionary;
			Dictionary<string, string> dictionary2;
			string result;
			if (LanguageControl.items2.TryGetValue("Database", out dictionary) && dictionary.TryGetValue(name, out dictionary2) && dictionary2.TryGetValue(prop, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x060015D0 RID: 5584 RVA: 0x000A62C4 File Offset: 0x000A44C4
		public static string GetFireworks(string name, string prop)
		{
			Dictionary<string, Dictionary<string, string>> dictionary;
			Dictionary<string, string> dictionary2;
			string result;
			if (LanguageControl.items2.TryGetValue("FireworksBlock", out dictionary) && dictionary.TryGetValue(name, out dictionary2) && dictionary2.TryGetValue(prop, out result))
			{
				return result;
			}
			return "";
		}

		// Token: 0x04000F7D RID: 3965
		public static Dictionary<string, Dictionary<string, string>> items;

		// Token: 0x04000F7E RID: 3966
		public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> items2;

		// Token: 0x020004E6 RID: 1254
		public enum LanguageType
		{
			// Token: 0x040017F2 RID: 6130
			zh_CN,
			// Token: 0x040017F3 RID: 6131
			en_US,
			// Token: 0x040017F4 RID: 6132
			ot_OT
		}
	}
}
