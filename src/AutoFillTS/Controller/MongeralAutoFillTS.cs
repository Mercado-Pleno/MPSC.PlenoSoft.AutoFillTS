using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSC.PlenoSoft.Selenium.Extension;
using System;
using System.Collections.Generic;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
	public class MongeralAutoFillTS : AbstractAutoFillTS
	{
		protected override string UrlLogin { get { return "http://gestaodeatividades.mongeral.seguros/Lists/RegistrosSistemas/NewForm.aspx"; } }
		protected override IEnumerable<String> Urls { get { yield return UrlLogin; } }

		public MongeralAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(SeleniumRWD seleniumRWD)
		{
			while (seleniumRWD.IsEmptyPageSource)
				WaitExtension.Wait();

			var _ = seleniumRWD.WaitUntilContainsAllText(Token, false, "Tipo de Atividade");
		}

		protected override bool Fill(SeleniumRWD seleniumRWD, TimeSheet timeSheet)
		{
			var ok = true;
			foreach (var item in timeSheet.Tarefas)
			{
				try
				{
					ok = ok && PreencherPorTarefa(seleniumRWD, item);
				}
				catch (Exception)
				{
					ok = false;
				}
			}
			return ok;
		}

		private Boolean PreencherPorTarefa(SeleniumRWD seleniumRWD, Tarefa tarefa)
		{
			WaitExtension.Wait();

			seleniumRWD.Set("ddlProjeto", tarefa.Projeto, 150);
			seleniumRWD.Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy"), 150);
			seleniumRWD.Set("txtInicioAtividade", tarefa.Inicio, 150);
			seleniumRWD.Set("txtFimAtividade", tarefa.Termino, 150);
			seleniumRWD.Set("ddlSistema", tarefa.Sistema, 150);
			seleniumRWD.Set("ddlCategoria", tarefa.Categoria, 150);
			seleniumRWD.Set("ddlTipoControle", tarefa.TipoControle, 150);
			seleniumRWD.Set("txtTipoControleDetalhes", tarefa.ValorControle, 150);
			seleniumRWD.Set("txtDescricao", tarefa.Descricao, 150);
			seleniumRWD.Set("ddlTipoAtividade", tarefa.TipoAtividade, 150);

			seleniumRWD.Set("btnSalvar", AutoSaveClick);

			while (seleniumRWD.EstaPreenchido("txtDescricao", tarefa.Descricao))
				WaitExtension.Wait();

			return true;
		}

		protected override void WaitFinish(SeleniumRWD seleniumRWD)
		{
			try
			{
				seleniumRWD.Set("btn_Fechar", true);
				while (seleniumRWD.ContainsAnyText(false, "Soma="))
					WaitExtension.Wait();
			}
			catch (Exception) { }
			finally
			{
				base.WaitFinish(seleniumRWD);
			}
		}
	}
}