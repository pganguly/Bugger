﻿using BigEgg.Framework.Applications.ViewModels;
using Bugger.Proxys.TFS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Bugger.Proxys.TFS.Documents
{
    internal class SettingDocumentType : DataModel
    {
        #region Fields
        private const string FileExtension = ".setting";
        private static string filePath;
        #endregion

        static SettingDocumentType()
        {
            filePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().FullName) + FileExtension);
        }

        #region Properties
        public static string FilePath { get { return filePath; } }
        #endregion

        #region Methods
        #region Public Methods
        public static SettingDocument New()
        {
            return new SettingDocument();;
        }

        public static SettingDocument Open()
        {
            SettingDocument document = new SettingDocument();

            XElement root = XDocument.Load(filePath).Root;
            byte[] entropy = Convert.FromBase64String(root.Attribute("entropy").Value);

            document.ConnectUri = new Uri(root.Element("Uri").Value);
            document.UserName = Decrypt(entropy, root.Element("UserName").Value);
            document.Password = Decrypt(entropy, root.Element("Password").Value);
            document.BugFilterField = root.Element("BugFilterField").Value;
            document.BugFilterValue = root.Element("BugFilterValue").Value;
            document.PriorityRed = root.Element("PriorityRed").Value;

            IEnumerable<XElement> elements = root.Element("PropertyMappings").Elements();
            foreach (XElement element in elements)
            {
                document.PropertyMappingList.First(x => x.PropertyName == element.Name).FieldName = element.Value;
            }

            return document;
        }

        public static void Save(SettingDocument document)
        {
            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            XElement propertyMappingElement = new XElement("PropertyMappings");
            foreach (MappingPair pair in document.PropertyMappingList)
            {
                propertyMappingElement.Add(new XElement(pair.PropertyName, pair.FieldName));
            }

            XDocument settingDocument = new XDocument(
                new XComment("TFS Proxy Settings"),
                new XElement("Settings",
                    new XAttribute("entropy", Convert.ToBase64String(entropy)),
                    new XElement("Uri", document.ConnectUri.AbsoluteUri),
                    new XElement("UserName", Encrypt(entropy, document.UserName)),
                    new XElement("Password", Encrypt(entropy, document.Password)),
                    new XElement("BugFilterField", document.BugFilterField),
                    new XElement("BugFilterValue", document.BugFilterValue),
                    new XElement("PriorityRed", document.PriorityRed),
                    propertyMappingElement
                )
            );

            settingDocument.Save(FilePath);
        }
        #endregion

        #region Private Methods
        private static string Decrypt(byte[] entropy, string cipherString)
        {
            if (entropy == null) { throw new ArgumentException("entropy"); }
            if (string.IsNullOrEmpty(cipherString)) { throw new ArgumentException("cipherString"); }

            byte[] cipherText = Convert.FromBase64String(cipherString);
            byte[] plainText = ProtectedData.Unprotect(cipherText, entropy, DataProtectionScope.CurrentUser);

            return Encoding.Default.GetString(plainText);
        }

        private static string Encrypt(byte[] entropy, string plainString)
        {
            if (entropy == null) { throw new ArgumentException("entropy"); }
            if (string.IsNullOrEmpty(plainString)) { throw new ArgumentException("plainString"); }

            byte[] plainText = Encoding.Default.GetBytes(plainString);
            byte[] cipherText = ProtectedData.Protect(plainText, entropy, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(cipherText);
        }
        #endregion
        #endregion
    }
}
