using UnityEngine;

namespace Michsky.UI.Shift
{
    public class BagPanelManager : MonoBehaviour
    {
        Animator windowAnimator;
        bool isOn = false;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                AnimateWindow();
            }
        }
        void Start()
        {
            windowAnimator = gameObject.GetComponent<Animator>();
        }

        public void AnimateWindow()
        {
            if (isOn == false)
            {
                windowAnimator.CrossFade("Window In", 0.1f);
                isOn = true;
            }

            else
            {
                windowAnimator.CrossFade("Window Out", 0.1f);
                isOn = false;
            }
        }

        public void WindowIn()
        {
            if (isOn == false)
            {
                windowAnimator.CrossFade("Window In", 0.1f);
                isOn = true;
            }
        }

        public void WindowOut()
        {
            if (isOn == true)
            {
                windowAnimator.CrossFade("Window Out", 0.1f);
                isOn = false;
            }
        }
    }
}