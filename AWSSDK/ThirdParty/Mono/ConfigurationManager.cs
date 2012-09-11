#region File Description
//-----------------------------------------------------------------------------
// GameScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using System.Collections.Specialized;
using System.IO;


#endregion

#region Using Statements


using System;
using System.Collections.Generic;
using System.Xml.Linq;


#endregion

namespace System.Configuration
{
    /// <summary>
    /// Supplies access to configuration data.
    /// </summary>
    public static class ConfigurationManager
    {
       
        #region Public Static Methods
		static NameValueCollection	appSettings;
		public static NameValueCollection AppSettings {
			get {
				if(appSettings != null)
					return appSettings;
				LoadConfiguration();
				return appSettings;
			}
		}

        /// <summary>
        /// Load configuration from an XML document.
        /// </summary>
        /// <param name="doc">The XML document containing configuration.</param>
        static void LoadConfiguration()
        {
			appSettings = new NameValueCollection();
			if(!File.Exists("App.config"))
				return;
			var doc = XDocument.Load(File.OpenRead("App.config"));
			foreach (XElement element in doc.Element("configuration").Elements("appSettings").Elements("add"))
            {
				if(element.Name.LocalName == "add")
				{
					var key = 	element.Attribute("key").Value;
					var value = element.Attribute("value").Value;
					appSettings.Set(key,value);
				}
                
            }
        }


        #endregion
    }

}
