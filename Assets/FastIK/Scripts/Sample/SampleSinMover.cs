using UnityEngine;

namespace DitzelGames.FastIK
{
    public class SampleSinMover : MonoBehaviour
    {
        public Vector3 Dir;
        public Vector3 Start;

        private void Awake()
        {
            Start = transform.localPosition;
        }

        void Update()
        {
            //just move the object from a to b and back
            transform.localPosition = Start + Dir * Mathf.Sin(Time.timeSinceLevelLoad);
        }
    }
}
