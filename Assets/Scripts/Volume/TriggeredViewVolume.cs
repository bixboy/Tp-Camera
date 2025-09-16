using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggeredViewVolume : AViewVolume
{
    [Tooltip("Tag du joueur ou de l'objet qui peut activer ce volume.")]
    public string targetTag = "Player";

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SetActive(false);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label($"TriggeredViewVolume (UID={uid}) - Active={IsActive}");
    }
}