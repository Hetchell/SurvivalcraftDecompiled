using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x0200021F RID: 543
	public abstract class BasePerspectiveCamera : Camera
	{
		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060010A1 RID: 4257 RVA: 0x0007E870 File Offset: 0x0007CA70
		public override Vector3 ViewPosition
		{
			get
			{
				return this.m_viewPosition;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060010A2 RID: 4258 RVA: 0x0007E878 File Offset: 0x0007CA78
		public override Vector3 ViewDirection
		{
			get
			{
				return this.m_viewDirection;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060010A3 RID: 4259 RVA: 0x0007E880 File Offset: 0x0007CA80
		public override Vector3 ViewUp
		{
			get
			{
				return this.m_viewUp;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060010A4 RID: 4260 RVA: 0x0007E888 File Offset: 0x0007CA88
		public override Vector3 ViewRight
		{
			get
			{
				return this.m_viewRight;
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060010A5 RID: 4261 RVA: 0x0007E890 File Offset: 0x0007CA90
		public override Matrix ViewMatrix
		{
			get
			{
				if (this.m_viewMatrix == null)
				{
					if (base.Eye == null)
					{
						this.m_viewMatrix = new Matrix?(Matrix.CreateLookAt(this.m_viewPosition, this.m_viewPosition + this.m_viewDirection, this.m_viewUp));
					}
					else
					{
						Matrix eyeToHeadTransform = VrManager.GetEyeToHeadTransform(base.Eye.Value);
						this.m_viewMatrix = new Matrix?(Matrix.CreateLookAt(this.m_viewPosition, this.m_viewPosition + this.m_viewDirection, this.m_viewUp) * Matrix.Invert(eyeToHeadTransform));
					}
				}
				return this.m_viewMatrix.Value;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x0007E943 File Offset: 0x0007CB43
		public override Matrix InvertedViewMatrix
		{
			get
			{
				if (this.m_invertedViewMatrix == null)
				{
					this.m_invertedViewMatrix = new Matrix?(Matrix.Invert(this.ViewMatrix));
				}
				return this.m_invertedViewMatrix.Value;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x060010A7 RID: 4263 RVA: 0x0007E974 File Offset: 0x0007CB74
		public override Matrix ProjectionMatrix
		{
			get
			{
				if (this.m_projectionMatrix == null)
				{
					this.m_projectionMatrix = new Matrix?(this.CalculateBaseProjectionMatrix());
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					if (viewWidget.ScalingRenderTargetSize == null && base.Eye == null)
					{
						this.m_projectionMatrix *= MatrixUtils.CreateScaleTranslation(0.5f * viewWidget.ActualSize.X, -0.5f * viewWidget.ActualSize.Y, viewWidget.ActualSize.X / 2f, viewWidget.ActualSize.Y / 2f) * viewWidget.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)Display.Viewport.Width, -2f / (float)Display.Viewport.Height, -1f, 1f);
					}
				}
				return this.m_projectionMatrix.Value;
			}
		}

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x060010A8 RID: 4264 RVA: 0x0007EAA0 File Offset: 0x0007CCA0
		public override Matrix ScreenProjectionMatrix
		{
			get
			{
				if (this.m_screenProjectionMatrix == null)
				{
					if (base.Eye == null)
					{
						Point2 size = Window.Size;
						ViewWidget viewWidget = base.GameWidget.ViewWidget;
						this.m_screenProjectionMatrix = new Matrix?(this.CalculateBaseProjectionMatrix() * MatrixUtils.CreateScaleTranslation(0.5f * viewWidget.ActualSize.X, -0.5f * viewWidget.ActualSize.Y, viewWidget.ActualSize.X / 2f, viewWidget.ActualSize.Y / 2f) * viewWidget.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)size.X, -2f / (float)size.Y, -1f, 1f));
					}
					else
					{
						this.m_screenProjectionMatrix = new Matrix?(this.CalculateBaseProjectionMatrix());
					}
				}
				return this.m_screenProjectionMatrix.Value;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x060010A9 RID: 4265 RVA: 0x0007EB98 File Offset: 0x0007CD98
		public override Matrix InvertedProjectionMatrix
		{
			get
			{
				if (this.m_invertedProjectionMatrix == null)
				{
					this.m_invertedProjectionMatrix = new Matrix?(Matrix.Invert(this.ProjectionMatrix));
				}
				return this.m_invertedProjectionMatrix.Value;
			}
		}

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x060010AA RID: 4266 RVA: 0x0007EBC8 File Offset: 0x0007CDC8
		public override Matrix ViewProjectionMatrix
		{
			get
			{
				if (this.m_viewProjectionMatrix == null)
				{
					this.m_viewProjectionMatrix = new Matrix?(this.ViewMatrix * this.ProjectionMatrix);
				}
				return this.m_viewProjectionMatrix.Value;
			}
		}

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060010AB RID: 4267 RVA: 0x0007EC00 File Offset: 0x0007CE00
		public override Vector2 ViewportSize
		{
			get
			{
				if (this.m_viewportSize == null)
				{
					ViewWidget viewWidget = base.GameWidget.ViewWidget;
					if (viewWidget.ScalingRenderTargetSize != null)
					{
						this.m_viewportSize = new Vector2?(new Vector2(viewWidget.ScalingRenderTargetSize.Value));
					}
					else if (base.Eye == null)
					{
						this.m_viewportSize = new Vector2?(new Vector2(viewWidget.ActualSize.X * viewWidget.GlobalTransform.Right.Length(), viewWidget.ActualSize.Y * viewWidget.GlobalTransform.Up.Length()));
					}
					else
					{
						this.m_viewportSize = new Vector2?(new Vector2((float)VrManager.VrRenderTarget.Width, (float)VrManager.VrRenderTarget.Height));
					}
				}
				return this.m_viewportSize.Value;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060010AC RID: 4268 RVA: 0x0007ECF8 File Offset: 0x0007CEF8
		public override Matrix ViewportMatrix
		{
			get
			{
				if (this.m_viewportMatrix == null)
				{
					if (base.Eye == null)
					{
						ViewWidget viewWidget = base.GameWidget.ViewWidget;
						if (viewWidget.ScalingRenderTargetSize != null)
						{
							this.m_viewportMatrix = new Matrix?(Matrix.Identity);
						}
						else
						{
							Matrix identity = Matrix.Identity;
							identity.Right = Vector3.Normalize(viewWidget.GlobalTransform.Right);
							identity.Up = Vector3.Normalize(viewWidget.GlobalTransform.Up);
							identity.Forward = viewWidget.GlobalTransform.Forward;
							identity.Translation = viewWidget.GlobalTransform.Translation;
							this.m_viewportMatrix = new Matrix?(identity);
						}
					}
					else
					{
						this.m_viewportMatrix = new Matrix?(Matrix.Identity);
					}
				}
				return this.m_viewportMatrix.Value;
			}
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060010AD RID: 4269 RVA: 0x0007EDEC File Offset: 0x0007CFEC
		public override BoundingFrustum ViewFrustum
		{
			get
			{
				if (!this.m_viewFrustumValid)
				{
					if (this.m_viewFrustum == null)
					{
						this.m_viewFrustum = new BoundingFrustum(this.ViewProjectionMatrix);
					}
					else
					{
						this.m_viewFrustum.Matrix = this.ViewProjectionMatrix;
					}
					this.m_viewFrustumValid = true;
				}
				return this.m_viewFrustum;
			}
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x0007EE40 File Offset: 0x0007D040
		public override void PrepareForDrawing(VrEye? eye)
		{
			base.PrepareForDrawing(eye);
			this.m_viewMatrix = null;
			this.m_invertedViewMatrix = null;
			this.m_projectionMatrix = null;
			this.m_invertedProjectionMatrix = null;
			this.m_screenProjectionMatrix = null;
			this.m_viewProjectionMatrix = null;
			this.m_viewportSize = null;
			this.m_viewportMatrix = null;
			this.m_viewFrustumValid = false;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x0007EEBB File Offset: 0x0007D0BB
		public BasePerspectiveCamera(GameWidget gameWidget) : base(gameWidget)
		{
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x0007EEC4 File Offset: 0x0007D0C4
		public void SetupPerspectiveCamera(Vector3 position, Vector3 direction, Vector3 up)
		{
			this.m_viewPosition = position;
			this.m_viewDirection = Vector3.Normalize(direction);
			this.m_viewUp = Vector3.Normalize(up);
			this.m_viewRight = Vector3.Normalize(Vector3.Cross(this.m_viewDirection, this.m_viewUp));
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x0007EF04 File Offset: 0x0007D104
		public Matrix CalculateBaseProjectionMatrix()
		{
			if (base.Eye == null)
			{
				float num = 90f;
				float num2 = 1f;
				if (SettingsManager.ViewAngleMode == ViewAngleMode.Narrow)
				{
					num2 = 0.8f;
				}
				else if (SettingsManager.ViewAngleMode == ViewAngleMode.Normal)
				{
					num2 = 0.9f;
				}
				ViewWidget viewWidget = base.GameWidget.ViewWidget;
				float num3 = viewWidget.ActualSize.X / viewWidget.ActualSize.Y;
				float num4 = MathUtils.Min(num * num3, num);
				float num5 = num4 * num3;
				if (num5 < 90f)
				{
					num4 *= 90f / num5;
				}
				else if (num5 > 175f)
				{
					num4 *= 175f / num5;
				}
				return Matrix.CreatePerspectiveFieldOfView(MathUtils.DegToRad(num4 * num2), num3, 0.1f, 2048f);
			}
			return VrManager.GetProjectionMatrix(base.Eye.Value, 0.1f, 2048f);
		}

		// Token: 0x04000ADA RID: 2778
		public Vector3 m_viewPosition;

		// Token: 0x04000ADB RID: 2779
		public Vector3 m_viewDirection;

		// Token: 0x04000ADC RID: 2780
		public Vector3 m_viewUp;

		// Token: 0x04000ADD RID: 2781
		public Vector3 m_viewRight;

		// Token: 0x04000ADE RID: 2782
		public Matrix? m_viewMatrix;

		// Token: 0x04000ADF RID: 2783
		public Matrix? m_invertedViewMatrix;

		// Token: 0x04000AE0 RID: 2784
		public Matrix? m_projectionMatrix;

		// Token: 0x04000AE1 RID: 2785
		public Matrix? m_invertedProjectionMatrix;

		// Token: 0x04000AE2 RID: 2786
		public Matrix? m_screenProjectionMatrix;

		// Token: 0x04000AE3 RID: 2787
		public Matrix? m_viewProjectionMatrix;

		// Token: 0x04000AE4 RID: 2788
		public Vector2? m_viewportSize;

		// Token: 0x04000AE5 RID: 2789
		public Matrix? m_viewportMatrix;

		// Token: 0x04000AE6 RID: 2790
		public BoundingFrustum m_viewFrustum;

		// Token: 0x04000AE7 RID: 2791
		public bool m_viewFrustumValid;
	}
}
