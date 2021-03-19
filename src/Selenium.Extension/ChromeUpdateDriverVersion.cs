namespace MPSC.PlenoSoft.Selenium.Extension
{
	public class ChromeUpdateDriverVersion : BrowserUpdateDriverVersion
	{
		protected override string XmlKeyDriverVersion => "chromedriver_win32";
		protected override string DriverFileName => "ChromeDriver*.exe";
		protected override string BrowserFileName => "Chrome*.exe";
		protected override string DefaultBrowserPath => @"Google\Chrome\Application\chrome.exe";
		protected override string DriverName => "Google Chrome";
		protected override string RootPath => @"..\packages\WebDriver\";
		protected override string BaseURL => "https://chromedriver.storage.googleapis.com";
		protected override string XmlPath => "/a:ListBucketResult/a:Contents";
		protected override string XmlKey => "Key";
		protected override string GetBaseURL(string versao) => BaseURL;

		public static string Update()
		{
			SeleniumFactory.BrowserType = BrowserType.Chrome;
			return new ChromeUpdateDriverVersion().Start();
		}
	}
}