using System;
using System.Threading;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public static class WaitExtension
	{
		public static void Wait(TimeSpan? timeOut = null)
		{
			Thread.Sleep(timeOut ?? TimeSpan.FromMilliseconds(250));
		}
	}
}