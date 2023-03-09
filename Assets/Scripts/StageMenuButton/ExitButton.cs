using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 종료시 실행될 클래스
/// </summary>
public class ExitButton : MonoBehaviour
{
    Button exitButton;

    private void Awake()
    {
        exitButton = GetComponent<Button>();
    }

    private void Start()
    {
        exitButton.onClick.AddListener(ExitGame);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
