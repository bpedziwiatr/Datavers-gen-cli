using DataverseGen.Core.Metadata;
using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DataverseGen.Core.T4
{
    public class Generator
    {
        private readonly Context _context;
        private readonly string _outPath;
        private readonly string _t4TemplateFile;
        public Generator(string t4TemplateFile, string outPath, Context context)
        {
            _t4TemplateFile = t4TemplateFile;
            _outPath = outPath;
            _context = context;
        }

        public void GenerateTemplate()
        {
            string templateFileName = _t4TemplateFile;
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
            string dir = AppDomain.CurrentDomain.BaseDirectory + _outPath;
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
            Console.WriteLine($"Done in: {stopper.Elapsed:g}");
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