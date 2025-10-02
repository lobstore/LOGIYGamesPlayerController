using System.Collections.Generic;
namespace LOGIYGames
{
    public interface IDBContext<T>
    {
        IEnumerable<T> GetEntities();
    }
}