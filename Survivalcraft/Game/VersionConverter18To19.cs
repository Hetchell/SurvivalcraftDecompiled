using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000349 RID: 841
	public class VersionConverter18To19 : VersionConverter
	{
		// Token: 0x170003AF RID: 943
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x000BB850 File Offset: 0x000B9A50
		public override string SourceVersion
		{
			get
			{
				return "1.8";
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x060017C2 RID: 6082 RVA: 0x000BB857 File Offset: 0x000B9A57
		public override string TargetVersion
		{
			get
			{
				return "1.9";
			}
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x000BB85E File Offset: 0x000B9A5E
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x000BB874 File Offset: 0x000B9A74
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
