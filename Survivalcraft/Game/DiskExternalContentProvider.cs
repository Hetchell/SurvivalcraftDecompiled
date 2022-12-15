using System;
using System.IO;
using System.Windows.Forms;

namespace Game
{
	// Token: 0x02000252 RID: 594
	public class DiskExternalContentProvider : IExternalContentProvider, IDisposable
	{
		// Token: 0x17000292 RID: 658
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x0008A61C File Offset: 0x0008881C
		public string DisplayName
		{
			get
			{
				return "disk";
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x0008A623 File Offset: 0x00088823
		public bool SupportsLinks
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x060011EA RID: 4586 RVA: 0x0008A626 File Offset: 0x00088826
		public bool SupportsListing
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x0008A629 File Offset: 0x00088829
		public bool RequiresLogin
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x060011EC RID: 4588 RVA: 0x0008A62C File Offset: 0x0008882C
		public bool IsLoggedIn
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x060011ED RID: 4589 RVA: 0x0008A62F File Offset: 0x0008882F
		public string Description
		{
			get
			{
				return "No login required; Save to disk";
			}
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x0008A636 File Offset: 0x00088836
		public void Dispose()
		{
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x0008A638 File Offset: 0x00088838
		public void Download(string path, CancellableProgress progress, Action<Stream> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x0008A646 File Offset: 0x00088846
		public void Link(string path, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x060011F1 RID: 4593 RVA: 0x0008A654 File Offset: 0x00088854
		public void List(string path, CancellableProgress progress, Action<ExternalContentEntry> success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x060011F2 RID: 4594 RVA: 0x0008A662 File Offset: 0x00088862
		public void Login(CancellableProgress progress, Action success, Action<Exception> failure)
		{
			failure(new NotSupportedException());
		}

		// Token: 0x060011F3 RID: 4595 RVA: 0x0008A66F File Offset: 0x0008886F
		public void Logout()
		{
			throw new NotSupportedException();
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0008A678 File Offset: 0x00088878
		public void Upload(string path, Stream stream, CancellableProgress progress, Action<string> success, Action<Exception> failure)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Survivalcraft World|*.scworld";
			saveFileDialog.Title = "Save an Survivalcraft World File";
			saveFileDialog.ShowDialog();
			try
			{
				using (Stream stream2 = saveFileDialog.OpenFile())
				{
					stream.CopyTo(stream2);
				}
				success(null);
			}
			catch (Exception obj)
			{
				failure(obj);
			}
		}
	}
}
