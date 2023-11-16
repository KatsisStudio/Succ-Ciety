using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using LewdieJam.VN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LewdieJam.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private Image _healthBar;

        [SerializeField]
        private AudioClip[] _atkSounds, _kissSounds, _painSounds, _gameoverSounds, _dashSounds;

        [SerializeField]
        private GameObject _gameoverPopup;

        public IInteractible CurrentInteraction { set; private get; }

        /// <summary>
        /// Current movement
        /// </summary>
        private Vector2 _mov;
        /// <summary>
        /// Last direction we went in, can't be 0;0
        /// </summary>
        private Vector2 _dashDirection = Vector2.up;

        /// <summary>
        /// When taking a hit, need to wait a bit before being able to take a new one
        /// </summary>
        private bool _isInvulnerabilityFrame;

        private float _invultDuration = 2f;
        private float _invultFrameIntensity = .5f;

        private Animator _anim;

        private bool _isDashing;

        public bool CanMove => !_isAttacking && !IsStunned && IsAlive;

        protected override int MaxHealth => _info.BaseHealth + _info.BaseHealth * (int)GameManager.Instance.GetStatValue(UpgradableStat.BaseHealth, GameManager.Instance.Info.MaxHealthCurveGain, GameManager.Instance.Info.MaxHealthMultiplerGain);

        private Dictionary<Skill, bool> _skills = new()
        {
            { Skill.MainAttack, true },
            { Skill.SubAttack, true },
            { Skill.Dash, true }
        };

        private AEnemyController _charmed;

        private AudioSource _source;

        public bool IsStunned { private get; set; }

        private void Awake()
        {
            AwakeParent();
            _anim = GetComponentInChildren<Animator>();
            _source = GetComponentInChildren<AudioSource>();
        }

        private void Start()
        {
            StartParent();

            var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
            transform.position = new(spawnPoint.transform.position.x, transform.position.y, spawnPoint.transform.position.z);
        }

        private void Update()
        {
            UpdateParent();
        }

        private void FixedUpdate()
        {
            if (!GameManager.Instance.CanPlay || !CanMove)
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
                _anim.SetFloat("Speed", 0f);
            }
            else if (_isDashing)
            {
                _rb.velocity = new Vector3(_dashDirection.x, 0f, _dashDirection.y) * _info.Speed * _info.DashSpeedMultiplier;
            }
            else
            {
                var dir = _mov.y * transform.forward + _mov.x * transform.right;

                if (!_isDashing && _mov.magnitude != 0)
                {
                    _dashDirection = new(dir.x, dir.z);
                }

                dir *= _info.Speed;
                dir.y = _rb.velocity.y;

                _rb.velocity = dir;
                _anim.SetFloat("Speed", _mov.magnitude);
                if (_mov.x != 0f)
                {
                    _sr.flipX = _mov.x < 0f; 
                }
            }
        }

        protected override bool CanTakeDamage => !_isInvulnerabilityFrame;

        public override Team Team => Team.Allie;

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            if (damage > 0)
            {
                StartCoroutine(DisplayInvulnerability());
                _healthBar.transform.localScale = new Vector3(_health / (float)MaxHealth, 1f, 1f);
                _source.PlayOneShot(_painSounds[UnityEngine.Random.Range(0, _painSounds.Length)]);
            }
        }

        private IEnumerator DisplayInvulnerability()
        {
            _isInvulnerabilityFrame = true;
            bool state = true;
            for (float i = 0; i < _invultDuration; i += _invultFrameIntensity)
            {
                yield return new WaitForSeconds(_invultFrameIntensity);
                state = !state;
                _sr.color = state ? Color.white : new Color(1f, 1f, 1f, 0f);
            }

            _sr.color = Color.white;
            _isInvulnerabilityFrame = false;
        }

        public override void Die()
        {
            PersistentData.Energy += PersistentData.PendingEnergy / 2;
            PersistentData.PendingEnergy = 0;
            _gameoverPopup.SetActive(true);
            _source.PlayOneShot(_gameoverSounds[UnityEngine.Random.Range(0, _gameoverSounds.Length)]);
        }

        public void LoadLobby()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();
        }

        private SpellHitInfo FireOnTarget()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, _floorMask))
            {
                // Calculate mouse forward vector
                var direction = hit.point - transform.position;
                direction.y = 0f;
                var dirNorm = direction.normalized;
                var targetPos = transform.position + dirNorm * _info.Range;

                // Damage all enemies in range
                var colliders = Physics.OverlapSphere(targetPos, _info.Range, _characterMask);
                return new SpellHitInfo()
                {
                    Hits = colliders.Where(x => x.CompareTag("Enemy") && x != null).Select(x => x.GetComponent<ACharacter>()),
                    Point = targetPos
                };
            }
            return null;
        }

        private IEnumerator Attack(Skill s, Action<IEnumerable<ACharacter>> attack, GameObject vfx, float reloadTime, AudioClip sound)
        {
            _skills[s] = false;
            _isAttacking = true;

            var atk = FireOnTarget();

            if (atk == null) // We fired outside of the map, not supposed to happen...
            {
                _isAttacking = false;
                _skills[s] = true;
                _anim.SetInteger("Attack", 0);
            }
            else
            {
                _source.PlayOneShot(sound);
                _sr.flipX = transform.position.x - atk.Point.x > 0f;

                yield return new WaitForSeconds(_info.PreAttackWaitTime);
                Destroy(Instantiate(vfx, atk.Point, vfx.transform.rotation), 1f);
                attack(atk.Hits);
                yield return new WaitForSeconds(_info.PostAttackWaitTime);
                _isAttacking = false;
                _anim.SetInteger("Attack", 0);
                yield return Reload(s, reloadTime);
            }
        }

        private IEnumerator Reload(Skill s, float time)
        {
            yield return new WaitForSeconds(time);
            _skills[s] = true;
        }

        private IEnumerator Dash(float reloadTime)
        {
            _skills[Skill.Dash] = false;
            _anim.SetBool("IsDashing", true);
            _isDashing = true;
            _source.PlayOneShot(_dashSounds[UnityEngine.Random.Range(0, _dashSounds.Length)]);
            yield return new WaitForSeconds(_info.DashDuration);
            _anim.SetBool("IsDashing", false);
            _isDashing = false;

            yield return Reload(Skill.Dash, reloadTime);
        }

        private void NormalAttack(IEnumerable<ACharacter> targets)
        {
            var damage = _info.AttackForce + Mathf.CeilToInt(GameManager.Instance.GetStatValue(UpgradableStat.AtkPower, GameManager.Instance.Info.AtkCurveGain, GameManager.Instance.Info.MaxAtkMultiplerGain));
            foreach (var coll in targets)
            {
                coll.TakeDamage(damage);
            }
        }

        private void CharmAttack(IEnumerable<ACharacter> targets)
        {
            var target = targets.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).FirstOrDefault();
            if (target != null)
            {
                // Un-charm the last enemy charmed
                if (_charmed != null)
                {
                    _charmed.IsCharmed = false;
                }

                // Charm new target
                _charmed = (AEnemyController)target;
                _charmed.IsCharmed = true;
            }
        }

        public void OnFire(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (VNManager.Instance.IsPlayingStory)
                {
                    VNManager.Instance.DisplayNextDialogue();
                }
                else if (GameManager.Instance.CanPlay && CanMove && _skills[Skill.MainAttack])
                {
                    _anim.SetInteger("Attack", 3);
                    StartCoroutine(Attack(Skill.MainAttack, NormalAttack, _info.MainAttackVfx, _info.MainAttackReloadTime, _atkSounds[UnityEngine.Random.Range(0, _atkSounds.Length)]));
                }
            }
        }

        public void OnUltimate(InputAction.CallbackContext value)
        {
            if (value.performed && GameManager.Instance.CanPlay && CanMove && _skills[Skill.SubAttack])
            {
                _anim.SetInteger("Attack", 2);
                StartCoroutine(Attack(Skill.SubAttack, CharmAttack, _info.SubAttackVfx, _info.SubAttackReloadTime, _kissSounds[UnityEngine.Random.Range(0, _kissSounds.Length)]));
            }
        }

        public void OnDash(InputAction.CallbackContext value)
        {
            if (value.performed && GameManager.Instance.CanPlay && CanMove && _skills[Skill.Dash])
            {
                StartCoroutine(Dash(2f));
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed && GameManager.Instance.CanPlay && CanMove && CurrentInteraction != null && CurrentInteraction.CanInteract(this))
            {
                CurrentInteraction.Interact();
            }
        }

        private enum Skill
        {
            MainAttack,
            SubAttack,
            Dash
        }

        private class SpellHitInfo
        {
            public IEnumerable<ACharacter> Hits { set; get; }
            public Vector3 Point { set; get; }
        }
    }
}