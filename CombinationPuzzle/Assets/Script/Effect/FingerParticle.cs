using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerParticle : MonoBehaviour
{
    [SerializeField]
    GameObject effectPool = null;

    [SerializeField]
    ParticleSystem fingerParticle = null;

    int count = 0;

    const int ParticleFrequency = 3;

    List<ParticleSystem> stockList = new List<ParticleSystem>();

    void Update()
    {
        if (PeaceManager.Instance.nowHoldPeace != null && GameSystem.Instance.GetTimer.StopTimeFlag == false && Input.GetMouseButton(0) == true)
        {
            count++;
            if (count < ParticleFrequency) return;

            count = 0;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10, 100);
            if (null == hit.transform)
                return;

            ParticleSystem particle;
            if (stockList.Count == 0)
            {
                particle = Instantiate(fingerParticle);
                particle.transform.SetParent(effectPool.transform, false);
                particle.transform.localPosition = PeaceManager.Instance.nowHoldPeace.transform.localPosition;
            }
            else
            {
                particle = stockList[0];
                particle.gameObject.SetActive(true);
                stockList.RemoveAt(0);
                particle.Play();
            }
            particle.transform.localPosition = PeaceManager.Instance.nowHoldPeace.transform.localPosition;
            particle.transform.localPosition += new Vector3(150, -100, -500);
            StartCoroutine(ResetParticle(particle));
        }
    }

    private IEnumerator ResetParticle(ParticleSystem p)
    {
        yield return new WaitForSeconds(0.3f);//少し表示したら消す、エフェクトが2回出るため
        p.gameObject.SetActive(false);
        stockList.Add(p);
    }


    [SerializeField]
    ParticleSystem fallParticle = null;

    List<ParticleSystem> fallParticleList = new List<ParticleSystem>();

    public void PlayParticle(Vector3 vector)
    {
        ParticleSystem particle;
        if (fallParticleList.Count == 0)
        {
            particle = Instantiate(fallParticle);
            particle.transform.SetParent(effectPool.transform, false);
        }
        else
        {
            particle = fallParticleList[0];
            particle.gameObject.SetActive(true);
            fallParticleList.RemoveAt(0);
            particle.Play();
        }
        particle.transform.localPosition = vector;
        particle.transform.localPosition += new Vector3(150, -100, -500);
        StartCoroutine(ResetFallParticle(particle));
    }

    private IEnumerator ResetFallParticle(ParticleSystem p)
    {
        yield return new WaitForSeconds(0.3f);//少し表示したら消す、エフェクトが2回出るため
        p.gameObject.SetActive(false);
        fallParticleList.Add(p);
    }
}
