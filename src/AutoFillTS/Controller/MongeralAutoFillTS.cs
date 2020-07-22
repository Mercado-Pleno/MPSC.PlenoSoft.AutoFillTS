using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSC.PlenoSoft.Selenium.Extension;
using MPSC.PlenoSoft.WatiN.Extension.Util;
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
				WatiNExtension.Wait();

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
			WatiNExtension.Wait();

			seleniumRWD.Set("ddlProjeto", tarefa.Projeto, 1500);
			seleniumRWD.Set("ddlSistema", tarefa.Sistema);
			seleniumRWD.Set("ddlCategoria", tarefa.Categoria, 1500);
			seleniumRWD.Set("ddlTipoAtividade", tarefa.TipoAtividade);

			seleniumRWD.Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy"));
			seleniumRWD.Set("txtInicioAtividade", tarefa.Inicio);
			seleniumRWD.Set("txtFimAtividade", tarefa.Termino);
			seleniumRWD.Set("txtDescricao", tarefa.Descricao);
			seleniumRWD.Set("ddlTipoControle", tarefa.TipoControle);
			seleniumRWD.Set("txtTipoControleDetalhes", tarefa.ValorControle);
			seleniumRWD.Set("btnSalvar", AutoSaveClick);

			while (seleniumRWD.EstaPreenchido("txtDescricao", tarefa.Descricao))
				WatiNExtension.Wait();

			return true;
		}

		protected override void WaitFinish(SeleniumRWD seleniumRWD)
		{
			try
			{
				seleniumRWD.Set("btn_Fechar", true);
				while (seleniumRWD.ContainsAnyText(false, "Soma="))
					WatiNExtension.Wait();
			}
			catch (Exception) { }
			finally
			{
				base.WaitFinish(seleniumRWD);
			}
		}
	}
}