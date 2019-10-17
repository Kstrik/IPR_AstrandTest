using ÄstrandTestServer.Net;
using Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ÄstrandTestServer.Files
{
    public class FileHandler
    {
        private static string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string testsFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Files/Tests");
        private static string usersFolderPath = System.IO.Path.Combine(Directory.GetParent(appFolderPath).Parent.FullName, "Files/Users");

        public static ÄstrandTest GetAstrandTestData(string filename)
        {
            lock (filename)
            {
                if (File.Exists(testsFolderPath + @"/" + filename + ".json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(testsFolderPath + @"/" + filename + ".json"), DataEncryptor.FileKey);

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        try
                        {
                            ÄstrandTest astrandTest = new ÄstrandTest();

                            JObject historydataJson = JObject.Parse(fileContent);

                            JObject personalData = historydataJson.GetValue("personaldata").ToObject<JObject>();
                            astrandTest.Username = personalData.GetValue("name").ToString();
                            astrandTest.BirthYear = int.Parse(personalData.GetValue("birthyear").ToString());
                            astrandTest.Weight = int.Parse(personalData.GetValue("weight").ToString());
                            astrandTest.IsMan = (personalData.GetValue("gender").ToString() == "man") ? true : false;

                            JArray heartratesJson = historydataJson.GetValue("heartrates").ToObject<JArray>();
                            JArray distancesJson = historydataJson.GetValue("distances").ToObject<JArray>();
                            JArray speedsJson = historydataJson.GetValue("speeds").ToObject<JArray>();
                            JArray cycleRhythmsJson = historydataJson.GetValue("cyclerhythms").ToObject<JArray>();

                            foreach (JObject heartrateJson in heartratesJson)
                                astrandTest.HeartrateValues.Add((int.Parse(heartrateJson.GetValue("heartrate").ToString()), DateTime.Parse(heartrateJson.GetValue("time").ToString())));

                            foreach (JObject distanceJson in distancesJson)
                                astrandTest.DistanceValues.Add((int.Parse(distanceJson.GetValue("distance").ToString()), DateTime.Parse(distanceJson.GetValue("time").ToString())));

                            foreach (JObject speedJson in speedsJson)
                                astrandTest.SpeedValues.Add((int.Parse(speedJson.GetValue("speed").ToString()), DateTime.Parse(speedJson.GetValue("time").ToString())));

                            foreach (JObject cycleRhythmJson in cycleRhythmsJson)
                                astrandTest.CycleRhythmValues.Add((int.Parse(cycleRhythmJson.GetValue("cyclerhythm").ToString()), DateTime.Parse(cycleRhythmJson.GetValue("time").ToString())));

                            return astrandTest;
                        }
                        catch (Exception e) { }
                    }
                }
                return null;
            }
        }

        public static void SaveAstrandTestData(ÄstrandTest testData)
        {
            JObject personalData = new JObject();
            JArray heartratesJson = new JArray();
            JArray distancesJson = new JArray();
            JArray speedsJson = new JArray();
            JArray cycleRhythmsjson = new JArray();

            personalData.Add("name", testData.Username);
            personalData.Add("birthyear", testData.BirthYear);
            personalData.Add("weight", testData.Weight);
            personalData.Add("gender", (testData.IsMan) ? "man" : "woman");

            foreach ((int heartrate, DateTime time) heartrateData in testData.HeartrateValues)
            {
                JObject heartrateJson = new JObject();
                heartrateJson.Add("heartrate", heartrateData.heartrate);
                heartrateJson.Add("time", heartrateData.time.ToString());
                heartratesJson.Add(heartrateJson);
            }

            foreach ((int distance, DateTime time) distanceData in testData.DistanceValues)
            {
                JObject distanceJson = new JObject();
                distanceJson.Add("distance", distanceData.distance);
                distanceJson.Add("time", distanceData.time.ToString());
                distancesJson.Add(distanceJson);
            }

            foreach ((int speed, DateTime time) speedData in testData.SpeedValues)
            {
                JObject speedJson = new JObject();
                speedJson.Add("speed", speedData.speed);
                speedJson.Add("time", speedData.time.ToString());
                speedsJson.Add(speedJson);
            }

            foreach ((int cycleRhythm, DateTime time) cycleRhythmData in testData.CycleRhythmValues)
            {
                JObject cycleRhythmJson = new JObject();
                cycleRhythmJson.Add("cyclerhythm", cycleRhythmData.cycleRhythm);
                cycleRhythmJson.Add("time", cycleRhythmData.time.ToString());
                cycleRhythmsjson.Add(cycleRhythmJson);
            }

            JObject testJson = new JObject();
            testJson.Add("personaldata", personalData);
            testJson.Add("heartrates", heartratesJson);
            testJson.Add("distances", distancesJson);
            testJson.Add("speeds", speedsJson);
            testJson.Add("cyclerhythms", cycleRhythmsjson);

            File.WriteAllText(testsFolderPath + @"/" + testData.Username + " " + DateTime.Now.ToString() + ".json", DataEncryptor.Encrypt(testJson.ToString(), DataEncryptor.FileKey));
        }

        public static (int birthYear, int weight, bool isMan) GetPersonalData(string username)
        {
            lock (username)
            {
                if (File.Exists(usersFolderPath + @"/Authentifications.json"))
                {
                    string fileContent = DataEncryptor.Decrypt(File.ReadAllText(usersFolderPath + @"/Authentifications.json"), DataEncryptor.FileKey);

                    if (!String.IsNullOrEmpty(fileContent))
                    {
                        JObject json = JObject.Parse(fileContent);
                        JArray authentifications = json.GetValue("authentifications").ToObject<JArray>();

                        foreach (JToken authToken in authentifications)
                        {
                            JObject authentification = authToken.ToObject<JObject>();

                            string usernameAuth = authentification.GetValue("username").ToString();

                            if (username == usernameAuth)
                            {
                                int birthYear = int.Parse(authentification.GetValue("birthyear").ToString());
                                int weight = int.Parse(authentification.GetValue("weight").ToString());
                                bool isMan = (authentification.GetValue("gender").ToString() == "man") ? true : false;

                                return (birthYear, weight, isMan);
                            }
                        }
                    }
                }
                return (0, 0, true);
            }
        }

        public static List<string> GetAllTests()
        {
            List<string> bsns = new List<string>();

            string[] filePaths = Directory.GetFiles(testsFolderPath);

            foreach (string filePath in filePaths)
            {
                string[] split = filePath.Split('\\');
                string filename = split[split.Length - 1].Replace(".json", "");
                if (filename.ToLower() != "authentifications")
                    bsns.Add(filename);
            }

            return bsns;
        }
    }
}
