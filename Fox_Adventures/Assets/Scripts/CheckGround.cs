using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public bool isGrounded = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if (collision.tag == "JumpableGround")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "JumpableGround")
        {
            isGrounded = false;
        }
    }
}