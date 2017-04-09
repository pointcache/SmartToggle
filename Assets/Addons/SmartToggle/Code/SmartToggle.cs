namespace pointcache {

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public class SmartToggle : MonoBehaviour {

        public enum InitialState {
            @true, @false, manual
        }

        public InitialState initialState;
        public Image on;
        public Image knob;
        public RectTransform knobRt, toggleOff;
        public Color knobOffColor, knobOnColor;
        public float transition = 0.2f;

        private float knobXOn = 39.86f, knobXOff = 13.4f;
        private float driver = 0f;
        private float lerp = 0f;
        private float direction = 1f;
        private Vector2 initApos;
        private bool state;

        [System.Serializable]
        public class OnToggle : UnityEvent<bool> { }
        public OnToggle onToggle;

        public UnityEvent onTrue;
        public UnityEvent onFalse;


        void OnEnable() {
            if (initialState != InitialState.manual) {
                driver = transition / 2f;
                state = initialState == InitialState.@true ? true : false;
                SetState(state);
            }
        }

        public void Toggle() {
            state = !state;
            onToggle.Invoke(state);
            SetState(state);
        }

        public void SetState(bool val) {
            if (val)
                SetTrue();
            else
                SetFalse();
        }

        /// <summary>
        /// Will not raise events
        /// </summary>
        /// <param name="val"></param>
        public void SetStateDirectly(bool val) {
            state = val;
            driver = transition / 2f;
            if (val) {
                direction = 1f;
            }
            else {
                direction = -1f;
            }
        }

        void Update() {
            driver += Time.deltaTime * direction;
            driver = Mathf.Clamp(driver, 0f, transition);

            if (driver > 0f && driver < transition) {
                float driver01 = Remap(driver, 0f, transition, 0f, 1f);
                lerp = state ? OutQuint(driver01) : InQuint(driver01);
                Color oncol = on.color;
                oncol.a = Mathf.Lerp(0f, 1f, lerp);
                on.color = oncol;
                knob.color = Color.Lerp(knobOffColor, knobOnColor, lerp);
                Vector2 apos = knobRt.anchoredPosition;
                apos.x = Mathf.Lerp(knobXOff, knobXOn, lerp);
                knobRt.anchoredPosition = apos;
            }
        }


        public void SetTrue() {
            onTrue.Invoke();
            direction = 1f;
        }

        public void SetFalse() {
            onFalse.Invoke();
            direction = -1f;
        }

        float OutQuint(float n) {
            return --n * n * n * n * n + 1;
        }
        float InQuint(float n) {
            return n * n * n * n * n;
        }

        float Remap(float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}