using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium.Drivers;
using System;
using System.Collections.Generic;
using System.Threading;
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
				ok = ok && Fill(seleniumDriver, item);

			return ok;
		}

		private bool Fill(SeleniumDriver seleniumDriver, Tarefa item, int loop = 0)
		{
			try
			{
				Application.DoEvents();
				return PreencherPorTarefa(seleniumDriver, item);
			}
			catch when (loop < 5)
			{
				seleniumDriver.Wait(wait);
				return Fill(seleniumDriver, item, loop + 1);
			}
			catch (Exception)
			{
				return false;
			}
		}


		private void TryCore(DateTime limitTime, Action action)
		{
			try
			{
				action.Invoke();
			}
			catch when (DateTime.UtcNow < limitTime)
			{
				Thread.Sleep(wait);
				TryCore(limitTime, action);
			}
		}

		private void Try(TimeSpan timeOut, Action action) => TryCore(DateTime.UtcNow.Add(timeOut), action);

		private Boolean PreencherPorTarefa(SeleniumDriver seleniumDriver, Tarefa tarefa)
		{
			Try(tryTimeOut, () => seleniumDriver.Set("ddlCategoria", tarefa.Categoria));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlProjeto", tarefa.Projeto));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlSistema", tarefa.Sistema));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlTipoAtividade", tarefa.TipoAtividade));
			Try(tryTimeOut, () => seleniumDriver.Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy")));
			Try(tryTimeOut, () => seleniumDriver.Set("txtInicioAtividade", tarefa.Inicio));
			Try(tryTimeOut, () => seleniumDriver.Set("txtFimAtividade", tarefa.Termino));
			Try(tryTimeOut, () => seleniumDriver.Set("txtDescricao", tarefa.Descricao));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlTipoControle", tarefa.TipoControle));
			Try(tryTimeOut, () => seleniumDriver.Set("txtTipoControleDetalhes", tarefa.ValorControle));
			Try(tryTimeOut, () => seleniumDriver.Set("btnSalvar", AutoSaveClick));

			while (seleniumDriver.Wait(wait).IsEquals("txtDescricao", tarefa.Descricao))
			{
				Application.DoEvents();
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