using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UiView
{
    public class CounterText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterText;
        private int counter = 0;

        private Coroutine _coroutine;

        private Tween _tween;

        public void UpdateCounter()
        {
            counter++;
            counterText.text = counter.ToString();

            _tween?.Kill();

            _tween = transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .1f)
                .OnComplete(() => transform.DOScale(Vector3.one, .1f));

            StopCoroutine(DisableView());
            StartCoroutine(DisableView());
        }

        IEnumerator DisableView()
        {
            yield return new WaitForSeconds(1);
            counter = 0;
            gameObject.SetActive(false);
        }
    }
}