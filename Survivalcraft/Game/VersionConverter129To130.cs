using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000343 RID: 835
	public class VersionConverter129To130 : VersionConverter
	{
		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x000BB430 File Offset: 0x000B9630
		public override string SourceVersion
		{
			get
			{
				return "1.29";
			}
		}

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x060017A4 RID: 6052 RVA: 0x000BB437 File Offset: 0x000B9637
		public override string TargetVersion
		{
			get
			{
				return "1.30";
			}
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x000BB43E File Offset: 0x000B963E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x000BB454 File Offset: 0x000B9654
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
