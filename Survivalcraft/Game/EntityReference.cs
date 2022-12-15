using System;
using System.Globalization;
using GameEntitySystem;

namespace Game
{
	// Token: 0x02000266 RID: 614
	public struct EntityReference
	{
		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x0600124E RID: 4686 RVA: 0x0008D9E0 File Offset: 0x0008BBE0
		public string ReferenceString
		{
			get
			{
				if (this.m_referenceType == EntityReference.ReferenceType.Null)
				{
					return "null:";
				}
				if (this.m_referenceType == EntityReference.ReferenceType.Local)
				{
					return "local:" + this.m_componentReference;
				}
				if (this.m_referenceType == EntityReference.ReferenceType.ByEntityId)
				{
					return "id:" + this.m_entityReference + ":" + this.m_componentReference;
				}
				if (this.m_referenceType == EntityReference.ReferenceType.ByEntityName)
				{
					return "name:" + this.m_entityReference + ":" + this.m_componentReference;
				}
				throw new Exception("Unknown entity reference type.");
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x0600124F RID: 4687 RVA: 0x0008DA6C File Offset: 0x0008BC6C
		public static EntityReference Null
		{
			get
			{
				return default(EntityReference);
			}
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x0008DA84 File Offset: 0x0008BC84
		public Entity GetEntity(Entity localEntity, IdToEntityMap idToEntityMap, bool throwIfNotFound)
		{
			Entity entity;
			if (this.m_referenceType == EntityReference.ReferenceType.Null)
			{
				entity = null;
			}
			else if (this.m_referenceType == EntityReference.ReferenceType.Local)
			{
				entity = localEntity;
			}
			else if (this.m_referenceType == EntityReference.ReferenceType.ByEntityId)
			{
				int id = int.Parse(this.m_entityReference, CultureInfo.InvariantCulture);
				entity = idToEntityMap.FindEntity(id);
			}
			else
			{
				if (this.m_referenceType != EntityReference.ReferenceType.ByEntityName)
				{
					throw new Exception("Unknown entity reference type.");
				}
				entity = localEntity.Project.FindSubsystem<SubsystemNames>(true).FindEntityByName(this.m_entityReference);
			}
			if (entity != null)
			{
				return entity;
			}
			if (throwIfNotFound)
			{
				throw new Exception("Required entity \"" + this.ReferenceString + "\" not found.");
			}
			return null;
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x0008DB20 File Offset: 0x0008BD20
		public T GetComponent<T>(Entity localEntity, IdToEntityMap idToEntityMap, bool throwIfNotFound) where T : class
		{
			Entity entity = this.GetEntity(localEntity, idToEntityMap, throwIfNotFound);
			if (entity == null)
			{
				return default(T);
			}
			return entity.FindComponent<T>(this.m_componentReference, throwIfNotFound);
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x0008DB54 File Offset: 0x0008BD54
		public bool IsNullOrEmpty()
		{
			return this.m_referenceType == EntityReference.ReferenceType.Null || (this.m_referenceType == EntityReference.ReferenceType.Local && string.IsNullOrEmpty(this.m_componentReference)) || (this.m_referenceType == EntityReference.ReferenceType.ByEntityId && this.m_entityReference == "0") || (this.m_referenceType == EntityReference.ReferenceType.ByEntityName && string.IsNullOrEmpty(this.m_entityReference));
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x0008DBB4 File Offset: 0x0008BDB4
		public static EntityReference Local(Component component)
		{
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.Local,
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x0008DBF4 File Offset: 0x0008BDF4
		public static EntityReference FromId(Component component, EntityToIdMap entityToIdMap)
		{
			int num = entityToIdMap.FindId((component != null) ? component.Entity : null);
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityId,
				m_entityReference = num.ToString(CultureInfo.InvariantCulture),
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x0008DC5C File Offset: 0x0008BE5C
		public static EntityReference FromId(Entity entity, EntityToIdMap entityToIdMap)
		{
			int num = entityToIdMap.FindId(entity);
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityId,
				m_entityReference = num.ToString(CultureInfo.InvariantCulture),
				m_componentReference = string.Empty
			};
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x0008DCA4 File Offset: 0x0008BEA4
		public static EntityReference FromName(Component component)
		{
			string entityReference = (component != null) ? component.Entity.FindComponent<ComponentName>(null, true).Name : string.Empty;
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityName,
				m_entityReference = entityReference,
				m_componentReference = ((component != null) ? component.ValuesDictionary.DatabaseObject.Name : string.Empty)
			};
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x0008DD08 File Offset: 0x0008BF08
		public static EntityReference FromName(Entity entity)
		{
			string entityReference = (entity != null) ? entity.FindComponent<ComponentName>(null, true).Name : string.Empty;
			return new EntityReference
			{
				m_referenceType = EntityReference.ReferenceType.ByEntityName,
				m_entityReference = entityReference,
				m_componentReference = string.Empty
			};
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x0008DD54 File Offset: 0x0008BF54
		public static EntityReference FromReferenceString(string referenceString)
		{
			EntityReference result = default(EntityReference);
			if (string.IsNullOrEmpty(referenceString))
			{
				result.m_referenceType = EntityReference.ReferenceType.Null;
				result.m_entityReference = string.Empty;
				result.m_componentReference = string.Empty;
			}
			else
			{
				string[] array = referenceString.Split(new char[]
				{
					':'
				});
				if (array.Length == 1)
				{
					result.m_referenceType = EntityReference.ReferenceType.Local;
					result.m_entityReference = string.Empty;
					result.m_componentReference = array[0];
				}
				else
				{
					if (array.Length != 2 && array.Length != 3)
					{
						throw new Exception("Invalid entity reference. Too many tokens.");
					}
					if (array[0] == "null" && array.Length == 2)
					{
						result.m_referenceType = EntityReference.ReferenceType.Null;
						result.m_entityReference = string.Empty;
						result.m_componentReference = string.Empty;
					}
					else if (array[0] == "local" && array.Length == 2)
					{
						result.m_referenceType = EntityReference.ReferenceType.Local;
						result.m_componentReference = array[1];
					}
					else if (array[0] == "id")
					{
						result.m_referenceType = EntityReference.ReferenceType.ByEntityId;
						result.m_entityReference = array[1];
						result.m_componentReference = ((array.Length == 3) ? array[2] : string.Empty);
					}
					else
					{
						if (!(array[0] == "name"))
						{
							throw new Exception("Unknown entity reference type.");
						}
						result.m_referenceType = EntityReference.ReferenceType.ByEntityId;
						result.m_entityReference = array[1];
						result.m_componentReference = ((array.Length == 3) ? array[2] : string.Empty);
					}
				}
			}
			return result;
		}

		// Token: 0x04000C95 RID: 3221
		public EntityReference.ReferenceType m_referenceType;

		// Token: 0x04000C96 RID: 3222
		public string m_entityReference;

		// Token: 0x04000C97 RID: 3223
		public string m_componentReference;

		// Token: 0x02000490 RID: 1168
		public enum ReferenceType
		{
			// Token: 0x040016EE RID: 5870
			Null,
			// Token: 0x040016EF RID: 5871
			Local,
			// Token: 0x040016F0 RID: 5872
			ByEntityId,
			// Token: 0x040016F1 RID: 5873
			ByEntityName
		}
	}
}
