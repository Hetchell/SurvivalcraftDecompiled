using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Game.Properties
{
	// Token: 0x020003AA RID: 938
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06001C44 RID: 7236 RVA: 0x000D9D49 File Offset: 0x000D7F49
		internal Resources()
		{
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06001C45 RID: 7237 RVA: 0x000D9D51 File Offset: 0x000D7F51
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Game.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06001C46 RID: 7238 RVA: 0x000D9D7D File Offset: 0x000D7F7D
		// (set) Token: 0x06001C47 RID: 7239 RVA: 0x000D9D84 File Offset: 0x000D7F84
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x04001389 RID: 5001
		public static ResourceManager resourceMan;

		// Token: 0x0400138A RID: 5002
		public static CultureInfo resourceCulture;
	}
}
