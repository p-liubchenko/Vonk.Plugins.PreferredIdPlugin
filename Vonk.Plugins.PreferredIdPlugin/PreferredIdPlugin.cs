//using Hl7.Fhir.Model;

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading.Tasks;

using Vonk.Core.Common;
using Vonk.Core.Context;
using Vonk.Fhir.R4;
using Vonk.Plugins.PreferredIdPlugin.Exceptions;
using Vonk.Plugins.PreferredIdPlugin.Repositories;

using static Hl7.Fhir.Model.NamingSystem;

namespace Vonk.Plugins.PreferredIdPlugin;

public sealed class PreferredIdPlugin
{
	private readonly AdminDomainResourceSearchRepository<NamingSystem> _namingSystemRepository;
	private readonly ILogger<PreferredIdPlugin> _logger;

	public PreferredIdPlugin(AdminDomainResourceSearchRepository<NamingSystem> namingSystemRepository, ILogger<PreferredIdPlugin> logger)
	{
		_namingSystemRepository = namingSystemRepository;
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

		try
		{
			UniqueIdComponent uid = await FindUidOfType(context.ServerBase, ResolveParameters(context));
			Ok(context, new Parameters()
			{
				{ "result", new FhirString(uid.Value) }
			}.ToIResource());

		}
		catch (Exception ex) when (ex is DomainResourceSearchException || ex is NamingSystemException)
		{
			_logger.LogDebug(ex.Message);
			NotFound(context, ex.Message);
			return;
		}
		catch (InvalidOperationException)
		{
			context.Response.HttpResult = StatusCodes.Status405MethodNotAllowed;
		}

		_logger.LogDebug("preferred-id get ended");
	}

	private static void Ok(IVonkContext context, IResource parameters)
	{
		context.Arguments.Handled();
		context.Response.Payload = parameters;
		context.Response.HttpResult = StatusCodes.Status200OK;
	}

	private static void NotFound(IVonkContext context, string? message = null)
	{
		if (!string.IsNullOrWhiteSpace(message))
			context.Response.Outcome.AddIssue(VonkOutcome.IssueSeverity.Error, VonkOutcome.IssueType.NotFound, "", message);
		context.Response.HttpResult = StatusCodes.Status404NotFound;
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
		var namingSystem = (await _namingSystemRepository
			.Get(serverUrl, x => x.UniqueId.Any(uid => uid.Value == param.id)))
				.SingleOrDefault()
			?? throw new NamingSystemException("NamingSystem with requested uniqueId was not found");

		var result = namingSystem?.UniqueId
			.FirstOrDefault(u => u.Type.HasValue && u.Type.ToString().ToLower() == param.type.ToLower())
			?? throw new NamingSystemException($"UniqueId of required type was not fount in {param.id}");

		return result;
	}

	/// <summary>
	/// Resolves request parameters depending on http method
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	private static (string id, string type) ResolveParameters(IVonkContext context)
	{
		if (context.Request.Method == HttpMethods.Get)
			return new()
			{
				id = context.Arguments.GetArgument("id").ArgumentValue,
				type = context.Arguments.GetArgument("type").ArgumentValue
			};

		if (context.Request.Method == HttpMethods.Post)
		{
			var body = context.Request.Payload.Resource.ToPoco<Parameters>() ?? throw new InvalidOperationException("Body parameters are not defined");

			return new()
			{
				id = body.Parameter.First(x => x.Name == "id").Value.ToString(),
				type = body.Parameter.First(x => x.Name == "type").Value.ToString()
			};
		}

		throw new InvalidOperationException("Unsupported HTTP method");
	}
}