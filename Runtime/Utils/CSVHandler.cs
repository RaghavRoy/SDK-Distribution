using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JioXSDK.Utils
{
    public static class CSVHandler
    {
        private const string Separator = ";";
        private static readonly List<string> listOfData = new List<string>();
        private static string headerString;

        public static void SaveCSV(string fileName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("sep=" + Separator);
            sb.AppendLine(headerString);

            foreach (var line in listOfData)
            {
                sb.AppendLine(line);
            }

            var filePath = Path.Combine(Application.persistentDataPath, fileName + ".csv");

            try
            {
                File.WriteAllText(filePath, sb.ToString());
                Debug.Log($"The file is saved at: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public static void SetHeaderData(params string[] headers)
        {
            headerString = "Time;";
            headerString += string.Join(Separator, headers);
        }

        public static void AddLineData(params object[] data)
        {
            string line = GetTimestamp();
            line += string.Join(Separator, data.Select(FormatData));
            listOfData.Add(line);
        }

        private static string GetTimestamp() =>
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss};";


        private static string FormatData(object value) =>
          value?.ToString() ?? string.Empty;
    }
}