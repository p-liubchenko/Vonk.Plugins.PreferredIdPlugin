using Vonk.Core.Context;

namespace Vonk.Plugins.PreferredIdPlugin.Unit;

public class ValidatorTest
{
	//unable to mock context
	//[Theory]
	//[ClassData(typeof(ValidatorTestVariants))]
	public void ValidateRequestParameters(IVonkContext vonkContext, bool success)
	{
		PreferredIdPluginRequestValidator v = new();
		v.ValidateGetRequest(vonkContext);

		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
	}
}