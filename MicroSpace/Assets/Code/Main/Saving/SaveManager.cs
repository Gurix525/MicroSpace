﻿using ScriptableObjects;
using Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ExtensionMethods;
using Miscellaneous;

namespace Main
{
    public class SaveManager : MonoBehaviour
    {
        #region Fields

        private static readonly string _savesFolderName = "Saves";

        #endregion Fields

        #region Properties

        public static SaveManager Instance { get; set; }

        private static string SavesFolderPath => Path.Combine(
            Application.persistentDataPath,
            _savesFolderName);

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
            GameObject navMesh = Instantiate(Prefabs.NavMesh);
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
            astronautComponent.SetId(astronautToLoad.Id);
            astronautComponent.SetParentId(astronautToLoad.ParentId);
            astronaut.transform.parent = satellites
                .Find(satellite => satellite.Id == astronautToLoad.ParentId)
                .transform;
            astronaut.transform.localPosition = astronautToLoad.LocalPosition;
            agentComponent.SetTarget(
                astronaut.transform.parent);
            agentComponent.SetObstacleRigidbody(
                astronaut.transform.parent.GetComponentUpInHierarchy<Rigidbody2D>());
        }

        private static void InstantiateAstronaut(out GameObject astronaut)
        {
            astronaut = Instantiate(Prefabs.Astronaut);
        }

        private static void LoadFocusedSatellite(int focusedSatelliteId, List<Satellite> satellites,
            out GameObject satellite)
        {
            satellite = FindSatelliteById(focusedSatelliteId, satellites);
            GameManager.SelectFocusedSatellite(satellite);
        }

        private static GameObject FindSatelliteById(int focusedSatelliteId, List<Satellite> satellites)
        {
            var satellite = satellites.Find(satellite => satellite.Id == focusedSatelliteId);
            if (satellite != null)
                return satellite.gameObject;
            return null;
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
            LoadBlocks(satellite, satelliteToLoad, out IEnumerable<Block> blocks);
            LoadGasses(satellite, satelliteToLoad, blocks);
            return satellite.GetComponent<Satellite>();
        }

        private static void LoadGasses(
            GameObject satellite,
            SerializableSatellite satelliteToLoad,
            IEnumerable<Block> blocks)
        {
            foreach (var gas in satelliteToLoad.Gasses)
            {
                var container = blocks
                    .Where(container => container.Id == gas.ContainerId)
                    .FirstOrDefault() as IGasContainer;
                container.Gasses.Add(gas.ModelId, gas.Amount);
            }
        }

        private static void LoadBlocks(
            GameObject satellite,
            SerializableSatellite satelliteToLoad,
            out IEnumerable<Block> blocks)
        {
            List<Block> newBlocks = new();
            foreach (SerializableBlock block in satelliteToLoad.Blocks)
                newBlocks.Add(LoadBlock(satellite, block));
            blocks = newBlocks;
        }

        private static Block LoadBlock(GameObject satellite, SerializableBlock blockToLoad)
        {
            InstantiateBlock(out GameObject block, satellite, blockToLoad);
            SetBlockParameters(block, blockToLoad, out Block blockComponent);
            LoadBlockModel(block);
            LoadShape(block);
            return blockComponent;
        }

        private static void LoadBlockModel(GameObject block)
        {
            var blockComponent = block.GetComponent<Block>();
            var model = BlockModel.GetModel(blockComponent.ModelId);
            blockComponent.gameObject.name = model.name;
        }

        private static void LoadShape(GameObject block)
        {
            var blockComponent = block.GetComponent<Block>();
            GameObject shape = Shape
                .GetShape(blockComponent.ShapeId).Prefab;
            if (blockComponent is Wall || blockComponent is WallDesignation)
            {
                if (blockComponent.ShapeId == 0)
                    Destroy(block.GetComponent<SpriteMask>());
                else
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

        private static void SetBlockParameters(
            GameObject block,
            SerializableBlock blockToLoad,
            out Block blockComponent)
        {
            blockComponent = block.GetComponent<Block>();
            blockComponent.SetId(blockToLoad.Id);
            blockComponent.ModelId = blockToLoad.ModelId;
            blockComponent.ShapeId = blockToLoad.ShapeId;
            block.transform.localPosition = blockToLoad.LocalPosition;
            block.transform.localEulerAngles = new(0, 0, blockToLoad.LocalRotation);
        }

        private static void InstantiateBlock(out GameObject block,
            GameObject satellite, SerializableBlock blockToLoad)
        {
            GetBlockPrefab(blockToLoad, out GameObject blockPrefab);
            block = InstantiateBlockFromPrefab(blockPrefab, satellite);
        }

        private static void GetBlockPrefab(
            SerializableBlock blockToLoad,
            out GameObject blockPrefab)
        {
            blockPrefab = blockToLoad.BlockType switch
            {
                BlockType.Floor => Prefabs.Floor,
                BlockType.WallDesignation => Prefabs.WallDesignation,
                BlockType.FloorDesignation => Prefabs.FloorDesignation,
                _ => Prefabs.Wall
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
            satelliteComponent.SetId(satelliteToLoad.Id);
            satellite.transform.position = satelliteToLoad.Position;
            satellite.transform.eulerAngles = new Vector3(0, 0, satelliteToLoad.Rotation);
            satellite.GetComponent<Rigidbody2D>().velocity = satelliteToLoad.Velocity;
        }

        private static void InstantiateSatellite(out GameObject satellite)
        {
            satellite = Instantiate(
                Prefabs.Satellite, References.World);
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
            var directory = GetSavesDirectory();
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

        private static DirectoryInfo GetSavesDirectory()
        {
            if (Directory.Exists(SavesFolderPath))
                return new DirectoryInfo(SavesFolderPath);
            return Directory.CreateDirectory(SavesFolderPath);
        }

        private static string GetJsonFromFile(string name)
        {
            return File.ReadAllText(
                                Path.Combine(SavesFolderPath, name));
        }

        private static bool IsFileExisting(string name)
        {
            return File.Exists(Path.Combine(SavesFolderPath, name));
        }

        private static void SaveToFile(string saveJson, string filePath)
        {
            File.WriteAllText(filePath, saveJson);
        }

        private static string CreateSavePath()
        {
            string path = Path.Combine(SavesFolderPath, "saveFile");
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
            return new();
        }

        private static void ClearWorld()
        {
            Camera.main.transform.parent = null;
            var world = References.World;
            for (int i = 0; i < world.childCount; i++)
            {
                Transform child = world.GetChild(i);
                if (child.GetComponent<Satellite>())
                    GameObject.Destroy(child.gameObject);
            }
            Astronaut.Astronauts.Clear();
            Satellite.Satellites.Clear();
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