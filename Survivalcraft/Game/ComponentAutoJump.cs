using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001BD RID: 445
	public class ComponentAutoJump : Component, IUpdateable
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000B28 RID: 2856 RVA: 0x0005331B File Offset: 0x0005151B
		public UpdateOrder UpdateOrder
		{
			get
			{
				return UpdateOrder.Default;
			}
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x00053320 File Offset: 0x00051520
		public void Update(float dt)
		{
			if ((SettingsManager.AutoJump || this.m_alwaysEnabled) && this.m_subsystemTime.GameTime - this.m_lastAutoJumpTime > 0.25)
			{
				Vector2? lastWalkOrder = this.m_componentCreature.ComponentLocomotion.LastWalkOrder;
				if (lastWalkOrder != null)
				{
					Vector2 vector = new Vector2(this.m_componentCreature.ComponentBody.CollisionVelocityChange.X, this.m_componentCreature.ComponentBody.CollisionVelocityChange.Z);
					if (vector != Vector2.Zero && !this.m_collidedWithBody)
					{
						Vector2 v = Vector2.Normalize(vector);
						Vector3 vector2 = this.m_componentCreature.ComponentBody.Matrix.Right * lastWalkOrder.Value.X + this.m_componentCreature.ComponentBody.Matrix.Forward * lastWalkOrder.Value.Y;
						Vector2 v2 = Vector2.Normalize(new Vector2(vector2.X, vector2.Z));
						bool flag = false;
						Vector3 vector3 = Vector3.Zero;
						Vector3 vector4 = Vector3.Zero;
						Vector3 vector5 = Vector3.Zero;
						if (Vector2.Dot(v2, -v) > 0.6f)
						{
							if (Vector2.Dot(v2, Vector2.UnitX) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position + Vector3.UnitX;
								vector4 = vector3 - Vector3.UnitZ;
								vector5 = vector3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitX) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position - Vector3.UnitX;
								vector4 = vector3 - Vector3.UnitZ;
								vector5 = vector3 + Vector3.UnitZ;
								flag = true;
							}
							else if (Vector2.Dot(v2, Vector2.UnitY) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position + Vector3.UnitZ;
								vector4 = vector3 - Vector3.UnitX;
								vector5 = vector3 + Vector3.UnitX;
								flag = true;
							}
							else if (Vector2.Dot(v2, -Vector2.UnitY) > 0.6f)
							{
								vector3 = this.m_componentCreature.ComponentBody.Position - Vector3.UnitZ;
								vector4 = vector3 - Vector3.UnitX;
								vector5 = vector3 + Vector3.UnitX;
								flag = true;
							}
						}
						if (flag)
						{
							int cellContents = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y), Terrain.ToCell(vector3.Z));
							int cellContents2 = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y), Terrain.ToCell(vector4.Z));
							int cellContents3 = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector5.X), Terrain.ToCell(vector5.Y), Terrain.ToCell(vector5.Z));
							int cellContents4 = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector3.X), Terrain.ToCell(vector3.Y) + 1, Terrain.ToCell(vector3.Z));
							int cellContents5 = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector4.X), Terrain.ToCell(vector4.Y) + 1, Terrain.ToCell(vector4.Z));
							int cellContents6 = this.m_subsystemTerrain.Terrain.GetCellContents(Terrain.ToCell(vector5.X), Terrain.ToCell(vector5.Y) + 1, Terrain.ToCell(vector5.Z));
							Block block = BlocksManager.Blocks[cellContents];
							Block block2 = BlocksManager.Blocks[cellContents2];
							Block block3 = BlocksManager.Blocks[cellContents3];
							Block block4 = BlocksManager.Blocks[cellContents4];
							Block block5 = BlocksManager.Blocks[cellContents5];
							Block block6 = BlocksManager.Blocks[cellContents6];
							if (!block.NoAutoJump && ((block.IsCollidable && !block4.IsCollidable) || (block2.IsCollidable && !block5.IsCollidable) || (block3.IsCollidable && !block6.IsCollidable)))
							{
								this.m_componentCreature.ComponentLocomotion.JumpOrder = MathUtils.Max(this.m_jumpStrength, this.m_componentCreature.ComponentLocomotion.JumpOrder);
								this.m_lastAutoJumpTime = this.m_subsystemTime.GameTime;
							}
						}
					}
				}
			}
			this.m_collidedWithBody = false;
		}

        // Token: 0x06000B2A RID: 2858 RVA: 0x000537D0 File Offset: 0x000519D0
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
			this.m_componentCreature = base.Entity.FindComponent<ComponentCreature>(true);
			this.m_alwaysEnabled = valuesDictionary.GetValue<bool>("AlwaysEnabled");
			this.m_jumpStrength = valuesDictionary.GetValue<float>("JumpStrength");
			this.m_componentCreature.ComponentBody.CollidedWithBody += delegate (ComponentBody p)
			{
				this.m_collidedWithBody = true;
			};
		}

		// Token: 0x0400061F RID: 1567
		public SubsystemTime m_subsystemTime;

		// Token: 0x04000620 RID: 1568
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x04000621 RID: 1569
		public ComponentCreature m_componentCreature;

		// Token: 0x04000622 RID: 1570
		public double m_lastAutoJumpTime;

		// Token: 0x04000623 RID: 1571
		public bool m_alwaysEnabled;

		// Token: 0x04000624 RID: 1572
		public float m_jumpStrength;

		// Token: 0x04000625 RID: 1573
		public bool m_collidedWithBody;
	}
}
