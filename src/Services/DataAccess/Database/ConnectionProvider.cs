using System;
using SQLite;

namespace Flashcards.Services.DataAccess.Database
{
	public interface IConnectionProvider
	{
		IConnection Connection { get; }
	}

	public interface IDisconnect
	{
		void Disconnect();
	}

	public class ConnectionProvider : IConnectionProvider, IDisconnect
	{
		private readonly DatabaseConnectionFactory _databaseConnectionFactory;

		private readonly string _databasePath;

		private Lazy<IConnection> _connection;

		private SQLiteAsyncConnection _sqLiteAsyncConnection;

		public ConnectionProvider(
			DatabaseConnectionFactory databaseConnectionFactory,
			string databasePath)
		{
			_databaseConnectionFactory = databaseConnectionFactory;
			_databasePath = databasePath;
			_connection = new Lazy<IConnection>(Connect);
		}

		public IConnection Connection => _connection.Value;

		public void Disconnect()
		{
			_sqLiteAsyncConnection.CloseAsync();
			_connection = new Lazy<IConnection>(Connect);
		}

		private IConnection Connect()
		{
			_sqLiteAsyncConnection = _databaseConnectionFactory.CreateAsyncConnection(_databasePath);
			return new AsyncConnection(_sqLiteAsyncConnection);
		}
	}
}