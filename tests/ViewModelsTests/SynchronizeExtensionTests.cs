using System.Collections.ObjectModel;
using Xunit;
using Flashcards.Domain.ViewModels.Tools;
using FluentAssertions;

namespace ViewModelsTests
{
	public class SynchronizeExtensionTests
	{
		[Fact]
		public void Synchronizes()
		{
			var synchronized = new Collection<int>
			{
				1, 2, 3
			};
			var unsynchronized = new Collection<int>
			{
				1, 3, 4, 2
			};
			unsynchronized.SynchronizeWith(synchronized);

			unsynchronized.Should().BeEquivalentTo(synchronized);
		}
	}
}