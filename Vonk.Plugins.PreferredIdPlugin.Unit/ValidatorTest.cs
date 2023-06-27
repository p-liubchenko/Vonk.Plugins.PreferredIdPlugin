using Vonk.Core.Context;
using Vonk.Plugins.PreferredIdPlugin.Unit.Data;

namespace Vonk.Plugins.PreferredIdPlugin.Unit;

public class ValidatorTest
{
	[Theory]
	[ClassData(typeof(ValidatorTestVariants))]
	public void ValidateRequestParameters(IVonkContext vonkContext, bool success)
	{
		PreferredIdPluginRequestValidator v = new();
		v.ValidateGetRequest(vonkContext);

		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
	}
}