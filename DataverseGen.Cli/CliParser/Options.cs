using CommandLine;

namespace DataverseGen.Cli.CliParser
{
    public class Options
    {
        [Option('d', "config", Required = false, HelpText = "Set config file name default", Default = "dataversegen.config.json")]
        public string Config { get; set; }

        [Option('c', "connectionstring", Required = false, HelpText = "Set connectionstring")]
        public string ConnectionString { get; set; }

        [Option('n', "namespace", Required = false, HelpText = "Set namespace")]
        public string NameSpace { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}