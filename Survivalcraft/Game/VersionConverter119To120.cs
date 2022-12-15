using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000339 RID: 825
	public class VersionConverter119To120 : VersionConverter
	{
		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06001769 RID: 5993 RVA: 0x000B98A8 File Offset: 0x000B7AA8
		public override string SourceVersion
		{
			get
			{
				return "1.19";
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x0600176A RID: 5994 RVA: 0x000B98AF File Offset: 0x000B7AAF
		public override string TargetVersion
		{
			get
			{
				return "1.20";
			}
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x000B98B6 File Offset: 0x000B7AB6
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x000B98CC File Offset: 0x000B7ACC
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
