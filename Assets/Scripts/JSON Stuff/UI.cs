using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public GameSettings settings;
    public TextMeshProUGUI text;
    //public Transform player;

    void Update()
    {
        settings.playerSpeed += 1f;
        /*text.text =
            "Speed: " + settings.playerSpeed +
            "\nHealth: " + settings.defaultHealth +
            "\nPlayer X: " + player.position.x.ToString("F2") +
            "\nPlayer Y: " + player.position.y.ToString("F2");
        */
    }
}