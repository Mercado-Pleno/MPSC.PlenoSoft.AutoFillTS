using MPSC.AutoFillTS.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPSC.AutoFillTS.Controller
{
	public class AutoFillTSController
	{
		public static Boolean Processar(IEnumerable<AbstractAutoFillTS> listaAutoFillTS, TimeSheet timeSheet)
		{
			return listaAutoFillTS.All(af => af.Processar(timeSheet));
		}
	}
}