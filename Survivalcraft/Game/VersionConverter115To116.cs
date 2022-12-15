using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000335 RID: 821
	public class VersionConverter115To116 : VersionConverter
	{
		// Token: 0x17000387 RID: 903
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x000B95E8 File Offset: 0x000B77E8
		public override string SourceVersion
		{
			get
			{
				return "1.15";
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x000B95EF File Offset: 0x000B77EF
		public override string TargetVersion
		{
			get
			{
				return "1.16";
			}
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x000B95F6 File Offset: 0x000B77F6
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x000B960C File Offset: 0x000B780C
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
