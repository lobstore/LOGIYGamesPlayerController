using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
namespace LOGIYGames
{
    public class ModulesController : MonoBehaviour
    {
        List<IModule> modules = new();
        private void Awake()
        {
            modules = GetComponents<IModule>().ToList();

            foreach (var module in modules)
            {
                module.Initialize();
            }
            modules = modules.AsParallel().OrderBy(x => x.ModulePriority).ToList();
        }
        void Update()
        {
            modules.ForEach((m) => { m.OnUpdate(Time.deltaTime); });
        }
        private void FixedUpdate()
        {
            modules.ForEach((m) => { m.OnFixedUpdate(Time.fixedDeltaTime); });
        }
        private void LateUpdate()
        {
            modules.ForEach((m) => { m.OnLateUpdate(Time.deltaTime); });
        }
    }
}