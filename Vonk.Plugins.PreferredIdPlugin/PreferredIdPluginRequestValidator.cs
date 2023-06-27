using Microsoft.AspNetCore.Http;

using Vonk.Core.Context;
using Vonk.Core.Support;

namespace Vonk.Plugins.PreferredIdPlugin;
internal class PreferredIdPluginRequestValidator
{

	public void ValidateRequest(IVonkContext context)
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
}