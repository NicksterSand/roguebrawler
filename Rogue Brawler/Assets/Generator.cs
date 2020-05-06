using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
    [SerializeField]
    GameObject firstSection;

    //GameObject Prefabs for world generation
    GameObject sectionTemplate;
    GameObject activeItem;
    GameObject passiveItem;
    GameObject turret;
    GameObject roboEnemy;
    GameObject housePrefab;
    GameObject flower;
    GameObject tree;

    //Flower images
    Sprite[] flowers;

    Queue<GameObject> sections;

    static int[] enemyXValues = { -13, -8, -3, 3, 8, 13 };

    int progress;
    int itemCounter;

    private void Start() {
        //Initialize variables
        sectionTemplate = Resources.Load<GameObject>("Prefabs/section");
        activeItem = Resources.Load<GameObject>("Prefabs/activeItem");
        passiveItem = Resources.Load<GameObject>("Prefabs/passiveItem");
        turret = Resources.Load<GameObject>("Prefabs/turret");
        roboEnemy = Resources.Load<GameObject>("Prefabs/roboEnemy");
        housePrefab = Resources.Load<GameObject>("Prefabs/house");
        flower = Resources.Load<GameObject>("Prefabs/flower");
        tree = Resources.Load<GameObject>("Prefabs/tree");

        flowers = new Sprite[6];
        for(int i = 0; i < 6; i++) {
            flowers[i] = Resources.Load<Sprite>("Flowers/flower" + (i + 1));
        }

        sections = new Queue<GameObject>();
        sections.Enqueue(firstSection);

        progress = -55;
        itemCounter = 1;
        LoadNextSection();
        LoadNextSection();
    }

    void Update()
    {
        //Either load a section of land or block/unblock leftward movement
        if(transform.position.x > progress + 30) {
            LoadNextSection();
        } else if(transform.position.x < progress - 60) {
            gameObject.GetComponent<Player>().tooFarLeft = true;
        } else {
            gameObject.GetComponent<Player>().tooFarLeft = false;
        }
    }

    private void LoadNextSection() {
        GameObject section = Instantiate(sectionTemplate);
        sections.Enqueue(section);
        section.transform.position = new Vector3(progress + 90, 0, 0);

        progress += 30;

        //Determine if the section has an item box or enemies
        if (Random.value < itemCounter / 4) {
            itemCounter = 0;
            int itemID = Random.Range(0, 4);
            GameObject itemBox;

            //Make either passive or active item
            if (itemID < 3) {
                itemBox = Instantiate(passiveItem);
            } else {
                itemBox = Instantiate(activeItem);
            }

            //Setup item box
            itemBox.GetComponent<Item>().Initialize(itemID);
            itemBox.transform.position = new Vector3(progress + 60, 0, 0);
        } else {
            //Make an item box be more likely to appear
            itemCounter++;

            //Determine amount of enemies
            int enemyCount;
            float enemyRand = Random.value;

            if(enemyRand < 0.15f) {
                enemyCount = 1;
            } else if(enemyRand < 0.45f) {
                enemyCount = 2;
            } else if(enemyRand < 0.75f) {
                enemyCount = 3;
            } else {
                enemyCount = 4;
            }

            GameObject[] enemies = new GameObject[enemyCount];
            for(int i = 0; i < enemyCount; i++) {
                //Determine enemy type
                if (Random.value < 0.5f)
                    enemies[i] = Instantiate(turret);
                else
                    enemies[i] = Instantiate(roboEnemy);

                //Choose non-overlapping positions for the enemies
                Vector3 tempPos = new Vector3(0,0,0);
                bool properlyPlaced = false;

                while (!properlyPlaced) {
                    properlyPlaced = true;
                    tempPos = new Vector3(progress + 60, 0, 0);

                    tempPos.x += enemyXValues[Random.Range(0, 6)];
                    int zPos = Random.Range(0, 3);

                    if (zPos == 0)
                        tempPos.z = 3;
                    else if (zPos == 1)
                        tempPos.z = -3;

                    for(int j = 0; j < i; j++) {
                        if (Vector3.Distance(enemies[j].transform.position, tempPos) < 0.5f)
                            properlyPlaced = false;
                    }
                }

                enemies[i].transform.position = tempPos;
            }
        }
        //Make decorations

        //Place a bunch of flowers
        int flowerNum = Random.Range(20, 40);
        Vector3 flowerPos = new Vector3(0, 0, 0);
        for (int i = 0; i < flowerNum; i++) {
            GameObject newFlower = Instantiate(flower, section.transform);
            newFlower.GetComponent<SpriteRenderer>().sprite = flowers[Random.Range(0, 6)];
            flowerPos = new Vector3(Random.Range(-15f, 15f), 0.414f, 0);
            if (Random.value > .3f) {
                flowerPos.z = Random.Range(4.46f, 20f);
            } else {
                flowerPos.z = Random.Range(-9f, -5.14f);
            }
            newFlower.transform.localPosition = flowerPos;
        }

        if (Random.value < 0.1f) {
            //House
            GameObject house = Instantiate(housePrefab, section.transform);
            house.transform.localPosition = new Vector3(0, 0, 11);
        } else if (Random.value > 0.3f) {
            //Tree
            GameObject newTree = Instantiate(tree, section.transform);
            newTree.transform.localPosition = new Vector3(Random.Range(-15f, 15f), 0, Random.Range(6f, 20f));
        }
        //Remove previous sections to save memory
        if (sections.Count > 6) {
            GameObject delSection = sections.Dequeue();
            GameObject.Destroy(delSection);
        } 
    }
}
