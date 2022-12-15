using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000365 RID: 869
	public static class DatabaseManager
	{
		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06001886 RID: 6278 RVA: 0x000C3379 File Offset: 0x000C1579
		public static GameDatabase GameDatabase
		{
			get
			{
				if (DatabaseManager.m_gameDatabase != null)
				{
					return DatabaseManager.m_gameDatabase;
				}
				throw new InvalidOperationException("Database not loaded.");
			}
		}

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001887 RID: 6279 RVA: 0x000C3392 File Offset: 0x000C1592
		public static ICollection<ValuesDictionary> EntitiesValuesDictionaries
		{
			get
			{
				return DatabaseManager.m_valueDictionaries.Values;
			}
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x000C33A0 File Offset: 0x000C15A0
		public static void Initialize()
		{
			if (DatabaseManager.m_gameDatabase == null)
			{
				XElement node = ContentManager.Get<XElement>("Database");
				ContentManager.Dispose("Database");
				ModsManager.CombineXml(node, ModsManager.GetEntries(".xdb"), "Guid", "Name", null);
				DatabaseManager.m_gameDatabase = new GameDatabase(XmlDatabaseSerializer.LoadDatabase(node));
				foreach (DatabaseObject databaseObject in DatabaseManager.GameDatabase.Database.Root.GetExplicitNestingChildren(DatabaseManager.GameDatabase.EntityTemplateType, false))
				{
					ValuesDictionary valuesDictionary = new ValuesDictionary();
					valuesDictionary.PopulateFromDatabaseObject(databaseObject);
					DatabaseManager.m_valueDictionaries.Add(databaseObject.Name, valuesDictionary);
				}
				return;
			}
			throw new InvalidOperationException("Database already loaded.");
		}

		// Token: 0x06001889 RID: 6281 RVA: 0x000C3474 File Offset: 0x000C1674
		public static ValuesDictionary FindEntityValuesDictionary(string entityTemplateName, bool throwIfNotFound)
		{
			ValuesDictionary result;
			if (!DatabaseManager.m_valueDictionaries.TryGetValue(entityTemplateName, out result) && throwIfNotFound)
			{
				throw new InvalidOperationException("EntityTemplate \"" + entityTemplateName + "\" not found.");
			}
			return result;
		}

		// Token: 0x0600188A RID: 6282 RVA: 0x000C34AC File Offset: 0x000C16AC
		public static ValuesDictionary FindValuesDictionaryForComponent(ValuesDictionary entityVd, Type componentType)
		{
			foreach (ValuesDictionary valuesDictionary in entityVd.Values.OfType<ValuesDictionary>())
			{
				if (valuesDictionary.DatabaseObject.Type == DatabaseManager.GameDatabase.MemberComponentTemplateType)
				{
					Type type = TypeCache.FindType(valuesDictionary.GetValue<string>("Class"), true, true);
					if (componentType.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
					{
						return valuesDictionary;
					}
				}
			}
			return null;
		}

		// Token: 0x0600188B RID: 6283 RVA: 0x000C353C File Offset: 0x000C173C
		public static Entity CreateEntity(Project project, string entityTemplateName, bool throwIfNotFound)
		{
			ValuesDictionary valuesDictionary = DatabaseManager.FindEntityValuesDictionary(entityTemplateName, throwIfNotFound);
			if (valuesDictionary == null)
			{
				return null;
			}
			return project.CreateEntity(valuesDictionary);
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x000C3560 File Offset: 0x000C1760
		public static Entity CreateEntity(Project project, string entityTemplateName, ValuesDictionary overrides, bool throwIfNotFound)
		{
			ValuesDictionary valuesDictionary = DatabaseManager.FindEntityValuesDictionary(entityTemplateName, throwIfNotFound);
			if (valuesDictionary != null)
			{
				valuesDictionary.ApplyOverrides(overrides);
				return project.CreateEntity(valuesDictionary);
			}
			return null;
		}

		// Token: 0x0400115D RID: 4445
		public static GameDatabase m_gameDatabase;

		// Token: 0x0400115E RID: 4446
		public static Dictionary<string, ValuesDictionary> m_valueDictionaries = new Dictionary<string, ValuesDictionary>();
	}
}
