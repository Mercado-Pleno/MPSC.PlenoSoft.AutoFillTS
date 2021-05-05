using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSC.PlenoSoft.Selenium.Extension;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public abstract class AbstractAutoFillTS
	{
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

		public AbstractAutoFillTS(Boolean vaiProcessar, Boolean autoSaveClick)
		{
			IgnoraProcessamento = !vaiProcessar;
			AutoSaveClick = autoSaveClick;
		}

		protected abstract void EsperarPeloLogin(SeleniumRWD seleniumRWD);
		protected abstract Boolean Fill(SeleniumRWD seleniumRWD, TimeSheet timeSheet);

		public Boolean Processar(TimeSheet timeSheet)
		{
			return IgnoraProcessamento || OrquestrarPreenchimento(timeSheet);
		}

		private Boolean OrquestrarPreenchimento(TimeSheet timeSheet)
		{
			var ok = true;
			using (var webDriver = SeleniumFactory.BrowserWebDriver())
			{
				var seleniumRWD = new SeleniumRWD(webDriver);
				seleniumRWD.IrParaEndereco(UrlLogin, 1);
				EsperarPeloLogin(seleniumRWD);

				foreach (var url in Urls)
					seleniumRWD.IrParaEndereco(url, 1);

				ok = Fill(seleniumRWD, timeSheet);
				if (ok) WaitFinish(seleniumRWD);
			}

			return ok;
		}


		protected virtual void WaitFinish(SeleniumRWD seleniumRWD)
		{
			WaitExtension.Wait();
			try { seleniumRWD.Encerrar(); }
			catch (Exception) { }
		}
	}
}