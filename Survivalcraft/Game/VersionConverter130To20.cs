using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000344 RID: 836
	public class VersionConverter130To20 : VersionConverter
	{
		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x060017A8 RID: 6056 RVA: 0x000BB4E0 File Offset: 0x000B96E0
		public override string SourceVersion
		{
			get
			{
				return "1.30";
			}
		}

		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x060017A9 RID: 6057 RVA: 0x000BB4E7 File Offset: 0x000B96E7
		public override string TargetVersion
		{
			get
			{
				return "2.0";
			}
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000BB4EE File Offset: 0x000B96EE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x000BB504 File Offset: 0x000B9704
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
