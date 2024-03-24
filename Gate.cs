using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCasual.Runner
{
    public class Gate : Spawnable
    {
        const string k_PlayerTag = "Player";

    [SerializeField]GateType m_GateType;
    [SerializeField]float m_Value;
    [SerializeField]RectTransform m_Text;

        bool m_Applied;
        Vector3 m_TextInitialScale;

        enum GateType
        {
            ChangeSpeed,
            ChangeSize,
        }

        public override void SetScale(Vector3 scale)
        {
            if (m_Text != null)
            {
                float xFactor = Mathf.Min(scale.y / scale.x, 1.0f);
                float yFactor = Mathf.Min(scale.x / scale.y, 1.0f);
                m_Text.localScale = Vector3.Scale(m_TextInitialScale, new Vector3(xFactor, yFactor, 1.0f));

                m_Transform.localScale = scale;
            }
        }
        public override void ResetSpawnable()
        {
            m_Applied = false;
        }

        protected override void Awake()
        {
            base.Awake();

            if (m_Text != null)
            {
                m_TextInitialScale = m_Text.localScale;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag) && !m_Applied)
            {
                ActivateGate();
            }
        }

        void ActivateGate()
        {
            switch (m_GateType)
            {
                case GateType.ChangeSpeed:
                    PlayerController.Instance.AdjustSpeed(m_Value);
                break;

                case GateType.ChangeSize:
                    PlayerController.Instance.AdjustScale(m_Value);
                break;
            }

            m_Applied = true;
        }
    }
}