using System;
using System.Net;
using System.Net.Sockets;

namespace Core.Network.Tcp
{
	internal class Connector : IDisposable
	{
		private int size;
		private IPEndPoint point;
		private Socket socket;

		public string LocalIP;

		public event ConnectEvent Connected;
		public event ExceptionEvent ErrorOccurred;

		public void Start(string address, int port, int buffersize, bool enableMultiBytes)
		{
			try
			{
				size = buffersize;
				IPAddress ip = Dns.GetHostAddresses(address)[0];
				point = new IPEndPoint(ip, port);
				socket?.Dispose();
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				void Callback(IAsyncResult ar)
				{
					try
					{
						socket.EndConnect(ar);
						LocalIP = socket.LocalEndPoint.ToString();
						Connected?.Invoke(new Session(socket, size, enableMultiBytes));
					}
					catch (Exception e) { ErrorOccurred?.Invoke(e); }
				}
				socket.BeginConnect(point, new AsyncCallback(Callback), null);
			}
			catch (Exception e) { ErrorOccurred?.Invoke(e); }
		}
		#region IDisposable Support
		private bool disposedValue = false;
		~Connector()
		{
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					socket.Dispose();
				}
				point = null;
				LocalIP = null;
				disposedValue = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}