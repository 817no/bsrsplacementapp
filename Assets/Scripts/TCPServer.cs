//References: https://teratail.com/questions/122389

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using TMPro;

public class TCPServer : MonoBehaviour
{
    // Definition of variables.
    private TcpListener server = null;
    bool isOpen = true;
    SynchronizationContext context;

    // Definition of constants.
    const int PORT_NUM = 2001;
    Encoding enc = Encoding.UTF8;

    void Start()
    {
        context = SynchronizationContext.Current;

    }


    void Awake()
    {
        StartSrv();
    }

    void StartSrv()
    {
        server = new TcpListener(IPAddress.Any, PORT_NUM);
        server.Start();
        Debug.Log("Started! - TCPServer");
        StartCoroutine(ServerContinue());
    }

    IEnumerator ServerContinue()
    {
        while (isOpen) {
            server.BeginAcceptTcpClient (new AsyncCallback (DoAcceptTcpClientCallback), server);
            yield return new WaitForSeconds (1f);
        }
    }

    void DoAcceptTcpClientCallback(IAsyncResult res) 
    {
        var listener = res.AsyncState as TcpListener;
        var client = listener.EndAcceptTcpClient(res);

        // Async call next Accept.
        listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        
        Task.Factory.StartNew(() =>
		{
			var stream = client.GetStream();
			IPEndPoint endpoint = (IPEndPoint)client.Client.RemoteEndPoint;
			IPAddress address = endpoint.Address;

            // a var. which the data to be stored.
			byte[] getData = new byte[1];
            // length of data
			int len;
            // temporary list
			List<byte> bytelist=new List<byte>();

			while((len = stream.Read(getData, 0, getData.Length)) > 0)
			{
				foreach(byte byteData in getData)
					bytelist.Add(byteData);
			}

            byte[] result = new byte[bytelist.Count];
            for(int i = 0 ; i < result.Length ; i++){
                    result[i] = bytelist[i];
            }

			string data = enc.GetString(result);
			print("res:" +  data);
            context.Post(_ =>
            {
                var go = GameObject.Find("ConnectTestTMP");
                var tmpu = go.GetComponent<TextMeshPro>();
                tmpu.text = tmpu.text + "\n" + DateTime.Now.ToString() + " - " + data;
            }, null);
        });
    }
}