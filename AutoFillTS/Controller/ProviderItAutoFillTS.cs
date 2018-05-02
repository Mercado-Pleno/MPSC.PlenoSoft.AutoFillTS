using MPSC.AutoFillTS.Infra;
using MPSC.AutoFillTS.Model;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using WatiN.Core;

namespace MPSC.AutoFillTS.Controller
{
	public class ProviderItAutoFillTS : AbstractAutoFillTS
	{
		protected override String UrlLogin { get { return "http://186.215.208.202/intranet/login/login.asp"; } }
		protected override IEnumerable<String> Urls { get { yield return String.Format("http://186.215.208.202/intranet/bcp/apropriacao_de_horas/timesheet/?th=HN&dt={0}&am={0}", DateTime.Today.ToString("yyyyMM")); } }

		private TextField[] cache1;
		private SelectList[] cache2;

		public ProviderItAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(Document document)
		{
			WatiNExtension.Wait();

			if (document.ContainsAnyText("INGRESSAR", "Recuperar senha"))
			{
				var userOk = !String.IsNullOrWhiteSpace(document.TextField(e => e.FindByIdOrName("USUARIO")).Text);
				var passOk = !String.IsNullOrWhiteSpace(document.TextField(e => e.FindByIdOrName("USUARIO")).Text);
				if (userOk || passOk)
					document.Button(e => e.FindByIdOrName("COMANDO")).Click();
			}

			while (document.ContainsAnyText("INGRESSAR", "Recuperar senha"))
				WatiNExtension.Wait();
		}

		private static void SelecionarCompetencia(Document document, TimeSheet timeSheet)
		{
			var data = timeSheet.TarefasDiarias.Min(t => t.Data);
			var competencia = data.Year * 100 + data.Month;
			document.SelectList(e => e.FindByIdOrName("ANOMES_TELA")).Select(competencia, true);
		}

		private static void SelecionarTipoHora(Document document, string tipo)
		{
			var form = document.Form(e => e.FindByIdOrName("frmCadDoc"));
			var cell = GetCell(form, tipo);
			while (GetCell(cell, tipo) != null)
				cell = GetCell(cell, tipo);
			cell.Click();
		}

		private void GuardarEmCache(Document document)
		{
			cache1 = document.TextFields.Where(tf => tf.Exists).ToArray();
			cache2 = document.SelectLists.Where(tf => tf.Exists).ToArray();
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

			cache2.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
			cache1.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.Inicio.ToHora(), false);
			cache1.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
			cache1.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(tarefaDiaria.Intervalo.ToHora(), false).Focus();
			cache1.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);

			return true;
		}

		private Boolean PreencherHorasExtas(Document document, TarefaDiaria tarefaDiaria)
		{
			if (tarefaDiaria.HorasExtras > TimeSpan.Zero)
			{
				var sufixo = String.Format("_{0}_{1}", tarefaDiaria.Data.ToString("yyyyMMdd"), "1");

				cache2.FirstOrDefault(e => e.FindByIdOrName("ID_PROJETO" + sufixo)).Select(51432, false);
				cache1.FirstOrDefault(e => e.FindByIdOrName("hora_ini" + sufixo)).Select(tarefaDiaria.TerminoHorasComuns.ToHora(), false);
				cache1.FirstOrDefault(e => e.FindByIdOrName("hora_fim" + sufixo)).Select(tarefaDiaria.Termino.ToHora(), false);
				cache1.FirstOrDefault(e => e.FindByIdOrName("intervalo" + sufixo)).Select(TimeSpan.Zero.ToHora(), false).Focus();
				cache1.FirstOrDefault(e => e.FindByIdOrName("DESCRICAO" + sufixo)).Select(tarefaDiaria.Descricao, false);
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