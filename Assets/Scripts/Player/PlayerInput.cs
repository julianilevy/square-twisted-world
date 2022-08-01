using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class PlayerInput : MonoBehaviour
{
    private Player _player;
    private PlayerSprite _playerSprite;
    private bool _leftTriggerPressed;
    private bool _lockedInput;
    private float _quitTime;

    void Start()
    {
        _player = GetComponent<Player>();
        _playerSprite = GetComponent<PlayerSprite>();
    }

    void LateUpdate()
    {
        if (GameManager.instance.IsPlayingLevel() && GameManager.instance.isAbleToPlay)
        {
            QuitLevel();
            if (!_player.locked)
            {
                Move();
                Jump();
                Run();
                FullMap();
                InteractWithObject();
            }
        }
    }

    void Move()
    {
        var directionalInput = new Vector2(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")));

        _player.idle = false;

        if (_lockedInput)
        {
            directionalInput = Vector2.zero;
            _player.idle = true;

            if (!_player.stats.isCarryingObject)
                _playerSprite.PlayAnimatorState("Idle");
            else
                _playerSprite.PlayAnimatorState("IdleKey");
        }
        else
        {
            if (!_player.wallSliding && !_player.jumping)
            {
                if (Mathf.Abs(directionalInput.x) > 0)
                {
                    if (!_player.falling)
                    {
                        if (!Input.GetButton("Run"))
                        {
                            if (!_player.stats.isCarryingObject)
                                _playerSprite.PlayAnimatorState("Walk");
                            else
                            {
                                if (directionalInput.x > 0)
                                    _playerSprite.PlayAnimatorState("WalkKey");
                                else
                                    _playerSprite.PlayAnimatorState("WalkKeyLeft");
                            }
                        }
                        else
                        {
                            if (!_player.stats.isCarryingObject)
                                _playerSprite.PlayAnimatorState("Run");
                            else
                            {
                                if (directionalInput.x > 0)
                                    _playerSprite.PlayAnimatorState("RunKey");
                                else
                                    _playerSprite.PlayAnimatorState("RunKeyLeft");
                            }
                        }
                    }
                    else
                    {
                        if (!_player.stats.isCarryingObject)
                            _playerSprite.PlayAnimatorState("FallingSide");
                        else
                        {
                            if (directionalInput.x > 0)
                            {
                                if (_player.fellWhileRunning)
                                    _playerSprite.PlayAnimatorState("FallingRunningSideKey");
                                else
                                    _playerSprite.PlayAnimatorState("FallingSideKey");
                            }
                            else
                            {
                                if (_player.fellWhileRunning)
                                    _playerSprite.PlayAnimatorState("FallingRunningSideKeyLeft");
                                else
                                    _playerSprite.PlayAnimatorState("FallingSideKeyLeft");
                            }
                        }
                    }
                }
                else
                {
                    if (!_player.falling)
                    {
                        _player.idle = true;

                        if (!_player.stats.isCarryingObject)
                            _playerSprite.PlayAnimatorState("Idle");
                        else
                            _playerSprite.PlayAnimatorState("IdleKey");
                    }
                    else
                    {
                        if (Mathf.Abs(_player.velocity.x) >= 5f || Mathf.Abs(directionalInput.x) > 0)
                        {
                            if (!_player.stats.isCarryingObject)
                                _playerSprite.PlayAnimatorState("FallingSide");
                            else
                            {
                                if (_player.velocity.x >= 5f)
                                {
                                    if (_player.fellWhileRunning)
                                        _playerSprite.PlayAnimatorState("FallingRunningSideKey");
                                    else
                                        _playerSprite.PlayAnimatorState("FallingSideKey");
                                }
                                else if (_player.velocity.x <= -5f)
                                {
                                    if (_player.fellWhileRunning)
                                        _playerSprite.PlayAnimatorState("FallingRunningSideKeyLeft");
                                    else
                                        _playerSprite.PlayAnimatorState("FallingSideKeyLeft");
                                }
                                else
                                    _playerSprite.PlayAnimatorState("FallingFrontKey");
                            }
                        }
                        else
                        {
                            if (!_player.stats.isCarryingObject)
                                _playerSprite.PlayAnimatorState("FallingFront");
                            else
                                _playerSprite.PlayAnimatorState("FallingFrontKey");
                        }
                    }
                }
            }
            else if (_player.jumping)
            {
                if (_player.stats.isCarryingObject)
                {
                    if (Mathf.Abs(_player.velocity.x) >= 5f)
                    {
                        if (_player.velocity.x > 0f)
                        {
                            if (_player.jumpedWhileRunning)
                                _playerSprite.PlayAnimatorState("JumpRunningSideKey");
                            else
                                _playerSprite.PlayAnimatorState("JumpSideKey");
                        }
                        else if (_player.velocity.x < 0f)
                        {
                            if (_player.jumpedWhileRunning)
                                _playerSprite.PlayAnimatorState("JumpRunningSideKeyLeft");
                            else
                                _playerSprite.PlayAnimatorState("JumpSideKeyLeft");
                        }
                    }
                    else
                    {
                        if (!_player.jumpedWhileRunning)
                            _playerSprite.PlayAnimatorState("JumpFrontKey");
                    }
                }
                else
                    _player.sprite.PlayAnimatorState("Jump");
            }
        }

        _player.SetDirectionalInput(directionalInput);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!_lockedInput)
                _player.OnJumpInputDown();
        }
        if (Input.GetButtonUp("Jump"))
            _player.OnJumpInputUp();
    }

    void Run()
    {
        var pressedRightTrigger = Mathf.Abs(Mathf.Round(Input.GetAxisRaw("Run")));
        if (_lockedInput)
            pressedRightTrigger = 0f;
        if (Input.GetButton("Run"))
        {
            if (!_lockedInput)
                _player.OnRunInput();
        }
        else if (pressedRightTrigger > 0)
            _player.SetRunAxis(pressedRightTrigger);
        else
            _player.OnRunInputUp();
    }

    void FullMap()
    {
        if (Input.GetButton("Full Map"))
        {
            _lockedInput = true;
            _player.playerCamera.transform.position = CanvasUIManager.instance.fullMapCamera.transform.position;
            Utility.CopyCamera(ref _player.playerCamera, CanvasUIManager.instance.fullMapCamera);
            CanvasUIManager.instance.blackBackground.SetActive(true);
            CanvasUIManager.instance.background.SetActive(false);
            CanvasUIManager.instance.EnableUI(false);
        }
        else
        {
            _lockedInput = false;
            _player.playerCamera.transform.position = _player.originalPlayerCamera.transform.position;
            Utility.CopyCamera(ref _player.playerCamera, _player.originalPlayerCamera);
            CanvasUIManager.instance.blackBackground.SetActive(false);
            CanvasUIManager.instance.background.SetActive(true);
            CanvasUIManager.instance.EnableUI(true);
        }
    }

    void InteractWithObject()
    {
        var pressedLeftTrigger = Mathf.Abs(Mathf.Round(Input.GetAxisRaw("Interact With Object LTrigger")));
        if (_lockedInput)
            pressedLeftTrigger = 0f;
        if (Input.GetButtonDown("Interact With Object"))
        {
            if (!_lockedInput)
                _player.OnInteractWithObject();
        }
        else if (pressedLeftTrigger > 0)
        {
            if (!_leftTriggerPressed)
            {
                _leftTriggerPressed = true;
                _player.OnInteractWithObject();
            }
        }
        else if(pressedLeftTrigger == 0)
            _leftTriggerPressed = false;
    }

    void QuitLevel()
    {
        if (Input.GetButtonDown("Quit Level Instant"))
            StartCoroutine(ExecuteQuitLevel());
        if (Input.GetButton("Quit Level Timer A") && Input.GetButton("Quit Level Timer B"))
        {
            _quitTime += Time.deltaTime;
            if (_quitTime >= 1.5f)
                StartCoroutine(ExecuteQuitLevel());
        }
        else
            _quitTime = 0f;
    }

    IEnumerator ExecuteQuitLevel()
    {
        CanvasUIManager.instance.EnableFade(CanvasUIManager.instance.diagonalLinesFade);

        yield return new WaitForSeconds(GameManager.fadeTime);

        GameManager.instance.QuitLevel();
    }
}