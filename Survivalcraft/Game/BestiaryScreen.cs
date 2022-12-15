using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000133 RID: 307
	public class BestiaryScreen : Screen
	{
		// Token: 0x060005A5 RID: 1445 RVA: 0x0001F098 File Offset: 0x0001D298
		public BestiaryScreen()
		{
			XElement node = ContentManager.Get<XElement>("Screens/BestiaryScreen");
			base.LoadContents(this, node);
			this.m_creaturesList = this.Children.Find<ListPanelWidget>("CreaturesList", true);
			this.m_creaturesList.ItemWidgetFactory = delegate(object item)
			{
				BestiaryCreatureInfo bestiaryCreatureInfo2 = (BestiaryCreatureInfo)item;
				XElement node2 = ContentManager.Get<XElement>("Widgets/BestiaryItem");
				ContainerWidget containerWidget = (ContainerWidget)Widget.LoadWidget(this, node2, null);
				ModelWidget modelWidget = containerWidget.Children.Find<ModelWidget>("BestiaryItem.Model", true);
				BestiaryScreen.SetupBestiaryModelWidget(bestiaryCreatureInfo2, modelWidget, (this.m_creaturesList.Items.IndexOf(item) % 2 == 0) ? new Vector3(-1f, 0f, -1f) : new Vector3(1f, 0f, -1f), false, false);
				containerWidget.Children.Find<LabelWidget>("BestiaryItem.Text", true).Text = bestiaryCreatureInfo2.DisplayName;
				containerWidget.Children.Find<LabelWidget>("BestiaryItem.Details", true).Text = bestiaryCreatureInfo2.Description;
				return containerWidget;
			};
			this.m_creaturesList.ItemClicked += delegate(object item)
			{
				ScreensManager.SwitchScreen("BestiaryDescription", new object[]
				{
					item,
					this.m_creaturesList.Items.Cast<BestiaryCreatureInfo>().ToList<BestiaryCreatureInfo>()
				});
			};
			List<BestiaryCreatureInfo> list = new List<BestiaryCreatureInfo>();
			foreach (ValuesDictionary valuesDictionary in DatabaseManager.EntitiesValuesDictionaries)
			{
				ValuesDictionary valuesDictionary2 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentCreature));
				if (valuesDictionary2 != null)
				{
					string text = valuesDictionary2.GetValue<string>("DisplayName");
					if (text.StartsWith("[") && text.EndsWith("]"))
					{
						string[] array = text.Substring(1, text.Length - 2).Split(new string[]
						{
							":"
						}, StringSplitOptions.RemoveEmptyEntries);
						text = LanguageControl.GetDatabase("DisplayName", array[1]);
					}
					if (!string.IsNullOrEmpty(text))
					{
						int order = -1;
						ValuesDictionary value = valuesDictionary.GetValue<ValuesDictionary>("CreatureEggData", null);
						ValuesDictionary value2 = valuesDictionary.GetValue<ValuesDictionary>("Player", null);
						if (value != null || value2 != null)
						{
							if (value != null)
							{
								int value3 = value.GetValue<int>("EggTypeIndex");
								if (value3 < 0)
								{
									continue;
								}
								order = value3;
							}
							ValuesDictionary valuesDictionary3 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentCreatureModel));
							ValuesDictionary valuesDictionary4 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentBody));
							ValuesDictionary valuesDictionary5 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentHealth));
							ValuesDictionary valuesDictionary6 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentMiner));
							ValuesDictionary valuesDictionary7 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentLocomotion));
							ValuesDictionary valuesDictionary8 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentHerdBehavior));
							ValuesDictionary valuesDictionary9 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentMount));
							ValuesDictionary valuesDictionary10 = DatabaseManager.FindValuesDictionaryForComponent(valuesDictionary, typeof(ComponentLoot));
							string text2 = valuesDictionary2.GetValue<string>("Description");
							if (text2.StartsWith("[") && text2.EndsWith("]"))
							{
								string[] array2 = text2.Substring(1, text2.Length - 2).Split(new string[]
								{
									":"
								}, StringSplitOptions.RemoveEmptyEntries);
								text2 = LanguageControl.GetDatabase("Description", array2[1]);
							}
							BestiaryCreatureInfo bestiaryCreatureInfo = new BestiaryCreatureInfo
							{
								Order = order,
								DisplayName = text,
								Description = text2,
								ModelName = valuesDictionary3.GetValue<string>("ModelName"),
								TextureOverride = valuesDictionary3.GetValue<string>("TextureOverride"),
								Mass = valuesDictionary4.GetValue<float>("Mass"),
								AttackResilience = valuesDictionary5.GetValue<float>("AttackResilience"),
								AttackPower = ((valuesDictionary6 != null) ? valuesDictionary6.GetValue<float>("AttackPower") : 0f),
								MovementSpeed = MathUtils.Max(valuesDictionary7.GetValue<float>("WalkSpeed"), valuesDictionary7.GetValue<float>("FlySpeed"), valuesDictionary7.GetValue<float>("SwimSpeed")),
								JumpHeight = MathUtils.Sqr(valuesDictionary7.GetValue<float>("JumpSpeed")) / 20f,
								IsHerding = (valuesDictionary8 != null),
								CanBeRidden = (valuesDictionary9 != null),
								HasSpawnerEgg = (value != null && value.GetValue<bool>("ShowEgg")),
								Loot = ((valuesDictionary10 != null) ? ComponentLoot.ParseLootList(valuesDictionary10.GetValue<ValuesDictionary>("Loot")) : new List<ComponentLoot.Loot>())
							};
							if (value2 != null && valuesDictionary.DatabaseObject.Name.ToLower().Contains("female"))
							{
								bestiaryCreatureInfo.AttackPower *= 0.8f;
								bestiaryCreatureInfo.AttackResilience *= 0.8f;
								bestiaryCreatureInfo.MovementSpeed *= 1.03f;
								bestiaryCreatureInfo.JumpHeight *= MathUtils.Sqr(1.03f);
							}
							list.Add(bestiaryCreatureInfo);
						}
					}
				}
			}
			foreach (BestiaryCreatureInfo item2 in from ci in list
			orderby ci.Order
			select ci)
			{
				this.m_creaturesList.AddItem(item2);
			}
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0001F520 File Offset: 0x0001D720
		public override void Enter(object[] parameters)
		{
			if (ScreensManager.PreviousScreen != ScreensManager.FindScreen<Screen>("BestiaryDescription"))
			{
				this.m_previousScreen = ScreensManager.PreviousScreen;
			}
			this.m_creaturesList.SelectedItem = null;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x0001F54C File Offset: 0x0001D74C
		public override void Update()
		{
			if (base.Input.Back || base.Input.Cancel || this.Children.Find<ButtonWidget>("TopBar.Back", true).IsClicked)
			{
				ScreensManager.SwitchScreen(this.m_previousScreen, Array.Empty<object>());
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x0001F59C File Offset: 0x0001D79C
		public static void SetupBestiaryModelWidget(BestiaryCreatureInfo info, ModelWidget modelWidget, Vector3 offset, bool autoRotate, bool autoAspect)
		{
			modelWidget.Model = ContentManager.Get<Model>(info.ModelName);
			modelWidget.TextureOverride = ContentManager.Get<Texture2D>(info.TextureOverride);
			Matrix[] absoluteTransforms = new Matrix[modelWidget.Model.Bones.Count];
			modelWidget.Model.CopyAbsoluteBoneTransformsTo(absoluteTransforms);
			BoundingBox boundingBox = modelWidget.Model.CalculateAbsoluteBoundingBox(absoluteTransforms);
			float x = MathUtils.Max(boundingBox.Size().X, 1.4f * boundingBox.Size().Y, boundingBox.Size().Z);
			modelWidget.ViewPosition = new Vector3(boundingBox.Center().X, 1.5f, boundingBox.Center().Z) + 2.6f * MathUtils.Pow(x, 0.75f) * offset;
			modelWidget.ViewTarget = boundingBox.Center();
			modelWidget.ViewFov = 0.3f;
			modelWidget.AutoRotationVector = (autoRotate ? new Vector3(0f, MathUtils.Clamp(1.7f / boundingBox.Size().Length(), 0.25f, 1.4f), 0f) : Vector3.Zero);
			if (autoAspect)
			{
				float num = MathUtils.Clamp(boundingBox.Size().XZ.Length() / boundingBox.Size().Y, 1f, 1.5f);
				modelWidget.Size = new Vector2(modelWidget.Size.Y * num, modelWidget.Size.Y);
			}
		}

		// Token: 0x0400028A RID: 650
		public ListPanelWidget m_creaturesList;

		// Token: 0x0400028B RID: 651
		public Screen m_previousScreen;
	}
}
