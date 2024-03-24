using System.Collections;
using System.Collections.Generic;
using HyperCasual.Core;
using UnityEngine;

namespace HyperCasual.Runner
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider))]
    public class Obstacle : Spawnable
    {
        const string k_PlayerTag = "Player";
        
        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(k_PlayerTag))
            {
                GameManager.Instance.Lose();
            }
        }
    }
}