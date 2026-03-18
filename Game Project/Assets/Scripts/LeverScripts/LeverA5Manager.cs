using System.Collections;
using UnityEngine;

public class LeverA5Manager : MonoBehaviour
{
    [Header("Block Settings")]
    public string blockTag = "BarrierA";
    public float moveDownAmount = 2f;
    public float blockMoveSpeed = 1.5f;

    private GameObject[] blocks;
    private Vector3[] blockStartPositions;
    private Vector3[] blockLoweredPositions;

    private int activatedLeverCount = 0;
    private bool blocksMoved = false;

    private void Start()
    {
        blocks = GameObject.FindGameObjectsWithTag(blockTag);

        blockStartPositions = new Vector3[blocks.Length];
        blockLoweredPositions = new Vector3[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            blockStartPositions[i] = blocks[i].transform.position;
            blockLoweredPositions[i] = blockStartPositions[i] + new Vector3(0, -moveDownAmount, 0);
        }

        Debug.Log("LeverA5Manager found " + blocks.Length + " block(s).");
    }

    public void RegisterLeverActivation()
    {
        activatedLeverCount++;
        Debug.Log("Activated levers: " + activatedLeverCount);

        if (!blocksMoved && activatedLeverCount == 2)
        {
            blocksMoved = true;
            StartCoroutine(MoveBlocksDown());
        }
    }

    private IEnumerator MoveBlocksDown()
    {
        Debug.Log("Exactly 2 levers activated. Lowering blocks now.");

        bool blocksMoving = true;
        while (blocksMoving)
        {
            blocksMoving = false;

            for (int i = 0; i < blocks.Length; i++)
            {
                Transform block = blocks[i].transform;

                if (Vector3.Distance(block.position, blockLoweredPositions[i]) > 0.01f)
                {
                    block.position = Vector3.MoveTowards(
                        block.position,
                        blockLoweredPositions[i],
                        blockMoveSpeed * Time.deltaTime
                    );
                    blocksMoving = true;
                }
            }

            yield return null;
        }
    }

    public void ResetForNewRun()
    {
        activatedLeverCount = 0;
        blocksMoved = false;
    }
}
