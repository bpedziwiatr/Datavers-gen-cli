using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using DataverseGen.Core.Config;
using DataverseGen.Core.Metadata;
using Microsoft.VisualStudio.TextTemplating;

namespace DataverseGen.Core.Generators.T4
{
    public class Generator:BaseGenerator
    {

        public Generator(string templateName, string outPath, Context context, TemplateEngineModel templateEngineModel)
            : base(templateName, outPath, context, templateEngineModel)
        {
        }

        public override void GenerateTemplate()
        {
            Console.WriteLine(@"Welcome to T4 template generator");
            string templateFileName = _templateName;
            TextTemplatingEngineHost host = new TextTemplatingEngineHost();
            Engine engine = new Engine();
            host.SetOutputEncoding(Encoding.UTF8, false);

            TextTemplatingSession session = new TextTemplatingSession
            {
                {"Context", _context}
            };

            host.Session = session;

            string input = File.ReadAllText(templateFileName);

            host.FileExtension = ".cs";
            Stopwatch stopper = Stopwatch.StartNew();
            string dir = $"{AppDomain.CurrentDomain.BaseDirectory}{_outPath}//";
            string outputFileName = string.Concat(
                dir,
                Path.GetFileNameWithoutExtension(templateFileName),
                host.FileExtension);
            host.TemplateFile = outputFileName;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string output = engine.ProcessTemplate(input, host);

            File.WriteAllText(outputFileName, output, host.FileEncoding);
            PrintErrors(host);
            stopper.Stop();
            Console.WriteLine($@"Generating T4 '{_templateName}' elapsed in: {stopper.Elapsed:g}");
        }

        private static void PrintErrors(TextTemplatingEngineHost host)
        {
            if (!host.Errors.HasErrors) return;
            foreach (CompilerError error in host.Errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}