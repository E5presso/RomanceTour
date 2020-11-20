using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utility
{
	/// <summary>
	/// 작업핸들을 정의합니다.
	/// </summary>
	public class TaskHandle
	{
		private readonly CancellationTokenSource token = new CancellationTokenSource();
		/// <summary>
		/// TaskHandle 클래스를 초기화합니다.
		/// </summary>
		/// <param name="token">작업 취소토큰을 지정합니다.</param>
		public TaskHandle(CancellationTokenSource token)
		{
			this.token = token;
		}
		internal void Stop()
		{
			token.Cancel();
		}
	}
	/// <summary>
	/// 함수작업에 사용할 수 있는 유틸리티 모음집입니다.
	/// </summary>
	public static class Function
	{
		/// <summary>
		/// 지정한 지연 후에 작업을 호출합니다.
		/// </summary>
		/// <param name="callback">호출할 작업을 지정합니다.</param>
		/// <param name="time">지연될 시간을 지정합니다.</param>
		/// <returns>작업 핸들을 반환합니다.</returns>
		public static TaskHandle SetTimeout(Action callback, int time)
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			new Task(async () =>
			{
				await Task.Delay(time);
				if (!token.IsCancellationRequested) callback();
			}, token).Start();
			return new TaskHandle(cts);
		}
		/// <summary>
		/// 지정한 지연 후에 작업을 호출합니다.
		/// </summary>
		/// <param name="callback">호출할 작업을 지정합니다.</param>
		/// <param name="time">지연될 시간을 지정합니다.</param>
		/// <returns>작업 핸들을 반환합니다.</returns>
		public static TaskHandle SetTimeout(Action callback, TimeSpan time)
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			new Task(async () =>
			{
				await Task.Delay(time);
				if (!token.IsCancellationRequested) callback();
			}, token).Start();
			return new TaskHandle(cts);
		}
		/// <summary>
		/// 지정한 주기마다 작업을 호출합니다.
		/// </summary>
		/// <param name="callback">호출할 작업을 지정합니다.</param>
		/// <param name="time">호출될 주기를 지정합니다.</param>
		/// <returns>작업 핸들을 반환합니다.</returns>
		public static TaskHandle SetInterval(Action callback, int time)
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			new Task(async () =>
			{
				while (true)
				{
					if (token.IsCancellationRequested) break;
					else callback();
					await Task.Delay(time);
				}
			}, token).Start();
			return new TaskHandle(cts);
		}
		/// <summary>
		/// 지정한 주기마다 작업을 호출합니다.
		/// </summary>
		/// <param name="callback">호출할 작업을 지정합니다.</param>
		/// <param name="time">호출될 주기를 지정합니다.</param>
		/// <returns>작업 핸들을 반환합니다.</returns>
		public static TaskHandle SetInterval(Action callback, TimeSpan time)
		{
			var cts = new CancellationTokenSource();
			var token = cts.Token;
			new Task(async () =>
			{
				while (true)
				{
					if (token.IsCancellationRequested) break;
					else callback();
					await Task.Delay(time);
				}
			}, token).Start();
			return new TaskHandle(cts);
		}

		/// <summary>
		/// 지정한 작업을 중단합니다.
		/// </summary>
		/// <param name="handle">중단할 작업을 지정합니다.</param>
		public static void Clear(TaskHandle handle) => handle.Stop();
	}
}