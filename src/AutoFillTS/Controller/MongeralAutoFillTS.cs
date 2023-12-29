using MPSC.PlenoSoft.AutoFillTS.Model;
using MPSTI.PlenoSoft.Core.Selenium.Drivers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace MPSC.PlenoSoft.AutoFillTS.Controller
{
    /// <summary>
    /// https://dev.azure.com/mongeral/Projetos/_git/VG/pullrequest/70911?_a=files&path=/Mongeral.eSim.VG.TestesAutomatizados/Faturamento.Contrato.Prestamista.VG/Helper/SeleniumHelper.cs
    /// </summary>
	public class MongeralAutoFillTS : AbstractAutoFillTS
	{
		protected override string UrlLogin { get { return "http://gestaodeatividades.mongeral.seguros/Lists/RegistrosSistemas/NewForm.aspx"; } }
		protected override IEnumerable<String> Urls { get { yield return UrlLogin; } }

		public MongeralAutoFillTS(Boolean processar, Boolean autoSaveClick) : base(processar, autoSaveClick) { }

		protected override void EsperarPeloLogin(SeleniumDriver seleniumDriver)
		{
			while (seleniumDriver.IsEmptyPageSource)
				seleniumDriver.Wait(wait);

			_ = seleniumDriver.WaitUntilContainsAllText(Token, false, "Tipo de Atividade");
		}

		protected override bool Fill(SeleniumDriver seleniumDriver, TimeSheet timeSheet)
		{
			var ok = true;

			foreach (var item in timeSheet.Tarefas)
				ok = ok && Fill(seleniumDriver, item);

			return ok;
		}

		private bool Fill(SeleniumDriver seleniumDriver, Tarefa item, int loop = 0)
		{
			try
			{
				Application.DoEvents();
				return PreencherPorTarefa(seleniumDriver, item);
			}
			catch when (loop < 5)
			{
				seleniumDriver.Wait(wait);
				return Fill(seleniumDriver, item, loop + 1);
			}
			catch (Exception)
			{
				return false;
			}
		}


		private void TryCore(DateTime limitTime, Action action)
		{
			try
			{
				action.Invoke();
			}
			catch when (DateTime.UtcNow < limitTime)
			{
				Thread.Sleep(wait);
				TryCore(limitTime, action);
			}
		}

		private void Try(TimeSpan timeOut, Action action) => TryCore(DateTime.UtcNow.Add(timeOut), action);

		private Boolean PreencherPorTarefa(SeleniumDriver seleniumDriver, Tarefa tarefa)
		{
			Try(tryTimeOut, () => seleniumDriver.Set("ddlCategoria", tarefa.Categoria));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlProjeto", tarefa.Projeto));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlSistema", tarefa.Sistema));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlTipoAtividade", tarefa.TipoAtividade));
			Try(tryTimeOut, () => seleniumDriver.Set("dtcDataDate", tarefa.Data.ToString("dd/MM/yyyy")));
			Try(tryTimeOut, () => seleniumDriver.Set("txtInicioAtividade", tarefa.Inicio));
			Try(tryTimeOut, () => seleniumDriver.Set("txtFimAtividade", tarefa.Termino));
			Try(tryTimeOut, () => seleniumDriver.Set("txtDescricao", tarefa.Descricao));
			Try(tryTimeOut, () => seleniumDriver.Set("ddlTipoControle", tarefa.TipoControle));
			Try(tryTimeOut, () => seleniumDriver.Set("txtTipoControleDetalhes", tarefa.ValorControle));
			Try(tryTimeOut, () => seleniumDriver.Set("btnSalvar", AutoSaveClick));

			while (seleniumDriver.Wait(wait).IsEquals("txtDescricao", tarefa.Descricao))
			{
				Application.DoEvents();
			}

			return true;
		}

		protected override void WaitFinish(SeleniumDriver seleniumDriver)
		{
			try
			{
				seleniumDriver.Set("btn_Fechar", true);
				while (seleniumDriver.ContainsAnyText(false, "Soma="))
					seleniumDriver.Wait(wait);
			}
			finally
			{
				base.WaitFinish(seleniumDriver);
			}
		}
	}
}

