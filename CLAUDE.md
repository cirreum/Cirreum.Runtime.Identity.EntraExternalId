# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **Cirreum.Runtime.Identity.EntraExternalId**, the Runtime Extensions package for the Cirreum Identity EntraExternalId provider. Contributes two app-facing extension methods that wrap the config-driven registration plumbing from `Cirreum.Runtime.IdentityProvider`:

- `AddEntraExternalIdIdentity()` on `IHostApplicationBuilder` — services-phase registration of the Entra External ID registrar + optional `IIdentityBuilder` callback for provisioner registration.
- `MapEntraExternalIdIdentity()` on `IEndpointRouteBuilder` — endpoints-phase mapping, filtered to Entra External ID instances only.

## Build Commands

```bash
dotnet build Cirreum.Runtime.Identity.EntraExternalId.slnx
dotnet pack --configuration Release
```

## Architecture

### What this package does

1. **`AddEntraExternalIdIdentity(builder, configure?)`** (`Extensions/Hosting/HostApplicationBuilderExtensions.cs`)
   - Marker-type dedup via `AddEntraExternalIdIdentityMarker` — provider registration runs once even across repeat calls.
   - Calls `builder.RegisterIdentityProvider<EntraExternalIdIdentityProviderRegistrar, EntraExternalIdIdentityProviderSettings, EntraExternalIdIdentityProviderInstanceSettings>()` from Layer 4.
   - Invokes the optional `Action<IIdentityBuilder>` callback so apps can call `.AddProvisioner<T>(key)` per configured instance.
   - Namespace `Microsoft.Extensions.Hosting` so consumers get it for free.

2. **`MapEntraExternalIdIdentity(endpoints)`** (`Extensions/Builder/EndpointRouteBuilderExtensions.cs`)
   - Resolves `IEnumerable<IdentityProviderMapping>` from DI.
   - Filters to `ProviderName == "EntraExternalId"` and invokes each mapping's deferred `Map(endpoints)` closure.
   - Namespace `Microsoft.AspNetCore.Builder` (where Map*/MapGet live by convention).

### What this package does NOT do

- **Does not duplicate or re-implement the Entra External ID registrar, handler, token validator, or settings types** — those all live in `Cirreum.Identity.EntraExternalId` (Infra layer).
- **Does not register `IUserProvisioner`** — that's the app's job, via the `IIdentityBuilder.AddProvisioner<T>(key)` callback.

## Project Structure

```
src/Cirreum.Runtime.Identity.EntraExternalId/
├── Extensions/
│   ├── Hosting/
│   │   └── HostApplicationBuilderExtensions.cs   # AddEntraExternalIdIdentity
│   └── Builder/
│       └── EndpointRouteBuilderExtensions.cs     # MapEntraExternalIdIdentity
└── Cirreum.Runtime.Identity.EntraExternalId.csproj
```

`RootNamespace` = `Cirreum.Runtime` (matches sibling Runtime Extensions packages), but extension classes override to `Microsoft.Extensions.Hosting` / `Microsoft.AspNetCore.Builder` for discoverability.

## Dependencies

- **Cirreum.Runtime.IdentityProvider** (v1.0.1+) — registration helper, `IIdentityBuilder` + `IdentityBuilder`, `IdentityProviderMapping`
- **Cirreum.Identity.EntraExternalId** (v2.0.1+) — Entra External ID registrar + settings types (referenced by the `RegisterIdentityProvider<>` generic arguments)
- **Microsoft.AspNetCore.App**

## Umbrella vs per-protocol

The umbrella `Cirreum.Runtime.Identity` exposes `AddIdentity()` / `MapIdentity()` which compose this package and `Cirreum.Runtime.Identity.Oidc`. Apps that need only Entra External ID install this package directly; apps that need multi-protocol install the umbrella.

## Development Notes

- Uses .NET 10.0 with latest C# language version
- Nullable reference types enabled
- Thin wrapper around Layer 4's `RegisterIdentityProvider<>` helper — no protocol-specific logic lives here
- File-scoped namespaces
- K&R braces, tabs for indentation (matches repo `.editorconfig`)
