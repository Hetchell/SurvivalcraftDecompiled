using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200033A RID: 826
	public class VersionConverter120To121 : VersionConverter
	{
		// Token: 0x17000391 RID: 913
		// (get) Token: 0x0600176E RID: 5998 RVA: 0x000B9958 File Offset: 0x000B7B58
		public override string SourceVersion
		{
			get
			{
				return "1.20";
			}
		}

		// Token: 0x17000392 RID: 914
		// (get) Token: 0x0600176F RID: 5999 RVA: 0x000B995F File Offset: 0x000B7B5F
		public override string TargetVersion
		{
			get
			{
				return "1.21";
			}
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x000B9968 File Offset: 0x000B7B68
		public override void ConvertProjectXml(XElement projectNode)
		{
			XmlUtils.SetAttributeValue(projectNode, "Version", this.TargetVersion);
			foreach (XElement xelement in projectNode.Element("Entities").Elements())
			{
				foreach (XElement xelement2 in from e in xelement.Elements("Values")
				where XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Body" || XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "Frame"
				select e)
				{
					using (IEnumerator<XElement> enumerator3 = xelement2.Elements("Value").Where((XElement e) => XmlUtils.GetAttributeValue<string>(e, "Name", string.Empty) == "LocalMatrix").GetEnumerator())
					{
						if (enumerator3.MoveNext())
						{
							XElement xelement3 = enumerator3.Current;
							Vector3 vector;
							Quaternion quaternion;
							Vector3 vector2;
							XmlUtils.GetAttributeValue<Matrix>(xelement3, "Value").Decompose(out vector, out quaternion, out vector2);
							XElement xelement4 = new XElement("Value");
							XElement xelement5 = new XElement("Value");
							XmlUtils.SetAttributeValue(xelement4, "Name", "Position");
							XmlUtils.SetAttributeValue(xelement4, "Type", "Microsoft.Xna.Framework.Vector3");
							XmlUtils.SetAttributeValue(xelement4, "Value", vector2);
							XmlUtils.SetAttributeValue(xelement5, "Name", "Rotation");
							XmlUtils.SetAttributeValue(xelement5, "Type", "Microsoft.Xna.Framework.Quaternion");
							XmlUtils.SetAttributeValue(xelement5, "Value", quaternion);
							xelement2.Add(xelement4);
							xelement2.Add(xelement5);
							xelement3.Remove();
						}
					}
				}
			}
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x000B9B80 File Offset: 0x000B7D80
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
