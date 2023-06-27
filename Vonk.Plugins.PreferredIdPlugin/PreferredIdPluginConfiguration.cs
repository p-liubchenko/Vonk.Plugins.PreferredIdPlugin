using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Vonk.Core.Common;
using Vonk.Core.Metadata;
using Vonk.Core.Pluggability;
using Vonk.Core.Pluggability.ContextAware;

namespace Vonk.Plugins.PreferredIdPlugin;

[VonkConfiguration(order: 4600)]
public static class PreferredIdPluginConfiguration
{
	public static IServiceCollection AddPreferredIdPlugin(this IServiceCollection services)
	{
		services.TryAddTransient<PreferredIdPluginRequestValidator>();
		services.TryAddSingleton<PreferredIdPlugin>();
		services.TryAddContextAware<ICapabilityStatementContributor, PreferredIdPluginCapabilityStatementContributor>(ServiceLifetime.Transient);
		return services;
	}

	public static IApplicationBuilder AddPreferredIdPlugin(this IApplicationBuilder app)
	{
		app.OnCustomInteraction(Core.Context.VonkInteraction.type_custom, "preferred-id")
			.AndResourceTypes("NamingSystem")
			.AndMethod("GET")
			.AndInformationModel(VonkConstants.Model.FhirR4)
			.PreHandleWith<PreferredIdPluginRequestValidator>(
				(svc, ctx) => svc.ValidateRequest(ctx)
			);

		app.OnCustomInteraction(Core.Context.VonkInteraction.type_custom, "preferred-id")
			.AndResourceTypes("NamingSystem")
			.AndMethod("GET")
			.AndInformationModel(VonkConstants.Model.FhirR4)
			.HandleAsyncWith<PreferredIdPlugin>(
				(svc, ctx) => svc.ResolvePreferredId(ctx)
			);

		return app;
	}
}