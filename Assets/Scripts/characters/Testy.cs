using UnityEngine;

public class Testy : PlayerManager {

    public override void Burn()
    {

    }

    public override void Defend()
    {

    }

    public override void HardAttack()
    {
        
    }

    public override void Jump()
    {
        rb.AddForce(Vector3.up * character.jumpForce * 10, ForceMode.Impulse);
    }

    public override void SoftAttack()
    {

    }
}
