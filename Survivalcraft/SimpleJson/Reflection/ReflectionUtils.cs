using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace SimpleJson.Reflection
{
	// Token: 0x0200000C RID: 12
	[GeneratedCode("reflection-utils", "1.0.0")]
	internal class ReflectionUtils
	{
		// Token: 0x06000065 RID: 101 RVA: 0x000051CC File Offset: 0x000033CC
		public static TypeInfo GetTypeInfo(Type type)
		{
			return type.GetTypeInfo();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000051D4 File Offset: 0x000033D4
		public static Attribute GetAttribute(MemberInfo info, Type type)
		{
			if (info == null || type == null || !info.IsDefined(type))
			{
				return null;
			}
			return info.GetCustomAttribute(type);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000051FC File Offset: 0x000033FC
		public static Type GetGenericListElementType(Type type)
		{
			foreach (Type type2 in type.GetTypeInfo().ImplementedInterfaces)
			{
				if (ReflectionUtils.IsTypeGeneric(type2) && type2.GetGenericTypeDefinition() == typeof(IList<>))
				{
					return ReflectionUtils.GetGenericTypeArguments(type2)[0];
				}
			}
			return ReflectionUtils.GetGenericTypeArguments(type)[0];
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000527C File Offset: 0x0000347C
		public static Attribute GetAttribute(Type objectType, Type attributeType)
		{
			if (objectType == null || attributeType == null || !objectType.GetTypeInfo().IsDefined(attributeType))
			{
				return null;
			}
			return objectType.GetTypeInfo().GetCustomAttribute(attributeType);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000052AC File Offset: 0x000034AC
		public static Type[] GetGenericTypeArguments(Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000052B9 File Offset: 0x000034B9
		public static bool IsTypeGeneric(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000052C8 File Offset: 0x000034C8
		public static bool IsTypeGenericeCollectionInterface(Type type)
		{
			if (!ReflectionUtils.IsTypeGeneric(type))
			{
				return false;
			}
			Type genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == typeof(IList<>) || genericTypeDefinition == typeof(ICollection<>) || genericTypeDefinition == typeof(IEnumerable<>);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000531C File Offset: 0x0000351C
		public static bool IsAssignableFrom(Type type1, Type type2)
		{
			return ReflectionUtils.GetTypeInfo(type1).IsAssignableFrom(ReflectionUtils.GetTypeInfo(type2));
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00005330 File Offset: 0x00003530
		public static bool IsTypeDictionary(Type type)
		{
			return typeof(IDictionary<, >).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) || (ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<, >));
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000537F File Offset: 0x0000357F
		public static bool IsNullableType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000053A5 File Offset: 0x000035A5
		public static object ToNullableType(object obj, Type nullableType)
		{
			if (obj != null)
			{
				return Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
			}
			return null;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000053BD File Offset: 0x000035BD
		public static bool IsValueType(Type type)
		{
			return ReflectionUtils.GetTypeInfo(type).IsValueType;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000053CA File Offset: 0x000035CA
		public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
		{
			return type.GetTypeInfo().DeclaredConstructors;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000053D8 File Offset: 0x000035D8
		public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
		{
			foreach (ConstructorInfo constructorInfo in ReflectionUtils.GetConstructors(type))
			{
				ParameterInfo[] parameters = constructorInfo.GetParameters();
				if (argsType.Length == parameters.Length)
				{
					int num = 0;
					bool flag = true;
					ParameterInfo[] parameters2 = constructorInfo.GetParameters();
					for (int i = 0; i < parameters2.Length; i++)
					{
						if (parameters2[i].ParameterType != argsType[num])
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						return constructorInfo;
					}
				}
			}
			return null;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005474 File Offset: 0x00003674
		public static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetRuntimeProperties();
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000547C File Offset: 0x0000367C
		public static IEnumerable<FieldInfo> GetFields(Type type)
		{
			return type.GetRuntimeFields();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005484 File Offset: 0x00003684
		public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetMethod;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000548C File Offset: 0x0000368C
		public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
		{
			return propertyInfo.SetMethod;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005494 File Offset: 0x00003694
		public static ReflectionUtils.ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
		{
			return ReflectionUtils.GetConstructorByReflection(constructorInfo);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000549C File Offset: 0x0000369C
		public static ReflectionUtils.ConstructorDelegate GetContructor(Type type, params Type[] argsType)
		{
			return ReflectionUtils.GetConstructorByReflection(type, argsType);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000054A5 File Offset: 0x000036A5
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
		{
			return (object[] args) => constructorInfo.Invoke(args);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000054C0 File Offset: 0x000036C0
		public static ReflectionUtils.ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
		{
			ConstructorInfo constructorInfo = ReflectionUtils.GetConstructorInfo(type, argsType);
			if (!(constructorInfo == null))
			{
				return ReflectionUtils.GetConstructorByReflection(constructorInfo);
			}
			return null;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000054E6 File Offset: 0x000036E6
		public static ReflectionUtils.GetDelegate GetGetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetGetMethodByReflection(propertyInfo);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000054EE File Offset: 0x000036EE
		public static ReflectionUtils.GetDelegate GetGetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetGetMethodByReflection(fieldInfo);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000054F6 File Offset: 0x000036F6
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
			return (object source) => methodInfo.Invoke(source, ReflectionUtils.EmptyObjects);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005514 File Offset: 0x00003714
		public static ReflectionUtils.GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
		{
			return (object source) => fieldInfo.GetValue(source);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000552D File Offset: 0x0000372D
		public static ReflectionUtils.SetDelegate GetSetMethod(PropertyInfo propertyInfo)
		{
			return ReflectionUtils.GetSetMethodByReflection(propertyInfo);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005535 File Offset: 0x00003735
		public static ReflectionUtils.SetDelegate GetSetMethod(FieldInfo fieldInfo)
		{
			return ReflectionUtils.GetSetMethodByReflection(fieldInfo);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000553D File Offset: 0x0000373D
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
		{
			MethodInfo methodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
			return delegate(object source, object value)
			{
				methodInfo.Invoke(source, new object[]
				{
					value
				});
			};
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000555B File Offset: 0x0000375B
		public static ReflectionUtils.SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
		{
			return delegate(object source, object value)
			{
				fieldInfo.SetValue(source, value);
			};
		}

		// Token: 0x0400004A RID: 74
		public static readonly object[] EmptyObjects = new object[0];

		// Token: 0x020003B2 RID: 946
		// (Invoke) Token: 0x06001C57 RID: 7255
		public delegate object GetDelegate(object source);

		// Token: 0x020003B3 RID: 947
		// (Invoke) Token: 0x06001C5B RID: 7259
		public delegate void SetDelegate(object source, object value);

		// Token: 0x020003B4 RID: 948
		// (Invoke) Token: 0x06001C5F RID: 7263
		public delegate object ConstructorDelegate(params object[] args);

		// Token: 0x020003B5 RID: 949
		// (Invoke) Token: 0x06001C63 RID: 7267
		public delegate TValue ThreadSafeDictionaryValueFactory<TKey, TValue>(TKey key);

		// Token: 0x020003B6 RID: 950
		public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
		{
			// Token: 0x17000502 RID: 1282
			// (get) Token: 0x06001C66 RID: 7270 RVA: 0x000D9F12 File Offset: 0x000D8112
			public ICollection<TKey> Keys
			{
				get
				{
					return this._dictionary.Keys;
				}
			}

			// Token: 0x17000503 RID: 1283
			// (get) Token: 0x06001C67 RID: 7271 RVA: 0x000D9F1F File Offset: 0x000D811F
			public ICollection<TValue> Values
			{
				get
				{
					return this._dictionary.Values;
				}
			}

			// Token: 0x17000504 RID: 1284
			public TValue this[TKey key]
			{
				get
				{
					return this.Get(key);
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17000505 RID: 1285
			// (get) Token: 0x06001C6A RID: 7274 RVA: 0x000D9F3C File Offset: 0x000D813C
			public int Count
			{
				get
				{
					return this._dictionary.Count;
				}
			}

			// Token: 0x17000506 RID: 1286
			// (get) Token: 0x06001C6B RID: 7275 RVA: 0x000D9F49 File Offset: 0x000D8149
			public bool IsReadOnly
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x06001C6C RID: 7276 RVA: 0x000D9F50 File Offset: 0x000D8150
			public ThreadSafeDictionary(ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
			{
				this._valueFactory = valueFactory;
			}

			// Token: 0x06001C6D RID: 7277 RVA: 0x000D9F6C File Offset: 0x000D816C
			public TValue Get(TKey key)
			{
				if (this._dictionary == null)
				{
					return this.AddValue(key);
				}
				TValue result;
				if (!this._dictionary.TryGetValue(key, out result))
				{
					return this.AddValue(key);
				}
				return result;
			}

			// Token: 0x06001C6E RID: 7278 RVA: 0x000D9FA4 File Offset: 0x000D81A4
			public TValue AddValue(TKey key)
			{
				TValue tvalue = this._valueFactory(key);
				object @lock = this._lock;
				TValue result;
				lock (@lock)
				{
					if (this._dictionary != null)
					{
						TValue tvalue2;
						if (this._dictionary.TryGetValue(key, out tvalue2))
						{
							result = tvalue2;
						}
						else
						{
							Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(this._dictionary);
							dictionary[key] = tvalue;
							this._dictionary = dictionary;
							result = tvalue;
						}
					}
					else
					{
						this._dictionary = new Dictionary<TKey, TValue>();
						this._dictionary[key] = tvalue;
						result = tvalue;
					}
				}
				return result;
			}

			// Token: 0x06001C6F RID: 7279 RVA: 0x000DA048 File Offset: 0x000D8248
			public void Add(TKey key, TValue value)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C70 RID: 7280 RVA: 0x000DA04F File Offset: 0x000D824F
			public bool ContainsKey(TKey key)
			{
				return this._dictionary.ContainsKey(key);
			}

			// Token: 0x06001C71 RID: 7281 RVA: 0x000DA05D File Offset: 0x000D825D
			public bool Remove(TKey key)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C72 RID: 7282 RVA: 0x000DA064 File Offset: 0x000D8264
			public bool TryGetValue(TKey key, out TValue value)
			{
				value = this[key];
				return true;
			}

			// Token: 0x06001C73 RID: 7283 RVA: 0x000DA074 File Offset: 0x000D8274
			public void Add(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C74 RID: 7284 RVA: 0x000DA07B File Offset: 0x000D827B
			public void Clear()
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C75 RID: 7285 RVA: 0x000DA082 File Offset: 0x000D8282
			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C76 RID: 7286 RVA: 0x000DA089 File Offset: 0x000D8289
			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C77 RID: 7287 RVA: 0x000DA090 File Offset: 0x000D8290
			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06001C78 RID: 7288 RVA: 0x000DA097 File Offset: 0x000D8297
			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x06001C79 RID: 7289 RVA: 0x000DA0A9 File Offset: 0x000D82A9
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this._dictionary.GetEnumerator();
			}

			// Token: 0x040013D1 RID: 5073
			public readonly object _lock = new object();

			// Token: 0x040013D2 RID: 5074
			public readonly ReflectionUtils.ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;

			// Token: 0x040013D3 RID: 5075
			public Dictionary<TKey, TValue> _dictionary;
		}
	}
}
