using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Net.Sockets;
#if NETCOREAPP || NETFRAMEWORK
using Terminal.Gui;
using Rishi.PairStream;
#endif
using System.Text.RegularExpressions;

namespace Rishi.ProxyClient
{
	public class HTTPProxyClient
	{
		TcpClient Client=new TcpClient();
		string Username;
		string Pass;
		string Target=null;
		bool UseAuth=false;
		string Proxy;
		int ProxyPort;
		public HTTPProxyClient (string Target, string Proxy, int ProxyPort)
		{
			this.Target=Target;
			this.Proxy=Proxy;
			this.ProxyPort=ProxyPort;
		}
		public HTTPProxyClient (string Target, string Proxy, int ProxyPort, string Username, string Password)
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
		public Stream GetStream()
		{
			Client.Connect(Proxy, ProxyPort);
			Stream S = Client.GetStream();
			byte[] Buffer=new byte[]{};
			string AuthStr64=Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Pass}"));
			if(UseAuth==false) 
				Buffer = Encoding.UTF8.GetBytes($"CONNECT {Target} HTTP/1.1\r\n\r\n");
			else
				Buffer = Encoding.UTF8.GetBytes($"CONNECT {Target} HTTP/1.1\r\nHost: {Target}\r\nProxy-Authorization: {AuthStr64}\r\n\r\n");
			S.Write(Buffer);
			S.Flush();
			Regex R = new Regex("200");
			if ((R.Match((new StreamReader(S)).ReadLine())).Success)
			{	
				return S;
			}
			else {
				return null;
			}
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
#if NETCOREAPP || NETFRAMEWORK
			Application.Init ();
			var top = Application.Top;

			// Creates the top-level window to show
			var win = new Window ("MyApp") {
				X = 0,
				  Y = 1, // Leave one row for the toplevel menu

				  // By using Dim.Fill(), it will automatically resize without manual intervention
				  Width = Dim.Fill (),
				  Height = Dim.Fill ()
			};
			top.Add (win);
			var proxy = new Label ("Proxy: ") { X = 3, Y = 2 };
			var port = new Label ("Prox.Port.: ") {
				X = Pos.Left (proxy),
				  Y = Pos.Top (proxy) + 1
			};
			var target = new Label ("Target:     ") {
				X = Pos.Left (proxy),
				  Y = Pos.Top (proxy) + 2
			};
			var proxyText = new TextField ("") {
				X = Pos.Right (target),
				  Y = Pos.Top (proxy),
				  Width = 40
			};
			var portText = new TextField ("3128") {
				X = Pos.Right (target),
				  Y = Pos.Top (port),
				  Width = 40
			};
			var targetText = new TextField ("google.com:443") {
				X = Pos.Left (proxyText),
				  Y = Pos.Top (target),
				  Width = Dim.Width (proxyText)
			};
			var login = new Label ("Login: ") { X = 3, Y = 8 };
			var password = new Label ("Password:   ") {
				X = Pos.Left (login),
				  Y = Pos.Top (login) + 2
			};
			var loginText = new TextField ("") {
				X = Pos.Right (password),
				  Y = Pos.Top (login),
				  Width = 40
			};
			var passText = new TextField ("") {
				Secret = true,
					   X = Pos.Left (loginText),
					   Y = Pos.Top (password),
					   Width = Dim.Width (loginText)
			};
			var UseAuth = new CheckBox (3, 6, "Use Proxy Auth (Basic)");
			var TestBtn = new Button (3, 14, "Test", is_default: true){ Clicked = () => { try{RunProxy(proxyText.Text.ToString(), Int32.Parse(portText.Text.ToString()), targetText.Text.ToString()); } finally{}} };
			var ExitBtn =            new Button (14, 14, "Exit"){ Clicked = () => { Application.RequestStop (); } };
			win.Add (
					proxy, target, port, proxyText, portText,targetText,  login, password, loginText, passText, UseAuth, TestBtn, ExitBtn
					);

			Application.Run ();
			Console.WriteLine("Hello World!");
		}
		static void RunProxy(string Proxy, int ProxyPort, string Target){
			try {
				Stream S = (new HTTPProxyClient(Target, Proxy, ProxyPort)).GetStream();
				MessageBox.Query (60, 8, "No Errors.", "No Errors.", "OK");
				pair P = new pair(new StreamReader(Console.OpenStandardInput()), new StreamWriter(Console.OpenStandardOutput()));
				Rishi.PairStream.pair.BindStreams(P, S);
				Application.RequestStop ();
			}
			catch (Exception E){
				MessageBox.ErrorQuery (60, 8, "Error", E.Message, "OK");
			}
#endif
		}
	}
}	
