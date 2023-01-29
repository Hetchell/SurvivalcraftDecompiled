using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    public class CommandElectricElement : ElectricElement
    {
        List<ElectricConnection> temp;

		public CommandElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, new List<CellFace>
		{
			new CellFace (point.X, point.Y, point.Z, 0),
			new CellFace (point.X, point.Y, point.Z, 1),
			new CellFace (point.X, point.Y, point.Z, 2),
			new CellFace (point.X, point.Y, point.Z, 3),
			new CellFace (point.X, point.Y, point.Z, 4),
			new CellFace (point.X, point.Y, point.Z, 5)
		})
		{
		}

		public override bool Simulate()
		{
			float num = 0f;
			foreach (ElectricConnection current in Connections)
			{
				if (current.ConnectorType != ElectricConnectorType.Output && current.NeighborConnectorType != ElectricConnectorType.Input)
				{
					num = MathUtils.Max(num, current.NeighborElectricElement.GetOutputVoltage(current.NeighborConnectorFace));
				}
			}

            if ((int)(num * 15.999f) - 7 > 0)
			{
                SubsystemElectricity.Project.FindSubsystem<SubsystemCommandBlockBehavior>(true).RunCommand(CellFaces[0].Point);
			}
			return false;
		}

        public void SetState(bool b)
        {
            if (b)
			{
				Connections.AddRange(temp);
            }
            else
			{
				temp = Connections;
				Connections.Clear();
            }
        }
    }
}
