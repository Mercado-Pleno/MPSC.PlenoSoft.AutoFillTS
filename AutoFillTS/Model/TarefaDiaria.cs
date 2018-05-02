using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.AutoFillTS.Model
{
	public class TarefaDiaria
	{
		private readonly TimeSpan oitoHoras = TimeSpan.FromHours(8);
		public readonly DateTime Data;
		public readonly IEnumerable<Periodo> Horarios;
		public readonly String Descricao;

		public TimeSpan Inicio { get { return Horarios.Min(h => h.Inicio); } }
		public TimeSpan Termino { get { return Horarios.Max(h => h.Termino); } }
		public TimeSpan TerminoHorasComuns { get { return Termino.Add(-HorasExtras); } }
		public TimeSpan HorasExtras { get { return (TotalTrabalhado > oitoHoras) ? TotalTrabalhado - oitoHoras : TimeSpan.Zero; } }
		public TimeSpan TotalTrabalhado { get { return TimeSpan.FromTicks(Horarios.Sum(h => (h.Termino - h.Inicio).Ticks)); } }
		public TimeSpan TempoComIntervalos { get { return (Termino - Inicio); } }
		public TimeSpan Intervalo { get { return (TempoComIntervalos - TotalTrabalhado); } }

		private TarefaDiaria(DateTime data, IEnumerable<Tarefa> tarefas)
		{
			Data = data;
			Horarios = tarefas.Select(t => new Periodo(t)).ToArray();
			Descricao = String.Join(" + ", tarefas.GroupBy(t => t.Descricao).Select(t => t.Key));
		}

		public static IEnumerable<TarefaDiaria> Agrupar(IEnumerable<Tarefa> tarefas)
		{
			return tarefas.GroupBy(t => t.Data).Select(a => new TarefaDiaria(a.Key, a)).ToArray();
		}
	}
}