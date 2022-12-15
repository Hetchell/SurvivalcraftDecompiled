using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x0200034D RID: 845
	public static class VersionsManager
	{
		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x060017D9 RID: 6105 RVA: 0x000BD18B File Offset: 0x000BB38B
		public static Platform Platform
		{
			get
			{
				return Platform.Android;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x060017DA RID: 6106 RVA: 0x000BD18E File Offset: 0x000BB38E
		public static BuildConfiguration BuildConfiguration
		{
			get
			{
				return BuildConfiguration.Release;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x060017DB RID: 6107 RVA: 0x000BD191 File Offset: 0x000BB391
		// (set) Token: 0x060017DC RID: 6108 RVA: 0x000BD198 File Offset: 0x000BB398
		public static string Version { get; set; }

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x060017DD RID: 6109 RVA: 0x000BD1A0 File Offset: 0x000BB3A0
		// (set) Token: 0x060017DE RID: 6110 RVA: 0x000BD1A7 File Offset: 0x000BB3A7
		public static string SerializationVersion { get; set; }

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x060017DF RID: 6111 RVA: 0x000BD1AF File Offset: 0x000BB3AF
		// (set) Token: 0x060017E0 RID: 6112 RVA: 0x000BD1B6 File Offset: 0x000BB3B6
		public static string LastLaunchedVersion { get; set; }

		// Token: 0x060017E1 RID: 6113 RVA: 0x000BD1C0 File Offset: 0x000BB3C0
		static VersionsManager()
		{
			AssemblyName assemblyName = new AssemblyName(typeof(VersionsManager).GetTypeInfo().Assembly.FullName);
			VersionsManager.Version = string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				assemblyName.Version.Major,
				assemblyName.Version.Minor,
				assemblyName.Version.Build,
				assemblyName.Version.Revision
			});
			VersionsManager.SerializationVersion = string.Format("{0}.{1}", assemblyName.Version.Major, assemblyName.Version.Minor);
			Assembly[] array = TypeCache.LoadedAssemblies.ToArray<Assembly>();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TypeInfo typeInfo in array[i].DefinedTypes)
				{
					if (!typeInfo.IsAbstract && !typeInfo.IsInterface && typeof(VersionConverter).GetTypeInfo().IsAssignableFrom(typeInfo))
					{
						VersionConverter item = (VersionConverter)Activator.CreateInstance(typeInfo.AsType());
						VersionsManager.m_versionConverters.Add(item);
					}
				}
			}
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x000BD32C File Offset: 0x000BB52C
		public static void Initialize()
		{
			VersionsManager.LastLaunchedVersion = SettingsManager.LastLaunchedVersion;
			SettingsManager.LastLaunchedVersion = VersionsManager.Version;
			if (VersionsManager.Version != VersionsManager.LastLaunchedVersion)
			{
				AnalyticsManager.LogEvent("[VersionsManager] Upgrade game", new AnalyticsParameter[]
				{
					new AnalyticsParameter("LastVersion", VersionsManager.LastLaunchedVersion),
					new AnalyticsParameter("CurrentVersion", VersionsManager.Version)
				});
			}
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x000BD39C File Offset: 0x000BB59C
		public static void UpgradeProjectXml(XElement projectNode)
		{
			string attributeValue = XmlUtils.GetAttributeValue<string>(projectNode, "Version", "1.0");
			if (attributeValue != VersionsManager.SerializationVersion)
			{
				List<VersionConverter> list = VersionsManager.FindTransform(attributeValue, VersionsManager.SerializationVersion, VersionsManager.m_versionConverters, 0);
				if (list == null)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Cannot find conversion path from version \"",
						attributeValue,
						"\" to version \"",
						VersionsManager.SerializationVersion,
						"\""
					}));
				}
				foreach (VersionConverter versionConverter in list)
				{
					Log.Information(string.Concat(new string[]
					{
						"Upgrading world version \"",
						versionConverter.SourceVersion,
						"\" to \"",
						versionConverter.TargetVersion,
						"\"."
					}));
					versionConverter.ConvertProjectXml(projectNode);
				}
				string attributeValue2 = XmlUtils.GetAttributeValue<string>(projectNode, "Version", "1.0");
				if (attributeValue2 != VersionsManager.SerializationVersion)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Upgrade produced invalid project version. Expected \"",
						VersionsManager.SerializationVersion,
						"\", found \"",
						attributeValue2,
						"\"."
					}));
				}
			}
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000BD4E4 File Offset: 0x000BB6E4
		public static void UpgradeWorld(string directoryName)
		{
			WorldInfo worldInfo = WorldsManager.GetWorldInfo(directoryName);
			if (worldInfo == null)
			{
				throw new InvalidOperationException("Cannot determine version of world at \"" + directoryName + "\"");
			}
			if (worldInfo.SerializationVersion != VersionsManager.SerializationVersion)
			{
				ProgressManager.UpdateProgress("Upgrading World To " + VersionsManager.SerializationVersion, 0f);
				List<VersionConverter> list = VersionsManager.FindTransform(worldInfo.SerializationVersion, VersionsManager.SerializationVersion, VersionsManager.m_versionConverters, 0);
				if (list == null)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Cannot find conversion path from version \"",
						worldInfo.SerializationVersion,
						"\" to version \"",
						VersionsManager.SerializationVersion,
						"\""
					}));
				}
				foreach (VersionConverter versionConverter in list)
				{
					Log.Information(string.Concat(new string[]
					{
						"Upgrading world version \"",
						versionConverter.SourceVersion,
						"\" to \"",
						versionConverter.TargetVersion,
						"\"."
					}));
					versionConverter.ConvertWorld(directoryName);
				}
				WorldInfo worldInfo2 = WorldsManager.GetWorldInfo(directoryName);
				if (worldInfo2.SerializationVersion != VersionsManager.SerializationVersion)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Upgrade produced invalid project version. Expected \"",
						VersionsManager.SerializationVersion,
						"\", found \"",
						worldInfo2.SerializationVersion,
						"\"."
					}));
				}
				AnalyticsManager.LogEvent("[VersionConverter] Upgrade world", new AnalyticsParameter[]
				{
					new AnalyticsParameter("SourceVersion", worldInfo.SerializationVersion),
					new AnalyticsParameter("TargetVersion", VersionsManager.SerializationVersion)
				});
			}
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000BD6A0 File Offset: 0x000BB8A0
		public static int CompareVersions(string v1, string v2)
		{
			string[] array = v1.Split(new char[]
			{
				'.'
			});
			string[] array2 = v2.Split(new char[]
			{
				'.'
			});
			for (int i = 0; i < MathUtils.Min(array.Length, array2.Length); i++)
			{
				int num2;
				int num3;
				int num = (!int.TryParse(array[i], out num2) || !int.TryParse(array2[i], out num3)) ? string.CompareOrdinal(array[i], array2[i]) : (num2 - num3);
				if (num != 0)
				{
					return num;
				}
			}
			return array.Length - array2.Length;
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000BD720 File Offset: 0x000BB920
		public static List<VersionConverter> FindTransform(string sourceVersion, string targetVersion, IEnumerable<VersionConverter> converters, int depth)
		{
			if (depth > 100)
			{
				throw new InvalidOperationException("Too deep recursion when searching for version converters. Check for possible loops in transforms.");
			}
			if (sourceVersion == targetVersion)
			{
				return new List<VersionConverter>();
			}
			List<VersionConverter> result = null;
			int num = int.MaxValue;
			foreach (VersionConverter versionConverter in converters)
			{
				if (versionConverter.SourceVersion == sourceVersion)
				{
					List<VersionConverter> list = VersionsManager.FindTransform(versionConverter.TargetVersion, targetVersion, converters, depth + 1);
					if (list != null && list.Count < num)
					{
						num = list.Count;
						list.Insert(0, versionConverter);
						result = list;
					}
				}
			}
			return result;
		}

		// Token: 0x040010E2 RID: 4322
		public static List<VersionConverter> m_versionConverters = new List<VersionConverter>();
	}
}
