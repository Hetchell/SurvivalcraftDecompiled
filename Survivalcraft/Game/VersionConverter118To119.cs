using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000338 RID: 824
	public class VersionConverter118To119 : VersionConverter
	{
		// Token: 0x1700038D RID: 909
		// (get) Token: 0x06001764 RID: 5988 RVA: 0x000B97F8 File Offset: 0x000B79F8
		public override string SourceVersion
		{
			get
			{
				return "1.18";
			}
		}

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06001765 RID: 5989 RVA: 0x000B97FF File Offset: 0x000B79FF
		public override string TargetVersion
		{
			get
			{
				return "1.19";
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x000B9806 File Offset: 0x000B7A06
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x000B981C File Offset: 0x000B7A1C
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
