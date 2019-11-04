using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MPSC.PlenoSoft.AutoFillTS.Infra
{
	public static class Util
	{
		private static readonly List<String> mesesPT_BR = new List<String>(new String[] { "jan", "fev", "mar", "abr", "mai", "jun", "jul", "ago", "set", "out", "nov", "dez" });
		private static readonly List<String> mesesEN_US = new List<String>(new String[] { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "ouc", "nov", "dec" });
		private static readonly IFormatProvider pt_BR = CultureInfo.GetCultureInfo("pt-BR");
		private const String cFormatoHora = "hh':'mm";

		public static String ToHora(this TimeSpan timeSpan)
		{
			return timeSpan.ToString(cFormatoHora);
		}

		public static TimeSpan ToTimeSpan(this String hora)
		{
			return TimeSpan.ParseExact(hora, cFormatoHora, pt_BR);
		}

		public static Int32 DiasUteisNoMes(this DateTime data)
		{
			var retorno = 0;
			var mes = data.Month;
			while (mes == data.Month)
			{
				if ((data.DayOfWeek != DayOfWeek.Saturday) && (data.DayOfWeek != DayOfWeek.Sunday) && !data.Feriado())
					retorno++;
				data = data.AddDays(1);
			}

			return retorno;
		}

		public static Boolean Feriado(this DateTime data)
		{
			var Feriados = new List<DateTime>(new DateTime[]
			{
				new DateTime(data.Year, 01, 01),
				new DateTime(data.Year, 01, 20),
				new DateTime(data.Year, 04, 18),
				new DateTime(data.Year, 04, 21),
				new DateTime(data.Year, 04, 23),
				new DateTime(data.Year, 05, 01),
				new DateTime(data.Year, 06, 19),
				new DateTime(data.Year, 09, 07),
				new DateTime(data.Year, 10, 12),
				new DateTime(data.Year, 11, 02),
				new DateTime(data.Year, 11, 15),
				new DateTime(data.Year, 11, 20),
				new DateTime(data.Year, 12, 25)
			});
			return Feriados.Contains(data);
		}

		public static String EmHoras(this Int32 minutos)
		{
			return (minutos / 60).ToString().PadLeft(2, '0') + ":" + (minutos % 60).ToString().PadLeft(2, '0');
		}

		public static Int32 EmMinutos(this String hora)
		{
			if (String.IsNullOrWhiteSpace(hora))
				return 0;
			else
			{
				hora = hora.PadLeft(5, '0').Substring(0, 5);
				try
				{
					return Convert.ToInt32(hora.Substring(0, 2)) * 60 + Convert.ToInt32(hora.Substring(3, 2));
				}
				catch (Exception)
				{
					return Int32.MinValue;
				}
			}
		}

		public static Boolean IsHora(this String hora)
		{
			return hora.IsHoraEntre("00:00", "23:59");
		}

		public static Boolean IsHoraEntre(this String strHora, String inicio, String fim)
		{
			return strHora.Contains(":") && (strHora.Length >= 4) && (strHora.Length <= 5) && (strHora.EmMinutos() >= inicio.EmMinutos()) && (strHora.EmMinutos() <= fim.EmMinutos());
		}

		public static DateTime ConverterEmData(this String data)
		{
			try
			{
				var dma = data.Split('/', '-', '.');
				int ano = (dma.Length == 3) ? Convert.ToInt32(dma[2]) : DateTime.Today.Year;
				int mes = IsDigits(dma[1]) ? Convert.ToInt32(dma[1]) : ConverterEmNumero(dma[1]);
				int dia = Convert.ToInt32(dma[0]);
				if (ano < 100)
					ano = ano + ((DateTime.Today.Year / 100) * 100);
				return new DateTime(ano, mes, dia);
			}
			catch (Exception)
			{
				return DateTime.MinValue;
			}
		}

		public static Boolean IsDigits(this String str)
		{
			return !String.IsNullOrWhiteSpace(str) && str.All(Char.IsDigit);
		}

		public static Int32 ConverterEmNumero(this String mes)
		{
			mes = (mes.Length > 3 ? mes.Substring(0, 3) : mes).ToLower();
			var indice = mesesPT_BR.IndexOf(mes);
			if (indice < 0)
				indice = mesesEN_US.IndexOf(mes);
			return ((indice >= 0) ? indice + 1 : DateTime.Today.Month);
		}

		public static TipoColuna[] ObterFormatos(this String[] campos)
		{
			return ObterFormatoColunas(campos).ToArray();
		}

		private static IEnumerable<TipoColuna> ObterFormatoColunas(String[] campos)
		{
			foreach (String campo in campos)
				yield return ObterFormatoColuna(campo);
		}

		private static TipoColuna ObterFormatoColuna(String campo)
		{
			TipoColuna tipoColuna = TipoColuna.Indefinido;

			DateTime data = campo.ConverterEmData();
			if ((data >= DateTime.Today.AddDays(-32)) && (data <= DateTime.Today))
				tipoColuna = TipoColuna.Data;
			else if (campo.IsHora())
				tipoColuna = TipoColuna.Hora;

			return tipoColuna;
		}

		public enum TipoColuna
		{
			Indefinido,
			Data,
			Hora,
			Texto
		}
	}
}