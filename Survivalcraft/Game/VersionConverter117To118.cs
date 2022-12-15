using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000337 RID: 823
	public class VersionConverter117To118 : VersionConverter
	{
		// Token: 0x1700038B RID: 907
		// (get) Token: 0x0600175F RID: 5983 RVA: 0x000B9748 File Offset: 0x000B7948
		public override string SourceVersion
		{
			get
			{
				return "1.17";
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06001760 RID: 5984 RVA: 0x000B974F File Offset: 0x000B794F
		public override string TargetVersion
		{
			get
			{
				return "1.18";
			}
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x000B9756 File Offset: 0x000B7956
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x000B976C File Offset: 0x000B796C
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
