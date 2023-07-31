using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium.Drivers;
using MPSTI.PlenoSoft.Core.Selenium.Factories;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public abstract class AbstractAutoFillTS
	{
		protected readonly TimeSpan tryTimeOut = TimeSpan.FromSeconds(2);
		protected readonly TimeSpan wait = TimeSpan.FromMilliseconds(99);

		private const String deleteAllCookies = @"
function deleteAllCookies() {
    var cookies = document.cookie.split("";"");

    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
		var eqPos = cookie.indexOf(""="");
		var name = (eqPos > -1) ? cookie.substr(0, eqPos) : cookie;
		document.cookie = name + ""=;expires=Thu, 01 Jan 1970 00:00:00 GMT"";
    }
}
deleteAllCookies();";

		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		protected CancellationToken Token
		{
			get
			{
				if (_cancellationTokenSource.Token.IsCancellationRequested)
					_cancellationTokenSource = new CancellationTokenSource();
				return _cancellationTokenSource.Token;
			}
		}

		protected abstract String UrlLogin { get; }
		protected abstract IEnumerable<String> Urls { get; }
		protected readonly Boolean AutoSaveClick;
		private readonly Boolean IgnoraProcessamento;

		protected AbstractAutoFillTS(Boolean vaiProcessar, Boolean autoSaveClick)
		{
			IgnoraProcessamento = !vaiProcessar;
			AutoSaveClick = autoSaveClick;
		}

		protected abstract void EsperarPeloLogin(SeleniumDriver seleniumDriver);
		protected abstract Boolean Fill(SeleniumDriver seleniumDriver, TimeSheet timeSheet);

		public Boolean Processar(TimeSheet timeSheet)
		{
			return IgnoraProcessamento || OrquestrarPreenchimento(timeSheet);
		}

		private Boolean OrquestrarPreenchimento(TimeSheet timeSheet)
		{
			var ok = true;
			using (var webDriver = SeleniumFactory.GetDriver())
			{
				var seleniumDriver = new SeleniumDriver(webDriver);
				seleniumDriver.GoTo(UrlLogin);
				EsperarPeloLogin(seleniumDriver);

				foreach (var url in Urls)
					seleniumDriver.GoTo(url);

				ok = Fill(seleniumDriver, timeSheet);
				if (ok) WaitFinish(seleniumDriver);
			}

			return ok;
		}


		protected virtual void WaitFinish(SeleniumDriver seleniumDriver)
		{
			seleniumDriver.Wait(wait);
			seleniumDriver.CloseAndDispose();
		}
	}
}