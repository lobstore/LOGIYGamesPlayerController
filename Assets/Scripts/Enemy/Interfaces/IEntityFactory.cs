using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public interface IEntityFactory<T, TConfig>
    {
        T Create(TConfig config, Vector3 position);
    }
}