using MPSC.AutoFillTS.Infra;
using System;

namespace MPSC.AutoFillTS.Model
{
	public class Periodo
	{
		public readonly TimeSpan Inicio;
		public readonly TimeSpan Termino;

		public Periodo(Tarefa tarefa)
		{
			Inicio = tarefa.Inicio.ToTimeSpan();
			Termino = tarefa.Termino.ToTimeSpan();
		}
	}
}