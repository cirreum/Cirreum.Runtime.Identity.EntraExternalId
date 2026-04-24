# Cirreum Runtime Identity EntraExternalId

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Identity.EntraExternalId.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity.EntraExternalId/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Identity.EntraExternalId.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity.EntraExternalId/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Identity.EntraExternalId?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Identity.EntraExternalId/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Identity.EntraExternalId?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Identity.EntraExternalId/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Runtime Extensions package for Cirreum Identity EntraExternalId — the app-facing entry point for the Microsoft Entra External ID custom-claims provisioning callback.**

## Overview

Install this package when your application uses Microsoft Entra External ID (CIAM) with a custom authentication extension (`onTokenIssuanceStart`) that calls into Cirreum for user provisioning. Install the umbrella `Cirreum.Runtime.Identity` instead if you need multiple identity provider protocols (e.g. Oidc + Entra External ID).

This package contributes two extension methods:

- `builder.AddEntraExternalIdIdentity(configure?)` — registers the Entra External ID provider and, via the optional callback, app-provided `IUserProvisioner` implementations keyed per configured instance.
- `app.MapEntraExternalIdIdentity()` — maps the provisioning routes for every enabled Entra External ID instance.

## Installation

```
dotnet add package Cirreum.Runtime.Identity.EntraExternalId
```

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddEntraExternalIdIdentity(p => p
    .AddProvisioner<EmployeeProvisioner>("primary"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapEntraExternalIdIdentity();

app.Run();
```

### App-provided provisioner class

Derive from the base that matches the instance's onboarding model:

```csharp
using Cirreum.Identity.Provisioning;

public sealed class EmployeeProvisioner(AppDbContext db)
    : InvitationUserProvisionerBase<EmployeeUser> {

    protected override Task<EmployeeUser?> FindUserAsync(string externalUserId, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId, ct);

    protected override async Task<EmployeeUser?> RedeemInvitationAsync(
        string email, string externalUserId, CancellationToken ct) {
        // atomically find, validate, and claim the invitation; create user record
    }
}
```

See `Cirreum.IdentityProvider` for the full provisioner hierarchy (`UserProvisionerBase<TUser>`, `InvitationUserProvisionerBase<TUser>`, `SelfServiceUserProvisionerBase<TUser>`) and `Cirreum.Identity.EntraExternalId` for the Entra External ID wire contract, configuration keys, Azure Portal setup, and security model.

## Configuration

```json
{
  "Cirreum": {
    "Identity": {
      "Providers": {
        "EntraExternalId": {
          "Instances": {
            "primary": {
              "Enabled": true,
              "Route": "/auth/entra/provision",
              "ClientId": "<app-registration-client-id>",
              "Issuer": "https://<tenant-id>.ciamlogin.com/<tenant-id>/v2.0",
              "MetadataEndpoint": "https://<tenant-id>.ciamlogin.com/<tenant-id>/v2.0/.well-known/openid-configuration",
              "AllowedAppIds": "<allowed-client-app-guid>"
            }
          }
        }
      }
    }
  }
}
```

> **Issuer format gotcha:** the tenant-ID subdomain form is required — e.g.
> `https://<tenant-id>.ciamlogin.com/<tenant-id>/v2.0`. The domain-name form
> (e.g. `yourtenant.ciamlogin.com`) causes silent token-validation failure.

See [`Cirreum.Identity.EntraExternalId`](https://www.nuget.org/packages/Cirreum.Identity.EntraExternalId/) for the full per-instance settings reference and Azure Portal setup guide.

## Dependencies

- **Cirreum.Runtime.IdentityProvider** — the `RegisterIdentityProvider<>` helper, `IIdentityBuilder`, and `IdentityProviderMapping` types
- **Cirreum.Identity.EntraExternalId** — the Entra External ID registrar, handler, token validator, and settings

## Multi-protocol apps

If you need both Oidc and Entra External ID, install the umbrella `Cirreum.Runtime.Identity` instead — it exposes `builder.AddIdentity(configure?)` / `app.MapIdentity()` which register both providers and map all their routes.

## License

MIT — see [LICENSE](LICENSE).

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*
