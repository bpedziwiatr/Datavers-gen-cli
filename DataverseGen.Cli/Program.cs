﻿//using CommandLine;
//using DataverseGen.Cli.CliParser;
using DataverseGen.Core.Config;
using DataverseGen.Core.DataConverter;
using DataverseGen.Core.Metadata;
using DataverseGen.Core.T4;
using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace DataverseGen.Cli
{
    internal class Program
    {
        private const string title = @"
 _____                                                 ______                                                  ______ _       _____
(____ \       _                                       / _____)                             _                  / _____) |     (_____)
 _   \ \ ____| |_  ____ _   _ ____  ____ ___  ____   | /  ___  ____ ____   ____  ____ ____| |_  ___   ____   | /     | |        _
| |   | / _  |  _)/ _  | | | / _  )/ ___)___)/ _  )  | | (___)/ _  )  _ \ / _  )/ ___) _  |  _)/ _ \ / ___)  | |     | |       | |
| |__/ ( ( | | |_( ( | |\ V ( (/ /| |  |___ ( (/ /   | \____/( (/ /| | | ( (/ /| |  ( ( | | |_| |_| | |      | \_____| |_____ _| |_
|_____/ \_||_|\___)_||_| \_/ \____)_|  (___/ \____)   \_____/ \____)_| |_|\____)_|   \_||_|\___)___/|_|       \______)_______|_____)

";

        private static ConfigModel GetConfig()
        {
            using (var stream = File.OpenRead("config.json"))
            {
                DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
                {
                    UseSimpleDictionaryFormat = true
                };

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ConfigModel), settings);
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

                Console.WriteLine(title);
                ConfigModel config = GetConfig();

                DataverseConnector connector = new DataverseConnector(config.ConnectionString, config.Entities);
                MappingEntity[] data = connector.GetMappedEntities();
                Console.WriteLine("Finish Load data");
                // Generator gen = new Generator("Ttfile.tt","out\\",null);

                //  gen.GenerateTemplate();
                Context context = new Context
                {
                    Namespace = config.Namespace,
                    Entities = data
                };
                Console.WriteLine("Start generator");
                Generator gen2 = new Generator("dataversetemplate.tt", config.OutDirectory, context);
                gen2.GenerateTemplate();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("#StackTrace:");
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
                throw;
            }
            Console.WriteLine("Bye Bye, see you next time");
            Console.ReadKey();
        }
    }
}