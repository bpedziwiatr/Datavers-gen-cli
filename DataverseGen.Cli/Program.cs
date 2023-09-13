﻿using System.Diagnostics;
using DataverseGen.Core.Config;
using DataverseGen.Core.DataConverter;
using DataverseGen.Core.Generators.Scriban;
using DataverseGen.Core.Metadata;
using Newtonsoft.Json;
using static DataverseGen.Core.ColorConsole;
namespace DataverseGen.Cli;
//*
// NOTE!!
// DO NOT USE 'Trim unused assemblies'! It'll break json deserialization!
//*//
internal static class Program
{
    private const string DataverseGenConfigJson = "dataversegen.config.json";

    private const string Title = @"
    ____        __       _    __                       ______         
   / __ \____ _/ /_____ | |  / /__  _____________     / ____/__  ____ 
  / / / / __ `/ __/ __ `/ | / / _ \/ ___/ ___/ _ \   / / __/ _ \/ __ \
 / /_/ / /_/ / /_/ /_/ /| |/ /  __/ /  (__  )  __/  / /_/ /  __/ / / /
/_____/\__,_/\__/\__,_/ |___/\___/_/  /____/\___/   \____/\___/_/ /_/                                                                       
";

    private static ConfigModel GetConfig()
    {
        if (!File.Exists(DataverseGenConfigJson))
        {
            throw new FileNotFoundException("Config not found in current directory",
                DataverseGenConfigJson);
        }

        string json = File.ReadAllText(DataverseGenConfigJson);
        return JsonConvert.DeserializeObject<ConfigModel>(json);
    }

    private static void Main(string[] args)
    {
        try
        {
            Console.SetWindowSize(140, 30);
            WriteLine(Title);
            WriteVersion();

            ConfigModel config = GetConfig();

            DataverseMetadataConverter dataConverter = new(
                new DataverseConnector
                (config.ConnectionString,
                    config.EnableConnectionStringValidation),
                config.ThrowOnEntityNotFound,
                config.Entities
            );
            MappingEntity[] data = dataConverter.GetMappedEntities();
            WriteLine(@"Finish Load data");
            Context context = new()
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

    private static void WriteVersion()
    {
        ProcessModule? assembly = Process.GetCurrentProcess().MainModule;

        if (assembly is null)
        {
            WriteLine("Unable to get main module version.");
            return;
        }
        FileVersionInfo assemblyFileVersion = assembly.FileVersionInfo;
        WriteLine($"Runtime Version: {assemblyFileVersion.FileVersion}");
    }

    private static void ScribanRun(ConfigModel config, Context context)
    {
        ScribanGenerator scribanGenerator = new(
            config.TemplateName,
            config.TemplateDirectoryName,
            config.OutDirectory,
            context,
            config.TemplateEngine);
        scribanGenerator.GenerateTemplate();
    }
}