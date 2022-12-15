using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034A RID: 842
	public class VersionConverter19To110 : VersionConverter
	{
		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060017C6 RID: 6086 RVA: 0x000BB900 File Offset: 0x000B9B00
		public override string SourceVersion
		{
			get
			{
				return "1.9";
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x000BB907 File Offset: 0x000B9B07
		public override string TargetVersion
		{
			get
			{
				return "1.10";
			}
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x000BB90E File Offset: 0x000B9B0E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017C9 RID: 6089 RVA: 0x000BB924 File Offset: 0x000B9B24
		public override void ConvertWorld(string directoryName)
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
	}
}
