using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Enums
    public enum Stat {
        Rhythm,
        Bass,
        Harmony,
        Volume,
        Tempo
    }

    // Constants
    private const int MAX_STAT_TOTAL = 100;
    private const int MAX_SKILLS = 4;

    // Private Members
    private readonly Dictionary<Stat, int> _stats = new Dictionary<Stat, int>();
    private readonly Skill[] _baseClassSkills = new Skill[MAX_SKILLS];
    private readonly Skill[] _superClassSkills = new Skill[MAX_SKILLS];
    private readonly ArrayList _passives = new ArrayList();
    private readonly string unitName;
    
    // Unit Properties
    private int StatTotal {
        get {
            int total = 0;
            foreach (int stat in _stats.Values) {
                total += stat;
            }
            return total;
        }
    }

    public int this[Stat stat] {
        get {
            return _stats.ContainsKey(stat) ? _stats[stat] : 0;
        }
        private set {
            if (StatTotal - _stats[stat] + value <= MAX_STAT_TOTAL) {
                _stats[stat] = value;
            } else {
                // Debug.LogWarning($"{unitName} cannot exceed max stat total of {MAX_STAT_TOTAL}");
                _stats[stat] = MAX_STAT_TOTAL - (StatTotal - _stats[stat]); // Set to max value
            }
        }
    }


    // Constructors
    public Unit(string name) {
        unitName = name;
        _stats.Add(Stat.Rhythm, 0);
        _stats.Add(Stat.Bass, 0);
        _stats.Add(Stat.Harmony, 0);
        _stats.Add(Stat.Volume, 0);
        _stats.Add(Stat.Tempo, 0);
    }


    public void AddStat(Stat stat, int value) {
        this[stat] += value;
    }
}
