using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Globalization;
using UnityEngine.SceneManagement;
using System;

public class SoketIOUpdate : MonoBehaviour
{
    private SocketIOComponent socket;
    private string room = null;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject enemy;


    private GameObject playerSpine;
    private Animator playerAnimator;
    private Dictionary<string, string> oldPlayerAnimator = new Dictionary<string, string>();

    private bool joined = false;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    void Start()
    {
        playerSpine = player.transform.Find("skeleton").Find("Hips").Find("Spine").Find("Spine1").gameObject;
        playerAnimator = player.GetComponent<Animator>();
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        InitAnimator();
        socket.On("open", TestOpen);

        StartCoroutine("SendAnimator");
        StartCoroutine("SendCurrentCoords");
        StartCoroutine("SendSpineCoords");
        socket.On("disconnect-from-room", DisconnectFromRoom);
        socket.On("display-room", DasplayRoom);
        socket.On("enter-new-player", NewPlayer);
        socket.On("update-other-player-animator", UpdateOtherPlayerAnimation);
        socket.On("update-users-cords", UpdatePlayersCords);
        socket.On("update-spine-cords", UpdateSpineCords);
        /*
          socket.On("new-bullet", NewBullet);
          StartCoroutine("SendCurrentCoords");*/





    }

    private void FixedUpdate()
    {

    }


    private void InitAnimator()
    {
        for (int i = 0; i < playerAnimator.parameterCount; i++)
        {
            if (playerAnimator.GetParameter(i).type == AnimatorControllerParameterType.Bool || playerAnimator.GetParameter(i).type == AnimatorControllerParameterType.Trigger)
            {
                oldPlayerAnimator[playerAnimator.GetParameter(i).name] = playerAnimator.GetBool(playerAnimator.GetParameter(i).name).ToString();
            }
            else if (playerAnimator.GetParameter(i).type == AnimatorControllerParameterType.Float)
            {
                oldPlayerAnimator[playerAnimator.GetParameter(i).name] = playerAnimator.GetFloat(playerAnimator.GetParameter(i).name).ToString();
            }
            else if (playerAnimator.GetParameter(i).type == AnimatorControllerParameterType.Int)
            {
                oldPlayerAnimator[playerAnimator.GetParameter(i).name] = playerAnimator.GetInteger(playerAnimator.GetParameter(i).name).ToString();
            }
        }
    }

    public void TestOpen(SocketIOEvent e)
    {
        if (!joined)
        {
            var dataToSend = new JSONObject();
            dataToSend["name"] = JSONObject.CreateStringObject("Test");
            dataToSend["room"] = JSONObject.CreateStringObject("TestRoom");
            socket.Emit("join", dataToSend);
            Debug.Log("ALLLO");
            joined = true;
        }

    }
    public void DasplayRoom(SocketIOEvent e)
    {
        //disconnectBtn.gameObject.SetActive(true);
        float x = float.Parse(e.data.GetField("posx").str.Replace(",", "."), CultureInfo.InvariantCulture);

        player.transform.position = new Vector3(x, 0.0f, -29);
    }

    public void NewPlayer(SocketIOEvent e)
    {
        float x = float.Parse(e.data.GetField("posx").str.Replace(",", "."), CultureInfo.InvariantCulture);
        players[e.data.GetField("id").str] = Instantiate(enemy, new Vector3(x, 0, -29), Quaternion.identity);
        var enemyControl = players[e.data.GetField("id").str].AddComponent<EnemyMovwSmooth>();
       // enemyControl.setTargetPosition(new Vector3(x, 0.5f, y));
    }

    public void DisconnectFromRoom(SocketIOEvent e)
    {
        Destroy(players[e.data.GetField("id").str]);
    }




