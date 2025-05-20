using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
  public System.Action<Coin> coinTap;

    private void OnMouseDown()
    {
        coinTap?.Invoke(this);
    }
}
