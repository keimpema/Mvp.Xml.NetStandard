using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace MethodRenamer
{
    /// <summary>
    /// An utility to rename methods in MSIL code.
    /// </summary>
    public static class MethodRenamer
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // configure serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information($"MethodRenamer started with {args.Length} arguments");

            if (args.Length < 3)
            {
                Log.Error("MethodRenamer: missing parameter(s).");
                Console.WriteLine("Usage: MethodRenamer [path to config file] [path to input il] [path to output il]");
                return;
            }

            // read app settings
            Dictionary<string, string> dictionary = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(args[0], false, false)
                .Build()
                .AsEnumerable()
                .ToDictionary(item => item.Key, item => item.Value);

            // Reads input IL code
            var reader = new StreamReader(args[1]);

            // Writes output IL code
            var writer = new StreamWriter(args[2]);

            string line;

            // Go read line by line
            while ((line = reader.ReadLine()) != null)
            {
                // Method definition?
                if (line.Trim().StartsWith(".method"))
                {
                    writer.WriteLine(line);
                    line = reader.ReadLine();
                    if (line != null && line.IndexOf("(", StringComparison.Ordinal) != -1)
                    {
                        string methodName =
                            line.Trim().Substring(0, line.Trim().IndexOf("(", StringComparison.Ordinal));
                        if (dictionary.TryGetValue(methodName, out string replaceMethodName))
                        {
                            writer.WriteLine(line.Replace(methodName + "(", "'" + replaceMethodName + "'("));
                            Console.WriteLine($"Found '{methodName}' method, renamed to '{replaceMethodName}'");
                            continue;
                        }
                    }
                }
                writer.WriteLine(line);
            }
            reader.Close();
            writer.Close();
        }
    }
}
