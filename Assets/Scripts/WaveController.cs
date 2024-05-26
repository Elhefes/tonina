using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;

public class WaveController : MonoBehaviour
{
    public ThreatLevels threatLevels;
    private ThreatLevels.ThreatLevel threatLevel;
    private int currentRoundNumber;
    public TextAsset txt;
    private int friendlyWarriorsAmount;
    private bool isSpawningEnemies;
    private int secondsInBattle;

    public Transform[] spawnPoints;
    private Vector3 housePos;

    private List<Coroutine> coroutines;
    private Coroutine rewardsRisingCoroutine;
    public GameObject overworldOptionsButton;
    public GameObject battleUI;
    public GameObject battleWinningScreen;
    public TMP_Text threatLevelText;
    public TMP_Text battleTimeText;
    public TMP_Text rewardsText;
    public GameObject enemyPrefab;
    public GameObject friendlyWarriorPrefab;
    private GameObject kingHouse;

    public MusicPlayer musicPlayer;

    void Start()
    {
        kingHouse = GameObject.Find("king_house");
        housePos = kingHouse.transform.position;
        coroutines = new List<Coroutine>();
    }

    public void StartRound(int roundNumber, int battleSongID)
    {
        currentRoundNumber = roundNumber;
        threatLevel = threatLevels.GetThreatLevel(roundNumber - 1);
        friendlyWarriorsAmount = threatLevel.friendlyWarriorsAmount;
        coroutines.Add(StartCoroutine(ParseRound(threatLevel.wave)));
        musicPlayer.PlayBattleSong(battleSongID);
        EnableBattleUI();
        StartCoroutine(SecondCounter());
    }

    void EnableBattleUI()
    {
        overworldOptionsButton.SetActive(false);
        battleUI.SetActive(true);
    }

    void DisableBattleUI()
    {
        battleUI.SetActive(false);
        overworldOptionsButton.SetActive(true);
    }

    public IEnumerator ParseRound(string round)
    {
        List<string> bits = new(round.Split(' '));
        isSpawningEnemies = true;
        bool skipToNextBit = false;
        int index;

        if (friendlyWarriorsAmount > 0) SpawnFriendlies(friendlyWarriorsAmount);

        foreach (string bit in bits)
        {
            if (bit.StartsWith("P"))
            {
                int delay = Int32.Parse(bit.Substring(1, bit.Length - 1));
                float spawnHaltDelay = (float)delay / 10;
                yield return new WaitForSeconds(spawnHaltDelay);
            }
            else
            {
                var splittedBit = bit.Split('-');
                Int32.TryParse(splittedBit[1], out index);
                string bitStart = splittedBit[0];
            
                if (bitStart.StartsWith("X"))
                {
                    skipToNextBit = true;
                }
                int enemyCount = 0;
                string intConstructor = "";
                char enemy = 'a';

                foreach (char c in bitStart)
                {
                    if (Char.IsDigit(c))
                    {
                        intConstructor += c;
                    }
                    else if (Char.IsLetter(c))
                    {
                        enemy = c;
                        Int32.TryParse(intConstructor, out enemyCount);
                        intConstructor = "";
                    }
                }

                int time = Int32.Parse(intConstructor);
                float spawnDuration = (float)time / 10 / (float)enemyCount;

                int spawned = 0;
                while (spawned < enemyCount)
                {
                    SpawnEnemyOfType(enemy, index);
                    spawned++;
                    if (!skipToNextBit) yield return new WaitForSeconds(spawnDuration);
                }
            }
        }
        InvokeRepeating("CheckForEnemies", 1f, 1f);
    }

