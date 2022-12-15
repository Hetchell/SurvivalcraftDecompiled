using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000211 RID: 529
	public class SPMBoxExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x17000252 RID: 594
		// (get) Token: 0x0600105E RID: 4190 RVA: 0x0007D31F File Offset: 0x0007B51F
		public string DisplayName
		{
			get
			{
				return "SPMBox中国社区";
			}
		}

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600105F RID: 4191 RVA: 0x0007D326 File Offset: 0x0007B526
		public string Description
		{
			get
			{
				if (!this.IsLoggedIn)
				{
					return "未登录";
				}
				return "登陆";
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06001060 RID: 4192 RVA: 0x0007D33B File Offset: 0x0007B53B
		public bool SupportsListing
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001061 RID: 4193 RVA: 0x0007D33E File Offset: 0x0007B53E
		public bool SupportsLinks
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06001062 RID: 4194 RVA: 0x0007D341 File Offset: 0x0007B541
		public bool RequiresLogin
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001063 RID: 4195 RVA: 0x0007D344 File Offset: 0x0007B544
		public bool IsLoggedIn
		{
			get
			{
				return !string.IsNullOrEmpty(SettingsManager.ScpboxAccessToken);
			}
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0007D353 File Offset: 0x0007B553
		public SPMBoxExternalContentProvider()
		{
			Program.HandleUri += this.HandleUri;
			Window.Activated += this.WindowActivated;
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0007D37D File Offset: 0x0007B57D
		public void Dispose()
		{
			Program.HandleUri -= this.HandleUri;
			Window.Activated -= this.WindowActivated;
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0007D3A4 File Offset: 0x0007B5A4
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			try
			{
				if (this.m_loginProcessData != null)
				{
					throw new InvalidOperationException("登陆已经在进程中");
				}
				if (!WebManager.IsInternetConnectionAvailable())
				{
					throw new InvalidOperationException("网络连接错误");
				}
				this.Logout();
				progress.Cancelled += delegate()
				{
					if (this.m_loginProcessData != null)
					{
						SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
						this.m_loginProcessData = null;
						loginProcessData.Fail(this, null);
					}
				};
				this.m_loginProcessData = new SPMBoxExternalContentProvider.LoginProcessData();
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

		// Token: 0x06001067 RID: 4199 RVA: 0x0007D440 File Offset: 0x0007B640
		public void Logout()
		{
			this.m_loginProcessData = null;
			SettingsManager.ScpboxAccessToken = string.Empty;
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x0007D454 File Offset: 0x0007B654
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.ScpboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("recursive", false);
				jsonObject.Add("include_media_info", false);
				jsonObject.Add("include_deleted", false);
				jsonObject.Add("include_has_explicit_shared_members", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://m.schub.top/com/files/list_folder", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(SPMBoxExternalContentProvider.JsonObjectToEntry(jsonObject2));
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

		// Token: 0x06001069 RID: 4201 RVA: 0x0007D568 File Offset: 0x0007B768
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				WebManager.Get("https://m.schub.top/com/files/download", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.ScpboxAccessToken
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

		// Token: 0x0600106A RID: 4202 RVA: 0x0007D620 File Offset: 0x0007B820
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("mode", "add");
				jsonObject.Add("autorename", true);
				jsonObject.Add("mute", false);
				WebManager.Post("https://m.schub.top/com/files/upload", null, new Dictionary<string, string>
				{
					{
						"Authorization",
						"Bearer " + SettingsManager.ScpboxAccessToken
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

		// Token: 0x0600106B RID: 4203 RVA: 0x0007D71C File Offset: 0x0007B91C
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				this.VerifyLoggedIn();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Authorization", "Bearer " + SettingsManager.ScpboxAccessToken);
				dictionary.Add("Content-Type", "application/json");
				JsonObject jsonObject = new JsonObject();
				jsonObject.Add("path", SPMBoxExternalContentProvider.NormalizePath(path));
				jsonObject.Add("short_url", false);
				MemoryStream data = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));
				WebManager.Post("https://m.schub.top/com/sharing/create_shared_link", null, dictionary, data, progress, delegate(byte[] result)
				{
					try
					{
						JsonObject jsonObject2 = (JsonObject)WebManager.JsonFromBytes(result);
						success(SPMBoxExternalContentProvider.JsonObjectToLinkAddress(jsonObject2));
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

		// Token: 0x0600106C RID: 4204 RVA: 0x0007D7FC File Offset: 0x0007B9FC
		public void LoginLaunchBrowser()
		{
			try
			{
				LoginDialog login = new LoginDialog();
				login.succ = delegate(byte[] a)
				{
					JsonObject jsonObject = (JsonObject)SimpleJson.SimpleJson.DeserializeObject(new StreamReader(new MemoryStream(a)).ReadToEnd());
					int num = int.Parse(jsonObject["code"].ToString());
					string text = jsonObject["msg"].ToString();
					if (num == 200)
					{
						SettingsManager.ScpboxAccessToken = ((JsonObject)jsonObject["data"])["accessToken"].ToString();
						DialogsManager.HideAllDialogs();
						return;
					}
					login.tip.Text = text;
				};
				login.fail = delegate(Exception e)
				{
					login.tip.Text = e.ToString();
				};
				DialogsManager.ShowDialog(null, login);
			}
			catch (Exception error)
			{
				this.m_loginProcessData.Fail(this, error);
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0007D878 File Offset: 0x0007BA78
		public void WindowActivated()
		{
			if (this.m_loginProcessData != null && !this.m_loginProcessData.IsTokenFlow)
			{
				SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
				this.m_loginProcessData = null;
				TextBoxDialog dialog = new TextBoxDialog("输入用户登录Token:", "", 256, delegate(string s)
				{
					if (s != null)
					{
						try
						{
							WebManager.Post("https://m.schub.top/com/oauth2/token", new Dictionary<string, string>
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
								SettingsManager.ScpboxAccessToken = ((IDictionary<string, object>)WebManager.JsonFromBytes(result))["access_token"].ToString();
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

		// Token: 0x0600106E RID: 4206 RVA: 0x0007D8E4 File Offset: 0x0007BAE4
		public void HandleUri(Uri uri)
		{
			if (this.m_loginProcessData == null)
			{
				this.m_loginProcessData = new SPMBoxExternalContentProvider.LoginProcessData();
				this.m_loginProcessData.IsTokenFlow = true;
			}
			SPMBoxExternalContentProvider.LoginProcessData loginProcessData = this.m_loginProcessData;
			this.m_loginProcessData = null;
			if (loginProcessData.IsTokenFlow)
			{
				try
				{
					if (!(uri != null) || string.IsNullOrEmpty(uri.Fragment))
					{
						throw new Exception("不能接收来自SPMBox的身份验证信息");
					}
					Dictionary<string, string> dictionary = WebManager.UrlParametersFromString(uri.Fragment.TrimStart(new char[]
					{
						'#'
					}));
					if (!dictionary.ContainsKey("access_token"))
					{
						if (dictionary.ContainsKey("error"))
						{
							throw new Exception(dictionary["error"]);
						}
						throw new Exception("不能接收来自SPMBox的身份验证信息");
					}
					else
					{
						SettingsManager.ScpboxAccessToken = dictionary["access_token"];
						loginProcessData.Succeed(this);
					}
				}
				catch (Exception error)
				{
					loginProcessData.Fail(this, error);
				}
			}
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0007D9D0 File Offset: 0x0007BBD0
		public void VerifyLoggedIn()
		{
			if (!this.IsLoggedIn)
			{
				throw new InvalidOperationException("这个应用未登录到SPMBox中国社区");
			}
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x0007D9E8 File Offset: 0x0007BBE8
		internal static ExternalContentEntry JsonObjectToEntry(JsonObject jsonObject)
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

		// Token: 0x06001071 RID: 4209 RVA: 0x0007DB2C File Offset: 0x0007BD2C
		internal static string JsonObjectToLinkAddress(JsonObject jsonObject)
		{
			if (jsonObject.ContainsKey("url"))
			{
				return jsonObject["url"].ToString();
			}
			throw new InvalidOperationException("没有分享链接信息");
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x0007DB56 File Offset: 0x0007BD56
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

		// Token: 0x04000ABE RID: 2750
		public const string m_appKey = "1uGA5aADX43p";

		// Token: 0x04000ABF RID: 2751
		public const string m_appSecret = "9aux67wg5z";

		// Token: 0x04000AC0 RID: 2752
		public const string m_redirectUri = "https://m.schub.top";

		// Token: 0x04000AC1 RID: 2753
		public SPMBoxExternalContentProvider.LoginProcessData m_loginProcessData;

		// Token: 0x02000466 RID: 1126
		public class LoginProcessData
		{
			// Token: 0x06001F05 RID: 7941 RVA: 0x000E0B8A File Offset: 0x000DED8A
			public void Succeed(SPMBoxExternalContentProvider provider)
			{
				provider.m_loginProcessData = null;
				Action success = this.Success;
				if (success == null)
				{
					return;
				}
				success();
			}

			// Token: 0x06001F06 RID: 7942 RVA: 0x000E0BA3 File Offset: 0x000DEDA3
			public void Fail(SPMBoxExternalContentProvider provider, Exception error)
			{
				provider.m_loginProcessData = null;
				Action<Exception> failure = this.Failure;
				if (failure == null)
				{
					return;
				}
				failure(error);
			}

			// Token: 0x0400166C RID: 5740
			public bool IsTokenFlow;

			// Token: 0x0400166D RID: 5741
			public Action Success;

			// Token: 0x0400166E RID: 5742
			public Action<Exception> Failure;

			// Token: 0x0400166F RID: 5743
			public CancellableProgress Progress;
		}
	}
}
