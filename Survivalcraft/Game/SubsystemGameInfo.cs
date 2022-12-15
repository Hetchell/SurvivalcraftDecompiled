using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000181 RID: 385
	public class SubsystemGameInfo : Subsystem, IUpdateable
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060008C6 RID: 2246 RVA: 0x0003CCCE File Offset: 0x0003AECE
		// (set) Token: 0x060008C7 RID: 2247 RVA: 0x0003CCD6 File Offset: 0x0003AED6
		public WorldSettings WorldSettings { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060008C8 RID: 2248 RVA: 0x0003CCDF File Offset: 0x0003AEDF
		// (set) Token: 0x060008C9 RID: 2249 RVA: 0x0003CCE7 File Offset: 0x0003AEE7
		public string DirectoryName { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060008CA RID: 2250 RVA: 0x0003CCF0 File Offset: 0x0003AEF0
		// (set) Token: 0x060008CB RID: 2251 RVA: 0x0003CCF8 File Offset: 0x0003AEF8
		public double TotalElapsedGameTime { get; set; }

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x0003CD01 File Offset: 0x0003AF01
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x0003CD09 File Offset: 0x0003AF09
		public float TotalElapsedGameTimeDelta { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x0003CD12 File Offset: 0x0003AF12
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x0003CD1A File Offset: 0x0003AF1A
		public int WorldSeed { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x0003CD23 File Offset: 0x0003AF23
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x0003CD26 File Offset: 0x0003AF26
		public IEnumerable<ActiveExternalContentInfo> GetActiveExternalContent()
		{
			string downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.World, this.DirectoryName);
			if (!string.IsNullOrEmpty(downloadedContentAddress))
			{
				yield return new ActiveExternalContentInfo
				{
					Address = downloadedContentAddress,
					DisplayName = this.WorldSettings.Name,
					Type = ExternalContentType.World
				};
			}
			if (!BlocksTexturesManager.IsBuiltIn(this.WorldSettings.BlocksTextureName))
			{
				downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.BlocksTexture, this.WorldSettings.BlocksTextureName);
				if (!string.IsNullOrEmpty(downloadedContentAddress))
				{
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = BlocksTexturesManager.GetDisplayName(this.WorldSettings.BlocksTextureName),
						Type = ExternalContentType.BlocksTexture
					};
				}
			}
			SubsystemPlayers subsystemPlayers = base.Project.FindSubsystem<SubsystemPlayers>(true);
			foreach (PlayerData playerData in subsystemPlayers.PlayersData)
			{
				if (!CharacterSkinsManager.IsBuiltIn(playerData.CharacterSkinName))
				{
					downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.CharacterSkin, playerData.CharacterSkinName);
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = CharacterSkinsManager.GetDisplayName(playerData.CharacterSkinName),
						Type = ExternalContentType.CharacterSkin
					};
				}
			}
			SubsystemFurnitureBlockBehavior subsystemFurnitureBlockBehavior = base.Project.FindSubsystem<SubsystemFurnitureBlockBehavior>(true);
			foreach (FurnitureSet furnitureSet in subsystemFurnitureBlockBehavior.FurnitureSets)
			{
				if (furnitureSet.ImportedFrom != null)
				{
					downloadedContentAddress = CommunityContentManager.GetDownloadedContentAddress(ExternalContentType.FurniturePack, furnitureSet.ImportedFrom);
					yield return new ActiveExternalContentInfo
					{
						Address = downloadedContentAddress,
						DisplayName = FurniturePacksManager.GetDisplayName(furnitureSet.ImportedFrom),
						Type = ExternalContentType.FurniturePack
					};
				}
			}
		}

        // Token: 0x060008D2 RID: 2258 RVA: 0x0003CD38 File Offset: 0x0003AF38
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.WorldSettings = new WorldSettings();
			this.WorldSettings.Load(valuesDictionary);
			this.DirectoryName = valuesDictionary.GetValue<string>("WorldDirectoryName");
			this.TotalElapsedGameTime = valuesDictionary.GetValue<double>("TotalElapsedGameTime");
			//this.WorldSeed = valuesDictionary.GetValue<int>("WorldSeed");
			this.WorldSeed = 1738478217;
		}

        // Token: 0x060008D3 RID: 2259 RVA: 0x0003CDA1 File Offset: 0x0003AFA1
        public override void Save(ValuesDictionary valuesDictionary)
		{
			this.WorldSettings.Save(valuesDictionary, false);
			valuesDictionary.SetValue<int>("WorldSeed", this.WorldSeed);
			valuesDictionary.SetValue<double>("TotalElapsedGameTime", this.TotalElapsedGameTime);
		}

		// Token: 0x060008D4 RID: 2260 RVA: 0x0003CDD4 File Offset: 0x0003AFD4
		public void Update(float dt)
		{
			this.TotalElapsedGameTime += (double)dt;
			this.TotalElapsedGameTimeDelta = ((this.m_lastTotalElapsedGameTime != null) ? ((float)(this.TotalElapsedGameTime - this.m_lastTotalElapsedGameTime.Value)) : 0f);
			this.m_lastTotalElapsedGameTime = new double?(this.TotalElapsedGameTime);
			if (this.m_subsystemTime.GameTime >= 600.0 && this.m_subsystemTime.GameTime - (double)this.m_subsystemTime.GameTimeDelta < 600.0 && UserManager.ActiveUser != null)
			{
				foreach (ActiveExternalContentInfo activeExternalContentInfo in this.GetActiveExternalContent())
				{
					CommunityContentManager.SendPlayTime(activeExternalContentInfo.Address, UserManager.ActiveUser.UniqueId, this.m_subsystemTime.GameTime, null, delegate
					{
					}, delegate
					{
					});
				}
			}
		}

		// Token: 0x0400049F RID: 1183
		public double? m_lastTotalElapsedGameTime;

		// Token: 0x040004A0 RID: 1184
		public SubsystemTime m_subsystemTime;
	}
}
