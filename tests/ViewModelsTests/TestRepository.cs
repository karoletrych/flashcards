using System.Collections.Generic;
using Flashcards.Infrastructure.DataAccess;
using Flashcards.Services.DataAccess;

namespace ViewModelsTests
{
	public static class TestRepository
	{
		public static IRepository<T> InitializedWith<T>(IEnumerable<T> entities) where T : new()
		{
			var connection = new Connection(new DatabaseConnectionFactory().CreateInMemoryConnection());
			var repository = new Repository<T>(() => connection);

			repository.InsertOrReplaceAllWithChildren(entities);
			return repository;
		}
	}
}