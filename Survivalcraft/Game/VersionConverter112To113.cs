using System;
using System.IO;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000332 RID: 818
	public class VersionConverter112To113 : VersionConverter
	{
		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06001742 RID: 5954 RVA: 0x000B924C File Offset: 0x000B744C
		public override string SourceVersion
		{
			get
			{
				return "1.12";
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06001743 RID: 5955 RVA: 0x000B9253 File Offset: 0x000B7453
		public override string TargetVersion
		{
			get
			{
				return "1.13";
			}
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x000B925A File Offset: 0x000B745A
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			this.ProcessNode(projectNode);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x000B9274 File Offset: 0x000B7474
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

		// Token: 0x06001746 RID: 5958 RVA: 0x000B92F8 File Offset: 0x000B74F8
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

		// Token: 0x06001747 RID: 5959 RVA: 0x000B9384 File Offset: 0x000B7584
		public void ProcessAttribute(XAttribute attribute)
		{
			if (attribute.Name == "Value" && attribute.Value == "Dangerous")
			{
				attribute.Value = "Normal";
			}
		}
	}
}
