using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Account
{
	public class MicrosoftAuth
	{
		public MicrosoftAuth(string clientId, int port)
		{
			ClientId = clientId;
			Port = port;
		}

		//string ClientId = "d827de77-7896-41ee-b446-c3f0684a8c53";
		string ClientId { get; set; }
		int Port { get; set; }

		public HttpListener listener;
		public string pageData =
			"<!DOCTYPE>" +
			"<html>" +
			"  <head>" +
			"    <title>Melonhcer</title>" +
			"    <script>self:close()</script>" +
			"  </head>" +
			"  <body>" +
			"    <h1>Можешь закрывать страницу</h1>" +
			"  </body>" +
			"</html>";


		bool runServer = true;

		public async Task<MicrosoftAuthResult> HandleIncomingConnections()
		{

			// While a user hasn't visited the `shutdown` url, keep on handling requests
			while (runServer)
			{
				// Will wait here until we hear from a connection
				HttpListenerContext ctx = await listener.GetContextAsync();

				// Peel out the requests and response objects
				HttpListenerRequest req = ctx.Request;
				HttpListenerResponse resp = ctx.Response;

				// Print out some info about the request
				Console.WriteLine(req.Url.ToString());
				Console.WriteLine(req.HttpMethod);
				Console.WriteLine(req.UserHostName);
				Console.WriteLine(req.UserAgent);
				Console.WriteLine();

				// Write the response info
				byte[] data = Encoding.UTF8.GetBytes(pageData);
				resp.ContentType = "text/html";
				resp.ContentEncoding = Encoding.UTF8;
				resp.ContentLength64 = data.LongLength;

				// Write out to the response stream (asynchronously), then close it
				await resp.OutputStream.WriteAsync(data, 0, data.Length);
				resp.Close();

				if (req.HttpMethod == "GET")
				{
					string code = null;
					string state = null;
					for (int i = 0; i < req.QueryString.Count; i++)
					{
						if (req.QueryString.GetKey(i) == "code") code = req.QueryString.Get(i);
						else if (req.QueryString.GetKey(i) == "state") state = req.QueryString.Get(i);
					}
					if (code != null && state != null)
					{
						runServer = false;
						return new MicrosoftAuthResult(code, state);
					}
				}

			}
			return null;
		}

		public void Stop()
		{
			runServer = false;
		}

		public async Task<MicrosoftAuthResult> Start()
		{
			// Create a Http server and start listening for incoming connections

			string url = "http://localhost:" + Port + "/";
			string authUrl = "https://login.live.com/oauth20_authorize.srf?client_id=" + ClientId + "&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A" + Port + "&scope=Xboxlive.signin+offline_access&state=7530618937927700268";

			//string authUrl = "https://login.live.com/oauth20_authorize.srf?client_id=a332a5f6-c4dc-45b6-9fe3-d881490252b2&response_type=code&redirect_uri=http%3A%2F%2Flocalhost%3A5000&scope=Xboxlive.signin+offline_access&state=7530618937927700268";
			//Process.Start(new ProcessStartInfo("cmd", $"/c start {authUrl}") { CreateNoWindow = true });
			var psi = new ProcessStartInfo();
			psi.UseShellExecute = true;
			psi.FileName = authUrl;
			Process.Start(psi);
			listener = new HttpListener();
			listener.Prefixes.Add(url);
			listener.Start();
			Console.WriteLine("Listening for connections on {0}", url);

			// Handle requests
			var result = await HandleIncomingConnections();

			// Close the listener
			listener.Close();
			return result;
		}
	}
	public class MicrosoftAuthResult
	{
		public MicrosoftAuthResult(string code, string state)
		{
			Code = code;
			State = state;
		}

		public string Code { get; set; }
		public string State { get; set; }
	}
}
