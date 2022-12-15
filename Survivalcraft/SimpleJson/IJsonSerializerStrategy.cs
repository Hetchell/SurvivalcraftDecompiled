using System;
using System.CodeDom.Compiler;

namespace SimpleJson
{
	// Token: 0x02000007 RID: 7
	[GeneratedCode("simple-json", "1.0.0")]
	internal interface IJsonSerializerStrategy
	{
		// Token: 0x06000022 RID: 34
		bool TrySerializeNonPrimitiveObject(object input, out object output);

		// Token: 0x06000023 RID: 35
		object DeserializeObject(object value, Type type);
	}
}
