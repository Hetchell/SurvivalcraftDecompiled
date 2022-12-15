using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson
{
	// Token: 0x02000008 RID: 8
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class JsonArray : List<object>
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00003737 File Offset: 0x00001937
		public JsonArray()
		{
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000373F File Offset: 0x0000193F
		public JsonArray(int capacity) : base(capacity)
		{
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003748 File Offset: 0x00001948
		public override string ToString()
		{
			return SimpleJson.SerializeObject(this) ?? string.Empty;
		}
	}
}
