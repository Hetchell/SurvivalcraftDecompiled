using System;
using System.Collections.Generic;
using System.Text;
using Engine;

namespace Game
{
    public class CommandStream
    {
        bool hasCreature;
        readonly Point3? exePosition;

        public Point3 ExePosition
        {
            get
            {
                if (hasCreature)
                {
                    return SubsystemCommandEngine.ToPoint3(m_creature.ComponentBody.Position);
                }
                return exePosition.Value;
            }
        }

        public ComponentCreature Creature
        {
            get
            {
                if (hasCreature)
                {
                    return m_creature;
                }
                throw new Exception("command is not executed by a creature");
            }
        }
        readonly ComponentCreature m_creature;

        public string Name
        {
            get
            {
                return m_commands[0];
            }
        }

        string[] m_commands;
        int m_position = 1;

        public string Last
        {
            get
            {
                return m_commands[m_position - 1];
            }
        }

        public string Current
        {
            get
            {
                return m_commands[m_position];
            }
        }

        public string[] Commands
        {
            get
            {
                return m_commands;
            }
        }

        public CommandStream(ComponentCreature creature, Point3? exePosition, string command)
        {
            m_commands = GetCommands(command);
            m_creature = creature;
            this.exePosition = exePosition;
            hasCreature = creature != null;
        }

        public bool HasNext
        {
            get
            {
                return m_position < m_commands.Length;
            }
        }

        public string Peek()
        {
            if (HasNext)
            {
                var s = m_commands[m_position];
                return s;
            }
            return string.Empty;
        }

        string NextArg(string type)
        {
            if (HasNext)
            {
                var s = m_commands[m_position++];
                return s;
            }
            throw new ArgNotFoundException(type, m_commands[m_position - 1]);
        }

        public object Next(Type t)
        {
            return convertors[t](this);
        }

        public string NextString()
        {
            return NextArg("string");
        }

        public string NextString(string def)
        {
            try
            {
                return NextString();
            }
            catch (ArgNotFoundException)
            {
                return def;
            }
        }

        public bool NextBool()
        {
            try
            {
                return bool.Parse(NextArg("boolean"));
            }
            catch (FormatException)
            {
                throw new WrongArgTypeException(m_commands[m_position - 1], "boolean");
            }
        }

        public bool NextBool(bool def)
        {
            try
            {
                return NextBool();
            }
            catch (ArgNotFoundException)
            {
                return def;
            }
        }

        public int NextInt()
        {
            return ExceptionHelper.ParseInt(NextArg("integer"));
        }

        public int NextInt(int def)
        {
            try
            {
                return NextInt();
            }
            catch (ArgNotFoundException)
            {
                return def;
            }
        }

        public float NextFloat()
        {
            return ExceptionHelper.ParseFloat(NextArg("float"));
        }

        public float NextFloat(float def)
        {
            try
            {
                return NextFloat();
            }
            catch (ArgNotFoundException)
            {
                return def;
            }
        }

        public Point3 PeekPoint()
        {
            var p = NextPoint3();
            m_position -= 3;
            return p;
        }

        public Vector3 NextVector3()
        {
            Vector3 result = new Vector3();
            string str;

            Vector3 src;
            if (hasCreature)
            {
                src = m_creature.ComponentBody.Position;
            }
            else
            {
                src = new Vector3(exePosition.Value);
            }

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.X = src.X;
                }
                else
                {
                    result.X = src.X + ExceptionHelper.ParseFloat(str.Substring(1));
                }
            }
            else
            {
                result.X = ExceptionHelper.ParseFloat(str);
            }

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.Y = src.Y;
                }
                else
                {
                    result.Y = src.Y + ExceptionHelper.ParseFloat(str.Substring(1));
                }
            }
            else
            {
                result.Y = ExceptionHelper.ParseFloat(str);
            }

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.Z = src.Z;
                }
                else
                {
                    result.Z = src.Z + ExceptionHelper.ParseFloat(str.Substring(1));
                }
            }
            else
            {
                result.Z = ExceptionHelper.ParseFloat(str);
            }
            return result;
        }

        public Point3 NextPoint3()
        {
            Point3 result = new Point3();
            string str;

            Point3 src = ExePosition;

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.X = src.X;
                }
                else
                {
                    result.X = src.X + ExceptionHelper.ParseInt(str.Substring(1));
                }
            }
            else
            {
                result.X = ExceptionHelper.ParseInt(str);
            }

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.Y = src.Y;
                }
                else
                {
                    result.Y = src.Y + ExceptionHelper.ParseInt(str.Substring(1));
                }
            }
            else
            {
                result.Y = ExceptionHelper.ParseInt(str);
            }

            str = NextString();
            if (str[0] == '~')
            {
                if (str.Length == 1)
                {
                    result.Z = src.Z;
                }
                else
                {
                    result.Z = src.Z + ExceptionHelper.ParseInt(str.Substring(1));
                }
            }
            else
            {
                result.Z = ExceptionHelper.ParseInt(str);
            }
            return result;
        }

        public string GetAllLeft()
        {
            var stringBuilder = new StringBuilder(NextString());
            while (HasNext)
            {
                stringBuilder.Append(' ');
                stringBuilder.Append(NextString());
            }
            return stringBuilder.ToString();
        }

        string[] GetCommands(string source)
        {
            var result = new List<string>();
            var str = new StringBuilder();
            bool ignore = false;
            for (int i = 0; i < source.Length; i++)
            {
                switch (source[i])
                {
                    case ' ':
                        if (ignore)
                        {
                            str.Append(' ');
                            break;
                        }
                        result.Add(str.ToString());
                        str.Clear();
                        break;
                    case '"':
                        if (ignore)
                        {
                            result.Add(str.ToString());
                            str.Clear();
                            i++;
                        }
                        ignore = !ignore;
                        break;
                    case '\\':
                        switch (source[++i])
                        {
                            case 'n':
                                str.Append('\n');
                                break;
                            case 'r':
                                str.Append('\r');
                                break;
                            case 't':
                                str.Append('\t');
                                break;
                            case '"':
                                str.Append('"');
                                break;
                            case ' ':
                                str.Append(' ');
                                break;
                        }
                        break;
                    default:
                        str.Append(source[i]);
                        break;
                }
            }
            result.Add(str.ToString());
            result.RemoveAll((string obj) => obj.Equals(string.Empty));
            return result.ToArray();
        }

        public static bool IsTypeSupported(Type t)
        {
            return convertors.ContainsKey(t);
        }

        static readonly Dictionary<Type, Func<CommandStream, object>> convertors = new Dictionary<Type, Func<CommandStream, object>>
        {
            {typeof(string), s => s.NextString()},
            {typeof(bool), s => s.NextBool()},
            {typeof(int), s => s.NextInt()},
            {typeof(float), s => s.NextFloat()},
            {typeof(Point2), s => new Point2(s.NextInt(), s.NextInt())},
            {typeof(Vector2), s => new Vector2(s.NextFloat(), s.NextFloat())},
            {typeof(Point3), s => s.NextPoint3()},
            {typeof(Vector3), s => s.NextVector3()}
        };
    }
}
