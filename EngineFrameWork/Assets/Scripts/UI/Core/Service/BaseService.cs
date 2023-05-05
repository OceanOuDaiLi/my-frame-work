using Model;
using System;
using System.Collections.Generic;

/********************************************************************
	Copyright © 2018 - 2050 by DaiLi.Ou. All Rights Reserved. e-mail: odaili@163.com

	Created:	2018 ~ 2023
	Filename: 	BaseService.cs
	Author:		DaiLi.Ou

	Descriptions: 
*********************************************************************/

namespace UI
{
    public class BaseService
    {
        [Inject]
        public TcpService tcpService { get; set; }
        [Inject]
        public GlobalData globalData { get; set; }

        public BaseService() { }

        protected void Request(string action, Dictionary<string, object> param, Action<Dictionary<string, object>> succeed)
        {
            tcpService.Request(action, param, (dict) =>
            {
                // handledata for xxxModelManger or handledata on 'succeed' callback.
                succeed(dict);
            });
        }

        protected void HandleData(Dictionary<string, object> data)
        {

        }
    }
}