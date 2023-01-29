using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Engine;
using GameEntitySystem;

namespace Game
{
    public class SubsystemCommandEngine : Subsystem
    {
        public readonly Dictionary<string, string> creatureTemplateNames = new Dictionary<string, string>();
        public readonly Dictionary<string, int> blockIds = new Dictionary<string, int>();

        SubsystemCreatureSpawn subsystemCreature;
        SubsystemPlayers subsystemPlayers;
        SubsystemTerrain subsystemTerrain;
        SubsystemSky subsystemSky;
        SubsystemTimeOfDay subsystemTime;

        struct CommandDefination
        {
            public string Usage;
            public Action<CommandStream> Operation;
            public Action<AutoCompleteStream> AutoComplete;
        }

        readonly Dictionary<string, CommandDefination> commands = new Dictionary<string, CommandDefination>();
        static readonly Dictionary<string, string> commandUsage = new Dictionary<string, string>();

        readonly Dictionary<string, Point3> storedPoints = new Dictionary<string, Point3>();

        readonly Dictionary<string, int> creatureDatas = new Dictionary<string, int>();

        static string[] enumCreatureType = { "@a", "@r", "@p", "@e" };

        protected override void Load(TemplatesDatabase.ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            subsystemPlayers = Project.FindSubsystem<SubsystemPlayers>();
            subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>();
            subsystemSky = Project.FindSubsystem<SubsystemSky>();
            subsystemCreature = Project.FindSubsystem<SubsystemCreatureSpawn>();
            subsystemTime = Project.FindSubsystem<SubsystemTimeOfDay>();

            LoadCreatureTemplateNames();
            LoadCreatureDatas();

            foreach (KeyValuePair<string, object> pair in valuesDictionary.GetValue<TemplatesDatabase.ValuesDictionary>("Points"))
            {
                var p = Engine.Serialization.HumanReadableConverter.ConvertFromString<Point3>(pair.Key);
                storedPoints[pair.Key] = p;
            }

            InitCommands();
        }

        protected override void Save(TemplatesDatabase.ValuesDictionary valuesDictionary)
        {
            base.Save(valuesDictionary);
            var dict = new TemplatesDatabase.ValuesDictionary();
            valuesDictionary.SetValue("Points", dict);
            foreach (KeyValuePair<string, Point3> i in storedPoints)
            {
                dict.SetValue(i.Key, i.Value);
            }
        }

        public void SetPoint(string name, Point3 p)
        {
            storedPoints[name] = p;
        }

        public bool RemovePoint(string name)
        {
            return storedPoints.Remove(name);
        }

        public bool RunCommand(ComponentCreature creature, string command)
        {
            return RunCommand(new CommandStream(creature, null, command));
        }

        public bool RunCommand(Point3 position, string command)
        {
            return RunCommand(new CommandStream(null, position, command));
        }

        bool RunCommand(CommandStream stream)
        {
            try
            {
                if (commands.TryGetValue(stream.Name, out CommandDefination defination))
                {
                    defination.Operation.Invoke(stream);
                    return true;
                }
                throw new Exception("command not found");
            }
            catch (Exception e)
            {
                if (!(e is SilentException))
                {
                    Log.Error(string.Format("{0} : {1}\n{2}", stream.Name, e.Message, e.StackTrace));
                    string str = string.Format("{0} : {1}", stream.Name, e.Message);
                    foreach (ComponentPlayer p in subsystemPlayers.ComponentPlayers)
                    {
                        p.ComponentGui.DisplaySmallMessage(str, false, false);
                    }
                }
                return false;
            }
        }

        public void AutoCompleteCommand(string command, AutoCompleteStream.IAutoCompleteReciver reciver)
        {
            if (command == string.Empty)
            {
                reciver.ProvideEnumDiscription(commandUsage);
                return;
            }
            var autoComplete = new AutoCompleteStream(command, reciver);
            if (commands.TryGetValue(autoComplete.CommandStream.Name, out CommandDefination def))
            {
                try
                {
                    def.AutoComplete(autoComplete);
                }
                catch (SilentException)
                {
                }
            }
            else
            {
                reciver.CompleteEnum(autoComplete.CommandStream.Name, commands.Keys);
            }
        }

