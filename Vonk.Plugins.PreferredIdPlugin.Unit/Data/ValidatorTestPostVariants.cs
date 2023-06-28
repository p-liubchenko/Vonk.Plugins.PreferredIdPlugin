using Hl7.Fhir.Model;

using System.Collections;

using Vonk.Core.Context;
using Vonk.Fhir.R4;
using Vonk.Plugins.PreferredIdPlugin.Unit.Context;

namespace Vonk.Plugins.PreferredIdPlugin.Unit.Data;
internal class ValidatorTestPostVariants : IEnumerable<object[]>
{

	VonkTestContext withTypeOnly = new();
	VonkTestContext withIdOnly = new();
	VonkTestContext valid = new();

	public ValidatorTestPostVariants()
	{
		withTypeOnly.Request.Payload = new Parameters().Add("type", new FhirString("any")).ToIResource().ToPayload();
		withIdOnly.Request.Payload = new Parameters().Add("id", new FhirString("any")).ToIResource().ToPayload();
		valid.Request.Payload = new Parameters().Add("id", new FhirString("any"))
			.Add("type", new FhirString("any")).ToIResource().ToPayload();
	}

	public IEnumerator<object[]> GetEnumerator()
	{

		yield return new object[] { withTypeOnly, false };

		yield return new object[] { withIdOnly, false };

		yield return new object[] { valid, true };
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}