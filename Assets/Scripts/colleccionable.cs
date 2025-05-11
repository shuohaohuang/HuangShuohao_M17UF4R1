using UnityEngine;

public class colleccionable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PCWeapon>() is PCWeapon pCWeapon)
        {
            pCWeapon.Equip();
            Destroy(gameObject);
        }
    }
}
