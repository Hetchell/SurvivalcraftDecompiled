using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000331 RID: 817
	public class VersionConverter111To112 : VersionConverter
	{
		// Token: 0x1700037F RID: 895
		// (get) Token: 0x0600173D RID: 5949 RVA: 0x000B919C File Offset: 0x000B739C
		public override string SourceVersion
		{
			get
			{
				return "1.11";
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x0600173E RID: 5950 RVA: 0x000B91A3 File Offset: 0x000B73A3
		public override string TargetVersion
		{
			get
			{
				return "1.12";
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x000B91AA File Offset: 0x000B73AA
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x000B91C0 File Offset: 0x000B73C0
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
