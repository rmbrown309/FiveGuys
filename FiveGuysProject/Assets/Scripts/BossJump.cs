using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossJump : MonoBehaviour
{
    [SerializeField] BossAI boss;
    [SerializeField] float MinJumpDistance = 2f;
    [SerializeField] float MaxJumpDistance = 15f;
    [SerializeField] float JumpSpeed = 1f;
    [SerializeField] float cooldown = 4f;
    [SerializeField] int damage = 1;

    public AnimationCurve HeightCurve;

    public bool isActive;

    float useTime;

    public  void UseAbility()
    {
        boss.StartCoroutine(Jump(GameManager.instance.player.transform.position));
    }

    public bool CanUseAbility()
    {
        float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);

        return !isActive 
            && (useTime + cooldown < Time.time) 
            && (distance >= MinJumpDistance) 
            && (distance <= MaxJumpDistance); 
    }

    private IEnumerator Jump(Vector3 targetPosition)
    {
        //boss.Agent.enabled = false;
        //boss.Movement.enabled = false;
        //boss.Movement.State = BossState.UsingAbility;

        Vector3 startingPosition = boss.transform.position;
        //boss.Animator.SetTrigger("Jump");

        for(float time = 0; time < 1; time += Time.deltaTime * JumpSpeed)
        {
            boss.transform.position = Vector3.Lerp(startingPosition, targetPosition, time) + Vector3.up * HeightCurve.Evaluate(time);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, Quaternion.LookRotation(targetPosition - boss.transform.position), time);

            yield return null;
        }
        //boss.Animator.SetTrigger("Landed");

        useTime = Time.time;

        boss.enabled = true;
        //boss.Movement.enabled = true;
        //boss.Agent.enabled = true;

        //if(NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, boss.Agent.areaMask))
        //{
        //    boss.Agent.Warp(hit.position);
        //    boss.Movement.State = BossState.Chase;
        //}

        isActive = false;
    }
}
