#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class DropCurrency : MonoBehaviour
{
    [SerializeField] Currencies currencyType;

    [Header("Sound")]
    [SerializeField] AudioClip pickUpSound;
    [SerializeField] float pickUpDelay;

    public Currencies CurrencyType => currencyType;

    private Transform target;
    private bool isFollowing;

    private float speed;

    private static float lastAudioTime;

    private bool shouldRotate = false;


    public void Init()
    { 
        isFollowing = false;

        speed = 50;

        shouldRotate = false;

        transform.localScale = Vector3.zero;
        transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
        transform.DOScale(1, 0.3f).SetEasing(Ease.Type.BackOut);
        transform.DOMove(transform.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * Random.Range(0.5f, 1.5f), 0.3f).SetEasing(Ease.Type.QuadOut);

        Tween.DelayedCall(Random.Range(0.3f, 0.5f), () => shouldRotate = true);
    }

    public void FollowTransform(Transform transform)
    {
        target = transform;

        isFollowing = true;
    }

    private void FixedUpdate()
    {
        if (isFollowing)
        {

            if(Vector3.Distance(target.transform.position, transform.position) < 2)
            {
                gameObject.SetActive(false);
                isFollowing = false;
                target = null;

                float time = Time.time;
                
                if(time - lastAudioTime > pickUpDelay && pickUpSound  != null)
                {
                    AudioController.PlaySound(pickUpSound, GameController.Sound * 5);

                    lastAudioTime = time;
                }

                return;
            }

            transform.position = transform.position + (target.position - transform.position).normalized * speed * Time.deltaTime;
        } else
        {
            if (shouldRotate)
            {
                transform.eulerAngles = transform.eulerAngles + Vector3.up * 100 * Time.fixedDeltaTime;
            }
        }
    }

    

}
