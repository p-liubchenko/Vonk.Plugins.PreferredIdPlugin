using Hl7.Fhir.Model;

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
	[Theory]
	[ClassData(typeof(PluginSucessTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_200(IVonkContext context, int statusCode, IResource expectedResult, IEnumerable<NamingSystem> dataSet)
	{
		//setup
		Mock<IAdministrationSearchRepository> searchRepository = new Mock<IAdministrationSearchRepository>();
		searchRepository
				.Setup(x =>
					x.Search(It.IsAny<IArgumentCollection>(), It.IsAny<SearchOptions>()))
				.ReturnsAsync(new SearchResult(dataSet.Select(p => p.ToIResource()), 10));

		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		// execute
		await p.ResolvePreferredId(context);

		//verify
		Assert.Equal("Parameters", context.Response.Payload.Type);
		Assert.NotNull(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);
		Assert.True(context.Response.Payload.SelectText("result") == expectedResult.SelectText("result"));

	}

	[Theory]
	[ClassData(typeof(PluginNotFoundTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_OnError(IVonkContext context, int statusCode, IEnumerable<NamingSystem> dataSet)
	{
		//setup
		Mock<IAdministrationSearchRepository> searchRepository = new Mock<IAdministrationSearchRepository>();
		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		//execute
		await p.ResolvePreferredId(context);

		//verify
		Assert.Null(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);
	}

	[Theory]
	[ClassData(typeof(PluginNotFoundTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_NotFoundOnRepositoryError(IVonkContext context, int statusCode, IEnumerable<NamingSystem> dataSet)
	{
		//setup
		Mock<IAdministrationSearchRepository> searchRepository = new Mock<IAdministrationSearchRepository>();
		searchRepository
				.Setup(x =>
					x.Search(It.IsAny<IArgumentCollection>(), It.IsAny<SearchOptions>()))
				.ReturnsAsync(new SearchResult(Enumerable.Empty<IResource>(), 0));

		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		//execute
		await p.ResolvePreferredId(context);

		//verify
		Assert.Null(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);
	}

	[Theory]
	[ClassData(typeof(PluginNotFoundTestVariants))]
	public async System.Threading.Tasks.Task TestContextModifications_NotFoundRequestedUniqueIdError(IVonkContext context, int statusCode, IEnumerable<NamingSystem> dataSet)
	{
		//setup
		Mock<IAdministrationSearchRepository> searchRepository = new Mock<IAdministrationSearchRepository>();
		searchRepository
			.Setup(x =>
				x.Search(It.IsAny<IArgumentCollection>(), It.IsAny<SearchOptions>()))
			.ReturnsAsync(new SearchResult(dataSet.Select(x => x.ToIResource()), 0));

		AdminDomainResourceSearchRepository<NamingSystem> adrsr = new AdminDomainResourceSearchRepository<NamingSystem>(searchRepository.Object);
		PreferredIdPlugin p = new PreferredIdPlugin(adrsr, Mock.Of<ILogger<PreferredIdPlugin>>());

		//execute
		await p.ResolvePreferredId(context);

		//verify
		Assert.Null(context.Response.Payload);
		Assert.Equal(statusCode, context.Response.HttpResult);
	}
}
