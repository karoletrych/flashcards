using Flashcards.Services.DataAccess;

namespace Flashcards.Infrastructure.DataAccess
{
	public interface IConnectionProvider
	{
		IConnection Connection { get; }
	}
}