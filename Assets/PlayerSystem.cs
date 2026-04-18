using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public Player player;

    public bool externalLock = false;

    public bool IsMoving
    {
        get
        {
            return player != null && player.path != null && player.path.Count > 0;
        }
    }

    public bool IsBusy
    {
        get
        {
            return IsMoving || externalLock;
        }
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("PlayerSystem: player is not assigned.");
            return;
        }

        Cube[] cubes = Object.FindObjectsOfType<Cube>();
        float best = Mathf.Infinity;
        Cube closest = null;

        foreach (var c in cubes)
        {
            Vector2 a = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 b = new Vector2(c.transform.position.x, c.transform.position.z);
            float d = Vector2.Distance(a, b);

            if (d < best)
            {
                best = d;
                closest = c;
            }
        }

        player.current = closest;

        if (player.current != null)
        {
            SnapPlayerToCube(player.current);
        }
    }

    void Update()
    {
        if (player == null) return;
        if (player.path == null || player.path.Count == 0) return;

        Cube targetCube = player.path.First.Value;
        Vector3 targetPos = GetStandPosition(targetCube);
        Quaternion targetRot = GetStandRotation(targetCube);

        float step = player.walkSpeed * Time.deltaTime;
        float dist = Vector3.Distance(player.transform.position, targetPos);

        if (dist <= step)
        {
            player.transform.position = targetPos;
            player.transform.rotation = targetRot;

            player.current = targetCube;
            player.path.RemoveFirst();

            if (player.path.Count == 0)
            {
                player.path = null;
                AttachToCurrentCube();
            }
        }
        else
        {
            Vector3 dir = (targetPos - player.transform.position).normalized;
            player.transform.position += dir * step;

            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRot,
                Time.deltaTime * 10f
            );
        }
    }

    public void OnCubeClicked(Cube cube)
    {
        if (player == null || player.current == null || cube == null) return;
        if (IsBusy) return;
        if (cube == player.current) return;

        LinkedList<Cube> newPath = FindPath(player.current, cube);
        if (newPath == null)
        {
            Debug.LogWarning("No path found");
            return;
        }

        player.transform.SetParent(null);
        player.path = newPath;
    }

    LinkedList<Cube> FindPath(Cube source, Cube dest)
    {
        if (source == null || dest == null) return null;
        if (source == dest) return null;

        Queue<Cube> queue = new Queue<Cube>();
        Dictionary<Cube, Cube> prev = new Dictionary<Cube, Cube>();
        HashSet<Cube> visited = new HashSet<Cube>();

        queue.Enqueue(source);
        visited.Add(source);

        bool found = false;

        while (queue.Count > 0)
        {
            Cube cur = queue.Dequeue();

            if (cur == null || cur.neighbors == null) continue;

            foreach (var next in cur.neighbors)
            {
                if (next == null) continue;
                if (visited.Contains(next)) continue;

                visited.Add(next);
                prev[next] = cur;
                queue.Enqueue(next);

                if (next == dest)
                {
                    found = true;
                    break;
                }
            }

            if (found) break;
        }

        if (!found) return null;

        LinkedList<Cube> path = new LinkedList<Cube>();
        Cube p = dest;

        while (p != source)
        {
            path.AddFirst(p);
            p = prev[p];
        }

        return path;
    }

    void SnapPlayerToCube(Cube cube)
    {
        player.transform.position = GetStandPosition(cube);
        player.transform.rotation = GetStandRotation(cube);
        player.transform.SetParent(cube.transform);
    }

    void AttachToCurrentCube()
    {
        if (player.current != null)
        {
            player.transform.position = GetStandPosition(player.current);
            player.transform.rotation = GetStandRotation(player.current);
            player.transform.SetParent(player.current.transform);
        }
    }

    public Vector3 GetStandPosition(Cube cube)
    {
        if (cube == null) return player.transform.position;
        return cube.GetStandPosition(player.standingHeight);
    }

    public Quaternion GetStandRotation(Cube cube)
    {
        if (cube == null) return Quaternion.identity;
        return cube.GetStandRotation();
    }
}