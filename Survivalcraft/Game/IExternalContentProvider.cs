using System;
using System.IO;

namespace Game
{
	// Token: 0x02000294 RID: 660
	public interface IExternalContentProvider : IDisposable
	{
		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x0600133E RID: 4926
		string DisplayName { get; }

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x0600133F RID: 4927
		bool SupportsLinks { get; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06001340 RID: 4928
		bool SupportsListing { get; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06001341 RID: 4929
		bool RequiresLogin { get; }

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06001342 RID: 4930
		bool IsLoggedIn { get; }

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06001343 RID: 4931
		string Description { get; }

		// Token: 0x06001344 RID: 4932
		void Login(CancellableProgress progress, Action success, Action<Exception> failure);

		// Token: 0x06001345 RID: 4933
		void Logout();

		// Token: 0x06001346 RID: 4934
		void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure);

		// Token: 0x06001347 RID: 4935
		void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure);

		// Token: 0x06001348 RID: 4936
		void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure);

		// Token: 0x06001349 RID: 4937
		void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure);
	}
}
