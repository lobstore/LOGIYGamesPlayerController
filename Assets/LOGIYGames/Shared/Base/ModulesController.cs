using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LOGIYGames
{
    public class ModulesController : MonoBehaviour
    {
        List<IModule> modules = new();
        List<IModule> enabledModules = new();
        private void Awake()
        {
            modules = GetComponents<IModule>().ToList();

            foreach (var module in modules)
            {
                module.Initialize();
            }
            modules = modules.AsParallel().OrderBy(x => x.ModulePriority).ToList();
            enabledModules = modules.Where(x => x.Enabled).ToList();
        }
        void Update()
        {
            foreach (var module in enabledModules)
            {
                module.OnUpdate(Time.deltaTime);

            }
        }
        private void FixedUpdate()
        {
            foreach (var module in enabledModules)
            {
                module.OnFixedUpdate(Time.deltaTime);

            }
        }
        private void LateUpdate()
        {
            foreach (var module in enabledModules)
            {
                module.OnLateUpdate(Time.deltaTime);

            }
        }
    }
}