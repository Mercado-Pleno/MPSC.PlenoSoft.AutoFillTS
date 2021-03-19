using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace MPSC.PlenoSoft.Selenium.Extension
{
	public abstract class BrowserUpdateDriverVersion
	{
		protected abstract string XmlKeyDriverVersion { get; }
		protected abstract string DriverFileName { get; }
		protected abstract string BrowserFileName { get; }
		protected abstract string DefaultBrowserPath { get; }
		protected abstract string DriverName { get; }
		protected string EmptyVersion => "0000.0000.0000.0000";
		protected abstract string RootPath { get; }
		protected abstract string BaseURL { get; }
		protected abstract string XmlPath { get; }
		protected abstract string XmlKey { get; }
		protected abstract string GetBaseURL(string versao);

		protected BrowserUpdateDriverVersion() { }

		protected string Start()
		{
			var driverFile = SeleniumFactory.GetWebDriverLocation(null, DriverFileName);
			var browserFile = GetBrowserFile("Program Files (x86)");
			if (browserFile?.Exists ?? false)
			{
				var browserVersion = GetBrowserVersion(browserFile);
				var driverVersion = GetDriverVersion(driverFile);
				if (NeedUpdate(browserVersion, driverVersion))
				{
					var versions = SearchDriverVersions(browserVersion.Split('.'));
					var versaoEscolhida = ChooseBetterDriverVersion(versions);
					var arquivoZip = DownloadDriver(versaoEscolhida, new DirectoryInfo(RootPath));
					var arquivoExe = Unzip(arquivoZip);
					var webDriverLocation = SeleniumFactory.GetWebDriverLocation(arquivoExe, DriverFileName);
					if ((driverFile != null) && (driverFile.Directory.FullName != webDriverLocation.Directory.FullName))
						webDriverLocation.CopyTo(driverFile.FullName, true);

					return Start();
				}
				else
					return $"Atualizado! \r\n {DriverName}BrowserVersion: {browserVersion} \r\n {DriverName}DriverVersion: {driverVersion} ";
			}

			return $"Baixe e instale o {DriverName}!";
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

			var dirs = arquivoZip.Directory.GetDirectories();
			foreach (var dir in dirs) dir.Delete(true);

			ZipFile.ExtractToDirectory(arquivoZip.FullName, arquivoZip.Directory.FullName);

			return new FileInfo(arquivoZip.Directory.FullName);
		}

		private FileInfo DownloadDriver(string arquivoEscolhido, DirectoryInfo directoryInfo)
		{
			var arquivoZip = new FileInfo(Path.Combine(directoryInfo.FullName, arquivoEscolhido));
			return XmlUtil.DownloadFromUrl($"{BaseURL}/{arquivoEscolhido}", arquivoZip);
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
				var take = versionArray.Count();
				var versao = string.Join(".", versionArray.Take(take));
				var xmlUtil = XmlUtil.CreateFromUrl(GetBaseURL(versao));
				var keys = xmlUtil.Nodes(XmlPath, XmlKey);
				var files = keys.Where(k => k.InnerXml.Contains(XmlKeyDriverVersion)).ToArray();

				var versoes = new string[0];
				while ((versoes.Length == 0) && (take > 0))
				{
					versoes = files.Where(x => x.InnerXml.StartsWith(versao)).Select(n => n.InnerXml).ToArray();
					take--;
				}

				return versoes;
			}
			catch (Exception)
			{
				return new[] { EmptyVersion };
			}
		}

		private string GetBrowserVersion(FileInfo browserFile)
		{
			return FileVersionInfo.GetVersionInfo(browserFile.FullName).FileVersion;
		}

		private string GetDriverVersion(FileInfo driverFile)
		{
			if ((driverFile == null) || (!driverFile.Exists))
				return EmptyVersion;

			var startInfo = new ProcessStartInfo(driverFile.FullName, "--Version") { RedirectStandardOutput = true, CreateNoWindow = true, UseShellExecute = false };
			var process = new Process() { StartInfo = startInfo };
			process.Start();
			var driverVersion = process.StandardOutput.ReadToEnd() + " ";
			process.Close();
			return driverVersion.Split(' ')[1];
		}

		private FileInfo GetBrowserFile(string programFiles)
		{
			var fileInfo = new FileInfo(Path.Combine(@"C:\", programFiles, DefaultBrowserPath));
			var location = SeleniumFactory.GetWebDriverLocation(fileInfo, BrowserFileName);
			return (location?.Exists).GetValueOrDefault() ? location : null;
		}
	}
}