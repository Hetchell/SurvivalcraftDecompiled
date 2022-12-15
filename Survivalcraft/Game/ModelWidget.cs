using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	// Token: 0x02000390 RID: 912
	public class ModelWidget : Widget
	{
		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x000CFEE9 File Offset: 0x000CE0E9
		// (set) Token: 0x06001A69 RID: 6761 RVA: 0x000CFEF1 File Offset: 0x000CE0F1
		public Vector2 Size { get; set; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001A6A RID: 6762 RVA: 0x000CFEFA File Offset: 0x000CE0FA
		// (set) Token: 0x06001A6B RID: 6763 RVA: 0x000CFF02 File Offset: 0x000CE102
		public Color Color { get; set; }

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06001A6C RID: 6764 RVA: 0x000CFF0B File Offset: 0x000CE10B
		// (set) Token: 0x06001A6D RID: 6765 RVA: 0x000CFF13 File Offset: 0x000CE113
		public bool UseAlphaThreshold { get; set; }

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001A6E RID: 6766 RVA: 0x000CFF1C File Offset: 0x000CE11C
		// (set) Token: 0x06001A6F RID: 6767 RVA: 0x000CFF24 File Offset: 0x000CE124
		public bool IsPerspective { get; set; }

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001A70 RID: 6768 RVA: 0x000CFF2D File Offset: 0x000CE12D
		// (set) Token: 0x06001A71 RID: 6769 RVA: 0x000CFF35 File Offset: 0x000CE135
		public Vector3 OrthographicFrustumSize { get; set; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001A72 RID: 6770 RVA: 0x000CFF3E File Offset: 0x000CE13E
		// (set) Token: 0x06001A73 RID: 6771 RVA: 0x000CFF46 File Offset: 0x000CE146
		public Vector3 ViewPosition { get; set; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001A74 RID: 6772 RVA: 0x000CFF4F File Offset: 0x000CE14F
		// (set) Token: 0x06001A75 RID: 6773 RVA: 0x000CFF57 File Offset: 0x000CE157
		public Vector3 ViewTarget { get; set; }

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06001A76 RID: 6774 RVA: 0x000CFF60 File Offset: 0x000CE160
		// (set) Token: 0x06001A77 RID: 6775 RVA: 0x000CFF68 File Offset: 0x000CE168
		public float ViewFov { get; set; }

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001A78 RID: 6776 RVA: 0x000CFF71 File Offset: 0x000CE171
		// (set) Token: 0x06001A79 RID: 6777 RVA: 0x000CFF79 File Offset: 0x000CE179
		public Matrix ModelMatrix { get; set; } = Matrix.Identity;

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001A7A RID: 6778 RVA: 0x000CFF82 File Offset: 0x000CE182
		// (set) Token: 0x06001A7B RID: 6779 RVA: 0x000CFF8A File Offset: 0x000CE18A
		public Vector3 AutoRotationVector { get; set; }

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001A7C RID: 6780 RVA: 0x000CFF93 File Offset: 0x000CE193
		// (set) Token: 0x06001A7D RID: 6781 RVA: 0x000CFF9C File Offset: 0x000CE19C
		public Model Model
		{
			get
			{
				return this.m_model;
			}
			set
			{
				if (value != this.m_model)
				{
					this.m_model = value;
					if (this.m_model != null)
					{
						this.m_boneTransforms = new Matrix?[this.m_model.Bones.Count];
						this.m_absoluteBoneTransforms = new Matrix[this.m_model.Bones.Count];
						return;
					}
					this.m_boneTransforms = null;
					this.m_absoluteBoneTransforms = null;
				}
			}
		}

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001A7E RID: 6782 RVA: 0x000D000C File Offset: 0x000CE20C
		// (set) Token: 0x06001A7F RID: 6783 RVA: 0x000D0014 File Offset: 0x000CE214
		public Texture2D TextureOverride { get; set; }

		// Token: 0x06001A80 RID: 6784 RVA: 0x000D0020 File Offset: 0x000CE220
		public ModelWidget()
		{
			this.Size = new Vector2(float.PositiveInfinity);
			this.IsHitTestVisible = false;
			this.Color = Color.White;
			this.UseAlphaThreshold = false;
			this.IsPerspective = true;
			this.ViewPosition = new Vector3(0f, 0f, -5f);
			this.ViewTarget = new Vector3(0f, 0f, 0f);
			this.ViewFov = 1f;
			this.OrthographicFrustumSize = new Vector3(0f, 10f, 10f);
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x000D00C7 File Offset: 0x000CE2C7
		public Matrix? GetBoneTransform(int boneIndex)
		{
			return this.m_boneTransforms[boneIndex];
		}

		// Token: 0x06001A82 RID: 6786 RVA: 0x000D00D5 File Offset: 0x000CE2D5
		public void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			this.m_boneTransforms[boneIndex] = transformation;
		}

		// Token: 0x06001A83 RID: 6787 RVA: 0x000D00E4 File Offset: 0x000CE2E4
		public override void Draw(Widget.DrawContext dc)
		{
			if (this.Model == null)
			{
				return;
			}
			LitShader litShader = this.UseAlphaThreshold ? ModelWidget.m_shaderAlpha : ModelWidget.m_shader;
			litShader.Texture = this.TextureOverride;
			litShader.SamplerState = SamplerState.PointClamp;
			litShader.MaterialColor = new Vector4(this.Color * base.GlobalColorTransform);
			litShader.AmbientLightColor = new Vector3(0.66f, 0.66f, 0.66f);
			litShader.DiffuseLightColor1 = new Vector3(1f, 1f, 1f);
			litShader.LightDirection1 = Vector3.Normalize(new Vector3(1f, 1f, 1f));
			if (this.UseAlphaThreshold)
			{
				litShader.AlphaThreshold = 0f;
			}
			litShader.Transforms.View = Matrix.CreateLookAt(this.ViewPosition, this.ViewTarget, Vector3.UnitY);
			Viewport viewport = Display.Viewport;
			float num = base.ActualSize.X / base.ActualSize.Y;
			if (this.IsPerspective)
			{
				litShader.Transforms.Projection = Matrix.CreatePerspectiveFieldOfView(this.ViewFov, num, 0.1f, 100f) * MatrixUtils.CreateScaleTranslation(0.5f * base.ActualSize.X, -0.5f * base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			}
			else
			{
				Vector3 orthographicFrustumSize = this.OrthographicFrustumSize;
				if (orthographicFrustumSize.X < 0f)
				{
					orthographicFrustumSize.X = orthographicFrustumSize.Y / num;
				}
				else if (orthographicFrustumSize.Y < 0f)
				{
					orthographicFrustumSize.Y = orthographicFrustumSize.X * num;
				}
				litShader.Transforms.Projection = Matrix.CreateOrthographic(orthographicFrustumSize.X, orthographicFrustumSize.Y, 0f, this.OrthographicFrustumSize.Z) * MatrixUtils.CreateScaleTranslation(0.5f * base.ActualSize.X, -0.5f * base.ActualSize.Y, base.ActualSize.X / 2f, base.ActualSize.Y / 2f) * base.GlobalTransform * MatrixUtils.CreateScaleTranslation(2f / (float)viewport.Width, -2f / (float)viewport.Height, -1f, 1f);
			}
			Display.DepthStencilState = DepthStencilState.Default;
			Display.BlendState = BlendState.AlphaBlend;
			Display.RasterizerState = RasterizerState.CullNoneScissor;
			this.ProcessBoneHierarchy(this.Model.RootBone, Matrix.Identity, this.m_absoluteBoneTransforms);
			float num2 = (float)Time.RealTime + (float)(this.GetHashCode() % 1000) / 100f;
			Matrix m = (this.AutoRotationVector.LengthSquared() > 0f) ? Matrix.CreateFromAxisAngle(Vector3.Normalize(this.AutoRotationVector), this.AutoRotationVector.Length() * num2) : Matrix.Identity;
			foreach (ModelMesh modelMesh in this.Model.Meshes)
			{
				litShader.Transforms.World[0] = this.m_absoluteBoneTransforms[modelMesh.ParentBone.Index] * this.ModelMatrix * m;
				foreach (ModelMeshPart modelMeshPart in modelMesh.MeshParts)
				{
					if (modelMeshPart.IndicesCount > 0)
					{
						Display.DrawIndexed(PrimitiveType.TriangleList, litShader, modelMeshPart.VertexBuffer, modelMeshPart.IndexBuffer, modelMeshPart.StartIndex, modelMeshPart.IndicesCount);
					}
				}
			}
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x000D0528 File Offset: 0x000CE728
		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			base.IsDrawRequired = (this.Model != null);
			base.DesiredSize = this.Size;
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x000D0548 File Offset: 0x000CE748
		public void ProcessBoneHierarchy(ModelBone modelBone, Matrix currentTransform, Matrix[] transforms)
		{
			Matrix m = modelBone.Transform;
			if (this.m_boneTransforms[modelBone.Index] != null)
			{
				Vector3 translation = m.Translation;
				m.Translation = Vector3.Zero;
				m *= this.m_boneTransforms[modelBone.Index].Value;
				m.Translation += translation;
				Matrix.MultiplyRestricted(ref m, ref currentTransform, out transforms[modelBone.Index]);
			}
			else
			{
				Matrix.MultiplyRestricted(ref m, ref currentTransform, out transforms[modelBone.Index]);
			}
			foreach (ModelBone modelBone2 in modelBone.ChildBones)
			{
				this.ProcessBoneHierarchy(modelBone2, transforms[modelBone.Index], transforms);
			}
		}

		// Token: 0x0400126F RID: 4719
		public static LitShader m_shader = new LitShader(1, false, false, true, false, false, 1);

		// Token: 0x04001270 RID: 4720
		public static LitShader m_shaderAlpha = new LitShader(1, false, false, true, false, true, 1);

		// Token: 0x04001271 RID: 4721
		public Model m_model;

		// Token: 0x04001272 RID: 4722
		public Matrix?[] m_boneTransforms;

		// Token: 0x04001273 RID: 4723
		public Matrix[] m_absoluteBoneTransforms;
	}
}
