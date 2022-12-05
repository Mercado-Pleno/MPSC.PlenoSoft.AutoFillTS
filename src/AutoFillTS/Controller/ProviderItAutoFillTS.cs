using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium;
using MPSTI.PlenoSoft.Core.Selenium.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public class ProviderItAutoFillTS : AbstractAutoFillTS
	{
		private IWebElement[] cacheTexts;
		private IWebElement[] cacheCombos;
		protected override String UrlLogin { get { return "http://186.215.208.203/intranet/login/login.asp"; } }
		protected override IEnumerable<String> Urls
		{
			get
			{
				yield return "http://186.215.208.203/intranet/menu/redirecionadorPortal.asp?cdLinkSistema=BCP2";
				yield return "http://186.215.208.203/intranet/bcp/";
				yield return "http://186.215.208.203/intranet/bcp/index_menu/#";
				yield return "http://186.215.208.203/intranet/bcp/apropriacao_de_horas/timesheet/";
			}
		}

		public ProviderItAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(SeleniumDriver seleniumDriver)
		{
			seleniumDriver.Wait(wait);
			if (seleniumDriver.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(seleniumDriver.GetElementByIdOrName("USUARIO").Text);
				var passOk = !String.IsNullOrWhiteSpace(seleniumDriver.GetElementByIdOrName("SENHA").Text);
				if (userOk || passOk)
					seleniumDriver.GetButtonByText("COMANDO").Click();
			}
			while (seleniumDriver.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
				seleniumDriver.Wait(wait);
		}

		private static void SelecionarCompetencia(SeleniumDriver seleniumDriver, TimeSheet timeSheet)
		{
			var comboCompetencia = seleniumDriver.GetSelectByIdOrName("ANOMES_TELA");
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;

			if (comboCompetencia.WrappedElement.GetAttribute("value") != competencia.ToString())
				comboCompetencia.SetSelect(competencia.ToString());
		}

		private static void SelecionarTipoHora(SeleniumDriver seleniumDriver, string tipo)
		{
			var form = seleniumDriver.GetElementsByTagName("form").First(e => e.FindByIdOrName("frmCadDoc"));
			var txt = form.GetElementsByTagName("input").First(e => e.FindByIdOrName("TIPO_HORA"));
			var tipohora = $"tipoHora('{txt.Text}')";

			var cell = GetCell(form, tipo);
			while (GetCell(cell, tipo) != null)
				cell = GetCell(cell, tipo);

			if (!cell.Text.Contains(tipohora))
				cell.Click();
		}

		private static IWebElement GetCell(ISearchContext container, String tipo)
		{
			return container.FindElements(By.CssSelector("*"))
				.First(t => t.IsAlive()
					&& !String.IsNullOrWhiteSpace(t.Text)
					&& (t.Text.Trim().ToUpper() == tipo.Trim().ToUpper())
			);
		}

		private void GuardarEmCache(SeleniumDriver seleniumDriver)
		{
			while (!seleniumDriver.ContainsAllText(true, "REGISTRO DE HORAS", "COMPETÊNCIA", "HORAS NORMAIS", "HORAS EXTRAS", "SOBREAVISO"))
				seleniumDriver.Wait(wait);

			cacheTexts = seleniumDriver.GetElementsByTagName("input").Where(tf => tf.IsAlive()).ToArray();
			cacheCombos = seleniumDriver.GetElementsByTagName("Select").Where(tf => tf.IsAlive()).ToArray();
		}

		protected override bool Fill(SeleniumDriver seleniumDriver, TimeSheet timeSheet)
		{
			SelecionarCompetencia(seleniumDriver, timeSheet);
			SelecionarTipoHora(seleniumDriver, "HORAS NORMAIS");
			GuardarEmCache(seleniumDriver);
			foreach (var item in timeSheet.TarefasDiarias)
				PreencherHorasNormais(seleniumDriver, item);

			if (Salvar(seleniumDriver))
			{
				SelecionarTipoHora(seleniumDriver, "HORAS EXTRAS");
				GuardarEmCache(seleniumDriver);
				foreach (var item in timeSheet.TarefasDiarias)
					PreencherHorasExtas(seleniumDriver, item);
			}
			return true;
		}

		private void PreencherHorasNormais(SeleniumDriver seleniumDriver, TarefaDiaria tarefaDiaria)
		{
			var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");
			cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).SetSelect("51432");
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).SetInput(tarefaDiaria.Inicio.ToHora());
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).SetInput(tarefaDiaria.TerminoHorasComuns.ToHora());
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).SetInput(tarefaDiaria.Intervalo.ToHora()).SetFocus();
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).SetInput(tarefaDiaria.Descricao);
			seleniumDriver.Wait(wait);
		}

		private void PreencherHorasExtas(SeleniumDriver seleniumDriver, TarefaDiaria tarefaDiaria)
		{
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).SetSelect("51432");
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).SetInput(tarefaDiaria.TerminoHorasComuns.ToHora());
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).SetInput(tarefaDiaria.Termino.ToHora());
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).SetInput(TimeSpan.Zero.ToHora()).SetFocus();
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).SetInput(tarefaDiaria.Descricao);
				seleniumDriver.Wait(wait);
			}
		}

		protected override void WaitFinish(SeleniumDriver seleniumDriver)
		{
			try
			{
				Salvar(seleniumDriver);
			}
			finally
			{
				seleniumDriver.Wait(TimeSpan.FromSeconds(5));
				base.WaitFinish(seleniumDriver);
			}
		}

		private Boolean Salvar(SeleniumDriver seleniumDriver)
		{
			var button = seleniumDriver.GetButtonByText("COMANDO_SALVAR_TOP");
			if (AutoSaveClick)
				button.Click();
			else
				AguardeUsuarioGravar(seleniumDriver, button);

			AguardeGravacao(seleniumDriver, button);

			return true;
		}

		private void AguardeUsuarioGravar(SeleniumDriver seleniumDriver, IWebElement button)
		{
			button.SetFocus();
			while (button.IsAlive() && button.Text == "SALVAR")
				seleniumDriver.Wait(wait);
		}

		private static void AguardeGravacao(SeleniumDriver seleniumDriver, IWebElement button)
		{
			seleniumDriver.Wait(TimeSpan.FromSeconds(1));
			while (button.IsAlive() && button.Text != "SALVAR")
				seleniumDriver.Wait(TimeSpan.FromSeconds(1));
		}
	}
}