using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// This file contains classes adapted from the examples published at
// http://blog.salamandersoft.co.uk/index.php/2009/10/how-to-mock-httpwebrequest-when-unit-testing/
namespace CtcApi.Testing
{
  /// <summary>
  /// Provides functionality for injecting mock HTTP objects into standard .NET calls
  /// </summary>
  /// <remarks>
  /// This class is primarily intended to make it easier to write unit tests for code
  /// which makes external HTTP calls.
  /// </remarks>
	public class FakeHttpHelper
	{
		private TestWebRequest _testRequest;
		
		/// <summary>
		/// Gets or sets the text body of the <see cref="WebResponse"/> to simulate
		/// </summary
		public string ResponseText{get;set;}
		
		/// <summary>
		/// Gets or sets the <see cref="WebRequest"/> headers expected to be set by the code being tested.
		/// </summary
		/// <seealso cref="AssertExpectedHeaders"/>
		public NameValueCollection ExpectedHeaders{get;set;}

		private FakeHttpHelper()
		{
			ExpectedHeaders = new NameValueCollection();
		}
		
		/// <summary></summary>
		/// <param name="schema">
		/// The URL schema to inject into the <see cref="WebRequest"/> factory. URLs
		/// with the specified schema will be handled by this <see cref="TestWebRequest"/>
		/// </param>
		/// <seealso cref="TestWebRequestCreate.CreateTestRequest"/>
		public FakeHttpHelper(string schema, string responseText) : this()
		{
			WebRequest.RegisterPrefix(schema, new TestWebRequestCreate());
			_testRequest = TestWebRequestCreate.CreateTestRequest(responseText);
			ResponseText = responseText;
		}

		/// <summary>
		/// 
		/// </summary>
		public void AssertExpectedHeaders()
		{
			foreach (string name in ExpectedHeaders)
			{
				Assert.AreEqual(ExpectedHeaders[name], _testRequest.Headers[name]);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="method"></param>
		public void AssertRequestMethod(string method)
		{
			Assert.AreEqual(method.ToUpper(), _testRequest.Method.ToUpper());
		}
	}


	/// <summary>
	/// A web request creator for unit testing.
	/// </summary>
	public class TestWebRequestCreate : IWebRequestCreate
	{
		static WebRequest _nextRequest;
		static object _lockObject = new object();

		static public WebRequest NextRequest
		{
			get { return _nextRequest; }
			set
			{
				lock (_lockObject)
				{
					_nextRequest = value;
				}
			}
		}

		/// <summary>
		/// See <see cref="IWebRequestCreate.Create"/>.
		/// </summary>
		public WebRequest Create(Uri uri)
		{
			return _nextRequest;
		}

		/// <summary>
		/// Utility method for creating a TestWebRequest and setting
		/// it to be the next WebRequest to use.
		/// </summary>
		/// <param name="response">The response the TestWebRequest will return.</param>
		public static TestWebRequest CreateTestRequest(string response)
		{
			TestWebRequest request = new TestWebRequest(response);
			NextRequest = request;
			return request;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class TestWebRequest : WebRequest
	{
		private MemoryStream _requestStream = new MemoryStream();
		private MemoryStream _responseStream;
		private WebHeaderCollection _headers = new WebHeaderCollection();
		private string _responseText = string.Empty;

		public override string Method { get; set; }
		public override string ContentType { get; set; }
		public override long ContentLength { get; set; }
		public override WebHeaderCollection Headers
		{
		  get {return _headers;}
		  set {_headers = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public MemoryStream RequestStream
		{
			get
			{
				if (!_requestStream.CanRead || !_requestStream.CanWrite)
				{
					// Create a new Stream if the previous one was closed
					// (This is necessary when the code makes successive WebRequests, closing the stream between each one.)
					_requestStream = InitMemoryStream();
				}
				return _requestStream;
			}
			private set {_requestStream = value;}
		}

		/// <summary>
		/// 
		/// </summary>
		public MemoryStream ResponseStream
		{
			get
			{
				if (!_responseStream.CanRead)
				{
					_responseStream = InitMemoryStream(_responseText);
				}
				return _responseStream;
			}
			private set {_responseStream = value;}
		}

		public TestWebRequest()
		{
			_headers = new WebHeaderCollection();
		}

		public TestWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
			_headers = new WebHeaderCollection();
		}

		/// <summary>
		/// Initializes a new instance of <see cref="TestWebRequest"/> with the response to return.
		/// </summary>
		public TestWebRequest(string response)
		{
			RequestStream = InitMemoryStream();
			_responseText = response;
			ResponseStream = InitMemoryStream(_responseText);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="responseText"></param>
		/// <returns></returns>
		private MemoryStream InitMemoryStream(string responseText = null)
		{
			if (string.IsNullOrWhiteSpace(responseText))
			{
				return new MemoryStream();
			}
			else
			{
				return new MemoryStream(Encoding.UTF8.GetBytes(responseText));
			}
		}

		/// <summary>
		/// Returns the request contents as a string.
		/// </summary>
		public string ContentAsString()
		{
			return Encoding.UTF8.GetString(RequestStream.ToArray());
		}

		/// <summary>
		/// See <see cref="WebRequest.GetRequestStream"/>.
		/// </summary>
		public override Stream GetRequestStream()
		{
			return RequestStream;
		}

		/// <summary>
		/// See <see cref="WebRequest.GetResponse"/>.
		/// </summary>
		public override WebResponse GetResponse()
		{
			return new TestWebReponse(ResponseStream);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class TestWebReponse : WebResponse
	{
		Stream _responseStream;
		private WebHeaderCollection _headers = new WebHeaderCollection();

		/// <summary>
		/// 
		/// </summary>
		public override WebHeaderCollection Headers
		{
		  get {return _headers;}
		}

		public TestWebReponse()
		{
			_headers = new WebHeaderCollection();
		}

		public TestWebReponse(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
			_headers = new WebHeaderCollection();
		}

		/// <summary>
		/// Initializes a new instance of <see cref="TestWebReponse"/>
		/// with the response stream to return.
		/// </summary>
		public TestWebReponse(Stream responseStream)
		{
			_responseStream = responseStream;
		}

		/// <summary>
		/// See <see cref="WebResponse.GetResponseStream"/>.
		/// </summary>
		public override Stream GetResponseStream()
		{
			return _responseStream;
		}
	}
}