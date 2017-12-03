using System;
using System.Collections;
using UnityEngine;

//http://baba-s.hatenablog.com/entry/2014/05/07/165344
/// <summary>
/// コルーチンに関する汎用クラス
/// </summary>
public static class CoroutineCommon
{
    private static readonly MonoBehaviour mMonoBehaviour;

    /// <summary>
    /// コルーチンを管理するゲームオブジェクトを生成するコンストラクタ
    /// </summary>
    static CoroutineCommon()
    {
        var gameObject = new GameObject("CoroutineCommon");
        GameObject.DontDestroyOnLoad(gameObject);
        mMonoBehaviour = gameObject.AddComponent<MonoBehaviour>();
    }

    /// <summary>
    /// 1 フレーム待機してから Action デリゲートを呼び出します
    /// </summary>
    public static void CallWaitForOneFrame(Action act)
    {
        mMonoBehaviour.StartCoroutine(DoCallWaitForOneFrame(act));
    }

    /// <summary>
    /// 指定された秒数待機してから Action デリゲートを呼び出します
    /// </summary>
    public static void CallWaitForSeconds(float seconds, Action act)
    {
        mMonoBehaviour.StartCoroutine(DoCallWaitForSeconds(seconds, act));
    }

    /// <summary>
    /// 1 フレーム待機してから Action デリゲートを呼び出します
    /// </summary>
    private static IEnumerator DoCallWaitForOneFrame(Action act)
    {
        yield return 0;
        act();
    }

    /// <summary>
    /// 指定された秒数待機してから Action デリゲートを呼び出します
    /// </summary>
    private static IEnumerator DoCallWaitForSeconds(float seconds, Action act)
    {
        yield return new WaitForSeconds(seconds);
        act();
    }
}