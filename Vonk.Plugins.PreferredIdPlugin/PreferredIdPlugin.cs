//using Hl7.Fhir.Model;

using Hl7.Fhir.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Linq;

using Vonk.Core.Context;
using Vonk.Core.Repository;
using Vonk.Plugins.PreferredIdPlugin.Exceptions;
using Vonk.Plugins.PreferredIdPlugin.Extensions;

using static Hl7.Fhir.Model.NamingSystem;

namespace Vonk.Plugins.PreferredIdPlugin;

internal class PreferredIdPlugin
{
	private readonly IAdministrationSearchRepository _administrationSearchRepository;
	private readonly ILogger<PreferredIdPlugin> _logger;

	public PreferredIdPlugin(IAdministrationSearchRepository administrationSearchRepository, ILogger<PreferredIdPlugin> logger)
	{
		_administrationSearchRepository = administrationSearchRepository;
		_logger = logger;
	}

	/// <summary>
	/// Sets Payload to UniqueId as Parameter 
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public async System.Threading.Tasks.Task ResolvePreferredId(IVonkContext context)
	{
		_logger.LogDebug("preferred-id get started");

		UniqueIdComponent uid;

		try
		{
			var pocos = await _administrationSearchRepository.FindNamingSystemByUniqueId(context.Arguments.GetArgument("id").ArgumentValue, $"{context.HttpContext().Request.Scheme}://{context.HttpContext().Request.Host}");

			uid = GetRequieredUidByType(pocos, context.Arguments.GetArgument("type").ArgumentValue.ToLower());

			context.Arguments.Handled();
		}
		catch (NamingSystemException ex)
		{
			_logger.LogDebug(ex.Message);
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.NotFound, "", ex.Message);
			context.Response.HttpResult = StatusCodes.Status404NotFound;
			return;
		}

		context.Response.Payload = uid.ToParameters();
		context.Response.HttpResult = StatusCodes.Status200OK;

		_logger.LogDebug("preferred-id get ended");
	}

	private static UniqueIdComponent GetRequieredUidByType(NamingSystem pocos, string type)
	{
		var result = pocos.UniqueId.FirstOrDefault(u => u.Type.HasValue && u.Type.ToString().ToLower() == type);

		if (result is null)
			throw new NamingSystemException($"UniqueId of requiered type was not fount in {pocos.Id}");

		return result;
	}
}