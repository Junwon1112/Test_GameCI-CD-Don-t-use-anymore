using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 문을 통과하면 클리어 스테이지로 이동하는 것에 관한 함수
/// </summary>
public class ClearDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Clear");
        }
    }
}
