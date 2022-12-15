using System;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000160 RID: 352
	public class SubsystemBlocksTexture : Subsystem
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x0002BB51 File Offset: 0x00029D51
		// (set) Token: 0x060006E5 RID: 1765 RVA: 0x0002BB59 File Offset: 0x00029D59
		public Texture2D BlocksTexture { get; set; }

        // Token: 0x060006E6 RID: 1766 RVA: 0x0002BB62 File Offset: 0x00029D62
        public override void Load(ValuesDictionary valuesDictionary)
		{
			Display.DeviceReset += this.Display_DeviceReset;
			this.LoadBlocksTexture();
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0002BB7B File Offset: 0x00029D7B
		public override void Dispose()
		{
			Display.DeviceReset -= this.Display_DeviceReset;
			this.DisposeBlocksTexture();
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x0002BB94 File Offset: 0x00029D94
		public void LoadBlocksTexture()
		{
			SubsystemGameInfo subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
			this.BlocksTexture = BlocksTexturesManager.LoadTexture(subsystemGameInfo.WorldSettings.BlocksTextureName);
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x0002BBC4 File Offset: 0x00029DC4
		public void DisposeBlocksTexture()
		{
			if (this.BlocksTexture != null && !ContentManager.IsContent(this.BlocksTexture))
			{
				this.BlocksTexture.Dispose();
				this.BlocksTexture = null;
			}
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0002BBED File Offset: 0x00029DED
		public void Display_DeviceReset()
		{
			this.LoadBlocksTexture();
		}
	}
}
