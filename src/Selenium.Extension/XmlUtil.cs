using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace MPSC.PlenoSoft.Selenium.Extension
{
	public class XmlUtil
	{
		private readonly XmlDocument _xmlDocument;
		private readonly XmlNamespaceManager _xmlnsManager;

		public XmlUtil(string xml)
		{
			_xmlDocument = GetXmlDocument(xml);
			_xmlnsManager = GetXmlNamespaceManager(_xmlDocument);
		}

		public XmlNode Node(string xPath)
		{
			return _xmlDocument.SelectSingleNode(xPath, _xmlnsManager);
		}

		public XmlNode[] Nodes(string xPath)
		{
			return _xmlDocument.SelectNodes(xPath, _xmlnsManager).OfType<XmlNode>().ToArray();
		}

		public XmlNode[] Nodes(string xPath, string childName)
		{
			return Nodes(xPath).Select(n => GetChild(n, childName)).ToArray();
		}

		public static XmlDocument GetXmlDocument(string xml)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument;
		}

		public static XmlNamespaceManager GetXmlNamespaceManager(XmlDocument xmlDocument)
		{
			var nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
			nsmgr.AddNamespace("a", xmlDocument.DocumentElement?.Attributes["xmlns"]?.Value ?? ".");
			return nsmgr;
		}

		public static XmlNode GetChild(XmlNode node, string name)
		{
			return node.ChildNodes.OfType<XmlNode>().FirstOrDefault(x => x.Name == name);
		}

		public static XmlUtil CreateFromUrl(string url)
		{
			var xml = GetFromUrl(url);
			return new XmlUtil(xml);
		}

		public static string GetFromUrl(string url)
		{
			var response = GetResponse(url).Result;
			return response.Content.ReadAsStringAsync().Result;
		}

		public static Stream DownloadFromUrl(string url)
		{
			var response = GetResponse(url).Result;
			return response.Content.ReadAsStreamAsync().Result;
		}

		public static Task<HttpResponseMessage> GetResponse(string url)
		{
			var httpClient = new HttpClient();
			return httpClient.GetAsync(url);
		}

		public static FileInfo DownloadFromUrl(string url, FileInfo fileInfo)
		{
			if (fileInfo.Exists)
				fileInfo.Delete();
			else if (!fileInfo.Directory.Exists)
				fileInfo.Directory.Create();

			using (var zip = DownloadFromUrl(url))
			{
				using (var fileStream = File.Create(fileInfo.FullName))
				{
					var len = 4096;
					var buffer = new byte[len];
					var read = zip.Read(buffer, 0, len);
					while (read > 0)
					{
						fileStream.Write(buffer, 0, read);
						read = zip.Read(buffer, 0, len);
					}
					fileStream.Flush();
					fileStream.Close();
				}
			}
			return new FileInfo(fileInfo.FullName);
		}
	}
}