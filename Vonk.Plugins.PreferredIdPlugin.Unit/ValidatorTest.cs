using Vonk.Core.Context;
using Vonk.Plugins.PreferredIdPlugin.Unit.Data;

namespace Vonk.Plugins.PreferredIdPlugin.Unit;

public class ValidatorTest
{
	[Theory]
	[ClassData(typeof(ValidatorTestVariants))]
	public void ValidateGetRequestParameters(IVonkContext vonkContext, bool success)
	{
		//setup
		PreferredIdPluginRequestValidator v = new();

		//execute
		v.ValidateGetRequest(vonkContext);

		//verify
		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
		if (!success)
		{
			//should add outcome when not successful
			Assert.NotNull(vonkContext.Response.Outcome);
			Assert.True(vonkContext.Response.Outcome.Issues.Any());
		}
	}

	[Theory]
	[ClassData(typeof(ValidatorTestPostVariants))]
	public void ValidatePostRequestParameters(IVonkContext vonkContext, bool success)
	{
		//setup
		PreferredIdPluginRequestValidator v = new();

		//execute
		v.ValidatePostRequest(vonkContext);

		//verify
		Assert.Equal(vonkContext.Response.HttpResult == 0, success);
		if (!success)
		{
			//should add outcome when not successful
			Assert.NotNull(vonkContext.Response.Outcome);
			Assert.True(vonkContext.Response.Outcome.Issues.Any());
		}
	}
}