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

public class TCPClientForLocalization : MonoBehaviour
{
	// Info of Server.
	public string SERVER_IP = "172.17.10.21";
    public int PORT_NUM = 50001;

	// Change to True when use only.
	[SerializeField]
	bool isUse = true;

	//[SerializeField]
	//GameObject ConnectTestTMPGameObject = null;
	SynchronizationContext context;

	Encoding enc = Encoding.UTF8;

	NetworkStream stream = null;
	bool isStopReading = false;
	byte[] readbuf;


	private IEnumerator Start()
	{
		context = SynchronizationContext.Current;
		Debug.Log("START TCP Client for Localization");
		context.Post(_ =>
		{
			var go = GameObject.Find("ConnectTestTMP");
			var tmpu = go.GetComponent<TextMeshPro>();
			tmpu.text = tmpu.text + "\n" + DateTime.Now.ToString() + " - " + $"Trying connect to {SERVER_IP}:{PORT_NUM}";
		}, null);
		readbuf = new byte[1024];
        // NOTE: Find() can be used only in main thread.
		while (isUse)
		{
			if (!isStopReading)
			{
				StartCoroutine(ReadMessage());
			}
			yield return new WaitForSeconds(1f);
		}
	}


	public void SendCurrentMessage(string message)
	{
		// ***Do not use "IEnumerator", just void.***
        try
        {
			Debug.Log("START SendMessage:" + message);
			string playerName = "[A]: ";
			byte[] sendBytes = enc.GetBytes(playerName + message + "\n");
			// Sending Data
			stream.Write(sendBytes, 0, sendBytes.Length);
		}
        catch (Exception ex)
        {
			Debug.Log(ex.ToString());
        }

	}

	private IEnumerator ReadMessage()
	{
        try
        {
			stream = GetNetworkStream();
			stream.BeginRead(readbuf, 0, readbuf.Length, new AsyncCallback(ReadCallback), null);
			isStopReading = true;
		}
        catch (Exception)
        {
        }
		yield return null;
	}

	public void ReadCallback(IAsyncResult ar)
	{
        try
        {
			stream = GetNetworkStream();
			int bytes = stream.EndRead(ar);
			string message = enc.GetString(readbuf, 0, bytes);
			message = message.Replace("\r", "").Replace("\n", "");
			isStopReading = false;
			Debug.Log(message);
			try
			{
				context.Post(_ =>
				{
					var go = GameObject.Find("ConnectTestTMP");
					var tmpu = go.GetComponent<TextMeshPro>();
					tmpu.text = tmpu.text + "\n" + DateTime.Now.ToString() + " - " + message;
				}, null);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}
        catch (Exception)
        {
        }
	}

	private NetworkStream GetNetworkStream()
	{
        try
        {
			var t = new TcpClient(SERVER_IP, PORT_NUM).GetStream();
			return t;
		}
        catch (Exception ex)
        {
			Debug.Log(ex.ToString());
			context.Post(_ =>
			{
				var go = GameObject.Find("ConnectTestTMP");
				var tmpu = go.GetComponent<TextMeshPro>();
				tmpu.text = tmpu.text + "\n" + DateTime.Now.ToString() + " - " + ex.ToString();
			}, null);
			isUse = false;
			return null;
        }
	}




}
