using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace MPSC.PlenoSoft.Selenium.Extension
{
	public class ChromeUpdateDriverVersion
	{
		private const string cEmptyVersion = "0000.0000.0000.0000";
		private const string rootPath = @"..\packages\WebDriver\";
		private const string baseURL = "https://chromedriver.storage.googleapis.com";

		private ChromeUpdateDriverVersion() { }

		private string Start()
		{
			var driverFile = SeleniumFactory.GetWebDriverLocation(null, "ChromeDriver*.exe");
			var browserFile = GetBrowserFile("Program Files (x86)");
			if (browserFile?.Exists ?? false)
			{
				var browserVersion = GetBrowserVersion(browserFile);
				var driverVersion = GetDriverVersion(driverFile);
				if (NeedUpdate(browserVersion, driverVersion))
				{
					var versions = SearchDriverVersions(browserVersion.Split('.'));
					var versaoEscolhida = ChooseBetterDriverVersion(versions);
					var arquivoZip = DownloadDriver(versaoEscolhida, new DirectoryInfo(rootPath));
					var arquivoExe = Unzip(arquivoZip);
					var webDriverLocation = SeleniumFactory.GetWebDriverLocation(arquivoExe, "ChromeDriver*.exe");
					if ((driverFile != null) && (driverFile.Directory.FullName != webDriverLocation.Directory.FullName))
						webDriverLocation.CopyTo(driverFile.FullName, true);

					return Start();
				}
				else
					return $"Atualizado! \r\n chromeBrowserVersion: {browserVersion} \r\n chromeDriverVersion: {driverVersion} ";
			}

			return "Baixe e instale o Google Chrome!";
		}

		private bool NeedUpdate(string browserVersion, string driverVersion)
		{
			return !equals(browserVersion.Split('.'), driverVersion.Split('.'), 0, 1, 2);
		}

		private bool equals(string[] bv, string[] dv, params int[] indexes) => indexes.All(i => bv[i] == dv[i]);

		private FileInfo Unzip(FileInfo arquivoZip)
		{
			var files = arquivoZip.Directory.GetFiles().Where(f => f.FullName != arquivoZip.FullName);
			foreach (var file in files) file.Delete();

			ZipFile.ExtractToDirectory(arquivoZip.FullName, arquivoZip.Directory.FullName);

			return new FileInfo(arquivoZip.Directory.FullName);
		}

		private FileInfo DownloadDriver(string arquivoEscolhido, DirectoryInfo directoryInfo)
		{
			var arquivoZip = new FileInfo(Path.Combine(directoryInfo.FullName, arquivoEscolhido));
			return XmlUtil.DownloadFromUrl($"{baseURL}/{arquivoEscolhido}", arquivoZip);
		}

		private string ChooseBetterDriverVersion(IEnumerable<string> versions)
		{
			int[] Converter(string path)
			{
				var paths = path.Split('/');
				var versao = paths[0];
				var versoes = versao.Split('.');
				var intVers = versoes.Select(i => Convert.ToInt32(i));
				return intVers.ToArray();
			}

			var version = versions.Select(v => Converter(v))
				.OrderBy(v => v[0]).ThenBy(v => v[1]).ThenBy(v => v[2]).ThenBy(v => v[3])
				.Select(v => string.Join(".", v)).LastOrDefault();

			return versions.FirstOrDefault(v => v.StartsWith(version));
		}

		private string[] SearchDriverVersions(IEnumerable<string> versionArray)
		{
			try
			{
				var xmlUtil = XmlUtil.CreateFromUrl(baseURL);
				var keys = xmlUtil.Nodes("/a:ListBucketResult/a:Contents", "Key");
				var files = keys.Where(k => k.InnerXml.Contains("chromedriver_win32")).ToArray();

				var versoes = new string[0];
				var take = versionArray.Count();
				while ((versoes.Length == 0) && (take > 0))
				{
					var versao = string.Join(".", versionArray.Take(take));
					versoes = files.Where(x => x.InnerXml.StartsWith(versao)).Select(n => n.InnerXml).ToArray();
					take--;
				}

				return versoes;
			}
			catch (Exception)
			{
				return new[] { cEmptyVersion };
			}
		}

		private string GetBrowserVersion(FileInfo browserFile)
		{
			return FileVersionInfo.GetVersionInfo(browserFile.FullName).FileVersion;
		}

		private string GetDriverVersion(FileInfo driverFile)
		{
			if ((driverFile == null) || (!driverFile.Exists))
				return cEmptyVersion;

			var startInfo = new ProcessStartInfo(driverFile.FullName, "--Version") { RedirectStandardOutput = true, CreateNoWindow = true, UseShellExecute = false };
			var process = new Process() { StartInfo = startInfo };
			process.Start();
			var driverVersion = process.StandardOutput.ReadToEnd() + " ";
			process.Close();
			return driverVersion.Split(' ')[1];
		}

		private FileInfo GetBrowserFile(string programFiles)
		{
			var fileInfo = new FileInfo($@"C:\{programFiles}\Google\Chrome\Application\chrome.exe");
			var location = SeleniumFactory.GetWebDriverLocation(fileInfo, "chrome*.exe");
			return location.Exists ? location : null;
		}

		public static string Update() => new ChromeUpdateDriverVersion().Start();
	}
}