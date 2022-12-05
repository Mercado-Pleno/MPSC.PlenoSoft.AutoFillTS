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

		protected override void EsperarPeloLogin(SeleniumDriver seleniumDriver)
		{
			while (seleniumDriver.IsEmptyPageSource)
				seleniumDriver.Wait(wait);

			_ = seleniumDriver.WaitUntilContainsAllText(Token, false, "Tipo de Atividade");
		}

		protected override bool Fill(SeleniumDriver seleniumDriver, TimeSheet timeSheet)
		{
			var ok = true;
			foreach (var item in timeSheet.Tarefas)
			{
				ok = ok && Fill(seleniumDriver, item, 1);
			}
			return ok;
		}

		private bool Fill(SeleniumDriver seleniumDriver, Tarefa item, int sleep)
		{
			try
			{
				return PreencherPorTarefa(seleniumDriver, item, TimeSpan.FromMilliseconds(sleep));
			}
			catch (Exception) when (sleep < 1000)
			{
				return Fill(seleniumDriver, item, sleep * 2);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private Boolean PreencherPorTarefa(SeleniumDriver seleniumDriver, Tarefa tarefa, TimeSpan sleep)
		{
			seleniumDriver.Wait(sleep);
			Application.DoEvents();

			seleniumDriver.Wait(sleep).Set("ddlProjeto", tarefa.Projeto);
			seleniumDriver.Wait(sleep).Set("ddlSistema", tarefa.Sistema);
			seleniumDriver.Wait(sleep).Set("ddlCategoria", tarefa.Categoria);
			seleniumDriver.Wait(sleep).Set("ddlTipoAtividade", tarefa.TipoAtividade);
			seleniumDriver.Wait(sleep).Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy"));
			seleniumDriver.Wait(sleep).Set("txtInicioAtividade", tarefa.Inicio);
			seleniumDriver.Wait(sleep).Set("txtFimAtividade", tarefa.Termino);
			seleniumDriver.Wait(sleep).Set("txtDescricao", tarefa.Descricao);
			seleniumDriver.Wait(sleep).Set("ddlTipoControle", tarefa.TipoControle);
			seleniumDriver.Wait(sleep).Set("txtTipoControleDetalhes", tarefa.ValorControle);
			seleniumDriver.Wait(sleep).Set("btnSalvar", AutoSaveClick);

			while (seleniumDriver.IsEquals("txtDescricao", tarefa.Descricao))
			{
				seleniumDriver.Wait(wait);
				Application.DoEvents();
				seleniumDriver.Set("btnSalvar", AutoSaveClick);
			}

			return true;
		}

		protected override void WaitFinish(SeleniumDriver seleniumDriver)
		{
			try
			{
				seleniumDriver.Set("btn_Fechar", true);
				while (seleniumDriver.ContainsAnyText(false, "Soma="))
					seleniumDriver.Wait(wait);
			}
			finally
			{
				base.WaitFinish(seleniumDriver);
			}
		}
	}
}