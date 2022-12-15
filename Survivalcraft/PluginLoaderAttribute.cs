using System;

// Token: 0x02000006 RID: 6
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class PluginLoaderAttribute : Attribute
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x0600001F RID: 31 RVA: 0x000036C9 File Offset: 0x000018C9
	public ModInfo ModInfo
	{
		get
		{
			return this.info;
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000036D4 File Offset: 0x000018D4
	public PluginLoaderAttribute(string name, string description, uint version, string scversion, string url, string updateUrl, string authorList, string credits, string logo, string screenshots, string parent, string dependency = null, string dependants = null, bool usedependencyInfo = false)
	{
		this.info = new ModInfo(name, description, version, scversion, url, updateUrl, authorList, credits, logo, screenshots, parent, dependency, dependants, usedependencyInfo);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000370B File Offset: 0x0000190B
	public PluginLoaderAttribute(string name, string description, uint version)
	{
		this.info.Name = name;
		this.info.Description = description;
		this.info.Version = version;
	}

	// Token: 0x04000031 RID: 49
	public readonly ModInfo info;
}
