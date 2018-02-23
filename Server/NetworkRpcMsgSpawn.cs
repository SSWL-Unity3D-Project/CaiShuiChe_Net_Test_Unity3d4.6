using UnityEngine;

/// <summary>
/// 产生循环动画网络通信.
/// </summary>
public class NetworkRpcMsgSpawn : MonoBehaviour
{
    public GameObject NetworkRpcObjPrefab;

    public void CreateNetworkRpc()
    {
        if (NetworkRpcMsgCtrl.GetInstance() != null || GlobalData.GetInstance().gameLeve != GameLeve.Movie)
        {
            return;
        }

        if (GlobalData.GetInstance().gameMode != GameMode.OnlineMode)
        {
            return;
        }

        if (NetworkRpcObjPrefab == null)
        {
            return;
        }

        GameObject obj = (GameObject)Network.Instantiate(NetworkRpcObjPrefab, Vector3.zero, Quaternion.identity, GlobalData.NetWorkGroup);
        NetworkRpcMsgCtrl netRpc = obj.GetComponent<NetworkRpcMsgCtrl>();
        netRpc.Init();
    }
}