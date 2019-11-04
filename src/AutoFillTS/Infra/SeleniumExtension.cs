using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading;

namespace MPSC.PlenoSoft.AutoFillTS.Infra
{
	public static class Factory
	{
		public static IWebDriver ChromeDriver(String driverPath, String driverExecutableFileName)
		{
			var chromeDriverService = ChromeDriverService.CreateDefaultService(driverPath, driverExecutableFileName ?? "chromedriver.exe");
			chromeDriverService.Port = 54321;
			chromeDriverService.PortServerAddress = "54321";
			chromeDriverService.HideCommandPromptWindow = true;
			var chromeOptions = new ChromeOptions { AcceptInsecureCertificates = true, PageLoadStrategy = PageLoadStrategy.Eager, UnhandledPromptBehavior = UnhandledPromptBehavior.Ignore };
			return new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromSeconds(10));
		}
	}


	public static class SeleniumExtension
	{
		public static Int32 WaitLoopSleep = 250;

		public static void IrParaEndereco(this IWebDriver webDriver, String endereco, int waitInSeconds = 0)
		{
			webDriver.Navigate().GoToUrl(new Uri(endereco, UriKind.Absolute));
		}

		public static void RunScript(this IWebDriver webDriver, String script, int waitInSeconds = 0)
		{
		}

		public static Boolean WaitUntilContainsAllText(this IWebDriver webDriver, Boolean caseSensitive, params String[] texts)
		{
			return WaitUntilContainsAllText(webDriver.GetBody(), caseSensitive, texts);
		}

		public static Boolean WaitWhileContainsAllText(this IWebDriver webDriver, Boolean caseSensitive, params String[] texts)
		{
			return WaitWhileContainsAllText(webDriver.GetBody(), caseSensitive, texts);
		}

		public static Boolean WaitUntilContainsAllText(this IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			while (!webElement.ContainsAllText(caseSensitive, texts))
				Thread.Sleep(WaitLoopSleep);

			return webElement.ContainsAllText(caseSensitive, texts);
		}

		public static Boolean WaitWhileContainsAllText(this IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			while (webElement.ContainsAllText(caseSensitive, texts))
				Thread.Sleep(WaitLoopSleep);

			return !webElement.ContainsAllText(caseSensitive, texts);
		}

		public static Boolean ContainsAllText(this IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			var webElementText = caseSensitive ? webElement.Text : webElement.Text.ToUpper();
			return (texts.Length > 0) && texts.Select(t => caseSensitive ? t : t.ToUpper()).All(t => webElementText.Contains(t));
		}

		public static Boolean ContainsAnyText(this IWebElement webElement, Boolean caseSensitive, params String[] texts)
		{
			var webElementText = caseSensitive ? webElement.Text : webElement.Text.ToUpper();
			return (texts.Length == 0) || texts.Select(t => caseSensitive ? t : t.ToUpper()).Any(t => webElementText.Contains(t));
		}

		public static Boolean Focus(this IWebElement webElement) { return false; }

		public static Boolean ContainsAllText(this IWebDriver webDriver, Boolean caseSensitive, params String[] texts)
		{
			return ContainsAllText(webDriver.GetBody(), caseSensitive, texts);
		}

		public static Boolean ContainsAnyText(this IWebDriver webDriver, Boolean caseSensitive, params String[] texts)
		{
			return ContainsAnyText(webDriver.GetBody(), caseSensitive, texts);
		}


		public static IWebElement Select(this IWebElement webElement, Boolean click, Boolean force)
		{
			if (click)
				webElement.Click();
			return webElement;
		}

		public static IWebElement Select(this IWebElement webElement, String text, Boolean force)
		{
			webElement.Clear();
			var formatedText = text.Replace("\r\n", "\n").Replace("\r", "\n")
				.Replace("\n", Keys.Shift + Keys.Enter + Keys.Shift);

			return webElement.TypeKeys(formatedText);
		}

		public static IWebElement Enter(this IWebElement webElement, Int32 dalayOfEnter)
		{
			Thread.Sleep(dalayOfEnter);
			webElement.TypeKeys(Keys.Enter);
			return webElement;
		}

		public static IWebElement Escape(this IWebElement webElement)
		{
			webElement.TypeKeys(Keys.Escape);
			Thread.Sleep(WaitLoopSleep);
			webElement.TypeKeys(Keys.Escape);
			Thread.Sleep(WaitLoopSleep);
			return webElement;
		}

		public static IWebElement TypeKeys(this IWebElement webElement, String text)
		{
			webElement.SendKeys(text);
			return webElement;
		}

		public static IWebElement GetBody(this IWebDriver webDriver)
		{
			return webDriver.GetElement("body", 0);
		}

		public static IWebElement GetElement(this ISearchContext searchContext, String tagName, Int32 skip = 0)
		{
			return searchContext.FindElements(By.TagName(tagName)).Skip(skip).FirstOrDefault();
		}

		public static IWebElement FindByIdOrName(this ISearchContext webDriver, String idOrName)
		{
			return webDriver.FindElement(By.Id(idOrName))
				?? webDriver.FindElement(By.Name(idOrName));
		}
	}
}