        void InitCommands()
        {
            commands["msg"] = new CommandDefination
            {
                Usage = "msg @a/r/p/e <string> [bool=true] [bool=true]",
                AutoComplete = (obj) => obj.Enum(enumCreatureType).String().Bool().Bool(),
                Operation = (obj) =>
                {
                    string type = obj.NextString();
                    string msg = obj.NextString();
                    bool b1 = obj.NextBool(true);
                    bool b2 = obj.NextBool(true);
                    FindPlayer(type, obj, (a) =>
                    {
                        a.ComponentGui.DisplaySmallMessage(msg, b1, b2);
                    });
                }
            };

            commands["msgl"] = new CommandDefination
            {
                Usage = "msgl @a/r/p/e <string> <string> <string> [float=5] [float=0]",
                AutoComplete = (obj) => obj.Enum(enumCreatureType).String().String().Float().Float(),
                Operation = (obj) =>
                {
                    string type = obj.NextString();
                    string m1 = obj.NextString();
                    string m2 = obj.NextString();
                    float f1 = obj.NextFloat(5);
                    float f2 = obj.NextFloat(0);
                    FindPlayer(type, obj, (a) =>
                    {
                        a.ComponentGui.DisplayLargeMessage(m1, m2, f1, f2);
                    });
                }
            };

            commands["kill"] = new CommandDefination
            {
                Usage = "kill @a/r/p/e [string=magic]",
                AutoComplete = (obj) => obj.Enum(enumCreatureType).String(),
                Operation = (obj) =>
                {
                    string type = obj.NextString();
                    string reason = obj.NextString("magic");
                    EnumCreatures(type, obj, (c) =>
                    {
                        c.ComponentHealth.Injure(1, null, true, reason);
                    });
                }
            };

            commands["health"] = new CommandDefination
            {
                Usage = "health heal/injure [float=1] [string=magic]",
                AutoComplete = (obj) => obj.Enum(new string[] { "heal", "injure" }).Float().String(),
                Operation = (obj) =>
                {
                    string type = obj.NextString();
                    switch (type)
                    {
                        case "heal":
                            float amount1 = obj.NextFloat(1);
                            obj.Creature.ComponentHealth.Heal(amount1);
                            break;
                        case "injure":
                            float amount = obj.NextFloat(1);
                            string reason = obj.NextString("magic");
                            obj.Creature.ComponentHealth.Injure(amount, null, true, reason);
                            break;
                        default:
                            throw new Exception("usage: health heal/injure [float=1] [string=magic]");
                    }
                }
            };

            commands["strike"] = new CommandDefination
            {
                Usage = "strike <vector3>",
                AutoComplete = (obj) => obj.Vector3(),
                Operation = (obj) =>
                {
                    subsystemSky.MakeLightningStrike(obj.NextVector3());
                }
            };

            commands["setblock"] = new CommandDefination
            {
                Usage = "setblock <point3> <int>",
                AutoComplete = (obj) => obj.Point3().Int(),
                Operation = (s) =>
                {
                    var p = s.NextPoint3();
                    subsystemTerrain.ChangeCell(p.X, p.Y, p.Z, s.NextInt());
                }
            };

            commands["place"] = new CommandDefination
            {
                Usage = "place <point3> <int> [bool=false] [bool=false]",
                AutoComplete = (obj) => obj.Point3().Int().Bool().Bool(),
                Operation = (obj) =>
                {
                    var p = obj.NextPoint3();
                    subsystemTerrain.DestroyCell(2, p.X, p.Y, p.Z, obj.NextInt(), obj.NextBool(false), obj.NextBool(false));
                }
            };

            commands["fill"] = new CommandDefination
            {
                Usage = "fill <point3> <point3>",
                AutoComplete = (obj) => obj.Point3().Point3().Int(),
                Operation = (obj) =>
                {
                    var p1 = obj.NextPoint3();
                    var p2 = obj.NextPoint3();
                    var startx = Math.Min(p1.X, p2.X);
                    var endx = Math.Max(p1.X, p2.X);
                    var starty = Math.Min(p1.Y, p2.Y);
                    var endy = Math.Max(p1.Y, p2.Y);
                    var startz = Math.Min(p1.Z, p2.Z);
                    var endz = Math.Max(p1.Z, p2.Z);

                    var val = obj.NextInt();

                    for (int x = startx; x <= endx; x++)
                    {
                        for (int y = starty; y <= endy; y++)
                        {
                            for (int z = startz; z <= endz; z++)
                            {
                                subsystemTerrain.Terrain.SetCellValueFast(x, y, z, val);
                            }
                        }
                    }

                    var startChunk = Terrain.ToChunk(startx, startz);
                    var endChunk = Terrain.ToChunk(endx, endz);

                    for (int x = startChunk.X; x <= endChunk.X; x++)
                    {
                        for (int y = startChunk.Y; y <= endChunk.Y; y++)
                        {
                            var c = subsystemTerrain.Terrain.GetChunkAtCoords(x, y);
                            if (c != null)
                            {
                                subsystemTerrain.TerrainUpdater.DowngradeChunkNeighborhoodState(c.Coords, 1, TerrainChunkState.InvalidLight, false);
                            }
                        }
                    }
                }
            };

            commands["time"] = new CommandDefination
            {
                Usage = "time add/set <float>",
                AutoComplete = (obj) => obj.Enum(new string[] { "add", "set" }).Float(),
                Operation = (obj) =>
                {
                    switch (obj.NextString())
                    {
                        case "add":
                            subsystemTime.TimeOfDayOffset += obj.NextFloat();
                            break;
                        case "set":
                            subsystemTime.TimeOfDayOffset = obj.NextFloat();
                            break;
                        default:
                            throw new Exception("usage: time add/set <float>");
                    }
                }
            };

            commands["execute"] = new CommandDefination
            {
                Usage = "execute @a/r/p/e <another command>",
                AutoComplete = (obj) =>
                {
                    obj.Enum(enumCreatureType);
                    AutoCompleteCommand(obj.CommandStream.GetAllLeft(), obj.Reciver);
                },
                Operation = (obj) =>
                {
                    var type = obj.NextString();
                    var command = obj.GetAllLeft();

                    EnumCreatures(type, obj, (a) =>
                    {
                        RunCommand(a, command);
                    });
                }
            };

            commands["setdata"] = new CommandDefination
            {
                Usage = "setdata <creature data> <data type>",
                AutoComplete = (obj) =>
                {
                    obj.Enum(creatureDatas.Keys);
                    string component = obj.CommandStream.Last;
                    int i = creatureDatas[component];
                    switch (i)
                    {
                        case 0:
                            obj.Any(typeof(ComponentLocomotion).GetProperty(component).PropertyType);
                            break;
                        case 1:
                            obj.Any(typeof(ComponentHealth).GetProperty(component).PropertyType);
                            break;
                        case 2:
                            obj.Any(typeof(ComponentBody).GetProperty(component).PropertyType);
                            break;
                    }
                },
                Operation = (s) =>
                {
                    while (s.HasNext)
                    {
                        var component = s.NextString();
                        PropertyInfo p;
                        if (creatureDatas.TryGetValue(component, out int i))
                        {
                            switch (i)
                            {
                                case 0:
                                    p = typeof(ComponentLocomotion).GetProperty(component);
                                    p.SetValue(s.Creature.ComponentLocomotion, s.Next(p.PropertyType), null);
                                    break;
                                case 1:
                                    p = typeof(ComponentHealth).GetProperty(component);
                                    p.SetValue(s.Creature.ComponentHealth, s.Next(p.PropertyType), null);
                                    break;
                                case 2:
                                    p = typeof(ComponentBody).GetProperty(component);
                                    p.SetValue(s.Creature.ComponentBody, s.Next(p.PropertyType), null);
                                    break;
                            }
                        }
                        else
                        {
                            throw new Exception(component + " is not a creature data");
                        }
                    }
                }
            };

            commands["gameinfo"] = new CommandDefination
            {
                Usage = "gameinfo <info name> <info value>",
                AutoComplete = (obj) =>
                {
                    Type settings = typeof(WorldSettings);
                    obj.Enum(settings.GetFields().Select(f => f.Name));
                    obj.Any(settings.GetField(obj.CommandStream.Last).FieldType);
                },
                Operation = (s) =>
                {
                    var setting = s.NextString();
                    var val = s.NextString();
                    var f = typeof(WorldSettings).GetField(setting);
                    if (f != null)
                    {
                        f.SetValue(GameManager.WorldInfo.WorldSettings, ChangeType(val, f.FieldType));
                    }
                }
            };

            commands["summon"] = new CommandDefination
            {
                Usage = "summon <animal name> <vector3> [float=0]",
                AutoComplete = (obj) => obj.String().Vector3().Float(),
                Operation = (s) =>
                {
                    var name = s.NextString();
                    var position = s.NextVector3();
                    var rotation = s.NextFloat(0);
                    Entity entity = DatabaseManager.CreateEntity(Project, creatureTemplateNames[name], true);
                    entity.FindComponent<ComponentBody>(true).Position = position;
                    entity.FindComponent<ComponentBody>(true).Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, rotation);
                    entity.FindComponent<ComponentSpawn>(true).SpawnDuration = 0.25f;
                    Project.AddEntity(entity);
                }
            };

