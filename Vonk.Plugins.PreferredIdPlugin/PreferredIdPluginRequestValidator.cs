using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using Microsoft.AspNetCore.Http;

using System.Linq;

using Vonk.Core.Context;
using Vonk.Core.Support;

namespace Vonk.Plugins.PreferredIdPlugin;
public class PreferredIdPluginRequestValidator
{

	public void ValidateGetRequest(IVonkContext context)
	{
		if (!context.Arguments.HasAny(x => x.ArgumentName == "id"))
		{
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.Value, "", "Id parameter is requiered");
			context.Response.HttpResult = StatusCodes.Status400BadRequest;
		}

		if (!context.Arguments.HasAny(x => x.ArgumentName == "type"))
		{
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.Value, "", "Type parameter not found");
			context.Response.HttpResult = StatusCodes.Status400BadRequest;
		}
	}

	public void ValidatePostRequest(IVonkContext context)
	{
		var body = context.Request.Payload.Resource.ToPoco<Parameters>();

		if (!body.Parameter.HasAny(x => x.Name == "id"))
		{
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.Value, "", "Id parameter is requiered");
			context.Response.HttpResult = StatusCodes.Status400BadRequest;
		}
		else
			context.Arguments.AddArgument(new Argument(ArgumentSource.Query, "id", body.Parameter.First(x => x.Name == "id").Value.ToString()));

		if (!body.Parameter.HasAny(x => x.Name == "type"))
		{
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.Value, "", "Type parameter not found");
			context.Response.HttpResult = StatusCodes.Status400BadRequest;
		}
		else
			context.Arguments.AddArgument(new Argument(ArgumentSource.Query, "type", body.Parameter.First(x => x.Name == "type").Value.ToString()));
	}
}