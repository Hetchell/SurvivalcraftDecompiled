using System;
using Engine;
using Engine.Input;

namespace Game
{
	// Token: 0x020003A0 RID: 928
	public class TouchInputWidget : Widget
	{
		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001B50 RID: 6992 RVA: 0x000D51D9 File Offset: 0x000D33D9
		// (set) Token: 0x06001B51 RID: 6993 RVA: 0x000D51E1 File Offset: 0x000D33E1
		public float Radius
		{
			get
			{
				return this.m_radius;
			}
			set
			{
				this.m_radius = MathUtils.Max(value, 1f);
			}
		}

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001B52 RID: 6994 RVA: 0x000D51F4 File Offset: 0x000D33F4
		public TouchInput? TouchInput
		{
			get
			{
				if (base.IsEnabledGlobal && base.IsVisibleGlobal)
				{
					return this.m_touchInput;
				}
				return null;
			}
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x000D5224 File Offset: 0x000D3424
		public override void Update()
		{
			this.m_touchInput = null;
			double frameStartTime = Time.FrameStartTime;
			int frameIndex = Time.FrameIndex;
			foreach (TouchLocation touchLocation in base.Input.TouchLocations)
			{
				if (touchLocation.State == TouchLocationState.Pressed)
				{
					if (base.HitTestGlobal(touchLocation.Position, null) == this)
					{
						this.m_touchId = new int?(touchLocation.Id);
						this.m_touchLastPosition = touchLocation.Position;
						this.m_touchOrigin = touchLocation.Position;
						this.m_touchOriginLimited = touchLocation.Position;
						this.m_touchTime = frameStartTime;
						this.m_touchFrameIndex = frameIndex;
						this.m_touchMoved = false;
					}
				}
				else if (touchLocation.State == TouchLocationState.Moved)
				{
					if (this.m_touchId != null && touchLocation.Id == this.m_touchId.Value)
					{
						this.m_touchMoved |= (Vector2.Distance(touchLocation.Position, this.m_touchOrigin) > SettingsManager.MinimumDragDistance * base.GlobalScale);
						TouchInput touchInput = new TouchInput
						{
							InputType = ((!this.m_touchMoved) ? TouchInputType.Hold : TouchInputType.Move),
							Duration = (float)(frameStartTime - this.m_touchTime),
							DurationFrames = frameIndex - this.m_touchFrameIndex,
							Position = touchLocation.Position,
							Move = touchLocation.Position - this.m_touchLastPosition,
							TotalMove = touchLocation.Position - this.m_touchOrigin,
							TotalMoveLimited = touchLocation.Position - this.m_touchOriginLimited
						};
						if (MathUtils.Abs(touchInput.TotalMoveLimited.X) > this.m_radius)
						{
							this.m_touchOriginLimited.X = touchLocation.Position.X - MathUtils.Sign(touchInput.TotalMoveLimited.X) * this.m_radius;
						}
						if (MathUtils.Abs(touchInput.TotalMoveLimited.Y) > this.m_radius)
						{
							this.m_touchOriginLimited.Y = touchLocation.Position.Y - MathUtils.Sign(touchInput.TotalMoveLimited.Y) * this.m_radius;
						}
						this.m_touchInput = new TouchInput?(touchInput);
						this.m_touchLastPosition = touchLocation.Position;
					}
				}
				else if (touchLocation.State == TouchLocationState.Released && this.m_touchId != null && touchLocation.Id == this.m_touchId.Value)
				{
					if (frameStartTime - this.m_touchTime <= (double)SettingsManager.MinimumHoldDuration && Vector2.Distance(touchLocation.Position, this.m_touchOrigin) < SettingsManager.MinimumDragDistance * base.GlobalScale)
					{
						this.m_touchInput = new TouchInput?(new TouchInput
						{
							InputType = TouchInputType.Tap,
							Duration = (float)(frameStartTime - this.m_touchTime),
							DurationFrames = frameIndex - this.m_touchFrameIndex,
							Position = touchLocation.Position
						});
					}
					this.m_touchId = null;
				}
			}
		}

		// Token: 0x040012ED RID: 4845
		public int? m_touchId;

		// Token: 0x040012EE RID: 4846
		public Vector2 m_touchLastPosition;

		// Token: 0x040012EF RID: 4847
		public Vector2 m_touchOrigin;

		// Token: 0x040012F0 RID: 4848
		public Vector2 m_touchOriginLimited;

		// Token: 0x040012F1 RID: 4849
		public bool m_touchMoved;

		// Token: 0x040012F2 RID: 4850
		public double m_touchTime;

		// Token: 0x040012F3 RID: 4851
		public int m_touchFrameIndex;

		// Token: 0x040012F4 RID: 4852
		public TouchInput? m_touchInput;

		// Token: 0x040012F5 RID: 4853
		public float m_radius = 30f;
	}
}
