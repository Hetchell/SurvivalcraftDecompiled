using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033F RID: 831
	public class VersionConverter125To126 : VersionConverter
	{
		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001787 RID: 6023 RVA: 0x000BA0B0 File Offset: 0x000B82B0
		public override string SourceVersion
		{
			get
			{
				return "1.25";
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x000BA0B7 File Offset: 0x000B82B7
		public override string TargetVersion
		{
			get
			{
				return "1.26";
			}
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x000BA0BE File Offset: 0x000B82BE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x000BA0D4 File Offset: 0x000B82D4
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
