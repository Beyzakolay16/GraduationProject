using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HyperCasual.Runner
{
    [ExecuteInEditMode]
    public class Spawnable : MonoBehaviour
    {
        protected Transform m_Transform;

        LevelDefinition m_LevelDefinition;
        Vector3 m_Position;
        Color m_BaseColor;
        bool m_SnappedThisFrame;
        float m_PreviousGridSize;

        MeshRenderer[] m_MeshRenderers;

        [SerializeField] bool m_SnapToGrid = true;
        public Vector3 SavedPosition => m_Position;

        public Color BaseColor => m_BaseColor;

        protected virtual void Awake()
        {
            m_Transform = transform;

            if (m_MeshRenderers == null || m_MeshRenderers.Length == 0)
            {
                m_MeshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            }

            if (m_MeshRenderers != null && m_MeshRenderers.Length > 0)
            {
                m_BaseColor = m_MeshRenderers[0].sharedMaterial.color;
            }

            if (LevelManager.Instance != null)
            {
#if UNITY_EDITOR
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(gameObject))
#endif
                m_Transform.SetParent(LevelManager.Instance.transform);
            }
        }

        public virtual void SetBaseColor(Color baseColor)
        {
            m_BaseColor = baseColor;

            if (m_MeshRenderers == null || m_MeshRenderers.Length == 0)
            {
                m_MeshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            }

            if (m_MeshRenderers != null)
            {
                for (int i = 0; i < m_MeshRenderers.Length; i++)
                {
                    MeshRenderer meshRenderer = m_MeshRenderers[i];

                    if (meshRenderer != null)
                    {
                        Material material = new Material(meshRenderer.sharedMaterial);
                        material.color = m_BaseColor;
                        meshRenderer.sharedMaterial = material;
                    }
                }
            }
        }

        
        public virtual void SetScale(Vector3 scale)
        {
           m_Transform.localScale = scale;
        }

        
        public void SetLevelDefinition(LevelDefinition levelDefinition)
        {
            if (levelDefinition == null)
            {
                return;
            }

            m_LevelDefinition = levelDefinition;
        }

       
        public virtual void ResetSpawnable() {}

        protected virtual void OnEnable()
        {
            m_Transform = transform;
            m_Position = m_Transform.position;
            m_Transform.hasChanged = false;

            if (LevelManager.Instance != null && !Application.isPlaying)
            {
                SetLevelDefinition(LevelManager.Instance.LevelDefinition);
                SnapToGrid();
            }
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying && m_LevelDefinition != null)
            {
                if (m_Transform.hasChanged)
                {
                    m_Position = m_Transform.position;
                    m_Transform.hasChanged = false;

                    if (m_LevelDefinition.SnapToGrid)
                    {
                        SnapToGrid();
                    }

                    SetScale(m_Transform.localScale);
                }
                else if (m_PreviousGridSize != m_LevelDefinition.GridSize)
                {
                    SnapToGrid();
                }
            }
        }

        protected virtual void SnapToGrid()
        {
            if (!m_SnapToGrid || m_LevelDefinition == null)
            {
                return;
            }

            Vector3 position = m_Position;

            position.x = m_LevelDefinition.GridSize * Mathf.Round(position.x/m_LevelDefinition.GridSize);
            position.z = m_LevelDefinition.GridSize * Mathf.Round(position.z/m_LevelDefinition.GridSize);

            m_Transform.position = position;

            m_PreviousGridSize = m_LevelDefinition.GridSize;

            m_Transform.hasChanged = false;
        }
    }
}