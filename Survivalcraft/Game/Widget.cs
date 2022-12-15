using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;
using Engine.Input;
using Engine.Serialization;
using XmlUtilities;

namespace Game
{
	// Token: 0x020003A5 RID: 933
	public class Widget : IDisposable
	{
		// Token: 0x170004BE RID: 1214
		// (get) Token: 0x06001B88 RID: 7048 RVA: 0x000D6F44 File Offset: 0x000D5144
		// (set) Token: 0x06001B89 RID: 7049 RVA: 0x000D6F4C File Offset: 0x000D514C
		public WidgetInput WidgetsHierarchyInput
		{
			get
			{
				return this.m_widgetsHierarchyInput;
			}
			set
			{
				if (value == null)
				{
					if (this.m_widgetsHierarchyInput != null)
					{
						this.m_widgetsHierarchyInput.m_widget = null;
						this.m_widgetsHierarchyInput = null;
					}
					return;
				}
				if (value.m_widget != null && value.m_widget != this)
				{
					throw new InvalidOperationException("WidgetInput already assigned to another widget.");
				}
				value.m_widget = this;
				this.m_widgetsHierarchyInput = value;
			}
		}

		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06001B8A RID: 7050 RVA: 0x000D6FA4 File Offset: 0x000D51A4
		public WidgetInput Input
		{
			get
			{
				Widget widget = this;
				while (widget.WidgetsHierarchyInput == null)
				{
					widget = widget.ParentWidget;
					if (widget == null)
					{
						return WidgetInput.EmptyInput;
					}
				}
				return widget.WidgetsHierarchyInput;
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06001B8B RID: 7051 RVA: 0x000D6FD1 File Offset: 0x000D51D1
		// (set) Token: 0x06001B8C RID: 7052 RVA: 0x000D6FD9 File Offset: 0x000D51D9
		public Matrix LayoutTransform
		{
			get
			{
				return this.m_layoutTransform;
			}
			set
			{
				this.m_layoutTransform = value;
				this.m_isLayoutTransformIdentity = (value == Matrix.Identity);
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06001B8D RID: 7053 RVA: 0x000D6FF3 File Offset: 0x000D51F3
		// (set) Token: 0x06001B8E RID: 7054 RVA: 0x000D6FFB File Offset: 0x000D51FB
		public Matrix RenderTransform
		{
			get
			{
				return this.m_renderTransform;
			}
			set
			{
				this.m_renderTransform = value;
				this.m_isRenderTransformIdentity = (value == Matrix.Identity);
			}
		}

		// Token: 0x170004C2 RID: 1218
		// (get) Token: 0x06001B8F RID: 7055 RVA: 0x000D7015 File Offset: 0x000D5215
		public Matrix GlobalTransform
		{
			get
			{
				return this.m_globalTransform;
			}
		}

		// Token: 0x170004C3 RID: 1219
		// (get) Token: 0x06001B90 RID: 7056 RVA: 0x000D7020 File Offset: 0x000D5220
		public float GlobalScale
		{
			get
			{
				if (this.m_globalScale == null)
				{
					this.m_globalScale = new float?(this.m_globalTransform.Right.Length());
				}
				return this.m_globalScale.Value;
			}
		}

		// Token: 0x170004C4 RID: 1220
		// (get) Token: 0x06001B91 RID: 7057 RVA: 0x000D7063 File Offset: 0x000D5263
		public Matrix InvertedGlobalTransform
		{
			get
			{
				if (this.m_invertedGlobalTransform == null)
				{
					this.m_invertedGlobalTransform = new Matrix?(Matrix.Invert(this.m_globalTransform));
				}
				return this.m_invertedGlobalTransform.Value;
			}
		}

		// Token: 0x170004C5 RID: 1221
		// (get) Token: 0x06001B92 RID: 7058 RVA: 0x000D7093 File Offset: 0x000D5293
		public BoundingRectangle GlobalBounds
		{
			get
			{
				return this.m_globalBounds;
			}
		}

		// Token: 0x170004C6 RID: 1222
		// (get) Token: 0x06001B93 RID: 7059 RVA: 0x000D709B File Offset: 0x000D529B
		// (set) Token: 0x06001B94 RID: 7060 RVA: 0x000D70A3 File Offset: 0x000D52A3
		public Color ColorTransform
		{
			get
			{
				return this.m_colorTransform;
			}
			set
			{
				this.m_colorTransform = value;
			}
		}

		// Token: 0x170004C7 RID: 1223
		// (get) Token: 0x06001B95 RID: 7061 RVA: 0x000D70AC File Offset: 0x000D52AC
		public Color GlobalColorTransform
		{
			get
			{
				return this.m_globalColorTransform;
			}
		}

		// Token: 0x170004C8 RID: 1224
		// (get) Token: 0x06001B96 RID: 7062 RVA: 0x000D70B4 File Offset: 0x000D52B4
		// (set) Token: 0x06001B97 RID: 7063 RVA: 0x000D70BC File Offset: 0x000D52BC
		public virtual string Name { get; set; }

		// Token: 0x170004C9 RID: 1225
		// (get) Token: 0x06001B98 RID: 7064 RVA: 0x000D70C5 File Offset: 0x000D52C5
		// (set) Token: 0x06001B99 RID: 7065 RVA: 0x000D70CD File Offset: 0x000D52CD
		public object Tag { get; set; }

		// Token: 0x170004CA RID: 1226
		// (get) Token: 0x06001B9A RID: 7066 RVA: 0x000D70D6 File Offset: 0x000D52D6
		// (set) Token: 0x06001B9B RID: 7067 RVA: 0x000D70DE File Offset: 0x000D52DE
		public virtual bool IsVisible
		{
			get
			{
				return this.m_isVisible;
			}
			set
			{
				if (value != this.m_isVisible)
				{
					this.m_isVisible = value;
					if (!this.m_isVisible)
					{
						this.UpdateCeases();
					}
				}
			}
		}

		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x06001B9C RID: 7068 RVA: 0x000D70FE File Offset: 0x000D52FE
		// (set) Token: 0x06001B9D RID: 7069 RVA: 0x000D7106 File Offset: 0x000D5306
		public virtual bool IsEnabled
		{
			get
			{
				return this.m_isEnabled;
			}
			set
			{
				if (value != this.m_isEnabled)
				{
					this.m_isEnabled = value;
					if (!this.m_isEnabled)
					{
						this.UpdateCeases();
					}
				}
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x06001B9E RID: 7070 RVA: 0x000D7126 File Offset: 0x000D5326
		// (set) Token: 0x06001B9F RID: 7071 RVA: 0x000D712E File Offset: 0x000D532E
		public virtual bool IsHitTestVisible { get; set; }

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x06001BA0 RID: 7072 RVA: 0x000D7137 File Offset: 0x000D5337
		public bool IsVisibleGlobal
		{
			get
			{
				return this.IsVisible && (this.ParentWidget == null || this.ParentWidget.IsVisibleGlobal);
			}
		}

		// Token: 0x170004CE RID: 1230
		// (get) Token: 0x06001BA1 RID: 7073 RVA: 0x000D7158 File Offset: 0x000D5358
		public bool IsEnabledGlobal
		{
			get
			{
				return this.IsEnabled && (this.ParentWidget == null || this.ParentWidget.IsEnabledGlobal);
			}
		}

		// Token: 0x170004CF RID: 1231
		// (get) Token: 0x06001BA2 RID: 7074 RVA: 0x000D7179 File Offset: 0x000D5379
		// (set) Token: 0x06001BA3 RID: 7075 RVA: 0x000D7181 File Offset: 0x000D5381
		public bool ClampToBounds { get; set; }

		// Token: 0x170004D0 RID: 1232
		// (get) Token: 0x06001BA4 RID: 7076 RVA: 0x000D718A File Offset: 0x000D538A
		// (set) Token: 0x06001BA5 RID: 7077 RVA: 0x000D7192 File Offset: 0x000D5392
		public virtual Vector2 Margin { get; set; }

		// Token: 0x170004D1 RID: 1233
		// (get) Token: 0x06001BA6 RID: 7078 RVA: 0x000D719B File Offset: 0x000D539B
		// (set) Token: 0x06001BA7 RID: 7079 RVA: 0x000D71A3 File Offset: 0x000D53A3
		public virtual WidgetAlignment HorizontalAlignment { get; set; }

		// Token: 0x170004D2 RID: 1234
		// (get) Token: 0x06001BA8 RID: 7080 RVA: 0x000D71AC File Offset: 0x000D53AC
		// (set) Token: 0x06001BA9 RID: 7081 RVA: 0x000D71B4 File Offset: 0x000D53B4
		public virtual WidgetAlignment VerticalAlignment { get; set; }

		// Token: 0x170004D3 RID: 1235
		// (get) Token: 0x06001BAA RID: 7082 RVA: 0x000D71BD File Offset: 0x000D53BD
		public Vector2 ActualSize
		{
			get
			{
				return this.m_actualSize;
			}
		}

		// Token: 0x170004D4 RID: 1236
		// (get) Token: 0x06001BAB RID: 7083 RVA: 0x000D71C5 File Offset: 0x000D53C5
		// (set) Token: 0x06001BAC RID: 7084 RVA: 0x000D71CD File Offset: 0x000D53CD
		public Vector2 DesiredSize
		{
			get
			{
				return this.m_desiredSize;
			}
			set
			{
				this.m_desiredSize = value;
			}
		}

		// Token: 0x170004D5 RID: 1237
		// (get) Token: 0x06001BAD RID: 7085 RVA: 0x000D71D6 File Offset: 0x000D53D6
		public Vector2 ParentDesiredSize
		{
			get
			{
				return this.m_parentDesiredSize;
			}
		}

		// Token: 0x170004D6 RID: 1238
		// (get) Token: 0x06001BAE RID: 7086 RVA: 0x000D71DE File Offset: 0x000D53DE
		// (set) Token: 0x06001BAF RID: 7087 RVA: 0x000D71E6 File Offset: 0x000D53E6
		public bool IsUpdateEnabled { get; set; } = true;

		// Token: 0x170004D7 RID: 1239
		// (get) Token: 0x06001BB0 RID: 7088 RVA: 0x000D71EF File Offset: 0x000D53EF
		// (set) Token: 0x06001BB1 RID: 7089 RVA: 0x000D71F7 File Offset: 0x000D53F7
		public bool IsDrawEnabled { get; set; } = true;

		// Token: 0x170004D8 RID: 1240
		// (get) Token: 0x06001BB2 RID: 7090 RVA: 0x000D7200 File Offset: 0x000D5400
		// (set) Token: 0x06001BB3 RID: 7091 RVA: 0x000D7208 File Offset: 0x000D5408
		public bool IsDrawRequired { get; set; }

		// Token: 0x170004D9 RID: 1241
		// (get) Token: 0x06001BB4 RID: 7092 RVA: 0x000D7211 File Offset: 0x000D5411
		// (set) Token: 0x06001BB5 RID: 7093 RVA: 0x000D7219 File Offset: 0x000D5419
		public bool IsOverdrawRequired { get; set; }

		// Token: 0x170004DA RID: 1242
		// (set) Token: 0x06001BB6 RID: 7094 RVA: 0x000D7222 File Offset: 0x000D5422
		public XElement Style
		{
			set
			{
				this.LoadContents(null, value);
			}
		}

		// Token: 0x170004DB RID: 1243
		// (get) Token: 0x06001BB7 RID: 7095 RVA: 0x000D722C File Offset: 0x000D542C
		// (set) Token: 0x06001BB8 RID: 7096 RVA: 0x000D7234 File Offset: 0x000D5434
		public ContainerWidget ParentWidget { get; set; }

		// Token: 0x170004DC RID: 1244
		// (get) Token: 0x06001BB9 RID: 7097 RVA: 0x000D723D File Offset: 0x000D543D
		public Widget RootWidget
		{
			get
			{
				if (this.ParentWidget == null)
				{
					return this;
				}
				return this.ParentWidget.RootWidget;
			}
		}

		// Token: 0x06001BBA RID: 7098 RVA: 0x000D7254 File Offset: 0x000D5454
		public Widget()
		{
			this.IsVisible = true;
			this.IsHitTestVisible = true;
			this.IsEnabled = true;
			this.DesiredSize = new Vector2(float.PositiveInfinity);
		}

		// Token: 0x06001BBB RID: 7099 RVA: 0x000D72D4 File Offset: 0x000D54D4
		public static Widget LoadWidget(object eventsTarget, XElement node, ContainerWidget parentWidget)
		{
			if (node.Name.LocalName.Contains("."))
			{
				throw new NotImplementedException("Node property specification not implemented.");
			}
			Widget widget = Activator.CreateInstance(Widget.FindTypeFromXmlName(node.Name.LocalName, node.Name.NamespaceName)) as Widget;
			if (widget == null)
			{
				throw new Exception("Type \"" + node.Name.LocalName + "\" is not a Widget.");
			}
			if (parentWidget != null)
			{
				parentWidget.Children.Add(widget);
			}
			widget.LoadContents(eventsTarget, node);
			return widget;
		}

		// Token: 0x06001BBC RID: 7100 RVA: 0x000D7364 File Offset: 0x000D5564
		public void LoadContents(object eventsTarget, XElement node)
		{
			this.LoadProperties(eventsTarget, node);
			this.LoadChildren(eventsTarget, node);
		}

		// Token: 0x06001BBD RID: 7101 RVA: 0x000D7378 File Offset: 0x000D5578
		public void LoadProperties(object eventsTarget, XElement node)
		{
			IEnumerable<PropertyInfo> runtimeProperties = base.GetType().GetRuntimeProperties();
			using (IEnumerator<XAttribute> enumerator = node.Attributes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XAttribute attribute = enumerator.Current;
					if (!attribute.IsNamespaceDeclaration && !attribute.Name.LocalName.StartsWith("_"))
					{
						if (attribute.Name.LocalName.Contains('.'))
						{
							string[] array = attribute.Name.LocalName.Split(new char[]
							{
								'.'
							});
							if (array.Length != 2)
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property reference must have form \"TypeName.PropertyName\", property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							Type type = Widget.FindTypeFromXmlName(array[0], (attribute.Name.NamespaceName != string.Empty) ? attribute.Name.NamespaceName : node.Name.NamespaceName);
							string setterName = "Set" + array[1];
							MethodInfo methodInfo = type.GetRuntimeMethods().FirstOrDefault((MethodInfo mi) => mi.Name == setterName && mi.IsPublic && mi.IsStatic);
							if (!(methodInfo != null))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property public static setter method \"",
									setterName,
									"\" not found, property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length != 2 || !(parameters[0].ParameterType == typeof(Widget)))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Attached property setter method must take 2 parameters and first one must be of type Widget, property \"",
									attribute.Name.LocalName,
									"\" in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							object obj = HumanReadableConverter.ConvertFromString(parameters[1].ParameterType, attribute.Value);
							methodInfo.Invoke(null, new object[]
							{
								this,
								obj
							});
						}
						else
						{
							PropertyInfo propertyInfo = (from pi in runtimeProperties
							where pi.Name == attribute.Name.LocalName
							select pi).FirstOrDefault<PropertyInfo>();
							if (!(propertyInfo != null))
							{
								throw new InvalidOperationException(string.Concat(new string[]
								{
									"Property \"",
									attribute.Name.LocalName,
									"\" not found in widget of type \"",
									base.GetType().FullName,
									"\"."
								}));
							}
							if (attribute.Value.StartsWith("{") && attribute.Value.EndsWith("}"))
							{
								string name = attribute.Value.Substring(1, attribute.Value.Length - 2);
								object value = ContentManager.Get(propertyInfo.PropertyType, name);
								propertyInfo.SetValue(this, value, null);
							}
							else
							{
								object obj2 = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attribute.Value);
								if (propertyInfo.PropertyType == typeof(string))
								{
									obj2 = ((string)obj2).Replace("\\n", "\n").Replace("\\t", "\t");
								}
								propertyInfo.SetValue(this, obj2, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001BBE RID: 7102 RVA: 0x000D776C File Offset: 0x000D596C
		public void LoadChildren(object eventsTarget, XElement node)
		{
			if (node.HasElements)
			{
				ContainerWidget containerWidget = this as ContainerWidget;
				if (containerWidget == null)
				{
					throw new Exception("Type \"" + node.Name.LocalName + "\" is not a ContainerWidget, but it contains child widgets.");
				}
				foreach (XElement node2 in node.Elements())
				{
					if (Widget.IsNodeIncludedOnCurrentPlatform(node2))
					{
						Widget widget = null;
						string attributeValue = XmlUtils.GetAttributeValue<string>(node2, "Name", null);
						if (attributeValue != null)
						{
							widget = containerWidget.Children.Find(attributeValue, false);
						}
						if (widget != null)
						{
							widget.LoadContents(eventsTarget, node2);
						}
						else
						{
							Widget.LoadWidget(eventsTarget, node2, containerWidget);
						}
					}
				}
			}
		}

		// Token: 0x06001BBF RID: 7103 RVA: 0x000D7828 File Offset: 0x000D5A28
		public bool IsChildWidgetOf(ContainerWidget containerWidget)
		{
			return containerWidget == this.ParentWidget || (this.ParentWidget != null && this.ParentWidget.IsChildWidgetOf(containerWidget));
		}

		// Token: 0x06001BC0 RID: 7104 RVA: 0x000D784B File Offset: 0x000D5A4B
		public virtual void ChangeParent(ContainerWidget parentWidget)
		{
			if (parentWidget != this.ParentWidget)
			{
				this.ParentWidget = parentWidget;
				if (parentWidget == null)
				{
					this.UpdateCeases();
				}
			}
		}

		// Token: 0x06001BC1 RID: 7105 RVA: 0x000D7868 File Offset: 0x000D5A68
		public void Measure(Vector2 parentAvailableSize)
		{
			if (this.MeasureOverride1 != null)
			{
				this.MeasureOverride1(parentAvailableSize);
				return;
			}
			this.MeasureOverride(parentAvailableSize);
			if (this.DesiredSize.X != float.PositiveInfinity && this.DesiredSize.Y != float.PositiveInfinity)
			{
				BoundingRectangle boundingRectangle = this.TransformBoundsToParent(this.DesiredSize);
				this.m_parentDesiredSize = boundingRectangle.Size();
				this.m_parentOffset = -boundingRectangle.Min;
				return;
			}
			this.m_parentDesiredSize = this.DesiredSize;
			this.m_parentOffset = Vector2.Zero;
		}

		// Token: 0x06001BC2 RID: 7106 RVA: 0x000D78FA File Offset: 0x000D5AFA
		public virtual void MeasureOverride(Vector2 parentAvailableSize)
		{
		}

		// Token: 0x06001BC3 RID: 7107 RVA: 0x000D78FC File Offset: 0x000D5AFC
		public void Arrange(Vector2 position, Vector2 parentActualSize)
		{
			float num = this.m_layoutTransform.M11 * this.m_layoutTransform.M11;
			float num2 = this.m_layoutTransform.M12 * this.m_layoutTransform.M12;
			float num3 = this.m_layoutTransform.M21 * this.m_layoutTransform.M21;
			float num4 = this.m_layoutTransform.M22 * this.m_layoutTransform.M22;
			this.m_actualSize.X = (num * parentActualSize.X + num3 * parentActualSize.Y) / (num + num3);
			this.m_actualSize.Y = (num2 * parentActualSize.X + num4 * parentActualSize.Y) / (num2 + num4);
			this.m_parentOffset = -this.TransformBoundsToParent(this.m_actualSize).Min;
			if (this.ParentWidget != null)
			{
				this.m_globalColorTransform = this.ParentWidget.m_globalColorTransform * this.m_colorTransform;
			}
			else
			{
				this.m_globalColorTransform = this.m_colorTransform;
			}
			if (this.m_isRenderTransformIdentity)
			{
				this.m_globalTransform = this.m_layoutTransform;
			}
			else if (this.m_isLayoutTransformIdentity)
			{
				this.m_globalTransform = this.m_renderTransform;
			}
			else
			{
				this.m_globalTransform = this.m_renderTransform * this.m_layoutTransform;
			}
			this.m_globalTransform.M41 = this.m_globalTransform.M41 + (position.X + this.m_parentOffset.X);
			this.m_globalTransform.M42 = this.m_globalTransform.M42 + (position.Y + this.m_parentOffset.Y);
			if (this.ParentWidget != null)
			{
				this.m_globalTransform *= this.ParentWidget.GlobalTransform;
			}
			this.m_invertedGlobalTransform = null;
			this.m_globalScale = null;
			this.m_globalBounds = this.TransformBoundsToGlobal(this.m_actualSize);
			this.ArrangeOverride();
		}

		// Token: 0x06001BC4 RID: 7108 RVA: 0x000D7AD0 File Offset: 0x000D5CD0
		public virtual void ArrangeOverride()
		{
		}

		// Token: 0x06001BC5 RID: 7109 RVA: 0x000D7AD2 File Offset: 0x000D5CD2
		public virtual void UpdateCeases()
		{
		}

		// Token: 0x06001BC6 RID: 7110 RVA: 0x000D7AD4 File Offset: 0x000D5CD4
		public virtual void Update()
		{
		}

		// Token: 0x06001BC7 RID: 7111 RVA: 0x000D7AD6 File Offset: 0x000D5CD6
		public virtual void Draw(Widget.DrawContext dc)
		{
		}

		// Token: 0x06001BC8 RID: 7112 RVA: 0x000D7AD8 File Offset: 0x000D5CD8
		public virtual void Overdraw(Widget.DrawContext dc)
		{
		}

		// Token: 0x06001BC9 RID: 7113 RVA: 0x000D7ADC File Offset: 0x000D5CDC
		public virtual bool HitTest(Vector2 point)
		{
			Vector2 vector = this.ScreenToWidget(point);
			return vector.X >= 0f && vector.Y >= 0f && vector.X <= this.ActualSize.X && vector.Y <= this.ActualSize.Y;
		}

		// Token: 0x06001BCA RID: 7114 RVA: 0x000D7B36 File Offset: 0x000D5D36
		public Widget HitTestGlobal(Vector2 point, Func<Widget, bool> predicate = null)
		{
			return Widget.HitTestGlobal(this.RootWidget, point, predicate);
		}

		// Token: 0x06001BCB RID: 7115 RVA: 0x000D7B45 File Offset: 0x000D5D45
		public Vector2 ScreenToWidget(Vector2 p)
		{
			return Vector2.Transform(p, this.InvertedGlobalTransform);
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x000D7B53 File Offset: 0x000D5D53
		public Vector2 WidgetToScreen(Vector2 p)
		{
			return Vector2.Transform(p, this.GlobalTransform);
		}

		// Token: 0x06001BCD RID: 7117 RVA: 0x000D7B61 File Offset: 0x000D5D61
		public virtual void Dispose()
		{
		}

		// Token: 0x06001BCE RID: 7118 RVA: 0x000D7B64 File Offset: 0x000D5D64
		public static bool TestOverlap(Widget w1, Widget w2)
		{
			return w2.m_globalBounds.Min.X < w1.m_globalBounds.Max.X - 0.001f && w2.m_globalBounds.Min.Y < w1.m_globalBounds.Max.Y - 0.001f && w1.m_globalBounds.Min.X < w2.m_globalBounds.Max.X - 0.001f && w1.m_globalBounds.Min.Y < w2.m_globalBounds.Max.Y - 0.001f;
		}

		// Token: 0x06001BCF RID: 7119 RVA: 0x000D7C1C File Offset: 0x000D5E1C
		public static bool IsNodeIncludedOnCurrentPlatform(XElement node)
		{
			string attributeValue = XmlUtils.GetAttributeValue<string>(node, "_IncludePlatforms", null);
			string attributeValue2 = XmlUtils.GetAttributeValue<string>(node, "_ExcludePlatforms", null);
			if (attributeValue != null && attributeValue2 == null)
			{
				if (attributeValue.Split(new char[]
				{
					' '
				}).Contains(VersionsManager.Platform.ToString()))
				{
					return true;
				}
			}
			else
			{
				if (attributeValue2 == null || attributeValue != null)
				{
					return true;
				}
				if (!attributeValue2.Split(new char[]
				{
					' '
				}).Contains(VersionsManager.Platform.ToString()))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001BD0 RID: 7120 RVA: 0x000D7CAC File Offset: 0x000D5EAC
		public static void UpdateWidgetsHierarchy(Widget rootWidget)
		{
			if (rootWidget.IsUpdateEnabled)
			{
				bool isMouseVisible = false;
				Widget.UpdateWidgetsHierarchy(rootWidget, ref isMouseVisible);
				Mouse.IsMouseVisible = isMouseVisible;
			}
		}

		// Token: 0x06001BD1 RID: 7121 RVA: 0x000D7CD1 File Offset: 0x000D5ED1
		public static void LayoutWidgetsHierarchy(Widget rootWidget, Vector2 availableSize)
		{
			rootWidget.Measure(availableSize);
			rootWidget.Arrange(Vector2.Zero, availableSize);
		}

		// Token: 0x06001BD2 RID: 7122 RVA: 0x000D7CE8 File Offset: 0x000D5EE8
		public static void DrawWidgetsHierarchy(Widget rootWidget)
		{
			Widget.DrawContext drawContext = (Widget.m_drawContextsCache.Count > 0) ? Widget.m_drawContextsCache.Dequeue() : new Widget.DrawContext();
			try
			{
				drawContext.DrawWidgetsHierarchy(rootWidget);
			}
			finally
			{
				Widget.m_drawContextsCache.Enqueue(drawContext);
			}
		}

		// Token: 0x06001BD3 RID: 7123 RVA: 0x000D7D3C File Offset: 0x000D5F3C
		public BoundingRectangle TransformBoundsToParent(Vector2 size)
		{
			float num = this.m_layoutTransform.M11 * size.X;
			float num2 = this.m_layoutTransform.M21 * size.Y;
			float x = num + num2;
			float num3 = this.m_layoutTransform.M12 * size.X;
			float num4 = this.m_layoutTransform.M22 * size.Y;
			float x2 = num3 + num4;
			float x3 = MathUtils.Min(0f, num, num2, x);
			float x4 = MathUtils.Max(0f, num, num2, x);
			float y = MathUtils.Min(0f, num3, num4, x2);
			float y2 = MathUtils.Max(0f, num3, num4, x2);
			return new BoundingRectangle(x3, y, x4, y2);
		}

		// Token: 0x06001BD4 RID: 7124 RVA: 0x000D7DEC File Offset: 0x000D5FEC
		public BoundingRectangle TransformBoundsToGlobal(Vector2 size)
		{
			float num = this.m_globalTransform.M11 * size.X;
			float num2 = this.m_globalTransform.M21 * size.Y;
			float x = num + num2;
			float num3 = this.m_globalTransform.M12 * size.X;
			float num4 = this.m_globalTransform.M22 * size.Y;
			float x2 = num3 + num4;
			float num5 = MathUtils.Min(0f, num, num2, x);
			float num6 = MathUtils.Max(0f, num, num2, x);
			float num7 = MathUtils.Min(0f, num3, num4, x2);
			float y = MathUtils.Max(0f, num3, num4, x2) + this.m_globalTransform.M42;
			return new BoundingRectangle(num5 + this.m_globalTransform.M41, num7 + this.m_globalTransform.M42, num6 + this.m_globalTransform.M41, y);
		}

		// Token: 0x06001BD5 RID: 7125 RVA: 0x000D7ECC File Offset: 0x000D60CC
		public static Type FindTypeFromXmlName(string name, string namespaceName)
		{
			if (string.IsNullOrEmpty(namespaceName))
			{
				throw new InvalidOperationException("Namespace must be specified when creating types in XML.");
			}
			Uri uri = new Uri(namespaceName);
			if (uri.Scheme == "runtime-namespace")
			{
				return TypeCache.FindType(uri.AbsolutePath + "." + name, false, true);
			}
			throw new InvalidOperationException("Unknown uri scheme when loading widget. Scheme must be runtime-namespace.");
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x000D7F28 File Offset: 0x000D6128
		public static Widget HitTestGlobal(Widget widget, Vector2 point, Func<Widget, bool> predicate)
		{
			if (widget != null && widget.IsVisible && (!widget.ClampToBounds || widget.HitTest(point)))
			{
				ContainerWidget containerWidget = widget as ContainerWidget;
				if (containerWidget != null)
				{
					WidgetsList children = containerWidget.Children;
					for (int i = children.Count - 1; i >= 0; i--)
					{
						Widget widget2 = Widget.HitTestGlobal(children[i], point, predicate);
						if (widget2 != null)
						{
							return widget2;
						}
					}
				}
				if (widget.IsHitTestVisible && widget.HitTest(point) && (predicate == null || predicate(widget)))
				{
					return widget;
				}
			}
			return null;
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x000D7FAC File Offset: 0x000D61AC
		public static void UpdateWidgetsHierarchy(Widget widget, ref bool isMouseCursorVisible)
		{
			if (!widget.IsVisible || !widget.IsEnabled)
			{
				return;
			}
			if (widget.WidgetsHierarchyInput != null)
			{
				widget.WidgetsHierarchyInput.Update();
				isMouseCursorVisible |= widget.WidgetsHierarchyInput.IsMouseCursorVisible;
			}
			ContainerWidget containerWidget = widget as ContainerWidget;
			if (containerWidget != null)
			{
				WidgetsList children = containerWidget.Children;
				for (int i = children.Count - 1; i >= 0; i--)
				{
					if (i < children.Count)
					{
						Widget.UpdateWidgetsHierarchy(children[i], ref isMouseCursorVisible);
					}
				}
			}
			if (widget.Update1 == null)
			{
				widget.Update();
				return;
			}
			widget.Update1();
		}

		// Token: 0x04001325 RID: 4901
		public Action<Vector2> MeasureOverride1;

		// Token: 0x04001326 RID: 4902
		public Action Update1;

		// Token: 0x04001327 RID: 4903
		public bool m_isVisible;

		// Token: 0x04001328 RID: 4904
		public bool m_isEnabled;

		// Token: 0x04001329 RID: 4905
		public Vector2 m_actualSize;

		// Token: 0x0400132A RID: 4906
		public Vector2 m_desiredSize;

		// Token: 0x0400132B RID: 4907
		public Vector2 m_parentDesiredSize;

		// Token: 0x0400132C RID: 4908
		public BoundingRectangle m_globalBounds;

		// Token: 0x0400132D RID: 4909
		public Vector2 m_parentOffset;

		// Token: 0x0400132E RID: 4910
		public bool m_isLayoutTransformIdentity = true;

		// Token: 0x0400132F RID: 4911
		public bool m_isRenderTransformIdentity = true;

		// Token: 0x04001330 RID: 4912
		public Matrix m_layoutTransform = Matrix.Identity;

		// Token: 0x04001331 RID: 4913
		public Matrix m_renderTransform = Matrix.Identity;

		// Token: 0x04001332 RID: 4914
		public Matrix m_globalTransform = Matrix.Identity;

		// Token: 0x04001333 RID: 4915
		public Matrix? m_invertedGlobalTransform;

		// Token: 0x04001334 RID: 4916
		public float? m_globalScale;

		// Token: 0x04001335 RID: 4917
		public Color m_colorTransform = Color.White;

		// Token: 0x04001336 RID: 4918
		public Color m_globalColorTransform;

		// Token: 0x04001337 RID: 4919
		public WidgetInput m_widgetsHierarchyInput;

		// Token: 0x04001338 RID: 4920
		public static Queue<Widget.DrawContext> m_drawContextsCache = new Queue<Widget.DrawContext>();

		// Token: 0x04001339 RID: 4921
		public static int LayersLimit = -1;

		// Token: 0x0400133A RID: 4922
		public static bool DrawWidgetBounds = false;

		// Token: 0x02000528 RID: 1320
		public class DrawContext
		{
			// Token: 0x06002159 RID: 8537 RVA: 0x000E6BF2 File Offset: 0x000E4DF2
			public void DrawWidgetsHierarchy(Widget rootWidget)
			{
				this.m_drawItems.Clear();
				this.CollateDrawItems(rootWidget, Display.ScissorRectangle);
				this.AssignDrawItemsLayers();
				this.RenderDrawItems();
				this.ReturnDrawItemsToCache();
			}

			// Token: 0x0600215A RID: 8538 RVA: 0x000E6C20 File Offset: 0x000E4E20
			public void CollateDrawItems(Widget widget, Rectangle scissorRectangle)
			{
				if (!widget.IsVisible || !widget.IsDrawEnabled)
				{
					return;
				}
				bool flag = widget.GlobalBounds.Intersection(new BoundingRectangle((float)scissorRectangle.Left, (float)scissorRectangle.Top, (float)scissorRectangle.Right, (float)scissorRectangle.Bottom));
				Rectangle? scissorRectangle2 = null;
				if (widget.ClampToBounds && flag)
				{
					scissorRectangle2 = new Rectangle?(scissorRectangle);
					int num = (int)MathUtils.Floor(widget.GlobalBounds.Min.X - 0.5f);
					int num2 = (int)MathUtils.Floor(widget.GlobalBounds.Min.Y - 0.5f);
					int num3 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.X - 0.5f);
					int num4 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.Y - 0.5f);
					scissorRectangle = Rectangle.Intersection(new Rectangle(num, num2, num3 - num, num4 - num2), scissorRectangle2.Value);
					Widget.DrawItem drawItemFromCache = this.GetDrawItemFromCache();
					drawItemFromCache.ScissorRectangle = new Rectangle?(scissorRectangle);
					this.m_drawItems.Add(drawItemFromCache);
				}
				if (widget.IsDrawRequired && flag)
				{
					Widget.DrawItem drawItemFromCache2 = this.GetDrawItemFromCache();
					drawItemFromCache2.Widget = widget;
					this.m_drawItems.Add(drawItemFromCache2);
				}
				if (flag || !widget.ClampToBounds)
				{
					ContainerWidget containerWidget = widget as ContainerWidget;
					if (containerWidget != null)
					{
						foreach (Widget widget2 in containerWidget.Children)
						{
							this.CollateDrawItems(widget2, scissorRectangle);
						}
					}
				}
				if (widget.IsOverdrawRequired && flag)
				{
					Widget.DrawItem drawItemFromCache3 = this.GetDrawItemFromCache();
					drawItemFromCache3.Widget = widget;
					drawItemFromCache3.IsOverdraw = true;
					this.m_drawItems.Add(drawItemFromCache3);
				}
				if (scissorRectangle2 != null)
				{
					Widget.DrawItem drawItemFromCache4 = this.GetDrawItemFromCache();
					drawItemFromCache4.ScissorRectangle = scissorRectangle2;
					this.m_drawItems.Add(drawItemFromCache4);
				}
				WidgetInput widgetsHierarchyInput = widget.WidgetsHierarchyInput;
				if (widgetsHierarchyInput == null)
				{
					return;
				}
				widgetsHierarchyInput.Draw(this);
			}

			// Token: 0x0600215B RID: 8539 RVA: 0x000E6E34 File Offset: 0x000E5034
			public void AssignDrawItemsLayers()
			{
				for (int i = 0; i < this.m_drawItems.Count; i++)
				{
					Widget.DrawItem drawItem = this.m_drawItems[i];
					for (int j = i + 1; j < this.m_drawItems.Count; j++)
					{
						Widget.DrawItem drawItem2 = this.m_drawItems[j];
						if (drawItem.ScissorRectangle != null || drawItem2.ScissorRectangle != null)
						{
							drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
						}
						else if (Widget.TestOverlap(drawItem.Widget, drawItem2.Widget))
						{
							drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
						}
					}
				}
				this.m_drawItems.Sort();
			}

			// Token: 0x0600215C RID: 8540 RVA: 0x000E6EFC File Offset: 0x000E50FC
			public void RenderDrawItems()
			{
				Rectangle scissorRectangle = Display.ScissorRectangle;
				int num = 0;
				foreach (Widget.DrawItem drawItem in this.m_drawItems)
				{
					if (Widget.LayersLimit >= 0 && drawItem.Layer > Widget.LayersLimit)
					{
						break;
					}
					if (drawItem.Layer != num)
					{
						num = drawItem.Layer;
						this.PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
						this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
					}
					if (drawItem.Widget != null)
					{
						if (drawItem.IsOverdraw)
						{
							drawItem.Widget.Overdraw(this);
						}
						else
						{
							drawItem.Widget.Draw(this);
						}
					}
					else
					{
						Display.ScissorRectangle = Rectangle.Intersection(scissorRectangle, drawItem.ScissorRectangle.Value);
					}
				}
				this.PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
				this.PrimitivesRenderer2D.Flush(true, int.MaxValue);
				Display.ScissorRectangle = scissorRectangle;
				this.CursorPrimitivesRenderer2D.Flush(true, int.MaxValue);
			}

			// Token: 0x0600215D RID: 8541 RVA: 0x000E7028 File Offset: 0x000E5228
			public Widget.DrawItem GetDrawItemFromCache()
			{
				if (Widget.DrawContext.m_drawItemsCache.Count > 0)
				{
					Widget.DrawItem result = Widget.DrawContext.m_drawItemsCache[Widget.DrawContext.m_drawItemsCache.Count - 1];
					Widget.DrawContext.m_drawItemsCache.RemoveAt(Widget.DrawContext.m_drawItemsCache.Count - 1);
					return result;
				}
				return new Widget.DrawItem();
			}

			// Token: 0x0600215E RID: 8542 RVA: 0x000E7074 File Offset: 0x000E5274
			public void ReturnDrawItemsToCache()
			{
				foreach (Widget.DrawItem drawItem in this.m_drawItems)
				{
					drawItem.Widget = null;
					drawItem.Layer = 0;
					drawItem.IsOverdraw = false;
					drawItem.ScissorRectangle = null;
					Widget.DrawContext.m_drawItemsCache.Add(drawItem);
				}
			}

			// Token: 0x040018FE RID: 6398
			public List<Widget.DrawItem> m_drawItems = new List<Widget.DrawItem>();

			// Token: 0x040018FF RID: 6399
			public static List<Widget.DrawItem> m_drawItemsCache = new List<Widget.DrawItem>();

			// Token: 0x04001900 RID: 6400
			public readonly PrimitivesRenderer2D PrimitivesRenderer2D = new PrimitivesRenderer2D();

			// Token: 0x04001901 RID: 6401
			public readonly PrimitivesRenderer3D PrimitivesRenderer3D = new PrimitivesRenderer3D();

			// Token: 0x04001902 RID: 6402
			public readonly PrimitivesRenderer2D CursorPrimitivesRenderer2D = new PrimitivesRenderer2D();
		}

		// Token: 0x02000529 RID: 1321
		public class DrawItem : IComparable<Widget.DrawItem>
		{
			// Token: 0x06002161 RID: 8545 RVA: 0x000E712C File Offset: 0x000E532C
			public int CompareTo(Widget.DrawItem other)
			{
				return this.Layer - other.Layer;
			}

			// Token: 0x04001903 RID: 6403
			public int Layer;

			// Token: 0x04001904 RID: 6404
			public bool IsOverdraw;

			// Token: 0x04001905 RID: 6405
			public Widget Widget;

			// Token: 0x04001906 RID: 6406
			public Rectangle? ScissorRectangle;
		}
	}
}
