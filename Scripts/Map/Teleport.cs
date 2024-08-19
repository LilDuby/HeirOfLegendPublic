using UnityEngine;
using Cinemachine;

public class Teleport : MonoBehaviour
{
    public Transform destination; // ������ ���� �ڷ���Ʈ ����Ʈ
    GameManager GM;
    private bool isTeleprting = false;
    public float teleportCooldown = 1f;

    void Start()
    {
        GM=GameManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if(TutorialCheck()) return;
            other.transform.position = destination.position;

            // ī�޶� �ݶ��̴� ������Ʈ
            CinemachineVirtualCamera virtualCamera = other.GetComponentInChildren<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                CinemachineConfiner2D confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
                if (confiner != null)
                {
                    PolygonCollider2D roomCollider = destination.parent.Find("cameraCollider").GetComponent<PolygonCollider2D>();
                    if (roomCollider != null)
                    {
                        confiner.m_BoundingShape2D = roomCollider;
                        Debug.Log("ī�޶� ��谡 ������Ʈ�Ǿ����ϴ�: " + destination.parent.name);
                    }
                    else
                    {
                        Debug.LogWarning("ī�޶� �ݶ��̴��� ã�� �� �����ϴ�.");
                    }
                }
            }
        }
    }
    bool TutorialCheck()
    {
        if(GM.tutoNum==9) 
        {
            GM.info.ChengChapter(3);
            GM.OnplayTutorial();
            Invoke("TutorialEnd",8f);
            return false;
        }
        else if(!GM.isTutorial)
            return false;
        
        else
            return true;
    }

    void TutorialEnd()
    {
        UIManager.instance.HideUI<TutorialInfoPopup>();
        GM.isTutorial=false;
        GM.tutoNum=0;
    }
}