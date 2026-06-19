using System;
using System.Linq;

namespace DataverseGen.Core.Config;

public static class ConfigValidation
{
	public static void Validate(ConfigModel config)
	{
		if (config == null)
		{
			throw new ArgumentNullException(nameof(config));
		}

		if (!HasCustomApis(config.CustomApis))
		{
			return;
		}

		if (config.TemplateEngine == null ||
			!string.Equals(config.TemplateEngine.Type, "ts", StringComparison.InvariantCultureIgnoreCase))
		{
			throw new InvalidOperationException("CustomApis can only be used when TemplateEngine.Type is set to 'ts'.");
		}
	}

	private static bool HasCustomApis(string[] customApis)
	{
		return customApis != null && customApis.Any(api => !string.IsNullOrWhiteSpace(api));
	}
}
