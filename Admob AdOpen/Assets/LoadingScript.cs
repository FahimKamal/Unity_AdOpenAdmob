using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScript : MonoBehaviour
{
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private Text loadingPercent;
    [SerializeField] private Transform spinner;
    private float _loadPercent;

    [SerializeField] private AdManager AM;
    // Start is called before the first frame update
    void Start()
    {
        _loadPercent = 0;
        // StartCoroutine(ShowAppOpenAd());
    }

    // Update is called once per frame
    void Update()
    {
        _loadPercent += 100 * Time.deltaTime / 8;
        loadingPercent.text = Mathf.Floor(_loadPercent) + "%";
        spinner.eulerAngles += new Vector3(0,0,50) * Time.deltaTime;

        if (_loadPercent >= 100)
        {
            loadingMenu.SetActive(false);

        }
    }

    IEnumerator ShowAppOpenAd()
    {
        yield return new WaitForSeconds(7);
        AM.ShowAppOpenAd();
    }
}
