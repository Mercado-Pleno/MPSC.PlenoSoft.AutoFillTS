using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace MPSC.PlenoSoft.Selenium.Extension
{
	public class SeleniumRWD
	{
		private readonly RemoteWebDriver _remoteWebDriver;

		public SeleniumRWD(RemoteWebDriver remoteWebDriver)
		{
			_remoteWebDriver = remoteWebDriver;
		}

		public bool IsEmptyPageSource => _remoteWebDriver.PageSource == "<html><head></head><body></body></html>";

		public IWebElement GetByIdOrName(string idOrName, int skip = 0) => GetElementByIdOrName(_remoteWebDriver, idOrName, skip);


		public IWebElement Set(string idOrName, Boolean click)
		{
			var webElement = GetByIdOrName(idOrName);
			if (click)
				webElement?.Click();
			else
				webElement?.Focus();

			return webElement;
		}

		public IWebElement Set(string idOrName, string text, int sleep = 50)
		{
			var webElement = GetByIdOrName(idOrName);
			if (webElement.TagName == "select")
			{
				SetSelect(idOrName, text, sleep);
			}
			else
			{
				SetInput(idOrName, text, sleep);
			}

			return webElement;
		}

		private void SetInput(string idOrName, string text, int sleep)
		{
			var formatedText = text.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Keys.Shift + Keys.Enter + Keys.Shift);
			var webElement = GetByIdOrName(idOrName);
			webElement.Clear();
			webElement.TypeKeys(formatedText);
		}

		private void SetSelect(string idOrName, string text, int sleep)
		{
			SelectElement GetSelectByIdOrName() => new SelectElement(GetByIdOrName(idOrName));

			if (text.Any())
			{
				GetSelectByIdOrName()?.SelectByValue(text);
				Thread.Sleep(sleep);

				if (string.IsNullOrWhiteSpace(GetSelectByIdOrName()?.SelectedOption?.Text))
				{
					GetSelectByIdOrName()?.SelectByText(text);
					Thread.Sleep(sleep);
				}

				if (string.IsNullOrWhiteSpace(GetSelectByIdOrName()?.SelectedOption?.Text))
				{
					GetSelectByIdOrName()?.SelectByText(text, true);
					Thread.Sleep(sleep);
				}

				if (text.All(char.IsDigit) && string.IsNullOrWhiteSpace(GetSelectByIdOrName()?.SelectedOption?.Text))
				{
					GetSelectByIdOrName()?.SelectByIndex(Convert.ToInt32(text));
					Thread.Sleep(sleep);
				}
			}
		}



		private IWebElement GetElementByIdOrName(ISearchContext searchContext, string idOrName, int skip = 0)
		{
			return searchContext.FindElements(By.Id(idOrName)).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.Name(idOrName)).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[id$='{idOrName}' i]")).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[id^='{idOrName}' i]")).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[id*='{idOrName}' i]")).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[name$='{idOrName}' i]")).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[name^='{idOrName}' i]")).Skip(skip).FirstOrDefault()
				?? searchContext.FindElements(By.CssSelector($"[name*='{idOrName}' i]")).Skip(skip).FirstOrDefault()
			;
		}

























		public Int32 WaitLoopSleep = 250;


		public Boolean WaitUntilContainsAllText(CancellationToken cancellationToken, Boolean caseSensitive, params String[] texts)
		{
			return _remoteWebDriver.GetBody().WaitUntilContainsAllText(cancellationToken, caseSensitive, texts);
		}

		public Boolean WaitWhileContainsAllText(CancellationToken cancellationToken, Boolean caseSensitive, params String[] texts)
		{
			return _remoteWebDriver.GetBody().WaitWhileContainsAllText(cancellationToken, caseSensitive, texts);
		}

		public Boolean ContainsAllText(Boolean caseSensitive, params String[] texts)
		{
			return _remoteWebDriver.GetBody().ContainsAllText(caseSensitive, texts);
		}

		public Boolean ContainsAnyText(Boolean caseSensitive, params String[] texts)
		{
			return _remoteWebDriver.GetBody().ContainsAnyText(caseSensitive, texts);
		}

		public Boolean WaitUntilContainsAllText(IWebElement webElement, CancellationToken cancellationToken, Boolean caseSensitive, params String[] texts)
		{
			while (!cancellationToken.IsCancellationRequested && !webElement.ContainsAllText(caseSensitive, texts))
				Thread.Sleep(WaitLoopSleep);

			return webElement.ContainsAllText(caseSensitive, texts);
		}

		public Boolean WaitWhileContainsAllText(IWebElement webElement, CancellationToken cancellationToken, Boolean caseSensitive, params String[] texts)
		{
			while (!cancellationToken.IsCancellationRequested && webElement.ContainsAllText(caseSensitive, texts))
				Thread.Sleep(WaitLoopSleep);

			return !webElement.ContainsAllText(caseSensitive, texts);
		}


		public Boolean ContainsAllText(IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			var webElementText = caseSensitive ? webElement.Text : webElement.Text.ToUpper();
			return (texts.Length > 0) && texts.Select(t => caseSensitive ? t : t.ToUpper()).All(t => webElementText.Contains(t));
		}

		public Boolean ContainsAnyText(IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			var webElementText = caseSensitive ? webElement.Text : webElement.Text.ToUpper();
			return (texts.Length == 0) || texts.Select(t => caseSensitive ? t : t.ToUpper()).Any(t => webElementText.Contains(t));
		}

		public Boolean EstaPreenchido(string idOrName, string value)
		{
			var element = _remoteWebDriver.GetElementByIdOrName(idOrName);
			return element?.GetAttribute("value") == value;
		}





		public IWebElement Enter(IWebElement webElement, Int32 dalayOfEnter)
		{
			Thread.Sleep(dalayOfEnter);
			webElement.TypeKeys(Keys.Enter);
			return webElement;
		}

		public IWebElement Escape(IWebElement webElement)
		{
			webElement.TypeKeys(Keys.Escape);
			Thread.Sleep(WaitLoopSleep);
			webElement.TypeKeys(Keys.Escape);
			Thread.Sleep(WaitLoopSleep);
			return webElement;
		}

		public IWebElement TypeKeys(IWebElement webElement, String text)
		{
			webElement.SendKeys(text);
			return webElement;
		}

		public IWebElement GetBody(IWebDriver webDriver)
		{
			return webDriver.GetElement("body", 0);
		}

		public IWebElement GetElement(ISearchContext searchContext, String tagName, Int32 skip = 0)
		{
			return searchContext.FindElements(By.TagName(tagName)).Skip(skip).FirstOrDefault();
		}

		public IWebElement GetButton(IFindsByCssSelector searchContext, string text)
		{
			var lowerText = text?.Trim()?.ToLower();
			return searchContext.FindElementsByCssSelector("button")?.FirstOrDefault(b => b.Text.ToLower().Contains(lowerText));
		}

		public void PrintScreen(IWebDriver webDriver, FileInfo fileInfo)
		{
			var camera = webDriver as ITakesScreenshot;
			var foto = camera.GetScreenshot();
			foto.SaveAsFile(fileInfo.FullName, ScreenshotImageFormat.Png);
		}

		public void Encerrar()
		{
			try
			{
				_remoteWebDriver.Close();
				_remoteWebDriver.Quit();
				_remoteWebDriver.Dispose();
			}
			catch (Exception) { }
		}

		public object Scroll(IWebDriver webDriver, int x, int y)
		{
			return webDriver.ExecuteScript($"window.scrollBy({x}, {y});");
		}

		public void IrParaEndereco(string address, int timeout = 0)
		{
			_remoteWebDriver.Navigate().GoToUrl(address);
		}

		public void Focus(IWebElement webElement)
		{
			var element = webElement as RemoteWebElement;
			var id = element?.GetAttribute("id");
			element?.WrappedDriver?.ExecuteScript($"document.getElementById('{id}').focus();");
		}

		public bool Exists(IWebElement webElement)
		{
			return webElement != null;
		}

		/// <summary>
		/// js.ExecuteScript("alert('Hello World!');")
		/// js.ExecuteScript("arguments[0].onmouseover()", webDriver.FindElement(By.LinkText("NomeDoMenu")));
		/// js.ExecuteScript("window.scrollBy(0, 300)", "");
		/// </summary>
		/// <param name="webDriver"></param>
		/// <param name="script"></param>
		/// <returns></returns>
		public object ExecuteScript(IWebDriver webDriver, string script)
		{
			var js = webDriver as IJavaScriptExecutor;
			return js?.ExecuteScript(script);
		}

		public void Posicionar(IWebDriver webDriver, int windowIndex, int windowCount, int offSet = 0)
		{
			webDriver.Manage().Window.Posicionar(windowIndex, windowCount, offSet);
		}

		public void Posicionar(IWindow window, int windowIndex, int windowCount, int offSet = 0)
		{
			window.Position = new Point(offSet, 0);
			window.Maximize();
			if (windowCount > 1)
			{
				var position = new Point(new Size(window.Position));
				var size = new Size(new Point(window.Size));

				window.Size = new Size(size.Width / windowCount + 8, size.Height);
				var maxWidth = (size.Width - window.Size.Width) / (windowCount - 1);

				window.Position = new Point((windowIndex * maxWidth) + position.X, position.Y);
			}
		}
	}
}