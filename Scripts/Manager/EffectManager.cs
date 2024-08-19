using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public Dictionary<int, EffectData> effects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadEffectCsv();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadEffectCsv()
    {
        effects = new Dictionary<int, EffectData>();

        // Resources 폴더에서 CSV 파일을 읽어옵니다.
        TextAsset csvFile = Resources.Load<TextAsset>("csv/ConsumableEffectData");
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split(new[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');

                // EffectType과 StatsChangeType 열거형 값을 숫자로 처리
                EffectType effectType = (EffectType)ParseInt(values[1]);
                StatsChangeType statsChangeType = (StatsChangeType)ParseInt(values[6]);

                EffectData effect = new EffectData
                {
                    effectId = ParseInt(values[0]),
                    effectType = effectType,
                    duration = ParseFloat(values[2]),
                    effectInterval = ParseFloat(values[3]),
                    healHPAmount = ParseInt(values[4]),
                    healStaminaAmount = ParseInt(values[5]),
                    stat = new CharacterStat
                    {
                        statsChangeType = statsChangeType,
                        maxHealth = ParseInt(values[7]),
                        maxStamina = ParseInt(values[8]),
                        speed = ParseFloat(values[9]),
                        defense = ParseInt(values[10]),
                        power = ParseInt(values[11])
                    }
                };

                effects.Add(effect.effectId, effect);
            }
        }

    }

    public int ParseInt(string value)
    {
        return string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
    }

    public float ParseFloat(string value)
    {
        return string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
    }

    public EffectData GetEffectData(int effectId)
    {
        effects.TryGetValue(effectId, out var effect);
        return effect;
    }
}