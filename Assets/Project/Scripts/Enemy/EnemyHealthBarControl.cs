using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarControl : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform healthbarFill;
    [SerializeField] private Enemy enemy;

    private void OnEnable()
    {
        enemy.OnHealthChange += UpadteHealthBar;
        enemy.OnDeath += DestroyHealthBar;
        canvas.worldCamera = Camera.main;

    }

    private void LateUpdate()
    {
        if(canvas != null)
        {
            canvas.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }

    private void OnDisable()
    {
        enemy.OnHealthChange -= UpadteHealthBar;
    }

    private void DestroyHealthBar()
    {
        Destroy(gameObject);
    }

    private void UpadteHealthBar(float health)
        {
            healthbarFill.localScale = new Vector3(Mathf.Clamp(health / enemy.startingHealth, 0, 1), 1, 1);
        }
}