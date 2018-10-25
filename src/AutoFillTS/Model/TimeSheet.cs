using System;
using System.Collections.Generic;

namespace MPSC.AutoFillTS.Model
{
	public class TimeSheet
	{
		private readonly IList<Tarefa> _listaTarefa = new List<Tarefa>();
		public IEnumerable<Tarefa> Tarefas { get { return _listaTarefa; } }
		public IEnumerable<TarefaDiaria> TarefasDiarias { get { return TarefaDiaria.Agrupar(_listaTarefa); } }

		public Tarefa Adicionar(String projeto, String sistema, String categoria, String tipoAtividade, DateTime data, String inicio, String termino, String descricao, String tipoControle, String valorControle)
		{
			var tarefa = new Tarefa(projeto, sistema, categoria, tipoAtividade, data, inicio, termino, descricao, tipoControle, valorControle);
			_listaTarefa.Add(tarefa);
			return tarefa;
		}
	}
}