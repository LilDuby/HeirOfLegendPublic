using UnityEngine;

public class ItemFind : MonoBehaviour
{  
    public bool isOpen=false;
    private void OnTriggerEnter2D(Collider2D collider)
    {           
        if(collider.gameObject.CompareTag("Player"))
        {   if(!isOpen)
            {                   
                SpawnRandomItem();
                isOpen=true;
                InitializeItemData();
            }
        }
    }
    private void SpawnRandomItem()
    {   
        ItemPrefab randomItem = DataManager.instance.EquipmentsRandomItem();
        Vector2 pos = new Vector2(gameObject.transform.position.x,gameObject.transform.position.y-0.5f);
        GameManager.instance.ItemPrefabPool(randomItem, pos);
    }
    private void InitializeItemData()
    {
        DataManager.instance.itemPrefab.itemData = null;
        DataManager.instance.itemPrefab.Initialize();
    }
}
