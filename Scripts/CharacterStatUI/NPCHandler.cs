using UnityEngine;

public class NPCHandler : DungeonSceneUI
{       
    public int NpcNum;
    void Start()
    {
        if(GameManager.instance.NPCToggle.Count<=NpcNum)
        {
            GameManager.instance.NPCToggle.Add(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {        
        if(collider.gameObject.CompareTag("Player"))
        {
            GameManager.instance.NPCToggle[NpcNum]=true; //Npc IN            
            if(NpcNum==0)
            {
                if(!GameManager.instance.TutoPlayLockCheck(NpcNum))
                    OnShopPopup();
                collider.GetComponent<Controller>().isAttacking = false;
            }
            else if(NpcNum==1)
            {
                if(!GameManager.instance.TutoPlayLockCheck(NpcNum))
                    OnForgePopup();
                collider.GetComponent<Controller>().isAttacking = false;

            }

            OnHealingStatue();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GameManager.instance.NPCToggle[NpcNum]=false; //Npc out            
            if(NpcNum==0)
            {
                OffShopPopup();
            }
            else if(NpcNum==1)
            {
                OffForgePopup();
            }
        }
    }

    void OnHealingStatue()
    {
        if(GameManager.instance.NPCToggle[2])
        {  
            HealthSystem healthSystem= GameManager.instance.player.GetComponent<HealthSystem>();
            if (healthSystem.curHealth >= healthSystem.maxHealth && healthSystem.curStamina >= healthSystem.maxStamina) return;
            healthSystem.ChangeHealthHeal(999);
            healthSystem.ChangeHealStamina();
        }
    } 
}
