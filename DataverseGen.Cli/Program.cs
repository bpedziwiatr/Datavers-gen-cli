//using CommandLine;
//using DataverseGen.Cli.CliParser;

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using DataverseGen.Core.Config;
using DataverseGen.Core.DataConverter;
using DataverseGen.Core.Generators.Scriban;
using DataverseGen.Core.Metadata;

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
                Console.WriteLine(Title);

                ConfigModel config = GetConfig();

                DataverseConnector connector =
                    new DataverseConnector(config.ConnectionString, config.Entities);
                MappingEntity[] data = connector.GetMappedEntities();
                Console.WriteLine(@"Finish Load data");
                Context context = new Context
                {
                    Namespace = config.Namespace,
                    Entities = data
                };

                Console.WriteLine(@"Start generator");
                switch (config.TemplateEngine.Name)
                {
                    case "scriban":
                        ScribanRun(config, context);
                        break;
                    default:
                        Console.WriteLine(
                            $@"Unsupported generator {config.TemplateEngine}, run scriban");
                        ScribanRun(config, context);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(@"#StackTrace:");
                Console.WriteLine(e.StackTrace);
                throw;
            }
            finally
            {
                Console.WriteLine(@"Bye Bye, see you next time Press any Key to exit");
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