using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{
    public class PipeChunkItem : MonoBehaviour
    {
        [SerializeField]
        private float m_Velocity;

        void Start()
        {
            Destroy(gameObject, 10.0f);
        }

        void Update()
        {
            transform.position += -transform.right * m_Velocity * Time.deltaTime;
        }
	
	}
}
