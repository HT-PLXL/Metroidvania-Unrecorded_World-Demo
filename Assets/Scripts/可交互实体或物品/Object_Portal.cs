using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Portal : MonoBehaviour,ISaveable
{
    public static Object_Portal instance;

    public bool IsActive { get; private set; }
    [SerializeField] private Vector2 defaultPosition;
    //[SerializeField] private string sceneName = "Level_0";

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered;

    private string currentSceneName;

    private void Awake()
    {
        instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
        transform.position = new Vector3(6657, 6657); // Hide by default
    }

    public void ActivatePortal(Vector3 position, int facingDir = 1)
    {
        IsActive = true;
        transform.position = position;

        if (facingDir == -1)
            transform.Rotate(0, 180, 0);
        SaveManager.instance.GetGameData().scenePortals.Clear();//清理字典
    }

    private void UseTeleport()  
    {
        GameSceneManager.instance.ChangeScene(currentSceneName, RespawnType.Portal);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false)
            return;

        UseTeleport();
    }

    private void OnTriggerExit2D(Collider2D collision) => canBeTriggered = true;

    public void SetTrigger(bool trigger) => canBeTriggered = trigger;

    public Vector3 GetPosition() => respawnPoint != null ? respawnPoint.position : transform.position;

    public void SavaData(ref GameData data)
    {
        //if(IsActive)
        //{
        //    data.scenePortals[currentSceneName] = transform.position;
        //}
        //else
        //    data.scenePortals.Remove(currentSceneName);
    }

    public void loadData(GameData data)
    {
        //data.scenePortals.TryGetValue(currentSceneName, out Vector3 portalPosition);
        //transform.position = portalPosition;
        //IsActive = true;
    }
}
