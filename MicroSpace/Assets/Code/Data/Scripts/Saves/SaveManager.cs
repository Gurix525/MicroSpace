using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Data
{
    public static class SaveManager
    {
        public static void SaveGame()
        {
            var save = Database.Save;
            var json = JsonUtility.ToJson(save, true);
            string path = Path.Combine(Application.persistentDataPath, "saveFile");
            int fileNumber = 0;
            string filePath = $"{path}{fileNumber}.txt";
            while (File.Exists(filePath))
            {
                fileNumber++;
                filePath = $"{path}{fileNumber}.txt";
            }
            File.WriteAllText(filePath, json);
            Debug.Log($"Succesfully saved under: {filePath}");
        }

        public static void LoadGame(string name = "")
        {
            ClearWorld();
            string toLoad = string.Empty;
            if (File.Exists(Path.Combine(Application.persistentDataPath, name)))
                toLoad = File.ReadAllText(
                    Path.Combine(Application.persistentDataPath, name));
            else
            {
                var directory = new DirectoryInfo(Application.persistentDataPath);
                try
                {
                    var newestSave = directory.GetFiles()
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();
                    if (newestSave != null)
                        toLoad = File.ReadAllText(newestSave.FullName);
                }
                catch (InvalidOperationException)
                {
                    Debug.Log("Load failed.");
                    return;
                }
            }
            if (toLoad != string.Empty)
            {
                Save save = JsonUtility.FromJson<Save>(toLoad);
                Database.LoadFromSave(save);
                Debug.Log("Save succesfully loaded.");
            }
        }

        private static void ClearWorld()
        {
            var world = GameObject.Find("World");
            for (int i = 0; i < world.transform.childCount; i++)
            {
                GameObject.Destroy(world.transform.GetChild(i).gameObject);
            }
        }
    }
}