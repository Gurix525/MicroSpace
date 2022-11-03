using Assets.Code.Ships;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Main
{
    public static class SaveManager
    {
        #region Public

        public static void SaveGame()
        {
            string saveJson = CreateSaveJson();
            string filePath = CreateSavePath();
            SaveToFile(saveJson, filePath);
            Debug.Log($"Succesfully saved under: {filePath}");
        }

        public static void LoadGame(string name = "")
        {
            ClearWorld();
            if (TryGetJsonFromFile(name, out string saveJson))
            {
                var save = GetSaveFromJson(saveJson);
                LoadFromSave(save);
                Debug.Log("Save succesfully loaded.");
            }
        }

        #endregion Public

        #region Private

        private static void LoadFromSave(Save save)
        {
            LoadShips(save.Ships);
        }

        private static void LoadShips(List<SerializableShip> ships)
        {
            foreach (var shipToLoad in ships)
                LoadShip(shipToLoad);
        }

        private static void LoadShip(SerializableShip shipToLoad)
        {
            InstantiateShip(out GameObject ship);
            SetShipParameters(ship, shipToLoad);
            LoadWalls(ship, shipToLoad);
            LoadFloors(ship, shipToLoad);
        }

        private static void LoadFloors(GameObject ship, SerializableShip shipToLoad)
        {
            foreach (SerializableFloor floor in shipToLoad.Floors)
                LoadFloor(ship, floor);
        }

        private static void LoadFloor(GameObject ship, SerializableFloor floorToLoad)
        {
            InstantiateFloor(out GameObject floor, ship);
            SetFloorParameters(floor, floorToLoad);
        }

        private static void SetFloorParameters(GameObject floor, SerializableFloor floorToLoad)
        {
            Floor floorComponent = floor.GetComponent<Floor>();
            floorComponent.Id = floorToLoad.Id;
            floor.transform.localPosition = floorToLoad.LocalPosition;
        }

        private static void InstantiateFloor(out GameObject floor, GameObject ship)
        {
            floor = GameObject.Instantiate(
                DesignManager.Instance.FloorPrefab, ship.transform);
        }

        private static void LoadWalls(GameObject ship, SerializableShip shipToLoad)
        {
            foreach (SerializableWall wall in shipToLoad.Walls)
                LoadWall(ship, wall);
        }

        private static void LoadWall(GameObject ship, SerializableWall wallToLoad)
        {
            InstantiateWall(out GameObject wall, ship);
            SetWallParameters(wall, wallToLoad);
        }

        private static void SetWallParameters(GameObject wall, SerializableWall wallToLoad)
        {
            Wall wallComponent = wall.GetComponent<Wall>();
            wallComponent.Id = wallToLoad.Id;
            wall.transform.localPosition = wallToLoad.LocalPosition;
        }

        private static void InstantiateWall(out GameObject wall, GameObject ship)
        {
            wall = GameObject.Instantiate(
                DesignManager.Instance.WallPrefab, ship.transform);
        }

        private static void SetShipParameters(GameObject ship, SerializableShip shipToLoad)
        {
            Ship shipComponent = ship.GetComponent<Ship>();
            shipComponent.Id = shipToLoad.Id;
            shipComponent.Rooms = shipToLoad.Rooms;
            ship.transform.position = shipToLoad.Position;
            ship.transform.eulerAngles = new Vector3(0, 0, shipToLoad.Rotation);
            ship.GetComponent<Rigidbody2D>().velocity = shipToLoad.Velocity;
        }

        private static void InstantiateShip(out GameObject ship)
        {
            ship = GameObject.Instantiate(GameManager.Instance.ShipPrefab);
        }

        private static Save GetSaveFromJson(string saveJson)
        {
            return JsonUtility.FromJson<Save>(saveJson);
        }

        private static bool TryGetJsonFromFile(string name, out string saveJson)
        {
            saveJson = string.Empty;
            if (IsFileExisting(name))
            {
                saveJson = GetJsonFromFile(name);
                return true;
            }
            else
                return TryGetJsonFromLatestFile(ref saveJson);
        }

        private static bool TryGetJsonFromLatestFile(ref string saveJson)
        {
            var directory = GetDirectory();
            try
            {
                return TryFindAndReadLatestFile(ref saveJson, directory);
            }
            catch (InvalidOperationException e)
            {
                return IsGettingJsonSuccessfull(false, e);
            }
            catch (Exception e)
            {
                return IsGettingJsonSuccessfull(false, e);
            }
        }

        private static bool IsGettingJsonSuccessfull(bool isSuccessfull, Exception e = null)
        {
            if (isSuccessfull)
            {
                Debug.Log($"File succesfully read");
                return isSuccessfull;
            }
            else
            {
                Debug.Log($"Load failed, {e}");
                return isSuccessfull;
            }
        }

        private static bool TryFindAndReadLatestFile(ref string saveJson, DirectoryInfo directory)
        {
            var newestSave = directory.GetFiles()
                            .OrderByDescending(f => f.LastWriteTime)
                            .First();
            if (newestSave != null)
                saveJson = File.ReadAllText(newestSave.FullName);
            return IsGettingJsonSuccessfull(true);
        }

        private static DirectoryInfo GetDirectory()
        {
            return new DirectoryInfo(Application.persistentDataPath);
        }

        private static string GetJsonFromFile(string name)
        {
            return File.ReadAllText(
                                Path.Combine(Application.persistentDataPath, name));
        }

        private static bool IsFileExisting(string name)
        {
            return File.Exists(Path.Combine(Application.persistentDataPath, name));
        }

        private static void SaveToFile(string saveJson, string filePath)
        {
            File.WriteAllText(filePath, saveJson);
        }

        private static string CreateSavePath()
        {
            string path = Path.Combine(Application.persistentDataPath, "saveFile");
            int fileNumber = 0;
            string filePath = $"{path}{fileNumber}.txt";
            while (File.Exists(filePath))
            {
                fileNumber++;
                filePath = $"{path}{fileNumber}.txt";
            }

            return filePath;
        }

        private static string CreateSaveJson()
        {
            var save = CreateSave();
            return JsonUtility.ToJson(save, true);
        }

        private static Save CreateSave()
        {
            return new(GetShipsList());
        }

        private static List<Ship> GetShipsList()
        {
            List<Ship> ships = new();
            foreach (Transform child in GameManager.Instance.World.transform)
                if (child.gameObject.TryGetComponent(out Ship ship))
                    ships.Add(ship);
            UpdateShips(ships);
            return ships;
        }

        private static void UpdateShips(List<Ship> ships)
        {
            foreach (var ship in ships)
                ship.UpdateShip();
        }

        private static void ClearWorld()
        {
            var world = GameObject.Find("World");
            for (int i = 0; i < world.transform.childCount; i++)
            {
                GameObject.Destroy(world.transform.GetChild(i).gameObject);
            }
        }

        #endregion Private
    }
}