using Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestServer.Net
{
    public class Authorizer
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string filesFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Files/Users");

        public static bool CheckAuthorization(string username, string password, string cryptoKey)
        {
            if (File.Exists(filesFolderPath + @"/Authentifications.json"))
            {
                string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/Authentifications.json"), cryptoKey);

                if (!String.IsNullOrEmpty(fileContent))
                {
                    JObject json = JObject.Parse(fileContent);
                    JArray authentifications = json.GetValue("authentifications").ToObject<JArray>();

                    foreach (JToken authToken in authentifications)
                    {
                        JObject authentification = authToken.ToObject<JObject>();

                        string usernameAuth = authentification.GetValue("username").ToString();
                        string passwordAuth = authentification.GetValue("password").ToString();

                        if (username == usernameAuth && password == passwordAuth)
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool AddNewSpecialistAuthorization(string username, string password, string cryptoKey)
        {
            if (!Authorizer.CheckAuthorization(username, password, cryptoKey))
            {
                if (File.Exists(filesFolderPath + @"/Authentifications.json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/Authentifications.json"), cryptoKey);

                    JObject json = null;
                    JArray authentifications = null;

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        json = JObject.Parse(fileContent);
                        authentifications = json.GetValue("authentifications").ToObject<JArray>();
                        json.Remove("authentifications");
                    }
                    else
                    {
                        authentifications = new JArray();
                        json = new JObject();
                    }

                    JObject authentification = new JObject();
                    authentification.Add("username", username);
                    authentification.Add("password", password);

                    authentifications.Add(authentification);
                    json.Add("authentifications", authentifications);

                    File.WriteAllText(filesFolderPath + @"/Authentifications.json", DataEncryptor.Encrypt(json.ToString(), cryptoKey));

                    return true;
                }
            }
            return false;
        }

        public static bool AddNewClientAuthorization(string username, string password, int birthYear, int weight, bool isMan, string cryptoKey)
        {
            if (!Authorizer.CheckAuthorization(username, password, cryptoKey))
            {
                if (File.Exists(filesFolderPath + @"/Authentifications.json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(filesFolderPath + @"/Authentifications.json"), cryptoKey);

                    JObject json = null;
                    JArray authentifications = null;

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        json = JObject.Parse(fileContent);
                        authentifications = json.GetValue("authentifications").ToObject<JArray>();
                        json.Remove("authentifications");
                    }
                    else
                    {
                        authentifications = new JArray();
                        json = new JObject();
                    }

                    JObject authentification = new JObject();
                    authentification.Add("username", username);
                    authentification.Add("password", password);
                    authentification.Add("birthyear", birthYear);
                    authentification.Add("weight", weight);
                    authentification.Add("gender", (isMan) ? "man" : "woman");

                    authentifications.Add(authentification);
                    json.Add("authentifications", authentifications);

                    File.WriteAllText(filesFolderPath + @"/Authentifications.json", DataEncryptor.Encrypt(json.ToString(), cryptoKey));

                    return true;
                }
            }
            return false;
        }
    }
}