            commands["tp"] = new CommandDefination
            {
                Usage = "tp <vector3>",
                AutoComplete = (obj) => obj.Vector3(),
                Operation = s => s.Creature.ComponentBody.Position = s.NextVector3()
            };

            commands["additem"] = new CommandDefination
            {
                Usage = "additem <vector3> <int> [int=1] [vector=0,0,0]",
                AutoComplete = (obj) => obj.Vector3().Int().Int().Vector3(),
                Operation = s =>
                {
                    var position = s.NextVector3();
                    var val = s.NextInt();
                    var count = s.NextInt(1);
                    Vector3? speed = null;
                    if (s.HasNext)
                    {
                        speed = s.NextVector3();
                    }
                    Project.FindSubsystem<SubsystemPickables>(true).AddPickable(s.NextInt(), s.NextInt(1), position, speed, null);
                }
            };

            commands["give"] = new CommandDefination
            {
                Usage = "give @a/r/p/e <int> [int=1]",
                AutoComplete = (obj) => obj.Enum(enumCreatureType).Int().Int(),
                Operation = s =>
                {
                    var enumType = s.NextString();
                    var val = s.NextInt();
                    var count = s.NextInt(1);

                    FindPlayer(enumType, s, (p) => ComponentInventoryBase.AcquireItems(p.ComponentMiner.Inventory, val, count));
                }
            };

