using Hl7.Fhir.Model;

using System.Collections;

using Vonk.Core.Context;
using Vonk.Fhir.R4;
using Vonk.Plugins.PreferredIdPlugin.Unit.Context;

namespace Vonk.Plugins.PreferredIdPlugin.Unit.Data;
internal class PluginSucessTestVariants : IEnumerable<object[]>
{
	VonkTestContext postRequestFound = new();

	VonkTestContext getRequestFound = new();

	public PluginSucessTestVariants()
	{
		postRequestFound.Request.Payload = new Parameters()
				.Add("type", new FhirString("oid"))
				.Add("id", new FhirString("http://hl7.org/fhir/sid/us-ssn"))
			.ToIResource()
			.ToPayload();
		postRequestFound.TestRequest.Method = "POST";

		getRequestFound.Arguments
			.AddArgument(new Argument(ArgumentSource.Query, "id", "http://hl7.org/fhir/sid/us-ssn", ArgumentStatus.NotHandled))
			.AddArgument(new Argument(ArgumentSource.Query, "type", "oid", ArgumentStatus.NotHandled));
		getRequestFound.TestRequest.Method = "GET";

	}

	public IEnumerator<object[]> GetEnumerator()
	{
		yield return new object[] { postRequestFound, 200, new Parameters().Add("result", new FhirString("2.16.840.1.113883.4.1")).ToIResource() };

		yield return new object[] { getRequestFound, 200, new Parameters().Add("result", new FhirString("2.16.840.1.113883.4.1")).ToIResource() };

	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class PluginNotFoundTestVariants : IEnumerable<object[]>
{
	VonkTestContext postRequestNotFound = new();

	VonkTestContext getRequestNotFound = new();

	public PluginNotFoundTestVariants()
	{

		postRequestNotFound.Request.Payload = new Parameters()
				.Add("id", new FhirString("1.3.160"))
				.Add("type", new FhirString("uri"))
			.ToIResource()
			.ToPayload();
		postRequestNotFound.TestRequest.Method = "POST";

		getRequestNotFound.Arguments
			.AddArgument(new Argument(ArgumentSource.Query, "id", "1.3.160", ArgumentStatus.NotHandled))
			.AddArgument(new Argument(ArgumentSource.Query, "type", "url", ArgumentStatus.NotHandled));
		getRequestNotFound.TestRequest.Method = "GET";
	}

	public IEnumerator<object[]> GetEnumerator()
	{

		yield return new object[] { postRequestNotFound, 404 };

		yield return new object[] { getRequestNotFound, 404 };
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}