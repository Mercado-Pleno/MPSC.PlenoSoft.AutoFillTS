using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

		protected override void EsperarPeloLogin(IWebDriver document)
		{
			if (document.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(document.FindByIdOrName("USUARIO").Text);
				var passOk = !String.IsNullOrWhiteSpace(document.FindByIdOrName("SENHA").Text);
				if (userOk || passOk)
					document.FindByIdOrName("COMANDO").Click();
			}

			while (document.ContainsAnyText(false, "INGRESSAR", "Recuperar senha"))
				Thread.Sleep(1000);
		}

		private static void SelecionarCompetencia(IWebDriver document, TimeSheet timeSheet)
		{
			var comboCompetencia = document.FindByIdOrName("ANOMES_TELA");
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;

			if (comboCompetencia.GetAttribute("value") != competencia.ToString())
				comboCompetencia.Select(competencia.ToString(), true);
		}

		private static void SelecionarTipoHora(IWebDriver document, string tipo)
		{
			var form = document.FindByIdOrName("frmCadDoc");
			var txt = form.FindByIdOrName("TIPO_HORA");
			var tipohora = $"tipoHora('{txt.Text}')";

			var cell = GetCell(form, tipo);
			while (GetCell(cell, tipo) != null)
				cell = GetCell(cell, tipo);

			if (!cell.Text.Contains(tipohora))
				cell.Click();
		}

		private void GuardarEmCache(IWebDriver document)
		{
			while (!document.ContainsAllText(false, "REGISTRO DE HORAS", "COMPETÊNCIA", "HORAS NORMAIS", "HORAS EXTRAS", "SOBREAVISO"))
				Thread.Sleep(1000);

			//cacheTexts = document.TextFields.Where(tf => tf.Exists).ToArray();
			//cacheCombos = document.SelectLists.Where(tf => tf.Exists).ToArray();
		}

		private static IWebElement GetCell(IWebElement container, String tipo)
		{
			return container.FindElement(By.PartialLinkText(tipo));
				//t => !String.IsNullOrWhiteSpace(t.tex) &&
				//(t.OuterText.Trim().ToUpper() == tipo.Trim().ToUpper()));
		}

		protected override bool Fill(IWebDriver document, TimeSheet timeSheet)
		{
			SelecionarCompetencia(document, timeSheet);
			SelecionarTipoHora(document, "HORAS NORMAIS");
			GuardarEmCache(document);
			foreach (var item in timeSheet.TarefasDiarias)
				PreencherHorasNormais(document, item);

			if (Salvar(document))
			{
				SelecionarTipoHora(document, "HORAS EXTRAS");
				GuardarEmCache(document);
				foreach (var item in timeSheet.TarefasDiarias)
					PreencherHorasExtas(document, item);
			}
			return true;
		}

		private Boolean PreencherHorasNormais(IWebDriver document, TarefaDiaria tarefaDiaria)
		{
			var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

			document.FindByIdOrName("ID_PROJETO" + sufixo).Select("51432", false);
			document.FindByIdOrName("hora_ini" + sufixo).Select(tarefaDiaria.Inicio.ToHora(), false);
			document.FindByIdOrName("hora_fim" + sufixo).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
			document.FindByIdOrName("intervalo" + sufixo).Select(tarefaDiaria.Intervalo.ToHora(), false);//?.Focus();
			document.FindByIdOrName("DESCRICAO" + sufixo).Select(tarefaDiaria.Descricao, false);

			return true;
		}

		private Boolean PreencherHorasExtas(IWebDriver document, TarefaDiaria tarefaDiaria)
		{
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				document.FindByIdOrName("ID_PROJETO" + sufixo).Select("51432", false);
				document.FindByIdOrName("hora_ini" + sufixo).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
				document.FindByIdOrName("hora_fim" + sufixo).Select(tarefaDiaria.Termino.ToHora(), false);
				document.FindByIdOrName("intervalo" + sufixo).Select(TimeSpan.Zero.ToHora(), false);//.Focus();
				document.FindByIdOrName("DESCRICAO" + sufixo).Select(tarefaDiaria.Descricao, false);
			}
			return true;
		}

		protected override void WaitFinish(IWebDriver browser)
		{
			try
			{
				Salvar(browser);
			}
			catch (Exception) { }
			finally
			{
				Thread.Sleep(5000);
				base.WaitFinish(browser);
			}
		}

		private Boolean Salvar(IWebDriver document)
		{
			var button = document.FindByIdOrName("COMANDO_SALVAR_TOP");
			if (AutoSaveClick)
				button.Click();
			else
				AguardeUsuarioGravar(button);

			AguardeGravacao(button);

			return true;
		}

		private void AguardeUsuarioGravar(IWebElement button)
		{
			button.Focus();
			while (button.Text == "SALVAR")
				Thread.Sleep(1000);
		}

		private static void AguardeGravacao(IWebElement button)
		{
			Thread.Sleep(1000);
			while (button.Text != "SALVAR")
				Thread.Sleep(1000);
		}
	}
}