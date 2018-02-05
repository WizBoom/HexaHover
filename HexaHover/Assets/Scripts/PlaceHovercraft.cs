using UnityEngine;

public class PlaceHovercraft : MonoBehaviour
{
    public static void PlaceAllPlayers(GameObject playerPrefab, float radius, int indexOffset)
    {
        GameManager.PlayerInfo[] players = GameManager.Players.ToArray();
        if (players.Length == 0) return;

        float dRotation = (2.0f * Mathf.PI) / players.Length;
        float rot = indexOffset * dRotation;

        foreach (GameManager.PlayerInfo playerInfo in players)
        {
            GameObject player = Instantiate(playerPrefab);
            Hovercraft hovercraft = player.GetComponent<Hovercraft>();
            hovercraft.PlayerNumber = playerInfo.Number;
            Visuals_HoverCraft visuals = player.GetComponent<Visuals_HoverCraft>();
            visuals.GlowColor = playerInfo.Color;

            player.transform.position = new Vector3(Mathf.Cos(rot) * radius, 0.0f, Mathf.Sin(rot) * radius);
            player.transform.LookAt(Vector3.zero);

            rot += dRotation;
            Mathf.Repeat(rot, Mathf.PI * 2.0f);
        } 
    }
}
