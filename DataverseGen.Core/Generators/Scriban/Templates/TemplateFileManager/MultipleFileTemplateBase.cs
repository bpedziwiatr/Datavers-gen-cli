using Scriban;
using System;
using System.IO;

namespace DataverseGen.Core.Generators.Scriban.Templates.TemplateFileManager
{
    public abstract class MultipleFileTemplateBase
    {
        protected string DirectoryPath;
        private readonly string _templateName;
        private readonly string _templateDirName;
        private string _entityTemplateFilePath;
        private string _enumsTemplateFilePath;
        private const string RestrictedTemplateMain = "Main";

        protected MultipleFileTemplateBase(string templateName, string templateDirName)
        {
            _templateName = templateName;
            _templateDirName = templateDirName;
        }

        internal Template EntityTemplate { get; set; }

        internal Template EnumsTemplate { get; set; }

        public void Init()
        {
            if (string.Equals(_templateName, RestrictedTemplateMain, StringComparison.InvariantCultureIgnoreCase))
            {
                SetMainTemplate();
                return;
            }
            SetTemplateByFiles();
        }

        protected virtual void CheckFiles()
        {
            DirectoryPath = $"{Directory.GetCurrentDirectory()}\\{_templateDirName}\\{_templateName}\\";
            Console.WriteLine($@"Loading templates from  path: {DirectoryPath}");
            if (!Directory.Exists(DirectoryPath))
            {
                throw new ArgumentException($"Template '{_templateName}' not found in directory {DirectoryPath}");
            }

            _entityTemplateFilePath = $"{DirectoryPath}\\entity.sbncs";
            _enumsTemplateFilePath = $"{DirectoryPath}\\enums.sbncs";

            if (!File.Exists(_entityTemplateFilePath))
            {
                throw new ArgumentException($"Template missing file for entity '{_entityTemplateFilePath}'");
            }
            if (!File.Exists(_enumsTemplateFilePath))
            {
                throw new ArgumentException($"Template missing file for enums '{_enumsTemplateFilePath}'");
            }
        }

        protected virtual void SetMainTemplate()
        {
            EntityTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_entity));

            EnumsTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_enums));
        }

        protected virtual void SetTemplateByFiles()
        {
            CheckFiles();
            EntityTemplate =
                Template.Parse(File.ReadAllText(_entityTemplateFilePath));

            EnumsTemplate = Template.Parse(File.ReadAllText(_enumsTemplateFilePath));
        }
    }
}