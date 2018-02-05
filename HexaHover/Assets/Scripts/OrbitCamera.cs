using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public static void OrbitLevel(Camera camera, float speed, float radius, bool smooth = true)
    {
        OrbitObject(camera, Vector3.zero, speed, radius, smooth);
    }

    public static void OrbitPlayer(Camera camera, GameObject player, float orbitDistance, float orbitSpeed, bool smooth = true)
    {
        ZoomInOnObject(camera, player.transform.position, orbitDistance);
        OrbitObject(camera, player.transform.position, orbitSpeed, orbitDistance, smooth);
    }

    private static void OrbitObject(Camera camera, Vector3 lookAt, float speed, float radius, bool smooth = true)
    {
        // Cheesey as fuck
        // Just move to the right by speed amount, then ensure distance away and look at object
        Vector3 newPos = camera.transform.position + camera.transform.right * speed;
        newPos = newPos.normalized * radius;
        float minCamY = 2.0f;
        newPos.y = Mathf.Max(minCamY, camera.transform.position.y * 0.9f);

        camera.transform.position = Vector3.MoveTowards(camera.transform.position, newPos, smooth ? 0.01f : 1000.0f);
        camera.transform.LookAt(lookAt);
    }

    private static void ZoomInOnObject(Camera camera, Vector3 position, float distance)
    {
        Vector3 dPos = position - camera.transform.position;
        float currentDistance = dPos.magnitude;
        Vector3 dPosNorm = dPos.normalized;

        float speed = currentDistance * 1.2f;

        Vector3 targetPos = position - (dPosNorm * distance);
        camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, 
            speed * Time.deltaTime);
    }
}
