using MPSC.PlenoSoft.AutoFillTS.Infra;
using System;
using System.IO;
using System.Linq;

namespace MPSC.PlenoSoft.AutoFillTS.Model
{
	public class TimeSheetFactory
	{
		public enum TimeSheetFormat
		{
			Invalido = 0,
			Data_IniFim_Descricao = 4,
			Data_IniFim_Soma_Descricao = 5,
			Data_IniFim_IniFim_Descricao = 6,
			Data_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade = 8,
			Data_IniFim_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade = 10,
			Data_IniFim_IniFim_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade = 12,
			Data_IniFim_IniFim_IniFim_Tipo_Controle_Descricao_Projeto_Sistema_Categoria_Atividade = 14,
			Data_IniFim_Tipo_Controle_Soma_Descricao_Projeto_Sistema_Categoria_Atividade = 11,
			Data_IniFim_IniFim_Tipo_Controle_Soma_Descricao_Projeto_Sistema_Categoria_Atividade = 13,
			CGS = 1001,
		}

		public static TimeSheet Load(String arquivo)
		{
			return Load(arquivo.Split(new String[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
		}

		public static TimeSheet Load(String[] linhas)
		{
			var timeSheet = new TimeSheet();
			var timeSheetFormat = IdentificarFormato(linhas);
			switch (timeSheetFormat)
			{
				case TimeSheetFormat.Invalido:
					break;
				case TimeSheetFormat.Data_IniFim_Descricao:
				case TimeSheetFormat.Data_IniFim_Soma_Descricao:
				case TimeSheetFormat.Data_IniFim_IniFim_Descricao:
					timeSheet = LoadTxtFormato04_05_06(linhas);
					break;
				case TimeSheetFormat.Data_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade:
				case TimeSheetFormat.Data_IniFim_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade:
				case TimeSheetFormat.Data_IniFim_IniFim_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade:
				case TimeSheetFormat.Data_IniFim_IniFim_IniFim_Tipo_Controle_Descricao_Projeto_Sistema_Categoria_Atividade:
					timeSheet = LoadTxtFormato08(linhas);
					break;
				case TimeSheetFormat.Data_IniFim_Tipo_Controle_Soma_Descricao_Projeto_Sistema_Categoria_Atividade:
				case TimeSheetFormat.Data_IniFim_IniFim_Tipo_Controle_Soma_Descricao_Projeto_Sistema_Categoria_Atividade:
					timeSheet = LoadTxtFormato11(linhas);
					break;
				case TimeSheetFormat.CGS:
					timeSheet = LoadTxtFormatoCGS(linhas);
					break;
				default:
					break;
			}
			return timeSheet;
		}

		private static TimeSheet LoadTxtFormato04_05_06(String[] linhas)
		{
			var timeSheet = new TimeSheet();
			var projeto = "261";
			var sistema = "77";
			var categoria = "Desenvolvimento";
			var tipoAtividade = "Desenvolvimento - Projeto";
			var tipoControle = "Sem definição";
			var valorControle = String.Empty;

			foreach (var linha in linhas)
			{
				if (!String.IsNullOrWhiteSpace(linha))
				{
					var campos = linha.Split('\t');
					if (campos.Length >= 4)
					{
						var data = campos[0].ConverterEmData();
						var descricao = campos[campos.Length - 1];
						if (!String.IsNullOrWhiteSpace(descricao))
						{
							for (int i = 1; i < campos.Length - 2; i += 2)
							{
								var inicio = campos[i];
								var termino = campos[i + 1];
								if ((termino.EmMinutos() - inicio.EmMinutos()) > 0)
								{
									var tarefa = timeSheet.Adicionar(projeto, sistema, categoria, tipoAtividade, data, inicio.PadLeft(5, '0'), termino.PadLeft(5, '0'), descricao, tipoControle, valorControle);
									Log(tarefa);
								}
							}
						}
					}
				}
			}
			return timeSheet;
		}

		private static TimeSheet LoadTxtFormato08(String[] linhas)
		{
			var timeSheet = new TimeSheet();
			var tipoControle = "Sem definição";
			var valorControle = "";

			foreach (var linha in linhas)
			{
				if (!String.IsNullOrWhiteSpace(linha))
				{
					var campos = linha.Split('\t');
					var offSet = campos.Length - 8;
					var horarios = (campos.Length / 2) - 3;
					if (campos.Length == (int)TimeSheetFormat.Data_IniFim_IniFim_IniFim_Tipo_Controle_Descricao_Projeto_Sistema_Categoria_Atividade)
					{
						tipoControle = String.IsNullOrWhiteSpace(campos[7]) || String.IsNullOrWhiteSpace(campos[8]) ? "Sem definição" : campos[7];
						valorControle = campos[8];
						horarios = 3;
					}

					if (campos.Length >= (int)TimeSheetFormat.Data_IniFim_Descricao_Projeto_Sistema_Categoria_Atividade)
					{
						var data = campos[0].ConverterEmData();
						var descricao = campos[offSet + 3];
						var projeto = campos[offSet + 4];
						var sistema = campos[offSet + 5];
						var categoria = campos[offSet + 6];
						var tipoAtividade = campos[offSet + 7];
						if (!String.IsNullOrWhiteSpace(descricao))
						{
							for (var i = 0; i < horarios; i++)
							{
								var inicio = campos[i * 2 + 1];
								var termino = campos[i * 2 + 2];
								if ((termino.EmMinutos() - inicio.EmMinutos()) > 0)
								{
									var tarefa = timeSheet.Adicionar(projeto, sistema, categoria, tipoAtividade, data, inicio.PadLeft(5, '0'), termino.PadLeft(5, '0'), descricao, tipoControle, valorControle);
									Log(tarefa);
								}
							}
						}
					}
				}
			}

			return timeSheet;
		}

		private static TimeSheet LoadTxtFormato11(String[] linhas)
		{
			var timeSheet = new TimeSheet();
			foreach (var linha in linhas)
			{
				if (!String.IsNullOrWhiteSpace(linha))
				{
					var campos = linha.Split('\t');
					var offSet = campos.Length - 11;
					var horarios = (campos.Length / 2) - 4;

					if (campos.Length >= (int)TimeSheetFormat.Data_IniFim_Tipo_Controle_Soma_Descricao_Projeto_Sistema_Categoria_Atividade)
					{
						var data = campos[0].ConverterEmData();
						var tipoControle = String.IsNullOrWhiteSpace(campos[offSet + 3]) || String.IsNullOrWhiteSpace(campos[offSet + 4]) ? "Sem definição" : campos[offSet + 3];
						var valorControle = campos[offSet + 4];
						var descricao = campos[offSet + 6];
						var projeto = campos[offSet + 7];
						var sistema = campos[offSet + 8];
						var categoria = campos[offSet + 9];
						var tipoAtividade = campos[offSet + 10];
						if (!String.IsNullOrWhiteSpace(descricao))
						{
							for (var i = 0; i < horarios; i++)
							{
								var inicio = campos[i * 2 + 1];
								var termino = campos[i * 2 + 2];
								if ((termino.EmMinutos() - inicio.EmMinutos()) > 0)
								{
									var tarefa = timeSheet.Adicionar(projeto, sistema, categoria, tipoAtividade, data, inicio.PadLeft(5, '0'), termino.PadLeft(5, '0'), descricao, tipoControle, valorControle);
									Log(tarefa);
								}
							}
						}
					}
				}
			}

			return timeSheet;
		}

		private static TimeSheet LoadTxtFormatoCGS(String[] linhas)
		{
			var timeSheet = new TimeSheet();
			var competencia = "{0}/" + DateTime.Today.ToString("MMM/yyyy");
			foreach (var linha in linhas)
			{
				if (!String.IsNullOrWhiteSpace(linha))
				{
					var campos = linha.Split('\t');

					if (campos.Length == 4)
						competencia = "{0}/" + campos[1] + "/" + campos[3];
					else if (campos.Length >= 24)
					{
						var data = String.Format(competencia, campos[0]).ConverterEmData();
						var tipoControle = "Sem definição";
						var valorControle = "";
						var descricao = campos[23];
						var projeto = "1";
						var sistema = "1";
						var categoria = "1";
						var tipoAtividade = "1";
						if (!String.IsNullOrWhiteSpace(descricao))
						{
							for (int i = 3; i < 12; i += 3)
							{
								var inicio = campos[i];
								var termino = campos[i + 1];
								if ((termino.EmMinutos() - inicio.EmMinutos()) > 0)
								{
									var tarefa = timeSheet.Adicionar(projeto, sistema, categoria, tipoAtividade, data, inicio.PadLeft(5, '0'), termino.PadLeft(5, '0'), descricao, tipoControle, valorControle);
									Log(tarefa);
								}
							}
						}
					}
				}
			}

			return timeSheet;
		}

		private static void Log(Tarefa tarefa)
		{
			Mensagem.Instancia.WriteLine("{0} * {1} * {2} * {3}", tarefa.Projeto, tarefa.Sistema, tarefa.TipoAtividade, tarefa.TipoControle.PadRight(5).Substring(0, 5));
			Mensagem.Instancia.WriteLine("{0:dd/MM/yy} {1} {2} {3}", tarefa.Data, tarefa.Inicio, tarefa.Termino, tarefa.Descricao.PadRight(59).Substring(0, 59));
		}

		private static TimeSheetFormat IdentificarFormato(String[] linhas)
		{
			var linha = linhas.FirstOrDefault(l => !String.IsNullOrWhiteSpace(l));
			var campos = linha?.Split('\t');

			if ((campos?.Length ?? 0) == 0)
				throw new Exception("conteúdo Inválido");

			if (Enum.TryParse(campos[0], out TimeSheetFormat timeSheetFormat))
				return timeSheetFormat;

			else if (campos.Length < 4)
				throw new Exception("conteúdo Inválido");

			return (TimeSheetFormat)campos.Length;
		}
	}
}