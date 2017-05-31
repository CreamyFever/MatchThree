using SingletonPattern;
using UnityEngine;
using System.Collections;

public class PuzzleManager : SingletonPattern<PuzzleManager>
{
    private const int X_MAX = 6;
    private const int Y_MAX = 8;
    public GameObject Animal;

    // 6X8 なので、まずはXの最大である６個のArrayListを生成する。
    // このArrayListにまたArrayListを生成して二次元配列を作る。
    public ArrayList[] Block = new ArrayList[X_MAX];

    public GameObject TouchedAnimal = null;
    public bool isMoving = false;

	void Start()
	{
        for (int i = 0; i < X_MAX; i++)
        {
            Block[i] = new ArrayList();
        }

        for (int x = 0; x < X_MAX; x++)
        {
            for (int y = 0; y < Y_MAX; y++)
            {
                Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), 5f + (y * 1.2f), 0f)));
            }
        }
	}

    void Update()
    {
        DestroyMatchedBlock();
    }

    /// <summary>
    /// マッチしたブロックオブジェクトを消滅させるメソッド
    /// </summary>
    void DestroyMatchedBlock()
    {
        // すべてのオブジェクトが移動しているか、または、消滅するうちならリターン
        for (int x = 0; x < X_MAX; x++)
        {
            for (int y = 0; y < Y_MAX; y++)
            {
                Animal target = ((GameObject)Block[x][y]).GetComponent<Animal>();
                if (target.GetComponent<Rigidbody>().velocity.magnitude > 0.3f || target.isDead)
                    return;
            }
        }

        // 縦ラインを下から上にチェック
        for (int x = 0; x < X_MAX; x++)
        {
            for (int y = 0; y < Y_MAX - 2; y++) // 同時に3個ずつチェックするので10回ではなく8回チェックする
            {
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x][y + 1]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x][y + 2]).GetComponent<Animal>();

                // 三つのオブジェクトのClipNameが同じなら消滅するようにする。
                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.2f);
                    second.DestroyAnimal(0f, 0.2f);
                    third.DestroyAnimal(0f, 0.2f);
                    KeepCombo();
                }
            }
        }

        // 横ラインを左から右にチェック
        for (int x = 0; x < X_MAX - 2; x++) // 同時に3個ずつチェックするので6回ではなく4回チェックする
        {
            for (int y = 0; y < Y_MAX; y++)
            {
                Animal first = ((GameObject)Block[x][y]).GetComponent<Animal>();
                Animal second = ((GameObject)Block[x + 1][y]).GetComponent<Animal>();
                Animal third = ((GameObject)Block[x + 2][y]).GetComponent<Animal>();

                // 三つのオブジェクトのClipNameが同じなら消滅するようにする。
                if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
                {
                    first.DestroyAnimal(0f, 0.5f);
                    second.DestroyAnimal(0f, 0.5f);
                    third.DestroyAnimal(0f, 0.5f);
                    KeepCombo();
                }
            }
        }
    }
    
     /// <summary>
     /// 三つあわせが成立したかをチェック
     /// </summary>
     /// <param name="animal"></param>
     /// <returns></returns>
    public bool CheckMatch(Animal animal)
    {
        int x = animal.Index; // animalの列のインデックス
        int y = Block[x].IndexOf(animal.gameObject); // animalの行のインデックス

        for (int i = 0; i < Y_MAX - 2; i++)
        {
            Animal first = ((GameObject)Block[x][i]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[x][i + 1]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[x][i + 2]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }

        for (int i = 0; i < X_MAX - 2; i++)
        {
            Animal first = ((GameObject)Block[i][y]).GetComponent<Animal>();
            Animal second = ((GameObject)Block[i + 1][y]).GetComponent<Animal>();
            Animal third = ((GameObject)Block[i + 2][y]).GetComponent<Animal>();

            if (first.ClipName == second.ClipName && second.ClipName == third.ClipName)
            {
                if (first == animal || second == animal || third == animal) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 無差別にブロックオブジェクトを生成する。
    /// </summary>
    /// <param name="idx">列のインデックス</param>
    /// <param name="pos">実際生成される位置</param>
    /// <returns></returns>
    public GameObject CreateRandomAnimal(int idx, Vector3 pos)
    {
        GameObject temp = Instantiate(Animal) as GameObject;
        temp.transform.parent = transform; // 生成したオブジェクトをPuzzleManagerの子にする。

        // ClipNameをchar01から06まで6種生成する。
        temp.GetComponent<Animal>().ClipName = string.Format("char{0:00}", Random.Range(1, 7));
        temp.GetComponent<Animal>().Index = idx; // 列のインデックスを入れる。
        temp.transform.localPosition = pos;
        temp.name = "Animal";
        return temp;
    }

    /// <summary>
    /// オブジェクトを消滅させる。
    /// </summary>
    /// <param name="animal"></param>
    public void DeleteAnimal(GameObject animal)
    {
        int x = animal.GetComponent<Animal>().Index; // オブジェクトの列のインデックスを取得
        Block[x].Remove(animal); // この列のリストから削除する。
    }

    /// <summary>
    /// 空間を埋めるオブジェクトを生成する。
    /// </summary>
    /// <param name="animal"></param>
    public void RebornAnimal(GameObject animal)
    {
        int x = animal.GetComponent<Animal>().Index; // オブジェクトの消滅した位置の列のインデックスを取得

        // 画面から見られないところにオブジェクトを生成して、すでに生成したオブジェクトがあるなら最後のオブジェクトのY座標から1.2f上に生成する。
        float y = Mathf.Max(5.0f, ((GameObject)Block[x][Block[x].Count - 1]).transform.position.y + 1.2f);

        // 新たに生成したオブジェクトをこの列のリストに入れる。
        Block[x].Add(CreateRandomAnimal(x, new Vector3(-2.07f + (x * 0.82f), y, 0f)));
    }

    /// <summary>
    /// コンボをつなぐ
    /// </summary>
    public void KeepCombo()
    {
        GameManager.Instance.UpdateCombo();
    }
}
