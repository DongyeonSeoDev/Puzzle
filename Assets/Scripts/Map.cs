using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform blockPositionImages = null;
    [SerializeField] private Transform bulletObject = null;

    [Header("(eBlockType 순서: red, blue, purple, orange, green)")]
    [Header("eBlockType 순서대로 넣어주어야 합니다.")]
    
    [SerializeField] private List<GameObject> blockList = new List<GameObject>();
    [SerializeField] private List<GameObject> blockPool = new List<GameObject>();

    private Transform[][] blockPositions = new Transform[9][];
    private List<List<Block>> blocks = new List<List<Block>>();

    private void Awake()
    {
        for (int i = 0; i < 9; i++)
        {
            Transform blockPosition = blockPositionImages.GetChild(i);
            blockPositions[i] = new Transform[9];

            for (int j = 0; j < 9; j++)
            {
                blockPositions[i][j] = blockPosition.GetChild(j);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            List<Block> block = new List<Block>();

            for (int j = 0; j < 9; j++)
            {
                block.Add(CreateBlock(Random.Range(0, 5)).GetComponent<Block>());
                block[j].gameObject.SetActive(true);
                block[j].gameObject.transform.position = blockPositions[i][j].position;
            }

            blocks.Add(block);
        }

        bool isChange = true;

        while (true)
        {
            isChange = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (blocks[i][j].blockType == blocks[i][j + 1].blockType && blocks[i][j + 1].blockType == blocks[i][j + 1].blockType)
                    {
                        int randomNum;

                        do
                        {
                            randomNum = Random.Range(0, 5);
                        }
                        while (blocks[i][j].blockType == (eBlockType)randomNum);

                        blocks[i][j].gameObject.SetActive(false);

                        Block block = CreateBlock(randomNum).GetComponent<Block>();
                        block.gameObject.SetActive(true);
                        block.gameObject.transform.position = blockPositions[i][j].position;

                        blocks[i][j] = block;
                        isChange = true;
                    }
                }
            }

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (blocks[i][j].blockType == blocks[i + 1][j].blockType && blocks[i + 1][j].blockType == blocks[i + 1][j].blockType)
                    {
                        int randomNum;

                        do
                        {
                            randomNum = Random.Range(0, 5);
                        }
                        while (blocks[i][j].blockType == (eBlockType)randomNum);

                        blocks[i][j].gameObject.SetActive(false);

                        Block block = CreateBlock(randomNum).GetComponent<Block>();
                        block.gameObject.SetActive(true);
                        block.gameObject.transform.position = blockPositions[i][j].position;

                        blocks[i][j] = block;
                        isChange = true;
                    }
                }
            }

            if (!isChange)
            {
                break;
            }
        }
    }

    private GameObject CreateBlock(int type)
    {
        blockPool.Add(Instantiate(blockList[type], bulletObject));
        blockPool[blockPool.Count - 1].SetActive(false);

        return blockPool[blockPool.Count - 1];
    }
}
