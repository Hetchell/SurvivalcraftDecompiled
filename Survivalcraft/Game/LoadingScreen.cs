using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Engine;
using Engine.Content;

namespace Game
{
	// Token: 0x0200013B RID: 315
	public class LoadingScreen : Screen
	{
		// Token: 0x060005DE RID: 1502 RVA: 0x00021380 File Offset: 0x0001F580
		public LoadingScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/LoadingScreen");
			base.LoadContents(this, node);
			this.AddLoadAction(delegate
			{
				VrManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				CommunityContentManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				MotdManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				LightingManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				StringsManager.LoadStrings();
			});
			this.AddLoadAction(delegate
			{
				TextureAtlasManager.LoadAtlases();
			});
			foreach (ContentInfo localContentInfo2 in ContentManager.List())
			{
				ContentInfo localContentInfo = localContentInfo2;
				this.AddLoadAction(delegate
				{
					ContentManager.Get(localContentInfo.Name);
				});
			}
			this.AddLoadAction(delegate
			{
				DatabaseManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				WorldsManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				BlocksTexturesManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				CharacterSkinsManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				FurniturePacksManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				BlocksManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				CraftingRecipesManager.Initialize();
			});
			this.AddLoadAction(delegate
			{
				MusicManager.CurrentMix = MusicManager.Mix.Menu;
			});
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x0002161C File Offset: 0x0001F81C
		public void AddLoadAction(Action action)
		{
			this.m_loadActions.Add(action);
		}

		// Token: 0x060005E0 RID: 1504 RVA: 0x0002162A File Offset: 0x0001F82A
		public override void Leave()
		{
			ContentManager.Dispose("Textures/Gui/CandyRufusLogo");
			ContentManager.Dispose("Textures/Gui/EngineLogo");
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x00021640 File Offset: 0x0001F840
		public override void Update()
		{
			if (!this.m_loadingStarted)
			{
				this.m_loadingStarted = true;
				return;
			}
			if (this.m_loadingFinished)
			{
				return;
			}
			double realTime = Time.RealTime;
			while (!this.m_pauseLoading && this.m_index < this.m_loadActions.Count)
			{
				try
				{
					List<Action> loadActions = this.m_loadActions;
					int index = this.m_index;
					this.m_index = index + 1;
					loadActions[index]();
				}
				catch (Exception ex)
				{
					Log.Error("Loading error. Reason: " + ex.Message);
					if (!this.m_loadingErrorsSuppressed)
					{
						this.m_pauseLoading = true;
						DialogsManager.ShowDialog(ScreensManager.RootWidget, new MessageDialog("Loading Error", ExceptionManager.MakeFullErrorMessage(ex), "确定", "Suppress", delegate(MessageDialogButton b)
						{
							if (b == MessageDialogButton.Button1)
							{
								this.m_pauseLoading = false;
								return;
							}
							if (b != MessageDialogButton.Button2)
							{
								return;
							}
							this.m_loadingErrorsSuppressed = true;
						}));
					}
				}
				if (Time.RealTime - realTime > 0.1)
				{
					break;
				}
			}
			if (this.m_index >= this.m_loadActions.Count)
			{
				this.m_loadingFinished = true;
				AudioManager.PlaySound("Audio/UI/ButtonClick", 1f, 0f, 0f);
				ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
			}
		}

		// Token: 0x040002B8 RID: 696
		public List<Action> m_loadActions = new List<Action>();

		// Token: 0x040002B9 RID: 697
		public int m_index;

		// Token: 0x040002BA RID: 698
		public bool m_loadingStarted;

		// Token: 0x040002BB RID: 699
		public bool m_loadingFinished;

		// Token: 0x040002BC RID: 700
		public bool m_pauseLoading;

		// Token: 0x040002BD RID: 701
		public bool m_loadingErrorsSuppressed;
	}
}
