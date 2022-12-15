using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000345 RID: 837
	public class VersionConverter14To15 : VersionConverter
	{
		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060017AD RID: 6061 RVA: 0x000BB590 File Offset: 0x000B9790
		public override string SourceVersion
		{
			get
			{
				return "1.4";
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x000BB597 File Offset: 0x000B9797
		public override string TargetVersion
		{
			get
			{
				return "1.5";
			}
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000BB59E File Offset: 0x000B979E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x000BB5B4 File Offset: 0x000B97B4
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
