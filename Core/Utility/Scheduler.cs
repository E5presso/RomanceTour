using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Utility
{
	/// <summary>
	/// 스케쥴 해당 시 발생하는 이벤트에 대한 정보입니다.
	/// </summary>
	public class ScheduleEventArgs : EventArgs
	{
		/// <summary>
		/// 스케쥴 발생 시간을 가져옵니다.
		/// </summary>
		public DateTime Time { get; private set; }

		/// <summary>
		/// ScheduleEventArgs 클래스를 초기화합니다.
		/// </summary>
		/// <param name="time">스케쥴 발생 시간을 지정합니다.</param>
		public ScheduleEventArgs(DateTime time)
		{
			Time = time;
		}
	}
	/// <summary>
	/// 지정한 시간에 이벤트를 발생시키는 기능을 구현합니다.
	/// </summary>
	public class Scheduler
	{
		private readonly List<DateTime> list = new List<DateTime>();

		/// <summary>
		/// 스케쥴 해당 시 발생하는 이벤트입니다.
		/// </summary>
		public event EventHandler<ScheduleEventArgs> ScheduleEvent;

		/// <summary>
		/// Scheduler 클래스를 초기화합니다.
		/// </summary>
		public Scheduler()
		{
			while (true)
			{
				var now = DateTime.Now;
				foreach (var time in list)
				{
					if (time.Hour == now.Hour && time.Minute == now.Minute && time.Second == now.Second)
					{
						ScheduleEvent?.BeginInvoke(this, new ScheduleEventArgs(now), new AsyncCallback((ar) =>
						{
							ScheduleEvent?.EndInvoke(ar);
						}), null);
					}
				}
				Thread.Sleep(1000);
			}
		}
		/// <summary>
		/// 새로운 스케쥴을 등록합니다.
		/// </summary>
		/// <param name="time">스케쥴이 발생할 시간을 지정합니다.</param>
		public void AddSchedule(DateTime time)
		{
			list.Add(time);
		}
	}
}