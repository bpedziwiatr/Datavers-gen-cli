using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataverseGen.Core.T4
{
    [Serializable]
    public class TextTemplatingEngineHost : ITextTemplatingEngineHost, ITextTemplatingSessionHost
    {
        private List<string> _localDlls = new List<string>
        {
            typeof(Uri).Assembly.Location,
            typeof(System.Dynamic.CallInfo).Assembly.Location,
            typeof(System.Xml.Linq.XCData).Assembly.Location,
            typeof(System.Xml.ConformanceLevel).Assembly.Location,
            typeof(System.Data.Linq.Binary).Assembly.Location,
            typeof(EnvDTE.BuildEventsClass).Assembly.Location,
            AppDomain.CurrentDomain.BaseDirectory+"DataverseGen.Core.dll"
        };

        /// <summary>
        ///Error message
        /// </summary>
        public CompilerErrorCollection Errors { get; set; }

        /// <summary>
        ///File extension
        /// </summary>
        public Encoding FileEncoding { get; set; }

        /// <summary>
        ///File extension
        /// </summary>
        public string FileExtension { get; set; }

        public List<string> LocalDlls { get => _localDlls; set => _localDlls = value; }

        public ITextTemplatingSession Session { get; set; }

        public IList<string> StandardAssemblyReferences => LocalDlls;

        public IList<string> StandardImports { get; } = new List<string>
        {
            "System",
            "DataverseGen.Core",
            "DataverseGen.Core.Metadata"
        };

        /// <summary>
        ///Template file
        /// </summary>
        public string TemplateFile { get; set; }
        public ITextTemplatingSession CreateSession()
        {
            return Session;
        }

        public object GetHostOption(string optionName)
        {
            switch (optionName)
            {
                case "CacheAssemblies":
                    return true;

                default:
                    return null;
            }
        }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = string.Empty;
            location = string.Empty;
            if (!File.Exists(requestFileName))
            {
                return false;
            }
            else
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }
        }
        public void LogErrors(CompilerErrorCollection errors)
        {
            Errors = errors;
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return AppDomain.CreateDomain("Generation App Domain");
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }

            string foundDll = LocalDlls.SingleOrDefault(dll => dll.Contains($"{assemblyReference}.dll"));
            if (!string.IsNullOrWhiteSpace(foundDll))
            {
                return foundDll;
            }
            string candidate = Path.Combine(
                Path.GetDirectoryName(TemplateFile) ?? throw new InvalidOperationException($"Path not found for template file {TemplateFile}"),
                assemblyReference);
            return File.Exists(candidate) ? candidate : $"error.{assemblyReference}";
        }

        public Type ResolveDirectiveProcessor(string processorName)
        {
            if (string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //return typeof();
            }
            throw new Exception("Directive Processor not found");
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException(nameof(directiveId), "the directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException(nameof(processorName), "the processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName), "the parameterName cannot be null");
            }
            return string.Empty;
        }

        public string ResolvePath(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName), "the file name cannot be null");
            }
            if (File.Exists(fileName))
            {
                return fileName;
            }
            string candidate = Path.Combine(Path.GetDirectoryName(TemplateFile) ?? throw new InvalidOperationException($"Path not found for template file {TemplateFile}"), fileName);
            return File.Exists(candidate) ? candidate : fileName;
        }
        public void SetFileExtension(string extension)
        {
            FileExtension = extension;
        }

        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            FileEncoding = encoding;
        }
    }
}