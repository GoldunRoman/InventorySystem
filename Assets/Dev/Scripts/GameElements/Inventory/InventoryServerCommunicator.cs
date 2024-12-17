using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryServerCommunicator
{
    private const string ServerUrl = "https://wadahub.manerai.com/api/inventory/status";
    private const string AuthorizationToken = "kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP";

    public async Task SendItemStatusAsync(int itemId, string status)
    {
        var bodyJson = JsonUtility.ToJson(new { itemId, status });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJson);

        using (var request = new UnityWebRequest(ServerUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {AuthorizationToken}");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Server request failed: " + request.error);
            }
        }
    }
}