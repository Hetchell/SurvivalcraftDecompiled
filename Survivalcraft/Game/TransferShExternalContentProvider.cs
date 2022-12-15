using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Game
{
	// Token: 0x02000326 RID: 806
	public class TransferShExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x17000372 RID: 882
		// (get) Token: 0x0600170E RID: 5902 RVA: 0x000B8632 File Offset: 0x000B6832
		public string DisplayName
		{
			get
			{
				return "transfer.sh";
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x0600170F RID: 5903 RVA: 0x000B8639 File Offset: 0x000B6839
		public string Description
		{
			get
			{
				return "No login required";
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001710 RID: 5904 RVA: 0x000B8640 File Offset: 0x000B6840
		public bool SupportsListing
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001711 RID: 5905 RVA: 0x000B8643 File Offset: 0x000B6843
		public bool SupportsLinks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x000B8646 File Offset: 0x000B6846
		public bool RequiresLogin
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000377 RID: 887
		// (get) Token: 0x06001713 RID: 5907 RVA: 0x000B8649 File Offset: 0x000B6849
		public bool IsLoggedIn
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x000B864C File Offset: 0x000B684C
		public void Dispose()
		{
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x000B864E File Offset: 0x000B684E
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x000B865B File Offset: 0x000B685B
		public void Logout()
		{
			throw new NotSupportedException();
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x000B8662 File Offset: 0x000B6862
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x000B8670 File Offset: 0x000B6870
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x000B8680 File Offset: 0x000B6880
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Content-Type", "application/octet-stream");
				WebManager.Put("https://transfer.sh/" + path, null, dictionary, stream, progress, delegate(byte[] result)
				{
					string obj2 = Encoding.UTF8.GetString(result, 0, result.Length).Trim();
					success(obj2);
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

		// Token: 0x0600171A RID: 5914 RVA: 0x000B8708 File Offset: 0x000B6908
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}
	}
}
