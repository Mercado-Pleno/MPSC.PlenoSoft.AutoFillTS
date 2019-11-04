using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;


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

		protected abstract String UrlLogin { get; }
		protected abstract IEnumerable<String> Urls { get; }
		protected readonly Boolean AutoSaveClick;
		private readonly Boolean IgnoraProcessamento;

		public AbstractAutoFillTS(Boolean vaiProcessar, Boolean autoSaveClick)
		{
			IgnoraProcessamento = !vaiProcessar;
			AutoSaveClick = autoSaveClick;
		}

		protected abstract void EsperarPeloLogin(IWebDriver document);
		protected abstract Boolean Fill(IWebDriver document, TimeSheet timeSheet);

		public Boolean Processar(TimeSheet timeSheet)
		{
			return IgnoraProcessamento || OrquestrarPreenchimento(timeSheet);
		}

		private Boolean OrquestrarPreenchimento(TimeSheet timeSheet)
		{
			var ok = true;
			using (var browser = Factory.ChromeDriver(@"..\Libs\", "chromedriver.exe"))
			{
				browser.IrParaEndereco(UrlLogin, 1);
				browser.RunScript(deleteAllCookies);
				EsperarPeloLogin(browser);

				foreach (var url in Urls)
					browser.IrParaEndereco(url, 1);

				ok = Fill(browser, timeSheet);
				if (ok) WaitFinish(browser);
			}

			return ok;
		}

		protected virtual void WaitFinish(IWebDriver browser)
		{
			try { browser.Close(); }
			catch (Exception) { }
		}
	}
}