/*
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Xunit;

namespace Produto.Functions.V2.End2End.Helper
{
    public class SeleniumHelper
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait wait;

        public SeleniumHelper(IWebDriver driver)
        {
            _driver = driver;
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        #region Navegar
        public string ObterUrl() => _driver.Url;

        public void RecarregarPagina() => _driver.Navigate().Refresh();

        public void IrParaUrl(string url) => _driver.Navigate().GoToUrl(url);

        public void ValidarUrl(string url)
        {
            var urlAtual = ObterUrl();
            Assert.True(urlAtual.Contains(url, StringComparison.InvariantCultureIgnoreCase), $"Teste foi redirecionado para a página errada. Página atual: {urlAtual}, path esperada: {url}");
        }

        public string NomeAbaAtual() => _driver.CurrentWindowHandle;

        public void MudarAba(string nomeAba) => _driver.SwitchTo().Window(nomeAba);

        public void AbrirMudarNovaAba()
        {
            var javascriptExecutor = (IJavaScriptExecutor)_driver;
            javascriptExecutor.ExecuteScript("window.open();");
            var novaAba = _driver.WindowHandles.Last();
            MudarAba(novaAba);
        }

        public void FechaAbaAtual()
        {
            var javascriptExecutor = (IJavaScriptExecutor)_driver;
            javascriptExecutor.ExecuteScript("window.close();");
        }

        public void EntrarIframe(string xpath) => _driver.SwitchTo().Frame(ProcurarElemento(xpath));

        public void SairPaiIframe() => _driver.SwitchTo().ParentFrame();

        public void SairDefaultIframe() => _driver.SwitchTo().DefaultContent();
        #endregion

        #region Procurar
        public IWebElement ProcurarElemento(string xpath, int tempo = 5)
        {
            IWebElement elemento;
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                elemento = _driver.FindElement(By.XPath(xpath));
            }
            catch (WebDriverTimeoutException)
            {
                Print($"ProcurarElementoErro", "Elemento não encontrado");
                throw new WebDriverException($"Não foi encontrado o elemento: {By.XPath(xpath)}");
            }
            return elemento;
        }

        public ReadOnlyCollection<IWebElement> ProcurarElementos(string xpath, int tempo = 5)
        {
            ReadOnlyCollection<IWebElement> elementos;
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            try
            {
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(xpath)));
                elementos = _driver.FindElements(By.XPath(xpath));
            }
            catch (WebDriverTimeoutException)
            {
                Print($"ProcurarElementosErro", "Elementos não encontrado");
                throw new WebDriverException($"Não foram encontrados os elementos: {By.XPath(xpath)}");
            }
            return elementos;
        }

        public void ValidarSeElementoNaoExiste(string xpath, int tempo = 5)
        {
            IWebElement elemento;
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                elemento = _driver.FindElement(By.XPath(xpath));
                Print("ValidarSeElementoNaoExisteErro", "Elemento encontrado");
            }
            catch (WebDriverTimeoutException)
            {
                elemento = null;
            }
            Assert.Null(elemento);
        }

        public void ValidarSeElementosNaoExistem(string xpath, int tempo = 5)
        {
            ReadOnlyCollection<IWebElement> elementos;
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            try
            {
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(xpath)));
                elementos = _driver.FindElements(By.XPath(xpath));
                Print("ValidarSeElementosNaoExistemErro", "Elementos encontrados");
            }
            catch (WebDriverTimeoutException)
            {
                elementos = null;
            }
            Assert.Null(elementos);
        }
        #endregion

        #region Clicar
        public void Clicar(string xpath, int tempo = 5)
        {
            var elemento = ProcurarElemento(xpath, tempo);
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(elemento));
                    elemento.Click();
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    Print($"ClicarElementoErro", "Elemento não clicavel");
                    throw new WebDriverException($"Não foi possivel clicar no elemento: {elemento}");
                }
                catch (ElementClickInterceptedException ex)
                {
                    Print($"AcaoInterceptada", "Ação interceptada");
                    throw new ElementClickInterceptedException(ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    if (i == 5)
                    {
                        Print($"StaleElementReferenceExceptionErro", "Stale Element erro");
                        throw new StaleElementReferenceException($"{ex.Message}: {elemento}");
                    }
                }
            }
        }

        public void AceitarAlert()
        {
            IAlert alert = wait.Until(ExpectedConditions.AlertIsPresent());
            alert.Accept();
        }
        #endregion

        #region Teclado
        public void Escrever(string xpath, string texto, int tempo = 5)
        {
            var elemento = ProcurarElemento(xpath, tempo);
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(elemento));
                    elemento.SendKeys(texto);
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    Print($"EscreverElementoErro", "Elemento não escrevível");
                    throw new WebDriverException($"Não foi possivel escrever no elemento: {elemento}");
                }
                catch (ElementClickInterceptedException ex)
                {
                    Print($"AcaoInterceptada", "Ação interceptada");
                    throw new ElementClickInterceptedException(ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    if (i == 5)
                    {
                        Print($"StaleElementReferenceExceptionErro", "Stale Element erro");
                        throw new StaleElementReferenceException($"{ex.Message} :{elemento}");
                    }
                }
            }
        }

        public void LimparEscrever(string xpath, string texto, int tempo = 5)
        {
            var elemento = ProcurarElemento(xpath, tempo);
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(elemento));
                    elemento.Click();
                    elemento.SendKeys(Keys.Control + "a");
                    elemento.SendKeys("\b");
                    elemento.SendKeys(texto);
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    Print($"EscreverElementoErro", "Elemento não escrevível");
                    throw new WebDriverException($"Não foi possivel escrever no elemento: {elemento}");
                }
                catch (ElementClickInterceptedException ex)
                {
                    Print($"AcaoInterceptada", "Ação interceptada");
                    throw new ElementClickInterceptedException(ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    if (i == 5)
                    {
                        Print($"StaleElementReferenceExceptionErro", "Stale Element erro");
                        throw new StaleElementReferenceException($"{ex.Message} :{elemento}");
                    }
                }
            }
        }

        public void Limpar(string xpath, int tempo = 5)
        {
            var elemento = ProcurarElemento(xpath, tempo);
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            for (int i = 0; i <= 5; i++)
            {
                try
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(elemento));
                    elemento.Click();
                    elemento.SendKeys(Keys.Control + "a");
                    elemento.SendKeys("\b");
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    Print($"LimparElementoErro", "Elemento não clicavel");
                    throw new WebDriverException($"Não foi possivel limpar o que estava escrito no elemento: {elemento}");
                }
                catch (ElementClickInterceptedException ex)
                {
                    Print($"AcaoInterceptada", "Ação interceptada");
                    throw new ElementClickInterceptedException(ex.Message);
                }
                catch (StaleElementReferenceException ex)
                {
                    if (i == 5)
                    {
                        Print($"StaleElementReferenceExceptionErro", "Stale Element erro");
                        throw new StaleElementReferenceException($"{ex.Message} :{elemento}");
                    }
                }
            }
        }

        public void Enter(string xpath, int tempo = 5)
        {
            var elemento = ProcurarElemento(xpath, tempo);
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            elemento.SendKeys(Keys.Enter);
        }
        #endregion

        #region Loadings
        public void AguardarLoading(string xpath, int tempo = 30)
        {
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            try
            {
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath(xpath)));
            }
            catch (WebDriverTimeoutException)
            {
                Print($"AguardarLoadingErro", "Loading não desapareceu");
                throw new WebDriverException($"Loading ultrapassou o limite esperado");
            }
        }
        #endregion

        #region Extras
        public void Print(string nomePrint, string msgConsole)
        {
            string diretorioPrint = string.Format(@$"{Directory.GetCurrentDirectory()}/{nomePrint}_{DateTime.Now:dd-MM-yyyy_HH_mm_ss}.png");

            ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile(diretorioPrint);
            Console.WriteLine($"{msgConsole}: {new Uri(diretorioPrint)}");
        }

        public void AguardarTotalCarregamento(int tempo = 30)
        {
            wait.Timeout = TimeSpan.FromSeconds(tempo);
            wait.Until(driver => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
        }
        #endregion
    }
} 
*/