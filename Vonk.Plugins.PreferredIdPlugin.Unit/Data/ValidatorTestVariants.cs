using System.Collections;

using Vonk.Core.Context;
using Vonk.Plugins.PreferredIdPlugin.Unit.Context;

namespace Vonk.Plugins.PreferredIdPlugin.Unit.Data;
internal class ValidatorTestVariants : IEnumerable<object[]>
{
	VonkTestContext withTypeOnly = new();
	VonkTestContext withIdOnly = new();
	VonkTestContext valid = new();

	public ValidatorTestVariants()
	{
		withTypeOnly.Arguments.AddArgument(new Argument(ArgumentSource.Query, "type", "any", ArgumentStatus.NotHandled));
		withIdOnly.Arguments.AddArgument(new Argument(ArgumentSource.Query, "id", "any", ArgumentStatus.NotHandled));
		valid.Arguments.AddArgument(new Argument(ArgumentSource.Query, "id", "any", ArgumentStatus.NotHandled))
					.AddArgument(new Argument(ArgumentSource.Query, "type", "any", ArgumentStatus.NotHandled));

	}

	public IEnumerator<object[]> GetEnumerator()
	{

		yield return new object[] { withTypeOnly, false };

		yield return new object[] { withIdOnly, false };

		yield return new object[] { valid, true };
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}