//using CommandLine;
//using DataverseGen.Cli.CliParser;

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using DataverseGen.Core;
using DataverseGen.Core.Config;
using DataverseGen.Core.DataConverter;
using DataverseGen.Core.Generators.Scriban;
using DataverseGen.Core.Metadata;
using static DataverseGen.Core.ColorConsole;

namespace DataverseGen.Cli
{
    internal static class Program
    {
        private const string Title = @"
    ____        __       _    __                       ______         
   / __ \____ _/ /_____ | |  / /__  _____________     / ____/__  ____ 
  / / / / __ `/ __/ __ `/ | / / _ \/ ___/ ___/ _ \   / / __/ _ \/ __ \
 / /_/ / /_/ / /_/ /_/ /| |/ /  __/ /  (__  )  __/  / /_/ /  __/ / / /
/_____/\__,_/\__/\__,_/ |___/\___/_/  /____/\___/   \____/\___/_/ /_/                                                                       
";

        private static ConfigModel GetConfig()
        {
            using (FileStream stream = File.OpenRead("config.json"))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
                {
                    UseSimpleDictionaryFormat = true
                };

                DataContractJsonSerializer ser =
                    new DataContractJsonSerializer(typeof(ConfigModel), settings);
                return (ConfigModel)ser.ReadObject(stream);
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                //Parser.Default.ParseArguments<Options>(args)
                //    .WithParsed<Options>(o =>
                //    {
                //        if (o.Verbose)
                //        {
                //            Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                //            Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                //        }
                //        else
                //        {
                //            Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                //            Console.WriteLine("Quick Start Example!");
                //        }
                //    });
                Console.SetWindowSize(140, 30);
                WriteLine(Title);

                ConfigModel config = GetConfig();

                DataverseConnector connector =
                    new DataverseConnector(config.ConnectionString, config.Entities, config.ThrowOnEntityNotFound);
                MappingEntity[] data = connector.GetMappedEntities();
                WriteLine(@"Finish Load data");
                Context context = new Context
                {
                    Namespace = config.Namespace,
                    Entities = data
                };

                WriteLine(@"Start generator");
                switch (config.TemplateEngine.Name)
                {
                    case "scriban":
                        ScribanRun(config, context);
                        break;
                    default:
                        WriteLine(
                            $@"Unsupported generator {config.TemplateEngine}, run scriban");
                        ScribanRun(config, context);
                        break;
                }
            }
            catch (Exception e)
            {
                WriteError(e.ToString());
                WriteError(@"#StackTrace:");
                WriteError(e.StackTrace);
                throw;
            }
            finally
            {
                WriteSuccess(@"Bye Bye, see you next time Press any Key to exit");
               Console.ReadKey();
            }
        }

        private static void ScribanRun(ConfigModel config, Context context)
        {
            ScribanGenerator scribanGenerator = new ScribanGenerator(
                config.TemplateName,
                config.OutDirectory,
                context,
                config.TemplateEngine);
            scribanGenerator.GenerateTemplate();
        }
    }
}