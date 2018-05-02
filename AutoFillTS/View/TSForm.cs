using MPSC.AutoFillTS.Controller;
using MPSC.AutoFillTS.Infra;
using MPSC.AutoFillTS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MPSC.AutoFillTS.View
{
	public partial class TSForm : Form, IMensagem
	{
		public String Msg { get; set; }

		public TSForm()
		{
			InitializeComponent();
			this.Text += " " + CoreAssembly.VersionString;
			this.TopLevel = true;
			Mensagem.iMensagem = this;
		}

		private void Analisar(Object sender, EventArgs e)
		{
			try
			{
				var timeSheet = TimeSheetFactory.Load(txtHorarios.Text);
				dgvHorarios.DataSource = (timeSheet == null) ? null : timeSheet.Tarefas;
				dgvHorarios.AutoResizeColumns();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		private void btProcessar_Click(Object sender, EventArgs e)
		{
			Msg = String.Empty;
			if (String.IsNullOrWhiteSpace(txtHorarios.Text))
				MessageBox.Show("Informe seus horários");
			else
			{
				var timeSheet = TimeSheetFactory.Load(txtHorarios.Text);
				if (timeSheet == null)
					MessageBox.Show("Informe seus horários corretamente");
				else if (!timeSheet.Tarefas.Any())
					MessageBox.Show("Não há horários para processar");
				else
				{
					dgvHorarios.DataSource = timeSheet.Tarefas;
					var st = WindowState;
					var continuar = TimeSheetAnalyser.AnalisarTimeSheet(timeSheet);
					var preenchedores = ObterPreenchedores(ckAutoSaveClick.Checked);

					WindowState = continuar ? FormWindowState.Minimized : st;
					continuar = continuar && AutoFillTSController.Processar(preenchedores, timeSheet);
					WindowState = st;
				}
			}
		}

		private IEnumerable<AbstractAutoFillTS> ObterPreenchedores(Boolean autoSaveClick)
		{
			yield return new MongeralAutoFillTS(ckPreencherMongeral.Checked, autoSaveClick);
			yield return new ProviderItAutoFillTS(ckPreencherProvider.Checked, autoSaveClick);
		}

		public void Write(String mensagem)
		{
			if (mensagem.Contains("=") && !mensagem.StartsWith("\n="))
				Msg += Regex.Replace(mensagem, "[ ]+", " ");
		}

		private void Observar(Object sender, KeyEventArgs e)
		{
			if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
				txtHorarios.SelectAll();
		}
	}
}