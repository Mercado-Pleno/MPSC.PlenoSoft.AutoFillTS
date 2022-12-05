using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium;
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

		protected override void EsperarPeloLogin(SeleniumWd seleniumWd)
		{
			while (seleniumWd.IsEmptyPageSource)
				seleniumWd.Wait(wait);

			_ = seleniumWd.WaitUntilContainsAllText(Token, false, "Tipo de Atividade");
		}

		protected override bool Fill(SeleniumWd seleniumWd, TimeSheet timeSheet)
		{
			var ok = true;
			foreach (var item in timeSheet.Tarefas)
			{
				ok = ok && Fill(seleniumWd, item, 1);
			}
			return ok;
		}

		private bool Fill(SeleniumWd seleniumWd, Tarefa item, int sleep)
		{
			try
			{
				return PreencherPorTarefa(seleniumWd, item, TimeSpan.FromMilliseconds(sleep));
			}
			catch (Exception) when (sleep < 1000)
			{
				return Fill(seleniumWd, item, sleep * 2);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private Boolean PreencherPorTarefa(SeleniumWd seleniumWd, Tarefa tarefa, TimeSpan sleep)
		{
			seleniumWd.Wait(sleep);
			Application.DoEvents();

			seleniumWd.Wait(sleep).Set("ddlProjeto", tarefa.Projeto);
			seleniumWd.Wait(sleep).Set("ddlSistema", tarefa.Sistema);
			seleniumWd.Wait(sleep).Set("ddlCategoria", tarefa.Categoria);
			seleniumWd.Wait(sleep).Set("ddlTipoAtividade", tarefa.TipoAtividade);
			seleniumWd.Wait(sleep).Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy"));
			seleniumWd.Wait(sleep).Set("txtInicioAtividade", tarefa.Inicio);
			seleniumWd.Wait(sleep).Set("txtFimAtividade", tarefa.Termino);
			seleniumWd.Wait(sleep).Set("txtDescricao", tarefa.Descricao);
			seleniumWd.Wait(sleep).Set("ddlTipoControle", tarefa.TipoControle);
			seleniumWd.Wait(sleep).Set("txtTipoControleDetalhes", tarefa.ValorControle);
			seleniumWd.Wait(sleep).Set("btnSalvar", AutoSaveClick);

			while (seleniumWd.TextIsEquals("txtDescricao", tarefa.Descricao))
			{
				seleniumWd.Wait(sleep);
				Application.DoEvents();
				seleniumWd.Set("btnSalvar", AutoSaveClick);
			}

			return true;
		}

		protected override void WaitFinish(SeleniumWd seleniumWd)
		{
			try
			{
				seleniumWd.Set("btn_Fechar", true);
				while (seleniumWd.ContainsAnyText(false, "Soma="))
					seleniumWd.Wait(wait);
			}
			finally
			{
				base.WaitFinish(seleniumWd);
			}
		}
	}
}