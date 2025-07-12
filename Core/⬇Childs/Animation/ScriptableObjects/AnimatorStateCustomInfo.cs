using System;
using UnityEngine;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Animation
{
    [CreateAssetMenu(fileName = "AnimatorStateInfo", menuName = "Scriptable Objects/Animator State Info")]
    public class AnimatorStateCustomInfo : ScriptableObject
    {
        [Header("Name of state in Animator")]
        [SerializeField] private string stateName;

        [SerializeField] private string layerName;
        [SerializeField] private int layerIndex;

        public string StateName => stateName;
        public string LayerName => layerName;
        public int LayerIndex => layerIndex;
        public bool IsInitialzied => !string.IsNullOrEmpty(stateName);

        /// <exception cref="InvalidOperationException"></exception>
        public void Construct(string stateName, string layerName, int layerIndex)
        {
            if (IsInitialzied)
            {
                throw new InvalidOperationException($"{this.GetTypeName()} already constructed.");
            }

            this.stateName = stateName;
            this.layerName = layerName;
            this.layerIndex = layerIndex;
        }
    }
}