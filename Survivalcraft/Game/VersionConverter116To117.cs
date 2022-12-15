using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000336 RID: 822
	public class VersionConverter116To117 : VersionConverter
	{
		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600175A RID: 5978 RVA: 0x000B9698 File Offset: 0x000B7898
		public override string SourceVersion
		{
			get
			{
				return "1.16";
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x0600175B RID: 5979 RVA: 0x000B969F File Offset: 0x000B789F
		public override string TargetVersion
		{
			get
			{
				return "1.17";
			}
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x000B96A6 File Offset: 0x000B78A6
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x000B96BC File Offset: 0x000B78BC
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
