# Extension for Umbraco Embedded Models Builder

* Extends "*.generated.cs" models with new static structure that lists Alias for each property belonging to the model.
At the moment only version 10.x of Umbraco is supported.

## Usage

* Simply call `.EnrichEmbeddedModelsBuilderWithModelPropsAliases()` on startup (available as extension to `IUmbracoBuilder`)