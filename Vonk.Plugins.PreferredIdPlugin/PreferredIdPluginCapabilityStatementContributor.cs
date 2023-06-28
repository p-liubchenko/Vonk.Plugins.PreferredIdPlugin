using Microsoft.Extensions.Options;

using Vonk.Core.Common;
using Vonk.Core.Context.Guards;
using Vonk.Core.Metadata;
using Vonk.Core.Model.Capability;
using Vonk.Core.Pluggability.ContextAware;
using Vonk.Core.Support;

namespace Vonk.Plugins.PreferredIdPlugin;

[ContextAware(InformationModels = new[] { VonkConstants.Model.FhirR4 })]
public sealed class PreferredIdPluginCapabilityStatementContributor : ICapabilityStatementContributor
{

	private const string _operationName = "preferred-id";
	private readonly SupportedInteractionOptions _supportedInteractionOptions;

	public PreferredIdPluginCapabilityStatementContributor(IOptions<SupportedInteractionOptions> options)
	{
		_supportedInteractionOptions = options.Value;
	}

	public void ContributeToCapabilityStatement(ICapabilityStatementBuilder builder)
	{
		Check.NotNull(builder, nameof(builder));
		if (_supportedInteractionOptions.SupportsCustomOperation(_operationName))
		{
			builder.UseRestComponentEditor(rce =>
			{
				rce.AddOperation(_operationName, "http://hl7.org/fhir/OperationDefinition/NamingSystem-preferred-id");
			});
		}
	}
}