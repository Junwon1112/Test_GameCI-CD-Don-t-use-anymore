using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGTextPlayer : MonoBehaviour
{
    public static DMGTextPlayer Instance { get; private set; }

    [SerializeField]
    private GameObject dmg_TextObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    public void CreateDMGText(Transform parentTransform ,float _damageValue, float _liftime = 1.5f)
    {
        GameObject DMGObj = Instantiate(dmg_TextObject, parentTransform);
        DMGTextObject dmgTextObj = DMGObj.GetComponent<DMGTextObject>();
        dmgTextObj.DamageValue = _damageValue;
        dmgTextObj.LifeTime = _liftime;
    }

    public void CreateDMGText(Transform parentTransform, Vector3 position, Quaternion rotation, float _damageValue, float _liftime = 1.5f)
    {
        GameObject DMGObj = Instantiate(dmg_TextObject, position, rotation, parentTransform);
        DMGTextObject dmgTextObj = DMGObj.GetComponent<DMGTextObject>();
        dmgTextObj.DamageValue = _damageValue;
        dmgTextObj.LifeTime = _liftime;
    }

}
