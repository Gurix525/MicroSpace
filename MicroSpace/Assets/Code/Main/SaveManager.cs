﻿using ScriptableObjects;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ExtensionMethods;
using UnityEngine.AI;

namespace Main
{
    public class SaveManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private BlockModelListScriptableObject _blockModels;

        [SerializeField]
        private ShapeListScriptableObject _blockShapes;

        #endregion Fields

        #region Properties

        public static SaveManager Instance { get; set; }

        #endregion Properties

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
            List<Satellite> satellites = LoadSatellites(save.Satellites);
            LoadFocusedSatellite(
                save.FocusedSatelliteId,
                satellites,
                out GameObject focusedSatellite);
            LoadNavMesh(focusedSatellite);
            List<Astronaut> astronauts = LoadAstronauts(
                save.Astronauts,
                satellites);
            LoadIdManager(save.NextId);
        }

        private static void LoadNavMesh(GameObject focusedSatellite)
        {
            GameObject navMesh = Instantiate(GameManager.Instance.NavMeshPrefab);
            navMesh.transform.parent = focusedSatellite.transform;
            navMesh.transform.localPosition = Vector3.zero;
        }

        private static List<Astronaut> LoadAstronauts(
            List<SerializableAstronaut> astronautsToLoad,
            List<Satellite> satellites)
        {
            List<Astronaut> astronauts = new();
            foreach (SerializableAstronaut astronautToLoad in astronautsToLoad)
                astronauts.Add(LoadAstronaut(astronautToLoad, satellites));
            return astronauts;
        }

        private static Astronaut LoadAstronaut(SerializableAstronaut astronautToLoad,
            List<Satellite> satellites)
        {
            InstantiateAstronaut(out GameObject astronaut);
            SetAstronautParameters(astronaut, astronautToLoad, satellites);
            return astronaut.GetComponent<Astronaut>();
        }

        private static void SetAstronautParameters(GameObject astronaut, SerializableAstronaut astronautToLoad, List<Satellite> satellites)
        {
            var astronautComponent = astronaut.GetComponent<Astronaut>();
            var agentComponent = astronaut.GetComponent<Agent>();
            var navMeshAgentComponent = astronaut.GetComponent<NavMeshAgent>();
            astronautComponent.SetId(astronautToLoad.Id);
            astronautComponent.SetParentId(astronautToLoad.ParentId);
            astronaut.transform.parent = satellites
                .Find(satellite => satellite.Id == astronautToLoad.ParentId)
                .transform;
            astronaut.transform.localPosition = astronautToLoad.LocalPosition;
            agentComponent.SetObstacleRigidbody(
                astronaut.GetComponentUpInHierarchy<Rigidbody2D>());
            navMeshAgentComponent.enabled = true;
        }

        private static void InstantiateAstronaut(out GameObject astronaut)
        {
            astronaut = Instantiate(GameManager.Instance.AstronautPrefab);
        }

        private static void LoadFocusedSatellite(int focusedSatelliteId, List<Satellite> satellites,
            out GameObject satellite)
        {
            satellite = FindSatelliteById(focusedSatelliteId, satellites);
            GameManager.SelectFocusedSatellite(satellite);
        }

        private static GameObject FindSatelliteById(int focusedSatelliteId, List<Satellite> satellites)
        {
            return satellites.Find(satellite => satellite.Id == focusedSatelliteId).gameObject;
        }

        private static void LoadIdManager(int nextId)
        {
            IdManager.NextId = nextId;
        }

        private static List<Satellite> LoadSatellites(List<SerializableSatellite> satellitesToLoad)
        {
            List<Satellite> satellites = new();
            foreach (SerializableSatellite satelliteToLoad in satellitesToLoad)
                satellites.Add(LoadSatellite(satelliteToLoad));
            return satellites;
        }

        private static Satellite LoadSatellite(SerializableSatellite satelliteToLoad)
        {
            InstantiateSatellite(out GameObject satellite);
            SetSatelliteParameters(satellite, satelliteToLoad);
            LoadBlocks(satellite, satelliteToLoad);
            UpdateSatellite(satellite);
            return satellite.GetComponent<Satellite>();
        }

        private static void UpdateSatellite(GameObject satellite)
        {
            satellite.GetComponent<Satellite>().UpdateSatellite();
        }

        private static void LoadBlocks(GameObject satellite, SerializableSatellite satelliteToLoad)
        {
            foreach (SerializableBlock block in satelliteToLoad.Blocks)
                LoadBlock(satellite, block);
        }

        private static void LoadBlock(GameObject satellite, SerializableBlock blockToLoad)
        {
            InstantiateBlock(out GameObject block, satellite, blockToLoad);
            SetBlockParameters(block, blockToLoad);
            LoadBlockModel(block);
            LoadShape(block);
        }

        private static void LoadBlockModel(GameObject block)
        {
            var blockComponent = block.GetComponent<Block>();
            var model = Instance._blockModels.GetModel(blockComponent.ModelId);
            blockComponent.gameObject.name = model.name;
            blockComponent.GetComponent<SpriteRenderer>().sprite = model.Sprite;
        }

        private static void LoadShape(GameObject block)
        {
            var blockComponent = block.GetComponent<Block>();
            GameObject shape = Instance._blockShapes
                .GetShape(blockComponent.ShapeId).Prefab;
            if (blockComponent is Wall || blockComponent is WallDesignation)
            {
                block.GetComponent<SpriteMask>().sprite =
                    shape.GetComponent<SpriteMask>().sprite;
            }
            if (blockComponent is Wall)
            {
                var blockCollider = block.GetComponent<PolygonCollider2D>();
                var shapeCollider = shape.GetComponent<PolygonCollider2D>();
                for (int i = 0; i < shapeCollider.pathCount; i++)
                    blockCollider.SetPath(i, shapeCollider.GetPath(i));
            }
        }

        private static void SetBlockParameters(GameObject block, SerializableBlock blockToLoad)
        {
            Block blockComponent = block.GetComponent<Block>();
            blockComponent.Id = blockToLoad.Id;
            blockComponent.ModelId = blockToLoad.ModelId;
            blockComponent.ShapeId = blockToLoad.ShapeId;
            block.transform.localPosition = blockToLoad.LocalPosition;
            block.transform.localEulerAngles = new(0, 0, blockToLoad.LocalRotation);
        }

        private static void InstantiateBlock(out GameObject block,
            GameObject satellite, SerializableBlock blockToLoad)
        {
            var buildingManager = BuildingManager.Instance;
            GetBlockPrefab(blockToLoad, buildingManager, out GameObject blockPrefab);
            block = InstantiateBlockFromPrefab(blockPrefab, satellite);
        }

        private static void GetBlockPrefab(SerializableBlock blockToLoad,
            BuildingManager buildingManager, out GameObject blockPrefab)
        {
            blockPrefab = blockToLoad.BlockType switch
            {
                BlockType.Floor => buildingManager.FloorPrefab,
                BlockType.WallDesignation => buildingManager.WallDesignationPrefab,
                BlockType.FloorDesignation => buildingManager.FloorDesignationPrefab,
                _ => buildingManager.WallPrefab
            };
        }

        private static GameObject InstantiateBlockFromPrefab(
            GameObject blockPrefab, GameObject satellite)
        {
            return GameObject.Instantiate(blockPrefab, satellite.transform);
        }

        private static void SetSatelliteParameters(GameObject satellite, SerializableSatellite satelliteToLoad)
        {
            Satellite satelliteComponent = satellite.GetComponent<Satellite>();
            satelliteComponent.Id = satelliteToLoad.Id;
            satellite.transform.position = satelliteToLoad.Position;
            satellite.transform.eulerAngles = new Vector3(0, 0, satelliteToLoad.Rotation);
            satellite.GetComponent<Rigidbody2D>().velocity = satelliteToLoad.Velocity;
        }

        private static void InstantiateSatellite(out GameObject satellite)
        {
            satellite = GameObject.Instantiate(
                GameManager.Instance.SatellitePrefab, GameManager.Instance.World);
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
            return new(GetSatellitesList());
        }

        private static List<Satellite> GetSatellitesList()
        {
            List<Satellite> satellites = new();
            GameManager.ForEachSatellite(satellite => satellites.Add(satellite));
            UpdateSatellites(satellites);
            return satellites;
        }

        private static void UpdateSatellites(List<Satellite> satellites)
        {
            foreach (var satellite in satellites)
                satellite.UpdateSatellite();
        }

        private static void ClearWorld()
        {
            Camera.main.transform.parent = null;
            var world = GameManager.Instance.World;
            for (int i = 0; i < world.transform.childCount; i++)
            {
                Transform child = world.GetChild(i);
                if (child.GetComponent<Satellite>())
                    GameObject.Destroy(child.gameObject);
            }
            Astronaut.Astronauts.Clear();
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            Instance = this;
        }

        #endregion Unity
    }
}