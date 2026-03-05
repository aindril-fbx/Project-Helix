#if PACKAGE_DOCS_GENERATION || UNITY_INPUT_SYSTEM_ENABLE_UI
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.OnScreen
{
    [SerializeField] private Slider slider;
    [AddComponentMenu("Input/On-Screen Button")]

    public class OnScreenSLIDER : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        public void OnValueChanged(PointerEventData eventData)
        {
            SendValueToControl(slider.value);
        }

        [InputControl(layout = "Value")]
        [SerializeField]private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}
#endif
