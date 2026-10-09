using UnityEngine;
using GDS.Core;
using GDS.Core.UGUI;

namespace GDS.Examples.UGUI {

    public class EquipOnChar_UiController : MonoBehaviour {

        [SerializeField, Required] EquipOnChar_Store store;
        [SerializeField, Required] ListBagView listBagView;
        [SerializeField, Required] SetBagView setBagView;
        [SerializeField] EquipOnChar_CharController charController;

        void Start() {
            store.Init(listBagView.Bag, setBagView.Bag);
            charController.Init(setBagView.Bag);
        }

    }

}