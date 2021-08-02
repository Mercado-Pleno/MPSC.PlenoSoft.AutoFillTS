using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Selenium.Extension;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;


namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public class ProviderItAutoFillTS : AbstractAutoFillTS
	{
		//private TextField[] cacheTexts;
		//private SelectList[] cacheCombos;
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

		protected override void EsperarPeloLogin(SeleniumRWD seleniumRWD)
		{
			WaitExtension.Wait();
			/*
			if (webDriver.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(webDriver.TextField(e => e.FindByIdOrName("USUARIO")).Text);
				var passOk = !String.IsNullOrWhiteSpace(webDriver.TextField(e => e.FindByIdOrName("SENHA")).Text);
				if (userOk || passOk)
					webDriver.GetButton(e => e.FindByIdOrName("COMANDO")).Click();
			}
			*/
			while (seleniumRWD.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
				WaitExtension.Wait();
		}

		private static void SelecionarCompetencia(SeleniumRWD seleniumRWD, TimeSheet timeSheet)
		{
			/*
			var comboCompetencia = webDriver.SelectList(e => e.FindByIdOrName("ANOMES_TELA"));
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;
			if (comboCompetencia.GetAttributeValue("value") != competencia.ToString())
				comboCompetencia.Select(competencia, true);
			*/
		}

		private static void SelecionarTipoHora(SeleniumRWD seleniumRWD, string tipo)
		{
			/*
			var form = webDriver.Form(e => e.FindByIdOrName("frmCadDoc"));
			var txt = form.TextField(e => e.FindByIdOrName("TIPO_HORA"));
			var tipohora = $"tipoHora('{txt.Value}')";

			var cell = GetCell(form, tipo);
			while (GetCell(cell, tipo) != null)
				cell = GetCell(cell, tipo);

			if (!cell.OuterHtml.Contains(tipohora))
				cell.Click();
			*/
		}

		private void GuardarEmCache(SeleniumRWD seleniumRWD)
		{
			while (!seleniumRWD.ContainsAllText(true,"REGISTRO DE HORAS", "COMPETÊNCIA", "HORAS NORMAIS", "HORAS EXTRAS", "SOBREAVISO"))
				WaitExtension.Wait();
			/*
			cacheTexts = webDriver.TextFields.Where(tf => tf.Exists).ToArray();
			cacheCombos = webDriver.SelectLists.Where(tf => tf.Exists).ToArray();
			*/
		}

		protected override bool Fill(SeleniumRWD seleniumRWD, TimeSheet timeSheet)
		{
			SelecionarCompetencia(seleniumRWD, timeSheet);
			SelecionarTipoHora(seleniumRWD, "HORAS NORMAIS");
			GuardarEmCache(seleniumRWD);
			foreach (var item in timeSheet.TarefasDiarias)
				PreencherHorasNormais(seleniumRWD, item);

			if (Salvar(seleniumRWD))
			{
				SelecionarTipoHora(seleniumRWD, "HORAS EXTRAS");
				GuardarEmCache(seleniumRWD);
				foreach (var item in timeSheet.TarefasDiarias)
					PreencherHorasExtas(seleniumRWD, item);
			}
			return true;
		}

		private Boolean PreencherHorasNormais(SeleniumRWD seleniumRWD, TarefaDiaria tarefaDiaria)
		{/*
			var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

			cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.Inicio.ToHora(), false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(tarefaDiaria.Intervalo.ToHora(), false)?.Focus();
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);
			*/
			return true;
		}

		private Boolean PreencherHorasExtas(SeleniumRWD seleniumRWD, TarefaDiaria tarefaDiaria)
		{
			/*
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.Termino.ToHora(), false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(TimeSpan.Zero.ToHora(), false).Focus();
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);
			}
			*/
			return true;
		}

		protected override void WaitFinish(SeleniumRWD seleniumRWD)
		{
			try
			{
				Salvar(seleniumRWD);
			}
			catch (Exception) { }
			finally
			{
				WaitExtension.Wait(TimeSpan.FromSeconds(5));
				base.WaitFinish(seleniumRWD);
			}
		}

		private Boolean Salvar(SeleniumRWD seleniumRWD)
		{
			/*
			var button = webDriver.GetButton(e => e.FindByIdOrName("COMANDO_SALVAR_TOP"));
			if (AutoSaveClick)
				button.Click();
			else
				AguardeUsuarioGravar(button);

			AguardeGravacao(button);
			*/
			return true;
		}

		private void AguardeUsuarioGravar(IWebElement button)
		{
			button.Focus();
			while (button.Exists() && button.Text == "SALVAR")
				WaitExtension.Wait();
		}

		private static void AguardeGravacao(IWebElement button)
		{
			WaitExtension.Wait(TimeSpan.FromSeconds(1));
			while (button.Exists() && button.Text != "SALVAR")
				WaitExtension.Wait(TimeSpan.FromSeconds(1));
		}
	}
}