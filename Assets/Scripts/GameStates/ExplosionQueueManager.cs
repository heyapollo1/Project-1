using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionQueueManager : BaseManager
{
    public static ExplosionQueueManager Instance { get; private set; }
    public override int Priority => 30;
    // store queued explosions
    private Queue<QueuedExplosion> explosionQueue = new Queue<QueuedExplosion>();
    private bool isProcessingQueue = false;
    private const float explosionInterval = 0.01f; // Adjust interval to balance performance
    private const int batchSize = 2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void OnInitialize()
    {
    }
    // Struct to hold both the effect and the associated enemy GameObject
    private struct QueuedExplosion
    {
        public IOnDeathEffect Effect; // Ensure this is accessible and implements Execute(GameObject)
        public GameObject Enemy;

        public QueuedExplosion(IOnDeathEffect effect, GameObject enemy)
        {
            Effect = effect;
            Enemy = enemy;
        }
    }

    public void EnqueueExplosion(IOnDeathEffect effect, GameObject enemy)
    {
        // Add a new explosion entry to the queue
        explosionQueue.Enqueue(new QueuedExplosion(effect, enemy));
        
        // Only start processing if it isn’t already
        if (!isProcessingQueue) StartCoroutine(ProcessExplosions());
    }

    private IEnumerator ProcessExplosions()
    {
        isProcessingQueue = true;

        // Process each queued explosion in sequence
        while (explosionQueue.Count > 0)
        {
            // Process a batch of explosions
            for (int i = 0; i < batchSize && explosionQueue.Count > 0; i++)
            {
                var queuedExplosion = explosionQueue.Dequeue();

                // Ensure the effect and enemy aren't null
                if (queuedExplosion.Effect != null && queuedExplosion.Enemy != null)
                {
                    queuedExplosion.Effect.Execute(queuedExplosion.Enemy);
                }
                else
                {
                    Debug.LogWarning("Queued explosion effect or enemy was null.");
                }
            }

            // Wait briefly before processing the next batch
            yield return new WaitForSeconds(explosionInterval);
        }


        isProcessingQueue = false;
    }
}