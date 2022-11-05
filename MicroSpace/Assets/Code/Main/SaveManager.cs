using Ships;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Main
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
            List<Ship> ships = LoadShips(save.Ships);
            LoadIdManager(save.NextId);
            LoadFocusedShip(save.FocusedShipId, ships);
        }

        private static void LoadFocusedShip(int focusedShipId, List<Ship> ships)
        {
            GameManager.SelectFocusedShip(FindShipById(focusedShipId, ships));
        }

        private static GameObject FindShipById(int focusedShipId, List<Ship> ships)
        {
            return ships.Find(ship => ship.Id == focusedShipId).gameObject;
        }

        private static void LoadIdManager(int nextId)
        {
            GameManager.Instance.IdManager.NextId = nextId;
        }

        private static List<Ship> LoadShips(List<SerializableShip> shipsToLoad)
        {
            List<Ship> ships = new();
            foreach (SerializableShip shipToLoad in shipsToLoad)
                ships.Add(LoadShip(shipToLoad));
            return ships;
        }

        private static Ship LoadShip(SerializableShip shipToLoad)
        {
            InstantiateShip(out GameObject ship);
            SetShipParameters(ship, shipToLoad);
            LoadBlocks(ship, shipToLoad);
            UpdateShip(ship);
            return ship.GetComponent<Ship>();
        }

        private static void UpdateShip(GameObject ship)
        {
            ship.GetComponent<Ship>().UpdateShip();
        }

        private static void LoadBlocks(GameObject ship, SerializableShip shipToLoad)
        {
            foreach (SerializableBlock block in shipToLoad.Blocks)
                LoadBlock(ship, block);
        }

        private static void LoadBlock(GameObject ship, SerializableBlock blockToLoad)
        {
            InstantiateBlock(out GameObject block, ship, blockToLoad);
            SetBlockParameters(block, blockToLoad);
        }

        private static void SetBlockParameters(GameObject block, SerializableBlock blockToLoad)
        {
            Block blockComponent = block.GetComponent<Block>();
            blockComponent.Id = blockToLoad.Id;
            block.transform.localPosition = blockToLoad.LocalPosition;
        }

        private static void InstantiateBlock(out GameObject block,
            GameObject ship, SerializableBlock blockToLoad)
        {
            var designManager = DesignManager.Instance;
            GetBlockPrefab(blockToLoad, designManager, out GameObject blockPrefab);
            block = InstantiateBlockFromPrefab(blockPrefab, ship);
        }

        private static void GetBlockPrefab(SerializableBlock blockToLoad,
            DesignManager designManager, out GameObject blockPrefab)
        {
            blockPrefab = blockToLoad.BlockType switch
            {
                BlockType.Floor => designManager.FloorPrefab,
                BlockType.WallDesignation => designManager.WallDesignationPrefab,
                BlockType.FloorDesignation => designManager.FloorDesignationPrefab,
                _ => designManager.WallPrefab
            };
        }

        private static GameObject InstantiateBlockFromPrefab(
            GameObject blockPrefab, GameObject ship)
        {
            return GameObject.Instantiate(blockPrefab, ship.transform);
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
            ship = GameObject.Instantiate(
                GameManager.Instance.ShipPrefab, GameManager.Instance.World);
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
            GameManager.ForEachShip(ship => ships.Add(ship));
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
            var world = GameManager.Instance.World;
            for (int i = 0; i < world.transform.childCount; i++)
            {
                Transform child = world.GetChild(i);
                if (child.GetComponent<Ship>())
                    GameObject.Destroy(child.gameObject);
            }
        }

        #endregion Private
    }
}