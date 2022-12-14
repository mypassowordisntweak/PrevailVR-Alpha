using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeTest : NetworkBehaviour
{
    public static GamemodeTest instance;

    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask PlayerLayerMask;
    [SerializeField] private LayerMask localPlayerLayerMask;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float respawnTime;
    [SerializeField] private bool playBackgroundMusic;
    [SerializeField] private ItemList itemList;
    [SerializeField] private NetworkConnection localPlayer;

    private AudioSource backgroundMusic;

    public ItemList ListOfItems { get => itemList; }
    public LayerMask GetLocalPlayerLayer { get => localPlayerLayerMask; }
    public NetworkConnection LocalPlayer { get => localPlayer; set => localPlayer = value; }


    private void Start()
    {
        instance = this;
        sceneCamera = Camera.main;

        backgroundMusic = GetComponent<AudioSource>();
        backgroundMusic.Play();

        if (!playBackgroundMusic)
            MuteMusic();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Pause))
            Debug.Break();
    }

    public void MuteMusic()
    {
        backgroundMusic.mute = !backgroundMusic.mute;
    }

    //public void DamageCharacter(ICharacter VictimCharacter, DamageInfo _DamageInfo, ICharacter AttackingCharacter = null)
    //{
    //    if (VictimCharacter.TakeDamage(AttackingCharacter, _DamageInfo))
    //    { 
    //        if (VictimCharacter.PlayerConnection != null && VictimCharacter.PlayerConnection.ClientId >= 0)
    //        {
    //            StartCoroutine(RespawnPlayer(VictimCharacter.PlayerConnection));
    //        }
    //    }
    //
    //    if (AttackingCharacter != null)
    //        AttackingCharacter.GiveDamage(VictimCharacter, _DamageInfo);
    //}
    //
    //public void SetCamera(bool Active)
    //{
    //    SceneCamera.gameObject.SetActive(Active);
    //}
    //
    //IEnumerator RespawnPlayer(NetworkConnection DeadPlayer)
    //{
    //    Debug.Log("RESPAWNING: " + DeadPlayer.ClientId);
    //    yield return new WaitForSeconds(RespawnTime);
    //
    //    GameObject newPlayer = Instantiate(PlayerPrefab);
    //    newPlayer.transform.position = Vector3.zero;
    //    newPlayer.transform.rotation = Quaternion.identity;
    //
    //    base.Spawn(newPlayer, DeadPlayer);
    //}
}