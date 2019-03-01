using System.IO;
using UnityEngine;

namespace Assets.scripts
{
    static class Util
    {
        public static string GenerateResultsFilePath()
        {
            string path = Application.dataPath.ToString() + "/results.csv";
            int num = 1;

            while (File.Exists(path))
            {
                num++;
                path = Application.dataPath.ToString() + "/results" + num + ".csv";
            }

            return path;
        }

    }
}
