using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200032F RID: 815
	public class VersionConverter10To14 : VersionConverter
	{
		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x000B8DB0 File Offset: 0x000B6FB0
		public override string SourceVersion
		{
			get
			{
				return "1.0";
			}
		}

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x06001734 RID: 5940 RVA: 0x000B8DB7 File Offset: 0x000B6FB7
		public override string TargetVersion
		{
			get
			{
				return "1.4";
			}
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x000B8DBE File Offset: 0x000B6FBE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x000B8DD4 File Offset: 0x000B6FD4
		public override void ConvertWorld(string directoryName)
		{
			string[] array = Storage.ListFileNames(Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks"
			})).ToArray<string>();
			using (Stream stream = Storage.OpenFile(Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks.dat"
			}), OpenFileMode.Create))
			{
				for (int i = 0; i < 65537; i++)
				{
					TerrainSerializer14.WriteTOCEntry(stream, 0, 0, 0);
				}
				int num = 0;
				foreach (string text in array)
				{
					try
					{
						if (num >= 65536)
						{
							throw new InvalidOperationException("Too many chunks.");
						}
						string[] array3 = Storage.GetFileNameWithoutExtension(text).Split(new char[]
						{
							'_'
						});
						int cx = int.Parse(array3[1], CultureInfo.InvariantCulture);
						int cz = int.Parse(array3[2], CultureInfo.InvariantCulture);
						using (Stream stream2 = Storage.OpenFile(Storage.CombinePaths(new string[]
						{
							directoryName,
							Storage.CombinePaths(new string[]
							{
								"Chunks",
								text
							})
						}), OpenFileMode.Read))
						{
							byte[] array4 = new byte[stream2.Length];
							stream2.Read(array4, 0, array4.Length);
							int num2 = (int)stream.Length;
							stream.Position = (long)num2;
							TerrainSerializer14.WriteChunkHeader(stream, cx, cz);
							stream.Write(array4, 0, array4.Length);
							stream.Position = (long)(num * 4 * 3);
							TerrainSerializer14.WriteTOCEntry(stream, cx, cz, num2);
							num++;
						}
					}
					catch (Exception ex)
					{
						Log.Error("Error converting chunk file \"" + text + "\". Skipping chunk. Reason: " + ex.Message);
					}
				}
				stream.Flush();
				Log.Information(string.Format("Converted {0} chunk(s).", num));
			}
			string path = Storage.CombinePaths(new string[]
			{
				directoryName,
				"Project.xml"
			});
			XElement xelement;
			using (Stream stream3 = Storage.OpenFile(path, OpenFileMode.Read))
			{
				xelement = XmlUtils.LoadXmlFromStream(stream3, null, true);
			}
			this.ConvertProjectXml(xelement);
			using (Stream stream4 = Storage.OpenFile(path, OpenFileMode.Create))
			{
				XmlUtils.SaveXmlToStream(xelement, stream4, null, true);
			}
			foreach (string text2 in array)
			{
				Storage.DeleteFile(Storage.CombinePaths(new string[]
				{
					directoryName,
					Storage.CombinePaths(new string[]
					{
						"Chunks",
						text2
					})
				}));
			}
			Storage.DeleteDirectory(Storage.CombinePaths(new string[]
			{
				directoryName,
				"Chunks"
			}));
		}
	}
}
