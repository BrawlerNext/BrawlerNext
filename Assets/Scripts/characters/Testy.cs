using UnityEngine;

public class Testy : MovementManager {

    public override void HardAttack()
    {
        
    }

    public override void Jump()
    {
        rb.AddForce(Vector3.up * character.jumpForce, ForceMode.Impulse);
    }

    public override void SoftAttack()
    {

    }
}
