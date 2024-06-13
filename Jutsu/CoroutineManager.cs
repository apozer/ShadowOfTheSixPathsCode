using System.Collections;
using UnityEngine;

namespace Jutsu
{
    public class CoroutineManager : MonoBehaviour
    {
        public CoroutineManager(){}

        public void CustomCoroutine(IEnumerator method)
        {
            StartCoroutine(method);
        }
        
    }
}