using System;
using System.ComponentModel;
using System.Net.Sockets;
using Core.Collections;

namespace Core.Network.Tcp
{
	internal class Session : IDisposable
	{
		private readonly int buffersize;
		private readonly bool enableMultiBytes;

		private struct Status
		{
			public enum Phase
			{
				GET_HEADER,
				GET_DATA
			}
			public Phase phase;
			public int length;
		}
		private Status status;
		private readonly Socket socket;
		private readonly RingBuffer queue;
		private readonly byte[] buffer;

		public string IP { get; }
		public bool IsClosed { private set; get; } = false;

		public event SendEvent Sended;
		public event ReceiveEvent Received;
		public event DisconnectEvent Disconnected;
		public event ExceptionEvent ErrorOccurred;

		public Session(Socket socket, int buffersize, bool enableMultiBytes)
		{
			this.socket = socket;
			this.buffersize = buffersize;
			this.enableMultiBytes = enableMultiBytes;

			IP = socket.RemoteEndPoint.ToString();
			buffer = new byte[buffersize];
			queue = new RingBuffer();
			if (socket.Connected)
			{
				try
				{
					IsClosed = false;
					socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
				}
				catch (Exception e) { ErrorOccurred?.Invoke(e); }
			}
		}

		public void Send(byte[] data)
		{
			if (socket.Connected)
			{
				byte[] packet;
				if (enableMultiBytes)
				{
					packet = new byte[4 + data.Length];
					byte[] length = BitConverter.GetBytes(data.Length);
					Buffer.BlockCopy(length, 0, packet, 0, 4);
					Buffer.BlockCopy(data, 0, packet, 4, data.Length);
				}
				else
				{
					packet = new byte[data.Length];
					Buffer.BlockCopy(data, 0, packet, 0, data.Length);
				}
				try
				{
					socket.BeginSend(packet, 0, packet.Length, 0, new AsyncCallback((ar) =>
					{
						int bytesSent = socket.EndSend(ar);
						Sended?.Invoke(this, enableMultiBytes ? bytesSent - 4 : bytesSent);
					}), null);
				}
				catch (Exception e) { ErrorOccurred?.Invoke(e); }
			}
			else
			{
				ErrorOccurred?.Invoke(new InvalidOperationException("현재 소켓이 연결되어있지 않습니다."));
			}
		}
		public void Close()
		{
			if (socket.Connected && !IsClosed)
			{
				try
				{
					IsClosed = true;
					socket.Shutdown(SocketShutdown.Both);
					socket.BeginDisconnect(false, new AsyncCallback((ar) =>
					{
						socket.EndDisconnect(ar);
						Disconnected?.Invoke(this);
					}), null);
				}
				catch (Exception e) { ErrorOccurred?.Invoke(e); }
			}
			else
			{
				ErrorOccurred?.Invoke(new InvalidOperationException("현재 소켓이 연결되어있지 않습니다."));
			}
		}

		private void OnReceived(IAsyncResult ar)
		{
			try
			{
				int bytesRead = socket.EndReceive(ar);
				if (bytesRead > 0)
				{
					if (enableMultiBytes)
					{
						queue.Write(buffer, 0, bytesRead);
						Array.Clear(buffer, 0, bytesRead);
						GetPacket();
					}
					else
					{
						byte[] data = new byte[bytesRead];
						Buffer.BlockCopy(buffer, 0, data, 0, bytesRead);
						Array.Clear(buffer, 0, bytesRead);
						socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
						Received?.Invoke(this, bytesRead, data);
					}
				}
				else
				{
					Close();
				}
			}
			catch (Exception e)
			{
				if (e is Win32Exception w && w.ErrorCode == 10054)
				{
					Disconnected?.Invoke(this);
				}
				else
				{
					ErrorOccurred?.Invoke(e);
				}
			}
		}
		private void GetPacket()
		{
			if (status.phase == Status.Phase.GET_HEADER)
			{
				if (queue.Count >= 4)
				{
					byte[] length = queue.Read(4);

					status.length = BitConverter.ToInt32(length, 0);
					status.phase = Status.Phase.GET_DATA;
					if (queue.Count > 0)
					{
						GetPacket();
					}
					else if (!IsClosed)
					{
						socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
					}
				}
				else if (!IsClosed)
				{
					socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
				}
			}
			else if (status.phase == Status.Phase.GET_DATA)
			{
				if (queue.Count >= status.length)
				{
					byte[] data = queue.Read(status.length);
					status.phase = Status.Phase.GET_HEADER;
					if (queue.Count > 0)
					{
						GetPacket();
					}
					else if (!IsClosed)
					{
						socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
						Received?.Invoke(this, status.length, data);
					}
				}
				else if (!IsClosed)
				{
					socket.BeginReceive(buffer, 0, buffersize, 0, new AsyncCallback(OnReceived), null);
				}
			}
		}
		#region IDisposable Support
		private bool disposedValue = false;
		~Session()
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
				Close();
				queue.Clear();
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