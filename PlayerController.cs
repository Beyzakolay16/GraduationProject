using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance => s_Instance;
        static PlayerController s_Instance;

     [SerializeField]Animator m_Animator;

     [SerializeField] SkinnedMeshRenderer m_SkinnedMeshRenderer;

     [SerializeField] PlayerSpeedPreset m_PlayerSpeed = PlayerSpeedPreset.Medium;

     [SerializeField]float m_CustomPlayerSpeed = 10.0f;

     [SerializeField]float m_AccelerationSpeed = 10.0f;

     [SerializeField]float m_DecelerationSpeed = 20.0f;

     [SerializeField]float m_HorizontalSpeedFactor = 0.5f;

     [SerializeField]float m_ScaleVelocity = 2.0f;

     [SerializeField]bool m_AutoMoveForward = true;

        Vector3 m_LastPosition;
        float m_StartHeight;

        const float k_MinimumScale = 0.1f;
        static readonly string s_Speed = "Speed";

        enum PlayerSpeedPreset
        {
            Slow,
            Medium,
            Fast,
            Custom
        }

        Transform m_Transform;
        Vector3 m_StartPosition;
        bool m_HasInput;
        float m_MaxXPosition;
        float m_XPos;
        float m_ZPos;
        float m_TargetPosition;
        float m_Speed;
        float m_TargetSpeed;
        Vector3 m_Scale;
        Vector3 m_TargetScale;
        Vector3 m_DefaultScale;

        const float k_HalfWidth = 0.5f;
        public Transform Transform => m_Transform;
        public float Speed => m_Speed;
        public float TargetSpeed => m_TargetSpeed;
        public float MinimumScale => k_MinimumScale;
        public Vector3 Scale => m_Scale;
        public Vector3 TargetScale => m_TargetScale;
        public Vector3 DefaultScale => m_DefaultScale;
        public float StartHeight => m_StartHeight;
        public float TargetPosition => m_TargetPosition;
        public float MaxXPosition => m_MaxXPosition;

        void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;

            Initialize();
        }
        public void Initialize()
        {
            m_Transform = transform;
            m_StartPosition = m_Transform.position;
            m_DefaultScale = m_Transform.localScale;
            m_Scale = m_DefaultScale;
            m_TargetScale = m_Scale;

            if (m_SkinnedMeshRenderer != null)
            {
                m_StartHeight = m_SkinnedMeshRenderer.bounds.size.y;
            }
            else 
            {
                m_StartHeight = 1.0f;
            }

            ResetSpeed();
        }
        public float GetDefaultSpeed()
        {
            switch (m_PlayerSpeed)
            {
                case PlayerSpeedPreset.Slow:
                    return 5.0f;

                case PlayerSpeedPreset.Medium:
                    return 10.0f;

                case PlayerSpeedPreset.Fast:
                    return 20.0f;
            }

            return m_CustomPlayerSpeed;
        }

        public void AdjustSpeed(float speed)
        {
            m_TargetSpeed += speed;
            m_TargetSpeed = Mathf.Max(0.0f, m_TargetSpeed);
        }

        public void ResetSpeed()
        {
            m_Speed = 0.0f;
            m_TargetSpeed = GetDefaultSpeed();
        }

        public void AdjustScale(float scale)
        {
            m_TargetScale += Vector3.one * scale;
            m_TargetScale = Vector3.Max(m_TargetScale, Vector3.one * k_MinimumScale);
        }

        public void ResetScale()
        {
            m_Scale = m_DefaultScale;
            m_TargetScale = m_DefaultScale;
        }
        public Vector3 GetPlayerTop()
        {
            return m_Transform.position + Vector3.up * (m_StartHeight * m_Scale.y - m_StartHeight);
        }
        public void SetDeltaPosition(float normalizedDeltaPosition)
        {
            if (m_MaxXPosition == 0.0f)
            {
                Debug.LogError("Player cannot move because SetMaxXPosition has never been called or Level Width is 0. If you are in the LevelEditor scene, ensure a level has been loaded in the LevelEditor Window!");
            }

            float fullWidth = m_MaxXPosition * 2.0f;
            m_TargetPosition = m_TargetPosition + fullWidth * normalizedDeltaPosition;
            m_TargetPosition = Mathf.Clamp(m_TargetPosition, -m_MaxXPosition, m_MaxXPosition);
            m_HasInput = true;
        }


        public void CancelMovement()
        {
            m_HasInput = false;
        }

        public void SetMaxXPosition(float levelWidth)
        {
            m_MaxXPosition = levelWidth * k_HalfWidth;
        }

        public void ResetPlayer()
        {
            m_Transform.position = m_StartPosition;
            m_XPos = 0.0f;
            m_ZPos = m_StartPosition.z;
            m_TargetPosition = 0.0f;

            m_LastPosition = m_Transform.position;

            m_HasInput = false;

            ResetSpeed();
            ResetScale();
        }

        public void MoveLeft()
        {
            SetDeltaPosition(-1.0f);
        }

        public void MoveRight()
        {
            SetDeltaPosition(1.0f);
        }


        void Update()
        {
            float deltaTime = Time.deltaTime;

            if (!Approximately(m_Transform.localScale, m_TargetScale))
            {
                m_Scale = Vector3.Lerp(m_Scale, m_TargetScale, deltaTime * m_ScaleVelocity);
                m_Transform.localScale = m_Scale;
            }

            if (!m_AutoMoveForward && !m_HasInput)
            {
                Decelerate(deltaTime, 0.0f);
            }
            else if (m_TargetSpeed < m_Speed)
            {
                Decelerate(deltaTime, m_TargetSpeed);
            }
            else if (m_TargetSpeed > m_Speed)
            {
                Accelerate(deltaTime, m_TargetSpeed);
            }

            float speed = m_Speed * deltaTime;

            m_ZPos += speed;

            if (m_HasInput)
            {
                float horizontalSpeed = speed * m_HorizontalSpeedFactor;

                float newPositionTarget = Mathf.Lerp(m_XPos, m_TargetPosition, horizontalSpeed);
                float newPositionDifference = newPositionTarget - m_XPos;

                newPositionDifference = Mathf.Clamp(newPositionDifference, -horizontalSpeed, horizontalSpeed);

                m_XPos += newPositionDifference;
            }
            
            m_Transform.position = new Vector3(m_XPos, m_Transform.position.y, m_ZPos);

            if (m_Animator != null && deltaTime > 0.0f)
            {
                float distanceTravelledSinceLastFrame = (m_Transform.position - m_LastPosition).magnitude;
                float distancePerSecond = distanceTravelledSinceLastFrame / deltaTime;

                m_Animator.SetFloat(s_Speed, distancePerSecond);
            }

            if (m_Transform.position != m_LastPosition)
            {
                m_Transform.forward = Vector3.Lerp(m_Transform.forward, (m_Transform.position - m_LastPosition).normalized, speed);
            }

            m_LastPosition = m_Transform.position;
        }

        void Accelerate(float deltaTime, float targetSpeed)
        {
            m_Speed += deltaTime * m_AccelerationSpeed;
            m_Speed = Mathf.Min(m_Speed, targetSpeed);
        }

        void Decelerate(float deltaTime, float targetSpeed)
        {
            m_Speed -= deltaTime * m_DecelerationSpeed;
            m_Speed = Mathf.Max(m_Speed, targetSpeed);
        }

        bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y) && Mathf.Approximately(a.z, b.z);
        }
    }
}