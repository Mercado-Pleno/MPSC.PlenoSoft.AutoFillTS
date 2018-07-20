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
			Text += " " + CoreAssembly.VersionString;
			TopLevel = true;
			Mensagem.iMensagem = this;
			lblStatusBar.Text = CoreAssembly.VersionString;
		}

		private void Analisar(Object sender, EventArgs e)
		{
			try
			{
				var timeSheet = TimeSheetFactory.Load(txtHorarios.Text);
				dgvHorarios.DataSource = (timeSheet == null) ? null : timeSheet.Tarefas;
				lblStatusBar.Text = $"Ok às {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";
			}
			catch (Exception exception)
			{
				dgvHorarios.DataSource = new TimeSheet().Tarefas;
				lblStatusBar.Text = $"{exception.Message} às {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}";
			}
			finally
			{
				dgvHorarios.AutoResizeColumns();
			}
		}

		private void btProcessar_Click(Object sender, EventArgs e)
		{
			Msg = String.Empty;
			if (String.IsNullOrWhiteSpace(txtHorarios.Text))
				MessageBox.Show(lblStatusBar.Text = $"Informe seus horários às {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}");
			else
			{
				var timeSheet = TimeSheetFactory.Load(txtHorarios.Text);
				if (timeSheet == null)
					MessageBox.Show(lblStatusBar.Text = $"Informe seus horários corretamente às {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}");
				else if (!timeSheet.Tarefas.Any())
					MessageBox.Show(lblStatusBar.Text = $"Não há horários para processar às {DateTime.Now.ToString("dd/MM/yy HH:mm:ss")}");
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