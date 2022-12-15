using System;
using System.Linq;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F4 RID: 500
	public class ComponentModel : Component
	{
		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000E9C RID: 3740 RVA: 0x000712FF File Offset: 0x0006F4FF
		// (set) Token: 0x06000E9D RID: 3741 RVA: 0x00071307 File Offset: 0x0006F507
		public float? Opacity { get; set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000E9E RID: 3742 RVA: 0x00071310 File Offset: 0x0006F510
		// (set) Token: 0x06000E9F RID: 3743 RVA: 0x00071318 File Offset: 0x0006F518
		public Vector3? DiffuseColor { get; set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x00071321 File Offset: 0x0006F521
		// (set) Token: 0x06000EA1 RID: 3745 RVA: 0x00071329 File Offset: 0x0006F529
		public Vector4? EmissionColor { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x00071332 File Offset: 0x0006F532
		// (set) Token: 0x06000EA3 RID: 3747 RVA: 0x0007133A File Offset: 0x0006F53A
		public Model Model
		{
			get
			{
				return this.m_model;
			}
			set
			{
				this.SetModel(value);
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x00071343 File Offset: 0x0006F543
		// (set) Token: 0x06000EA5 RID: 3749 RVA: 0x0007134B File Offset: 0x0006F54B
		public Texture2D TextureOverride { get; set; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x00071354 File Offset: 0x0006F554
		// (set) Token: 0x06000EA7 RID: 3751 RVA: 0x0007135C File Offset: 0x0006F55C
		public bool CastsShadow { get; set; }

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000EA8 RID: 3752 RVA: 0x00071365 File Offset: 0x0006F565
		// (set) Token: 0x06000EA9 RID: 3753 RVA: 0x0007136D File Offset: 0x0006F56D
		public int PrepareOrder { get; set; }

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000EAA RID: 3754 RVA: 0x00071376 File Offset: 0x0006F576
		// (set) Token: 0x06000EAB RID: 3755 RVA: 0x0007137E File Offset: 0x0006F57E
		public virtual ModelRenderingMode RenderingMode { get; set; }

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000EAC RID: 3756 RVA: 0x00071387 File Offset: 0x0006F587
		// (set) Token: 0x06000EAD RID: 3757 RVA: 0x0007138F File Offset: 0x0006F58F
		public int[] MeshDrawOrders { get; set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000EAE RID: 3758 RVA: 0x00071398 File Offset: 0x0006F598
		// (set) Token: 0x06000EAF RID: 3759 RVA: 0x000713A0 File Offset: 0x0006F5A0
		public bool IsVisibleForCamera { get; set; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000EB0 RID: 3760 RVA: 0x000713A9 File Offset: 0x0006F5A9
		// (set) Token: 0x06000EB1 RID: 3761 RVA: 0x000713B1 File Offset: 0x0006F5B1
		public Matrix[] AbsoluteBoneTransformsForCamera { get; set; }

		// Token: 0x06000EB2 RID: 3762 RVA: 0x000713BA File Offset: 0x0006F5BA
		public Matrix? GetBoneTransform(int boneIndex)
		{
			return this.m_boneTransforms[boneIndex];
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x000713C8 File Offset: 0x0006F5C8
		public void SetBoneTransform(int boneIndex, Matrix? transformation)
		{
			this.m_boneTransforms[boneIndex] = transformation;
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x000713D7 File Offset: 0x0006F5D7
		public void CalculateAbsoluteBonesTransforms(Camera camera)
		{
			this.ProcessBoneHierarchy(this.Model.RootBone, camera.ViewMatrix, this.AbsoluteBoneTransformsForCamera);
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x000713F8 File Offset: 0x0006F5F8
		public virtual void CalculateIsVisible(Camera camera)
		{
			if (camera.GameWidget.IsEntityFirstPersonTarget(base.Entity))
			{
				this.IsVisibleForCamera = false;
				return;
			}
			float num = MathUtils.Sqr(this.m_subsystemSky.VisibilityRange);
			Vector3 vector = this.m_componentFrame.Position - camera.ViewPosition;
			vector.Y *= this.m_subsystemSky.VisibilityRangeYMultiplier;
			if (vector.LengthSquared() < num)
			{
				BoundingSphere sphere = new BoundingSphere(this.m_componentFrame.Position, this.m_boundingSphereRadius);
				this.IsVisibleForCamera = camera.ViewFrustum.Intersection(sphere);
				return;
			}
			this.IsVisibleForCamera = false;
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x0007149A File Offset: 0x0006F69A
		public virtual void Animate()
		{
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x0007149C File Offset: 0x0006F69C
		public virtual void DrawExtras(Camera camera)
		{
		}

        // Token: 0x06000EB8 RID: 3768 RVA: 0x000714A0 File Offset: 0x0006F6A0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentFrame = base.Entity.FindComponent<ComponentFrame>(true);
			string value = valuesDictionary.GetValue<string>("ModelName");
			this.Model = ContentManager.Get<Model>(value);
			this.CastsShadow = valuesDictionary.GetValue<bool>("CastsShadow");
			string value2 = valuesDictionary.GetValue<string>("TextureOverride");
			this.TextureOverride = (string.IsNullOrEmpty(value2) ? null : ContentManager.Get<Texture2D>(value2));
			this.PrepareOrder = valuesDictionary.GetValue<int>("PrepareOrder");
			this.m_boundingSphereRadius = valuesDictionary.GetValue<float>("BoundingSphereRadius");
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x00071540 File Offset: 0x0006F740
		public virtual void SetModel(Model model)
		{
			this.m_model = model;
			if (this.m_model != null)
			{
				this.m_boneTransforms = new Matrix?[this.m_model.Bones.Count];
				this.AbsoluteBoneTransformsForCamera = new Matrix[this.m_model.Bones.Count];
				this.MeshDrawOrders = Enumerable.Range(0, this.m_model.Meshes.Count).ToArray<int>();
				return;
			}
			this.m_boneTransforms = null;
			this.AbsoluteBoneTransformsForCamera = null;
			this.MeshDrawOrders = null;
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x000715D4 File Offset: 0x0006F7D4
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

		// Token: 0x0400094C RID: 2380
		public SubsystemSky m_subsystemSky;

		// Token: 0x0400094D RID: 2381
		public ComponentFrame m_componentFrame;

		// Token: 0x0400094E RID: 2382
		public Model m_model;

		// Token: 0x0400094F RID: 2383
		public Matrix?[] m_boneTransforms;

		// Token: 0x04000950 RID: 2384
		public float m_boundingSphereRadius;
	}
}
