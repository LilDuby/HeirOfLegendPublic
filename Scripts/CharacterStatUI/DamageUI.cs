using System.Collections;
using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{       
    public TextMeshPro damageText;
    Color originalColor;
    
    public void DamageTxt(int damage)
    {        
        damageText.text=damage.ToString();

        if(damage<=0) //damage        
            damageText.color=new Color(1f, 0.2f, 0.2f, 1f);
        else //heal       
            damageText.color=new Color(0.2f, 1f, 0.2f, 1f);

        originalColor=damageText.color;
        StartCoroutine(TxtEffect());
    }
    IEnumerator TxtEffect()
    {
        float elapsedTime = 0f;
        float duration = 2f; // 효과 지속 시간

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 1, 0); // 시작지점.y +=1
        while (elapsedTime < duration)
        {            
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);            
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime; 
            yield return null; 
        }
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}
