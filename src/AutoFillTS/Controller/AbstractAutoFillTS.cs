using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public abstract class AbstractAutoFillTS
	{
		protected TimeSpan wait = TimeSpan.FromMilliseconds(100);

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

		protected abstract void EsperarPeloLogin(SeleniumWd seleniumWd);
		protected abstract Boolean Fill(SeleniumWd seleniumWd, TimeSheet timeSheet);

		public Boolean Processar(TimeSheet timeSheet)
		{
			return IgnoraProcessamento || OrquestrarPreenchimento(timeSheet);
		}

		private Boolean OrquestrarPreenchimento(TimeSheet timeSheet)
		{
			var ok = true;
			using (var webDriver = SeleniumFactory.BrowserWebDriver())
			{
				var seleniumWd = new SeleniumWd(webDriver);
				seleniumWd.GoTo(UrlLogin);
				EsperarPeloLogin(seleniumWd);

				foreach (var url in Urls)
					seleniumWd.GoTo(url);

				ok = Fill(seleniumWd, timeSheet);
				if (ok) WaitFinish(seleniumWd);
			}

			return ok;
		}


		protected virtual void WaitFinish(SeleniumWd seleniumWd)
		{
			seleniumWd.Wait(wait);
			seleniumWd.CloseAndDispose();
		}
	}
}