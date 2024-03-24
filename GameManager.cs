using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HyperCasual.Runner
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance => s_Instance;
        static GameManager s_Instance;

    [SerializeField] AbstractGameEvent m_WinEvent;

    [SerializeField] AbstractGameEvent m_LoseEvent;

        LevelDefinition m_CurrentLevel;

        public bool IsPlaying => m_IsPlaying;
        bool m_IsPlaying;
        GameObject m_CurrentLevelGO;
        GameObject m_CurrentTerrainGO;
        GameObject m_LevelMarkersGO;

        List<Spawnable> m_ActiveSpawnables = new List<Spawnable>();

#if UNITY_EDITOR
        bool m_LevelEditorMode;
#endif

        void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;

#if UNITY_EDITOR
            if (LevelManager.Instance != null)
            {
                StartGame();
                m_LevelEditorMode = true;
            }
#endif
        }

        public void LoadLevel(LevelDefinition levelDefinition)
        {
            m_CurrentLevel = levelDefinition;
            LoadLevel(m_CurrentLevel, ref m_CurrentLevelGO);
            CreateTerrain(m_CurrentLevel, ref m_CurrentTerrainGO);
            PlaceLevelMarkers(m_CurrentLevel, ref m_LevelMarkersGO);
            StartGame();
        }
        public void ResetLevel()
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.ResetPlayer();
            }

            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.ResetCamera();
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.ResetSpawnables();
            }
        }
        public static void LoadLevel(LevelDefinition levelDefinition, ref GameObject levelGameObject)
        {
            if (levelDefinition == null)
            {
                Debug.LogError("Invalid Level!");
                return;
            }

            if (levelGameObject != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(levelGameObject);
                }
                else
                {
                    DestroyImmediate(levelGameObject);
                }
            }

            levelGameObject = new GameObject("LevelManager");
            LevelManager levelManager = levelGameObject.AddComponent<LevelManager>();
            levelManager.LevelDefinition = levelDefinition;

            Transform levelParent = levelGameObject.transform;

            for (int i = 0; i < levelDefinition.Spawnables.Length; i++)
            {
                LevelDefinition.SpawnableObject spawnableObject = levelDefinition.Spawnables[i];

                if (spawnableObject.SpawnablePrefab == null)
                {
                    continue;
                }

                Vector3 position = spawnableObject.Position;
                Vector3 eulerAngles = spawnableObject.EulerAngles;
                Vector3 scale = spawnableObject.Scale;

                GameObject go = null;
                
                if (Application.isPlaying)
                {
                    go = GameObject.Instantiate(spawnableObject.SpawnablePrefab, position, Quaternion.Euler(eulerAngles));
                }
                else
                {
#if UNITY_EDITOR
                    go = (GameObject)PrefabUtility.InstantiatePrefab(spawnableObject.SpawnablePrefab);
                    go.transform.position = position;
                    go.transform.eulerAngles = eulerAngles;
#endif
                }

                if (go == null)
                {
                    return;
                }

                Spawnable spawnable = go.GetComponent<Spawnable>();
                if (spawnable != null)
                {
                    spawnable.SetBaseColor(spawnableObject.BaseColor);
                    spawnable.SetScale(scale);
                    levelManager.AddSpawnable(spawnable);
                }

                if (go != null)
                {
                    go.transform.SetParent(levelParent);
                }
            }
        }

        public void UnloadCurrentLevel()
        {
            if (m_CurrentLevelGO != null)
            {
                GameObject.Destroy(m_CurrentLevelGO);
            }

            if (m_LevelMarkersGO != null)
            {
                GameObject.Destroy(m_LevelMarkersGO);
            }

            if (m_CurrentTerrainGO != null)
            {
                GameObject.Destroy(m_CurrentTerrainGO);
            }

            m_CurrentLevel = null;
        }

        void StartGame()
        {
            ResetLevel();
            m_IsPlaying = true;
        }

        public static void PlaceLevelMarkers(LevelDefinition levelDefinition, ref GameObject levelMarkersGameObject)
        {
            if (levelMarkersGameObject != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(levelMarkersGameObject);
                }
                else
                {
                    DestroyImmediate(levelMarkersGameObject);
                }
            }

            levelMarkersGameObject = new GameObject("Level Markers");

            GameObject start = levelDefinition.StartPrefab;
            GameObject end = levelDefinition.EndPrefab;

            if (start != null)
            {
                GameObject go = GameObject.Instantiate(start, new Vector3(start.transform.position.x, start.transform.position.y, 0.0f), Quaternion.identity);
                go.transform.SetParent(levelMarkersGameObject.transform);
            }

            if (end != null)
            {
                GameObject go = GameObject.Instantiate(end, new Vector3(end.transform.position.x, end.transform.position.y, levelDefinition.LevelLength), Quaternion.identity);
                go.transform.SetParent(levelMarkersGameObject.transform);
            }
        }
        public static void CreateTerrain(LevelDefinition levelDefinition, ref GameObject terrainGameObject)
        {
            TerrainGenerator.TerrainDimensions terrainDimensions = new TerrainGenerator.TerrainDimensions()
            {
                Width = levelDefinition.LevelWidth,
                Length = levelDefinition.LevelLength,
                StartBuffer = levelDefinition.LevelLengthBufferStart,
                EndBuffer = levelDefinition.LevelLengthBufferEnd,
                Thickness = levelDefinition.LevelThickness
            };
            TerrainGenerator.CreateTerrain(terrainDimensions, levelDefinition.TerrainMaterial, ref terrainGameObject);
        }

        public void Win()
        {
            m_WinEvent.Raise();

#if UNITY_EDITOR
            if (m_LevelEditorMode)
            {
                ResetLevel();
            }
#endif
        }

        public void Lose()
        {
            m_LoseEvent.Raise();

#if UNITY_EDITOR
            if (m_LevelEditorMode)
            {
                ResetLevel();
            }
#endif
        }
    }
}