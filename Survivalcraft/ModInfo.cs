using System;

// Token: 0x02000004 RID: 4
[Serializable]
public struct ModInfo : IEquatable<ModInfo>
{
	// Token: 0x0600000A RID: 10 RVA: 0x00002A0C File Offset: 0x00000C0C
	public ModInfo(string name, string description, uint version, string scversion, string url, string updateUrl, string authorList, string credits, string logo, string screenshots, string parent, string dependency, string dependants = null, bool usedependencyInfo = false)
	{
		this.Name = name;
		this.Description = description;
		this.Version = version;
		this.ScVersion = scversion;
		this.Url = url;
		this.UpdateUrl = updateUrl;
		this.AuthorList = authorList;
		this.Credits = credits;
		this.Logo = logo;
		this.Screenshots = screenshots;
		this.Parent = parent;
		this.Dependency = dependency;
		this.Dependants = dependants;
		this.UseDependencyInfo = usedependencyInfo;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002A88 File Offset: 0x00000C88
	public override bool Equals(object obj)
	{
		return obj is ModInfo && this.ToString() == ((ModInfo)obj).ToString();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002AC4 File Offset: 0x00000CC4
	public bool Equals(ModInfo other)
	{
		return this.ToString() == other.ToString();
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00002AE4 File Offset: 0x00000CE4
	public override int GetHashCode()
	{
		return this.ToString().GetHashCode();
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002AF7 File Offset: 0x00000CF7
	public override string ToString()
	{
		return string.Format("{0} {1} - {2} ({3})", new object[]
		{
			this.Name,
			this.Version,
			this.Description,
			this.Url
		});
	}

	// Token: 0x0400000C RID: 12
	public string Name;

	// Token: 0x0400000D RID: 13
	public string Description;

	// Token: 0x0400000E RID: 14
	public uint Version;

	// Token: 0x0400000F RID: 15
	public string ScVersion;

	// Token: 0x04000010 RID: 16
	public string Url;

	// Token: 0x04000011 RID: 17
	public string UpdateUrl;

	// Token: 0x04000012 RID: 18
	public string AuthorList;

	// Token: 0x04000013 RID: 19
	public string Credits;

	// Token: 0x04000014 RID: 20
	public string Logo;

	// Token: 0x04000015 RID: 21
	public string Screenshots;

	// Token: 0x04000016 RID: 22
	public string Parent;

	// Token: 0x04000017 RID: 23
	public string Dependency;

	// Token: 0x04000018 RID: 24
	public string Dependants;

	// Token: 0x04000019 RID: 25
	public bool UseDependencyInfo;
}
