using System;

namespace DataverseGen.Core.Config;

public static class ConfigValidation
{
	public static void Validate(ConfigModel config)
	{
		if (config == null)
		{
			throw new ArgumentNullException(nameof(config));
		}

		if (config.TemplateEngine == null)
		{
			throw new InvalidOperationException("TemplateEngine must be configured.");
		}
	}
}
