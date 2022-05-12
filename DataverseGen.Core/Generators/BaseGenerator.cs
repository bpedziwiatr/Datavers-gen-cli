﻿using DataverseGen.Core.Config;
using DataverseGen.Core.Metadata;

namespace DataverseGen.Core.Generators
{
    public abstract class BaseGenerator
    {
        protected readonly TemplateEngineModel TemplateEngineModel;
        protected readonly Context Context;
        protected readonly string OutPath;
        protected readonly string TemplateName;
        protected readonly string TemplateDirName;


        protected BaseGenerator(
            string templateName,
            string templateDirName,
            string outPath,
            Context context,
            TemplateEngineModel templateEngineModel
            )
        {
            TemplateEngineModel = templateEngineModel;
            TemplateDirName = templateDirName;
            TemplateName = templateName;
            OutPath = outPath;
            Context = context;
        }

        public abstract void GenerateTemplate();

    }
}