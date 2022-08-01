using UnityEngine;
using System.Collections;

public class PlayerSprite : MonoBehaviour
{
    [HideInInspector]
    public SpriteRenderer baseSprite;
    [HideInInspector]
    public SpriteRenderer eyesEmission;
    [HideInInspector]
    public SpriteRenderer borderEmission;
    [HideInInspector]
    public ParticleSystem currentGravityParticles;
    [HideInInspector]
    public ParticleSystem startRunningParticles;
    [HideInInspector]
    public ParticleSystem deathParticles;
    [HideInInspector]
    public Animator baseAnimator;
    [HideInInspector]
    public Animator eyesEmissionAnimator;
    [HideInInspector]
    public Animator borderEmissionAnimator;

    private Player _player;
    private SpriteRenderer[] _allSprites = new SpriteRenderer[3];
    private Color[] _allColorsOpaque = new Color[3];
    private Color[] _allColorsTransparent = new Color[3];
    private bool _deathParticlesEnabled;
    private float _originalGravityParticlesEmissionRate;
    private float _runningParticlesCooldown = 1f;
    private bool _canPlayRunningParticles = true;
    private bool _wasRunningLastFrame;

    void Awake()
    {
        _player = GetComponent<Player>();
        _allSprites[0] = baseSprite;
        _allSprites[1] = eyesEmission;
        _allSprites[2] = borderEmission;
        _originalGravityParticlesEmissionRate = currentGravityParticles.emission.rateOverTime.constant;
        startRunningParticles.Stop();
        deathParticles.Stop();
    }

    void Update()
    {
        ChangeDirection();
        UpdateColorAndParticles();
        RunningParticles();
    }

    void ChangeDirection()
    {
        for (int i = 0; i < _allSprites.Length; i++)
        {
            if (!_player.stats.isCarryingObject)
            {
                if (_player.directionalInput.x == 1)
                    _allSprites[i].flipX = true;
                else if (_player.directionalInput.x == -1)
                    _allSprites[i].flipX = false;
            }
            else
                _allSprites[i].flipX = true;
        }
    }

    void UpdateColorAndParticles()
    {
        var gravityParticlesMain = currentGravityParticles.main;
        var startRunningParticlesMain = startRunningParticles.main;
        if (_player.currentGravity == Gravity.UP)
        {
            _allSprites[1].material.color = Gravity.COLOR_UP;
            _allSprites[2].material.color = Gravity.COLOR_UP;
            gravityParticlesMain.startColor = Gravity.COLOR_UP;
            startRunningParticlesMain.startColor = Gravity.COLOR_UP;
        }
        else if (_player.currentGravity == Gravity.RIGHT)
        {
            _allSprites[1].material.color = Gravity.COLOR_RIGHT;
            _allSprites[2].material.color = Gravity.COLOR_RIGHT;
            gravityParticlesMain.startColor = Gravity.COLOR_RIGHT;
            startRunningParticlesMain.startColor = Gravity.COLOR_RIGHT;
        }
        else if (_player.currentGravity == Gravity.LEFT)
        {
            _allSprites[1].material.color = Gravity.COLOR_LEFT;
            _allSprites[2].material.color = Gravity.COLOR_LEFT;
            gravityParticlesMain.startColor = Gravity.COLOR_LEFT;
            startRunningParticlesMain.startColor = Gravity.COLOR_LEFT;
        }
        else if (_player.currentGravity == Gravity.DOWN)
        {
            _allSprites[1].material.color = Gravity.COLOR_DOWN;
            _allSprites[2].material.color = Gravity.COLOR_DOWN;
            gravityParticlesMain.startColor = Gravity.COLOR_DOWN;
            startRunningParticlesMain.startColor = Gravity.COLOR_DOWN;
        }
    }

    void RunningParticles()
    {
        var emission = currentGravityParticles.emission;
        if (_player.running)
            emission.rateOverTime = _originalGravityParticlesEmissionRate * 3;
        else
            emission.rateOverTime = _originalGravityParticlesEmissionRate;

        if (_canPlayRunningParticles)
        {
            if (_player.running && _wasRunningLastFrame != _player.running)
            {
                startRunningParticles.Play();
                StartCoroutine(RunningParticlesCooldown());
            }
        }
        _wasRunningLastFrame = _player.running;
    }

    public void PlayDeathParticles()
    {
        if (!_deathParticlesEnabled)
        {
            deathParticles.Play();
            _deathParticlesEnabled = true;
        }
    }

    public void PlayAnimatorState(string stateName)
    {
        baseAnimator.Play(stateName);
        eyesEmissionAnimator.Play(stateName);
        borderEmissionAnimator.Play(stateName);
    }

    public void SetActive(bool value)
    {
        for (int i = 0; i < _allSprites.Length; i++)
            _allSprites[i].enabled = value;
    }

    IEnumerator RunningParticlesCooldown()
    {
        _canPlayRunningParticles = false;
        yield return new WaitForSeconds(_runningParticlesCooldown);
        _canPlayRunningParticles = true;
        startRunningParticles.Stop();
    }
}