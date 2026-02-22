using BigProject.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.Systems.DialogueSystem
{
    public class DialogueView : MonoBehaviour
    {
        [SerializeField]
        public GameObject _dialogueWindow;
        [SerializeField]
        private TextMeshProUGUI _dialogueText;
        [SerializeField]
        private Image _rightCharacterImage;
        [SerializeField]
        private Image _leftCharacterImage;
        [SerializeField]
        private Button _nextButton;

        [SerializeField]
        private float _speakerImageAlpha = 0.8f;
        [SerializeField]
        private float _speakerImageTone = 0.5f;

        [SerializeField]
        private List<Button> _answerOptionButtons = new List<Button>();
        private List<TextMeshProUGUI> _answerOptionButtonTexts = new List<TextMeshProUGUI>();

        public void Init(DialogueManager dialogueManager)
        {
            for (int i = 0; i < _answerOptionButtons.Count; i++)
            {
                // Для замыкания
                int index = i;
                // Обработчик нажатия на вариант ответа
                _answerOptionButtons[i].onClick.AddListener(() => dialogueManager.SelectAnswerOption(index));
                // Инициализируем кнопки для взаимодействия
                TextMeshProUGUI buttonText = _answerOptionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText)
                {
                    _answerOptionButtonTexts.Add(buttonText);
                }
            }

            // Обработчик нажатия на кнопку "Продолжить"
            _nextButton.onClick.AddListener(() => dialogueManager.ShowNextStep());
        }

        public void HideAnswerOptions()
        {
            foreach (var answerOptionButton in _answerOptionButtons)
            {
                answerOptionButton.gameObject.SetActive(false);
            }
        }

        public void ShowAnswerOptions(DialogueLine dialogueLine)
        {
            SetImageAlpha(_rightCharacterImage, _speakerImageTone, _speakerImageAlpha);
            SetImageAlpha(_leftCharacterImage, 1f, 1f);
            // Включаем отображение кнопки продолжить и текст NPC
            _nextButton.gameObject.SetActive(false);
            _dialogueText.gameObject.SetActive(false);
            // Количество кнопок, которые нужно показать
            int buttonCount = Mathf.Min(
                _answerOptionButtons.Count,
                dialogueLine.DialogueAnswerOptions.Count
                );

            for (int i = 0; i < buttonCount; i++)
            {
                _answerOptionButtons[i].gameObject.SetActive(true);
                _answerOptionButtonTexts[i].text = dialogueLine.DialogueAnswerOptions[i].Text;
            }
        }

        public void ShowDialogueWindow()
        {
            _dialogueWindow.SetActive(true);
        }
        public void HideDialogueWindow()
        {
            _dialogueWindow.SetActive(false);
        }
        public void ShowNPCPhrase(DialogueNPCPhrase dialogueNPCPhrase)
        {
            SetImageAlpha(_leftCharacterImage, _speakerImageTone, _speakerImageAlpha);
            SetImageAlpha(_rightCharacterImage, 1f, 1f);
            // Включаем отображение кнопки продолжить и текст NPC
            _nextButton.gameObject.SetActive(true);
            _dialogueText.gameObject.SetActive(true);
            _dialogueText.text = dialogueNPCPhrase.Text;
            _rightCharacterImage.sprite = dialogueNPCPhrase.CharacterSprite;
        }

        private void SetImageAlpha(Image image, float tone, float alpha)
        {
            Color color = new(tone, tone, tone, alpha);
            image.color = color;
        }
    }
}
