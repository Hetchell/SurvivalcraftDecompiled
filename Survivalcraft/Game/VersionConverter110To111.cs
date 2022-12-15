using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000330 RID: 816
	public class VersionConverter110To111 : VersionConverter
	{
		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06001738 RID: 5944 RVA: 0x000B90EC File Offset: 0x000B72EC
		public override string SourceVersion
		{
			get
			{
				return "1.10";
			}
		}

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06001739 RID: 5945 RVA: 0x000B90F3 File Offset: 0x000B72F3
		public override string TargetVersion
		{
			get
			{
				return "1.11";
			}
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x000B90FA File Offset: 0x000B72FA
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x000B9110 File Offset: 0x000B7310
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
