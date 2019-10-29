using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Net.Sockets;

namespace Rishi.ProxyClient
{
	public class HTTPProxy
	{
		TcpClient Client=new TcpClient();
		string Username;
		string Pass;
		string Target=null;
		bool UseAuth=false;
		string Proxy;
		int ProxyPort;
		HTTPProxy (string Target, string Proxy, int ProxyPort)
		{
			this.Target=Target;
			this.Proxy=Proxy;
			this.ProxyPort=ProxyPort;
		}
		HTTPProxy (string Target, string Proxy, int ProxyPort, string Username, string Password)
		{
			this.Target=Target;
			this.Proxy=Proxy;
			this.ProxyPort=ProxyPort;
			this.Username=Username;
			this.Pass=Password;
		}
		///<summary>
		///Get the Stream formed by the proxy.
		///</summary>
		Stream GetStream()
		{
			Client.Connect(Proxy, ProxyPort);
			Stream S = Client.GetStream();
			byte[] Buffer=new byte[]{};
			string AuthStr64=Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Pass}"));
			if(UseAuth==false) 
				Buffer = Encoding.UTF8.GetBytes($"CONNECT {Target} HTTP/1.1\r\n");
			else
				Buffer = Encoding.UTF8.GetBytes($"CONNECT {Target} HTTP/1.1\r\nHost: {Target}\r\nProxy-Authorization: {AuthStr64}\r\n");
			S.Write(Buffer);
			return S;
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
		}
	}
}	
