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
	// Token: 0x02000342 RID: 834
	public class VersionConverter128To129 : VersionConverter
	{
		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600179A RID: 6042 RVA: 0x000BA5F8 File Offset: 0x000B87F8
		public override string SourceVersion
		{
			get
			{
				return "1.28";
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x0600179B RID: 6043 RVA: 0x000BA5FF File Offset: 0x000B87FF
		public override string TargetVersion
		{
			get
			{
				return "1.29";
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x000BA608 File Offset: 0x000B8808
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			foreach (XElement xelement in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"
			select e)
			{
				foreach (XElement xelement2 in xelement.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Pickables"))
				{
					foreach (XElement xelement3 in xelement2.Elements("Values"))
					{
						foreach (XElement node in xelement3.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num = VersionConverter128To129.ConvertValue(XmlUtils.GetAttributeValue<int>(node, "Value"));
							XmlUtils.SetAttributeValue(node, "Value", num);
						}
					}
				}
			}
			foreach (XElement xelement4 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"
			select e)
			{
				foreach (XElement xelement5 in xelement4.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Projectiles"))
				{
					foreach (XElement xelement6 in xelement5.Elements("Values"))
					{
						foreach (XElement node2 in xelement6.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num2 = VersionConverter128To129.ConvertValue(XmlUtils.GetAttributeValue<int>(node2, "Value"));
							XmlUtils.SetAttributeValue(node2, "Value", num2);
						}
					}
				}
			}
			foreach (XElement xelement7 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlockBehavior"
			select e)
			{
				foreach (XElement xelement8 in xelement7.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CollapsingBlocks"))
				{
					foreach (XElement xelement9 in xelement8.Elements("Values"))
					{
						foreach (XElement node3 in xelement9.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Value"))
						{
							int num3 = VersionConverter128To129.ConvertValue(XmlUtils.GetAttributeValue<int>(node3, "Value"));
							XmlUtils.SetAttributeValue(node3, "Value", num3);
						}
					}
				}
			}
			foreach (XElement xelement10 in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement11 in from e in xelement10.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothing"
				select e)
				{
					foreach (XElement xelement12 in xelement11.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Clothes"))
					{
						foreach (XElement node4 in xelement12.Elements())
						{
							string attributeValue = XmlUtils.GetAttributeValue<string>(node4, "Value");
							int[] array = HumanReadableConverter.ValuesListFromString<int>(';', attributeValue);
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = VersionConverter128To129.ConvertValue(array[i]);
							}
							string value = HumanReadableConverter.ValuesListToString<int>(';', array);
							XmlUtils.SetAttributeValue(node4, "Value", value);
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
			foreach (XElement xelement13 in projectNode.Element("Entities").Elements())
			{
				IEnumerable<XElement> source = xelement13.Elements("Values");
				Func<XElement, bool> predicate;
				if ((predicate = predicate0) == null)
				{
					predicate = (predicate0 = ((XElement e) => inventoryNames.Contains(XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty))));
				}
				foreach (XElement xelement14 in source.Where(predicate))
				{
					foreach (XElement xelement15 in from e in xelement14.Elements("Values")
					where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Slots"
					select e)
					{
						foreach (XElement xelement16 in xelement15.Elements())
						{
							foreach (XElement node5 in xelement16.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Contents"))
							{
								int num4 = VersionConverter128To129.ConvertValue(XmlUtils.GetAttributeValue<int>(node5, "Value"));
								XmlUtils.SetAttributeValue(node5, "Value", num4);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000BAFD0 File Offset: 0x000B91D0
		public override void ConvertWorld(string directoryName)
		{
			this.ConvertProject(directoryName);
			this.ConvertChunks(directoryName);
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x000BAFE0 File Offset: 0x000B91E0
		public void ConvertProject(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream2 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream2, null, true);
			}
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x000BB064 File Offset: 0x000B9264
		public void ConvertChunks(string directoryName)
		{
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks.dat"
			});
			string path2 = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks32.dat"
			});
			using (Stream stream = Storage.OpenFile(path, OpenFileMode.Read))
			{
				using (Stream stream2 = Storage.OpenFile(path2, OpenFileMode.Create))
				{
					byte[] array = new byte[65536];
					byte[] array2 = new byte[131072];
					for (int i = 0; i < 65537; i++)
					{
						TerrainSerializer129.WriteTOCEntry(stream2, 0, 0, -1);
					}
					int num = 0;
					for (;;)
					{
						stream.Position = (long)(12 * num);
						int cx;
						int cz;
						int num2;
						TerrainSerializer14.ReadTOCEntry(stream, out cx, out cz, out num2);
						if (num2 == 0)
						{
							break;
						}
						stream2.Position = (long)(12 * num);
						TerrainSerializer129.WriteTOCEntry(stream2, cx, cz, num);
						stream.Position = (long)num2;
						stream2.Position = stream2.Length;
						TerrainSerializer14.ReadChunkHeader(stream);
						TerrainSerializer129.WriteChunkHeader(stream2, cx, cz);
						stream.Read(array, 0, 65536);
						int num3 = 0;
						for (int j = 0; j < 16; j++)
						{
							for (int k = 0; k < 16; k++)
							{
								for (int l = 0; l < 128; l++)
								{
									int num4 = (int)array[2 * num3] | (int)array[2 * num3 + 1] << 8;
									int num5 = VersionConverter128To129.ConvertValue(num4);
									if (l < 127)
									{
										int num6 = num4 & 255;
										if (num6 == 18 || num6 == 92)
										{
											int num7 = ((int)array[2 * num3 + 2] | (int)array[2 * num3 + 3] << 8) & 255;
											if (num6 != num7)
											{
												num5 |= 262144;
											}
										}
									}
									array2[4 * num3] = (byte)num5;
									array2[4 * num3 + 1] = (byte)(num5 >> 8);
									array2[4 * num3 + 2] = (byte)(num5 >> 16);
									array2[4 * num3 + 3] = (byte)(num5 >> 24);
									num3++;
								}
							}
						}
						stream2.Write(array2, 0, 131072);
						stream.Read(array, 0, 1024);
						stream2.Write(array, 0, 1024);
						num++;
					}
				}
			}
			Storage.DeleteFile(path);
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x000BB2C8 File Offset: 0x000B94C8
		public static int ConvertValue(int value)
		{
			int num = value & 255;
			int num2 = value >> 8 & 15;
			int num3 = value >> 12;
			VersionConverter128To129.ConvertContentsLightData(ref num, ref num2, ref num3);
			return num | num2 << 10 | num3 << 14;
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x000BB300 File Offset: 0x000B9500
		public static void ConvertContentsLightData(ref int contents, ref int light, ref int data)
		{
			if (contents >= 133 && contents <= 136)
			{
				data |= contents - 133 << 4;
				contents = 133;
				return;
			}
			if (contents == 152)
			{
				bool flag = (data & 1) != 0;
				int num = data >> 1 & 7;
				int num2 = (!flag) ? 2 : 5;
				data = (num | num2 << 3);
				return;
			}
			if (contents == 182)
			{
				bool flag2 = (data & 1) != 0;
				int num3 = data >> 1 & 7;
				int num4 = (!flag2) ? 3 : 0;
				data = (num3 | num4 << 3);
				return;
			}
			if (contents == 185)
			{
				int num5 = data & 3;
				int num6 = data >> 2 & 3;
				int num7 = 0;
				if (num5 == 0)
				{
					num7 = 2;
				}
				if (num5 == 1)
				{
					num7 = 5;
				}
				if (num5 == 2)
				{
					num7 = 3;
				}
				data = (num6 | num7 << 3);
				return;
			}
			if (contents == 139)
			{
				int num8 = data & 1;
				int num9 = data >> 1 & 7;
				int num10 = num8 * 15;
				data = (num9 | num10 << 3);
				return;
			}
			if (contents == 128)
			{
				contents = 21;
				data = (data << 1 | 1);
			}
			if (contents == 163)
			{
				contents = 3;
				data = (data << 1 | 1);
			}
			if (contents == 164)
			{
				contents = 73;
				data = (data << 1 | 1);
			}
			if (contents == 165)
			{
				contents = 67;
				data = (data << 1 | 1);
			}
		}
	}
}
