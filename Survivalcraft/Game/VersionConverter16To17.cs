using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000347 RID: 839
	public class VersionConverter16To17 : VersionConverter
	{
		// Token: 0x170003AB RID: 939
		// (get) Token: 0x060017B7 RID: 6071 RVA: 0x000BB6F0 File Offset: 0x000B98F0
		public override string SourceVersion
		{
			get
			{
				return "1.6";
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x060017B8 RID: 6072 RVA: 0x000BB6F7 File Offset: 0x000B98F7
		public override string TargetVersion
		{
			get
			{
				return "1.7";
			}
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x000BB6FE File Offset: 0x000B98FE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x000BB714 File Offset: 0x000B9914
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
