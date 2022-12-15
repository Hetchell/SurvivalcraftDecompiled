using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033C RID: 828
	public class VersionConverter122To123 : VersionConverter
	{
		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x000B9EA0 File Offset: 0x000B80A0
		public override string SourceVersion
		{
			get
			{
				return "1.22";
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001779 RID: 6009 RVA: 0x000B9EA7 File Offset: 0x000B80A7
		public override string TargetVersion
		{
			get
			{
				return "1.23";
			}
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x000B9EAE File Offset: 0x000B80AE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x000B9EC4 File Offset: 0x000B80C4
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
