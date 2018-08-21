using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class s_initial : MonoBehaviour {

    public GameObject MainCanvas;
    public GameObject StartCanvas;
    public GameObject CollectionCanvas;
    public GameObject Anime;
    public GameObject Anime2;

    void Start()
    {
        Anime2.GetComponent<SpriteRenderer>().sprite = null;
        MainCanvas.SetActive(false);
        StartCanvas.SetActive(false);
        CollectionCanvas.SetActive(false);

        StartCoroutine(StartAnime());
        
    }

    void Update()
    {
    }

    IEnumerator StartAnime()
    {
        Anime.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime1");
        yield return new WaitForSeconds(1f);
        Anime.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime2");
        yield return new WaitForSeconds(1f);
        for (int i=1; i <= 6; i++)
        {
            Anime.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime"+ i);
            yield return new WaitForSeconds(1f);
            if (i == 3)
            {
                Anime2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime10");
                yield return new WaitForSeconds(1f);
                Anime2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime7");
                yield return new WaitForSeconds(1f);
                Anime2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime8");
                yield return new WaitForSeconds(1f);
                Anime2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/anime9");
                yield return new WaitForSeconds(1f);
                Anime2.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        MainCanvas.SetActive(true);
        gameObject.SetActive(false);
        yield return null;
    }
}
