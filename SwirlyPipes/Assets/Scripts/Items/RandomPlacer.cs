using UnityEngine;
using System.Collections;
using System;

namespace SwirlyPipesSystem
{
    public class RandomPlacer : PipeItemGenerator
    {
        [SerializeField] private PipeItem[] m_ItemPrefabs;

        public override void GenerateItems(Pipe pipe)
        {
            if ((m_ItemPrefabs != null) && (m_ItemPrefabs.Length > 0))
            {
                float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;

                // Instance 1 item per segment in the curve
                //for (int i = 0; i < pipe.CurveSegmentCount; i++)
                int numberItems = UnityEngine.Random.Range(2,6);
                numberItems = 3;
                float ZPosition = 5.0f;
                float offset = 5.0f;

                int line;

                for (int i = 0; i < numberItems; i++)
                {
                    PipeItem item = Instantiate<PipeItem>(m_ItemPrefabs[UnityEngine.Random.Range(0, m_ItemPrefabs.Length)]);
                    //float pipeRotation = (UnityEngine.Random.Range(0, pipe.PipeSegmentCount) + 0.5f) * 360f / pipe.PipeSegmentCount;                   


                    // Item Z Position depends on angle of the curve and curve segment
                    //float offset = 0.0f;
                    //float itemZPosition = i * (angleStep + offset);

                    /*float rotaitonItemThroughRing = 0;
                    float chance = UnityEngine.Random.Range(0.0f, 100.0f);
                    if (chance < 33)
                    {
                        rotaitonItemThroughRing = 50.0f;
                    }
                    if ((chance >= 33) && (chance < 77.0f))
                    {
                        rotaitonItemThroughRing = -50.0f;
                    }
                    rotaitonItemThroughRing = 0.0f;*/

                    line = UnityEngine.Random.Range(0, 3);

                    if (line == 1)
                    {
                        item.Init(pipe, ZPosition, -45);
                    }
                    else if (line == 2)
                    {
                        item.Init(pipe, ZPosition, 0);
                    }
                    else
                    {
                        item.Init(pipe, ZPosition, 45);
                    }

                    //item.Init(pipe, ZPosition, rotaitonItemThroughRing);
                    ZPosition += offset;
                }
            }
        }
    }
}
