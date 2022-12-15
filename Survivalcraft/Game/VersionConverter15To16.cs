using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000346 RID: 838
	public class VersionConverter15To16 : VersionConverter
	{
		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x060017B2 RID: 6066 RVA: 0x000BB640 File Offset: 0x000B9840
		public override string SourceVersion
		{
			get
			{
				return "1.5";
			}
		}

		// Token: 0x170003AA RID: 938
		// (get) Token: 0x060017B3 RID: 6067 RVA: 0x000BB647 File Offset: 0x000B9847
		public override string TargetVersion
		{
			get
			{
				return "1.6";
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x000BB64E File Offset: 0x000B984E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x000BB664 File Offset: 0x000B9864
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
