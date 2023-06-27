//using Hl7.Fhir.Model;

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

using Vonk.Core.Context;
using Vonk.Core.Repository;
using Vonk.Plugins.PreferredIdPlugin.Exceptions;
using Vonk.Plugins.PreferredIdPlugin.Extensions;

using static Hl7.Fhir.Model.NamingSystem;

namespace Vonk.Plugins.PreferredIdPlugin;

public class PreferredIdPlugin
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
			uid = await FindUidOfType(context.ServerBase, ResolveParameters(context));

			context.Arguments.Handled();

			context.Response.Payload = uid.ToParameters();
			context.Response.HttpResult = StatusCodes.Status200OK;
		}
		catch (NamingSystemException ex)
		{
			_logger.LogDebug(ex.Message);
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.NotFound, "", ex.Message);
			context.Response.HttpResult = StatusCodes.Status404NotFound;
			return;
		}
		catch (InvalidOperationException)
		{
			context.Response.HttpResult = StatusCodes.Status405MethodNotAllowed;
		}

		_logger.LogDebug("preferred-id get ended");
	}

	/// <summary>
	/// Searches for namingSystem and UniqueId which correspond to requested type
	/// </summary>
	/// <param name="serverUrl"></param>
	/// <param name="param"></param>
	/// <returns></returns>
	/// <exception cref="NamingSystemException"></exception>
	private async Task<UniqueIdComponent> FindUidOfType(Uri serverUrl, (string id, string type) param)
	{
		var namingSystem = await _administrationSearchRepository.FindNamingSystemByUniqueId(param.id, serverUrl);

		var result = namingSystem.UniqueId.FirstOrDefault(u => u.Type.HasValue && u.Type.ToString().ToLower() == param.type.ToLower());

		if (result is null)
			throw new NamingSystemException($"UniqueId of requiered type was not fount in {param.id}");

		return result;
	}

	/// <summary>
	/// Resolves request parameters depending on http method
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static (string id, string type) ResolveParameters(IVonkContext context)
	{
		if (context.Request.Method == HttpMethods.Get)
		{
			return new()
			{
				id = context.Arguments.GetArgument("id").ArgumentValue,
				type = context.Arguments.GetArgument("type").ArgumentValue
			};
		}

		if (context.Request.Method == HttpMethods.Post)
		{
			var body = context.Request.Payload.Resource.ToPoco<Parameters>();

			return new()
			{
				id = body.Parameter.First(x => x.Name == "id").Value.ToString(),
				type = body.Parameter.First(x => x.Name == "type").Value.ToString()
			};
		}

		throw new InvalidOperationException("Unsupported HTTP method");
	}
}