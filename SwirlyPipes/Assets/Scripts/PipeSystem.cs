using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{
    public class PipeSystem : MonoBehaviour
    {
        public Pipe m_PipePrefab;

        public int m_PipeCount;

        private Pipe[] m_Pipes;

        

        private void Awake()
        {
            InitPipeSystem();
        }

        float relativeRotItems = 0.0f;
        private void InitPipeSystem()
        {
            m_Pipes = new Pipe[m_PipeCount];
            for (int i = 0; i < m_Pipes.Length; i++)
            {
                Pipe pipe = m_Pipes[i] = Instantiate<Pipe>(m_PipePrefab);
                pipe.transform.SetParent(transform, false);

                pipe.InitPipe();
                pipe.name = "Pipe_" + i.ToString();
                // Align each pipe according to the previous one
                if (i > 0)
                {
                    pipe.AlignWith(m_Pipes[i - 1]);

                    //relativeRotItems += m_Pipes[i - 1].RelativeRotation;
                    //Debug.Log("RELATIVE ROT " + relativeRotItems);
                    //m_Pipes[i - 1].AlignRotationItems(relativeRotItems);

                }

                
            }            
        }

        public Pipe GetFirstPipe()
        {
            AlignNextPipeWithOrigin();
            // To make sure that the pipe starts at the origin,
            // the whole system has to move down by an amount equal to that pipe's curve radius.
            transform.localPosition = new Vector3(0.0f, -m_Pipes[1].CurveRadius);
            return m_Pipes[1];
        }
        

        public Pipe SetNextPipe()
        {
            // Shift pipes to set next pipe as the first one
            ShiftPipes();
            AlignNextPipeWithOrigin();

            // Generate hte next pipe
            m_Pipes[m_Pipes.Length - 1].InitPipe();
            m_Pipes[m_Pipes.Length - 1].AlignWith(m_Pipes[m_Pipes.Length - 2]);


            // As with the first pipe, we need to locate the pipe in the origin accoding to the radius of the curve
            transform.localPosition = new Vector3(0.0f, -m_Pipes[1].CurveRadius);
            return m_Pipes[1];
        }


        /*public void SetItemsNextPipe(float rotation)
        {
            if (m_Pipes.Length >= 2)
            {
                m_Pipes[2].AlignRotationItems(rotation);
            }
        }*/

        private void ShiftPipes()
        {
            // Switch pipes
            Pipe temp = m_Pipes[0];
            for (int i = 1; i < m_Pipes.Length; i++)
            {
                m_Pipes[i - 1] = m_Pipes[i];
            }
            m_Pipes[m_Pipes.Length - 1] = temp;
        }

        private void AlignNextPipeWithOrigin()
        {
            // Aligning the new first pipe can be done by simply resetting its position and rotation. 
            // To make sure that all other pipes move along with it, just temporarily make them children of that pipe.
            Transform transformToAlign = m_Pipes[1].transform;
            //m_Pipes[1].AlignRotationItems(m_Pipes[1].transform.localRotation.eulerAngles.x);
            for (int i = 0; i < m_Pipes.Length; i++)
            {
                if (i != 1)
                {
                    m_Pipes[i].transform.SetParent(transformToAlign);                    
                }
            }

            transformToAlign.localPosition = Vector3.zero;
            transformToAlign.localRotation = Quaternion.identity;

            for (int i = 0; i < m_Pipes.Length; i++)
            {
                if (i != 1)
                {
                    m_Pipes[i].transform.SetParent(transform);
                }
                
            }
        }
    }
}
