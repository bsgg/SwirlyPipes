using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{
    public class PipeItem : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Rotator;

        [SerializeField]
        private Transform m_Avatar;

        private float m_CurrentAvatarRotation = 0;

        private float ringRotation;


        void Update()
        {
            m_Rotator.rotation = Quaternion.Euler(ringRotation, 0, 0);
            //m_Rotator.GetChild(0).rotation = m_Rotator.transform.parent.rotation;
            //m_Avatar.rotation = transform.rotation;
        }

        public void Init(Pipe pipe, float itemZPosition, float ringRotation)
        {
            this.ringRotation = ringRotation;
            //ringRotation = 0;

            //m_Avatar.rotation = Quaternion.Euler(0, 0, ringRotation);


            transform.SetParent(pipe.transform, false);

            // Location for the item (convert the rotation into the position)
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -itemZPosition);

            // Avatar position on the floor of the curve
            m_Rotator.localPosition = new Vector3(0.0f, pipe.CurveRadius, 0.0f);
            
            m_CurrentAvatarRotation = ringRotation;

            //m_Avatar.localRotation = Quaternion.identity;
            //m_Avatar.GetChild(0).rotation = m_Avatar.transform.rotation;
            //m_Avatar.rotation = Quaternion.Euler(0, m_Avatar.rotation.eulerAngles.y, m_Avatar.rotation.eulerAngles.z);
            //m_Avatar.rotation = Quaternion.Euler(0, 0, 0);
        }

        public void SetItemRotation(float xRotation)
        {
            //m_Avatar.rotation = Quaternion.Euler(0, 0, 0);

            //m_CurrentAvatarRotation += xRotation;
            //m_Avatar.localRotation = Quaternion.Euler(m_CurrentAvatarRotation, 0.0f, 0.0f);
            //m_Avatar.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        }
    }
}
