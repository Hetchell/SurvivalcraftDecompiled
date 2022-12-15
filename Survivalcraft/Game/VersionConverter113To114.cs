using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000333 RID: 819
	public class VersionConverter113To114 : VersionConverter
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06001749 RID: 5961 RVA: 0x000B93C2 File Offset: 0x000B75C2
		public override string SourceVersion
		{
			get
			{
				return "1.13";
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x0600174A RID: 5962 RVA: 0x000B93C9 File Offset: 0x000B75C9
		public override string TargetVersion
		{
			get
			{
				return "1.14";
			}
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x000B93D0 File Offset: 0x000B75D0
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ProcessNode(projectNode);
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x000B93EC File Offset: 0x000B75EC
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

		// Token: 0x0600174D RID: 5965 RVA: 0x000B9470 File Offset: 0x000B7670
		public void ProcessNode(XElement node)
		{
			foreach (XAttribute attribute in node.Attributes())
			{
				this.ProcessAttribute(attribute);
			}
			foreach (XElement node2 in node.Elements())
			{
				this.ProcessNode(node2);
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x000B94FC File Offset: 0x000B76FC
		public void ProcessAttribute(XAttribute attribute)
		{
			if (attribute.Name == "Value" && attribute.Value == "Dangerous")
			{
				attribute.Value = "Normal";
			}
		}
	}
}
