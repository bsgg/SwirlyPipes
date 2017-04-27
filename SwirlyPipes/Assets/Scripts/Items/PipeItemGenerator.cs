using UnityEngine;
using System.Collections;

namespace SwirlyPipesSystem
{
    public abstract class PipeItemGenerator : MonoBehaviour
    {
        public abstract void GenerateItems(Pipe pipe);
    }
}