            foreach (string name in commands.Keys)
            {
                commandUsage[commands[name].Usage] = name;
            }
        }

        void LoadBlockIds()
        {
            foreach (Block b in BlocksManager.Blocks)
            {
                foreach (int id in b.GetCreativeValues())
                {
                    var name = b.GetDisplayName(subsystemTerrain, id).Replace(' ', '_').ToLower();
                    blockIds[name] = id;
                }
                blockIds[b.DefaultDisplayName.Replace(' ', '_').ToLower()] = b.BlockIndex;
            }
        }

        void FindPlayer(string type, CommandStream s, Action<ComponentPlayer> a)
        {
            if (type[0] == '@')
            {
                EnumCreatures(type, s, c =>
                {
                    if (c is ComponentPlayer)
                    {
                        a(c as ComponentPlayer);
                    }
                });
            }
            else
            {
                FindPlayerByName(type, a);
            }
        }

        void FindPlayerByName(string name, Action<ComponentPlayer> a)
        {
            foreach (ComponentPlayer p in subsystemPlayers.ComponentPlayers)
            {
                if (p.PlayerData.Name == name)
                {
                    a(p);
                }
                else
                {
                    throw new WrongArgTypeException(name, "player");
                }
            }
        }

        void LoadCreatureTemplateNames()
        {
            var paramterType = DatabaseManager.GameDatabase.ParameterType;
            var entities = DatabaseManager.GameDatabase.Database.Root.GetExplicitNestingChildren(paramterType, false);
            var displayName = new Guid("715ff548-ef2b-430e-8e6b-51b934e5da1d");
            foreach (TemplatesDatabase.DatabaseObject o in entities)
            {
                if (o.EffectiveInheritanceRoot.Guid == displayName && o.Value.ToString() != string.Empty)
                {
                    creatureTemplateNames[o.NestingParent.NestingParent.Name.ToLower()] = o.Value.ToString();
                    //Log.Information("{0}, {1}", o.NestingParent.NestingParent.Name, o.Value);
                }
            }
        }

