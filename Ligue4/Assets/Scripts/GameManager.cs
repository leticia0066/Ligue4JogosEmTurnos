using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject red, green;

    bool isPlayer, hasGameFinished;

    [SerializeField] TMP_Text turnMessage;

    const string RED_MESSAGE = "Red's Turn";
    const string GREEN_MESSAGE = "Green's Turn";

    readonly Color RED_COLOR = new Color(231f / 255f, 29f / 255f, 54f / 255f);
    readonly Color GREEN_COLOR = new Color(0f / 255f, 222f / 255f, 1f / 255f);

    Board myBoard;
    Camera mainCamera;

    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;

        turnMessage.text = RED_MESSAGE;
        turnMessage.color = RED_COLOR;

        myBoard = new Board();
        mainCamera = Camera.main;
    }

    public void GameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (hasGameFinished) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (!hit.collider) return;

        if (hit.collider.CompareTag("Press"))
        {
            Column column = hit.collider.GetComponent<Column>();
            if (column == null) return;

            // Limite da coluna
            if (column.targetlocation.y > 1.5f) return;

            // Spawn
            GameObject circle = Instantiate(isPlayer ? red : green);
            circle.transform.position = column.spawnLocation;
            circle.GetComponent<Mover>().targetPostion = column.targetlocation;

            // Atualiza altura
            column.targetlocation += new Vector3(0, 0.7f, 0);

            // Atualiza board
            myBoard.UpdateBoard(column.col - 1, isPlayer);

            if (myBoard.Result(isPlayer))
            {
                turnMessage.text = (isPlayer ? "Red" : "Green") + " Wins!";
                hasGameFinished = true;
                return;
            }

            // Troca turno
            isPlayer = !isPlayer;

            turnMessage.text = isPlayer ? RED_MESSAGE : GREEN_MESSAGE;
            turnMessage.color = isPlayer ? RED_COLOR : GREEN_COLOR;
        }
    }
}