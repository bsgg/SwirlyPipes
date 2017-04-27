using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{ 
    [System.Serializable]
    public class RangeF
    {
        public float Min;
        public float Max;
    }

    [System.Serializable]
    public class RangeI
    {
        public int Min;
        public int Max;
    }

    public class Pipe : MonoBehaviour
    {
        [Header("Pipe Settings")]
        [SerializeField]
        private bool                        m_DrawGizmos;

        [Header("Pipe rings")]
        [Tooltip("Lenght of the pipe (the segment from a completely round torus)")]
        [SerializeField]
        private float                       m_PipeRadius;

        [Tooltip("Number of segments for the pipe  (each ring of the torus)")]
        [SerializeField]
        private int                         m_PipeSegmentCount;
        public int PipeSegmentCount
        {
            get { return m_PipeSegmentCount; }
        }

        [Tooltip("Amount of torus conver by this pipe (lenght of the pipe)")]
        [SerializeField]
        private  float                      m_RingDistance;


        [Header("Curves Segments")]
        [Tooltip("Torus's Thickness")]
        [SerializeField] private RangeF     m_RangeCurveRadius;
        private float                       m_CurveRadius;
        public float CurveRadius
        {
            get { return m_CurveRadius; }
        }


        [Tooltip("Number of segments along the torus (number of rings in the torus)")]
        [SerializeField] private RangeI     m_RangeCurveSegmentCount;
        private int                         m_CurveSegmentCount;
        public int CurveSegmentCount    
        {
            get { return m_CurveSegmentCount; }
        }

        private float                       m_RelativeRotation;

        public float RelativeRotation
        {
            get { return m_RelativeRotation; }
        }

        /// <summary>
        /// Angle of the curve
        /// </summary>
        private float                                   m_CurveAngle;
        public float CurveAngle
        {
            get { return m_CurveAngle; }
        }

        [Header("Item generation")]
        [SerializeField] private PipeItemGenerator[]    m_ItemGenerators; 

        


        // Mesh properties
        private Mesh                    m_Mesh;
        private Vector3[]               m_Vertices;
        private int[]                   m_Triangles;
        private Vector2[]               m_UV;

        void Awake()
        {
            // Set the mesh for the pipe
            m_Mesh = new Mesh();
            m_Mesh.name = "Pipe";
            GetComponent<MeshFilter>().mesh = m_Mesh;

           
        }

        /// <summary>
        /// Generates a pipe
        /// </summary>
        public void InitPipe()
        {
            // Randomize curve radius and curve segments
            m_CurveRadius = Random.Range(m_RangeCurveRadius.Min, m_RangeCurveRadius.Max);
            m_CurveSegmentCount = Random.Range(m_RangeCurveSegmentCount.Min, m_RangeCurveSegmentCount.Max + 1);


            m_Mesh.Clear();
            SetVertices();
            SetUV();
            SetTriangles();
            m_Mesh.RecalculateNormals();

            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            // Randomize the item generator to include a different each time
            if (m_ItemGenerators != null && m_ItemGenerators.Length > 0)
            {
                m_ItemGenerators[Random.Range(0, m_ItemGenerators.Length)].GenerateItems(this);
            }
        }

        

        /// <summary>
        /// Get a point in torus according to a angle in the curve and angle in the pipe
        /// </summary>
        /// <param name="angleAlongCurve">Angle along the curve of the torus, in radians, between 0 - 2PI radians</param>
        /// <param name="angleAlongPipe">Angle along the pipe</param>
        /// <returns></returns>
        private Vector3 GetPointOnTorus(float angleAlongCurve, float angleAlongPipe)
        {
            Vector3 p;
            float r = (m_CurveRadius + m_PipeRadius * Mathf.Cos(angleAlongPipe));
            p.x = r * Mathf.Sin(angleAlongCurve);
            p.y = r * Mathf.Cos(angleAlongCurve);
            p.z = m_PipeRadius * Mathf.Sin(angleAlongPipe);
            return p;
        }

        public void AlignWith(Pipe other)
        {
            // Random rotation, restrict the relative rotation to fit the pipe segments
            m_RelativeRotation = Random.Range(0, m_CurveSegmentCount) * 360f / m_PipeSegmentCount;            

            // Change the parent of the current pipe to a child of the other, this way we avoid overlaping between pipes
            transform.SetParent(other.transform, false);
            // we move up so our origin sits at the end point of our parent's pipe
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -other.m_CurveAngle);            


            transform.Translate(0.0f, other.m_CurveRadius, 0.0f);
            transform.Rotate(m_RelativeRotation, 0.0f, 0.0f);
            transform.Translate(0.0f, -m_CurveRadius, 0.0f);            

            // Sets the parent to original one
            transform.SetParent(other.transform.parent);

            transform.localScale = Vector3.one;
        }

        /*/// <summary>
        /// Align the items rotation in the xRotation (ring rotation of the item)
        /// </summary>
        /// <param name="xRotation"></param>
        public void AlignRotationItems(float xRotation)
        {
            for (int iChild = 0; iChild < transform.childCount; iChild++)
            {
                PipeItem item = transform.GetChild(iChild).GetComponent<PipeItem>();

                item.SetItemRotation(xRotation);
            }
        }*/


        #region MeshGeneration
        private void SetVertices()
        {
            // We'll fill the torus with quads, quad 4 vertices, we have to multiply those 4 vertices with the rest
            // to get the total of vertices
            int numberVertices = m_CurveSegmentCount * m_PipeSegmentCount * 4;
            m_Vertices = new Vector3[numberVertices];

            //float uStep = (2f * Mathf.PI) / m_CurveSegmentCount;
            // If m_RingDistance is (2f * Mathf.PI), we´ll have a complete torus, with a fixed value we'll have portion of the torus
            float uStep = m_RingDistance / m_CurveRadius;
            // Sets the angle of the curve for this pipe
            m_CurveAngle = uStep * m_CurveSegmentCount * (360f / (2f * Mathf.PI));
            CreateFirstQuadRing(uStep);

            int iDelta = m_PipeSegmentCount * 4;
            for (int u = 2, i = iDelta; u <= m_CurveSegmentCount; u++, i += iDelta)
            {
                CreateQuadRing(u * uStep, i);
            }
            m_Mesh.vertices = m_Vertices;
        }


        private void CreateFirstQuadRing(float u)
        {
            // vStep = angle between each segment in the pipe (2P = 360 degrees)
            float vStep = (2.0f * Mathf.PI) / m_PipeSegmentCount;

            // Fill the vertices taking points from segments and the pipes
            // The first step is to get two vertices – A and B – along u. 
            // Then we do one step along v and grab the next pair. We keep doing this until we've come full circle
            Vector3 vertexA = GetPointOnTorus(0f, 0f);
            Vector3 vertexB = GetPointOnTorus(u, 0f);
            for (int v = 1, i = 0; v <= m_PipeSegmentCount; v++, i += 4)
            {
                m_Vertices[i] = vertexA;
                m_Vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
                m_Vertices[i + 2] = vertexB;
                m_Vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
            }
        }

        private void CreateQuadRing(float u, int i)
        {
            float vStep = (2f * Mathf.PI) / m_PipeSegmentCount;
            int ringOffset = m_PipeSegmentCount * 4;

            Vector3 vertex = GetPointOnTorus(u, 0f);
            for (int v = 1; v <= m_PipeSegmentCount; v++, i += 4)
            {
                m_Vertices[i] = m_Vertices[i - ringOffset + 2];
                m_Vertices[i + 1] = m_Vertices[i - ringOffset + 3];
                m_Vertices[i + 2] = vertex;
                m_Vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
            }
        }


        private void SetUV()
        {
            m_UV = new Vector2[m_Vertices.Length];

            // Sets the uv for each vertice, ach quad needs to cover the 0–1 UV range in both dimensions.
            for (int i = 0; i < m_Vertices.Length; i += 4)
            {
                m_UV[i] = Vector2.zero;
                m_UV[i + 1] = Vector2.right;
                m_UV[i + 2] = Vector2.up;
                m_UV[i + 3] = Vector2.one;
            }
            m_Mesh.uv = m_UV;
        }


        private void SetTriangles()
        {
            //Each quad has two triangles, so six vertex indices.
            m_Triangles = new int[m_PipeSegmentCount * m_CurveSegmentCount * 6];

            for (int t = 0, i = 0; t < m_Triangles.Length; t += 6, i += 4)
            {
                m_Triangles[t] = i;
                m_Triangles[t + 1] = m_Triangles[t + 4] = i + 2;
                m_Triangles[t + 2] = m_Triangles[t + 3] = i + 1;
                m_Triangles[t + 5] = i + 3;
            }

            m_Mesh.triangles = m_Triangles;
        }

        #endregion MeshGeneration

        private void OnDrawGizmos()
        {
            if (m_DrawGizmos)
            {
                float uStepCurve = (2.0f * Mathf.PI) / m_CurveSegmentCount;
                float vStepPipe = (2.0f * Mathf.PI) / m_PipeSegmentCount;

                for (int uCurve = 0; uCurve < m_CurveSegmentCount; uCurve++)
                {
                    for (int vPipe = 0; vPipe < m_PipeSegmentCount; vPipe++)
                    {
                        Vector3 point = GetPointOnTorus(uCurve * uStepCurve , vPipe * vStepPipe);

                        Gizmos.color = new Color(
                        1.0f,
                        (float) vStepPipe / m_PipeSegmentCount,
                        (float) uStepCurve / m_CurveSegmentCount);

                        Gizmos.DrawSphere(point, 0.1f);
                    }
                }
            }
        }
    }
}
