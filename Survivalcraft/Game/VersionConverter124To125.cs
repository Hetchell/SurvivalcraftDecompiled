using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033E RID: 830
	public class VersionConverter124To125 : VersionConverter
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001782 RID: 6018 RVA: 0x000BA000 File Offset: 0x000B8200
		public override string SourceVersion
		{
			get
			{
				return "1.24";
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001783 RID: 6019 RVA: 0x000BA007 File Offset: 0x000B8207
		public override string TargetVersion
		{
			get
			{
				return "1.25";
			}
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000BA00E File Offset: 0x000B820E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000BA024 File Offset: 0x000B8224
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
