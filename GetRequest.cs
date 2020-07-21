﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace AchievementTest
{
    class GetRequest
    {
        public static XmlDocument XmlRequest(string RequestUrl) {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(RequestUrl);
            request.Method = "POST";
            request.ContentType = "text/xml";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(new StreamReader(response.GetResponseStream(), Encoding.UTF8));
                }
                catch
                {

                }
                return doc;
            }
        }
    }
}
