using System;

namespace Core.Network.Tcp
{
	/// <summary>
	/// 클라이언트 기능을 제공합니다.
	/// </summary>
	public class Client : IDisposable
	{
		private readonly Connector connector;
		private Session session;

		/// <summary>
		/// 클라이언트의 가동 여부를 가져옵니다.
		/// </summary>
		public bool IsActive { get; private set; } = false;
		/// <summary>
		/// 접속 중인 서버의 IP주소입니다.
		/// </summary>
		public string RemoteIP { get => session.IP; }
		/// <summary>
		/// 현재 클라이언트의 로컬 IP주소입니다.
		/// </summary>
		public string LocalIP { get => connector.LocalIP; }

		/// <summary>
		/// 서버 연결 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<ConnectEventArgs> Connected;
		/// <summary>
		/// 데이터 전송 완료 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<SendEventArgs> Sended;
		/// <summary>
		/// 데이터 수신 완료 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<ReceiveEventArgs> Received;
		/// <summary>
		/// 클라이언트 연결 종료 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<DisconnectEventArgs> Disconnected;
		/// <summary>
		/// 에러 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<ExceptionEventArgs> ErrorOccurred;

		/// <summary>
		/// Client 클래스를 초기화합니다.
		/// </summary>
		public Client()
		{
			connector = new Connector();
			connector.Connected += OnConnected;
			connector.ErrorOccurred += OnErrorOccurred;
		}
		/// <summary>
		/// 클라이언트를 시작합니다.
		/// </summary>
		/// <param name="ip">연결할 서버의 IP주소입니다.</param>
		/// <param name="port">연결에 사용할 포트번호입니다.</param>
		/// <param name="bufferSize">송수신에 사용할 버퍼의 크기입니다.</param>
		/// <param name="enableMultiBytes">데이터 송수신 시 멀티바이트 수신을 사용할 지 여부를 지정합니다. 멀티바이트 수신 시 데이터 전송 용량에 제한이 없어지지만, 패킷 구조 특성 상, 송수신 측 모두 해당 구현체를 동일하게 사용해야합니다.</param>
		public void Open(string ip, int port, int bufferSize, bool enableMultiBytes)
		{
			if (!IsActive)
			{
				connector.Start(ip, port, bufferSize, enableMultiBytes);
				IsActive = true;
			}
			else ErrorOccurred?.Invoke(this, new ExceptionEventArgs(new InvalidOperationException("클라이언트가 이미 동작 중입니다.")));
		}
		/// <summary>
		/// 데이터를 전송합니다.
		/// </summary>
		/// <param name="data">전송할 데이터입니다.</param>
		public void Send(byte[] data)
		{
			session?.Send(data);
		}
		/// <summary>
		/// 클라이언트를 정지합니다.
		/// </summary>
		public void Close()
		{
			if (IsActive)
			{
				session.Close();
				session.Sended -= OnSended;
				session.Received -= OnReceived;
				session.Disconnected -= OnDisconnected;
				session.ErrorOccurred -= OnErrorOccurred;

				IsActive = false;
			}
			else ErrorOccurred?.Invoke(this, new ExceptionEventArgs(new InvalidOperationException("클라이언트가 동작 중이 아닙니다.")));
		}

		private void OnConnected(Session session)
		{
			try
			{
				session.Sended += OnSended;
				session.Received += OnReceived;
				session.Disconnected += OnDisconnected;
				session.ErrorOccurred += OnErrorOccurred;
				this.session = session;

				Connected?.Invoke(this, new ConnectEventArgs(session.IP));
			}
			catch (Exception e) { ErrorOccurred?.Invoke(this, new ExceptionEventArgs(e)); }
		}
		private void OnSended(Session session, int bytesSent)
		{
			try { Sended?.Invoke(this, new SendEventArgs(session.IP, bytesSent)); }
			catch (Exception e) { ErrorOccurred?.Invoke(this, new ExceptionEventArgs(e)); }
		}
		private void OnReceived(Session session, int bytesRead, byte[] data)
		{
			try { Received?.Invoke(this, new ReceiveEventArgs(session.IP, bytesRead, data)); }
			catch (Exception e) { ErrorOccurred?.Invoke(this, new ExceptionEventArgs(e)); }
		}
		private void OnDisconnected(Session session)
		{
			try
			{
				session.Sended -= OnSended;
				session.Received -= OnReceived;
				session.Disconnected -= OnDisconnected;
				session.ErrorOccurred -= OnErrorOccurred;
				session.Dispose();
				IsActive = false;

				Disconnected?.Invoke(this, new DisconnectEventArgs(session.IP));
			}
			catch (Exception e) { ErrorOccurred?.Invoke(this, new ExceptionEventArgs(e)); }
		}
		private void OnErrorOccurred(Exception e)
		{
			ErrorOccurred?.Invoke(this, new ExceptionEventArgs(e));
		}
		#region IDisposable Support
		private bool disposedValue = false;
		/// <summary>
		/// Client 클래스를 제거합니다.
		/// </summary>
		~Client()
		{
			Dispose(false);
		}
		/// <summary>
		/// IDisposable 패턴을 구현합니다.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					connector.Dispose();
					session.Dispose();
				}
				disposedValue = true;
			}
		}
		/// <summary>
		/// 클래스를 제거하고 리소스를 반환합니다.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}