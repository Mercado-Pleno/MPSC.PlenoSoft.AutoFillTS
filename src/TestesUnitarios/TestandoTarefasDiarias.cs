using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPSC.PlenoSoft.AutoFillTS.Controller;
using MPSC.PlenoSoft.AutoFillTS.Infra;
using MPSC.PlenoSoft.AutoFillTS.Model;
using System;
using System.Linq;

namespace MPSC.PlenoSoft.AutoFillTS.TestesUnitarios
{
	[TestClass]
	public class TestandoTarefasDiarias
	{
        private TimeSheet CriarCenario(DateTime dia)
        {
            var timeSheet = new TimeSheet();

            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia, "08:00", "12:00", "Desc", "1", "Ctrl");
            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia, "13:00", "17:00", "Desc", "1", "Ctrl");
            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia, "19:00", "21:00", "Desc", "1", "Ctrl");

            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia.AddDays(1), "08:00", "10:30", "Desc1", "1", "Ctrl");
            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia.AddDays(1), "12:00", "14:00", "Desc1", "1", "Ctrl");
            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia.AddDays(1), "16:00", "18:00", "Desc2", "1", "Ctrl");
            timeSheet.Adicionar("Prj1", "Sis", "Cat", "Atv", dia.AddDays(1), "20:00", "22:00", "Desc2", "1", "Ctrl");

            return timeSheet;
        }

		[TestMethod]
		public void QuandoATimeSheetEhCompostaDeVariosHorariosDirarios()
		{
            var timeSheet = CriarCenario(new DateTime(2016, 02, 01));

			var tarefasDiarias = timeSheet.TarefasDiarias;

			Assert.IsNotNull(tarefasDiarias);
			Assert.IsTrue(tarefasDiarias.Any());
			Assert.AreEqual(2, tarefasDiarias.Count());

			var tarefaDiaria1 = tarefasDiarias.First();
			Assert.AreEqual("08:00", tarefaDiaria1.Inicio.ToHora());
			Assert.AreEqual("21:00", tarefaDiaria1.Termino.ToHora());
			Assert.AreEqual("10:00", tarefaDiaria1.TotalTrabalhado.ToHora());
			Assert.AreEqual("13:00", tarefaDiaria1.TempoComIntervalos.ToHora());
			Assert.AreEqual("03:00", tarefaDiaria1.Intervalo.ToHora());
			Assert.AreEqual("Desc", tarefaDiaria1.Descricao);

			var tarefaDiaria2 = tarefasDiarias.Last();
			Assert.AreEqual("08:00", tarefaDiaria2.Inicio.ToHora());
			Assert.AreEqual("22:00", tarefaDiaria2.Termino.ToHora());
			Assert.AreEqual("08:30", tarefaDiaria2.TotalTrabalhado.ToHora());
			Assert.AreEqual("14:00", tarefaDiaria2.TempoComIntervalos.ToHora());
			Assert.AreEqual("05:30", tarefaDiaria2.Intervalo.ToHora());
			Assert.AreEqual("Desc1 + Desc2", tarefaDiaria2.Descricao);
		}

        //[TestMethod]
        public void QuandoPedeParaPreencher()
        {
            var timeSheet = CriarCenario(DateTime.Today);
            var autoFill = new ProviderItAutoFillTS(true, false);
            autoFill.Processar(timeSheet);
        }
	}
}