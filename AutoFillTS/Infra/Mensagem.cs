using System;
using System.Windows.Forms;

namespace MPSC.AutoFillTS.Infra
{
	public interface IMensagem
	{
		void Write(String mensagem);
		String Msg { get; }
	}

	public abstract class Mensagem
	{
		public static IMensagem iMensagem { get; set; }
		public static Mensagem _instancia;
		public static Mensagem Instancia { get { return (_instancia ?? (_instancia = (iMensagem == null) ? new MensagemConsole() : new MensagemWindows() as Mensagem)); } }

		public abstract void Write(String format, params Object[] args);
		public abstract void WriteLine(String format, params Object[] args);
		public abstract Boolean Confirmar(String mensagem);

		private class MensagemConsole : Mensagem
		{
			public override void Write(String format, params Object[] args)
			{
				Console.Write(format, args);
			}

			public override void WriteLine(String format, params Object[] args)
			{
				Console.WriteLine(format, args);
			}

			public override Boolean Confirmar(String mensagem)
			{
				Write(mensagem + " _ (S / N)");
				var key = ConsoleKey.Backspace;
				while ((key != ConsoleKey.S) && (key != ConsoleKey.Y) && (key != ConsoleKey.N))
				{
					Console.CursorLeft = mensagem.Length;
					key = Console.ReadKey().Key;
				}
				return (key == ConsoleKey.S) || (key == ConsoleKey.Y);
			}
		}

		private class MensagemWindows : Mensagem
		{
			public override void Write(String format, params Object[] args)
			{
				iMensagem.Write(String.Format(format, args));
			}

			public override void WriteLine(String format, params Object[] args)
			{
				iMensagem.Write(String.Format(format, args) + "\r\n");
			}

			public override Boolean Confirmar(String mensagem)
			{
				return DialogResult.Yes == MessageBox.Show(iMensagem.Msg + mensagem, "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			}
		}
	}
}