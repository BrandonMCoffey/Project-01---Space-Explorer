using UnityEngine;

namespace Assets.Scripts {
public class PowerupController : MonoBehaviour {
    public void Reload()
    {
        foreach (Transform obj in transform)
        {
            Powerup powerup = obj.GetComponent<Powerup>();
            if (powerup == null) continue;
            powerup.Reload();
        }
    }
}
}
