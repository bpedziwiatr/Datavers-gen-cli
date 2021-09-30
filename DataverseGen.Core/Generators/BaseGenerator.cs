using DataverseGen.Core.Config;
using DataverseGen.Core.Metadata;

namespace DataverseGen.Core.Generators
{
    public abstract class BaseGenerator
    {
        protected readonly TemplateEngineModel _templateEngineModel;
        protected readonly Context _context;
        protected readonly string _outPath;
        protected readonly string _templateName;
        public BaseGenerator(string templateName, string outPath, Context context,TemplateEngineModel templateEngineModel)
        {
            _templateEngineModel = templateEngineModel;
            _templateName = templateName;
            _outPath = outPath;
            _context = context;
        }

        public abstract void GenerateTemplate();

    }
}