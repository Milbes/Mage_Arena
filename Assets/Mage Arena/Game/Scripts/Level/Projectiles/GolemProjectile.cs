#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;

public class GolemProjectile : ProjectileBehavior
{

    [Header("Rock Settings")]
    [SerializeField] float rockShowTime;
    [SerializeField] float rockStayTime;
    [SerializeField] float rockHideTime;
    [SerializeField] float delayBetweenRocks;
    [Space]
    [SerializeField] float depth;
    [Space]
    [SerializeField] List<Transform> rocks;

    private List<TweenCase> cases;

    public void Attack(ProjectileInfo projectileInfo)
    {
        TargetsPlayer = true;

        Init(projectileInfo);

        IsOnManualControl = true;

        if(cases == null)
        {
            cases = new List<TweenCase>();
        } else
        {
            cases.Clear();
        }

        transform.localEulerAngles = transform.localEulerAngles.SetX(0);

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        for (int i = 0; i < rocks.Count; i++)
        {
            Transform rock = rocks[i];

            rock.gameObject.SetActive(true);

            TweenCase moveCase = null;

            moveCase = rock.DOMoveY(0, rockShowTime)
                    .OnComplete(() => {
                        cases.Remove(moveCase);
                    });

            cases.Add(moveCase);

            yield return new WaitForSeconds(delayBetweenRocks);
        }
        yield return new WaitForSeconds(1f);

        TweenCase hideCase = this.DOAction((start, end, t) => {

            float value = start + (end - start) * t;
            
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].transform.position = rocks[i].transform.position.SetY(value);
            }
        }, 0, depth, rockHideTime).OnComplete(() => {
            for (int i = 0; i < rocks.Count; i++)
            {
                rocks[i].gameObject.SetActive(false);
            }
        });

        cases.Add(hideCase);

        yield return new WaitForSeconds(rockHideTime);

        gameObject.SetActive(false);

        cases.Clear();

        IsOnManualControl = false;
    }

    public void Disable()
    {
        for(int i = 0; i < cases.Count; i++)
        {
            TweenCase tweenCase = cases[i];

            if (tweenCase != null && !tweenCase.isCompleted) tweenCase.Kill();
        }

        cases.Clear();
    }
}
