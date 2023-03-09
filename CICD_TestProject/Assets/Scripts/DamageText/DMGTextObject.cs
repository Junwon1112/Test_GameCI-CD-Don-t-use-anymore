using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class DMGTextObject : MonoBehaviour
{
    TextMeshPro damageText;

    [SerializeField]
    float textSpeed = 6.0f;

    [SerializeField]
    float lifeTime = 2.0f;

    float damageValue;
    Vector3 moveDir;
    float randValue_x;
    float randValue_y;

    public float LifeTime
    {
        get { return lifeTime; }
        set { lifeTime = value; }
    }

    public float DamageValue
    {
        get { return damageValue; }
        set { damageValue = value; }
    }

    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
        Destroy(gameObject, lifeTime);
    }

    private void Start()
    {
        damageText.text = damageValue.ToString();
        randValue_x = Random.Range(-1f, 1f);
        randValue_x = Random.Range(0f, 1f);
        moveDir = new Vector3(randValue_x, randValue_y, 0);
        
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.position += textSpeed * Time.deltaTime * moveDir;
        damageText.alpha = Mathf.Lerp(damageText.alpha, 0, AlphaChange());

    }

    private float AlphaChange()
    {
        float firstAlpha = damageText.alpha;
        float alphaChangeSpeed = 1.5f;
        float spline = Mathf.Exp(alphaChangeSpeed);

        float lerpCalculation = spline * Time.deltaTime;

        return lerpCalculation;
    }
}
