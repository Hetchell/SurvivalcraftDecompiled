using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000257 RID: 599
	public class DropboxExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06001200 RID: 4608 RVA: 0x0008AE18 File Offset: 0x00089018
		public string DisplayName
		{
			get
			{
				return "Dropbox";
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06001201 RID: 4609 RVA: 0x0008AE1F File Offset: 0x0008901F
		public string Description
		{
			get
			{
				if (!this.IsLoggedIn)
				{
					return "Not logged in";
				}
				return "Logged in";
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06001202 RID: 4610 RVA: 0x0008AE34 File Offset: 0x00089034
		public bool SupportsListing
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x0008AE37 File Offset: 0x00089037
		public bool SupportsLinks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06001204 RID: 4612 RVA: 0x0008AE3A File Offset: 0x0008903A
		public bool RequiresLogin
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x0008AE3D File Offset: 0x0008903D
		public bool IsLoggedIn
		{
			get
			{
				return !string.IsNullOrEmpty(SettingsManager.DropboxAccessToken);
			}
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x0008AE4C File Offset: 0x0008904C
		public DropboxExternalContentProvider()
		{
			Program.HandleUri += this.HandleUri;
			Window.Activated += this.WindowActivated;
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x0008AE76 File Offset: 0x00089076
		public void Dispose()
		{
			Program.HandleUri -= this.HandleUri;
			Window.Activated -= this.WindowActivated;
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x0008AE9C File Offset: 0x0008909C
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			try
			{
				if (this.m_loginProcessData != null)
				{
					throw new InvalidOperationException("Login already in progress.");
				}
				if (!WebManager.IsInternetConnectionAvailable())
				{
					throw new InvalidOperationException("Internet connection is unavailable.");
				}
				this.Logout();
				progress.Cancelled += delegate()
				{
					if (this.m_loginProcessData != null)
					{
						DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
						this.m_loginProcessData = null;
						loginProcessData.Fail(this, null);
					}
				};
				this.m_loginProcessData = new DropboxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.Progress = progress;
				this.m_loginProcessData.Success = success;
				this.m_loginProcessData.Failure = failure;
				this.LoginLaunchBrowser();
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0008AF38 File Offset: 0x00089138
		public void Logout()
		{
			SettingsManager.DropboxAccessToken = string.Empty;
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x0008AF44 File Offset: 0x00089144
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.DropboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("recursive", false);
				jsonObject.Add("include_media_info", false);
				jsonObject.Add("include_deleted", false);
				jsonObject.Add("include_has_explicit_shared_members", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://api.dropboxapi.com/2/files/list_folder", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(DropboxExternalContentProvider.JsonObjectToEntry(jsonObject2));
					}
					catch (Exception obj2)
					{
						failure(obj2);
					}
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x0008B058 File Offset: 0x00089258
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				WebManager.Get("https://content.dropboxapi.com/2/files/download", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.DropboxAccessToken
					},
					{
						"Dropbox-API-Arg",
						jsonObject.ToString()
					}
				}, progress, delegate(byte[] result)
				{
					success(new MemoryStream(result));
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x0008B110 File Offset: 0x00089310
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("mode", "add");
				jsonObject.Add("autorename", true);
				jsonObject.Add("mute", false);
				WebManager.Post("https://content.dropboxapi.com/2/files/upload", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.DropboxAccessToken
					},
					{
						"Content-Type",
						"application/octet-stream"
					},
					{
						"Dropbox-API-Arg",
						jsonObject.ToString()
					}
				}, stream, progress, delegate
				{
					success(null);
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x0008B20C File Offset: 0x0008940C
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.DropboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", DropboxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("short_url", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://api.dropboxapi.com/2/sharing/create_shared_link", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(DropboxExternalContentProvider.JsonObjectToLinkAddress(jsonObject2));
					}
					catch (Exception obj2)
					{
						failure(obj2);
					}
				}, delegate(Exception error)
				{
					failure(error);
				});
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x0008B2EC File Offset: 0x000894EC
		public void LoginLaunchBrowser()
		{
			try
			{
				this.m_loginProcessData.IsTokenFlow = true;
				WebBrowserManager.LaunchBrowser("https://www.dropbox.com/oauth2/authorize?" + WebManager.UrlParametersToString(new Dictionary<string, string>
				{
					{
						"response_type",
						"token"
					},
					{
						"client_id",
						"1unnzwkb8igx70k"
					},
					{
						"redirect_uri",
						"com.candyrufusgames.survivalcraft2://redirect"
					}
				}));
			}
			catch (Exception error)
			{
				this.m_loginProcessData.Fail(this, error);
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x0008B374 File Offset: 0x00089574
		public void WindowActivated()
		{
			if (this.m_loginProcessData != null && !this.m_loginProcessData.IsTokenFlow)
			{
				DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
				this.m_loginProcessData = null;
				TextBoxDialog dialog = new TextBoxDialog("Enter Dropbox authorization code", "", 256, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							WebManager.Post("https://api.dropboxapi.com/oauth2/token", new Dictionary<string, string>
							{
								{
									"code",
									s.Trim()
								},
								{
									"client_id",
									"1unnzwkb8igx70k"
								},
								{
									"client_secret",
									"3i5u3j3141php7u"
								},
								{
									"grant_type",
									"authorization_code"
								}
							}, null, new MemoryStream(), loginProcessData.Progress, delegate(byte[] result)
							{
								SettingsManager.DropboxAccessToken = ((IDictionary<string, object>)WebManager.JsonFromBytes(result))["access_token"].ToString();
								loginProcessData.Succeed(this);
							}, delegate(Exception error)
							{
								loginProcessData.Fail(this, error);
							});
							return;
						}
						catch (Exception error)
						{
							loginProcessData.Fail(this, error);
							return;
						}
					}
					loginProcessData.Fail(this, null);
				});
				DialogsManager.ShowDialog(null, dialog);
			}
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x0008B3E0 File Offset: 0x000895E0
		public void HandleUri(Uri uri)
		{
			if (this.m_loginProcessData == null)
			{
				this.m_loginProcessData = new DropboxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.IsTokenFlow = true;
			}
			DropboxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
			this.m_loginProcessData = null;
			if (loginProcessData.IsTokenFlow)
			{
				try
				{
					if (uri != null && !string.IsNullOrEmpty(uri.Fragment))
					{
						Dictionary<string, string> dictionary = WebManager.UrlParametersFromString(uri.Fragment.TrimStart(new char[]
						{
							'#'
						}));
						if (dictionary.ContainsKey("access_token"))
						{
							SettingsManager.DropboxAccessToken = dictionary["access_token"];
							loginProcessData.Succeed(this);
							return;
						}
						if (dictionary.ContainsKey("error"))
						{
							throw new Exception(dictionary["error"]);
						}
					}
					throw new Exception("Could not retrieve Dropbox access token.");
				}
				catch (Exception error)
				{
					loginProcessData.Fail(this, error);
				}
			}
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0008B4C4 File Offset: 0x000896C4
		public void VerifyLoggedIn()
		{
			if (!this.IsLoggedIn)
			{
				throw new InvalidOperationException("Not logged in to Dropbox in this app.");
			}
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x0008B4DC File Offset: 0x000896DC
		public static ExternalContentEntry JsonObjectToEntry(JsonObject jsonObject)
		{
			ExternalContentEntry externalContentEntry = new ExternalContentEntry();
			if (jsonObject.ContainsKey("entries"))
			{
				foreach (object obj in ((JsonArray)jsonObject["entries"]))
				{
					JsonObject jsonObject2 = (JsonObject)obj;
					ExternalContentEntry externalContentEntry2 = new ExternalContentEntry();
					externalContentEntry2.Path = jsonObject2["path_display"].ToString();
					externalContentEntry2.Type = ((jsonObject2[".tag"].ToString() == "folder") ? ExternalContentType.Directory : ExternalContentManager.ExtensionToType(Storage.GetExtension(externalContentEntry2.Path)));
					if (externalContentEntry2.Type != ExternalContentType.Directory)
					{
						externalContentEntry2.Time = (jsonObject2.ContainsKey("server_modified") ? DateTime.Parse(jsonObject2["server_modified"].ToString(), CultureInfo.InvariantCulture) : new DateTime(2000, 1, 1));
						externalContentEntry2.Size = (jsonObject2.ContainsKey("size") ? ((long)jsonObject2["size"]) : 0L);
					}
					externalContentEntry.ChildEntries.Add(externalContentEntry2);
				}
				return externalContentEntry;
			}
			return externalContentEntry;
		}

		// Token: 0x06001213 RID: 4627 RVA: 0x0008B620 File Offset: 0x00089820
		public static string JsonObjectToLinkAddress(JsonObject jsonObject)
		{
			if (jsonObject.ContainsKey("url"))
			{
				return jsonObject["url"].ToString().Replace("www.dropbox.", "dl.dropbox.").Replace("?dl=0", "") + "?dl=1";
			}
			throw new InvalidOperationException("Share information not found.");
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x0008B67D File Offset: 0x0008987D
		public static string NormalizePath(string path)
		{
			if (path == "/")
			{
				return string.Empty;
			}
			if (path.Length > 0 && path[0] != '/')
			{
				return "/" + path;
			}
			return path;
		}

		// Token: 0x04000C1B RID: 3099
		public const string m_appKey = "1unnzwkb8igx70k";

		// Token: 0x04000C1C RID: 3100
		public const string m_appSecret = "3i5u3j3141php7u";

		// Token: 0x04000C1D RID: 3101
		public const string m_redirectUri = "com.candyrufusgames.survivalcraft2://redirect";

		// Token: 0x04000C1E RID: 3102
		public DropboxExternalContentProvider.LoginProcessData m_loginProcessData;

		// Token: 0x02000489 RID: 1161
		public class LoginProcessData
		{
			// Token: 0x06001F74 RID: 8052 RVA: 0x000E1D35 File Offset: 0x000DFF35
			public void Succeed(DropboxExternalContentProvider provider)
			{
				provider.m_loginProcessData = null;
				Action success = this.Success;
				if (success == null)
				{
					return;
				}
				success();
			}

			// Token: 0x06001F75 RID: 8053 RVA: 0x000E1D4E File Offset: 0x000DFF4E
			public void Fail(DropboxExternalContentProvider provider, Exception error)
			{
				provider.m_loginProcessData = null;
				Action<Exception> failure = this.Failure;
				if (failure == null)
				{
					return;
				}
				failure(error);
			}

			// Token: 0x040016DD RID: 5853
			public bool IsTokenFlow;

			// Token: 0x040016DE RID: 5854
			public Action Success;

			// Token: 0x040016DF RID: 5855
			public Action<Exception> Failure;

			// Token: 0x040016E0 RID: 5856
			public CancellableProgress Progress;
		}
	}
}
