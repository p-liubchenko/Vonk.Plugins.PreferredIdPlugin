using Moq;

using System.Collections;

using Vonk.Core.Context;

namespace Vonk.Plugins.PreferredIdPlugin.Unit.Data;
internal class ValidatorTestVariants : IEnumerable<object[]>
{

	Mock<IVonkContext> withTypeOnly = new Mock<IVonkContext>();
	Mock<IVonkContext> withIdOnly = new Mock<IVonkContext>();
	Mock<IVonkContext> valid = new Mock<IVonkContext>();

	public ValidatorTestVariants()
	{
		withTypeOnly.Setup(x =>
				x.Arguments)
			.Returns(new ArgumentCollection().AddArgument(new Argument(ArgumentSource.Query, "type", "any", ArgumentStatus.NotHandled)));

		withIdOnly.Setup(x =>
				x.Arguments)
			.Returns(new ArgumentCollection().AddArgument(new Argument(ArgumentSource.Query, "id", "any", ArgumentStatus.NotHandled)));

		valid.Setup(x =>
				x.Arguments)
			.Returns(new ArgumentCollection().AddArgument(new Argument(ArgumentSource.Query, "id", "any", ArgumentStatus.NotHandled))
					.AddArgument(new Argument(ArgumentSource.Query, "type", "any", ArgumentStatus.NotHandled)));

		//withTypeOnly.Setup(x => x.Response).Returns();
	}

	public IEnumerator<object[]> GetEnumerator()
	{

		yield return new object[] { withTypeOnly.Object, false };

		yield return new object[] { withIdOnly.Object, false };

		yield return new object[] { valid.Object, true };
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}