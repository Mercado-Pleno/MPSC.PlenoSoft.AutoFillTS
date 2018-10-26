using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using WatiN.Core;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public class ProviderItAutoFillTS : AbstractAutoFillTS
	{
		private TextField[] cacheTexts;
		private SelectList[] cacheCombos;
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

		protected override void EsperarPeloLogin(Document document)
		{
			WatiNExtension.Wait();

			if (document.ContainsAnyText("INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(document.TextField(e => e.FindByIdOrName("USUARIO")).Text);
				var passOk = !String.IsNullOrWhiteSpace(document.TextField(e => e.FindByIdOrName("SENHA")).Text);
				if (userOk || passOk)
					document.Button(e => e.FindByIdOrName("COMANDO")).Click();
			}

			while (document.ContainsAnyText("INGRESSAR", "Recuperar senha"))
				WatiNExtension.Wait();
		}

		private static void SelecionarCompetencia(Document document, TimeSheet timeSheet)
		{
			var comboCompetencia = document.SelectList(e => e.FindByIdOrName("ANOMES_TELA"));
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;
			if (comboCompetencia.GetAttributeValue("value") != competencia.ToString())
				comboCompetencia.Select(competencia, true);
		}

		private static void SelecionarTipoHora(Document document, string tipo)
		{
			var form = document.Form(e => e.FindByIdOrName("frmCadDoc"));
			var txt = form.TextField(e => e.FindByIdOrName("TIPO_HORA"));
			var tipohora = $"tipoHora('{txt.Value}')";

			var cell = GetCell(form, tipo);
			while (GetCell(cell, tipo) != null)
				cell = GetCell(cell, tipo);

			if (!cell.OuterHtml.Contains(tipohora))
				cell.Click();
		}

		private void GuardarEmCache(Document document)
		{
			while (!document.ContainsAllText("REGISTRO DE HORAS", "COMPETÊNCIA", "HORAS NORMAIS", "HORAS EXTRAS", "SOBREAVISO"))
				WatiNExtension.Wait();

			cacheTexts = document.TextFields.Where(tf => tf.Exists).ToArray();
			cacheCombos = document.SelectLists.Where(tf => tf.Exists).ToArray();
		}

		private static TableCell GetCell(IElementContainer container, String tipo)
		{
			return container.TableCells.FirstOrDefault(
				t => t.Exists &&
				!String.IsNullOrWhiteSpace(t.OuterText) &&
				(t.OuterText.Trim().ToUpper() == tipo.Trim().ToUpper()));
		}

		protected override bool Fill(Document document, TimeSheet timeSheet)
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

		private Boolean PreencherHorasNormais(Document document, TarefaDiaria tarefaDiaria)
		{
			var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

			cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.Inicio.ToHora(), false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(tarefaDiaria.Intervalo.ToHora(), false)?.Focus();
			cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);

			return true;
		}

		private Boolean PreencherHorasExtas(Document document, TarefaDiaria tarefaDiaria)
		{
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				cacheCombos.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.Termino.ToHora(), false);
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(TimeSpan.Zero.ToHora(), false).Focus();
				cacheTexts.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);
			}
			return true;
		}

		protected override void WaitFinish(Browser browser)
		{
			try
			{
				Salvar(browser);
			}
			catch (Exception) { }
			finally
			{
				WatiNExtension.Wait(TimeSpan.FromSeconds(5));
				base.WaitFinish(browser);
			}
		}

		private Boolean Salvar(Document document)
		{
			var button = document.Button(e => e.FindByIdOrName("COMANDO_SALVAR_TOP"));
			if (AutoSaveClick)
				button.ClickNoWait();
			else
				AguardeUsuarioGravar(button);

			AguardeGravacao(button);

			return true;
		}

		private void AguardeUsuarioGravar(Button button)
		{
			button.Focus();
			while (button.Exists && button.Value == "SALVAR")
				WatiNExtension.Wait();
		}

		private static void AguardeGravacao(Button button)
		{
			WatiNExtension.Wait(TimeSpan.FromSeconds(1));
			while (button.Exists && button.Value != "SALVAR")
				WatiNExtension.Wait(TimeSpan.FromSeconds(1));
		}
	}
}