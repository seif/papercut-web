/* Based on Test from ServiceStack RestFiles sample:
 * https://github.com/ServiceStack/ServiceStack.Examples/blob/master/src/RestFiles/RestFiles.Tests/RestClientTests.cs
 */

namespace Tests.Papercut
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using NUnit.Framework;

    using ServiceStack.Service;
    using ServiceStack.ServiceClient.Web;

    using global::Papercut.Smtp;
    using global::Papercut.WebHost;
    using global::Papercut.WebHost.Operations;
    using global::Papercut.WebHost.Types;

    [TestFixture]
    public class RestClientTests
    {
        public string MailFolder;
        protected const string WebServiceHostUrl = "http://localhost:52080/";
        protected const string EmlFileContents = @"MIME-Version: 1.0
From: test@papercut.com
To: seif@testing.com
Date: 26 Nov 2012 22:23:07 +0000
Subject: Hello From Papercut Tests!
Content-Type: text/plain; charset=us-ascii
Content-Transfer-Encoding: quoted-printable

This is a Body

";

        PapercutHttpListener appHost;
        

        [TestFixtureSetUp]
        public void TextFixtureSetUp()
        {
            this.appHost = new PapercutHttpListener();
            this.appHost.Init();
            this.appHost.Start("http://localhost:52080/");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            if (this.appHost != null) this.appHost.Dispose();
            this.appHost = null;
        }

        [SetUp]
        public void OnBeforeEachTest()
        {
            this.MailFolder = this.appHost.PapercutConfig.MailFolder;
            if (Directory.Exists(this.MailFolder))
            {
                Directory.Delete(this.MailFolder, true);
            }
            Directory.CreateDirectory(this.MailFolder);

            Directory.CreateDirectory(MailFolder);
            File.WriteAllText(Path.Combine(MailFolder, "2012112622230791-a8.eml"), EmlFileContents);
            File.WriteAllText(Path.Combine(MailFolder, "2012112622233799-a3.eml"), EmlFileContents);
        }

        [TearDown]
        public void OnAfterEachTest()
        {
            if (Directory.Exists(this.MailFolder))
            {
          //      Directory.Delete(this.MailFolder, true);
            }
        }

        public IRestClient CreateRestClient()
        {
            return new JsonServiceClient(WebServiceHostUrl);  //Best choice for Ajax web apps, faster than XML
            //return new XmlServiceClient(WebServiceHostUrl); //Ubiquitous structured data format best for supporting non .NET clients
            //return new JsvServiceClient(WebServiceHostUrl); //Fastest, most compact and resilient format great for .NET to .NET client / server
        }
        
        [Test]
        public void Can_Get_to_retrieve_email()
        {
            var restClient = this.CreateRestClient();

            EmailResponse response = restClient.Get(new Email { Id = "2012112622230791-a8" });

            Assert.That(response, Is.Not.Null);
            Assert.That(response.From, Is.Not.Null, "'From' property was null");
        }

        [Test]
        public void Can_Get_to_retrieve_mailbox()
        {
            var restClient = this.CreateRestClient();

            MailboxResponse response = restClient.Get(new MailboxRequest());

            Assert.That(response.Emails, Is.Not.Null);
            Assert.That(response.Emails.Count, Is.GreaterThan(0));
            Assert.That(response.Emails[0].Body, Is.Not.Null, "Email body was null");
        }

        [Test]
        public void Can_Delete_to_clear_mailbox()
        {
            var restClient = this.CreateRestClient();

            restClient.Delete(new MailboxRequest());

            Assert.That(Directory.Exists(this.MailFolder));
            Assert.That(Directory.GetFiles(this.MailFolder).Length, Is.EqualTo(0));
        }
    }
}