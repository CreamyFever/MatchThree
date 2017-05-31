using UnityEngine;
using System.Collections;

public class DestroyObject : MonoBehaviour
{
    public float Delay = 0.5f;

	void Start()
	{
        SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.efxClip[0]);    //ブロックが消滅したら効果音を出す。
        Destroy(gameObject, Delay);
	}
}
