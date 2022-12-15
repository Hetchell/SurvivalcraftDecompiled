using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000334 RID: 820
	public class VersionConverter114To115 : VersionConverter
	{
		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x000B953A File Offset: 0x000B773A
		public override string SourceVersion
		{
			get
			{
				return "1.14";
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06001751 RID: 5969 RVA: 0x000B9541 File Offset: 0x000B7741
		public override string TargetVersion
		{
			get
			{
				return "1.15";
			}
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x000B9548 File Offset: 0x000B7748
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x000B955C File Offset: 0x000B775C
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
