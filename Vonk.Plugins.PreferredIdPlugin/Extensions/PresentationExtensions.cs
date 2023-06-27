using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using Vonk.Core.Common;
using Vonk.Fhir.R4;

using static Hl7.Fhir.Model.NamingSystem;

namespace Vonk.Plugins.PreferredIdPlugin.Extensions;
internal static class PresentationExtensions
{
	public static IResource ToParameters(this UniqueIdComponent uniqueId)
	{
		return SourceNode.Resource(new Parameters().TypeName, "Parameters",
				SourceNode.Node("parameter",
					SourceNode.Valued("name", "result"),
					SourceNode.Valued("valueString", uniqueId.Value)
				)).ToIResource(VonkConstants.Model.FhirR4);
	}
}