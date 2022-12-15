using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x020002B2 RID: 690
	public static class MotdManager
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x060013CA RID: 5066 RVA: 0x000994DA File Offset: 0x000976DA
		// (set) Token: 0x060013CB RID: 5067 RVA: 0x000994E1 File Offset: 0x000976E1
		public static MotdManager.Message MessageOfTheDay
		{
			get
			{
				return MotdManager.m_message;
			}
			set
			{
				MotdManager.m_message = value;
				if (MotdManager.MessageOfTheDayUpdated != null)
				{
					MotdManager.MessageOfTheDayUpdated();
				}
			}
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060013CC RID: 5068 RVA: 0x000994FC File Offset: 0x000976FC
		// (remove) Token: 0x060013CD RID: 5069 RVA: 0x00099530 File Offset: 0x00097730
		public static event Action MessageOfTheDayUpdated;

		// Token: 0x060013CE RID: 5070 RVA: 0x00099563 File Offset: 0x00097763
		public static void ForceRedownload()
		{
			SettingsManager.MotdLastUpdateTime = DateTime.MinValue;
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0009956F File Offset: 0x0009776F
		public static void Initialize()
		{
			if (VersionsManager.Version != VersionsManager.LastLaunchedVersion)
			{
				MotdManager.ForceRedownload();
			}
		}

		// Token: 0x060013D0 RID: 5072 RVA: 0x00099588 File Offset: 0x00097788
		public static void Update()
		{
			if (Time.PeriodicEvent(1.0, 0.0))
			{
				TimeSpan t = TimeSpan.FromHours(SettingsManager.MotdUpdatePeriodHours);
				DateTime now = DateTime.Now;
				if (now >= SettingsManager.MotdLastUpdateTime + t)
				{
					SettingsManager.MotdLastUpdateTime = now;
					Log.Information("Downloading MOTD");
					AnalyticsManager.LogEvent("[MotdManager] Downloading MOTD", new AnalyticsParameter[]
					{
						new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff"))
					});
					string url = MotdManager.GetMotdUrl();
					WebManager.Get(url, null, null, null, delegate(byte[] result)
					{
						try
						{
							string motdLastDownloadedData = MotdManager.UnpackMotd(result);
							MotdManager.MessageOfTheDay = null;
							SettingsManager.MotdLastDownloadedData = motdLastDownloadedData;
							Log.Information("Downloaded MOTD");
							AnalyticsManager.LogEvent("[MotdManager] Downloaded MOTD", new AnalyticsParameter[]
							{
								new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff")),
								new AnalyticsParameter("Url", url)
							});
							SettingsManager.MotdUseBackupUrl = false;
						}
						catch (Exception ex)
						{
							Log.Error("Failed processing MOTD string. Reason: " + ex.Message);
							SettingsManager.MotdUseBackupUrl = !SettingsManager.MotdUseBackupUrl;
						}
					}, delegate(Exception error)
					{
						Log.Error("Failed downloading MOTD. Reason: {0}", new object[]
						{
							error.Message
						});
						SettingsManager.MotdUseBackupUrl = !SettingsManager.MotdUseBackupUrl;
					});
				}
			}
			if (MotdManager.MessageOfTheDay == null && !string.IsNullOrEmpty(SettingsManager.MotdLastDownloadedData))
			{
				MotdManager.MessageOfTheDay = MotdManager.ParseMotd(SettingsManager.MotdLastDownloadedData);
				if (MotdManager.MessageOfTheDay == null)
				{
					SettingsManager.MotdLastDownloadedData = string.Empty;
				}
			}
		}

		// Token: 0x060013D1 RID: 5073 RVA: 0x00099698 File Offset: 0x00097898
		public static string UnpackMotd(byte[] data)
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = new StreamReader(memoryStream).ReadToEnd();
			}
			return result;
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x000996DC File Offset: 0x000978DC
		public static MotdManager.Message ParseMotd(string dataString)
		{
			try
			{
				int num = dataString.IndexOf("<Motd");
				if (num < 0)
				{
					throw new InvalidOperationException("Invalid MOTD data string.");
				}
				int num2 = dataString.IndexOf("</Motd>");
				if (num2 >= 0 && num2 > num)
				{
					num2 += 7;
				}
				XElement xelement = XmlUtils.LoadXmlFromString(dataString.Substring(num, num2 - num), true);
				SettingsManager.MotdUpdatePeriodHours = (double)XmlUtils.GetAttributeValue<int>(xelement, "UpdatePeriodHours", 24);
				SettingsManager.MotdUpdateUrl = XmlUtils.GetAttributeValue<string>(xelement, "UpdateUrl", SettingsManager.MotdUpdateUrl);
				SettingsManager.MotdBackupUpdateUrl = XmlUtils.GetAttributeValue<string>(xelement, "BackupUpdateUrl", SettingsManager.MotdBackupUpdateUrl);
				MotdManager.Message message = new MotdManager.Message();
				foreach (XElement xelement2 in xelement.Elements())
				{
					if (Widget.IsNodeIncludedOnCurrentPlatform(xelement2))
					{
						MotdManager.Line item = new MotdManager.Line
						{
							Time = XmlUtils.GetAttributeValue<float>(xelement2, "Time"),
							Node = xelement2.Elements().FirstOrDefault<XElement>(),
							Text = xelement2.Value
						};
						message.Lines.Add(item);
					}
				}
				return message;
			}
			catch (Exception ex)
			{
				Log.Warning("Failed extracting MOTD string. Reason: " + ex.Message);
			}
			return null;
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x00099840 File Offset: 0x00097A40
		public static string GetMotdUrl()
		{
			if (SettingsManager.MotdUseBackupUrl)
			{
				return string.Format(SettingsManager.MotdBackupUpdateUrl, VersionsManager.SerializationVersion, ModsManager.modSettings.languageType);
			}
			return string.Format(SettingsManager.MotdUpdateUrl, VersionsManager.SerializationVersion, ModsManager.modSettings.languageType);
		}

		// Token: 0x04000DA3 RID: 3491
		public static MotdManager.Message m_message;

		// Token: 0x020004B5 RID: 1205
		public class Message
		{
			// Token: 0x04001772 RID: 6002
			public List<MotdManager.Line> Lines = new List<MotdManager.Line>();
		}

		// Token: 0x020004B6 RID: 1206
		public class Line
		{
			// Token: 0x04001773 RID: 6003
			public float Time;

			// Token: 0x04001774 RID: 6004
			public XElement Node;

			// Token: 0x04001775 RID: 6005
			public string Text;
		}
	}
}
