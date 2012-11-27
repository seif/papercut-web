/* For syntax highlighting and better readability of this file, view it on GitHub:
 * https://github.com/ServiceStack/ServiceStack.Examples/blob/master/src/RestFiles/RestFiles.Tests/AsyncRestClientTests.cs
 */

namespace Tests.Papercut
{
    using System;
    using System.IO;
    using System.Threading;

    using NUnit.Framework;

    using ServiceStack.Service;
    using ServiceStack.ServiceClient.Web;

    using global::Papercut.Smtp;
    using global::Papercut.WebHost;
    using global::Papercut.WebHost.Operations;
    using global::Papercut.WebHost.Types;

    /// <summary>
    /// These test show how you can call ServiceStack REST web services asynchronously using an IRestClientAsync.
    /// 
    /// Async service calls are a great for GUI apps as they can be called without blocking the UI thread.
    /// They are also great for performance as no time is spent on blocking IO calls.
    /// </summary>
    [TestFixture]
    public class AsyncRestClientTests
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
            this.MailFolder = this.appHost.Config.MailFolder;
            if (Directory.Exists(this.MailFolder))
            {
                Directory.Delete(this.MailFolder, true);
            }
            Directory.CreateDirectory(this.MailFolder);
            Directory.CreateDirectory(Path.Combine(this.MailFolder , "testdelete"));
            var path = Path.Combine(this.MailFolder, "default");
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "2012112622230791-a8.eml"), EmlFileContents);
            File.WriteAllText(Path.Combine(path, "2012112622233799-a3.eml"), EmlFileContents);
        }

        public IRestClientAsync CreateAsyncRestClient()
        {
            return new JsonServiceClient(WebServiceHostUrl);  //Best choice for Ajax web apps, faster than XML
            //return new XmlServiceClient(WebServiceHostUrl); //Ubiquitous structured data format best for supporting non .NET clients
            //return new JsvServiceClient(WebServiceHostUrl); //Fastest, most compact and resilient format great for .NET to .NET client / server
        }

        
        private static void FailOnAsyncError<T>(T response, Exception ex)
        {
            Assert.Fail(ex.Message);
        }

        [Test]
        public void Can_GetAsync_to_retrieve_mailbox()
        {
            var restClient = this.CreateAsyncRestClient();

            MailboxResult response = null;
            restClient.GetAsync<MailboxResult>("mailboxes/default",
                r => response = r, FailOnAsyncError);

            Thread.Sleep(5000);

            Assert.That(response.Emails, Is.Not.Null);
            Assert.That(response.Emails.Count, Is.GreaterThan(0));
            Assert.That(response.Emails[0].Body, Is.Not.Null, "Email body was null");
        }

        [Test]
        public void Can_PostAsync_to_create_new_mailbox()
        {
            var restClient = this.CreateAsyncRestClient();

            MailboxResult response = null;
            restClient.PostAsync<MailboxResult>("mailboxes/test",
                new Mailbox(),
                r => response = r, FailOnAsyncError);

            Thread.Sleep(5000);

            Assert.That(Directory.Exists(Path.Combine(this.MailFolder, "test")));
        }

        [Test]
        public void Can_DeleteAsync_to_delete_mailbox()
        {
            var restClient = this.CreateAsyncRestClient();

            MailboxResult response = null;
            restClient.DeleteAsync<MailboxResult>("mailboxes/testdelete",
                r => response = r, FailOnAsyncError);

            Thread.Sleep(5000);

            Assert.That(!Directory.Exists(Path.Combine(this.MailFolder, "testdelete")));
        }


        /* 
         * Error Handling Tests
         */
        [Test]
        public void GET_a_mailbox_that_doesnt_exist_throws_a_404_FileNotFoundException()
        {
            var restClient = this.CreateAsyncRestClient();

            WebServiceException webEx = null;
            MailboxResult response = null;

            restClient.GetAsync<MailboxResult>("mailboxes/UnknownMailbox",
               r => response = r,
               (r, ex) =>
               {
                   response = r;
                   webEx = (WebServiceException)ex;
               });

            Thread.Sleep(5000);

            Assert.That(webEx.StatusCode, Is.EqualTo(404));
            //Assert.That(webEx.ResponseStatus.ErrorCode, Is.EqualTo(typeof(FileNotFoundException).Name));
            //Assert.That(webEx.ResponseStatus.Message, Is.EqualTo("Could not find: UnknownMailbox"));
        }

        [Test]
        public void POST_to_an_existing_mailbox_throws_a_409_NotSupportedException()
        {
            var restClient = new JsonServiceClient(WebServiceHostUrl);

            try
            {
                restClient.Post<MailboxResult>("mailboxes/default", new Mailbox());

                Assert.Fail("Should fail with NotSupportedException");
            }
            catch (WebServiceException webEx)
            {
                Assert.That(webEx.StatusCode, Is.EqualTo(409));
                //Assert.That(webEx.ResponseStatus.ErrorCode, Is.EqualTo(typeof(NotSupportedException).Name));
                //Assert.That(webEx.ResponseStatus.Message,
                //    Is.EqualTo("Mailbox already exists: default"));
            }
        }

        [Test]
        public void DELETE_a_non_existing_mailbox_throws_404()
        {
            var restClient = this.CreateAsyncRestClient();

            WebServiceException webEx = null;
            MailboxResult response = null;

            restClient.DeleteAsync<MailboxResult>("mailboxes/non-existing",
               r => response = r,
               (r, ex) =>
               {
                   response = r;
                   webEx = (WebServiceException)ex;
               });

            Thread.Sleep(5000);

            Assert.That(webEx.StatusCode, Is.EqualTo(404));
            //Assert.That(webEx.ResponseStatus.ErrorCode, Is.EqualTo(typeof(FileNotFoundException).Name));
            //Assert.That(webEx.ResponseStatus.Message, Is.EqualTo("Could not find: non-existing"));
        }

    }
}