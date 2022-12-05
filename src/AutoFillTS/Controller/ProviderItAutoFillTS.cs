using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium;
using MPSTI.PlenoSoft.Core.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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

		protected override void EsperarPeloLogin(SeleniumWd seleniumWd)
		{
			seleniumWd.Wait(wait);
			if (seleniumWd.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(seleniumWd.GetByIdOrName("USUARIO").Text);
				var passOk = !String.IsNullOrWhiteSpace(seleniumWd.GetByIdOrName("SENHA").Text);
				if (userOk || passOk)
					seleniumWd.GetButton("COMANDO").Click();
			}
			while (seleniumWd.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
				seleniumWd.Wait(wait);
		}

		private static void SelecionarCompetencia(SeleniumWd seleniumWd, TimeSheet timeSheet)
		{
			var comboCompetencia = seleniumWd.GetSelect("ANOMES_TELA");
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;

			if (comboCompetencia.WrappedElement.GetAttribute("value") != competencia.ToString())
				comboCompetencia.SetSelect(competencia.ToString());
		}

		private static void SelecionarTipoHora(SeleniumWd seleniumWd, string tipo)
		{
			var form = seleniumWd.GetElementsByTagName("form").First(e => e.FindByIdOrName("frmCadDoc"));
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

		private void GuardarEmCache(SeleniumWd seleniumWd)
		{
			while (!seleniumWd.ContainsAllText(true, "REGISTRO DE HORAS", "COMPETÊNCIA", "HORAS NORMAIS", "HORAS EXTRAS", "SOBREAVISO"))
				seleniumWd.Wait(wait);

			cacheTexts = seleniumWd.GetElementsByTagName("input").Where(tf => tf.IsAlive()).ToArray();
			cacheCombos = seleniumWd.GetElementsByTagName("Select").Where(tf => tf.IsAlive()).ToArray();
		}

		protected override bool Fill(SeleniumWd seleniumWd, TimeSheet timeSheet)
		{
			SelecionarCompetencia(seleniumWd, timeSheet);
			SelecionarTipoHora(seleniumWd, "HORAS NORMAIS");
			GuardarEmCache(seleniumWd);
			foreach (var item in timeSheet.TarefasDiarias)
				PreencherHorasNormais(seleniumWd, item);

			if (Salvar(seleniumWd))
			{
				SelecionarTipoHora(seleniumWd, "HORAS EXTRAS");
				GuardarEmCache(seleniumWd);
				foreach (var item in timeSheet.TarefasDiarias)
					PreencherHorasExtas(seleniumWd, item);
			}
			return true;
		}

		private void PreencherHorasNormais(SeleniumWd seleniumWd, TarefaDiaria tarefaDiaria)
		{
			var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");
			cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).SetSelect("51432");
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).SetInput(tarefaDiaria.Inicio.ToHora());
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).SetInput(tarefaDiaria.TerminoHorasComuns.ToHora());
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).SetInput(tarefaDiaria.Intervalo.ToHora()).SetFocus();
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).SetInput(tarefaDiaria.Descricao);
			seleniumWd.Wait(wait);
		}

		private void PreencherHorasExtas(SeleniumWd seleniumWd, TarefaDiaria tarefaDiaria)
		{
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).SetSelect("51432");
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).SetInput(tarefaDiaria.TerminoHorasComuns.ToHora());
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).SetInput(tarefaDiaria.Termino.ToHora());
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).SetInput(TimeSpan.Zero.ToHora()).SetFocus();
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).SetInput(tarefaDiaria.Descricao);
				seleniumWd.Wait(wait);
			}
		}

		protected override void WaitFinish(SeleniumWd seleniumWd)
		{
			try
			{
				Salvar(seleniumWd);
			}
			finally
			{
				seleniumWd.Wait(TimeSpan.FromSeconds(5));
				base.WaitFinish(seleniumWd);
			}
		}

		private Boolean Salvar(SeleniumWd seleniumWd)
		{
			var button = seleniumWd.GetButton("COMANDO_SALVAR_TOP");
			if (AutoSaveClick)
				button.Click();
			else
				AguardeUsuarioGravar(seleniumWd, button);

			AguardeGravacao(seleniumWd, button);

			return true;
		}

		private void AguardeUsuarioGravar(SeleniumWd seleniumWd, IWebElement button)
		{
			button.SetFocus();
			while (button.IsAlive() && button.Text == "SALVAR")
				seleniumWd.Wait(wait);
		}

		private static void AguardeGravacao(SeleniumWd seleniumWd, IWebElement button)
		{
			seleniumWd.Wait(TimeSpan.FromSeconds(1));
			while (button.IsAlive() && button.Text != "SALVAR")
				seleniumWd.Wait(TimeSpan.FromSeconds(1));
		}
	}
}