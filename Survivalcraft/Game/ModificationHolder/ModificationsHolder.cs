using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Survivalcraft.Game.ModificationHolder
{
    public class ModificationsHolder
    {
        public static Boolean allowFlyingAnimal = false;
        public static Boolean allowWolfDespawn = true;
        public static bool allowForUnrestrictedTravel = true;
        public static float steppedLevelTravel = 10f;
        public static String[] animalTypes = { "Wolf", "Hyena" };
        public static float movementLimitPlayer = 100f * 100f;
        public static float movementLimitAnimalsDerFlying = 300f;

        public static XElement nodeForMainScreen;

        public static void open()
        {
            IEnumerable<XAttribute> enumerable = nodeForMainScreen.Attributes();
            foreach (XAttribute node in enumerable)
            {
                using (StreamWriter writer = new StreamWriter("D:\\SCPAK\\Wfile.txt"))
                {
                    writer.WriteLine(node.ToString());
                }
            }
           
        }
    }
}
