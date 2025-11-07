using UnityEngine;

public class InventoryContainer : MonoBehaviour
{

    public Inventory inventory; 

    public int capacity = 10;   

    private void Start()
    {
        inventory = new Inventory(capacity);
    }

}