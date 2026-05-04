using Unity.Netcode;
using UnityEngine;

public class Player2 : NetworkBehaviour
{
    public float speed = 5f;

    void Update()
    {
        if (!IsOwner) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);
        transform.Translate(move * speed * Time.deltaTime);
    }
}