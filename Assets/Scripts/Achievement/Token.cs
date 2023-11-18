using LewdieJam.Persistency;
using UnityEngine;

namespace LewdieJam.Achievement
{
    public class Token : MonoBehaviour
    {
        [SerializeField]
        private int _id;

        public int ID => _id;

        private void FixedUpdate()
        {
            transform.Rotate(0f, Time.fixedDeltaTime * 100f, 0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PersistencyManager.Instance.SaveData.GrabToken(ID);
                GameManager.Instance.StartCoroutine(GameManager.Instance.UpdateTokenDisplay());
                Destroy(gameObject);
            }
        }
    }
}
