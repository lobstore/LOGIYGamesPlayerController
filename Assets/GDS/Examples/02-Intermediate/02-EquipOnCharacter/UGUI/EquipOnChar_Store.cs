using GDS.Core;
using GDS.Core.Events;
using UnityEngine;

namespace GDS.Examples {

    public class EquipOnChar_Store : Store {


        public ListBag Backpack { get; private set; }
        public SetBag Equipment { get; private set; }

        void Awake() { StoreLocator.Register(this); }

        public void Init(ListBag backpack, SetBag equipment) {
            Backpack = backpack;
            Equipment = equipment;

            Debug.Log($"Store init: equipment {equipment}, backpack: {backpack}");
        }

        void OnEnable() {
            Bus.On<PickItem>(OnPickItem);
            Bus.On<PlaceGhostItem>(OnPlaceItem);
            Bus.On<DropGhostItem>(OnDropItem);
            Bus.On<PickWorldItem>(OnPickWorldItem);
        }

        void OnDisable() {
            Bus.Off<PickItem>(OnPickItem);
            Bus.Off<PlaceGhostItem>(OnPlaceItem);
            Bus.Off<DropGhostItem>(OnDropItem);
            Bus.Off<PickWorldItem>(OnPickWorldItem);
        }

        void OnPickItem(PickItem e) {
            Result result = e.Context.Bag.Remove(e.Context.Item);
            UpdateGhost(result, e.Context);
            Bus.Publish(result);
        }

        void OnPlaceItem(PlaceGhostItem e) {
            if (Ghost.Empty) return;
            Result result = e.Context.Bag.AddAt(e.Context.Slot, Ghost.Item);
            UpdateGhost(result, e.Context);
            Bus.Publish(result);
        }

        void OnDropItem(DropGhostItem e) {
            if (Ghost.Empty) return;
            if (e.IsOverUi) {
                Ghost.Bag.AddAt(Ghost.Slot, Ghost.Item);
                Ghost.Reset();
            } else {
                Bus.Publish(new SpawnWorldItem(Ghost.Item, e.WorldPosition));
                Ghost.Reset();
            }
        }

        void OnPickWorldItem(PickWorldItem e) {
            Debug.Log($"on pick world item");
            Result result = Backpack.Add(e.WorldItem.Item);
            if (result is Success) Bus.Publish(new DespawnWorldItem(e.WorldItem));
            Bus.Publish(result);
        }

    }

}