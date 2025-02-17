using Gists;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Purchasing;
using TMPro;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class GridTest : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<RuntimeAnimatorController> animators;
    public int numOfBalloons;
    public List<int> ids = new List<int>();
    private Vector2 bottomLeft, topRight;
    private List<Vector2> points;
    private List<Vector2> originalPoints;
    public int score = 0;
    public TMP_Text timer, help;

    // Start is called before the first frame update
    void Awake()
    {
        timer.enabled = PlayerPrefs.GetInt("mode") == 0 ? true : false;
        help.enabled = PlayerPrefs.GetInt("mode") == 1 ? true : false;
        Camera cam = Camera.main;
        bottomLeft = new Vector2(-1 * cam.orthographicSize * cam.aspect + 0.5f, -1 * cam.orthographicSize + 0.5f); // new Vector2(-11.1f, -4f);
        topRight = new Vector2(cam.orthographicSize * cam.aspect - 0.5f, cam.orthographicSize - 1.75f); // new Vector2(11.1f, 3f);
        points = PoissonDiskSampling.Sampling(bottomLeft, topRight, (float) Math.Sqrt(2));
        originalPoints = new List<Vector2>(points);
        for (int i = 0; i < numOfBalloons && points.Count > 0; i++)
        {
            summonBalloon(i);
        }
        PlayerPrefs.SetInt("score", 0);
    }

    void Update()
    {
        var scoreTextMesh = GameObject.Find("Score").GetComponent<TMP_Text>();
        scoreTextMesh.text = Regex.Replace(scoreTextMesh.text, "\\d+", PlayerPrefs.GetInt("score").ToString());
    }

    void createTile(Vector2 pos, int id)
    {
        GameObject gameObject = new GameObject();
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        int randomSprite = UnityEngine.Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[randomSprite];
        gameObject.AddComponent<clickDestroy>();
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.AddComponent<Identifier>();
        gameObject.tag = "tile";
        Animator anim = gameObject.AddComponent<Animator>();
        anim.runtimeAnimatorController = animators[randomSprite];
        
        
        GameObject tile = Instantiate(gameObject, this.transform);
        tile.transform.localPosition = pos;
        tile.GetComponent<Identifier>().id = (id + 1).ToString();
        ids.Add(id+1);
        Destroy(gameObject);
    }

    public void summonBalloon(int id)
    {
        var randPoint = points[UnityEngine.Random.Range(0, points.Count)];
        createTile(randPoint, id);
        points.Remove(randPoint);
    }

    public void resetPoints()
    {
        points.Clear();
        points = originalPoints;
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("tile"))
        {
            points.Remove(tile.transform.position);
        }
    }

    public int getPointCount()
    { 
        return points.Count;
    }
}
