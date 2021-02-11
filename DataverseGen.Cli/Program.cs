using DataverseGen.Core.Config;
using DataverseGen.Core.DataConverter;
using DataverseGen.Core.Metadata;
using DataverseGen.Core.T4;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.UI.WebControls;

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

                Console.WriteLine(title);
                ConfigModel config = GetConfig();

                DataverseConnector connector = new DataverseConnector(config.ConnectionString, config.Entities);
                var data = connector.GetMappedEntities();
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
                throw;
            }
            Console.WriteLine("Bye Bye, see you next time");
            //     Console.ReadKey();
        }
    }
}