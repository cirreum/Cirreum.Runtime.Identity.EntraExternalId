namespace Microsoft.AspNetCore.Builder;

using Cirreum.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App-facing extensions for mapping the Cirreum Identity EntraExternalId provisioning
/// endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions {

	private const string EntraExternalIdProviderName = "EntraExternalId";

	/// <summary>
	/// Maps all enabled Cirreum Identity EntraExternalId provisioning routes declared in
	/// configuration. Invokes each registered <see cref="IdentityProviderMapping"/> whose
	/// <see cref="IdentityProviderMapping.ProviderName"/> is <c>"EntraExternalId"</c>.
	/// </summary>
	/// <param name="endpoints">The endpoint route builder.</param>
	/// <returns>The endpoint route builder for chaining.</returns>
	/// <remarks>
	/// Call after <c>app.UseAuthentication()</c> / <c>app.UseAuthorization()</c>. Each
	/// instance's route is mapped as anonymous (<c>AllowAnonymous</c>) and excluded from
	/// OpenAPI — inbound token validation against Entra's OIDC discovery metadata is
	/// performed internally by the Entra External ID handler.
	/// </remarks>
	public static IEndpointRouteBuilder MapEntraExternalIdIdentity(this IEndpointRouteBuilder endpoints) {

		var mappings = endpoints.ServiceProvider.GetServices<IdentityProviderMapping>();
		foreach (var mapping in mappings) {
			if (mapping.ProviderName == EntraExternalIdProviderName) {
				mapping.Map(endpoints);
			}
		}

		return endpoints;
	}
}
