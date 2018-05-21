using System;
using System.Collections;
using characters.scriptables;
using UnityEngine;
using util;

public class CollisionManager : MonoBehaviour
{

    protected PlayerManager pm;
    protected AudioManager audioManager;

    public PunchCollider[] colliders;
    public bool isIgnoringForward = false;
    public Collider dashCollider;
    public GameObject shield;

    // This are references from PlayerManager.cs updated in every frame
    private Actions lastAction = Actions.IDLE;
    private Transform otherPlayer;
    private Character character;
    private ParticleManager particleManager;
    private Player player;
    private PlayerManager otherPlayerManager;
    private bool otherPlayerIsInvulnerable = false;

    private void Start()
    {
        pm = GetComponent<PlayerManager>();
        SetDataFromPlayerManager();

        particleManager = new ParticleManager(character);
        audioManager = pm.audioManager;

        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Shield"))
            {
                shield = child.gameObject;
                shield.SetActive(false);
            }

            if (child.name == "DashCollider")
            {
                dashCollider = child.GetComponent<Collider>();
                dashCollider.enabled = false;
            }
        }

        otherPlayerManager = otherPlayer.GetComponent<PlayerManager>();

    }

    private void Update()
    {
        SetDataFromPlayerManager();
    }

    private void SetDataFromPlayerManager()
    {
        this.lastAction = pm.lastAction;
        this.otherPlayer = pm.otherPlayer;
        this.character = pm.character;
        this.player = pm.player;

        if (this.otherPlayerManager != null) {
            this.otherPlayerIsInvulnerable = otherPlayerManager.isInvulnerable;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 positionToInstantiate = getCollisionPoint();

        if (!collision.collider.tag.Contains(otherPlayer.tag)) return;

        switch (lastAction)
        {
            case Actions.DASHING:
                PlayerManager otherPlayerManager = otherPlayer.GetComponent<PlayerManager>();

                if (!this.otherPlayerIsInvulnerable) {
                    otherPlayerManager.Stun(pm.delayImpulseOnHit);
                }

                pm.dashDelay = character.dashTimeInSeconds;
                break;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Vector3 positionToInstantiate = getCollisionPoint();

        if (!collider.tag.Contains(otherPlayer.tag) || this.otherPlayerIsInvulnerable) return;

        if (pm.shieldsUp) return;

        switch (lastAction)
        {
            case Actions.SOFT_PUNCH:
                if (positionToInstantiate == Vector3.zero) return;
                Hit(character.softPunchDamage, positionToInstantiate, ParticleType.SOFT_HIT, AudioType.SOFT_HIT, false);
                print("Soft punch! with " + collider.tag);
                break;
            case Actions.HARD_PUNCH:
                if (positionToInstantiate == Vector3.zero) return;
                float damage = character.hardPunchDamage;

                if (UnityEngine.Random.value < character.criticChance)
                {
                    damage *= 2;
                }

                print("Hard punch! with " + collider.tag);
                Hit(damage, positionToInstantiate, ParticleType.HARD_HIT, AudioType.HARD_HIT, true);
                break;
        }
    }

    private Vector3 getCollisionPoint()
    {
        foreach (PunchCollider punchCollider in colliders)
        {
            if (punchCollider.collider.enabled) return punchCollider.collider.transform.position;
        }

        return Vector3.zero;
    }

    public void TooglePunchCollider(ColliderType colliderType)
    {
        foreach (PunchCollider punchCollider in colliders)
        {
            if (punchCollider.colliderType == colliderType) punchCollider.collider.enabled = !punchCollider.collider.enabled;
        }
    }

    public void Hit(float impulse, Vector3 positionToInstantiate, ParticleType particleType, AudioType audioType, bool inmediateImpulse)
    {
        if (this.otherPlayerIsInvulnerable) return;

        lastAction = Actions.IDLE;

        otherPlayerManager.AddImpulse(impulse * pm.impulseMultiplier);

        audioManager.Play(audioType);

        particleManager.InstantiateParticle(particleType, positionToInstantiate);

        if (pm.currentCombo > 3 || inmediateImpulse) otherPlayerManager.ApplyImpulse();
    }

    public void OnCollisionStay(Collision other)
    {
        if ((other.gameObject.tag == "P1" && player == Player.P2) ||
            (other.gameObject.tag == "P2" && player == Player.P1))
        {
            isIgnoringForward = true;
        }
    }

    public void OnCollisionExit(Collision other)
    {
        if ((other.gameObject.tag == "P1" && player == Player.P2) ||
            (other.gameObject.tag == "P2" && player == Player.P1))
        {
            isIgnoringForward = false;
        }
    }

    public IEnumerator DisableDashAfterSeconds()
    {
        yield return new WaitForSecondsRealtime(2);
        dashCollider.enabled = false;
    }

    internal bool CheckCollision()
    {
        if (Vector3.Distance(transform.position, otherPlayer.transform.position) < 2)
        {
            if (!this.otherPlayerIsInvulnerable) {
                otherPlayer.GetComponent<PlayerManager>().Stun(pm.delayImpulseOnHit);
                
                Hit(character.aeroPunchDamage, otherPlayer.transform.position, ParticleType.SOFT_HIT, AudioType.SOFT_HIT, false);
            }

            return true;
        }

        return false;
    }
}