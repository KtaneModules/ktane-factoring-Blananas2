using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class FactoringScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public TextMesh DisplayText;
    public TextMesh[] ButtonText; //2nd B -> R, 4th D -> O, 5th E -> T, 8th H -> N
    public GameObject[] StageLEDs;
    public Material Green;

    int RNG = -1;
    string PuzzleString = "";
    char Answer = '?';
    int stages = 0;
    string Letters = "ABCDEFGHI";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;

        foreach (KMSelectable Button in Buttons) {
            Button.OnInteract += delegate () { buttonPress(Button); return false; };
        }
    }

    // Use this for initialization
    void Start () {
        GenerateStage();
    }

    void GenerateStage() {
        PuzzleString = "";
        for (int i = 0; i < 5; i++) {
            RNG = UnityEngine.Random.Range(0, 4);
            switch (RNG) {
                case 0: PuzzleString += "A"; break;
                case 1: PuzzleString += "B"; break;
                case 2: PuzzleString += "C"; break;
                case 3: PuzzleString += "D"; break;
                default: Debug.Log("Hey Blan fucked up his coding, please contact him. --Gary"); break;
            }
        }
        DisplayText.text = PuzzleString;

        switch (PuzzleString[0]) {
            case 'A': case 'B': //If the first letter is A or B...
                switch (PuzzleString[1]) {
                    case 'A': case 'B': //If the second letter is A or B...
                        switch (PuzzleString[2]) {
                            case 'A': //If the third letter is A...
                                switch (PuzzleString[3]) {
                                    case 'A': //If the fourth letter is A...
                                        switch (PuzzleString[4]) {
                                            case 'A': //If the fifth letter is A...
                                                Answer = 'A';
                                                break;
                                            case 'B': case 'C': //If the fifth letter is B or C...
                                                Answer = PuzzleString[4];
                                                break;
                                            default: //If the fifth letter is D...
                                                Answer = 'X';
                                                break;
                                        }
                                        break;
                                    case 'B': case 'C': //If the fourth letter is B or C...
                                        Answer = PuzzleString[4];
                                        break;
                                    default: //If the fourth letter is D...
                                        Answer = 'X';
                                        break;
                                }
                                break;
                            case 'B': case 'C': //If the third letter is B or C...
                                Answer = PuzzleString[4];
                                break;
                            default: //If the third letter is D...
                                Answer = 'X';
                                break;
                        }
                        break;
                    default: // If the second letter is C or D...
                        switch (PuzzleString[2]) {
                            case 'A': // If the third letter is A...
                                switch (PuzzleString[3]) {
                                    case 'A': case 'B': // If the fourth letter is A or B...
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

        Debug.LogFormat("[Factoring #{0}] Stage {1}: Sequence is {2}, Answer is {3}", moduleId, stages + 1, PuzzleString, Answer);

        if (stages == 1) {
            StageLEDs[0].GetComponent<MeshRenderer>().material = Green;
        } else if (stages == 2) {
            StageLEDs[1].GetComponent<MeshRenderer>().material = Green;
        }
    }

    void buttonPress(KMSelectable Button) {
        if (!moduleSolved) {
            Button.AddInteractionPunch();
            GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            if (Answer == 'X') {
                stages += 1;
                if (stages != 3) {
                    Debug.LogFormat("[Factoring #{0}] Input for stage {1} is correct.", moduleId, stages);
                    GenerateStage();
                } else {
                    Debug.LogFormat("[Factoring #{0}] Input for stage 3 is correct, module solved.", moduleId);
                    DisplayText.text = "Solved";
                    StageLEDs[2].GetComponent<MeshRenderer>().material = Green;
                    GetComponent<KMBombModule>().HandlePass();
                    moduleSolved = true;
                }
            } else {
                for (int i = 0; i < 9; i++) {
                    if (Button == Buttons[i]) {
                        if (Answer == Letters[i]) {
                            stages += 1;
                            if (stages != 3) {
                                Debug.LogFormat("[Factoring #{0}] Input for stage {1} is correct.", moduleId, stages);
                                GenerateStage();
                            } else {
                                Debug.LogFormat("[Factoring #{0}] Input for stage 3 is correct, module solved.", moduleId);
                                DisplayText.text = "Solved";
                                StageLEDs[2].GetComponent<MeshRenderer>().material = Green;
                                GetComponent<KMBombModule>().HandlePass();
                                moduleSolved = true;
                            }
                        } else {
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

    IEnumerator ProcessTwitchCommand(string command){
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
            yield return null;
            if (parameters.Length > 2) {
                yield return "sendtochaterror Too many parameters!";
            }
            else if (parameters.Length == 1)
            {
                yield return "sendtochaterror Please specify the button you would like to press!";
            } else {
                if (Regex.IsMatch(parameters[1], @"^\s*a\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[0].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*b\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[1].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*c\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[2].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*d\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[3].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*e\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[4].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*f\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[5].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*g\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[6].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*h\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[7].OnInteract();
                } else if (Regex.IsMatch(parameters[1], @"^\s*i\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) {
                    Buttons[8].OnInteract();
                } else {
                    yield return "sendtochaterror That button is not on the module!";
                }
            }
            yield break;
        }
    }
}
