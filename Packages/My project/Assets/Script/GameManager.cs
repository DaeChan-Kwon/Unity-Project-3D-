using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public GameObject gameLabel;
    Text gameText;
    Playermove player;
    private void Awake()
    {
        if(gm==null)
        {
            gm = this;
        }
    }
    public enum GameState
    {
        Ready,
        Run,
        GameOver
    }
    public GameState gState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gState = GameState.Ready;
        gameText = gameLabel.GetComponent<Text>();
        gameText.text = "Ready";
        gameText.color = new Color32(255, 181, 0, 255);
        StartCoroutine(ReadyToStart());
        player = GameObject.Find("Player").GetComponent<Playermove>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gm.gState != GameManager.GameState.Run)
        {
            return;
        }
        if(player.hp <= 0)
        {
            gameLabel.SetActive(true);
            gameText.text = "Game Over!";
            gameText.color = new Color32(255, 0, 0, 255);
            gState = GameState.GameOver;
        }
    }
    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(2f);
        gameText.text = "Go!";
        yield return new WaitForSeconds(1f);
        gameLabel.SetActive(false);
        gState = GameState.Run;

    }
}
