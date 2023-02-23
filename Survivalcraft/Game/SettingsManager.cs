using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x020002EE RID: 750
	public static class SettingsManager
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060014F1 RID: 5361 RVA: 0x000A25DC File Offset: 0x000A07DC
		// (set) Token: 0x060014F2 RID: 5362 RVA: 0x000A25E3 File Offset: 0x000A07E3
		public static float SoundsVolume
		{
			get
			{
				return SettingsManager.m_soundsVolume;
			}
			set
			{
				SettingsManager.m_soundsVolume = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060014F3 RID: 5363 RVA: 0x000A25F0 File Offset: 0x000A07F0
		// (set) Token: 0x060014F4 RID: 5364 RVA: 0x000A25F7 File Offset: 0x000A07F7
		public static float MusicVolume
		{
			get
			{
				return SettingsManager.m_musicVolume;
			}
			set
			{
				SettingsManager.m_musicVolume = MathUtils.Saturate(value);
			}
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x060014F5 RID: 5365 RVA: 0x000A2604 File Offset: 0x000A0804
		// (set) Token: 0x060014F6 RID: 5366 RVA: 0x000A260B File Offset: 0x000A080B
		public static int VisibilityRange { get; set; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x060014F7 RID: 5367 RVA: 0x000A2613 File Offset: 0x000A0813
		// (set) Token: 0x060014F8 RID: 5368 RVA: 0x000A261A File Offset: 0x000A081A
		public static bool UseVr { get; set; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x060014F9 RID: 5369 RVA: 0x000A2622 File Offset: 0x000A0822
		// (set) Token: 0x060014FA RID: 5370 RVA: 0x000A2629 File Offset: 0x000A0829
		public static ResolutionMode ResolutionMode
		{
			get
			{
				return SettingsManager.m_resolutionMode;
			}
			set
			{
				if (value != SettingsManager.m_resolutionMode)
				{
					SettingsManager.m_resolutionMode = value;
					if (SettingsManager.SettingChanged != null)
					{
						SettingsManager.SettingChanged("ResolutionMode");
					}
				}
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x060014FB RID: 5371 RVA: 0x000A264F File Offset: 0x000A084F
		// (set) Token: 0x060014FC RID: 5372 RVA: 0x000A2656 File Offset: 0x000A0856
		public static ViewAngleMode ViewAngleMode { get; set; }

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x060014FD RID: 5373 RVA: 0x000A265E File Offset: 0x000A085E
		// (set) Token: 0x060014FE RID: 5374 RVA: 0x000A2665 File Offset: 0x000A0865
		public static SkyRenderingMode SkyRenderingMode { get; set; }

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x060014FF RID: 5375 RVA: 0x000A266D File Offset: 0x000A086D
		// (set) Token: 0x06001500 RID: 5376 RVA: 0x000A2674 File Offset: 0x000A0874
		public static bool TerrainMipmapsEnabled { get; set; }

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x000A267C File Offset: 0x000A087C
		// (set) Token: 0x06001502 RID: 5378 RVA: 0x000A2683 File Offset: 0x000A0883
		public static bool ObjectsShadowsEnabled { get; set; }

		// Token: 0x17000329 RID: 809
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x000A268B File Offset: 0x000A088B
		// (set) Token: 0x06001504 RID: 5380 RVA: 0x000A2692 File Offset: 0x000A0892
		public static float Brightness
		{
			get
			{
				return SettingsManager.m_brightness;
			}
			set
			{
				value = MathUtils.Clamp(value, 0f, 1f);
				if (value != SettingsManager.m_brightness)
				{
					SettingsManager.m_brightness = value;
					Action<string> settingChanged = SettingsManager.SettingChanged;
					if (settingChanged == null)
					{
						return;
					}
					settingChanged("Brightness");
				}
			}
		}

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x000A26C8 File Offset: 0x000A08C8
		// (set) Token: 0x06001506 RID: 5382 RVA: 0x000A26CF File Offset: 0x000A08CF
		public static int PresentationInterval { get; set; }

		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x000A26D7 File Offset: 0x000A08D7
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x000A26DE File Offset: 0x000A08DE
		public static bool ShowGuiInScreenshots { get; set; }

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x000A26E6 File Offset: 0x000A08E6
		// (set) Token: 0x0600150A RID: 5386 RVA: 0x000A26ED File Offset: 0x000A08ED
		public static bool ShowLogoInScreenshots { get; set; }

		// Token: 0x1700032D RID: 813
		// (get) Token: 0x0600150B RID: 5387 RVA: 0x000A26F5 File Offset: 0x000A08F5
		// (set) Token: 0x0600150C RID: 5388 RVA: 0x000A26FC File Offset: 0x000A08FC
		public static ScreenshotSize ScreenshotSize { get; set; }

		// Token: 0x1700032E RID: 814
		// (get) Token: 0x0600150D RID: 5389 RVA: 0x000A2704 File Offset: 0x000A0904
		// (set) Token: 0x0600150E RID: 5390 RVA: 0x000A270C File Offset: 0x000A090C
		public static WindowMode WindowMode
		{
			get
			{
				return SettingsManager.m_windowMode;
			}
			set
			{
				if (value != SettingsManager.m_windowMode)
				{
					if (value == WindowMode.Borderless)
					{
						SettingsManager.m_resizableWindowSize = Window.Size;
						Window.Position = Point2.Zero;
						Window.Size = Window.ScreenSize;
					}
					else if (value == WindowMode.Resizable)
					{
						Window.Position = SettingsManager.m_resizableWindowPosition;
						Window.Size = SettingsManager.m_resizableWindowSize;
					}
					Window.WindowMode = Converter(value);
					SettingsManager.m_windowMode = value;
				}
			}
		}

		private static Engine.WindowMode Converter(Game.WindowMode windowmode)
		{
			switch (windowmode)
			{
				case WindowMode.Resizable:
					return Engine.WindowMode.Resizable;
				case WindowMode.Borderless:
					return Engine.WindowMode.Fullscreen;
				default:
					return Engine.WindowMode.Resizable;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x0600150F RID: 5391 RVA: 0x000A2768 File Offset: 0x000A0968
		// (set) Token: 0x06001510 RID: 5392 RVA: 0x000A276F File Offset: 0x000A096F
		public static GuiSize GuiSize { get; set; }

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x06001511 RID: 5393 RVA: 0x000A2777 File Offset: 0x000A0977
		// (set) Token: 0x06001512 RID: 5394 RVA: 0x000A277E File Offset: 0x000A097E
		public static bool HideMoveLookPads { get; set; }

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x06001513 RID: 5395 RVA: 0x000A2786 File Offset: 0x000A0986
		// (set) Token: 0x06001514 RID: 5396 RVA: 0x000A278D File Offset: 0x000A098D
		public static string BlocksTextureFileName { get; set; }

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x06001515 RID: 5397 RVA: 0x000A2795 File Offset: 0x000A0995
		// (set) Token: 0x06001516 RID: 5398 RVA: 0x000A279C File Offset: 0x000A099C
		public static MoveControlMode MoveControlMode { get; set; }

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x06001517 RID: 5399 RVA: 0x000A27A4 File Offset: 0x000A09A4
		// (set) Token: 0x06001518 RID: 5400 RVA: 0x000A27AB File Offset: 0x000A09AB
		public static LookControlMode LookControlMode { get; set; }

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x06001519 RID: 5401 RVA: 0x000A27B3 File Offset: 0x000A09B3
		// (set) Token: 0x0600151A RID: 5402 RVA: 0x000A27BA File Offset: 0x000A09BA
		public static bool LeftHandedLayout { get; set; }

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x0600151B RID: 5403 RVA: 0x000A27C2 File Offset: 0x000A09C2
		// (set) Token: 0x0600151C RID: 5404 RVA: 0x000A27C9 File Offset: 0x000A09C9
		public static bool FlipVerticalAxis { get; set; }

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600151D RID: 5405 RVA: 0x000A27D1 File Offset: 0x000A09D1
		// (set) Token: 0x0600151E RID: 5406 RVA: 0x000A27D8 File Offset: 0x000A09D8
		public static float MoveSensitivity { get; set; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x000A27E0 File Offset: 0x000A09E0
		// (set) Token: 0x06001520 RID: 5408 RVA: 0x000A27E7 File Offset: 0x000A09E7
		public static float LookSensitivity { get; set; }

		// Token: 0x17000338 RID: 824
		// (get) Token: 0x06001521 RID: 5409 RVA: 0x000A27EF File Offset: 0x000A09EF
		// (set) Token: 0x06001522 RID: 5410 RVA: 0x000A27F6 File Offset: 0x000A09F6
		public static float GamepadDeadZone { get; set; }

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x06001523 RID: 5411 RVA: 0x000A27FE File Offset: 0x000A09FE
		// (set) Token: 0x06001524 RID: 5412 RVA: 0x000A2805 File Offset: 0x000A0A05
		public static float GamepadCursorSpeed { get; set; }

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06001525 RID: 5413 RVA: 0x000A280D File Offset: 0x000A0A0D
		// (set) Token: 0x06001526 RID: 5414 RVA: 0x000A2814 File Offset: 0x000A0A14
		public static float CreativeDigTime { get; set; }

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06001527 RID: 5415 RVA: 0x000A281C File Offset: 0x000A0A1C
		// (set) Token: 0x06001528 RID: 5416 RVA: 0x000A2823 File Offset: 0x000A0A23
		public static float CreativeReach { get; set; }

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001529 RID: 5417 RVA: 0x000A282B File Offset: 0x000A0A2B
		// (set) Token: 0x0600152A RID: 5418 RVA: 0x000A2832 File Offset: 0x000A0A32
		public static float MinimumHoldDuration { get; set; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x0600152B RID: 5419 RVA: 0x000A283A File Offset: 0x000A0A3A
		// (set) Token: 0x0600152C RID: 5420 RVA: 0x000A2841 File Offset: 0x000A0A41
		public static float MinimumDragDistance { get; set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x0600152D RID: 5421 RVA: 0x000A2849 File Offset: 0x000A0A49
		// (set) Token: 0x0600152E RID: 5422 RVA: 0x000A2850 File Offset: 0x000A0A50
		public static bool AutoJump { get; set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x0600152F RID: 5423 RVA: 0x000A2858 File Offset: 0x000A0A58
		// (set) Token: 0x06001530 RID: 5424 RVA: 0x000A285F File Offset: 0x000A0A5F
		public static bool HorizontalCreativeFlight { get; set; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x000A2867 File Offset: 0x000A0A67
		// (set) Token: 0x06001532 RID: 5426 RVA: 0x000A286E File Offset: 0x000A0A6E
		public static string DropboxAccessToken { get; set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001533 RID: 5427 RVA: 0x000A2876 File Offset: 0x000A0A76
		// (set) Token: 0x06001534 RID: 5428 RVA: 0x000A287D File Offset: 0x000A0A7D
		public static string MotdUpdateUrl { get; set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001535 RID: 5429 RVA: 0x000A2885 File Offset: 0x000A0A85
		// (set) Token: 0x06001536 RID: 5430 RVA: 0x000A288C File Offset: 0x000A0A8C
		public static string MotdBackupUpdateUrl { get; set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001537 RID: 5431 RVA: 0x000A2894 File Offset: 0x000A0A94
		// (set) Token: 0x06001538 RID: 5432 RVA: 0x000A289B File Offset: 0x000A0A9B
		public static string ScpboxAccessToken { get; set; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001539 RID: 5433 RVA: 0x000A28A3 File Offset: 0x000A0AA3
		// (set) Token: 0x0600153A RID: 5434 RVA: 0x000A28AA File Offset: 0x000A0AAA
		public static bool MotdUseBackupUrl { get; set; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600153B RID: 5435 RVA: 0x000A28B2 File Offset: 0x000A0AB2
		// (set) Token: 0x0600153C RID: 5436 RVA: 0x000A28B9 File Offset: 0x000A0AB9
		public static double MotdUpdatePeriodHours { get; set; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600153D RID: 5437 RVA: 0x000A28C1 File Offset: 0x000A0AC1
		// (set) Token: 0x0600153E RID: 5438 RVA: 0x000A28C8 File Offset: 0x000A0AC8
		public static DateTime MotdLastUpdateTime { get; set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600153F RID: 5439 RVA: 0x000A28D0 File Offset: 0x000A0AD0
		// (set) Token: 0x06001540 RID: 5440 RVA: 0x000A28D7 File Offset: 0x000A0AD7
		public static string MotdLastDownloadedData { get; set; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x000A28DF File Offset: 0x000A0ADF
		// (set) Token: 0x06001542 RID: 5442 RVA: 0x000A28E6 File Offset: 0x000A0AE6
		public static string UserId { get; set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x000A28EE File Offset: 0x000A0AEE
		// (set) Token: 0x06001544 RID: 5444 RVA: 0x000A28F5 File Offset: 0x000A0AF5
		public static string LastLaunchedVersion { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x000A28FD File Offset: 0x000A0AFD
		// (set) Token: 0x06001546 RID: 5446 RVA: 0x000A2904 File Offset: 0x000A0B04
		public static CommunityContentMode CommunityContentMode { get; set; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x06001547 RID: 5447 RVA: 0x000A290C File Offset: 0x000A0B0C
		// (set) Token: 0x06001548 RID: 5448 RVA: 0x000A2913 File Offset: 0x000A0B13
		public static bool MultithreadedTerrainUpdate { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x06001549 RID: 5449 RVA: 0x000A291B File Offset: 0x000A0B1B
		// (set) Token: 0x0600154A RID: 5450 RVA: 0x000A291E File Offset: 0x000A0B1E
		public static bool EnableAndroidAudioTrackCaching
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x0600154B RID: 5451 RVA: 0x000A2920 File Offset: 0x000A0B20
		// (set) Token: 0x0600154C RID: 5452 RVA: 0x000A2923 File Offset: 0x000A0B23
		public static bool UseReducedZRange
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x0600154D RID: 5453 RVA: 0x000A2925 File Offset: 0x000A0B25
		// (set) Token: 0x0600154E RID: 5454 RVA: 0x000A292C File Offset: 0x000A0B2C
		public static int IsolatedStorageMigrationCounter { get; set; }

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x0600154F RID: 5455 RVA: 0x000A2934 File Offset: 0x000A0B34
		// (set) Token: 0x06001550 RID: 5456 RVA: 0x000A293B File Offset: 0x000A0B3B
		public static bool DisplayFpsCounter { get; set; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x000A2943 File Offset: 0x000A0B43
		// (set) Token: 0x06001552 RID: 5458 RVA: 0x000A294A File Offset: 0x000A0B4A
		public static bool DisplayFpsRibbon { get; set; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06001553 RID: 5459 RVA: 0x000A2952 File Offset: 0x000A0B52
		// (set) Token: 0x06001554 RID: 5460 RVA: 0x000A2959 File Offset: 0x000A0B59
		public static int NewYearCelebrationLastYear { get; set; }

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x06001555 RID: 5461 RVA: 0x000A2961 File Offset: 0x000A0B61
		// (set) Token: 0x06001556 RID: 5462 RVA: 0x000A2968 File Offset: 0x000A0B68
		public static ScreenLayout ScreenLayout1 { get; set; }

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x06001557 RID: 5463 RVA: 0x000A2970 File Offset: 0x000A0B70
		// (set) Token: 0x06001558 RID: 5464 RVA: 0x000A2977 File Offset: 0x000A0B77
		public static ScreenLayout ScreenLayout2 { get; set; }

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x06001559 RID: 5465 RVA: 0x000A297F File Offset: 0x000A0B7F
		// (set) Token: 0x0600155A RID: 5466 RVA: 0x000A2986 File Offset: 0x000A0B86
		public static ScreenLayout ScreenLayout3 { get; set; }

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x0600155B RID: 5467 RVA: 0x000A298E File Offset: 0x000A0B8E
		// (set) Token: 0x0600155C RID: 5468 RVA: 0x000A2995 File Offset: 0x000A0B95
		public static ScreenLayout ScreenLayout4 { get; set; }

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x0600155D RID: 5469 RVA: 0x000A299D File Offset: 0x000A0B9D
		// (set) Token: 0x0600155E RID: 5470 RVA: 0x000A29A4 File Offset: 0x000A0BA4
		public static bool UpsideDownLayout { get; set; }

		// Token: 0x17000357 RID: 855
		// (get) Token: 0x0600155F RID: 5471 RVA: 0x000A29AC File Offset: 0x000A0BAC
		// (set) Token: 0x06001560 RID: 5472 RVA: 0x000A29B6 File Offset: 0x000A0BB6
		public static bool FullScreenMode
		{
			get
			{
				return Window.WindowMode == Engine.WindowMode.Fullscreen;
			}
			set
			{
				Window.WindowMode = (value ? Engine.WindowMode.Fullscreen : Engine.WindowMode.Resizable);
			}
		}

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001561 RID: 5473 RVA: 0x000A29C4 File Offset: 0x000A0BC4
		// (remove) Token: 0x06001562 RID: 5474 RVA: 0x000A29F8 File Offset: 0x000A0BF8
		public static event Action<string> SettingChanged;

		// Token: 0x06001563 RID: 5475 RVA: 0x000A2A2C File Offset: 0x000A0C2C
		public static void Initialize()
		{
			SettingsManager.VisibilityRange = 128;
			SettingsManager.m_resolutionMode = ResolutionMode.High;
			SettingsManager.ViewAngleMode = ViewAngleMode.Normal;
			SettingsManager.SkyRenderingMode = SkyRenderingMode.Full;
			SettingsManager.TerrainMipmapsEnabled = false;
			SettingsManager.ObjectsShadowsEnabled = true;
			SettingsManager.m_soundsVolume = 0.5f;
			SettingsManager.m_musicVolume = 0.5f;
			SettingsManager.m_brightness = 0.5f;
			SettingsManager.PresentationInterval = 1;
			SettingsManager.ShowGuiInScreenshots = false;
			SettingsManager.ShowLogoInScreenshots = true;
			SettingsManager.ScreenshotSize = ScreenshotSize.ScreenSize;
			SettingsManager.MoveControlMode = MoveControlMode.Pad;
			SettingsManager.HideMoveLookPads = false;
			SettingsManager.BlocksTextureFileName = string.Empty;
			SettingsManager.LookControlMode = LookControlMode.EntireScreen;
			SettingsManager.FlipVerticalAxis = false;
			SettingsManager.MoveSensitivity = 0.5f;
			SettingsManager.LookSensitivity = 0.5f;
			SettingsManager.GamepadDeadZone = 0.16f;
			SettingsManager.GamepadCursorSpeed = 1f;
			SettingsManager.CreativeDigTime = 0.2f;
			SettingsManager.CreativeReach = 7.5f;
			SettingsManager.MinimumHoldDuration = 0.5f;
			SettingsManager.MinimumDragDistance = 10f;
			SettingsManager.AutoJump = true;
			SettingsManager.HorizontalCreativeFlight = false;
			SettingsManager.DropboxAccessToken = string.Empty;
			SettingsManager.ScpboxAccessToken = string.Empty;
			SettingsManager.MotdUpdateUrl = "https://m.schub.top/com/motd?v={0}&l={1}";
			SettingsManager.MotdBackupUpdateUrl = "https://m.schub.top/com/motd?v={0}&l={1}";
			SettingsManager.MotdUpdatePeriodHours = 12.0;
			SettingsManager.MotdLastUpdateTime = DateTime.MinValue;
			SettingsManager.MotdLastDownloadedData = string.Empty;
			SettingsManager.UserId = string.Empty;
			SettingsManager.LastLaunchedVersion = string.Empty;
			SettingsManager.CommunityContentMode = CommunityContentMode.Normal;
			SettingsManager.MultithreadedTerrainUpdate = true;
			SettingsManager.NewYearCelebrationLastYear = 2015;
			SettingsManager.ScreenLayout1 = ScreenLayout.Single;
			SettingsManager.ScreenLayout2 = (((float)Window.ScreenSize.X / (float)Window.ScreenSize.Y > 1.3333334f) ? ScreenLayout.DoubleVertical : ScreenLayout.DoubleHorizontal);
			SettingsManager.ScreenLayout3 = (((float)Window.ScreenSize.X / (float)Window.ScreenSize.Y > 1.3333334f) ? ScreenLayout.TripleVertical : ScreenLayout.TripleHorizontal);
			SettingsManager.ScreenLayout4 = ScreenLayout.Quadruple;
			SettingsManager.GuiSize = GuiSize.Smallest;
			SettingsManager.HorizontalCreativeFlight = true;
			SettingsManager.TerrainMipmapsEnabled = true;
			SettingsManager.LoadSettings();
			VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "1.29");
			if (VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "2.1") < 0)
			{
				SettingsManager.MinimumDragDistance = 10f;
			}
			if (VersionsManager.CompareVersions(SettingsManager.LastLaunchedVersion, "2.2") < 0)
			{
				if (Utilities.GetTotalAvailableMemory() < 524288000)
				{
					SettingsManager.VisibilityRange = MathUtils.Min(64, SettingsManager.VisibilityRange);
				}
				else if (Utilities.GetTotalAvailableMemory() < 1048576000)
				{
					SettingsManager.VisibilityRange = MathUtils.Min(112, SettingsManager.VisibilityRange);
				}
			}
			Window.Deactivated += delegate()
			{
				SettingsManager.SaveSettings();
			};
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000A2C98 File Offset: 0x000A0E98
		public static void LoadSettings()
		{
			try
			{
				if (Storage.FileExists("app:/Settings.xml"))
				{
					using (Stream stream = Storage.OpenFile(m_settingsFileName, OpenFileMode.Read))
					{
						foreach (XElement node in XmlUtils.LoadXmlFromStream(stream, null, true).Elements())
						{ 
							string name = "<unknown>";
							try
							{
								name = XmlUtils.GetAttributeValue<string>(node, "Name");
								Console.WriteLine(name);
								string attributeValue = XmlUtils.GetAttributeValue<string>(node, "Value");
								PropertyInfo propertyInfo = (from pi in typeof(SettingsManager).GetRuntimeProperties()
								where pi.Name == name && pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
								select pi).FirstOrDefault<PropertyInfo>();
								if (propertyInfo != null && !name.Contains("MotdLastDownloadedData"))
								{
									object value = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attributeValue);
									propertyInfo.SetValue(null, value, null);
								}
							}
							catch (Exception ex)
							{
								Log.Warning(string.Format("Setting \"{0}\" could not be loaded. Reason: {1}", new object[]
								{
									name,
									ex.Message
								}));
							}
						}
					}
					Log.Information("Loaded settings.");
				}
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Loading settings failed.", e);
			}
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x000A2E28 File Offset: 0x000A1028
		public static void SaveSettings()
		{
			try
			{
				XElement xelement = new XElement("Settings");
				foreach (PropertyInfo propertyInfo in from pi in typeof(SettingsManager).GetRuntimeProperties()
				where pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
				select pi)
				{
					try
					{
						string value = HumanReadableConverter.ConvertToString(propertyInfo.GetValue(null, null));
						XElement node = XmlUtils.AddElement(xelement, "Setting");
						XmlUtils.SetAttributeValue(node, "Name", propertyInfo.Name);
						XmlUtils.SetAttributeValue(node, "Value", value);
					}
					catch (Exception ex)
					{
						Log.Warning(string.Format("Setting \"{0}\" could not be saved. Reason: {1}", new object[]
						{
							propertyInfo.Name,
							ex.Message
						}));
					}
				}
				using (Stream stream = Storage.OpenFile(m_settingsFileName, OpenFileMode.Create))
				{
					XmlUtils.SaveXmlToStream(xelement, stream, null, true);
				}
				Log.Information("Saved settings");
			}
			catch (Exception e)
			{
				ExceptionManager.ReportExceptionToUser("Saving settings failed.", e);
			}
		}

		// Token: 0x04000EF1 RID: 3825
		private const string m_settingsFileName = "app:/Settings.xml";

		// Token: 0x04000EF2 RID: 3826
		public static float m_soundsVolume;

		// Token: 0x04000EF3 RID: 3827
		public static float m_musicVolume;

		// Token: 0x04000EF4 RID: 3828
		public static float m_brightness;

		// Token: 0x04000EF5 RID: 3829
		public static ResolutionMode m_resolutionMode;

		// Token: 0x04000EF6 RID: 3830
		public static WindowMode m_windowMode;

		// Token: 0x04000EF7 RID: 3831
		public static Point2 m_resizableWindowPosition;

		// Token: 0x04000EF8 RID: 3832
		public static Point2 m_resizableWindowSize;
	}
}
