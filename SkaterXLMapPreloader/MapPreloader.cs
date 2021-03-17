using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using GameManagement;
using Harmony12;

namespace SkaterXLMapPreloader
{
    public class MapPreloader : MonoBehaviour
    {
        private string _configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SkaterXL\\mappreloader";

        void Start()
        {
            LevelManager.Instance.UpdateCustomMaps();

            EnsurePreloadConfig();
            ChangeMap(GetPreloadConfig());
        }

        private List<List<LevelInfo>> GetAllLevelRegisters()
        {
            return new List<List<LevelInfo>>
            {
                LevelManager.Instance.Levels,
                LevelManager.Instance.ModLevels,
                LevelManager.Instance.CommunityLevels
            };
        }

        private LevelInfo FindLevelByName(string name)
        {
            foreach(List<LevelInfo> register in GetAllLevelRegisters())
            {
                foreach(LevelInfo info in register)
                {
                    UnityModManager.Logger.Log($"[MapPreloader] found map called: {info.FullName} (short: {info.name})");

                    if (info.name == name)
                    {
                        return info;
                    }
                }
            }

            return null;
        }

        public IEnumerator LoadMap(LevelInfo levelInfo)
        {
            yield return new WaitWhile(() => GameStateMachine.Instance.IsLoading);
            if (!levelInfo.Equals(LevelManager.Instance.currentLevel))
            {
                LevelSelectionController levelSelectionController = GameStateMachine.Instance.LevelSelectionObject.GetComponentInChildren<LevelSelectionController>();

                IndexPath targetIndex = Traverse.Create(levelSelectionController).Method("GetIndexForLevel", levelInfo).GetValue<IndexPath>();
                Traverse.Create(levelSelectionController).Method("OnLevelHighlighted", targetIndex).GetValue();
                levelSelectionController.OnItemSelected(targetIndex);

                yield return new WaitWhile(() => GameStateMachine.Instance.IsLoading);

                PlayerController.Instance.respawn.ForceRespawn();
            }
            yield break;
        }

        private void SetPreloadConfig(string name)
        {
            StreamWriter configFile = File.CreateText(_configPath);
            configFile.Write(name);
            configFile.Close();
        }

        private string GetPreloadConfig()
        {
            return File.ReadAllText(_configPath);
        }

        private void EnsurePreloadConfig()
        {
            if (File.Exists(_configPath)) return;

            SetPreloadConfig(LevelManager.Instance.currentLevel.name);
        }

        private void ChangeMap(string name)
        {
            UnityModManager.Logger.Log($"[MapPreloader] attempting to load map: {name}");

            LevelInfo levelInfo = FindLevelByName(name);
            if (levelInfo == null)
            {
                UnityModManager.Logger.Log($"[MapPreloader] could not find map by name \"{name}\"; gracefully exiting");
                return;
            }

            this.StartCoroutine(LoadMap(levelInfo));
        }
    }
}
