using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001AA RID: 426
	public class SubsystemSoundMaterials : Subsystem
	{
		// Token: 0x06000A61 RID: 2657 RVA: 0x0004CEF4 File Offset: 0x0004B0F4
		public void PlayImpactSound(int value, Vector3 position, float loudnessMultiplier)
		{
			int num = Terrain.ExtractContents(value);
			string soundMaterialName = BlocksManager.Blocks[num].GetSoundMaterialName(this.m_subsystemTerrain, value);
			if (!string.IsNullOrEmpty(soundMaterialName))
			{
				string value2 = this.m_impactsSoundsValuesDictionary.GetValue<string>(soundMaterialName, null);
				if (!string.IsNullOrEmpty(value2))
				{
					float pitch = this.m_random.Float(-0.2f, 0.2f);
					this.m_subsystemAudio.PlayRandomSound(value2, 0.5f * loudnessMultiplier, pitch, position, 5f * loudnessMultiplier, true);
				}
			}
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x0004CF70 File Offset: 0x0004B170
		public bool PlayFootstepSound(ComponentCreature componentCreature, float loudnessMultiplier)
		{
			string footstepSoundMaterialName = this.GetFootstepSoundMaterialName(componentCreature);
			if (!string.IsNullOrEmpty(footstepSoundMaterialName))
			{
				string value = componentCreature.ComponentCreatureSounds.ValuesDictionary.GetValue<ValuesDictionary>("CustomFootstepSounds").GetValue<string>(footstepSoundMaterialName, null);
				if (string.IsNullOrEmpty(value))
				{
					value = this.m_footstepSoundsValuesDictionary.GetValue<string>(footstepSoundMaterialName, null);
				}
				if (!string.IsNullOrEmpty(value))
				{
					float pitch = this.m_random.Float(-0.2f, 0.2f);
					this.m_subsystemAudio.PlayRandomSound(value, 0.75f * loudnessMultiplier, pitch, componentCreature.ComponentBody.Position, 2f * loudnessMultiplier, true);
					ComponentPlayer componentPlayer = componentCreature as ComponentPlayer;
					if (componentPlayer != null && componentPlayer.ComponentVitalStats.Wetness > 0f)
					{
						string value2 = this.m_footstepSoundsValuesDictionary.GetValue<string>("Squishy", null);
						if (!string.IsNullOrEmpty(value2))
						{
							float volume = 0.7f * loudnessMultiplier * MathUtils.Pow(componentPlayer.ComponentVitalStats.Wetness, 4f);
							this.m_subsystemAudio.PlayRandomSound(value2, volume, pitch, componentCreature.ComponentBody.Position, 2f * loudnessMultiplier, true);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x0004D088 File Offset: 0x0004B288
		public string GetFootstepSoundMaterialName(ComponentCreature componentCreature)
		{
			Vector3 position = componentCreature.ComponentBody.Position;
			if (componentCreature.ComponentBody.ImmersionDepth > 0.2f && componentCreature.ComponentBody.ImmersionFluidBlock is WaterBlock)
			{
				return "Water";
			}
			if (componentCreature.ComponentLocomotion.LadderValue != null)
			{
				if (Terrain.ExtractContents(componentCreature.ComponentLocomotion.LadderValue.Value) == 59)
				{
					return "WoodenLadder";
				}
				return "MetalLadder";
			}
			else
			{
				int cellValue = this.m_subsystemTerrain.Terrain.GetCellValue(Terrain.ToCell(position.X), Terrain.ToCell(position.Y + 0.1f), Terrain.ToCell(position.Z));
				int num = Terrain.ExtractContents(cellValue);
				string soundMaterialName = BlocksManager.Blocks[num].GetSoundMaterialName(this.m_subsystemTerrain, cellValue);
				if (string.IsNullOrEmpty(soundMaterialName) && componentCreature.ComponentBody.StandingOnValue != null)
				{
					soundMaterialName = BlocksManager.Blocks[Terrain.ExtractContents(componentCreature.ComponentBody.StandingOnValue.Value)].GetSoundMaterialName(this.m_subsystemTerrain, componentCreature.ComponentBody.StandingOnValue.Value);
				}
				if (!string.IsNullOrEmpty(soundMaterialName))
				{
					return soundMaterialName;
				}
				return string.Empty;
			}
		}

        // Token: 0x06000A64 RID: 2660 RVA: 0x0004D1CC File Offset: 0x0004B3CC
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
			this.m_subsystemAudio = base.Project.FindSubsystem<SubsystemAudio>(true);
			this.m_impactsSoundsValuesDictionary = valuesDictionary.GetValue<ValuesDictionary>("ImpactSounds");
			this.m_footstepSoundsValuesDictionary = valuesDictionary.GetValue<ValuesDictionary>("FootstepSounds");
		}

		// Token: 0x040005A9 RID: 1449
		public SubsystemTerrain m_subsystemTerrain;

		// Token: 0x040005AA RID: 1450
		public SubsystemAudio m_subsystemAudio;

		// Token: 0x040005AB RID: 1451
		public Game.Random m_random = new Game.Random();

		// Token: 0x040005AC RID: 1452
		public ValuesDictionary m_impactsSoundsValuesDictionary;

		// Token: 0x040005AD RID: 1453
		public ValuesDictionary m_footstepSoundsValuesDictionary;
	}
}
