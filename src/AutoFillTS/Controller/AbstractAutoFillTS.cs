using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using System;
using System.Collections.Generic;
using WatiN.Core;

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

		protected abstract void EsperarPeloLogin(Document document);
		protected abstract Boolean Fill(Document document, TimeSheet timeSheet);

		public Boolean Processar(TimeSheet timeSheet)
		{
			return IgnoraProcessamento || OrquestrarPreenchimento(timeSheet);
		}

		private Boolean OrquestrarPreenchimento(TimeSheet timeSheet)
		{
			var ok = true;
			using (var browser = WatiNExtension.ObterNavegador<IE>())
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

		protected virtual void WaitFinish(Browser browser)
		{
			WatiNExtension.Wait();
			try { browser.Close(); }
			catch (Exception) { }
		}
	}
}