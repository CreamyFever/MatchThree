using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
    Animator anim;
    public string ClipName, NewName;
    public int Index; // PuzzleManagerの列のインデックス(ArrayListからのインデックスをすぐに取得するために)
    public bool isDead = false;

    Vector3 oldPos;

    public GameObject Effect; // ブロックが消滅する際にポップするDestroyEffectのプレハブ

    Animal target;
    PuzzleManager manager; // PuzzleManager オブジェクト

    void Awake()
    {
        // ブロックオブジェクトの生成の当時には速度がすべて０であるため、
        // 三つ合わせられたオブジェクトが着地する前に消滅する現象が起こりうる。
        // 故にy軸の速度を-5fと設定する。
        GetComponent<Rigidbody>().velocity = new Vector3(0f, -5f, 0f);
    }

	void Start()
	{
        anim = GetComponent<Animator>();
        anim.Play(ClipName);
        manager = GameObject.Find("PuzzleManager").GetComponent<PuzzleManager>();
	}

    void Update()
    {
        if (!isDead)
        {
            if (Input.GetMouseButton(0) && GameManager.Instance.canControl)
            {
                if (manager.TouchedAnimal == null)
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (GetComponent<Rigidbody>().velocity.magnitude <= 0.3f) // 重力によって落ちないオブジェクトだけをチェック
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(pos, Vector3.forward, out hit, 100f))
                        {
                            if (hit.collider.gameObject == gameObject)
                            {
                                manager.TouchedAnimal = gameObject; // タッチしたオブジェクトを保存。
                            }
                        }
                    }
                }
            }
            else if (manager.TouchedAnimal == gameObject) // マウスから手を離した時、タッチしたオブジェクトが選択したオブジェクトなら
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // タッチしたオブジェクトと手を離した時の位置を取得する。そして、距離を求める。
                float xdist = Mathf.Abs(pos.x - transform.position.x);
                float ydist = Mathf.Abs(pos.y - transform.position.y);

                if (xdist > ydist) // スワイプした距離のXがYより長ければ、横に移動
                {
                    if (xdist >= 0.4f) // 最小限0.4以上動かした場合
                    {
                        int x = Index; // 選択したオブジェクトの列のインデックス
                        int y = manager.Block[x].IndexOf(gameObject); // 選択したオブジェクトの行のインデックス

                        // 手を離したとこのX座標がオブジェクトの左にあって、
                        // 列のインデックスが０の時はもう左に動かせないため
                        // 列のインデックスが１以上である場合にチェック
                        if (pos.x < transform.position.x && x >= 1) // 左に移動
                        {
                            manager.isMoving = true; // 移動中には、すべてのオブジェクトが重力によって落ちないようにする。

                            // 入れ替えるとこにあるターゲットのAnimalコンポーネントを取得
                            target = ((GameObject)manager.Block[x - 1][y]).GetComponent<Animal>();

                            // 選択したオブジェクトを左に移動させる
                            MoveX(target.transform.localPosition.x, target.ClipName);

                            // ターゲットを右に移動させる
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                        // 手を離したとこのX座標がオブジェクトの右にあって、
                        // 列のインデックスが５の時はもう左に動かせないため
                        // 列のインデックスが４以上である場合にチェック
                        else if (pos.x > transform.position.x && x <= 4) // 右に移動
                        {
                            manager.isMoving = true; // 移動中には、すべてのオブジェクトが重力によって落ちないようにする。

                            // 入れ替えるとこにあるターゲットのAnimalコンポーネントを取得
                            target = ((GameObject)manager.Block[x + 1][y]).GetComponent<Animal>();

                            // 選択したオブジェクトを右に移動させる
                            MoveX(target.transform.localPosition.x, target.ClipName);

                            // ターゲットを左に移動させる
                            target.MoveX(transform.localPosition.x, ClipName);
                        }
                    }
                }
                else // スワイプした距離のYがXより長ければ、縦に移動
                {
                    if (ydist >= 0.4f) // 最小限0.4以上動かした場合
                    {
                        int x = Index; // 選択したオブジェクトの列のインデックス
                        int y = manager.Block[x].IndexOf(gameObject); // 選択したオブジェクトの行のインデックス

                        // 手を離したとこのY座標がオブジェクトの上にあって、
                        // 行のインデックスが９(最上端の行)の時はもう左に動かせないため
                        // 行のインデックスが８以上である場合にチェック
                        if (pos.y > transform.position.y && y <= 8) // 上に移動
                        {
                            manager.isMoving = true; // 移動中には、すべてのオブジェクトが重力によって落ちないようにする。

                            // 入れ替えるとこにあるターゲットのAnimalコンポーネントを取得
                            target = ((GameObject)manager.Block[x][y + 1]).GetComponent<Animal>();

                            // 選択したオブジェクトを上に移動させる
                            MoveY(target.transform.localPosition.y, target.ClipName);

                            // ターゲットを下に移動させる
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                        // 手を離したとこのY座標がオブジェクトの下にあって、
                        // 行のインデックスが０(最下端の行)の時はもう左に動かせないため
                        // 行のインデックスが１以上である場合にチェック
                        else if (pos.y < transform.position.y && y >= 1) // 下に移動
                        {
                            manager.isMoving = true; // 移動中には、すべてのオブジェクトが重力によって落ちないようにする。

                            // 入れ替えるとこにあるターゲットのAnimalコンポーネントを取得
                            target = ((GameObject)manager.Block[x][y - 1]).GetComponent<Animal>();

                            // 選択したオブジェクトを下に移動させる
                            MoveY(target.transform.localPosition.y, target.ClipName);

                            // ターゲットを上に移動させる
                            target.MoveY(transform.localPosition.y, ClipName);
                        }
                    }
                }
                manager.TouchedAnimal = null;
            }
        }
        GetComponent<Rigidbody>().useGravity = !manager.isMoving;
    }
    
    /// <summary>
    /// オブジェクトを左右移動させる。
    /// 入れ替えが終わった後には、ターゲットのClipNameと交代されて、
    /// 元の位置であるoldPosになるため、ターゲットオブジェクトと
    /// 入れ替えられたように見える効果がある。 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="name"></param>
    public void MoveX(float x, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalX(gameObject, x, 0.2f).setOnComplete(MoveComplete);
    }
    
     /// <summary>
     /// オブジェクトを上下移動させる。
     /// 入れ替えが終わった後には、ターゲットのClipNameと交代されて、
     /// 元の位置であるoldPosになるため、ターゲットオブジェクトと
     /// 入れ替えられたように見える効果がある。
     /// </summary>
     /// <param name="y"></param>
     /// <param name="name"></param>
    public void MoveY(float y, string name)
    {
        NewName = name;
        oldPos = transform.localPosition;
        LeanTween.moveLocalY(gameObject, y, 0.2f).setOnComplete(MoveComplete);
    }
    
     /// <summary>
     /// 入れ替えた後、マッチしないオブジェクトがないなら
     /// 元の位置であるoldPosに戻すために使われる。
     /// </summary>
     /// <param name="pos"></param>
     /// <param name="name"></param>
    public void Move(Vector3 pos, string name)
    {
        NewName = name;
        LeanTween.moveLocal(gameObject, pos, 0.2f).setOnComplete(MoveComplete);
    }

    
    /// <summary>
    /// ブロックオブジェクトの移動が終わると呼び出される。
    /// manager.isMovingをfalseにしてすべてのオブジェクトが移動できるようにする。
    /// オブジェクトの重力(useGravity)はmanager.isMovingの逆。
    /// オブジェクトの入れ替えが終わってないと重力がfalseになり、他のオブジェクトが重力の影響を受けないようになる。
    /// 入れ替えが終わったら重力がtrueになって重力の影響を受けるようになる。
    /// これは二つのオブジェクトを入れ替える途中で、上にあるオブジェクトが落ちないようにするメソッドである。
    /// </summary>
    void MoveComplete()
    {
        ClipName = NewName; // ターゲットのClipNameに変える。

        if (target != null) // プレイヤーが動かしたオブジェクトである場合に、このルーチンを実行する。
        {
            target.ClipName = target.NewName; // ターゲットオブジェクトのClipNameをも変える。

            // 選択したオブジェクトあるいは入れ替えたターゲット、この二つの中で1個でもマッチするものがあるかをチェック
            if (manager.CheckMatch(this) || manager.CheckMatch(target))
            {
                // 新たに設定したClipNameでアニメーションをやり直す。
                // 実際イメージがこのときから交代される。
                anim.Play(ClipName);
                target.anim.Play(target.ClipName);

                // オブジェクトの種類がすでにターゲットの種類に変えられた時点なので、
                // 座標を元に戻して、入れ替えられたように見せかける。
                transform.localPosition = oldPos;
                target.transform.localPosition = target.oldPos;

                manager.isMoving = false; // 移動が終わったら、重力を働かせる。
            }
            else // 入れ替えた二つのオブジェクトの中で一つもマッチしないなら
            {
                // 二つのオブジェクトを元の位置に戻す。
                target.Move(target.oldPos, ClipName);
                Move(oldPos, target.ClipName);
            }
            target = null;
        }
    }

     /// <summary>
     /// PuzzleManagerからオブジェクトが消滅するたびに呼び出される。
     /// hideDelayはイメージが消える前にかかる遅延時間であり、タッチした順番によってどんどん長くなる。
     /// 順次的にタッチしたオブジェクトが消えるように見せかける効果がある。
     /// removeDelayは実際オブジェクトが消滅する時間。
     /// </summary>
     /// <param name="hideDelay"></param>
     /// <param name="removeDelay"></param>
    public void DestroyAnimal(float hideDelay, float removeDelay)
    {
        if (!isDead)
        {
            Invoke("Hide", hideDelay);
            Invoke("Remove", removeDelay);
            isDead = true;
        }
    }

    void Hide()
    {
        // オブジェクトのアルファ値を0fと設定する。画面上では見えないが、Colliderによるあたり判定は維持する。
        // すなわち、オブジェクトのイメージは消えたように見せかけてるので、上のオブジェクトが落ちて来ない。
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        // ブロックが消滅するときに出るDestroyEffectのプレハブを生成。
        Instantiate(Effect, transform.localPosition, Quaternion.identity);
    }

    void Remove()
    {
        manager.DeleteAnimal(gameObject); // リストからこのオブジェクトを消す。
        manager.RebornAnimal(gameObject); // 空間を埋める新しいオブジェクトを生成。
        Destroy(gameObject); // このオブジェクトを削除する。
        GotScore(10);
    }

    public void GotScore(int score)
    {
        GameManager.Instance.UpdateScore((int)(score * GameManager.Instance.comboRate));
    }
}
