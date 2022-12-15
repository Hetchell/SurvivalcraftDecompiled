using System;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001F9 RID: 505
	public class ComponentOuterClothingModel : ComponentModel
	{
        // Token: 0x06000EE2 RID: 3810 RVA: 0x000721EE File Offset: 0x000703EE
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			this.m_subsystemSky = base.Project.FindSubsystem<SubsystemSky>(true);
			this.m_componentHumanModel = base.Entity.FindComponent<ComponentHumanModel>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x00072230 File Offset: 0x00070430
		public override void Animate()
		{
			base.Opacity = this.m_componentHumanModel.Opacity;
			foreach (ModelBone modelBone in base.Model.Bones)
			{
				ModelBone modelBone2 = this.m_componentHumanModel.Model.FindBone(modelBone.Name, true);
				base.SetBoneTransform(modelBone.Index, this.m_componentHumanModel.GetBoneTransform(modelBone2.Index));
			}
			if (base.Opacity != null && base.Opacity.Value < 1f)
			{
				bool flag = this.m_componentCreature.ComponentBody.ImmersionFactor >= 1f;
				bool flag2 = this.m_subsystemSky.ViewUnderWaterDepth > 0f;
				if (flag == flag2)
				{
					this.RenderingMode = ModelRenderingMode.TransparentAfterWater;
				}
				else
				{
					this.RenderingMode = ModelRenderingMode.TransparentBeforeWater;
				}
			}
			else
			{
				this.RenderingMode = ModelRenderingMode.AlphaThreshold;
			}
			base.Animate();
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x00072344 File Offset: 0x00070544
		public override void SetModel(Model model)
		{
			base.SetModel(model);
			if (base.MeshDrawOrders.Length != 4)
			{
				throw new InvalidOperationException("Invalid number of meshes in OuterClothing model.");
			}
			base.MeshDrawOrders[0] = model.Meshes.IndexOf(model.FindMesh("Leg1", true));
			base.MeshDrawOrders[1] = model.Meshes.IndexOf(model.FindMesh("Leg2", true));
			base.MeshDrawOrders[2] = model.Meshes.IndexOf(model.FindMesh("Body", true));
			base.MeshDrawOrders[3] = model.Meshes.IndexOf(model.FindMesh("Head", true));
		}

		// Token: 0x04000974 RID: 2420
		public new SubsystemSky m_subsystemSky;

		// Token: 0x04000975 RID: 2421
		public ComponentHumanModel m_componentHumanModel;

		// Token: 0x04000976 RID: 2422
		public ComponentCreature m_componentCreature;
	}
}
