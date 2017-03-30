using UnityEngine;

public class MagicalRing : MonoBehaviour
{
	private bool active = true;

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerControl>())
        {
            GetComponent<Animation>().Play();
            GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.GetComponent<PlayerControl>())
        {
            GameComponents.GetGameController().RingActivated();
            GetComponent<MeshCollider>().enabled = false;
        	Hide();
        }
    }

    private void Hide()
    {
    	active = false;
    	foreach(Renderer r in GetComponentsInChildren<Renderer>())
        {
        	r.enabled = false;
        }
    }

    public bool IsActive()
    {
    	return active;
    }
}