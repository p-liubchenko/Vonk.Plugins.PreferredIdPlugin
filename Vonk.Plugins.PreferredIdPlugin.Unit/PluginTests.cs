﻿using Hl7.Fhir.Model;

using Microsoft.Extensions.Logging;

using Moq;

using Vonk.Core.Common;
using Vonk.Core.Context;
using Vonk.Core.ElementModel;
using Vonk.Core.Repository;
using Vonk.Fhir.R4;
using Vonk.Plugins.PreferredIdPlugin.Repositories;
using Vonk.Plugins.PreferredIdPlugin.Unit.Data;

namespace Vonk.Plugins.PreferredIdPlugin.Unit;
public sealed class PluginTests
{
	private Mock<IAdministrationSearchRepository> searchRepository = new Mock<IAdministrationSearchRepository>();

	private static readonly List<NamingSystem> _predefined = new List<NamingSystem>()
	{
		new NamingSystem()
		{
			UniqueId = new List<NamingSystem.UniqueIdComponent>()
			{
				new NamingSystem.UniqueIdComponent()
				{
					Value = "2.16.840.1.113883.4.1",
					Type = NamingSystem.NamingSystemIdentifierType.Oid
				},
				new NamingSystem.UniqueIdComponent()
				{
					Value = "http://hl7.org/fhir/sid/us-ssn",
					Type = NamingSystem.NamingSystemIdentifierType.Uri
				}
			}
		},
		new NamingSystem()
		{
			UniqueId = new List<NamingSystem.UniqueIdComponent>()
			{
				new NamingSystem.UniqueIdComponent()
				{
					Value = "2.16.840.1.113883.4.642.4.2",
					Type = NamingSystem.NamingSystemIdentifierType.Oid
				},
				new NamingSystem.UniqueIdComponent()
				{
					Value = "http://hl7.org/fhir/administrative-gender",
					Type = NamingSystem.NamingSystemIdentifierType.Uri
				}
			}
		}
	};

	public PluginTests()
	{
		searchRepository
			.Setup(x =>
				x.Search(It.IsAny<IArgumentCollection>(), It.IsAny<SearchOptions>()))
			.ReturnsAsync(new SearchResult(_predefined.Select(p => p.ToIResource()), 10));
	}

	[Theory]
	[ClassData(typeof(PluginSucessTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_200(IVonkContext context, int statusCode, IResource expectedResult)
	{
		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		await p.ResolvePreferredId(context);
		Assert.Equal("Parameters", context.Response.Payload.Type);
		Assert.NotNull(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);
		Assert.True(context.Response.Payload.SelectText("result") == expectedResult.SelectText("result"));

	}

	[Theory]
	[ClassData(typeof(PluginNotFoundTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_OnError(IVonkContext context, int statusCode)
	{
		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		await p.ResolvePreferredId(context);

		Assert.Null(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);

	}
}
