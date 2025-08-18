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
            var deltaTime = Time.deltaTime;
            modules.ForEach((m) => { m.OnUpdate(deltaTime); });
        }
        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            modules.ForEach((m) => { m.OnFixedUpdate(fixedDeltaTime); });
        }
        private void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            modules.ForEach((m) => { m.OnLateUpdate(deltaTime); });
        }
    }
}