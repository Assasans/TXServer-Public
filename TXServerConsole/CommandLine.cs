using System.Collections.Generic;

namespace TXServerConsole
{
    internal static class CommandLine
    {
        public static Dictionary<string, string[]> Parse(string[] args)
        {
            Dictionary<string, string[]> parameters = new();

            if (args.Length == 0)
                return parameters;

            string key = null;
            List<string> values = new();

            for (int i = 0; i < args.Length; i++)
            {
                string current = args[i];

                if (current[0] == '-')
                {
                    if (key != null)
                    {
                        parameters.Add(key, values.ToArray());
                        values.Clear();
                    }

                    key = current[1..];
                    continue;
                }
                else if (key == null)
                {
                    return null;
                }

                values.Add(current);
            }

            parameters.Add(key, values.ToArray());

            return parameters;
        }
    }
}
