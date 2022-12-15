using System;
using Engine.Serialization;

namespace Game
{
	// Token: 0x02000267 RID: 615
	[HumanReadableConverter(typeof(EntityReference))]
	public class EntityReferenceHumanReadableConverter : IHumanReadableConverter
	{
		// Token: 0x06001259 RID: 4697 RVA: 0x0008DECC File Offset: 0x0008C0CC
		public string ConvertToString(object value)
		{
			return ((EntityReference)value).ReferenceString;
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x0008DEE7 File Offset: 0x0008C0E7
		public object ConvertFromString(Type type, string data)
		{
			return EntityReference.FromReferenceString(data);
		}
	}
}
