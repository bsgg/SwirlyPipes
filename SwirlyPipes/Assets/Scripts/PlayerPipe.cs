using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{
    public class PlayerPipe : MonoBehaviour
    {
        [SerializeField] private PipeSystem     m_PipeSystem;

        

        [SerializeField] private Pipe           m_CurrentPipe;
        private Transform                       m_PipeWorld;

        private bool m_MovePlayer = false;
        private  float                          m_DistanceTraveled;

        // Player Rotation
        private float                           m_DeltaToRotation;



        private float m_SystemRotation;
        // Rotation of the world pipe system
        private float m_PipeWorldRotation;

        [SerializeField]
        private float          m_LinearVelocity;
        // The rotation velocity is in angles per second. 
        // It should be quite fast. If it sets to 180, which means that it would take two seconds to go all the way around a pipe.
        [SerializeField]  private float m_RotationVelocity;

        [SerializeField]
        private Transform m_Avatar;
        [SerializeField]
        private float m_AvatarRotation;



        private int m_CurrentLane = 1;

        private int m_NextLane = 1;

        [SerializeField]
        private float m_ClampRotation = 50.0f;


        private void Start()
        {
            m_MovePlayer = false;

            InitPlayer();
        }

        public void InitPlayer()
        {
            m_PipeWorld = m_PipeSystem.transform.parent;
            // Gets the first pipe
            m_CurrentPipe = m_PipeSystem.GetFirstPipe();
            SetupCurrentPipe();          

            m_MovePlayer = true;   
        }
        
        private void Update()
        {
            if (m_MovePlayer)
            {
                // Linear velocity, the player doesn´t move it's the system pipe which moves
                float delta = m_LinearVelocity * Time.deltaTime;
                m_DistanceTraveled += delta;

                // Sets the rotation according to the rotation of the player
                m_SystemRotation += delta * m_DeltaToRotation;

               
                // Checks if player has passed the angle of the current curve
                if (m_SystemRotation >= m_CurrentPipe.CurveAngle)
                {
                    // Converts the delta distance and gets the next pipe
                    delta = (m_SystemRotation - m_CurrentPipe.CurveAngle) / m_DeltaToRotation;

                    //m_CurrentPipe.AlignRotationItems(-m_SystemRotation);
                    //m_CurrentPipe.AlignRotationItems(-m_PipeWorldRotation);

                    m_CurrentPipe = m_PipeSystem.SetNextPipe();
                    SetupCurrentPipe();
                    m_SystemRotation = delta * m_DeltaToRotation;

                    //m_CurrentPipe.AlignRotationItems(-m_PipeWorldRotation);

                }
                
                m_PipeSystem.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, m_SystemRotation);

                UpdateAvatar();
            }

        }

        private void SetupCurrentPipe()
        {
            // As we don't move in a straight line. We have to convert the delta into a rotation
            m_DeltaToRotation = 360.0f / (2.0f * Mathf.PI * m_CurrentPipe.CurveRadius);
            m_PipeWorldRotation += m_CurrentPipe.RelativeRotation;

            if (m_PipeWorldRotation < 0.0f)
            {
                m_PipeWorldRotation += 360.0f;
            }
            else if (m_PipeWorldRotation >= 360.0f)
            {
                m_PipeWorldRotation -= 360.0f;
            }

            m_PipeWorld.localRotation = Quaternion.Euler(m_PipeWorldRotation, 0f, 0f);

            // Correct pipe items
            //m_CurrentPipe.AlignRotationItems(-m_PipeWorldRotation);
            //m_PipeSystem.SetItemsNextPipe(-m_PipeWorldRotation);

        }
       

       
        private void UpdateAvatar()
        {
            // Input            
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {

                //m_Direction = -1;
                m_NextLane = m_CurrentLane - 1;
                m_NextLane = Mathf.Max(m_NextLane, 0);
                //m_IndexLane = Mathf.Max(m_IndexLane, 0);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {

                //m_Direction = 1;
                m_NextLane = m_CurrentLane + 1;
                m_NextLane = Mathf.Min(m_NextLane, 2);
            }

            // Midle lane
            if (m_CurrentLane == 1)
            {
                // Goto left
                if (m_NextLane == 0)
                {
                    if (m_AvatarRotation > (-m_ClampRotation))
                    {
                        m_AvatarRotation -= (m_RotationVelocity * Time.deltaTime);
                    }
                    else
                    {
                        m_AvatarRotation = -m_ClampRotation;
                        m_CurrentLane = m_NextLane;
                    }
                }

                // Goto right
                if (m_NextLane == 2)
                {
                    if (m_AvatarRotation < (m_ClampRotation))
                    {
                        m_AvatarRotation += (m_RotationVelocity * Time.deltaTime);
                    }
                    else
                    {
                        m_AvatarRotation = m_ClampRotation;
                        m_CurrentLane = m_NextLane;
                    }
                }

            }

            // Right lane
            if (m_CurrentLane == 2)
            {
                // Goto left
                if (m_NextLane == 1)
                {
                    if (m_AvatarRotation > 0.0f)
                    {
                        m_AvatarRotation -= (m_RotationVelocity * Time.deltaTime);
                    }
                    else
                    {
                        m_AvatarRotation = 0.0f;
                        m_CurrentLane = m_NextLane;
                    }
                }
            }

            // Left lane
            if (m_CurrentLane == 0)
            {
                // Goto Right
                if (m_NextLane == 1)
                {
                    if (m_AvatarRotation < 0.0f)
                    {
                        m_AvatarRotation += (m_RotationVelocity * Time.deltaTime);
                    }
                    else
                    {
                        m_AvatarRotation = 0.0f;
                        m_CurrentLane = m_NextLane;
                    }
                }
            }
            m_Avatar.localRotation = Quaternion.Euler(m_AvatarRotation, 0f, 0f);

        }

    }
}