        void LoadCreatureDatas()
        {
            foreach (PropertyInfo p in typeof(ComponentLocomotion).GetRuntimeProperties())
            {
                if (CommandStream.IsTypeSupported(p.PropertyType))
                {
                    creatureDatas[p.Name] = 0;
                }
            }
            foreach (PropertyInfo p in typeof(ComponentHealth).GetRuntimeProperties())
            {
                if (CommandStream.IsTypeSupported(p.PropertyType))
                {
                    creatureDatas[p.Name] = 1;
                }
            }
            foreach (PropertyInfo p in typeof(ComponentBody).GetRuntimeProperties())
            {
                if (CommandStream.IsTypeSupported(p.PropertyType))
                {
                    creatureDatas[p.Name] = 2;
                }
            }
        }

        void EnumCreatures(string type, CommandStream s, Action<ComponentCreature> a)
        {
            var data = new EnumData(type, subsystemCreature.Creatures.Count, s.ExePosition);
            if (data.name == "player")
            {
                data.name = string.Empty;
                SearchCreature(data, SortCreatures(data, subsystemPlayers.ComponentPlayers), a);
            }
            else
            {
                SearchCreature(data, SortCreatures(data, subsystemCreature.Creatures), a);
            }
        }

        List<ComponentCreature> SortCreatures<T>(EnumData data, IEnumerable<T> collection) where T : ComponentCreature
        {
            var l = new List<ComponentCreature>();
            if (data.mode1 == SearchingMode1.Regular)
            {
                foreach (ComponentCreature c in collection)
                {
                    if (CheckCreatureWithData(data, c))
                    {
                        l.Add(c);
                    }
                }
            }
            else if (data.mode1 == SearchingMode1.Nearest)
            {
                var v = new Vector3(data.x, data.y, data.z);
                var dict = new Dictionary<float, ComponentCreature>();
                foreach (ComponentCreature c in collection)
                {
                    if (CheckCreatureWithData(data, c))
                    {
                        dict.Add(Vector3.DistanceSquared(v, c.ComponentBody.Position), c);
                    }
                }
                var l2 = new List<float>();
                foreach (float i in dict.Keys)
                {
                    l2.Add(i);
                }
                l2.Sort(delegate (float i1, float i2)
                {
                    if (i1 > i2)
                    {
                        return 1;
                    }
                    return -1;
                });

                foreach (float i in l2)
                {
                    l.Add(dict[i]);
                }
            }
            else
            {
                foreach (ComponentCreature c in collection)
                {
                    if (CheckCreatureWithData(data, c))
                    {
                        l.Add(c);
                    }
                }
                var r = new Random();
                var count = l.Count;

                int n = l.Count;
                while (n > 1)
                {
                    n--;
                    int k = r.UniformInt(0, n);
                    var value = l[k];
                    l[k] = l[n];
                    l[n] = value;
                }
            }

            return l;
        }

