using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Graphics;
using Engine.Media;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200023A RID: 570
	public static class CharacterSkinsManager
	{
		// Token: 0x17000289 RID: 649
		// (get) Token: 0x0600118D RID: 4493 RVA: 0x00087C4F File Offset: 0x00085E4F
		public static ReadOnlyList<string> CharacterSkinsNames
		{
			get
			{
				return new ReadOnlyList<string>(CharacterSkinsManager.m_characterSkinNames);
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x0600118E RID: 4494 RVA: 0x00087C5B File Offset: 0x00085E5B
		public static string CharacterSkinsDirectoryName
		{
			get
			{
				return "app:/CharacterSkins";
			}
		}

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x0600118F RID: 4495 RVA: 0x00087C64 File Offset: 0x00085E64
		// (remove) Token: 0x06001190 RID: 4496 RVA: 0x00087C98 File Offset: 0x00085E98
		public static event Action<string> CharacterSkinDeleted;

		// Token: 0x06001191 RID: 4497 RVA: 0x00087CCB File Offset: 0x00085ECB
		public static void Initialize()
		{
			Storage.CreateDirectory(CharacterSkinsManager.CharacterSkinsDirectoryName);
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00087CD7 File Offset: 0x00085ED7
		public static bool IsBuiltIn(string name)
		{
			return name.StartsWith("$");
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00087CE4 File Offset: 0x00085EE4
		public static PlayerClass? GetPlayerClass(string name)
		{
			name = name.ToLower();
			if (name.Contains("female") || name.Contains("girl") || name.Contains("woman"))
			{
				return new PlayerClass?(PlayerClass.Female);
			}
			if (name.Contains("male") || name.Contains("boy") || name.Contains("man"))
			{
				return new PlayerClass?(PlayerClass.Male);
			}
			return null;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00087D5E File Offset: 0x00085F5E
		public static string GetFileName(string name)
		{
			if (CharacterSkinsManager.IsBuiltIn(name))
			{
				return null;
			}
			return Storage.CombinePaths(new string[]
			{
				CharacterSkinsManager.CharacterSkinsDirectoryName,
				name
			});
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00087D84 File Offset: 0x00085F84
		public static string GetDisplayName(string name)
		{
			if (!CharacterSkinsManager.IsBuiltIn(name))
			{
				return Storage.GetFileNameWithoutExtension(name);
			}
			if (name.Contains("Female"))
			{
				if (name.Contains("1"))
				{
					return "Doris";
				}
				if (name.Contains("2"))
				{
					return "Mabel";
				}
				if (name.Contains("3"))
				{
					return "Ada";
				}
				return "Shirley";
			}
			else
			{
				if (name.Contains("1"))
				{
					return "Walter";
				}
				if (name.Contains("2"))
				{
					return "Basil";
				}
				if (name.Contains("3"))
				{
					return "Geoffrey";
				}
				return "Zachary";
			}
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00087E30 File Offset: 0x00086030
		public static DateTime GetCreationDate(string name)
		{
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					return Storage.GetFileLastWriteTime(fileName);
				}
			}
			catch
			{
			}
			return new DateTime(2000, 1, 1);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x00087E78 File Offset: 0x00086078
		public static Texture2D LoadTexture(string name)
		{
			Texture2D texture2D = null;
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					using (Stream stream = Storage.OpenFile(fileName, OpenFileMode.Read))
					{
						CharacterSkinsManager.ValidateCharacterSkin(stream);
						stream.Position = 0L;
						texture2D = Texture2D.Load(stream, false, 1);
						goto IL_62;
					}
				}
				texture2D = ContentManager.Get<Texture2D>("Textures/Creatures/Human" + name.Substring(1).Replace(" ", ""));
				IL_62:;
			}
			catch (Exception ex)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Could not load character skin \"",
					name,
					"\". Reason: ",
					ex.Message,
					"."
				}));
			}
			if (texture2D == null)
			{
				texture2D = ContentManager.Get<Texture2D>("Textures/Creatures/HumanMale1");
			}
			return texture2D;
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x00087F4C File Offset: 0x0008614C
		public static string ImportCharacterSkin(string name, Stream stream)
		{
			Exception ex = ExternalContentManager.VerifyExternalContentName(name);
			if (ex != null)
			{
				throw ex;
			}
			if (Storage.GetExtension(name) != ".scskin")
			{
				name += ".scskin";
			}
			CharacterSkinsManager.ValidateCharacterSkin(stream);
			stream.Position = 0L;
			string result;
			using (Stream stream2 = Storage.OpenFile(CharacterSkinsManager.GetFileName(name), OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00087FC8 File Offset: 0x000861C8
		public static void DeleteCharacterSkin(string name)
		{
			try
			{
				string fileName = CharacterSkinsManager.GetFileName(name);
				if (!string.IsNullOrEmpty(fileName))
				{
					Storage.DeleteFile(fileName);
					Action<string> characterSkinDeleted = CharacterSkinsManager.CharacterSkinDeleted;
					if (characterSkinDeleted != null)
					{
						characterSkinDeleted(name);
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete character skin \"" + name + "\"", e);
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00088028 File Offset: 0x00086228
		public static void UpdateCharacterSkinsList()
		{
			CharacterSkinsManager.m_characterSkinNames.Clear();
			CharacterSkinsManager.m_characterSkinNames.Add("$Male1");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male2");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male3");
			CharacterSkinsManager.m_characterSkinNames.Add("$Male4");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female1");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female2");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female3");
			CharacterSkinsManager.m_characterSkinNames.Add("$Female4");
			foreach (string text in Storage.ListFileNames(CharacterSkinsManager.CharacterSkinsDirectoryName))
			{
				if (Storage.GetExtension(text).ToLower() == ".scskin")
				{
					CharacterSkinsManager.m_characterSkinNames.Add(text);
				}
			}
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00088118 File Offset: 0x00086318
		public static Model GetPlayerModel(PlayerClass playerClass)
		{
			Model model;
			if (!CharacterSkinsManager.m_playerModels.TryGetValue(playerClass, out model))
			{
				ValuesDictionary valuesDictionary;
				if (playerClass != PlayerClass.Male)
				{
					if (playerClass != PlayerClass.Female)
					{
						throw new InvalidOperationException("Unknown player class.");
					}
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("FemalePlayer", true);
				}
				else
				{
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("MalePlayer", true);
				}
				model = ContentManager.Get<Model>(valuesDictionary.GetValue<ValuesDictionary>("HumanModel").GetValue<string>("ModelName"));
				CharacterSkinsManager.m_playerModels.Add(playerClass, model);
			}
			return model;
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0008818C File Offset: 0x0008638C
		public static Model GetOuterClothingModel(PlayerClass playerClass)
		{
			Model model;
			if (!CharacterSkinsManager.m_outerClothingModels.TryGetValue(playerClass, out model))
			{
				ValuesDictionary valuesDictionary;
				if (playerClass != PlayerClass.Male)
				{
					if (playerClass != PlayerClass.Female)
					{
						throw new InvalidOperationException("Unknown player class.");
					}
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("FemalePlayer", true);
				}
				else
				{
					valuesDictionary = DatabaseManager.FindEntityValuesDictionary("MalePlayer", true);
				}
				model = ContentManager.Get<Model>(valuesDictionary.GetValue<ValuesDictionary>("OuterClothingModel").GetValue<string>("ModelName"));
				CharacterSkinsManager.m_outerClothingModels.Add(playerClass, model);
			}
			return model;
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00088200 File Offset: 0x00086400
		public static void ValidateCharacterSkin(Stream stream)
		{
			Image image = Image.Load(stream);
			if (image.Width > 256 || image.Height > 256)
			{
				throw new InvalidOperationException(string.Format("Character skin is larger than 256x256 pixels (size={0}x{1})", image.Width, image.Height));
			}
			if (!MathUtils.IsPowerOf2((long)image.Width) || !MathUtils.IsPowerOf2((long)image.Height))
			{
				throw new InvalidOperationException(string.Format("Character skin does not have power-of-two size (size={0}x{1})", image.Width, image.Height));
			}
		}

		// Token: 0x04000BAE RID: 2990
		public static List<string> m_characterSkinNames = new List<string>();

		// Token: 0x04000BAF RID: 2991
		public static Dictionary<PlayerClass, Model> m_playerModels = new Dictionary<PlayerClass, Model>();

		// Token: 0x04000BB0 RID: 2992
		public static Dictionary<PlayerClass, Model> m_outerClothingModels = new Dictionary<PlayerClass, Model>();
	}
}
