using UnityEngine;
using UnityEngine.EventSystems;
namespace LOGIYGames
{
    public class DragPointerHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        public float InputX;
        public float InputY;
        public float sensitivity = 50;
        public void OnBeginDrag(PointerEventData eventData)
        {
            InputX = 0;
            InputY = 0;
        }
        private void Update()
        {
            if (InputX != 0)
            {
                InputX = Mathf.Lerp(InputX, 0, Time.deltaTime * 10);
            }
            if (InputY != 0)
            {
                InputY = Mathf.Lerp(InputY, 0, Time.deltaTime * 10);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            InputX = eventData.delta.x * Time.deltaTime * sensitivity;
            InputY = eventData.delta.y * Time.deltaTime * sensitivity;


        }

        public void OnEndDrag(PointerEventData eventData)
        {
            InputX = 0;
            InputY = 0;
        }
    }
}