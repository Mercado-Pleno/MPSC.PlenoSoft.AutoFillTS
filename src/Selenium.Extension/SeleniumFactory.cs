using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.IO;
using System.Linq;

namespace MPSC.PlenoSoft.Selenium.Extension
{
	public enum BrowserType { Chrome, Edge, FireFox, }

	public static class SeleniumFactory
	{
		public static BrowserType BrowserType { get; set; }

		public static RemoteWebDriver BrowserWebDriver(int? portaTCP = null, FileInfo webDriverLocation = null, BrowserType? browserType = null)
		{
			switch (browserType ?? BrowserType)
			{
				case BrowserType.Chrome:
					return ChromeWebDriver(portaTCP, webDriverLocation);
				case BrowserType.Edge:
					return EdgeWebDriver(portaTCP, webDriverLocation);
				case BrowserType.FireFox:
					return FirefoxWebDriver(portaTCP, webDriverLocation);
				default:
					return ChromeWebDriver(portaTCP, webDriverLocation);
			}
		}

		/// <summary>
		/// http://learn-automation.com/install-selenium-webdriver-with-c/
		/// https://medium.com/@carol.ciola/selenium-webdriver-com-c-artigo-1-de-4-captura-de-screenshot-9f917a43cf6f
		/// </summary>
		public static RemoteWebDriver ChromeWebDriver(int? portaTCP = null, FileInfo webDriverLocation = null)
		{
			webDriverLocation = GetWebDriverLocation(webDriverLocation, "ChromeDriver*.exe");
			var driverService = ChromeDriverService.CreateDefaultService(webDriverLocation.Directory.FullName, webDriverLocation.Name);
			if (portaTCP.HasValue)
			{
				driverService.Port = portaTCP.Value;
				driverService.PortServerAddress = portaTCP.Value.ToString();
			}
			var driverOptions = new ChromeOptions() { AcceptInsecureCertificates = true, PageLoadStrategy = PageLoadStrategy.Normal };
			return new ChromeDriver(driverService, driverOptions, TimeSpan.FromSeconds(30));
		}

		public static RemoteWebDriver FirefoxWebDriver(int? portaTCP = null, FileInfo webDriverLocation = null)
		{
			webDriverLocation = GetWebDriverLocation(webDriverLocation, "geckodriver*.exe");
			var driverService = FirefoxDriverService.CreateDefaultService(webDriverLocation.Directory.FullName, webDriverLocation.Name);
			driverService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
			if (portaTCP.HasValue)
				driverService.Port = portaTCP.Value;
			var driverOptions = new FirefoxOptions() { AcceptInsecureCertificates = true, PageLoadStrategy = PageLoadStrategy.Normal };
			return new FirefoxDriver(driverService, driverOptions, TimeSpan.FromSeconds(30));
		}

		public static RemoteWebDriver EdgeWebDriver(int? portaTCP = null, FileInfo webDriverLocation = null)
		{
			webDriverLocation = GetWebDriverLocation(webDriverLocation, "*EdgeDriver*.exe");
			var driverService = EdgeDriverService.CreateDefaultService(webDriverLocation.Directory.FullName, webDriverLocation.Name);
			if (portaTCP.HasValue)
				driverService.Port = portaTCP.Value;
			var driverOptions = new EdgeOptions() { AcceptInsecureCertificates = true, PageLoadStrategy = PageLoadStrategy.Normal };
			return new EdgeDriver(driverService, driverOptions, TimeSpan.FromSeconds(30));
		}

		public static FileInfo GetWebDriverLocation(FileInfo webDriverLocation, string driverName)
		{
			try
			{
				var directory = new DirectoryInfo(webDriverLocation?.FullName ?? ".");
				while (!directory.Exists && (directory.Parent != null))
					directory = directory.Parent;

				if (directory.Exists)
				{
					webDriverLocation = directory.EnumerateFiles(driverName, SearchOption.AllDirectories).FirstOrDefault();
					while (((webDriverLocation == null) || !webDriverLocation.Exists) && (directory.Parent != null))
					{
						directory = directory.Parent;
						webDriverLocation = directory.EnumerateFiles(driverName, SearchOption.AllDirectories).FirstOrDefault();
					}
				}
			}
			catch (Exception) { }

			return webDriverLocation;
		}
	}
}