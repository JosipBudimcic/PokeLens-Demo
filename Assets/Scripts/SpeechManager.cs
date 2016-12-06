using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Reset world", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnReset");
        });

		keywords.Add("Use Quick Attack", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Quick Attack");
			});
		keywords.Add("Use Growl", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Growl");
			});

		keywords.Add("Use Iron Tail", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Iron Tail");
			});

		keywords.Add("Use Thunder Shock", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Thunder Shock");
			});

		keywords.Add("Use Gust", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Gust");
			});

		keywords.Add("Use Sand Attack", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Sand Attack");
			});
		keywords.Add("Use Tackle", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Tackle");
			});

		keywords.Add("Come Back", () =>
			{
				// Call the OnReset method on every descendant object.
				this.BroadcastMessage("Voice_Battle_Command", "Come Back");
			});
		
        keywords.Add("Restart Game", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnSceneReset");
        });
        keywords.Add("Battle Start", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("Start_Battle");
        });
        keywords.Add("Drop Sphere", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the OnDrop method on just the focused object.
                focusObject.SendMessage("OnDrop");
            }
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
