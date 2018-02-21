using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testy : MovementManager {


    public override void HardAttackSkill()
    {

    }

    public override void JumpSkill()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public override void SoftAttackSkill()
    {

    }
}
