# Datavers-gen-cli

Dataverse early-bound generator for Power Platform, built as a .NET 7 CLI on top of Scriban templates.

## What Changed Recently

- The CLI now reads `dataversegen.config.json` from the current working directory.
- If `ConnectionString` is missing, the app falls back to Windows Credential Manager and prompts for one.
- The Scriban generator supports both `C#` and `ts` output types.
- Generation can run in single-file or multi-file mode.
- TypeScript generation also emits custom API request wrappers under `customapi/`.
- Built-in templates are available through `TemplateName = "Main"`, and project templates can be copied under a `Templates` folder.
- Release builds are packaged into self-contained and framework-dependent ZIP archives by `build.ps1`.

## Current Behavior

- The app loads Dataverse metadata for the entities listed in `Entities`.
- If `CustomApis` is set, TypeScript output is narrowed to the listed custom API unique names.
- `CustomApis` is only valid when `TemplateEngine.Type` is `ts`.
- It validates the connection string when `EnableConnectionStringValidation` is enabled.
- It can throw when an entity from `Entities` is missing if `ThrowOnEntityNotFound` is enabled.
- `TemplateEngine.Name` currently supports `scriban`.
- `TemplateEngine.Type` accepts `C#` or `ts`.
- When `TemplateEngine.IsSingleOutput` is `true`, the generator writes one output file per target.
- When `TemplateEngine.IsSingleOutput` is `false`, the generator writes multiple files per entity.

## Quick Start

1. Create `dataversegen.config.json` in the directory where you want to run the generator.
2. Fill in the Dataverse connection string, entity names, namespace, and output directory.
3. Run the CLI from that directory.

Example:

```powershell
dotnet run --project DataverseGen.Cli
```

## Configuration Example

### C# output

```json
{
  "Entities": [
    "account",
    "contact"
  ],
  "ConnectionString": "AuthType=ClientSecret;Url={url};ClientId={ClientId};ClientSecret={ClientSecret}",
  "Namespace": "sad.Dataverse.DataAccess.Entities",
  "OutDirectory": "Dataverse",
  "TemplateName": "Main",
  "TemplateDirectoryName": "Templates",
  "EnableConnectionStringValidation": true,
  "ThrowOnEntityNotFound": true,
  "TemplateEngine": {
    "IsSingleOutput": false,
    "Name": "scriban",
    "Type": "C#"
  }
}
```

### TypeScript output

```json
{
  "Entities": [
    "account",
    "contact"
  ],
  "CustomApis": [
    "newupdatestatus"
  ],
  "ConnectionString": "AuthType=ClientSecret;Url={url};ClientId={ClientId};ClientSecret={ClientSecret}",
  "Namespace": "sad.dataverse.ui.webresource",
  "OutDirectory": "/src/dataversegen/",
  "TemplateName": "dataverse-template",
  "TemplateDirectoryName": "Templates",
  "EnableConnectionStringValidation": true,
  "ThrowOnEntityNotFound": true,
  "TemplateEngine": {
    "IsSingleOutput": true,
    "Name": "scriban",
    "Type": "ts"
  }
}
```

## Custom API for TypeScript

Custom API request wrappers are generated automatically for TypeScript output.
Configuration is minimal:

- set `TemplateEngine.Type` to `ts`
- keep the normal Dataverse connection string and output directory configuration
- optionally set `CustomApis` to a list of custom API unique names to generate
- make sure the environment contains `customapi`, `customapirequestparameter`, and `customapiresponseproperty` metadata

No extra flag is required in `dataversegen.config.json` and there is no separate custom API switch.
The generator reads the Dataverse metadata and writes TypeScript wrappers into:

```text
<OutDirectory>/customapi/
```

Example TypeScript config that enables the feature:

```json
{
  "Entities": [
    "account",
    "contact"
  ],
  "CustomApis": [
    "newupdatestatus"
  ],
  "ConnectionString": "AuthType=ClientSecret;Url={url};ClientId={ClientId};ClientSecret={ClientSecret}",
  "Namespace": "sad.dataverse.ui.webresource",
  "OutDirectory": "/src/dataversegen/",
  "TemplateName": "dataverse-template",
  "TemplateDirectoryName": "Templates",
  "EnableConnectionStringValidation": true,
  "ThrowOnEntityNotFound": true,
  "TemplateEngine": {
    "IsSingleOutput": true,
    "Name": "scriban",
    "Type": "ts"
  }
}
```

The generated wrapper follows the Web API request contract and uses:

- the custom API unique name as the operation name
- request parameter names from Dataverse metadata
- `boundParameter` metadata when the custom API is bound to an entity

If `CustomApis` is omitted or empty, all available custom APIs are generated.

If you only generate `C#`, custom API wrappers are not emitted.

### Filtering custom APIs

Use `CustomApis` to limit TypeScript generation to specific custom API unique names:

```json
{
  "CustomApis": [
    "newupdatestatus",
    "newregistercustomer"
  ]
}
```

The names are matched against the Dataverse `uniquename` field, case-insensitively.

## Configuration Fields

- `Entities` - Dataverse schema names to generate.
- `CustomApis` - Custom API unique names to generate. If omitted or empty, all custom APIs are generated for TypeScript output. Requires `TemplateEngine.Type = ts`.
- `ConnectionString` - Dataverse connection string. If omitted, the CLI asks for it and stores it in Windows Credential Manager.
- `Namespace` - Namespace used in generated code.
- `OutDirectory` - Output folder for generated files.
- `TemplateName` - Template set to use. `Main` selects the built-in templates.
- `TemplateDirectoryName` - Folder name that contains project templates.
- `EnableConnectionStringValidation` - Enables connection string validation. Default: `false`.
- `ThrowOnEntityNotFound` - Throws when an entity from `Entities` is not found. Default: `false`.
- `TemplateEngine.Name` - Template engine name. Current value: `scriban`.
- `TemplateEngine.Type` - Output type. Use `C#` or `ts`.
- `TemplateEngine.IsSingleOutput` - Controls single-file or multi-file generation.

## Templates

### Built-in templates

Use the built-in template set with:

```json
{
  "TemplateName": "Main"
}
```

### Project templates

Copy the Scriban template files into a `Templates` folder next to your config file:

```text
MyProject
  Templates
    Dataverse-Template
      ...
  dataversegen.config.json
```

Then reference the template folder name in the config:

```json
{
  "TemplateName": "Dataverse-Template",
  "TemplateDirectoryName": "Templates"
}
```

## Release Build

- `pwsh -NoProfile -File build.ps1` publishes the CLI in Release mode.
- The script creates both `includedotnet` and `nodotnet` ZIP packages under `publish/`.
- Release automation also publishes those ZIPs as GitHub Release assets.

## Development Notes

- The solution targets `net7.0`.
- Run `dotnet test DataverseGen.sln` after changing generator logic.
- The test project is `Tests/DataverseGen.Core.Tests`.
