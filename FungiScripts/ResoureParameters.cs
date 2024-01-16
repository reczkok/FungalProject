using UnityEngine;

namespace FungiScripts
{
    public class ResourceParameters : MonoBehaviour
    {
        [SerializeField] [Range(1, 50)] private int _concentrations = 10;
        [SerializeField] [Range(0.0f, 500f)] private float _maxResourceAmount = 10f;
        [SerializeField] [Range(1, 20)] private int _maxRadius = 10;
        [SerializeField] [Range(10, 500)] private int _xMax = 100;
        [SerializeField] [Range(10, 500)] private int _yMax = 100;
        
        private void Awake()
        {
            concentrations = _concentrations;
            maxResourceAmount = _maxResourceAmount;
            maxRadius = _maxRadius;
            xMax = _xMax;
            yMax = _yMax;
        }
        
        public static int concentrations = 10;
        public static float maxResourceAmount = 1f;
        public static int maxRadius = 10; 
        public static int xMax = 100;
        public static int yMax = 100;
    }
}

