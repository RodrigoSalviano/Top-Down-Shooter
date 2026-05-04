
using System.Collections;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody _myRigidBody;

    [SerializeField] private Vector2 ejectionVariation;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private float fadeTime = 1.5f;

    void Start()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        float force = Random.Range(ejectionVariation.x, ejectionVariation.y);
        _myRigidBody.AddForce(transform.right * force, ForceMode.Impulse);
        _myRigidBody.AddTorque(transform.right * force, ForceMode.Impulse);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifetime);

        float percent = 0f;
        float fadeSpeed = 1/fadeTime;

        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(originalColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
