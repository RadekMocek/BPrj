using System.Collections;
using UnityEngine;

public class CustomTrigger_Outside : Trigger
{
    [Header("Custom trigger – Outside")]
    [SerializeField] private GameObject FM_DoorGO;
    [SerializeField] private GameObject FM_Door_TopGO;
    [SerializeField] private GameObject FM_Door_AnimMovingGO;
    [SerializeField] private Sprite FM_Door_AnimTopSprite;

    private Player playerScript;
    private SpriteRenderer playerSR;
    private Transform FM_Door_AnimMovingTransform;
    private float FM_Door_AnimMovingX;
    private float FM_Door_AnimMovingY;
    private SpriteRenderer FM_Door_AnimMovingSR;

    protected override void TriggerLogic()
    {
        var FM_DoorSR = FM_DoorGO.GetComponent<SpriteRenderer>();
        FM_DoorSR.sprite = FM_Door_AnimTopSprite;

        var playerGO = GameObject.Find("Player");
        playerSR = playerGO.GetComponent<SpriteRenderer>();
        playerScript = playerGO.GetComponent<Player>();
        playerSR.enabled = false;
        playerScript.DialogueStart(); // Prevent from moving

        FM_Door_TopGO.SetActive(false);

        FM_Door_AnimMovingTransform = FM_Door_AnimMovingGO.transform;
        FM_Door_AnimMovingX = FM_Door_AnimMovingGO.transform.localPosition.x;
        FM_Door_AnimMovingY = FM_Door_AnimMovingGO.transform.localPosition.y;
        FM_Door_AnimMovingSR = FM_Door_AnimMovingGO.GetComponent<SpriteRenderer>();

        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        while (FM_Door_AnimMovingY > .7f) {
            FM_Door_AnimMovingY -= .1f;
            FM_Door_AnimMovingTransform.localPosition = new Vector2(FM_Door_AnimMovingX, FM_Door_AnimMovingY);
            yield return new WaitForFixedUpdate();
        }
        FM_Door_AnimMovingSR.sortingLayerName = "Player";
        FM_Door_AnimMovingSR.sortingOrder = 100;
        CameraShake.Instance.ShakeCamera();
        yield return new WaitForSeconds(1.5f);
        playerScript.DialogueEnd();
        playerSR.enabled = true;
        ManagerAccessor.instance.SceneManager.ChangeScene("Floor1");
    }
}
