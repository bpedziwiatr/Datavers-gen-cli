using Scriban;
using System;
using System.IO;

namespace DataverseGen.Core.Generators.Scriban
{
    public class MultipleFileTemplate
    {
        private readonly string _templateName;

        public MultipleFileTemplate(string templateName)
        {
            _templateName = templateName;
            switch (_templateName)
            {
                case "Main":
                    SetMainTemplate();
                    break;

                default:
                    SetTemplateByFiles();
                    break;
            }
        }

        public Template EntityTemplate { get; private set; }

        public Template EnumsTemplate { get; private set; }

        public Template FieldsTemplate { get; private set; }

        public Template XrmContextTemplate { get; private set; }

        private void SetMainTemplate()
        {
            EntityTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_entity));
            FieldsTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_fields));
            EnumsTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_enums));
            XrmContextTemplate =
                Template.Parse(System.Text.Encoding.UTF8.GetString(ScribanTemplates.Main_xrmservicecontext));
        }

        private void SetTemplateByFiles()
        {
            CheckFiles();
            EntityTemplate =
                Template.Parse(File.ReadAllText(_entityTemplateFilePath));
            FieldsTemplate= Template.Parse(File.ReadAllText(_fieldsTemplateFilePath));
            EnumsTemplate= Template.Parse(File.ReadAllText(_enumsTemplateFilePath));
            XrmContextTemplate= Template.Parse(File.ReadAllText(_xrmServiceContextTemplateFilePath));
        }

        private string _entityTemplateFilePath;
        private string _enumsTemplateFilePath;
        private string _fieldsTemplateFilePath;
        private string _xrmServiceContextTemplateFilePath;
        private void CheckFiles()
        {
            string dir = $"{Directory.GetCurrentDirectory()}\\Templates\\{_templateName}\\";
            Console.WriteLine($"Loading templates from  path: {dir}");
            if (!Directory.Exists(dir))
            {
                throw  new ArgumentException($"Template '{_templateName}' not found in directory {dir}");
            }

            _entityTemplateFilePath= $"{dir}\\entity.sbncs";
            _enumsTemplateFilePath = $"{dir}\\enums.sbncs";
            _fieldsTemplateFilePath= $"{dir}\\fields.sbncs";
            _xrmServiceContextTemplateFilePath= $"{dir}\\xrmservicecontext.sbncs";
            if (!File.Exists(_entityTemplateFilePath))
            {
                throw  new ArgumentException($"Template missing file for entity '{_entityTemplateFilePath}'");
            }
            if (!File.Exists(_enumsTemplateFilePath))
            {
                throw  new ArgumentException($"Template missing file for enums '{_enumsTemplateFilePath}'");
            }
            if (!File.Exists(_fieldsTemplateFilePath))
            {
                throw  new ArgumentException($"Template missing file for fields '{_fieldsTemplateFilePath}'");
            }
            if (!File.Exists(_xrmServiceContextTemplateFilePath))
            {
                throw  new ArgumentException($"Template missing file for xrmServiceContext '{_xrmServiceContextTemplateFilePath}'");
            }
        }
    }
}