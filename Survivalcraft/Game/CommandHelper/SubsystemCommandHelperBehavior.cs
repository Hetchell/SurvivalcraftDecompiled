using System;
namespace Game
{
    public class SubsystemCommandHelperBehavior : SubsystemBlockBehavior
    {

        public override int[] HandledBlocks
        {
            get
            {
                return new int[] { 502 };
            }
        }

        public override bool OnUse(Engine.Vector3 start, Engine.Vector3 direction, ComponentMiner componentMiner)
        {
            var bodyResult = componentMiner.PickBody(start, direction);
            if (bodyResult.HasValue)
            {
                var creature = bodyResult.Value.ComponentBody.Entity.FindComponent<ComponentCreature>();
                if (creature != null)
                {
                    string name = creature.DisplayName.Replace(' ', '_').ToLower();
                    ClipboardManager.ClipboardString = name;
                    componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage(name + "is copied to the clipboard", false, false);
                    return false;
                }
            }

            var terrainResult = componentMiner.PickTerrainForDigging(start, direction);
            if (terrainResult.HasValue)
            {
                var val = terrainResult.Value.Value.ToString();
                ClipboardManager.ClipboardString = val;
                componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage(val + "is copied to the clipboard", false, false);
            }
            return false;
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            DialogsManager.ShowDialog(componentPlayer.View.GameWidget, new CommandHelperDialog(Project.FindSubsystem<SubsystemCommandEngine>()));
            return false;
        }
    }
}
