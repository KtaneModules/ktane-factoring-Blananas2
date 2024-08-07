using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class FactoringScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeed;

    public KMSelectable[] Buttons;
    public TextMesh DisplayText;
    public TextMesh[] ButtonText; //2nd B -> R, 4th D -> O, 5th E -> T, 8th H -> N
    public GameObject[] StageLEDs;
    public Material Green;

    MonoRandom random;
    Dictionary<string, char> ruleSeedTree = new Dictionary<string, char>();
    int RNG = -1;
    string PuzzleString = "";
    char Answer = '?';
    int stages = 0;
    string Letters = "ABCDEFGHI";
    bool activated;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Button in Buttons)
        {
            Button.OnInteract += delegate () { buttonPress(Button); return false; };
        }

        GetComponent<KMBombModule>().OnActivate += Activate;
    }

    List<string> GetRandomSplit()
    {
        switch (random.Next(0, 5))
        {
            case 0:
                switch (random.Next(0, 3))
                {
                    case 0: return new List<string> { "AB", "C", "D" };
                    case 1: return new List<string> { "A", "BC", "D" };
                    default: return new List<string> { "A", "B", "CD" };
                }
            default:
                switch (random.Next(0, 3))
                {
                    case 0: return new List<string> { "A", "BCD" };
                    case 1: return new List<string> { "AB", "CD" };
                    default: return new List<string> { "ABC", "D" };
                }
        }
    }

    // Use this for initialization
    void Start()
    {
        DisplayText.text = "";
        random = RuleSeed.GetRNG();
        if (random.Seed != 1)
        {
            Debug.LogFormat("[Factoring #{0}] Using rule seed: {1}", moduleId, random.Seed);
            foreach (var a in GetRandomSplit())
            {
                var prefixesA = a.Select(ch => ch.ToString()).ToArray();

                if (random.Next(1, 101) <= 20)
                {
                    var answer = GetRandomRuleSeedAnswer();
                    foreach (var prefix in prefixesA)
                        ruleSeedTree.Add(prefix, answer);
                }
                else
                {
                    foreach (var b in GetRandomSplit())
                    {
                        var prefixesB = b.SelectMany(ch => prefixesA.Select(prefix => prefix + ch)).ToArray();

                        if (random.Next(1, 101) <= 40)
                        {
                            var answer = GetRandomRuleSeedAnswer();
                            foreach (var prefix in prefixesB)
                                ruleSeedTree.Add(prefix, answer);
                        }
                        else
                        {
                            foreach (var c in GetRandomSplit())
                            {
                                var prefixesC = c.SelectMany(ch => prefixesB.Select(prefix => prefix + ch)).ToArray();

                                if (random.Next(1, 101) <= 60)
                                {
                                    var answer = GetRandomRuleSeedAnswer();
                                    foreach (var prefix in prefixesC)
                                        ruleSeedTree.Add(prefix, answer);
                                }
                                else
                                {
                                    foreach (var d in GetRandomSplit())
                                    {
                                        var prefixesD = d.SelectMany(ch => prefixesC.Select(prefix => prefix + ch)).ToArray();

                                        if (random.Next(1, 101) <= 80)
                                        {
                                            var answer = GetRandomRuleSeedAnswer();
                                            foreach (var prefix in prefixesD)
                                                ruleSeedTree.Add(prefix, answer);
                                        }
                                        else
                                        {
                                            foreach (var e in GetRandomSplit())
                                            {
                                                var prefixesE = e.SelectMany(ch => prefixesD.Select(prefix => prefix + ch)).ToArray();
                                                var answer = GetRandomRuleSeedAnswer();
                                                foreach (var prefix in prefixesE)
                                                    ruleSeedTree.Add(prefix, answer);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Debug.LogFormat("<Factoring #{0}> The diagram for this seed is:", moduleId);
            foreach (KeyValuePair<string, char> entry in ruleSeedTree)
                Debug.LogFormat("<Factoring #{0}> {1} → {2}", moduleId, entry.Key, entry.Value);
        }
        GenerateStage();
    }

    char GetRandomRuleSeedAnswer()
    {
        int choice = random.Next(1, 4);
        if (choice == 1)
            return 'X';
        else if (choice == 2)
            return 'L';
        else
            return Letters[random.Next(0, Letters.Length)];
    }

    // Use when the lights turn on
    void Activate()
    {
        DisplayText.text = PuzzleString;
        activated = true;
    }

    void GenerateStage()
    {
        PuzzleString = "";
        for (int i = 0; i < 5; i++)
        {
            RNG = UnityEngine.Random.Range(0, 4);
            switch (RNG)
            {
                case 0: PuzzleString += "A"; break;
                case 1: PuzzleString += "B"; break;
                case 2: PuzzleString += "C"; break;
                case 3: PuzzleString += "D"; break;
                default: Debug.Log("Hey Blan fucked up his coding, please contact him. --Gary"); break;
            }
        }
        if (activated)
            DisplayText.text = PuzzleString;

        if (random.Seed == 1)
        {
            switch (PuzzleString[0])
            {
                case 'A':
                case 'B': //If the first letter is A or B...
                    switch (PuzzleString[1])
                    {
                        case 'A':
                        case 'B': //If the second letter is A or B...
                            switch (PuzzleString[2])
                            {
                                case 'A': //If the third letter is A...
                                    switch (PuzzleString[3])
                                    {
                                        case 'A': //If the fourth letter is A...
                                            switch (PuzzleString[4])
                                            {
                                                case 'A': //If the fifth letter is A...
                                                    Answer = 'A';
                                                    break;
                                                case 'B':
                                                case 'C': //If the fifth letter is B or C...
                                                    Answer = PuzzleString[4];
                                                    break;
                                                default: //If the fifth letter is D...
                                                    Answer = 'X';
                                                    break;
                                            }
                                            break;
                                        case 'B':
                                        case 'C': //If the fourth letter is B or C...
                                            Answer = PuzzleString[4];
                                            break;
                                        default: //If the fourth letter is D...
                                            Answer = 'X';
                                            break;
                                    }
                                    break;
                                case 'B':
                                case 'C': //If the third letter is B or C...
                                    Answer = PuzzleString[4];
                                    break;
                                default: //If the third letter is D...
                                    Answer = 'X';
                                    break;
                            }
                            break;
                        default: // If the second letter is C or D...
                            switch (PuzzleString[2])
                            {
                                case 'A': // If the third letter is A...
                                    switch (PuzzleString[3])
                                    {
                                        case 'A':
                                        case 'B': // If the fourth letter is A or B...
                                            Answer = 'G';
                                            break;
                                        default: // If the fourth letter is C or D...
                                            Answer = PuzzleString[4];
                                            break;
                                    }
                                    break;
                                default: // If the third letter is B, C or D...
                                    Answer = 'B';
                                    break;
                            }
                            break;
                    }
                    break;
                default: // If the first letter is C or D...
                    Answer = PuzzleString[4];
                    break;
            }
        }
        else
        {
            string tempPuzzleString = PuzzleString.ToString();
            for (int i = 0; i < 5; i++)
            {
                if (ruleSeedTree.ContainsKey(tempPuzzleString))
                {
                    if (ruleSeedTree[tempPuzzleString] == 'L')
                        Answer = PuzzleString[4];
                    else
                        Answer = ruleSeedTree[tempPuzzleString];
                    break;
                }
                tempPuzzleString = tempPuzzleString.Remove(tempPuzzleString.Length - 1, 1);
            }
        }

        Debug.LogFormat("[Factoring #{0}] Stage {1}: Sequence is {2}, Answer is {3}", moduleId, stages + 1, PuzzleString, Answer);

        if (stages == 1)
        {
            StageLEDs[0].GetComponent<MeshRenderer>().material = Green;
        }
        else if (stages == 2)
        {
            StageLEDs[1].GetComponent<MeshRenderer>().material = Green;
        }
    }

    void buttonPress(KMSelectable Button)
    {
        if (!moduleSolved && activated)
        {
            Button.AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Button.transform);
            if (Answer == 'X')
            {
                stages += 1;
                if (stages != 3)
                {
                    Debug.LogFormat("[Factoring #{0}] Input for stage {1} is correct.", moduleId, stages);
                    GenerateStage();
                }
                else
                {
                    Debug.LogFormat("[Factoring #{0}] Input for stage 3 is correct, module solved.", moduleId);
                    DisplayText.text = "Solved";
                    StageLEDs[2].GetComponent<MeshRenderer>().material = Green;
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    if (Button == Buttons[i])
                    {
                        if (Answer == Letters[i])
                        {
                            stages += 1;
                            if (stages != 3)
                            {
                                Debug.LogFormat("[Factoring #{0}] Input for stage {1} is correct.", moduleId, stages);
                                GenerateStage();
                            }
                            else
                            {
                                Debug.LogFormat("[Factoring #{0}] Input for stage 3 is correct, module solved.", moduleId);
                                DisplayText.text = "Solved";
                                StageLEDs[2].GetComponent<MeshRenderer>().material = Green;
                                GetComponent<KMBombModule>().HandlePass();
                                moduleSolved = true;
                            }
                        }
                        else
                        {
                            Debug.LogFormat("[Factoring #{0}] Input for stage {1} is incorrect, strike!", moduleId, stages + 1);
                            GetComponent<KMBombModule>().HandleStrike();
                        }
                    }
                }
            }
        }
    }

    //I ADD TWITCH :D
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <letter> [Presses that letter]";
#pragma warning disable 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the button you would like to press!";
            }
            else
            {
                if (Regex.IsMatch(parameters[1], @"^\s*a\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[0].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*b\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[1].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*c\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[2].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*d\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[3].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*e\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[4].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*f\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[5].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*g\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[6].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*h\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[7].OnInteract();
                }
                else if (Regex.IsMatch(parameters[1], @"^\s*i\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    Buttons[8].OnInteract();
                }
                else
                {
                    yield return "sendtochaterror That button is not on the module!";
                }
            }
            yield break;
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (!activated) yield return true;
        for (int i = stages; i < 3; i++)
        {
            if (Answer == 'X')
                Buttons[UnityEngine.Random.Range(0, 9)].OnInteract();
            else
                Buttons[Letters.IndexOf(Answer)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}