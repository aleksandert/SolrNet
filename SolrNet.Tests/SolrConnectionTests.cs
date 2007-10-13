using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrConnectionTests {
		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void InvalidUrl_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("http:/locl", reqFactory);
		}

		[Test]
		[ExpectedException(typeof (InvalidURLException))]
		public void UrlNotHttp_ShouldThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("ftp://pepe", reqFactory);
		}

		[Test]
		public void UrlHttp_ShouldntThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("http://pepe", reqFactory);
		}

		[Test]
		public void UrlHttps_ShouldntThrowException() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
		}

		[Test]
		public void get() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			Expect.Call(reqFactory.Create(new UriBuilder().Uri)).IgnoreArguments().Repeat.Once().Return(request);
			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
			Expect.Call(response.GetResponseStream()).Repeat.Once().Return(new MemoryStream());
			mocks.ReplayAll();
			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
			conn.Get("", new Dictionary<string, string>());
		}

		[Test]
		public void Post() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();
			Expect.Call(request.GetRequestStream()).Repeat.Once().Return(new MemoryStream());
			Expect.Call(reqFactory.Create("")).IgnoreArguments().Repeat.Once().Return(request);
			request.Method = HttpWebRequestMethod.POST;
			LastCall.Repeat.Once();
			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
			Expect.Call(response.GetResponseStream()).Repeat.Once().Return(new MemoryStream());
			mocks.ReplayAll();

			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
			conn.Post("", "");
			mocks.VerifyAll();
		}

		[Test]
		public void PostXML() {
			MockRepository mocks = new MockRepository();
			IHttpWebRequestFactory reqFactory = mocks.CreateMock<IHttpWebRequestFactory>();
			IHttpWebRequest request = mocks.DynamicMock<IHttpWebRequest>();

			request.Method = HttpWebRequestMethod.POST;
			LastCall.Repeat.Once();

			IHttpWebResponse response = mocks.DynamicMock<IHttpWebResponse>();
			Expect.Call(request.GetRequestStream()).Repeat.Once().Return(new MemoryStream());
			Expect.Call(request.GetResponse()).Repeat.Once().Return(response);
			Expect.Call(reqFactory.Create("")).IgnoreArguments().Repeat.Once().Return(request);
			string responseXml = "<response/>";
			MemoryStream responseStream = new MemoryStream(Encoding.UTF8.GetBytes(responseXml));
			Expect.Call(response.GetResponseStream()).Repeat.Once().Return(responseStream);
			mocks.ReplayAll();

			ISolrConnection conn = new SolrConnection("https://pepe", reqFactory);
			XmlDocument doc = new XmlDocument();
			conn.PostXml(doc);
			mocks.VerifyAll();
		}

		[Test]
		[Category("Integration")]
		public void ActualConnectionTest() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			IDictionary<string, string> p = new Dictionary<string, string>();
			p["version"] = "2.1";
			p["indent"] = "on";
			p["q"] = "+video +price:[* TO 400]";
			Console.WriteLine(conn.Get("/select/", p));
		}
	}
}