using MPSC.AutoFillTS.Model;
using MPSC.PlenoSoft.WatiN.Extension.Util;
using System;
using System.Collections.Generic;
using WatiN.Core;

namespace MPSC.AutoFillTS.Controller
{
	public class MongeralAutoFillTS : AbstractAutoFillTS
	{
		protected override string UrlLogin { get { return "http://gestaodeatividades.mongeral.seguros/Lists/RegistrosSistemas/NewForm.aspx"; } }
		protected override IEnumerable<String> Urls { get { yield return UrlLogin; } }

		public MongeralAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(Document document)
		{
			var ok = document.WaitContainsAllText(15, "Tipo de Atividade");
		}

		protected override bool Fill(Document document, TimeSheet timeSheet)
		{
			var ok = true;
			foreach (var item in timeSheet.Tarefas)
			{
				try
				{
					ok = ok && PreencherPorTarefa(document, item);
				}
				catch (Exception)
				{
					ok = false;
				}
			}
			return ok;

		}

		private Boolean PreencherPorTarefa(Document document, Tarefa tarefa)
		{
			WatiNExtension.Wait();

			document.SelectList(e => e.FindByIdOrName("ddlProjeto")).Select(tarefa.Projeto, true);
			document.SelectList(e => e.FindByIdOrName("ddlSistema")).Select(tarefa.Sistema, true);
			document.SelectList(e => e.FindByIdOrName("ddlCategoria")).Select(tarefa.Categoria, true);
			document.SelectList(e => e.FindByIdOrName("ddlTipoAtividade")).Select(tarefa.TipoAtividade, true);

			document.TextField(e => e.FindByIdOrName("dtcDataDate")).Select(tarefa.Data.ToString("dd/MM/yyyy"), false);
			document.TextField(e => e.FindByIdOrName("txtInicioAtividade")).Select(tarefa.Inicio, false);
			document.TextField(e => e.FindByIdOrName("txtFimAtividade")).Select(tarefa.Termino, false);
			document.TextField(e => e.FindByIdOrName("txtDescricao")).Select(tarefa.Descricao, false);
			document.SelectList(e => e.FindByIdOrName("ddlTipoControle")).Select(tarefa.TipoControle, false);
			document.TextField(e => e.FindByIdOrName("txtTipoControleDetalhes")).Select(tarefa.ValorControle, false);
			document.Link(e => e.FindByIdOrName("btnSalvar")).Select(AutoSaveClick, false);

			while (document.EstaPreenchido("txtDescricao", tarefa.Descricao))
				WatiNExtension.Wait();

			return true;
		}

		protected override void WaitFinish(Browser browser)
		{
			try
			{
				browser.Link(e => e.FindByIdOrName("Btn_Fechar")).Select(true, false);
				while (browser.ContainsText("Soma="))
					WatiNExtension.Wait();
			}
			catch (Exception) { }
			finally
			{
				base.WaitFinish(browser);
			}
		}
	}
}