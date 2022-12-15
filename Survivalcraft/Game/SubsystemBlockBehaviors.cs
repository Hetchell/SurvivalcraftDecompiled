using System;
using System.Collections.Generic;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x0200015D RID: 349
	public class SubsystemBlockBehaviors : Subsystem
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x0002B583 File Offset: 0x00029783
		public ReadOnlyList<SubsystemBlockBehavior> BlockBehaviors
		{
			get
			{
				return new ReadOnlyList<SubsystemBlockBehavior>(this.m_blockBehaviors);
			}
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x0002B590 File Offset: 0x00029790
		public SubsystemBlockBehavior[] GetBlockBehaviors(int contents)
		{
			return this.m_blockBehaviorsByContents[contents];
		}

        // Token: 0x060006D7 RID: 1751 RVA: 0x0002B59C File Offset: 0x0002979C
        public override void Load(ValuesDictionary valuesDictionary)
		{
			this.m_blockBehaviorsByContents = new SubsystemBlockBehavior[BlocksManager.Blocks.Length][];
			Dictionary<int, List<SubsystemBlockBehavior>> dictionary = new Dictionary<int, List<SubsystemBlockBehavior>>();
			for (int i = 0; i < this.m_blockBehaviorsByContents.Length; i++)
			{
				dictionary[i] = new List<SubsystemBlockBehavior>();
				foreach (string text in BlocksManager.Blocks[i].Behaviors.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					SubsystemBlockBehavior item = base.Project.FindSubsystem<SubsystemBlockBehavior>(text.Trim(), true);
					dictionary[i].Add(item);
				}
			}
			foreach (SubsystemBlockBehavior subsystemBlockBehavior in base.Project.FindSubsystems<SubsystemBlockBehavior>())
			{
				this.m_blockBehaviors.Add(subsystemBlockBehavior);
				foreach (int key in subsystemBlockBehavior.HandledBlocks)
				{
					dictionary[key].Add(subsystemBlockBehavior);
				}
			}
			for (int k = 0; k < this.m_blockBehaviorsByContents.Length; k++)
			{
				this.m_blockBehaviorsByContents[k] = dictionary[k].ToArray();
			}
		}

		// Token: 0x040003CA RID: 970
		public SubsystemBlockBehavior[][] m_blockBehaviorsByContents;

		// Token: 0x040003CB RID: 971
		public List<SubsystemBlockBehavior> m_blockBehaviors = new List<SubsystemBlockBehavior>();
	}
}
