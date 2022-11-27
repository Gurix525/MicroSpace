using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Miscellaneous
{
    public class GasExchangeTimer : MonoBehaviour
    {
        public static float _timer = 0;
        public static readonly float _exchangeTickLength = 0.1F;

        public static UnityEvent GasExchangeTicked = new();

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;
            if (_timer >= 0.1F)
            {
                _timer -= 0.1F;
                GasExchangeTicked.Invoke();
            }
        }
    }
}