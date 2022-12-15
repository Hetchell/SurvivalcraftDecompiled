using System;
using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	// Token: 0x020001C3 RID: 451
	public class ComponentBlockEntity : Component
	{
		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000B56 RID: 2902 RVA: 0x00054F41 File Offset: 0x00053141
		// (set) Token: 0x06000B57 RID: 2903 RVA: 0x00054F49 File Offset: 0x00053149
		public Point3 Coordinates { get; set; }

        // Token: 0x06000B58 RID: 2904 RVA: 0x00054F52 File Offset: 0x00053152
        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			this.Coordinates = valuesDictionary.GetValue<Point3>("Coordinates");
		}

        // Token: 0x06000B59 RID: 2905 RVA: 0x00054F65 File Offset: 0x00053165
        public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue<Point3>("Coordinates", this.Coordinates);
		}
	}
}
