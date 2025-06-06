using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class ManualUpdateSpawner : MonoBehaviour
{
    public int objectCount = 10000; // Количество объектов
    private List<ManualMover> movers = new List<ManualMover>();

    void Start()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = new GameObject("Manual_" + i);
            ManualMover mover = new ManualMover(obj.transform); // Передаем Transform объекта
            movers.Add(mover);
        }
    }

    void Update()
    {
        var time = Time.deltaTime;


        Parallel.ForEach(movers, (t) => { t.ManualUpdateLogic(time); });
        //Task.Run(() =>
        //{
        //    foreach (var item in movers)
        //    {
        //        item.ManualUpdateLogic(time);
        //    } 
        //});

        //movers.AsParallel().ForAll(mover => { mover.ManualUpdateLogic(time); });

        foreach (var mover in movers)
        {
            mover.ManualUpdatePhisics(time); // Вызываем обновление вручную
        }
    }
    private void FixedUpdate()
    {

    }
}
