using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Dreamteck.Splines;

public class Controll : MonoBehaviour
{
    [SerializeField] float xx, zz, line_dist;

    public static Controll Instance;
    [SerializeField] int cur_player, win_count;
    [SerializeField] SplineFollower foll;
    [SerializeField] bool game, draw;
    [SerializeField] float speed;

    [SerializeField] Transform pos, player;
    [SerializeField] GameObject player_prefab, short_line, play_button;

    [SerializeField] List<GameObject> all_palyer;

    Vector3 old_pos;
    [SerializeField] List<Vector3> game_pos;
    
    [SerializeField] GameObject end_panel;

    [SerializeField] Camera cam;
    [SerializeField] TrailRenderer line;
    void Start()
    {
        if (Instance == null)
            Instance = this;   
        
    }
    void Update()
    {
        if (draw)
        {
            if(Input.GetMouseButtonDown(0))
            {
                line.time = 0;
                line.gameObject.transform.position = cam.ScreenToWorldPoint(Input.mousePosition);     
                
                game_pos.Clear();
                Vector3 new_pos = cam.ScreenToWorldPoint(Input.mousePosition) - pos.position;
                print(new Vector3((new_pos.x), (new_pos.y), 0));
                old_pos = Input.mousePosition;
                game_pos.Add(new Vector3((new_pos.x * xx), 0, (new_pos.y * zz)));
            }
            if (Input.GetMouseButton(0))
            {                
                line.gameObject.transform.position =  cam.ScreenToWorldPoint(Input.mousePosition);
                if (Mathf.Abs((Input.mousePosition - old_pos).sqrMagnitude) >= line_dist)
                {
                    Vector3 add_pos = cam.ScreenToWorldPoint(Input.mousePosition) - pos.position;
                    game_pos.Add(new Vector3((add_pos.x * xx), 0, (add_pos.y * zz)));
                    old_pos = Input.mousePosition;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                line.time = 0;
                if (!game)
                {
                    if (game_pos.Count < cur_player)
                        StartCoroutine(Short());
                    else
                    {
                        play_button.SetActive(true);
                        Spawn_players();
                    }                        
                }    
                else
                    Spawn_players();
            }
        }
    }
    public void Remove_players(GameObject obj)
    {
        cur_player--;
        if (cur_player <= 0)
            Game_over();
        all_palyer.Remove(obj);
    }
    void Spawn_players()
    {
        for(int i = 0; i < cur_player; i++)
        {
            if (all_palyer.Count <= i)
            {
                GameObject pl = Instantiate(player_prefab, player_prefab.transform.parent) as GameObject;
                pl.transform.localPosition = game_pos[i];
                pl.SetActive(true);
                all_palyer.Add(pl);              
            }
            else
            {
                if (all_palyer[i].activeSelf)
                {
                    StartCoroutine(DoMove(0.3f, all_palyer[i], game_pos[i]));
                }
                else
                {
                    all_palyer[i].transform.localPosition = game_pos[i];
                    all_palyer[i].SetActive(true);
                }
            }           
        }
    }
   IEnumerator Short()
    {
        short_line.SetActive(true);
        yield return new WaitForSeconds(1);
        short_line.SetActive(false);
    }

    private IEnumerator DoMove(float time, GameObject move_player, Vector3 targetPosition)
    {
        Vector3 startPosition = move_player.transform.localPosition;
        float startTime = Time.realtimeSinceStartup;
        float fraction = 0f;
        while (fraction < 1f)
        {
            fraction = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / time);
            move_player.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, fraction);
            yield return null;
        }
    }
   
    public void Add_players()
    {
        cur_player++;
        GameObject pl = Instantiate(player_prefab, player_prefab.transform.parent) as GameObject;
        pl.transform.localPosition = new Vector3(all_palyer[all_palyer.Count / 2].transform.localPosition.x, 0, all_palyer[all_palyer.Count / 2].transform.localPosition.z - 2);
        pl.SetActive(true);
        pl.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("run");
        all_palyer.Add(pl);
    }

    public void Start_Game()
    {
        game = true;
        foll.followSpeed = speed;
        for(int i =0; i <all_palyer.Count; i++)
        {
            all_palyer[i].transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("run");
        }
    }
    public void Draw(bool drw)
    {
        draw = drw;
    }
    void Game_over()
    {
        end_panel.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "Game over";
        foll.followSpeed = 0;
        end_panel.SetActive(true);
    }
    public void Win()
    {
        win_count++;
        if(win_count >= cur_player)
        {
            foll.followSpeed = 0;
            end_panel.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "You win";
            end_panel.SetActive(true);
        }
    }
    public void Reload()
    {
        SceneManager.LoadScene("game");
    }
}
