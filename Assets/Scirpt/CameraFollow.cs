using UnityEngine;

/// <summary>
/// Membuat kamera mengikuti target dengan gerakan halus (smooth follow).
/// Pasang script ini di GameObject kamera utama.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;         // Objek yang diikuti kamera

    [Header("Settings")]
    public float smoothSpeed = 0.125f;          // Kecepatan mengikuti (0 = instan, 1 = tidak bergerak)
    public Vector3 offset = new Vector3(0, 0, -10); // Jarak kamera dari target

    private void LateUpdate()
    {
        // LateUpdate berjalan setelah semua Update, cocok untuk kamera
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y, // ← sekarang ikut Y juga
            offset.z
        );

        // Lerp membuat gerakan kamera terasa mulus
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
