using Unity.VisualScripting;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour{
    [SerializeField] private Sprite[] flashSprites;
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    [SerializeField] private GameObject muzzleFlashHolder;
    [SerializeField] private float flashDuration;
    [SerializeField] private Color _color;

    private void Start()
    {
        
        for(int i =0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = _color;
        }
        Deactivate();
    }

    public void Activate()
    {   
        muzzleFlashHolder.SetActive(true);
        Sprite currentFlash = flashSprites[Random.Range(0, flashSprites.Length)];
        for(int i =0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = currentFlash;
        }
        Invoke(nameof(Deactivate), flashDuration);
    }

    private void Deactivate()
    {
        muzzleFlashHolder.SetActive(false);
    }
} 