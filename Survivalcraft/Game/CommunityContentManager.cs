using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Engine;
using XmlUtilities;

namespace Game
{
	// Token: 0x02000241 RID: 577
	public static class CommunityContentManager
	{
		// Token: 0x060011AA RID: 4522 RVA: 0x000885F4 File Offset: 0x000867F4
		public static void Initialize()
		{
			CommunityContentManager.Load();
			WorldsManager.WorldDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.World, path));
			};
			BlocksTexturesManager.BlocksTextureDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.BlocksTexture, path));
			};
			CharacterSkinsManager.CharacterSkinDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.CharacterSkin, path));
			};
			FurniturePacksManager.FurniturePackDeleted += delegate(string path)
			{
				CommunityContentManager.m_idToAddressMap.Remove(CommunityContentManager.MakeContentIdString(ExternalContentType.FurniturePack, path));
			};
			Window.Deactivated += delegate()
			{
				CommunityContentManager.Save();
			};
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x000886BC File Offset: 0x000868BC
		public static string GetDownloadedContentAddress(ExternalContentType type, string name)
		{
			string result;
			CommunityContentManager.m_idToAddressMap.TryGetValue(CommunityContentManager.MakeContentIdString(type, name), out result);
			return result;
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x000886E0 File Offset: 0x000868E0
		public static bool IsContentRated(string address, string userId)
		{
			string key = CommunityContentManager.MakeFeedbackCacheKey(address, "Rating", userId);
			return CommunityContentManager.m_feedbackCache.ContainsKey(key);
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00088708 File Offset: 0x00086908
		public static void List(string cursor, string userFilter, string typeFilter, string moderationFilter, string sortOrder, CancellableProgress progress, Action<List<CommunityContentEntry>, string> success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			WebManager.Post("https://m.schub.top/resource", null, null, WebManager.UrlParametersToStream(new Dictionary<string, string>
			{
				{
					"Action",
					"list"
				},
				{
					"Cursor",
					cursor ?? string.Empty
				},
				{
					"UserId",
					userFilter ?? string.Empty
				},
				{
					"Type",
					typeFilter ?? string.Empty
				},
				{
					"Moderation",
					moderationFilter ?? string.Empty
				},
				{
					"SortOrder",
					sortOrder ?? string.Empty
				},
				{
					"Platform",
					VersionsManager.Platform.ToString()
				},
				{
					"Version",
					VersionsManager.Version
				}
			}), progress, delegate(byte[] result)
			{
				try
				{
					XElement xelement = XmlUtils.LoadXmlFromString(Encoding.UTF8.GetString(result, 0, result.Length), true);
					string attributeValue = XmlUtils.GetAttributeValue<string>(xelement, "NextCursor");
					List<CommunityContentEntry> list = new List<CommunityContentEntry>();
					foreach (XElement node in xelement.Elements())
					{
						try
						{
							list.Add(new CommunityContentEntry
							{
								Type = XmlUtils.GetAttributeValue<ExternalContentType>(node, "Type", ExternalContentType.Unknown),
								Name = XmlUtils.GetAttributeValue<string>(node, "Name"),
								Address = XmlUtils.GetAttributeValue<string>(node, "Url"),
								UserId = XmlUtils.GetAttributeValue<string>(node, "UserId"),
								Size = XmlUtils.GetAttributeValue<long>(node, "Size"),
								ExtraText = XmlUtils.GetAttributeValue<string>(node, "ExtraText", string.Empty),
								RatingsAverage = XmlUtils.GetAttributeValue<float>(node, "RatingsAverage", 0f)
							});
						}
						catch (Exception)
						{
						}
					}
					success(list, attributeValue);
				}
				catch (Exception obj)
				{
					failure(obj);
				}
			}, delegate(Exception error)
			{
				failure(error);
			});
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00088830 File Offset: 0x00086A30
		public static void Download(string address, string name, ExternalContentType type, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			WebManager.Get(address, null, null, progress, delegate(byte[] data)
			{
				string hash = CommunityContentManager.CalculateContentHashString(data);
				ExternalContentManager.ImportExternalContent(new MemoryStream(data), type, name, delegate(string downloadedName)
				{
					CommunityContentManager.m_idToAddressMap[CommunityContentManager.MakeContentIdString(type, downloadedName)] = address;
					CommunityContentManager.Feedback(address, "Success", null, hash, (long)data.Length, userId, progress, delegate
					{
					}, delegate
					{
					});
					AnalyticsManager.LogEvent("[CommunityContentManager] Download Success", new AnalyticsParameter[]
					{
						new AnalyticsParameter("Name", name)
					});
					success();
				}, delegate(Exception error)
				{
					CommunityContentManager.Feedback(address, "ImportFailure", null, hash, (long)data.Length, userId, null, delegate
					{
					}, delegate
					{
					});
					AnalyticsManager.LogEvent("[CommunityContentManager] Import Failure", new AnalyticsParameter[]
					{
						new AnalyticsParameter("Name", name),
						new AnalyticsParameter("Error", error.Message.ToString())
					});
					failure(error);
				});
			}, delegate(Exception error)
			{
				CommunityContentManager.Feedback(address, "DownloadFailure", null, null, 0L, userId, null, delegate
				{
				}, delegate
				{
				});
				AnalyticsManager.LogEvent("[CommunityContentManager] Download Failure", new AnalyticsParameter[]
				{
					new AnalyticsParameter("Name", name),
					new AnalyticsParameter("Error", error.Message.ToString())
				});
				failure(error);
			});
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x000888D4 File Offset: 0x00086AD4
		public static void Publish(string address, string name, ExternalContentType type, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (MarketplaceManager.IsTrialMode)
			{
				failure(new InvalidOperationException("Cannot publish links in trial mode."));
				return;
			}
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			CommunityContentManager.VerifyLinkContent(address, name, type, progress, delegate(byte[] data)
			{
				string value = CommunityContentManager.CalculateContentHashString(data);
				WebManager.Post("https://m.schub.top/resource", null, null, WebManager.UrlParametersToStream(new Dictionary<string, string>
				{
					{
						"Action",
						"publish"
					},
					{
						"UserId",
						userId
					},
					{
						"Name",
						name
					},
					{
						"Url",
						address
					},
					{
						"Type",
						type.ToString()
					},
					{
						"Hash",
						value
					},
					{
						"Size",
						data.Length.ToString(CultureInfo.InvariantCulture)
					},
					{
						"Platform",
						VersionsManager.Platform.ToString()
					},
					{
						"Version",
						VersionsManager.Version
					}
				}), progress, delegate
				{
					success();
					AnalyticsManager.LogEvent("[CommunityContentManager] Publish Success", new AnalyticsParameter[]
					{
						new AnalyticsParameter("Name", name),
						new AnalyticsParameter("Type", type.ToString()),
						new AnalyticsParameter("Size", data.Length.ToString()),
						new AnalyticsParameter("User", userId)
					});
				}, delegate(Exception error)
				{
					failure(error);
					AnalyticsManager.LogEvent("[CommunityContentManager] Publish Failure", new AnalyticsParameter[]
					{
						new AnalyticsParameter("Name", name),
						new AnalyticsParameter("Type", type.ToString()),
						new AnalyticsParameter("Size", data.Length.ToString()),
						new AnalyticsParameter("User", userId),
						new AnalyticsParameter("Error", error.Message.ToString())
					});
				});
			}, failure);
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0008899C File Offset: 0x00086B9C
		public static void Delete(string address, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			WebManager.Post("https://m.schub.top/resource", null, null, WebManager.UrlParametersToStream(new Dictionary<string, string>
			{
				{
					"Action",
					"delete"
				},
				{
					"UserId",
					userId
				},
				{
					"Url",
					address
				},
				{
					"Platform",
					VersionsManager.Platform.ToString()
				},
				{
					"Version",
					VersionsManager.Version
				}
			}), progress, delegate
			{
				success();
				AnalyticsManager.LogEvent("[CommunityContentManager] Delete Success", new AnalyticsParameter[]
				{
					new AnalyticsParameter("Name", address),
					new AnalyticsParameter("User", userId)
				});
			}, delegate(Exception error)
			{
				failure(error);
				AnalyticsManager.LogEvent("[CommunityContentManager] Delete Failure", new AnalyticsParameter[]
				{
					new AnalyticsParameter("Name", address),
					new AnalyticsParameter("User", userId),
					new AnalyticsParameter("Error", error.Message.ToString())
				});
			});
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00088A88 File Offset: 0x00086C88
		public static void Rate(string address, string userId, int rating, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			rating = MathUtils.Clamp(rating, 1, 5);
			CommunityContentManager.Feedback(address, "Rating", rating.ToString(CultureInfo.InvariantCulture), null, 0L, userId, progress, success, failure);
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00088AC0 File Offset: 0x00086CC0
		public static void Report(string address, string userId, string report, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			CommunityContentManager.Feedback(address, "Report", report, null, 0L, userId, progress, success, failure);
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00088AE4 File Offset: 0x00086CE4
		public static void SendPlayTime(string address, string userId, double time, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			CommunityContentManager.Feedback(address, "PlayTime", MathUtils.Round(time).ToString(CultureInfo.InvariantCulture), null, 0L, userId, progress, success, failure);
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00088B18 File Offset: 0x00086D18
		public static void VerifyLinkContent(string address, string name, ExternalContentType type, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			WebManager.Get(address, null, null, progress, delegate(byte[] data)
			{
				ExternalContentManager.ImportExternalContent(new MemoryStream(data), type, "__Temp", delegate(string downloadedName)
				{
					ExternalContentManager.DeleteExternalContent(type, downloadedName);
					success(data);
				}, failure);
			}, failure);
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00088B6C File Offset: 0x00086D6C
		public static void Feedback(string address, string feedback, string feedbackParameter, string hash, long size, string userId, CancellableProgress progress, Action success, Action<Exception> failure)
		{
			progress = (progress ?? new CancellableProgress());
			if (!WebManager.IsInternetConnectionAvailable())
			{
				failure(new InvalidOperationException("Internet connection is unavailable."));
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Action", "feedback");
			dictionary.Add("Feedback", feedback);
			if (feedbackParameter != null)
			{
				dictionary.Add("FeedbackParameter", feedbackParameter);
			}
			dictionary.Add("UserId", userId);
			if (address != null)
			{
				dictionary.Add("Url", address);
			}
			if (hash != null)
			{
				dictionary.Add("Hash", hash);
			}
			if (size > 0L)
			{
				dictionary.Add("Size", size.ToString(CultureInfo.InvariantCulture));
			}
			dictionary.Add("Platform", VersionsManager.Platform.ToString());
			dictionary.Add("Version", VersionsManager.Version);
			WebManager.Post("https://m.schub.top/resource", null, null, WebManager.UrlParametersToStream(dictionary), progress, delegate
			{
				string key = CommunityContentManager.MakeFeedbackCacheKey(address, feedback, userId);
				if (CommunityContentManager.m_feedbackCache.ContainsKey(key))
				{
					Task.Run(delegate()
					{
						Task.Delay(1500).Wait();
						failure(new InvalidOperationException("Duplicate feedback."));
					});
					return;
				}
				CommunityContentManager.m_feedbackCache[key] = true;
				success();
			}, delegate(Exception error)
			{
				failure(error);
			});
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00088CB8 File Offset: 0x00086EB8
		public static string CalculateContentHashString(byte[] data)
		{
			string result;
			using (SHA1Managed sha1Managed = new SHA1Managed())
			{
				result = Convert.ToBase64String(sha1Managed.ComputeHash(data));
			}
			return result;
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00088CF8 File Offset: 0x00086EF8
		public static string MakeFeedbackCacheKey(string address, string feedback, string userId)
		{
			return string.Concat(new string[]
			{
				address,
				"\n",
				feedback,
				"\n",
				userId
			});
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00088D21 File Offset: 0x00086F21
		public static string MakeContentIdString(ExternalContentType type, string name)
		{
			return type.ToString() + ":" + name;
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00088D3C File Offset: 0x00086F3C
		public static void Load()
		{
			try
			{
				if (Storage.FileExists("app:CommunityContentCache.xml"))
				{
					using (Stream stream = Storage.OpenFile("app:CommunityContentCache.xml", OpenFileMode.Read))
					{
						XElement xelement = XmlUtils.LoadXmlFromStream(stream, null, true);
						foreach (XElement node in xelement.Element("Feedback").Elements())
						{
							string attributeValue = XmlUtils.GetAttributeValue<string>(node, "Key");
							CommunityContentManager.m_feedbackCache[attributeValue] = true;
						}
						foreach (XElement node2 in xelement.Element("Content").Elements())
						{
							string attributeValue2 = XmlUtils.GetAttributeValue<string>(node2, "Path");
							string attributeValue3 = XmlUtils.GetAttributeValue<string>(node2, "Address");
							CommunityContentManager.m_idToAddressMap[attributeValue2] = attributeValue3;
						}
					}
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Loading Community Content cache failed.", e);
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00088E6C File Offset: 0x0008706C
		public static void Save()
		{
			try
			{
				XElement xelement = new XElement("Cache");
				XElement xelement2 = new XElement("Feedback");
				xelement.Add(xelement2);
				foreach (string value in CommunityContentManager.m_feedbackCache.Keys)
				{
					XElement xelement3 = new XElement("Item");
					XmlUtils.SetAttributeValue(xelement3, "Key", value);
					xelement2.Add(xelement3);
				}
				XElement xelement4 = new XElement("Content");
				xelement.Add(xelement4);
				foreach (KeyValuePair<string, string> keyValuePair in CommunityContentManager.m_idToAddressMap)
				{
					XElement xelement5 = new XElement("Item");
					XmlUtils.SetAttributeValue(xelement5, "Path", keyValuePair.Key);
					XmlUtils.SetAttributeValue(xelement5, "Address", keyValuePair.Value);
					xelement4.Add(xelement5);
				}
				using (Stream stream = Storage.OpenFile("app:CommunityContentCache.xml", OpenFileMode.Create))
				{
					XmlUtils.SaveXmlToStream(xelement, stream, null, true);
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Saving Community Content cache failed.", e);
			}
		}

		// Token: 0x04000BD1 RID: 3025
		public const string m_cacheFilename = "app:/CommunityContentCache.xml";

		// Token: 0x04000BD2 RID: 3026
		public const string m_scResDirAddress = "https://m.schub.top/resource";

		// Token: 0x04000BD3 RID: 3027
		public static Dictionary<string, string> m_idToAddressMap = new Dictionary<string, string>();

		// Token: 0x04000BD4 RID: 3028
		public static Dictionary<string, bool> m_feedbackCache = new Dictionary<string, bool>();
	}
}
