using System;

namespace MPSC.AutoFillTS.Model
{
	public class Tarefa
	{
		public String Projeto { get; private set; }
		public String Sistema { get; private set; }
		public String Categoria { get; private set; }
		public String TipoAtividade { get; private set; }
		public DateTime Data { get; private set; }
		public String Inicio { get; private set; }
		public String Termino { get; private set; }
		public String Descricao { get; private set; }
		public String TipoControle { get; private set; }
		public String ValorControle { get; private set; }

		public Tarefa(String projeto, String sistema, String categoria, String tipoAtividade, DateTime data, String inicio, String termino, String descricao, String tipoControle, String valorControle)
		{
			Projeto = projeto;
			Sistema = sistema;
			Categoria = categoria;
			TipoAtividade = tipoAtividade;
			Data = data;
			Inicio = inicio;
			Termino = termino;
			Descricao = descricao;
			TipoControle = tipoControle;
			ValorControle = valorControle;
		}
	}
}