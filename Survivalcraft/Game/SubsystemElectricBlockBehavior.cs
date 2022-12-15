using System;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x02000175 RID: 373
	public class SubsystemElectricBlockBehavior : SubsystemBlockBehavior
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600080A RID: 2058 RVA: 0x00034965 File Offset: 0x00032B65
		public override int[] HandledBlocks
		{
			get
			{
				return new int[]
				{
					133,
					140,
					137,
					143,
					156,
					134,
					135,
					145,
					224,
					146,
					157,
					180,
					181,
					183,
					138,
					139,
					141,
					142,
					184,
					187,
					186,
					188,
					144,
					151,
					179,
					152,
					254,
					253,
					182,
					185,
					56,
					57,
					58,
					83,
					84,
					166,
					194,
					86,
					63,
					97,
					98,
					210,
					211,
					105,
					106,
					107,
					234,
					235,
					236,
					147,
					153,
					154,
					223,
					155,
					243,
					120,
					121,
					199,
					216,
					227,
					237
				};
			}
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00034979 File Offset: 0x00032B79
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			this.m_subsystemElectricity.OnElectricElementBlockGenerated(x, y, z);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0003498A File Offset: 0x00032B8A
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockAdded(x, y, z);
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0003499C File Offset: 0x00032B9C
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockRemoved(x, y, z);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x000349AE File Offset: 0x00032BAE
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			this.m_subsystemElectricity.OnElectricElementBlockModified(x, y, z);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x000349C0 File Offset: 0x00032BC0
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			this.m_subsystemElectricity.OnChunkDiscarding(chunk);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x000349D0 File Offset: 0x00032BD0
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnNeighborBlockChanged(new CellFace(x, y, z, i), neighborX, neighborY, neighborZ);
				}
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00034A14 File Offset: 0x00032C14
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int x = raycastResult.CellFace.X;
			int y = raycastResult.CellFace.Y;
			int z = raycastResult.CellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					return electricElement.OnInteract(raycastResult, componentMiner);
				}
			}
			return false;
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00034A74 File Offset: 0x00032C74
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnCollide(cellFace, velocity, componentBody);
					return;
				}
			}
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x00034AC4 File Offset: 0x00032CC4
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ElectricElement electricElement = this.m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					electricElement.OnHitByProjectile(cellFace, worldItem);
					return;
				}
			}
		}

        // Token: 0x06000814 RID: 2068 RVA: 0x00034B11 File Offset: 0x00032D11
        public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			this.m_subsystemElectricity = base.Project.FindSubsystem<SubsystemElectricity>(true);
		}

		// Token: 0x04000435 RID: 1077
		public SubsystemElectricity m_subsystemElectricity;
	}
}
