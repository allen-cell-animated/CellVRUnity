using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.Cell
{
    public class Cell : MonoBehaviour
    {
        BoxCollider _boxCollider;
        public BoxCollider boxCollider
        {
            get
            {
                if (_boxCollider == null)
                {
                    _boxCollider = GetComponent<BoxCollider>();
                }
                return _boxCollider;
            }
        }
    }
}