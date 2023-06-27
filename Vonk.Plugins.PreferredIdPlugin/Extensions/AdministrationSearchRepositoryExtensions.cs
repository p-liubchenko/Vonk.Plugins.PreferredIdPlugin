using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;

using System;
using System.Linq;
using System.Threading.Tasks;

using Vonk.Core.Context;
using Vonk.Core.Repository;
using Vonk.Core.Support;
using Vonk.Plugins.PreferredIdPlugin.Exceptions;

namespace Vonk.Plugins.PreferredIdPlugin.Extensions;
internal static class AdministrationSearchRepositoryExtensions
{

	public static async Task<NamingSystem> FindNamingSystemByUniqueId(this IAdministrationSearchRepository repo, string uuid, Uri currentUrl)
	{
		var options = SearchOptions.Latest(currentUrl, VonkInteraction.type_custom);
		var result = (await repo
			.Search(new ArgumentCollection()
				.AddArgument(
					new Argument(ArgumentSource.Internal, ArgumentNames.resourceType, "NamingSystem") { MustHandle = true }),
					options
			));
		if (result.IsNullOrEmpty())
			throw new NamingSystemException("No objects of type NamingSystem was found");
		var response = result.Select(x => x.ToPoco<NamingSystem>()).Where(x => x.UniqueId.Any(uid => uid.Value == uuid)).SingleOrDefault();

		if (response is null)
			throw new NamingSystemException("NamingSystem with requested uniqueId was not found");

		return response;
	}
}