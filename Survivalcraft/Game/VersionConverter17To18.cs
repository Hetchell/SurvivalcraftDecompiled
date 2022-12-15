using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000348 RID: 840
	public class VersionConverter17To18 : VersionConverter
	{
		// Token: 0x170003AD RID: 941
		// (get) Token: 0x060017BC RID: 6076 RVA: 0x000BB7A0 File Offset: 0x000B99A0
		public override string SourceVersion
		{
			get
			{
				return "1.7";
			}
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x000BB7A7 File Offset: 0x000B99A7
		public override string TargetVersion
		{
			get
			{
				return "1.8";
			}
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x000BB7AE File Offset: 0x000B99AE
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x000BB7C4 File Offset: 0x000B99C4
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
