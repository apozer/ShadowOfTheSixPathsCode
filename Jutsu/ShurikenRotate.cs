using System;
using UnityEngine;

namespace Jutsu
{
    public class ShurikenRotate : MonoBehaviour
    {
        void Update() {
            Vector3 rotation = transform.localEulerAngles;
            rotation.z += ( Time.deltaTime * 15f) * 90f;
            transform.localEulerAngles = rotation;
        }
        
    }
}