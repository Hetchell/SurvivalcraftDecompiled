using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034B RID: 843
	public class VersionConverter20To21 : VersionConverter
	{
		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x000BB9B0 File Offset: 0x000B9BB0
		public override string SourceVersion
		{
			get
			{
				return "2.0";
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060017CC RID: 6092 RVA: 0x000BB9B7 File Offset: 0x000B9BB7
		public override string TargetVersion
		{
			get
			{
				return "2.1";
			}
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x000BB9C0 File Offset: 0x000B9BC0
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			string value = string.Empty;
			foreach (XElement xelement in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "GameInfo"
			select e)
			{
				foreach (XElement node in xelement.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "TerrainGenerationMode"))
				{
					if (XmlUtils.GetAttributeValue<string>(node, "Value", "") == "Normal")
					{
						XmlUtils.SetAttributeValue(node, "Value", "Continent");
					}
				}
				XElement xelement2 = xelement.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "CharacterSkinName").FirstOrDefault<XElement>();
				if (xelement2 != null)
				{
					value = XmlUtils.GetAttributeValue<string>(xelement2, "Value", string.Empty);
				}
			}
			if (string.IsNullOrEmpty(value))
			{
				value = "$Male1";
			}
			foreach (XElement xelement3 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Player"
			select e)
			{
				XmlUtils.SetAttributeValue(xelement3, "Name", "Players");
				XElement xelement4 = new XElement("Values");
				xelement4.SetAttributeValue("Name", "Players");
				XElement xelement5 = new XElement("Values");
				xelement5.SetAttributeValue("Name", "1");
				xelement4.Add(xelement5);
				foreach (XElement xelement6 in xelement3.Elements().ToArray<XElement>())
				{
					xelement6.Remove();
					xelement5.Add(xelement6);
				}
				xelement5.Add(new XElement("Value", new object[]
				{
					new XAttribute("Name", "CharacterSkinName"),
					new XAttribute("Type", "string"),
					new XAttribute("Value", value)
				}));
				xelement3.Add(xelement4);
				xelement3.Add(new XElement("Value", new object[]
				{
					new XAttribute("Name", "NextPlayerIndex"),
					new XAttribute("Type", "int"),
					new XAttribute("Value", "2")
				}));
			}
			foreach (XElement xelement7 in from e in projectNode.Element("Subsystems").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "PlayerStats"
			select e)
			{
				XElement xelement8 = xelement7.Elements("Values").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "PlayerStats").FirstOrDefault<XElement>();
				if (xelement8 != null)
				{
					XElement xelement9 = new XElement("Values");
					XmlUtils.SetAttributeValue(xelement9, "Name", "Stats");
					xelement7.Add(xelement9);
					XmlUtils.SetAttributeValue(xelement8, "Name", "1");
					xelement8.Remove();
					xelement9.Add(xelement8);
				}
			}
			foreach (XElement node2 in from e in projectNode.Element("Entities").Elements()
			where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty).StartsWith("Player")
			select e)
			{
				XmlUtils.SetAttributeValue(node2, "Guid", "bef1b918-6418-41c9-a598-95e8ffd39ab3");
				XmlUtils.SetAttributeValue(node2, "Name", "MalePlayer");
			}
			foreach (XElement xelement10 in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement11 in (from e in xelement10.Descendants("Value")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "SpawnPool"
				select e).ToList<XElement>())
				{
					xelement11.Remove();
				}
			}
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x000BBFAC File Offset: 0x000BA1AC
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
