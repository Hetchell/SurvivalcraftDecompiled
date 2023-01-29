using System;
using Engine;

namespace Game
{
    public class SubsystemCommandBlockBehavior : SubsystemEditableItemBehavior<CommandData>
    {
        SubsystemTerrain subsystemTerrain;
        SubsystemElectricity subsystemElectricity;
        SubsystemCommandEngine mCommandEngine;

        public override int[] HandledBlocks
        {
            get
            {
                return new int[] { 501 };
            }
        }

        public SubsystemCommandEngine CommandEngine
        {
            get
            {
                return mCommandEngine;
            }
        }

        public SubsystemCommandBlockBehavior() : base(501)
        {
        }

        protected override void Load(TemplatesDatabase.ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
            subsystemElectricity = Project.FindSubsystem<SubsystemElectricity>(true);
            mCommandEngine = Project.FindSubsystem<SubsystemCommandEngine>(true);
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            CommandData commandBlockData = GetBlockData(new Point3(x, y, z)) ?? new CommandData();
            DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new TextBoxDialog("enter one-line command", commandBlockData.Command, 300, delegate (string result)
            {
                commandBlockData.Command = result;
                SetBlockData(new Point3(x, y, z), commandBlockData);
                var electricElement = subsystemElectricity.GetElectricElement(x, y, z, 0);
                if (electricElement != null)
                {
                    subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
                }
            }));
            return true;
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            int value1 = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value1);
            var commandBlockData = GetItemData(id);
            if (commandBlockData != null)
            {
                commandBlockData = (CommandData)commandBlockData.Copy();
            }
            else
            {
                commandBlockData = new CommandData();
            }
            DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new TextBoxDialog("enter a one-line command", commandBlockData.Command, 300, delegate (string result)
            {
                commandBlockData.Command = result;
                var data = StoreItemDataAtUniqueId(commandBlockData);
                var value = Terrain.ReplaceData(value1, data);
                inventory.RemoveSlotItems(slotIndex, count);
                inventory.AddSlotItems(slotIndex, value, 1);
            }));
            return true;
        }

        public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
        {
            base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
            var terrain = subsystemTerrain.Terrain;
            var data = GetBlockData(new Point3(x, y, z));
            if (data != null)
            {
                if (!data.HasNext())
                {
                    if (terrain.GetCellValue(neighborX, neighborY, neighborZ) == 501)
                    {
                        data.Next = new Point3(neighborX, neighborY, neighborZ);
                    }
                }
                else
                {
                    if (terrain.GetCellValue(data.Next.X, data.Next.Y, data.Next.Z) != 501)
                    {
                        data.Next = Point3.Zero;
                    }
                }
            }
        }

        public void RunCommand(Point3 position)
        {
            var data = GetBlockData(position);
            if (mCommandEngine.RunCommand(position, data.Command))
			{
				if (data.HasNext())
				{
					RunCommand(data.Next);
				}   
            }
            else
            {
                data.IsAvaliable = false;
            }
        }
    }

    public class CommandData : IEditableItemData
    {
        public string Command = string.Empty;
        public Point3 Next;
        public bool IsAvaliable = true;

        public IEditableItemData Copy()
        {
            return new CommandData()
            {
                Command = Command,
                Next = Next
            };
        }

        public void LoadString(string data)
        {
            string[] s = data.Split(';');
            Command = s[0];
            int.TryParse(s[1], out int x);
            int.TryParse(s[2], out int y);
            int.TryParse(s[3], out int z);
            Next = new Point3(x, y, z);
        }

        public string SaveString()
        {
            return string.Format("{0};{1};{2};{3}", Command, Next.X, Next.Y, Next.Z);
        }

        public bool HasNext()
        {
            return Next != Point3.Zero;
        }
    }
}
