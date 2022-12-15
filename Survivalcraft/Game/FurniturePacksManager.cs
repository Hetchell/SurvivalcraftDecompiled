using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Engine;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000280 RID: 640
	public static class FurniturePacksManager
	{
		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x00093E1C File Offset: 0x0009201C
		public static ReadOnlyList<string> FurniturePackNames
		{
			get
			{
				return new ReadOnlyList<string>(FurniturePacksManager.m_furniturePackNames);
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x00093E28 File Offset: 0x00092028
		public static string FurniturePacksDirectoryName
		{
			get
			{
				return "app:/FurniturePacks";
			}
		}

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x060012FA RID: 4858 RVA: 0x00093E30 File Offset: 0x00092030
		// (remove) Token: 0x060012FB RID: 4859 RVA: 0x00093E64 File Offset: 0x00092064
		public static event Action<string> FurniturePackDeleted;

		// Token: 0x060012FD RID: 4861 RVA: 0x00093EA3 File Offset: 0x000920A3
		public static void Initialize()
		{
			Storage.CreateDirectory(FurniturePacksManager.FurniturePacksDirectoryName);
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x00093EAF File Offset: 0x000920AF
		public static string GetFileName(string name)
		{
			return Storage.CombinePaths(new string[]
			{
				FurniturePacksManager.FurniturePacksDirectoryName,
				name
			});
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x00093EC8 File Offset: 0x000920C8
		public static string GetDisplayName(string name)
		{
			return Storage.GetFileNameWithoutExtension(name);
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x00093ED0 File Offset: 0x000920D0
		public static DateTime GetCreationDate(string name)
		{
			DateTime result;
			try
			{
				result = Storage.GetFileLastWriteTime(FurniturePacksManager.GetFileName(name));
			}
			catch
			{
				result = new DateTime(2000, 1, 1);
			}
			return result;
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x00093F0C File Offset: 0x0009210C
		public static string ImportFurniturePack(string name, Stream stream)
		{
			if (MarketplaceManager.IsTrialMode)
			{
				throw new InvalidOperationException("Cannot import furniture packs in trial mode.");
			}
			FurniturePacksManager.ValidateFurniturePack(stream);
			stream.Position = 0L;
			string fileNameWithoutExtension = Storage.GetFileNameWithoutExtension(name);
			name = fileNameWithoutExtension + ".scfpack";
			string fileName = FurniturePacksManager.GetFileName(name);
			int num = 0;
			while (Storage.FileExists(fileName))
			{
				num++;
				if (num > 9)
				{
					throw new InvalidOperationException("Duplicate name. Delete existing content with conflicting names.");
				}
				name = string.Format("{0} ({1}).scfpack", fileNameWithoutExtension, num);
				fileName = FurniturePacksManager.GetFileName(name);
			}
			string result;
			using (Stream stream2 = Storage.OpenFile(fileName, OpenFileMode.Create))
			{
				stream.CopyTo(stream2);
				result = name;
			}
			return result;
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x00093FC0 File Offset: 0x000921C0
		public static void ExportFurniturePack(string name, Stream stream)
		{
			using (Stream stream2 = Storage.OpenFile(FurniturePacksManager.GetFileName(name), OpenFileMode.Read))
			{
				stream2.CopyTo(stream);
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x00094000 File Offset: 0x00092200
		public static string CreateFurniturePack(string name, ICollection<FurnitureDesign> designs)
		{
			MemoryStream memoryStream = new MemoryStream();
			using (ZipArchive zipArchive = ZipArchive.Create(memoryStream, true))
			{
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				SubsystemFurnitureBlockBehavior.SaveFurnitureDesigns(valuesDictionary, designs);
				XElement xelement = new XElement("FurnitureDesigns");
				valuesDictionary.Save(xelement);
				MemoryStream memoryStream2 = new MemoryStream();
				xelement.Save(memoryStream2);
				memoryStream2.Position = 0L;
				zipArchive.AddStream("FurnitureDesigns.xml", memoryStream2);
			}
			memoryStream.Position = 0L;
			return FurniturePacksManager.ImportFurniturePack(name, memoryStream);
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0009408C File Offset: 0x0009228C
		public static void DeleteFurniturePack(string name)
		{
			try
			{
				Storage.DeleteFile(FurniturePacksManager.GetFileName(name));
				Action<string> furniturePackDeleted = FurniturePacksManager.FurniturePackDeleted;
				if (furniturePackDeleted != null)
				{
					furniturePackDeleted(name);
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Unable to delete furniture pack \"" + name + "\"", e);
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000940E0 File Offset: 0x000922E0
		public static void UpdateFurniturePacksList()
		{
			FurniturePacksManager.m_furniturePackNames.Clear();
			foreach (string text in Storage.ListFileNames(FurniturePacksManager.FurniturePacksDirectoryName))
			{
				if (Storage.GetExtension(text).ToLower() == ".scfpack")
				{
					FurniturePacksManager.m_furniturePackNames.Add(text);
				}
			}
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x00094158 File Offset: 0x00092358
		public static List<FurnitureDesign> LoadFurniturePack(SubsystemTerrain subsystemTerrain, string name)
		{
			List<FurnitureDesign> result;
			using (Stream stream = Storage.OpenFile(FurniturePacksManager.GetFileName(name), OpenFileMode.Read))
			{
				result = FurniturePacksManager.LoadFurniturePack(subsystemTerrain, stream);
			}
			return result;
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x00094198 File Offset: 0x00092398
		public static void ValidateFurniturePack(Stream stream)
		{
			FurniturePacksManager.LoadFurniturePack(null, stream);
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x000941A4 File Offset: 0x000923A4
		public static List<FurnitureDesign> LoadFurniturePack(SubsystemTerrain subsystemTerrain, Stream stream)
		{
			List<FurnitureDesign> result;
			using (ZipArchive zipArchive = ZipArchive.Open(stream, true))
			{
				List<ZipArchiveEntry> list = zipArchive.ReadCentralDir();
				if (list.Count != 1 || list[0].FilenameInZip != "FurnitureDesigns.xml")
				{
					throw new InvalidOperationException("Invalid furniture pack.");
				}
				MemoryStream memoryStream = new MemoryStream();
				zipArchive.ExtractFile(list[0], memoryStream);
				memoryStream.Position = 0L;
				XElement overridesNode = XElement.Load(memoryStream);
				ValuesDictionary valuesDictionary = new ValuesDictionary();
				valuesDictionary.ApplyOverrides(overridesNode);
				result = SubsystemFurnitureBlockBehavior.LoadFurnitureDesigns(subsystemTerrain, valuesDictionary);
			}
			return result;
		}

		// Token: 0x04000D0A RID: 3338
		public static List<string> m_furniturePackNames = new List<string>();
	}
}
