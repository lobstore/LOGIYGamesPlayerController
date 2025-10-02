using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LOGIYGames
{
    public interface IRepository<T>
    {
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(Guid id);
    }
}