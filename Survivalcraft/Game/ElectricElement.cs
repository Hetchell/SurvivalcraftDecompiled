using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
	// Token: 0x02000265 RID: 613
	public abstract class ElectricElement
	{
		// Token: 0x1700029E RID: 670
		// (get) Token: 0x0600123B RID: 4667 RVA: 0x0008D8C8 File Offset: 0x0008BAC8
		// (set) Token: 0x0600123C RID: 4668 RVA: 0x0008D8D0 File Offset: 0x0008BAD0
		public SubsystemElectricity SubsystemElectricity { get; set; }

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0008D8D9 File Offset: 0x0008BAD9
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x0008D8E1 File Offset: 0x0008BAE1
		public ReadOnlyList<CellFace> CellFaces { get; set; }

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x0008D8EA File Offset: 0x0008BAEA
		// (set) Token: 0x06001240 RID: 4672 RVA: 0x0008D8F2 File Offset: 0x0008BAF2
		public List<ElectricConnection> Connections { get; set; }

		// Token: 0x06001241 RID: 4673 RVA: 0x0008D8FB File Offset: 0x0008BAFB
		public ElectricElement(SubsystemElectricity subsystemElectricity, IEnumerable<CellFace> cellFaces)
		{
			this.SubsystemElectricity = subsystemElectricity;
			this.CellFaces = new ReadOnlyList<CellFace>(new List<CellFace>(cellFaces));
			this.Connections = new List<ElectricConnection>();
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x0008D926 File Offset: 0x0008BB26
		public ElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : this(subsystemElectricity, new List<CellFace>
		{
			cellFace
		})
		{
		}

		// Token: 0x06001243 RID: 4675 RVA: 0x0008D93B File Offset: 0x0008BB3B
		public virtual float GetOutputVoltage(int face)
		{
			return 0f;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x0008D942 File Offset: 0x0008BB42
		public virtual bool Simulate()
		{
			return false;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0008D945 File Offset: 0x0008BB45
		public virtual void OnAdded()
		{
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0008D947 File Offset: 0x0008BB47
		public virtual void OnRemoved()
		{
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x0008D949 File Offset: 0x0008BB49
		public virtual void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
		{
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x0008D94B File Offset: 0x0008BB4B
		public virtual bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			return false;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0008D94E File Offset: 0x0008BB4E
		public virtual void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x0008D950 File Offset: 0x0008BB50
		public virtual void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x0008D952 File Offset: 0x0008BB52
		public virtual void OnConnectionsChanged()
		{
		}

		// Token: 0x0600124C RID: 4684 RVA: 0x0008D954 File Offset: 0x0008BB54
		public static bool IsSignalHigh(float voltage)
		{
			return voltage >= 0.5f;
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x0008D964 File Offset: 0x0008BB64
		public int CalculateHighInputsCount()
		{
			int num = 0;
			foreach (ElectricConnection electricConnection in this.Connections)
			{
				if (electricConnection.ConnectorType != ElectricConnectorType.Output && electricConnection.NeighborConnectorType != ElectricConnectorType.Input && ElectricElement.IsSignalHigh(electricConnection.NeighborElectricElement.GetOutputVoltage(electricConnection.NeighborConnectorFace)))
				{
					num++;
				}
			}
			return num;
		}
	}
}
