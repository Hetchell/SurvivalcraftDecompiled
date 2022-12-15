using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033D RID: 829
	public class VersionConverter123To124 : VersionConverter
	{
		// Token: 0x17000397 RID: 919
		// (get) Token: 0x0600177D RID: 6013 RVA: 0x000B9F50 File Offset: 0x000B8150
		public override string SourceVersion
		{
			get
			{
				return "1.23";
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x0600177E RID: 6014 RVA: 0x000B9F57 File Offset: 0x000B8157
		public override string TargetVersion
		{
			get
			{
				return "1.24";
			}
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x000B9F5E File Offset: 0x000B815E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x000B9F74 File Offset: 0x000B8174
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
