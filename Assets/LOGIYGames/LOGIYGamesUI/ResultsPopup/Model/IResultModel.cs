using UnityEngine.Events;
namespace LOGIYGames
{
    public interface IResultModel
    {
        public UnityEvent OnValueChaned { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }
    }
}