using MPSC.AutoFillTS.Infra;
using MPSC.AutoFillTS.Model;
using System;
using System.Linq;

namespace MPSC.AutoFillTS.Controller
{
	public class TimeSheetAnalyser
	{
		public static Boolean AnalisarTimeSheet(TimeSheet timeSheet)
		{
			Boolean confirmar = false;
			if (timeSheet != null)
			{
				var mesesApurados = timeSheet.Tarefas.Select(t => t.Data.Month).Distinct().Count();
				var menorMesApurado = timeSheet.Tarefas.Select(t => t.Data.AddDays(-t.Data.Day + 1)).Distinct().Min();
				var maiorMesApurado = timeSheet.Tarefas.Select(t => t.Data.AddDays(-t.Data.Day + 1)).Distinct().Max();
				var apenasUmMes = menorMesApurado == maiorMesApurado;
				var expedienteEmFinaisDeSemana = timeSheet.Tarefas.Select(t => t.Data).Distinct().Where(d => d.DayOfWeek == (DayOfWeek.Saturday | DayOfWeek.Sunday)).Count();
				var totalDiasTrabalhados = timeSheet.Tarefas.Select(t => t.Data).Distinct().Count();
				var totalPeriodos = timeSheet.Tarefas.Count();
				var minutosTrabalhados = timeSheet.Tarefas.Sum(t => t.Termino.EmMinutos() - t.Inicio.EmMinutos());
				var minutosPorDia = minutosTrabalhados / totalDiasTrabalhados;

				Mensagem.Instancia.WriteLine("\n===============================================================================\n");
				Mensagem.Instancia.WriteLine("Quantidade de Meses Apurados    = {0,8} ({1:MMMM/yyyy} - {2:MMMM/yyyy})", mesesApurados, menorMesApurado, maiorMesApurado);
				Mensagem.Instancia.WriteLine("Quantidade de Dias Apurados     = {0,8}", apenasUmMes ? menorMesApurado.AddMonths(1).AddDays(-1).Day : menorMesApurado.AddMonths(1).AddDays(-1).Day + maiorMesApurado.AddMonths(1).AddDays(-1).Day);
				Mensagem.Instancia.WriteLine("Quantidade de Dias Úteis        = {0,8}", apenasUmMes ? menorMesApurado.DiasUteisNoMes() : menorMesApurado.DiasUteisNoMes() + maiorMesApurado.DiasUteisNoMes());
				Mensagem.Instancia.WriteLine("Expediente em Final de Semana   = {0,8}", expedienteEmFinaisDeSemana);
				Mensagem.Instancia.WriteLine("Primeiro Dia Trabalhado         = {0}", timeSheet.Tarefas.Min(t => t.Data).ToString("dd/MM/yy (dddd)"));
				Mensagem.Instancia.WriteLine("Último Dia Trabalhado           = {0}", timeSheet.Tarefas.Max(t => t.Data).ToString("dd/MM/yy (dddd)"));
				Mensagem.Instancia.WriteLine("Quantidade de Dias Trabalhados  = {0,8}", totalDiasTrabalhados);
				Mensagem.Instancia.WriteLine("Total de Horários registrados   = {0,8}", totalPeriodos);
				Mensagem.Instancia.WriteLine("Total de Horas Trabalhadas      = {0,8} ({1:##0.00})", minutosTrabalhados.EmHoras(), Convert.ToDecimal(minutosTrabalhados) / 60M);
				Mensagem.Instancia.WriteLine("Média de Horas Diárias          = {0,8} ({1:##0.00})", minutosPorDia.EmHoras(), Convert.ToDecimal(minutosPorDia) / 60M);
				Mensagem.Instancia.WriteLine("\n===============================================================================\n");
				confirmar = Mensagem.Instancia.Confirmar("\nConfirma importação?");
			}
			return confirmar;
		}
	}
}