        void SearchCreature(EnumData data, List<ComponentCreature> sorted, Action<ComponentCreature> a)
        {
            for (int i = 0; i < data.count; i++)
            {
                if (i >= sorted.Count)
                {
                    return;
                }
                a.Invoke(sorted[i]);
            }
        }

        bool CheckCreatureWithData(EnumData data, ComponentCreature c)
        {
            if (data.name != string.Empty)
            {
                string dataname;
                if (data.name[0] == '!')
                {
                    Log.Information(c.DisplayName);
                    if (creatureTemplateNames.TryGetValue(data.name.Substring(1), out dataname))
                    {
                        if (dataname == c.DisplayName)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (creatureTemplateNames.TryGetValue(data.name, out dataname))
                    {
                        if (dataname != c.DisplayName)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            switch (data.SearchingMode)
            {
                case SearchingMode.Radius:
                    if (data.r > 0)
                    {
                        if (Vector3.DistanceSquared(new Vector3(data.x, data.y, data.z), c.ComponentBody.Position) >= data.r2)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Vector3.DistanceSquared(new Vector3(data.x, data.y, data.z), c.ComponentBody.Position) <= data.r2)
                        {
                            return false;
                        }
                    }
                    break;
                case SearchingMode.Volume:
                    var p = c.ComponentBody.Position;
                    var x = p.X - data.x;
                    var y = p.Y - data.y;
                    var z = p.Z - data.z;
                    if (x < 0 && x > data.dx && y < 0 && y > data.dy && z < 0 && z > data.dz)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        struct EnumData
        {
            public string name;
            public int count;
            public SearchingMode1 mode1;
            public SearchingMode SearchingMode;
            public int x;
            public int y;
            public int z;
            public int r;
            public int r2;
            public int dx;
            public int dy;
            public int dz;

            public EnumData(string data, int maxCount, Point3 exePoint)
            {
                if (data[0] != '@')
                {
                    throw new Exception("use @a/r/p/e[<argument=value>] for creature searching");
                }

                x = exePoint.X;
                y = exePoint.Y;
                z = exePoint.Z;

                switch (data[1])
                {
                    case 'a':
                        name = "player";
                        count = maxCount;
                        mode1 = SearchingMode1.Regular;
                        break;
                    case 'r':
                        name = "player";
                        count = 1;
                        mode1 = SearchingMode1.Random;
                        break;
                    case 'p':
                        name = "player";
                        count = 1;
                        mode1 = SearchingMode1.Nearest;
                        break;
                    case 'e':
                        name = string.Empty;
                        count = maxCount;
                        mode1 = SearchingMode1.Regular;
                        break;
                    default:
                        throw new Exception("use @a/r/p/e[<argument=value>] for creature searching");
                }

                SearchingMode = SearchingMode.None;
                r = 0;
                r2 = 0;
                dx = -1;
                dy = -1;
                dz = -1;

                if (data.Length != 2)
                {
                    StringBuilder key = new StringBuilder();
                    StringBuilder val = new StringBuilder();
                    int i = 3;
                    int length = data.Length;
                    bool iskey = true;
                    while (i < length)
                    {
                        switch (data[i])
                        {
                            case '=':
                                iskey = false;
                                break;
                            case ',':
                                iskey = true;
                                ReadKeyValue(key.ToString(), val.ToString());
                                key.Clear();
                                val.Clear();
                                break;
                            case ']':
                                ReadKeyValue(key.ToString(), val.ToString());
                                key.Clear();
                                val.Clear();
                                i = length;
                                break;
                            case '_':
                                if (iskey)
                                {
                                    key.Append(' ');
                                }
                                else
                                {
                                    val.Append(' ');
                                }
                                break;
                            default:
                                if (iskey)
                                {
                                    key.Append(data[i]);
                                }
                                else
                                {
                                    val.Append(data[i]);
                                }
                                break;
                        }
                        i++;
                    }

                    if (dx != -1 && dy != -1 && dz != -1)
                    {
                        SearchingMode = SearchingMode.Volume;
                    }
                    else if (r != 0)
                    {
                        SearchingMode = SearchingMode.Radius;
                        r2 = r * r;
                    }
                }
            }

            void ReadKeyValue(string key, string val)
            {
                switch (key)
                {
                    case "r":
                        r = ExceptionHelper.ParseInt(val);
                        break;
                    case "name":
                        name = val;
                        break;
                    case "c":
                        count = ExceptionHelper.ParseInt(val);
                        break;
                    case "x":
                        x = ExceptionHelper.ParseInt(val);
                        break;
                    case "y":
                        y = ExceptionHelper.ParseInt(val);
                        break;
                    case "z":
                        z = ExceptionHelper.ParseInt(val);
                        break;
                    case "dx":
                        dx = ExceptionHelper.ParseInt(val);
                        break;
                    case "dy":
                        dy = ExceptionHelper.ParseInt(val);
                        break;
                    case "dz":
                        dz = ExceptionHelper.ParseInt(val);
                        break;
                }
            }
        }

        public enum SearchingMode1
        {
            Regular,
            Random,
            Nearest
        }

        public enum SearchingMode
        {
            None,
            Radius,
            Volume
        }

        public static Point3 ToPoint3(Vector3 v)
        {
            return Terrain.ToCell(v);
        }

        public static object ChangeType(string str, Type t)
        {
            return Engine.Serialization.HumanReadableConverter.ConvertFromString(t, str);
        }
    }

    struct OffsetCache
    {
        bool[] hasOffset;
        int[] cache;

        public OffsetCache(CommandStream s)
        {
            hasOffset = new bool[3];
            cache = new int[3];

            for (int i = 0; i < 3; i++)
            {
                var str = s.NextString();
                if (str[0] == '~')
                {
                    hasOffset[i] = true;
                    if (str.Length != 1)
                    {
                        cache[i] = ExceptionHelper.ParseInt(str.Substring(1));
                    }
                }
                else
                {
                    cache[i] = ExceptionHelper.ParseInt(str);
                }
            }
        }

        public Point3 Offset(Point3 p)
        {
            var result = new Point3();
            if (hasOffset[0])
            {
                result.X = cache[0] + p.X;
            }
            else
            {
                result.X = cache[0];
            }

            if (hasOffset[1])
            {
                result.Y = cache[1] + p.Y;
            }
            else
            {
                result.Y = cache[1];
            }

            if (hasOffset[2])
            {
                result.Z = cache[2] + p.Z;
            }
            else
            {
                result.Z = cache[2];
            }
            return result;
        }
    }

    public class ArgNotFoundException : Exception
    {
        public ArgNotFoundException(string type, string lastPart) : base(string.Format("another {0} is expected after {1}", type, lastPart))
        {
        }
    }

    public class WrongArgTypeException : Exception
    {
        public WrongArgTypeException(string name, string type) : base(string.Format("{0} is not a {1}", name, type))
        {
        }
    }

    public class SilentException : Exception
    {
    }

    static class ExceptionHelper
    {
        public static Exception CommandNotFound
        {
            get
            {
                return new Exception("command not found");
            }
        }

        public static int ParseInt(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (FormatException)
            {
                throw new WrongArgTypeException(s, "integer");
            }
        }

        public static float ParseFloat(string s)
        {
            try
            {
                return (float)double.Parse(s);
            }
            catch (FormatException)
            {
                throw new WrongArgTypeException(s, "float");
            }
        }
    }
}