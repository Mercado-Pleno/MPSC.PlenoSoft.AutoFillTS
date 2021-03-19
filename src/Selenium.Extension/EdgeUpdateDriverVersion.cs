namespace MPSC.PlenoSoft.Selenium.Extension
{
	public class EdgeUpdateDriverVersion : BrowserUpdateDriverVersion
	{
		protected override string XmlKeyDriverVersion => "edgedriver_win32";
		protected override string DriverFileName => "*edgedriver*.exe";
		protected override string BrowserFileName => "*edge*.exe";
		protected override string DefaultBrowserPath => @"Microsoft\Edge\Application\msedge.exe";
		protected override string DriverName => "MS Edge";
		protected override string RootPath => @"..\packages\WebDriver\";
		protected override string BaseURL => "https://msedgewebdriverstorage.blob.core.windows.net/edgewebdriver";
		protected override string XmlPath => "EnumerationResults/Blobs/Blob";
		protected override string XmlKey => "Name";
		protected override string GetBaseURL(string versao) => BaseURL + $"?comp=list&maxresults=500&prefix={versao}";

		public static string Update()
		{
			SeleniumFactory.BrowserType = BrowserType.Edge;
			return new EdgeUpdateDriverVersion().Start();
		}
	}
}