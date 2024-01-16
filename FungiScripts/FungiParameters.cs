using System;
using UnityEngine;

namespace FungiScripts
{
    public class FungiParameters : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1f)] private float _directOppositeChance = 0.3f;
        [SerializeField] [Range(0.0f, 1f)] private float _broadOppositeChance = 0.1f;
        [SerializeField] [Range(0.0f, 1f)] private float _explorationDirectOppositeChance = 0.9f;
        [SerializeField] [Range(0.0f, 1f)] private float _explorationBroadOppositeChance = 0.01f;
        [SerializeField] [Range(0.0f, 1f)] private float _maxNeighborBeforeStarvation = 0.3f;
        [SerializeField] [Range(0.0f, 1f)] private float _safeNeighbor = 0.0f;
        [SerializeField] [Range(0.0f, 1f)] private float _explorationMaxNeighborBeforeStarvation = 0.2f;
        [SerializeField] [Range(0.0f, 1f)] private float _explorationSafeNeighbor = 0.1f;

        private void Update()
        {
            directOppositeChance = _directOppositeChance;
            broadOppositeChance = _broadOppositeChance;
            explorationDirectOppositeChance = _explorationDirectOppositeChance;
            explorationBroadOppositeChance = _explorationBroadOppositeChance;
            maxNeighborBeforeStarvation = _maxNeighborBeforeStarvation;
            safeNeighbor = _safeNeighbor;
            explorationMaxNeighborBeforeStarvation = _explorationMaxNeighborBeforeStarvation;
            explorationSafeNeighbor = _explorationSafeNeighbor;
        }

        public static float directOppositeChance = 0.3f;
        public static float broadOppositeChance = 0.1f;
        public static float explorationDirectOppositeChance = 0.9f;
        public static float explorationBroadOppositeChance = 0.01f;
        public static float maxNeighborBeforeStarvation = 0.3f;
        public static float safeNeighbor = 0.0f;
        public static float explorationMaxNeighborBeforeStarvation = 0.2f;
        public static float explorationSafeNeighbor = 0.1f;
    }
}