    void SpawnEnemyOfType(char c, int i)
    {
        if (c.Equals('A')) Instantiate(enemyPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
        return;


        string enemyName = "";
        if (c.Equals('A')) enemyName = "ScarredMummy";
        else if (c.Equals('B')) enemyName = "MintMummy";
        else if (c.Equals('C')) enemyName = "ArmoredMummy";
        else if (c.Equals('D')) enemyName = "Skeleton";
        else if (c.Equals('E')) enemyName = "GoldenSkeleton";
        else if (c.Equals('F')) enemyName = "MummyWagon";
        else if (c.Equals('G')) enemyName = "SkeletonWagon";
        else if (c.Equals('H')) enemyName = "StoneMan";
        else if (c.Equals('I')) enemyName = "Sandman";
        else if (c.Equals('J')) enemyName = "DarkSandman";
        else if (c.Equals('K')) enemyName = "Ibis";
        else if (c.Equals('L')) enemyName = "MummyKing";
        else if (c.Equals('M')) enemyName = "UndeadChariot";
        else if (c.Equals('N')) enemyName = "MiniMummyKing";
        else if (c.Equals('Q')) enemyName = "Anubis";
        else if (c.Equals('R')) enemyName = "Khnum";
        else if (c.Equals('S')) enemyName = "Sobek";
        // gameController.SpawnEnemy(enemyName);
    }

    void CheckForEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            isSpawningEnemies = false;
            CancelInvoke("CheckForEnemies");
            WinBattle();
        }
    }

    void WinBattle()
    {
        battleWinningScreen.SetActive(true);
        threatLevelText.text = currentRoundNumber.ToString();
        battleTimeText.text = GetBattleTimerString(secondsInBattle);
        rewardsText.text = GetTotalRewards().ToString();
        StopCoroutine(SecondCounter());
        rewardsRisingCoroutine = StartCoroutine(PlayRewardsRisingAnimation());
        DisableBattleUI();
    }

    int GetTotalRewards()
    {
        if (secondsInBattle <= threatLevel.timeLimitForBonusReward) return threatLevel.baseReward + threatLevel.timeBonusReward;
        return threatLevel.baseReward;
    }

    private IEnumerator PlayRewardsRisingAnimation()
    {
        int tempRewards = 0;
        int totalRewardsNumber = GetTotalRewards();
        while (tempRewards < totalRewardsNumber)
        {
            rewardsText.text = tempRewards.ToString();
            tempRewards += 1;
            yield return new WaitForSeconds(2.5f / totalRewardsNumber);
        }
        if (tempRewards <= totalRewardsNumber)
        {
            rewardsText.text = totalRewardsNumber.ToString();
        }
    }

    void SpawnFriendlies(int friendlyWarriorsAmount)
    {
        float xOffset = -11f;
        float zOffset = -11f;
        float xStep = 1.5f;
        float zStep = 1.5f;

        int pairs = friendlyWarriorsAmount / 2;
        bool isOdd = friendlyWarriorsAmount % 2 != 0;

        for (int i = 0; i < pairs; i++)
        {
            SpawnFriendlyPair(new Vector3(housePos.x + xOffset, housePos.y, housePos.z + zOffset));

            if ((i + 1) % 3 == 0)
            {
                zOffset += zStep;
                xOffset = -11f; // Reset xOffset for the next row
            }
            else xOffset -= xStep; // Increment xOffset using xStep
            
        }

        if (isOdd)
        {
            // Spawn a single friendly warrior to the negative x side
            Instantiate(friendlyWarriorPrefab, new Vector3(housePos.x + xOffset, housePos.y, housePos.z + zOffset), kingHouse.transform.rotation);
        }
    }

    void SpawnFriendlyPair(Vector3 spawnPosition)
    {
        Instantiate(friendlyWarriorPrefab, spawnPosition, kingHouse.transform.rotation);
        Instantiate(friendlyWarriorPrefab, new Vector3((spawnPosition.x * -1f) - 2f, spawnPosition.y, spawnPosition.z), kingHouse.transform.rotation);
    }

    IEnumerator SecondCounter()
    {
        secondsInBattle = 0;
        while (!battleWinningScreen.activeSelf)
        {
            yield return new WaitForSecondsRealtime(1f);
            secondsInBattle++;
        }
    }

    public string GetBattleTimerString(int seconds)
    {
        return string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
    }
}