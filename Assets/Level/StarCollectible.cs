using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollectible : MonoBehaviour
{
    [Header("Reference")]
    public StarSequenceManager sequenceManager;

    [Header("Optional Visual Root")]
    public GameObject visualRoot;

    [Header("Debug")]
    public bool showDebugLog = true;

    private Collider col;
    private Rigidbody rb;
    private bool collected = false;

    void Awake()
    {
        // 如果没有可视根，就默认自己
        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }

        // 保证一定有 Collider
        col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sphere = gameObject.AddComponent<SphereCollider>();
            sphere.radius = 0.5f;
            col = sphere;
        }

        col.isTrigger = true;

        // 保证一定有 Rigidbody，这样 Trigger 更稳
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void SetVisible(bool visible)
    {
        collected = false;

        if (visualRoot != null)
        {
            visualRoot.SetActive(visible);
        }

        if (col != null)
        {
            col.enabled = visible;
        }

        if (showDebugLog)
        {
            Debug.Log(name + " visible = " + visible);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (showDebugLog)
        {
            Debug.Log(name + " triggered by: " + other.name + " | tag = " + other.tag);
        }

        if (collected) return;

        if (!other.CompareTag("Player"))
        {
            if (showDebugLog)
            {
                Debug.Log(name + " ignored because tag is not Player.");
            }
            return;
        }

        Collect();
    }

    private void Collect()
    {
        if (collected) return;

        collected = true;

        if (showDebugLog)
        {
            Debug.Log(name + " collected!");
        }

        if (sequenceManager != null)
        {
            sequenceManager.NotifyStarCollected(this);
        }

        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }

        if (col != null)
        {
            col.enabled = false;
        }

        if (rb != null)
        {
            rb.detectCollisions = false;
        }
    }
}