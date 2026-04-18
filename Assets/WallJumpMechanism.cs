using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpMechanism : MonoBehaviour
{
    [Header("References")]
    public PlayerSystem playerSystem;

    [Header("Two-way Jump Cubes")]
    public Cube lowerCube;
    public Cube upperCube;

    [Header("Two-way Jump Points")]
    public Transform lowerPoint;
    public Transform upperPoint;

    [Header("Jump")]
    public float jumpHeight = 0.8f;
    public float jumpDuration = 0.35f;

    bool isRunning = false;

    public bool IsRunning => isRunning;

    public void TryActivate()
    {
        if (isRunning) return;
        if (playerSystem == null || playerSystem.player == null) return;
        if (lowerCube == null || upperCube == null) return;
        if (lowerPoint == null || upperPoint == null) return;
        if (playerSystem.IsBusy) return;

        Player player = playerSystem.player;

        if (player.current == lowerCube)
        {
            StartCoroutine(JumpRoutine(
                fromCube: lowerCube,
                toCube: upperCube,
                targetPoint: upperPoint
            ));
            return;
        }

        if (player.current == upperCube)
        {
            StartCoroutine(JumpRoutine(
                fromCube: upperCube,
                toCube: lowerCube,
                targetPoint: lowerPoint
            ));
            return;
        }

        Debug.Log("Player is not standing on either end of this WallJumpMechanism.");
    }

    IEnumerator JumpRoutine(Cube fromCube, Cube toCube, Transform targetPoint)
    {
        isRunning = true;
        playerSystem.externalLock = true;

        Player player = playerSystem.player;

        player.transform.SetParent(null);

        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;

        Vector3 endPos = targetPoint.position;
        Quaternion endRot = targetPoint.rotation;

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / jumpDuration);

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            player.transform.position = pos;
            player.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        player.transform.position = endPos;
        player.transform.rotation = endRot;
        player.current = toCube;
        player.path = null;
        player.transform.SetParent(toCube.transform);

        playerSystem.externalLock = false;
        isRunning = false;
    }
}