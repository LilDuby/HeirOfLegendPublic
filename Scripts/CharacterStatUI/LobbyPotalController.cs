using UnityEngine;

public class LobbyPotalController : MonoBehaviour
{
    public LobbySceneUI Lobby;

    private void OnTriggerEnter2D(Collider2D collider)
    { 
        if(collider.gameObject.CompareTag("Player"))
        {
           Lobby.OnDungeanPopup();
        }        
    }        
}
