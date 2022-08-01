using UnityEngine;

[RequireComponent(typeof(SimpleBlit))]
public class FadeTransition : MonoBehaviour
{
    private SimpleBlit _simpleBlit;
    private bool _enablingFade;
    private bool _disablingFade;
    private float _progress;
    private float _time;

    private void Awake()
    {
        _simpleBlit = GetComponent<SimpleBlit>();
    }

    public void Initialize(Material material, float value)
    {
        GameManager.instance.lastFadedMaterial = material;
        _simpleBlit.transitionMaterial = material;
        _simpleBlit.transitionMaterial.SetFloat("_Cutoff", value);
    }

    private void Update()
    {
        if (_enablingFade)
        {
            _progress += Time.fixedDeltaTime / _time;
            _simpleBlit.transitionMaterial.SetFloat("_Cutoff", _progress);

            if (_progress >= 1f)
                _enablingFade = false;
        }
        if (_disablingFade)
        {
            _progress -= Time.fixedDeltaTime / _time;
            _simpleBlit.transitionMaterial.SetFloat("_Cutoff", _progress);

            if (_progress <= 0f)
                _disablingFade = false;
        }
    }

    public void EnableFade(float time)
    {
        _enablingFade = true;
        _disablingFade = false;
        _progress = 0f;
        _time = time;
    }

    public void DisableFade(float time)
    {
        _enablingFade = false;
        _disablingFade = true;
        _progress = 1f;
        _time = time;
    }
}