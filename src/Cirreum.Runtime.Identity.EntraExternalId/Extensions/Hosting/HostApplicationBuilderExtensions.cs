namespace Microsoft.Extensions.Hosting;

using Cirreum.Identity;
using Cirreum.Identity.Configuration;
using Cirreum.Identity.Provisioning;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App-facing extensions for registering the Cirreum Identity EntraExternalId provider.
/// </summary>
public static class HostApplicationBuilderExtensions {

	private sealed class AddEntraExternalIdIdentityMarker { }

	/// <summary>
	/// Registers the Cirreum Identity EntraExternalId provider — a custom authentication
	/// extension (<c>onTokenIssuanceStart</c>) endpoint that validates Entra-signed tokens,
	/// provisions users, and returns custom claims for the issued token. Binds instances
	/// from <c>Cirreum:Identity:Providers:EntraExternalId:Instances:*</c>.
	/// </summary>
	/// <param name="builder">The host application builder.</param>
	/// <param name="configure">
	/// Optional callback to register per-instance <see cref="IUserProvisioner"/>
	/// implementations using the fluent <see cref="IIdentityBuilder.AddProvisioner{TProvisioner}"/>
	/// method.
	/// </param>
	/// <returns>The host application builder for chaining.</returns>
	/// <example>
	/// <code>
	/// builder.AddEntraExternalIdIdentity(p => p
	///     .AddProvisioner&lt;EmployeeProvisioner&gt;("primary"));
	/// </code>
	/// </example>
	public static IHostApplicationBuilder AddEntraExternalIdIdentity(
		this IHostApplicationBuilder builder,
		Action<IIdentityBuilder>? configure = null) {

		// Marker dedup — provider registration runs once regardless of repeat calls.
		// The configure callback always runs so repeated calls can still add provisioners.
		if (!builder.Services.IsMarkerTypeRegistered<AddEntraExternalIdIdentityMarker>()) {
			builder.Services.MarkTypeAsRegistered<AddEntraExternalIdIdentityMarker>();

			builder.RegisterIdentityProvider<
				EntraExternalIdIdentityProviderRegistrar,
				EntraExternalIdIdentityProviderSettings,
				EntraExternalIdIdentityProviderInstanceSettings>();
		}

		configure?.Invoke(new IdentityBuilder(builder));
		return builder;
	}
}
