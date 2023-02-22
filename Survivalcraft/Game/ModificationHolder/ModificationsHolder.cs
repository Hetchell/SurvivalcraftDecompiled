using Engine.Input;
using Game;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Survivalcraft.Game.ModificationHolder
{
    public class ModificationsHolder
    {
        public static bool allowFlyingAnimal = false;
        public static bool allowWolfDespawn = true;
        public static bool fogEnable = false;
        public static bool allowForUnrestrictedTravel = true;
        public static float steppedLevelTravel = 10f;
        public static String[] animalTypes = { "Wolf", "Hyena" };
        public static float movementLimitPlayer = 100f * 100f;
        public static float movementLimitAnimalsDerFlying = 300f;

        public static XElement nodeForMainScreen;

        private static int repeat = 0;

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

        public static void keyboardActions(WidgetInput input)
        {
            if (input.IsKeyDownOnce(Key.UpArrow))
            {
                ComponentInput.speed++;
                Debug.WriteLine("Speed is increased");
            }
            if (input.IsKeyDownOnce(Key.DownArrow))
            {
                ComponentInput.speed--;
                Debug.WriteLine("Speed is decreased");
            }
            if (input.IsKeyDown(Key.LeftArrow))
            {
                ComponentInput.state = true;
                ComponentInput.step = ModifierHolder.steppedTravel;
            }
            else if (input.IsKeyDown(Key.RightArrow))
            {
                ComponentInput.state = true;
                ComponentInput.step = -ModifierHolder.steppedTravel;
            }
            else
            {
                ComponentInput.state = false;
            }
            if (input.IsKeyDownOnce(Key.N))
            {
                //Console.WriteLine("output is " + repeat);
                if (ComponentInput.repeat == 0)
                {
                    ComponentInput.noclipState = true;
                    ComponentInput.repeat++;
                }
                else
                {
                    --ComponentInput.repeat;
                    ComponentInput.noclipState = false;
                }
            }
            //allow animallist to fly
            if (input.IsKeyDownOnce(Key.Control))
            {
                ModificationsHolder.allowFlyingAnimal = true;
            }
            //allow animallist to drop
            if (input.IsKeyDownOnce(Key.Tab))
            {
                ModificationsHolder.allowFlyingAnimal = false;
            }
            if (input.IsKeyDownOnce(Key.Enter))
            {
                ModificationsHolder.open();
            }
            //if (input.IsKeyDownOnce(Key.J))
            //{
            //    Console.WriteLine("output is " + repeat);
            //    if (repeat == 0)
            //    {
            //        fogEnable = true;
            //        repeat++;
            //    }
            //    else
            //    {
            //        --repeat;
            //        fogEnable = false;
            //    }
            //}
        }
    }
}
