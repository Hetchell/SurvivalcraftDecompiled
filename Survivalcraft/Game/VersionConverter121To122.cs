using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033B RID: 827
	public class VersionConverter121To122 : VersionConverter
	{
		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001773 RID: 6003 RVA: 0x000B9C0C File Offset: 0x000B7E0C
		public override string SourceVersion
		{
			get
			{
				return "1.21";
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x000B9C13 File Offset: 0x000B7E13
		public override string TargetVersion
		{
			get
			{
				return "1.22";
			}
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x000B9C1C File Offset: 0x000B7E1C
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			foreach (XElement xelement in projectNode.Element("Subsystems").Elements())
			{
				foreach (XElement xelement2 in from e in xelement.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreatureSpawn"
				select e)
				{
					XmlUtils.SetAttributeValue(xelement2, "Name", "Spawn");
					foreach (XElement node in xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreaturesData"))
					{
						XmlUtils.SetAttributeValue(node, "Name", "SpawnsData");
					}
					foreach (XElement node2 in xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CreaturesGenerated"))
					{
						XmlUtils.SetAttributeValue(node2, "Name", "IsSpawned");
					}
				}
			}
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x000B9E14 File Offset: 0x000B8014
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
