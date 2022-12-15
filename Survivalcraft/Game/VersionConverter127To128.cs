using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000341 RID: 833
	public class VersionConverter127To128 : VersionConverter
	{
		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06001795 RID: 6037 RVA: 0x000BA54A File Offset: 0x000B874A
		public override string SourceVersion
		{
			get
			{
				return "1.27";
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x000BA551 File Offset: 0x000B8751
		public override string TargetVersion
		{
			get
			{
				return "1.28";
			}
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x000BA558 File Offset: 0x000B8758
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x000BA56C File Offset: 0x000B876C
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