    public void UpdatePlayersCords(SocketIOEvent e)
    {

        float x = float.Parse(e.data.GetField("posx").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float y = float.Parse(e.data.GetField("posy").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float z = float.Parse(e.data.GetField("posz").str.Replace(",", "."), CultureInfo.InvariantCulture);

        float rx = float.Parse(e.data.GetField("rotx").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float ry = float.Parse(e.data.GetField("roty").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float rz = float.Parse(e.data.GetField("rotz").str.Replace(",", "."), CultureInfo.InvariantCulture);

        players[e.data.GetField("id").str].GetComponent<EnemyMovwSmooth>().setTargetPosition(new Vector3(x, y, z));
        players[e.data.GetField("id").str].GetComponent<EnemyMovwSmooth>().setTargetRotation(new Vector3(rx, ry, rz));
    }

    public void UpdateSpineCords(SocketIOEvent e)
    {

        float rx = float.Parse(e.data.GetField("rotx").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float ry = float.Parse(e.data.GetField("roty").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float rz = float.Parse(e.data.GetField("rotz").str.Replace(",", "."), CultureInfo.InvariantCulture);
        float rw = float.Parse(e.data.GetField("rotw").str.Replace(",", "."), CultureInfo.InvariantCulture);


        players[e.data.GetField("id").str].GetComponent<TestAnimator>().target = new Quaternion(rx, ry, rz,rw);


        //players[e.data.GetField("id").str].GetComponent<Animator>().SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.EulerAngles(new Vector3(rx, ry, rz)));

        //players[e.data.GetField("id").str].transform.Find("skeleton").Find("Hips").Find("Spine").Find("Spine1").localEulerAngles = new Vector3(rx, ry, rz);
        //players[e.data.GetField("id").str].transform.Find("Spine1").GetComponent<EnemyMovwSmooth>().setTargetRotation(new Vector3(rx, ry, rz));
    }


    IEnumerator SendCurrentCoords()
    {
        while (true)
        {
            if (joined)
            {
                var dataToSend = new JSONObject();
                dataToSend["px"] = JSONObject.CreateStringObject(player.transform.position.x.ToString());
                dataToSend["py"] = JSONObject.CreateStringObject(player.transform.position.y.ToString());
                dataToSend["pz"] = JSONObject.CreateStringObject(player.transform.position.z.ToString());

                dataToSend["rx"] = JSONObject.CreateStringObject(player.transform.eulerAngles.x.ToString());
                dataToSend["ry"] = JSONObject.CreateStringObject(player.transform.eulerAngles.y.ToString());
                dataToSend["rz"] = JSONObject.CreateStringObject(player.transform.eulerAngles.z.ToString());
                socket.Emit("send-player-cords", dataToSend);
            }
            // yield return new WaitForSeconds(0.05f);
            yield return new WaitForEndOfFrame();


        }
    }


    IEnumerator SendSpineCoords()
    {
        while (true)
        {
            if (joined)
            {
                var dataToSend = new JSONObject();
                dataToSend["x"] = JSONObject.CreateStringObject(playerSpine.transform.localRotation.x.ToString());
                dataToSend["y"] = JSONObject.CreateStringObject(playerSpine.transform.localRotation.y.ToString());
                dataToSend["z"] = JSONObject.CreateStringObject(playerSpine.transform.localRotation.z.ToString());
                dataToSend["w"] = JSONObject.CreateStringObject(playerSpine.transform.localRotation.w.ToString());
                socket.Emit("send-spine-cords", dataToSend);
            }
            // yield return new WaitForSeconds(0.05f);
            yield return new WaitForEndOfFrame();

        }
    }


    public IEnumerator AnimationUpdate(SocketIOEvent e)
    {
        var animatorData = e.data.ToDictionary();
        string playerId = e.data.GetField("id").str;
        animatorData.Remove("id");
        Animator otherPlayerAnimator = players[playerId].GetComponent<Animator>();
        for (int i = 0; i < otherPlayerAnimator.parameterCount; i++)
        {
            var currParams = otherPlayerAnimator.GetParameter(i);
            if (animatorData.ContainsKey(currParams.name))
            {
                if (currParams.name == "Weapon")
                {
                    otherPlayerAnimator.SetInteger("Weapon", int.Parse(animatorData["Weapon"]));
                    yield return new WaitForSeconds(0.1f);
                    if (animatorData.ContainsKey("ChangeWeapon"))
                    {
                        if (bool.Parse(animatorData["ChangeWeapon"]))
                        {
                            otherPlayerAnimator.SetTrigger("ChangeWeapon");
                        }
                        else
                        {
                            otherPlayerAnimator.ResetTrigger("ChangeWeapon");
                        }

                        animatorData.Remove("ChangeWeapon");
                    }
                }

                if (currParams.type == AnimatorControllerParameterType.Bool)
                {
                    otherPlayerAnimator.SetBool(currParams.name, bool.Parse(animatorData[currParams.name]));
                }
                else if (currParams.type == AnimatorControllerParameterType.Float)
                {
                    otherPlayerAnimator.SetFloat(currParams.name, float.Parse(animatorData[currParams.name].Replace(",", "."), CultureInfo.InvariantCulture));
                }
                else if (currParams.type == AnimatorControllerParameterType.Int)
                {
                    otherPlayerAnimator.SetInteger(currParams.name, int.Parse(animatorData[currParams.name]));
                }
                else if (currParams.type == AnimatorControllerParameterType.Trigger)
                {
                    if (bool.Parse(animatorData[currParams.name]))
                    {
                        otherPlayerAnimator.SetTrigger(currParams.name);
                    }
                    else
                    {
                        otherPlayerAnimator.ResetTrigger(currParams.name);
                    }

                }
            }  
        }
        yield return null;
    }


    public void UpdateOtherPlayerAnimation(SocketIOEvent e)
    {
        StartCoroutine(AnimationUpdate(e));
    }

    IEnumerator SendAnimator()
    {
        bool changed = false;
        while (true)
        {
            if (joined)
            {

                var dataToSend = new JSONObject();
                for (int i = 0;i < playerAnimator.parameterCount; i++)
                {

                    var currParams = playerAnimator.GetParameter(i);
                    string variable;
                    
                    if (currParams.type == AnimatorControllerParameterType.Bool || currParams.type == AnimatorControllerParameterType.Trigger)
                    {
                        variable = playerAnimator.GetBool(currParams.name).ToString();
                        if (oldPlayerAnimator[currParams.name] != variable)
                        {
                            dataToSend[currParams.name] = JSONObject.CreateStringObject(variable);
                            oldPlayerAnimator[currParams.name] = variable;
                            changed = true;
                        }

                    }
                    else if (currParams.type == AnimatorControllerParameterType.Float)
                    {
                        variable = Math.Round(playerAnimator.GetFloat(currParams.name), 2).ToString();
                        if (oldPlayerAnimator[currParams.name] != variable)
                        {
                            dataToSend[currParams.name] = JSONObject.CreateStringObject(variable);
                            oldPlayerAnimator[currParams.name] = variable;
                            changed = true;
                        }
                    }
                    else if (currParams.type == AnimatorControllerParameterType.Int)
                    {
                        variable = playerAnimator.GetInteger(currParams.name).ToString();
                        if (oldPlayerAnimator[currParams.name] != variable)
                        {
                            dataToSend[currParams.name] = JSONObject.CreateStringObject(variable);
                            oldPlayerAnimator[currParams.name] = variable;
                            changed = true;
                        }
                    }
                }
                if (changed)
                {
                    socket.Emit("update-user-animation", dataToSend);
                    changed = false;
                }

            }
           // yield return new WaitForSeconds(0.01f);
            yield return new WaitForEndOfFrame();
        }
    }
}
