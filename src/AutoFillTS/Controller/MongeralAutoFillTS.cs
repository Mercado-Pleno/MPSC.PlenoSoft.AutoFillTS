using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public class MongeralAutoFillTS : AbstractAutoFillTS
	{
		protected override string UrlLogin { get { return "http://gestaodeatividades.mongeral.seguros/Lists/RegistrosSistemas/NewForm.aspx"; } }
		protected override IEnumerable<String> Urls { get { yield return UrlLogin; } }

		public MongeralAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(IWebDriver document)
		{
			var ok = document.WaitUntilContainsAllText(false, "Tipo de Atividade");
		}

		protected override bool Fill(IWebDriver document, TimeSheet timeSheet)
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

		private Boolean PreencherPorTarefa(IWebDriver document, Tarefa tarefa)
		{
			document.FindByIdOrName("ddlProjeto").Select(tarefa.Projeto, true);
			document.FindByIdOrName("ddlSistema").Select(tarefa.Sistema, true);
			document.FindByIdOrName("ddlCategoria").Select(tarefa.Categoria, true);
			document.FindByIdOrName("ddlTipoAtividade").Select(tarefa.TipoAtividade, true);

			document.FindByIdOrName("dtcDataDate").Select(tarefa.Data.ToString("dd/MM/yyyy"), false);
			document.FindByIdOrName("txtInicioAtividade").Select(tarefa.Inicio, false);
			document.FindByIdOrName("txtFimAtividade").Select(tarefa.Termino, false);
			document.FindByIdOrName("txtDescricao").Select(tarefa.Descricao, false);
			document.FindByIdOrName("ddlTipoControle").Select(tarefa.TipoControle, false);
			document.FindByIdOrName("txtTipoControleDetalhes").Select(tarefa.ValorControle, false);
			document.FindByIdOrName("btnSalvar").Select(AutoSaveClick, false);

			//while (document.EstaPreenchido("txtDescricao", tarefa.Descricao))
			//	WatiNExtension.Wait();

			return true;
		}

		protected override void WaitFinish(IWebDriver browser)
		{
			try
			{
				browser.FindByIdOrName("Btn_Fechar").Select(true, false);
				browser.WaitWhileContainsAllText(true, "Soma=");
			}
			catch (Exception) { }
			finally
			{
				base.WaitFinish(browser);
			}
		}
	}
}