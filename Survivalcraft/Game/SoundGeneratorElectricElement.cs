using System;
using Engine;

namespace Game
{
	// Token: 0x020002F8 RID: 760
	public class SoundGeneratorElectricElement : RotateableElectricElement
	{
		// Token: 0x0600159B RID: 5531 RVA: 0x000A4AAC File Offset: 0x000A2CAC
		public SoundGeneratorElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace)
		{
			this.m_subsystemNoise = subsystemElectricity.Project.FindSubsystem<SubsystemNoise>(true);
			this.m_subsystemParticles = subsystemElectricity.Project.FindSubsystem<SubsystemParticles>(true);
			Vector3 vector = CellFace.FaceToVector3(cellFace.Face);
			Vector3 position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.2f * vector;
			this.m_particleSystem = new SoundParticleSystem(subsystemElectricity.SubsystemTerrain, position, vector);
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x000A4C48 File Offset: 0x000A2E48
		public override bool Simulate()
		{
			int num = 0;
			int num2 = 15;
			int num3 = 2;
			int num4 = 0;
			int rotation = base.Rotation;
			foreach (ElectricConnection electricConnection in base.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(base.CellFaces[0].Face, rotation, electricConnection.ConnectorFace);
					if (connectorDirection != null)
					{
						if (connectorDirection.Value == ElectricConnectorDirection.In || connectorDirection.Value == ElectricConnectorDirection.Bottom)
						{
							num4 = (int)MathUtils.Round(15f * electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else if (connectorDirection.Value == ElectricConnectorDirection.Left)
						{
							num = (int)MathUtils.Round(15f * electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else if (connectorDirection.Value == ElectricConnectorDirection.Right)
						{
							num3 = (int)MathUtils.Round(15f * electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
						else if (connectorDirection.Value == ElectricConnectorDirection.Top)
						{
							num2 = (int)MathUtils.Round(15f * electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace));
						}
					}
				}
			}
			if (this.m_lastToneInput == 0 && num4 != 0 && num != 15 && base.SubsystemElectricity.SubsystemTime.GameTime >= this.m_playAllowedTime)
			{
				this.m_playAllowedTime = base.SubsystemElectricity.SubsystemTime.GameTime + 0.07999999821186066;
				string text = this.m_tones[num4];
				float num5 = 0f;
				string text2 = null;
				if (text == "Drums")
				{
					num5 = 1f;
					if (num >= 0 && num < this.m_drums.Length)
					{
						text2 = "Audio/SoundGenerator/Drums" + this.m_drums[num];
					}
				}
				else if (!string.IsNullOrEmpty(text))
				{
					float num6 = 130.8125f * MathUtils.Pow(1.0594631f, (float)(num + 12 * num3));
					int num7 = 0;
					for (int i = 4; i <= this.m_maxOctaves[num4]; i++)
					{
						float num8 = num6 / (523.25f * MathUtils.Pow(2f, (float)(i - 5)));
						if (num7 == 0 || (num8 >= 0.5f && num8 < num5))
						{
							num7 = i;
							num5 = num8;
						}
					}
					text2 = string.Format("Audio/SoundGenerator/{0}C{1}", text, num7);
				}
				if (num5 != 0f && !string.IsNullOrEmpty(text2))
				{
					CellFace cellFace = base.CellFaces[0];
					Vector3 position = new Vector3((float)cellFace.X, (float)cellFace.Y, (float)cellFace.Z);
					float volume = (float)num2 / 15f;
					float pitch = MathUtils.Clamp(MathUtils.Log(num5) / MathUtils.Log(2f), -1f, 1f);
					float minDistance = 0.5f + 5f * (float)num2 / 15f;
					base.SubsystemElectricity.SubsystemAudio.PlaySound(text2, volume, pitch, position, minDistance, true);
					float loudness = (num2 < 8) ? 0.25f : 0.5f;
					float range = MathUtils.Lerp(2f, 20f, (float)num2 / 15f);
					this.m_subsystemNoise.MakeNoise(position, loudness, range);
					if (this.m_particleSystem.SubsystemParticles == null)
					{
						this.m_subsystemParticles.AddParticleSystem(this.m_particleSystem);
					}
					Vector3 hsv = new Vector3(22.5f * (float)num + this.m_random.Float(0f, 22f), 0.5f + (float)num2 / 30f, 1f);
					this.m_particleSystem.AddNote(new Color(Color.HsvToRgb(hsv)));
				}
			}
			this.m_lastToneInput = num4;
			return false;
		}

		// Token: 0x04000F53 RID: 3923
		public SubsystemNoise m_subsystemNoise;

		// Token: 0x04000F54 RID: 3924
		public SubsystemParticles m_subsystemParticles;

		// Token: 0x04000F55 RID: 3925
		public SoundParticleSystem m_particleSystem;

		// Token: 0x04000F56 RID: 3926
		public Game.Random m_random = new Game.Random();

		// Token: 0x04000F57 RID: 3927
		public int m_lastToneInput;

		// Token: 0x04000F58 RID: 3928
		public double m_playAllowedTime;

		// Token: 0x04000F59 RID: 3929
		public string[] m_tones = new string[]
		{
			"",
			"Bell",
			"Organ",
			"Ping",
			"String",
			"Trumpet",
			"Voice",
			"Piano",
			"PianoLong",
			"Drums",
			"",
			"",
			"",
			"",
			"",
			"Piano"
		};

		// Token: 0x04000F5A RID: 3930
		public int[] m_maxOctaves = new int[]
		{
			0,
			5,
			5,
			5,
			5,
			5,
			5,
			6,
			6,
			0,
			0,
			0,
			0,
			0,
			0,
			6
		};

		// Token: 0x04000F5B RID: 3931
		public string[] m_drums = new string[]
		{
			"Snare",
			"BassDrum",
			"ClosedHiHat",
			"PedalHiHat",
			"OpenHiHat",
			"LowTom",
			"HighTom",
			"CrashCymbal",
			"RideCymbal",
			"HandClap"
		};
	}
}
