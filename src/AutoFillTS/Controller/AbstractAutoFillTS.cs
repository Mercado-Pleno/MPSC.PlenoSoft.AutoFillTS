using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;


namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public abstract class AbstractAutoFillTS
	{
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
			using (var webDriver = Factory.ChromeDriver(@"..\Libs\", "chromedriver.exe"))
			{
				webDriver.IrParaEndereco(UrlLogin, 1);
				webDriver.Manage().Cookies.DeleteAllCookies();
				EsperarPeloLogin(webDriver);

				foreach (var url in Urls)
					webDriver.IrParaEndereco(url, 1);

				ok = Fill(webDriver, timeSheet);
				if (ok) WaitFinish(webDriver);
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