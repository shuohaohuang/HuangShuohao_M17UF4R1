using UnityEngine;

public class PCWeapon : MonoBehaviour
{
    [SerializeField]
    GameObject weapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void Equip()
    {
        weapon.SetActive(true);
    }
}
