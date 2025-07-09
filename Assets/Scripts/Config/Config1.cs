using System;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "Config", menuName = "Config", order = 0)]
    public class Config1 : ScriptableObject
    {
        [Serializable]
        public class Item
        {
            public string Name;
        }
        
        public int Value;
        public float Modifier;
        public Item[] Items;
    }
}