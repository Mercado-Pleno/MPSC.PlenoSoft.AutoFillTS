using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Selenium.Extension;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
				ok = ok && Fill(seleniumRWD, item, 1);
			}
			return ok;
		}

		private bool Fill(SeleniumRWD seleniumRWD, Tarefa item, int sleep)
		{
			try
			{
				return PreencherPorTarefa(seleniumRWD, item, sleep);
			}
			catch (Exception) when (sleep < 1000)
			{
				return Fill(seleniumRWD, item, sleep * 2);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private Boolean PreencherPorTarefa(SeleniumRWD seleniumRWD, Tarefa tarefa, int sleep)
		{
			WaitExtension.Wait();
			Application.DoEvents();

			seleniumRWD.Set("ddlProjeto", tarefa.Projeto, sleep);
			seleniumRWD.Set("ddlSistema", tarefa.Sistema, sleep);
			seleniumRWD.Set("ddlCategoria", tarefa.Categoria, sleep);
			seleniumRWD.Set("ddlTipoAtividade", tarefa.TipoAtividade, sleep);
			seleniumRWD.Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy"), sleep);
			seleniumRWD.Set("txtInicioAtividade", tarefa.Inicio, sleep);
			seleniumRWD.Set("txtFimAtividade", tarefa.Termino, sleep);
			seleniumRWD.Set("txtDescricao", tarefa.Descricao, sleep);
			seleniumRWD.Set("ddlTipoControle", tarefa.TipoControle, sleep);
			seleniumRWD.Set("txtTipoControleDetalhes", tarefa.ValorControle, sleep);

			seleniumRWD.Set("btnSalvar", AutoSaveClick);

			while (seleniumRWD.EstaPreenchido("txtDescricao", tarefa.Descricao))
			{ 
				WaitExtension.Wait();
				Application.DoEvents();
				seleniumRWD.Set("btnSalvar", AutoSaveClick);
			}

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