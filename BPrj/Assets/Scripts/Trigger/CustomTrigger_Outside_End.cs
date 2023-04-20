using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomTrigger_Outside_End : Trigger
{
    [Header("Custom trigger – Outside_End")]
    [SerializeField] private Enemy[] enemyScripts;
    [SerializeField] private GameObject FM_Door_AnimMovingGO;
    [SerializeField] private GameObject FM_Door_TopGO;
    [SerializeField] private GameObject FM_DoorGO;
    [SerializeField] private GameObject FM_Door_TopGOPlayer;

    protected override void TriggerLogic()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        // Init
        var playerGO = GameObject.Find("Player");

        var playerSR = playerGO.GetComponent<SpriteRenderer>();
        playerSR.enabled = false;

        var playerScript = playerGO.GetComponent<Player>();
        playerScript.DialogueStart(Direction.S); // Prevent player from moving

        // Kill robots
        yield return new WaitForSeconds(1.2f);
        foreach (Enemy enemyScript in enemyScripts) {
            yield return new WaitForSeconds(0.7f);
            enemyScript.ReceiveDamage(Vector2.zero, int.MaxValue);
        }

        // Raise the bars
        var FM_Door_AnimMovingSR = FM_Door_AnimMovingGO.GetComponent<SpriteRenderer>();
        FM_Door_AnimMovingSR.sortingOrder = -1;

        var FM_Door_AnimMovingTransform = FM_Door_AnimMovingGO.transform;
        var FM_Door_AnimMovingX = FM_Door_AnimMovingGO.transform.localPosition.x;
        var FM_Door_AnimMovingY = FM_Door_AnimMovingGO.transform.localPosition.y;

        while (FM_Door_AnimMovingY < 3.8f) {
            FM_Door_AnimMovingY += .1f;
            FM_Door_AnimMovingTransform.localPosition = new Vector2(FM_Door_AnimMovingX, FM_Door_AnimMovingY);
            yield return new WaitForFixedUpdate();
        }

        // Open the door
        yield return new WaitForSeconds(0.3f);
        FM_Door_TopGO.SetActive(false);
        FM_DoorGO.SetActive(true);
        FM_Door_TopGOPlayer.SetActive(true);

        // Walk out
        yield return new WaitForSeconds(1.0f);
        playerSR.enabled = true;
        playerScript.Anim.CrossFade("Player_Walk_Down", 0);
        playerScript.RB.velocity = 4.7f * Vector2.down;

        // Thanks for playing
        yield return new WaitForSeconds(2.0f);
        playerScript.Anim.CrossFade("Player_Idle_Down", 0);
        playerScript.RB.velocity = Vector2.zero;
        ManagerAccessor.instance.HUD.ShowThanksForPlaying();

        // Return to main menu
        /*
        yield return new WaitForSeconds(2.5f);
        ManagerAccessor.instance.SceneManager.MainMenu();
        /**/
    }
}
