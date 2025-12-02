using UnityEngine;

public class PointPickUp : MonoBehaviour
{
    public void Trigger()
    {
        Debug.Log("PointPickUp");
        MazeManager.Instance.Pickup();
        gameObject.SetActive(false);
    }
}
