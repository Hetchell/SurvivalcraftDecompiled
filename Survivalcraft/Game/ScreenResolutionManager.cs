using System;
using Engine;

namespace Game
{
	// Token: 0x02000149 RID: 329
	public static class ScreenResolutionManager
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600062B RID: 1579 RVA: 0x00025204 File Offset: 0x00023404
		// (set) Token: 0x0600062C RID: 1580 RVA: 0x0002520B File Offset: 0x0002340B
		public static float ApproximateScreenDpi { get; set; } = 0.5f * (float)(Window.ScreenSize.X / Window.Size.X + Window.ScreenSize.Y / Window.Size.Y);

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600062D RID: 1581 RVA: 0x00025213 File Offset: 0x00023413
		public static float ApproximateScreenInches
		{
			get
			{
				return MathUtils.Sqrt((float)(Window.ScreenSize.X * Window.ScreenSize.X + Window.ScreenSize.Y * Window.ScreenSize.Y)) / ScreenResolutionManager.ApproximateScreenDpi;
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0002524C File Offset: 0x0002344C
		static ScreenResolutionManager()
		{
			ScreenResolutionManager.ApproximateScreenDpi = MathUtils.Clamp(ScreenResolutionManager.ApproximateScreenDpi, 96f, 800f);
		}
	}
}
