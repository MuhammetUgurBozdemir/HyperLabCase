using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UiView
{
    public class TargetView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI targetText;

        public void Init(Color bgColor, int Count)
        {
            background.color = bgColor;
            targetText.text = Count.ToString();
        }

        public void UpdateText(int count)
        {
            targetText.text = count < 0 ? "0" : count.ToString();
        }
    }
}