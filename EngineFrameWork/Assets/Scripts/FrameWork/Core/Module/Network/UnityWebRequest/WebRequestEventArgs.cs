
using System;
using UnityEngine.Networking;
using Core.Interface.Network;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	WebRequestEventArgs.cs
	Author:		DaiLi.Ou

	Descriptions: 
*********************************************************************/
namespace Core.Network
{
    public class WebRequestEventArgs : EventArgs, IHttpResponse
    {

        public UnityWebRequest WebRequest { get; set; }

        public object Request { get { return WebRequest; } }

        public byte[] Bytes { get { return WebRequest.downloadHandler == null ? null : WebRequest.downloadHandler.data; } }

        public string Text { get { return WebRequest.downloadHandler == null ? string.Empty : WebRequest.downloadHandler.text; } }

        public bool IsError { get { return WebRequest.result.Equals(UnityWebRequest.Result.ConnectionError); } }

        public string Error { get { return WebRequest.error; } }

        public long ResponseCode { get { return WebRequest.responseCode; } }

        public Restfuls Restful { get { return (Restfuls)Enum.Parse(typeof(Restfuls), WebRequest.method); } }

        public WebRequestEventArgs(UnityWebRequest request)
        {
            WebRequest = request;
        }
    }

}
