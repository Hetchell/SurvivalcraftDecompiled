using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Engine;
using Game;
using XmlUtilities;

// Token: 0x02000005 RID: 5
public static class ModsManager
{
	// Token: 0x0600000F RID: 15 RVA: 0x00002B34 File Offset: 0x00000D34
	public static XElement CombineXml(XElement node, IEnumerable<FileEntry> files, string attr1 = null, string attr2 = null, string type = null)
	{
		Func<XElement, IEnumerable<FileEntry>, string, string, string, XElement> combineXml = ModsManager.CombineXml1;
		if (combineXml != null)
		{
			return combineXml(node, files, attr1, attr2, type);
		}
		IEnumerator<FileEntry> enumerator = files.GetEnumerator();
		while (enumerator.MoveNext())
		{
			try
			{
				XElement src = XmlUtils.LoadXmlFromStream(enumerator.Current.Stream, null, true);
				ModsManager.Modify(node, src, attr1, attr2, type);
			}
			catch (Exception arg)
			{
				ModsManager.ErrorHandler(enumerator.Current, arg);
			}
		}
		return node;
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002BB4 File Offset: 0x00000DB4
	public static void SaveSettings()
	{
		XElement xelement = new XElement("Settings");
		XElement xelement2 = XmlUtils.AddElement(xelement, "Set");
		xelement2.SetAttributeValue("Name", "Language");
		xelement2.SetAttributeValue("Value", (int)ModsManager.modSettings.languageType);
		using (Stream stream = Storage.OpenFile("app:/ModSettings.xml", OpenFileMode.Create))
		{
			XmlUtils.SaveXmlToStream(xelement, stream, null, true);
		}
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00002C40 File Offset: 0x00000E40
	public static void GetSetting()
	{
		ModsManager.ModSettings modSettings = new ModsManager.ModSettings();
		if (Storage.FileExists(ModsManager.ModsSetPath))
		{
			using (Stream stream = Storage.OpenFile(ModsManager.ModsSetPath, OpenFileMode.Read))
			{
				try
				{
					foreach (XElement xelement in XmlUtils.LoadXmlFromStream(stream, null, true).Elements())
					{
						if (xelement.Attribute("Name").Value == "Language")
						{
							modSettings.languageType = (LanguageControl.LanguageType)int.Parse(xelement.Attribute("Value").Value);
						}
					}
					goto IL_A8;
				}
				catch
				{
					goto IL_A8;
				}
			}
		}
		modSettings.languageType = LanguageControl.LanguageType.zh_CN;
		IL_A8:
		ModsManager.modSettings = modSettings;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002D24 File Offset: 0x00000F24
	public static void Modify(XElement dst, XElement src, string attr1 = null, string attr2 = null, XName type = null)
	{
		List<XElement> list = new List<XElement>();
		foreach (XElement xelement in src.Elements())
		{
			string localName = xelement.Name.LocalName;
			XAttribute xattribute = xelement.Attribute(attr1);
			string text = (xattribute != null) ? xattribute.Value : null;
			XAttribute xattribute2 = xelement.Attribute(attr2);
			string text2 = (xattribute2 != null) ? xattribute2.Value : null;
			int num = (localName.Length >= 2 && localName[0] == 'r' && localName[1] == '-') ? (xelement.IsEmpty ? 2 : -2) : 0;
			IEnumerator<XElement> enumerator2 = dst.DescendantsAndSelf((localName.Length == 2 && num != 0) ? type : xelement.Name.LocalName.Substring(Math.Abs(num))).GetEnumerator();
			IL_1C8:
			while (enumerator2.MoveNext())
			{
				XElement xelement2 = enumerator2.Current;
				IEnumerator<XAttribute> enumerator3 = xelement2.Attributes().GetEnumerator();
				while (enumerator3.MoveNext())
				{
					localName = enumerator3.Current.Name.LocalName;
					string value = enumerator3.Current.Value;
					XAttribute xattribute3;
					if (text != null && string.Equals(localName, attr1))
					{
						if (!string.Equals(value, text))
						{
							goto IL_1C8;
						}
					}
					else if (text2 != null && string.Equals(localName, attr2))
					{
						if (!string.Equals(value, text2))
						{
							goto IL_1C8;
						}
					}
					else if ((xattribute3 = xelement.Attribute(XName.Get("new-" + localName))) != null)
					{
						xelement2.SetAttributeValue(XName.Get(localName), xattribute3.Value);
					}
				}
				if (num < 0)
				{
					xelement2.RemoveNodes();
					xelement2.Add(xelement.Elements());
				}
				else if (num > 0)
				{
					list.Add(xelement2);
				}
				else if (!xelement.IsEmpty)
				{
					xelement2.Add(xelement.Elements());
				}
			}
		}
		foreach (XElement xelement3 in list)
		{
			xelement3.Remove();
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002F30 File Offset: 0x00001130
	public static void Initialize()
	{
		ModsManager.loadedAssemblies = new List<Assembly>();
		ModsManager.LoadedMods = new List<ModInfo>();
		ModsManager.CachedMods = new HashSet<string>();
		ModsManager.DisabledMods = new HashSet<string>();
		ModsManager.Files = new List<string>();
		ModsManager.Directories = new List<string>();
		ModsManager.customer_Strings = new Dictionary<string, string>();
		ModsManager.ReadZip = true;
		ModsManager.SearchDepth = 3;
		ModsManager.ErrorHandler = new Action<FileEntry, Exception>(ModsManager.LogException);
		ModsManager.zip_filelist = new Dictionary<string, ZipArchive>();
		if (!Storage.DirectoryExists(ModsManager.ModsPath))
		{
			Storage.CreateDirectory(ModsManager.ModsPath);
		}
		ModsManager.GetAllFiles(ModsManager.ModsPath);
		ModsManager.GetSetting();
		LanguageControl.init(ModsManager.modSettings.languageType);
		List<FileEntry> entries = ModsManager.GetEntries(".dll");
		ModsManager.GetEntries(".pdb");
		int num = 0;
		int num2 = 0;
		foreach (FileEntry fileEntry in entries)
		{
			try
			{
				FileEntry file = ModsManager.GetFile(Storage.GetFileNameWithoutExtension(fileEntry.Filename) + ".pdb");
				if (file != null)
				{
					ModsManager.LoadMod(Assembly.Load(ModsManager.StreamToBytes(fileEntry.Stream), ModsManager.StreamToBytes(file.Stream)));
				}
				else
				{
					ModsManager.LoadMod(Assembly.Load(ModsManager.StreamToBytes(fileEntry.Stream)));
				}
				num2++;
			}
			catch (Exception)
			{
				Log.Error("未能成功加载[" + fileEntry.Filename + "]");
				num++;
			}
		}
		Log.Information("Mods manager initialize success");
		Log.Information(string.Format("Loaded {0} dlls;{1} dlls not loaded", num2, num));
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000030E8 File Offset: 0x000012E8
	public static FileEntry GetFile(string name)
	{
		foreach (FileEntry fileEntry in ModsManager.quickAddModsFileList)
		{
			if (fileEntry.Filename == name)
			{
				return fileEntry;
			}
		}
		foreach (ZipArchive zipArchive in ModsManager.zip_filelist.Values)
		{
			foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.ReadCentralDir())
			{
				string filenameInZip = zipArchiveEntry.FilenameInZip;
				if (Storage.GetFileName(filenameInZip) == name)
				{
					MemoryStream memoryStream = new MemoryStream();
					zipArchive.ExtractFile(zipArchiveEntry, memoryStream);
					FileEntry fileEntry2 = new FileEntry();
					fileEntry2.Filename = filenameInZip;
					memoryStream.Position = 0L;
					fileEntry2.Stream = memoryStream;
					return fileEntry2;
				}
			}
		}
		return null;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000321C File Offset: 0x0000141C
	public static void GetAllFiles(string path)
	{
		foreach (string text in Storage.ListFileNames(path))
		{
			string extension = Storage.GetExtension(text);
			string str = Storage.CombinePaths(new string[]
			{
				path,
				text
			});
			Stream stream = Storage.OpenFile(str, OpenFileMode.Read);
			ModsManager.quickAddModsFileList.Add(new FileEntry
			{
				Stream = stream,
				Filename = text
			});
			try
			{
				if (extension == ".zip" || extension == ".scmod")
				{
					ZipArchive value = ZipArchive.Open(stream, true);
					ModsManager.zip_filelist.Add(text, value);
				}
			}
			catch (Exception ex)
			{
				Log.Error("load file [" + str + "] error." + ex.ToString());
			}
		}
		foreach (string text2 in Storage.ListDirectoryNames(path))
		{
			ModsManager.GetAllFiles(Storage.CombinePaths(new string[]
			{
				path,
				text2
			}));
		}
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00003358 File Offset: 0x00001558
	public static List<FileEntry> GetEntries(string ext)
	{
		List<FileEntry> list = new List<FileEntry>();
		foreach (FileEntry fileEntry in ModsManager.quickAddModsFileList)
		{
			if (Storage.GetExtension(fileEntry.Filename) == ext)
			{
				list.Add(fileEntry);
			}
		}
		foreach (ZipArchive zipArchive in ModsManager.zip_filelist.Values)
		{
			foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.ReadCentralDir())
			{
				string filenameInZip = zipArchiveEntry.FilenameInZip;
				if (Storage.GetExtension(filenameInZip) == ext)
				{
					MemoryStream memoryStream = new MemoryStream();
					zipArchive.ExtractFile(zipArchiveEntry, memoryStream);
					FileEntry fileEntry2 = new FileEntry();
					fileEntry2.Filename = filenameInZip;
					memoryStream.Position = 0L;
					fileEntry2.Stream = memoryStream;
					list.Add(fileEntry2);
				}
			}
		}
		return list;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000349C File Offset: 0x0000169C
	public static void LogException(FileEntry file, Exception ex)
	{
		Log.Warning("Loading \"" + file.Filename.Substring(ModsManager.path.Length + 1) + "\" failed: " + ex.ToString());
		file.Stream.Close();
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000034DC File Offset: 0x000016DC
	public static byte[] StreamToBytes(Stream stream)
	{
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, array.Length);
		stream.Seek(0L, SeekOrigin.Begin);
		return array;
	}

	// Token: 0x06000019 RID: 25 RVA: 0x0000350D File Offset: 0x0000170D
	public static Stream BytesToStream(byte[] bytes)
	{
		return new MemoryStream(bytes);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00003518 File Offset: 0x00001718
	public static void StreamToFile(Stream stream, string fileName)
	{
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, array.Length);
		stream.Seek(0L, SeekOrigin.Begin);
		FileStream fileStream = new FileStream(fileName, FileMode.Create);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream);
		binaryWriter.Write(array);
		binaryWriter.Close();
		fileStream.Close();
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00003568 File Offset: 0x00001768
	public static Stream FileToStream(string fileName)
	{
		FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, array.Length);
		fileStream.Close();
		return new MemoryStream(array);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x000035A4 File Offset: 0x000017A4
	public static void LoadMod(Assembly asm)
	{
		if (asm == null)
		{
			return;
		}
		Type typeFromHandle = typeof(PluginLoaderAttribute);
		Type[] types = asm.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			PluginLoaderAttribute pluginLoaderAttribute = (PluginLoaderAttribute)Attribute.GetCustomAttribute(types[i], typeFromHandle);
			if (pluginLoaderAttribute != null)
			{
				MethodInfo method;
				if ((method = types[i].GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) != null)
				{
					method.Invoke(Activator.CreateInstance(types[i]), null);
				}
				ModsManager.LoadedMods.Add(pluginLoaderAttribute.ModInfo);
				Log.Information("loaded mod [" + pluginLoaderAttribute.ModInfo.Name + "]");
			}
		}
		ModsManager.loadedAssemblies.Add(asm);
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003654 File Offset: 0x00001854
	public static string GetMd5(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0400001A RID: 26
	public static List<Assembly> loadedAssemblies;

	// Token: 0x0400001B RID: 27
	public static string Extension;

	// Token: 0x0400001C RID: 28
	public static HashSet<string> DisabledMods;

	// Token: 0x0400001D RID: 29
	public static HashSet<string> CachedMods;

	// Token: 0x0400001E RID: 30
	public static bool ReadZip;

	// Token: 0x0400001F RID: 31
	public static bool AutoCleanCache;

	// Token: 0x04000020 RID: 32
	public static int SearchDepth;

	// Token: 0x04000021 RID: 33
	public static List<string> Files;

	// Token: 0x04000022 RID: 34
	public static List<string> Directories;

	// Token: 0x04000023 RID: 35
	public static Dictionary<string, ZipArchive> Archives;

	// Token: 0x04000024 RID: 36
	public static string CacheDir;

	// Token: 0x04000025 RID: 37
	public static Action<FileEntry, Exception> ErrorHandler;

	// Token: 0x04000026 RID: 38
	public static Action<StreamWriter> ConfigSaved;

	// Token: 0x04000027 RID: 39
	public static Action Initialized;

	// Token: 0x04000028 RID: 40
	public static List<ModInfo> LoadedMods;

	// Token: 0x04000029 RID: 41
	public static Dictionary<string, string> customer_Strings;

	// Token: 0x0400002A RID: 42
	public static Func<XElement, IEnumerable<FileEntry>, string, string, string, XElement> CombineXml1;

	// Token: 0x0400002B RID: 43
	public static string ModsPath = "app:/Mods";

	// Token: 0x0400002C RID: 44
	public static string ModsSetPath = "app:/ModSettings.xml";

	// Token: 0x0400002D RID: 45
	public static string path;

	// Token: 0x0400002E RID: 46
	public static ModsManager.ModSettings modSettings;

	// Token: 0x0400002F RID: 47
	public static Dictionary<string, ZipArchive> zip_filelist;

	// Token: 0x04000030 RID: 48
	public static List<FileEntry> quickAddModsFileList = new List<FileEntry>();

	// Token: 0x020003B1 RID: 945
	public class ModSettings
	{
		// Token: 0x040013D0 RID: 5072
		public LanguageControl.LanguageType languageType;
	}
}
