using Vonk.Core.Context;
using Vonk.Plugins.PreferredIdPlugin.Unit.Data;

namespace Vonk.Plugins.PreferredIdPlugin.Unit;

public class ValidatorTest
{
	[Theory]
	[ClassData(typeof(ValidatorTestVariants))]
	public void ValidateGetRequestParameters(IVonkContext vonkContext, bool success)
	{
		PreferredIdPluginRequestValidator v = new();
		v.ValidateGetRequest(vonkContext);

		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
	}

	[Theory]
	[ClassData(typeof(ValidatorTestPostVariants))]
	public void ValidatePostRequestParameters(IVonkContext vonkContext, bool success)
	{
		PreferredIdPluginRequestValidator v = new();
		v.ValidatePostRequest(vonkContext);

		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
	}
}