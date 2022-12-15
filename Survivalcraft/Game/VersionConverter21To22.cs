using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034C RID: 844
	public class VersionConverter21To22 : VersionConverter
	{
		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060017D0 RID: 6096 RVA: 0x000BC038 File Offset: 0x000BA238
		public override string SourceVersion
		{
			get
			{
				return "2.1";
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060017D1 RID: 6097 RVA: 0x000BC03F File Offset: 0x000BA23F
		public override string TargetVersion
		{
			get
			{
				return "2.2";
			}
		}

		// Token: 0x060017D2 RID: 6098 RVA: 0x000BC048 File Offset: 0x000BA248
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			string empty = string.Empty;
			foreach (XElement xelement in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "GameInfo"
			select e)
			{
				foreach (XElement node in xelement.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "TerrainGenerationMode"))
				{
					if (XmlUtils.GetAttributeValue<string>(node, "Value", "") == "Flat")
					{
						XmlUtils.SetAttributeValue(node, "Value", "FlatContinent");
					}
				}
			}
			foreach (XElement xelement2 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"
			select e)
			{
				foreach (XElement xelement3 in xelement2.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"))
				{
					foreach (XElement xelement4 in xelement3.Elements("Values"))
					{
						foreach (XElement node2 in xelement4.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node2, "Value"));
							XmlUtils.SetAttributeValue(node2, "Value", num);
						}
					}
				}
			}
			foreach (XElement xelement5 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"
			select e)
			{
				foreach (XElement xelement6 in xelement5.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"))
				{
					foreach (XElement xelement7 in xelement6.Elements("Values"))
					{
						foreach (XElement node3 in xelement7.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num2 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node3, "Value"));
							XmlUtils.SetAttributeValue(node3, "Value", num2);
						}
					}
				}
			}
			foreach (XElement xelement8 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlockBehavior"
			select e)
			{
				foreach (XElement xelement9 in xelement8.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlocks"))
				{
					foreach (XElement xelement10 in xelement9.Elements("Values"))
					{
						foreach (XElement node4 in xelement10.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num3 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node4, "Value"));
							XmlUtils.SetAttributeValue(node4, "Value", num3);
						}
					}
				}
			}
			foreach (XElement xelement11 in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement12 in from e in xelement11.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothing"
				select e)
				{
					foreach (XElement xelement13 in xelement12.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothes"))
					{
						foreach (XElement node5 in xelement13.Elements())
						{
							string attributeValue = XmlUtils.GetAttributeValue<string>(node5, "Value");
							int[] array = HumanReadableConverter.ValuesListFromString<int>(';', attributeValue);
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = VersionConverter21To22.ConvertValue(array[i]);
							}
							string value = HumanReadableConverter.ValuesListToString<int>(';', array);
							XmlUtils.SetAttributeValue(node5, "Value", value);
						}
					}
				}
			}
			string[] inventoryNames = new string[]
			{
				"Inventory",
				"CreativeInventory",
				"CraftingTable",
				"Chest",
				"Furnace",
				"Dispenser"
			};
			Func<XElement, bool> predicate0 = null;
			foreach (XElement xelement14 in projectNode.Element("Entities").Elements())
			{
				IEnumerable<XElement> source = xelement14.Elements("Values");
				Func<XElement, bool> predicate;
				if ((predicate = predicate0) == null)
				{
					predicate = (predicate0 = ((XElement e) => inventoryNames.Contains(XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty))));
				}
				foreach (XElement xelement15 in source.Where(predicate))
				{
					foreach (XElement xelement16 in from e in xelement15.Elements("Values")
					where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Slots"
					select e)
					{
						foreach (XElement xelement17 in xelement16.Elements())
						{
							foreach (XElement node6 in xelement17.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Contents"))
							{
								int num4 = VersionConverter21To22.ConvertValue(XmlUtils.GetAttributeValue<int>(node6, "Value"));
								XmlUtils.SetAttributeValue(node6, "Value", num4);
							}
						}
					}
				}
			}
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x000BCB40 File Offset: 0x000BAD40
		public override void ConvertWorld(string directoryName)
		{
			try
			{
				this.ConvertChunks(directoryName);
				this.ConvertProject(directoryName);
				foreach (string text in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".new"
				select f)
				{
					string sourcePath = Storage.CombinePaths(new string[]
					{
						directoryName,
						text
					});
					string destinationPath = Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileNameWithoutExtension(text)
					});
					Storage.MoveFile(sourcePath, destinationPath);
				}
				foreach (string text2 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".old"
				select f)
				{
					Storage.DeleteFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						text2
					}));
				}
			}
			catch (Exception ex)
			{
				foreach (string text3 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".old"
				select f)
				{
					string sourcePath2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						text3
					});
					string destinationPath2 = Storage.CombinePaths(new string[]
					{
						directoryName,
						Storage.GetFileNameWithoutExtension(text3)
					});
					Storage.MoveFile(sourcePath2, destinationPath2);
				}
				foreach (string text4 in from f in Storage.ListFileNames(directoryName)
				where Storage.GetExtension(f) == ".new"
				select f)
				{
					Storage.DeleteFile(Storage.CombinePaths(new string[]
					{
						directoryName,
						text4
					}));
				}
				throw ex;
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x000BCD7C File Offset: 0x000BAF7C
		public void ConvertProject(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			string path2 = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml.new"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path2, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x000BCE1C File Offset: 0x000BB01C
		public void ConvertChunks(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			});
			string path2 = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32h.dat.new"
			});
			long num = 2L * Storage.GetFileSize(path) + 52428800L;
			if (Storage.FreeSpace < num)
			{
				throw new InvalidOperationException(string.Format("Not enough free space to convert world. {0}MB required.", num / 1024L / 1024L));
			}
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				using (Stream stream2 = Storage.OpenFile(path2, OpenFileMode.Create))
				{
					byte[] array = new byte[131072];
					byte[] array2 = new byte[262144];
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer22.WriteTOCEntry(stream2, 0, 0, -1);
					}
					int num2 = 0;
					for (;;)
					{
						stream.Position = (long)(12 * num2);
						int cx;
						int cz;
						int num3;
						TerrainSerializer129.ReadTOCEntry(stream, out cx, out cz, out num3);
						if (num3 < 0)
						{
							break;
						}
						stream2.Position = (long)(12 * num2);
						TerrainSerializer22.WriteTOCEntry(stream2, cx, cz, num2);
						stream.Position = 786444L + 132112L * (long)num3;
						stream2.Position = stream2.Length;
						TerrainSerializer129.ReadChunkHeader(stream);
						TerrainSerializer22.WriteChunkHeader(stream2, cx, cz);
						stream.Read(array, 0, 131072);
						int num4 = 0;
						int num5 = 0;
						for (int j = 0; j < 16; j++)
						{
							for (int k = 0; k < 16; k++)
							{
								for (int l = 0; l < 256; l++)
								{
									int num6;
									if (l <= 127)
									{
										num6 = VersionConverter21To22.ConvertValue((int)array[4 * num4] | (int)array[4 * num4 + 1] << 8 | (int)array[4 * num4 + 2] << 16 | (int)array[4 * num4 + 3] << 24);
										num4++;
									}
									else
									{
										num6 = 0;
									}
									array2[4 * num5] = (byte)num6;
									array2[4 * num5 + 1] = (byte)(num6 >> 8);
									array2[4 * num5 + 2] = (byte)(num6 >> 16);
									array2[4 * num5 + 3] = (byte)(num6 >> 24);
									num5++;
								}
							}
						}
						stream2.Write(array2, 0, 262144);
						stream.Read(array, 0, 1024);
						stream2.Write(array, 0, 1024);
						num2++;
					}
				}
			}
			Storage.MoveFile(Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			}), Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat.old"
			}));
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x000BD0E8 File Offset: 0x000BB2E8
		public static int ConvertValue(int value)
		{
			int num = value & 1023;
			int num2 = value >> 10 & 15;
			int num3 = value >> 14;
			VersionConverter21To22.ConvertContentsLightData(ref num, ref num2, ref num3);
			return num | num2 << 10 | num3 << 14;
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x000BD120 File Offset: 0x000BB320
		public static void ConvertContentsLightData(ref int contents, ref int light, ref int data)
		{
			if (contents == 30)
			{
				contents = 29;
			}
			if (contents == 34)
			{
				contents = 29;
			}
			if (contents == 32)
			{
				contents = 29;
			}
			if (contents == 35)
			{
				contents = 29;
			}
			if (contents == 33)
			{
				contents = 29;
			}
			if (contents == 170)
			{
				contents = 169;
			}
			if (contents == 122)
			{
				contents = 122;
			}
			if (contents == 123)
			{
				contents = 123;
			}
		}
	}
}
