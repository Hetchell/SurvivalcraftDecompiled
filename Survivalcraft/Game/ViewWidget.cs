using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x020003A3 RID: 931
	public class ViewWidget : TouchInputWidget, IDragTargetWidget
	{
		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x06001B78 RID: 7032 RVA: 0x000D6095 File Offset: 0x000D4295
		// (set) Token: 0x06001B79 RID: 7033 RVA: 0x000D609D File Offset: 0x000D429D
		public GameWidget GameWidget { get; set; }

		// Token: 0x170004BD RID: 1213
		// (get) Token: 0x06001B7A RID: 7034 RVA: 0x000D60A8 File Offset: 0x000D42A8
		public Point2? ScalingRenderTargetSize
		{
			get
			{
				if (this.m_scalingRenderTarget == null)
				{
					return null;
				}
				return new Point2?(new Point2(this.m_scalingRenderTarget.Width, this.m_scalingRenderTarget.Height));
			}
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x000D60E8 File Offset: 0x000D42E8
		public override void ChangeParent(ContainerWidget parentWidget)
		{
			if (parentWidget is GameWidget)
			{
				this.GameWidget = (GameWidget)parentWidget;
				this.m_subsystemDrawing = this.GameWidget.SubsystemGameWidgets.Project.FindSubsystem<SubsystemDrawing>(true);
				base.ChangeParent(parentWidget);
				return;
			}
			throw new InvalidOperationException("ViewWidget must be a child of GameWidget.");
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x000D6137 File Offset: 0x000D4337
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = true;
			base.MeasureOverride(parentAvailableSize);
		}

		// Token: 0x06001B7D RID: 7037 RVA: 0x000D6148 File Offset: 0x000D4348
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.GameWidget.PlayerData.ComponentPlayer != null && this.GameWidget.PlayerData.IsReadyForPlaying && !this.GameWidget.PlayerData.ComponentPlayer.ComponentInput.IsControlledByVr)
			{
				this.DrawToScreen(dc);
			}
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x000D619C File Offset: 0x000D439C
		public override void Dispose()
		{
			base.Dispose();
			Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x000D61AF File Offset: 0x000D43AF
		public void DragOver(Widget dragWidget, object data)
		{
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x000D61B4 File Offset: 0x000D43B4
		public void DragDrop(Widget dragWidget, object data)
		{
			InventoryDragData inventoryDragData = data as InventoryDragData;
			if (inventoryDragData != null && GameManager.Project != null)
			{
				SubsystemPickables subsystemPickables = GameManager.Project.FindSubsystem<SubsystemPickables>(true);
				ComponentPlayer componentPlayer = this.GameWidget.PlayerData.ComponentPlayer;
				int slotValue = inventoryDragData.Inventory.GetSlotValue(inventoryDragData.SlotIndex);
				int count = (componentPlayer != null && componentPlayer.ComponentInput.SplitSourceInventory == inventoryDragData.Inventory && componentPlayer.ComponentInput.SplitSourceSlotIndex == inventoryDragData.SlotIndex) ? 1 : ((inventoryDragData.DragMode != DragMode.SingleItem) ? inventoryDragData.Inventory.GetSlotCount(inventoryDragData.SlotIndex) : MathUtils.Min(inventoryDragData.Inventory.GetSlotCount(inventoryDragData.SlotIndex), 1));
				int num = inventoryDragData.Inventory.RemoveSlotItems(inventoryDragData.SlotIndex, count);
				if (num > 0)
				{
					Vector2 vector = dragWidget.WidgetToScreen(dragWidget.ActualSize / 2f);
					Vector3 value = Vector3.Normalize(this.GameWidget.ActiveCamera.ScreenToWorld(new Vector3(vector.X, vector.Y, 1f), Matrix.Identity) - this.GameWidget.ActiveCamera.ViewPosition) * 12f;
					subsystemPickables.AddPickable(slotValue, num, this.GameWidget.ActiveCamera.ViewPosition, new Vector3?(value), null);
				}
			}
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x000D631C File Offset: 0x000D451C
		public void SetupScalingRenderTarget()
		{
			float num = (SettingsManager.ResolutionMode == ResolutionMode.Low) ? 0.5f : ((SettingsManager.ResolutionMode != ResolutionMode.Medium) ? 1f : 0.75f);
			float num2 = base.GlobalTransform.Right.Length();
			float num3 = base.GlobalTransform.Up.Length();
			Vector2 vector = new Vector2(base.ActualSize.X * num2, base.ActualSize.Y * num3);
			Point2 point = new Point2
			{
				X = (int)MathUtils.Round(vector.X * num),
				Y = (int)MathUtils.Round(vector.Y * num)
			};
			if ((num < 1f || base.GlobalColorTransform != Color.White) && point.X > 0 && point.Y > 0)
			{
				if (this.m_scalingRenderTarget == null || this.m_scalingRenderTarget.Width != point.X || this.m_scalingRenderTarget.Height != point.Y)
				{
					Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
					this.m_scalingRenderTarget = new RenderTarget2D(point.X, point.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);
				}
				Display.RenderTarget = this.m_scalingRenderTarget;
				Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
				return;
			}
			Utilities.Dispose<RenderTarget2D>(ref this.m_scalingRenderTarget);
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x000D649C File Offset: 0x000D469C
		public void ApplyScalingRenderTarget(Widget.DrawContext dc)
		{
			if (this.m_scalingRenderTarget != null)
			{
				BlendState blendState = (base.GlobalColorTransform.A < byte.MaxValue) ? BlendState.AlphaBlend : BlendState.Opaque;
				TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(this.m_scalingRenderTarget, false, 0, DepthStencilState.None, RasterizerState.CullNoneScissor, blendState, SamplerState.PointClamp);
				int count = texturedBatch2D.TriangleVertices.Count;
				texturedBatch2D.QueueQuad(Vector2.Zero, base.ActualSize, 0f, Vector2.Zero, Vector2.One, base.GlobalColorTransform);
				texturedBatch2D.TransformTriangles(base.GlobalTransform, count, -1);
				dc.PrimitivesRenderer2D.Flush(true, int.MaxValue);
			}
		}

		// Token: 0x06001B83 RID: 7043 RVA: 0x000D654C File Offset: 0x000D474C
		public void DrawToScreen(Widget.DrawContext dc)
		{
			this.GameWidget.ActiveCamera.PrepareForDrawing(null);
			RenderTarget2D renderTarget = Display.RenderTarget;
			this.SetupScalingRenderTarget();
			try
			{
				this.m_subsystemDrawing.Draw(this.GameWidget.ActiveCamera);
			}
			finally
			{
				Display.RenderTarget = renderTarget;
			}
			this.ApplyScalingRenderTarget(dc);
		}

		// Token: 0x04001306 RID: 4870
		public SubsystemDrawing m_subsystemDrawing;

		// Token: 0x04001307 RID: 4871
		public RenderTarget2D m_scalingRenderTarget;
	}
}
