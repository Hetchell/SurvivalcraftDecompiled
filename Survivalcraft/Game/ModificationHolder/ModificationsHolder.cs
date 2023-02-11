using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survivalcraft.Game.ModificationHolder
{
    public class ModificationsHolder
    {
        public static Boolean allowFlyingAnimal = false;
        public static Boolean allowWolfDespawn = true;
        public static bool allowForUnrestrictedTravel = true;
        public static float steppedLevelTravel = 5.0f;
        public static String[] animalTypes = { "Wolf", "Hyena" };
        public static float movementLimitPlayer = 100f * 100f;
        public static float movementLimitAnimalsDerFlying = 300f;
    }
}
