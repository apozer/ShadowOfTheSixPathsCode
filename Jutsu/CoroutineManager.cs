using System.Collections;
using UnityEngine;

namespace Jutsu
{
    /**
     * Used to allow mod to call coroutine in non monobehavior class
     */
    public class CoroutineManager : MonoBehaviour
    {
        /*
         * CustomCoroutine
         * parameters - method (IEnumerator)
         */
        public void CustomCoroutine(IEnumerator method)
        {
            StartCoroutine(method);
        }
        
    }
}