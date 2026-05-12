# Datavers-gen-cli
Power Platform Dataverse early bound generator based on T4 template as cli with GUI options.

## C# Template Dataverse Configuration Example

```json
{
  "Entities": [
    "account", 
    "contact"
  ],
  "ConnectionString": "AuthType=ClientSecret;Url={url};ClientId={ClientId};ClientSecret={ClientSecret}",
  "Namespace": "sad.Dataverse.DataAccess.Entities",
  "OutDirectory": "Dataverse",
  "TemplateName": "Main", //Template get from Dataverse-Gen.Cli
  "TemplateEngine": {
    "IsSingleOutput": false,
    "Name": "scriban",
    "Type": "C#"
  }
}
```

## TypeScript Template Dataverse Configuration Example

```json
{
  "Entities": [
    "account", 
    "contact"
  ],
  "ConnectionString": "AuthType=ClientSecret;Url={url};ClientId={ClientId};ClientSecret={ClientSecret}",
  "Namespace": "sad.dataverse.ui.webresource",
  "OutDirectory": "/src/dataversegen/",
  "TemplateName": "dataverse-template",
  "EnableConnectionStringValidation": true,
  "ThrowOnEntityNotFound": true,
  "TemplateDirectoryName": "template",
  "TemplateEngine": {
    "IsSingleOutput": false,
    "Name": "scriban",
    "Type": "ts"
  }
}
```

## Templates configuration

### Build in template

To use build in template configure TemplateName params as **Main**.
```json
{
    "TemplateName": "Main"
}
```

### Project Templates

To use project template you need to copy T4 Templates files to project, where the configuration file exist in folder **template**.

**Folder structure**

- sad.Dataverse.DataAccess.Entities
    - Template
        - Dataverse-Template
            - _t4_template_files_
    - dataversegen.config.json

```json
{
    "TemplateName": "Dataverse-Template"
}
```

## Params

- **Entities** - array with schema name list of generated entities
- **ConnectionString** - connection string use to connetct to the specific Dataverse. Recomended use ClientSecret.
- **Namespace** - namespace generated with the entities
- **OutDirectory** - folder where file are generated
- **TemplateName** - name of template used in generated files.
- **TemplateDirectoryName** - folder name where the folder with templates exist.
- **EnableConnectionStringValidation** - validate if connection string is correct.
- **ThrowOnEntityNotFound** - throw error if generator cannot find a record specify in entities array.
- **TemplateEngine** - configuration what should generate
    - **IsSingleOutput** - true/false. If true then generated all records in one file. 
    - **Name** - scriban
    - **Type** - type of generated values. Use: C# or ts