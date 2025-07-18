using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
   public string cellState; //"Empty": ô trống; "HavingFood": ô chứa food; "LockedCell": ô bị khoá; "HavingState": ô chứa khối gỗ

    public GameObject food; // biến chứa prefab của ô

    public int xIndex; // biến chứa chỉ số hàng của ô
    public int yIndex; // biến chứa chỉ số cột của ô

    public ParticleSystem smokeVFX1;
    public ParticleSystem smokeVFX2;

    public Node (string cellState, GameObject _food)
    {
        this.cellState = cellState;
        this.food = _food;   
    }

    public void SetIndex(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void TurnOnSmokeVFX()
    { 
        smokeVFX1.Play();
        smokeVFX2.Play();
    }
}
