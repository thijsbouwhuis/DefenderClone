using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHealthComponent : HealthComponent
{
    [SerializeField]
    private GameObject pauseMenu;

    void Awake()
    {
        Assert.IsNotNull(pauseMenu);
    }

    public override void Die()
    {
        GameOver();
    }

    private void GameOver()
    {
        pauseMenu.GetComponent<UIManager>()?.OpenRestartScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.GetComponent<UIManager>()?.PauseButtonClicked();
        }
    }
}
