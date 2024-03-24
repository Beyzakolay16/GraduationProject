using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Runner
{
    [ExecuteInEditMode]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance => s_Instance;
        static LevelManager s_Instance;

        public LevelDefinition LevelDefinition
        {
            get => m_LevelDefinition;
            set 
            {
                m_LevelDefinition = value;

                if (m_LevelDefinition != null && PlayerController.Instance != null)
                {
                    PlayerController.Instance.SetMaxXPosition(m_LevelDefinition.LevelWidth);
                }
            }
        }
        LevelDefinition m_LevelDefinition;

        List<Spawnable> m_ActiveSpawnables = new List<Spawnable>();

        public void AddSpawnable(Spawnable spawnable)
        {
            m_ActiveSpawnables.Add(spawnable);
        }

        public void ResetSpawnables()
        {
            for (int i = 0, c = m_ActiveSpawnables.Count; i < c; i++)
            {
                m_ActiveSpawnables[i].ResetSpawnable();
            }
        }

        void Awake()
        {
            SetupInstance();
        }

        void OnEnable()
        {
            SetupInstance();
        }

        void SetupInstance()
        {
            if (s_Instance != null && s_Instance != this)
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
                return;
            }

            s_Instance = this;
        }
    }
}
