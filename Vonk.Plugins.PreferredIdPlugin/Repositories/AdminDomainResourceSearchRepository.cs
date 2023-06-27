using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Vonk.Core.Context;
using Vonk.Core.Repository;
using Vonk.Core.Support;
using Vonk.Plugins.PreferredIdPlugin.Exceptions;

namespace Vonk.Plugins.PreferredIdPlugin.Repositories;
public class AdminDomainResourceSearchRepository<T> where T : DomainResource
{
	protected readonly IAdministrationSearchRepository _searchRepository;

	protected readonly string _typeName;

	public AdminDomainResourceSearchRepository(IAdministrationSearchRepository searchRepository)
	{
		_searchRepository = searchRepository;

		_typeName = Activator.CreateInstance<T>().TypeName;
	}

	public async Task<IEnumerable<T>> Get(Uri currentUrl)
	{
		var options = SearchOptions.Latest(currentUrl, VonkInteraction.type_custom);
		var result = (await _searchRepository
			.Search(new ArgumentCollection()
				.AddArgument(
					new Argument(ArgumentSource.Internal, ArgumentNames.resourceType, _typeName) { MustHandle = true }),
					options
			));
		if (result.IsNullOrEmpty())
			throw new DomainResourceSearchException($"No objects of type {_typeName} was found");

		return result.Select(x => x.ToPoco<T>());
	}

	public async Task<IEnumerable<T>> Get(Uri currentUrl, Func<T, bool> predicated)
	{
		var options = SearchOptions.Latest(currentUrl, VonkInteraction.type_custom);
		var result = (await _searchRepository
			.Search(new ArgumentCollection()
				.AddArgument(
					new Argument(ArgumentSource.Internal, ArgumentNames.resourceType, _typeName) { MustHandle = true }),
					options
			));
		if (result.IsNullOrEmpty())
			throw new DomainResourceSearchException($"No objects of type {_typeName} was found");

		return result.Select(x => x.ToPoco<T>()).Where(predicated);
	}
}
