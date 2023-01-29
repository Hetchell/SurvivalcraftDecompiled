using System;

namespace Game
{
    public class CommandBlock : CubeBlock, IElectricElementBlock
    {
        public const int Index = 501;

        public ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
        {
            return new CommandElectricElement(subsystemElectricity, new Engine.Point3(x, y, z));
        }

        public int GetConnectionMask(int value)
        {
            return 2147483647;
        }

        public ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            return ElectricConnectorType.Input;
        }
    }
}
