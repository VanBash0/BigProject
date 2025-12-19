using UnityEngine;
using Zenject;
using BigProject.Systems;

namespace BigProject.Test.VFrolov
{
    /// <summary>
    /// Реализует квестовое действие "установка шестерни".
    /// </summary>
    public class TVFInstallGear : MonoBehaviour
    {
        // Все, что нам надо знать о квесте с тчоки зрения объекта на сцене:
        // QuestActionHandler - следит за изменениями конкретной активности в квесте.
        // Работаем с квестом через него.
        // Нас интересует активность "установка шестерни".
        // В инспекторе уже висит QuestActionHandler с настроенным id квеста и id этой активности.
        // (сами id прописаны в файле квеста условным дизайнером).
        [SerializeField]
        private QuestActionHandlerMono _actionHandler;

        // И идентификаторы доступных переходов
        // В данном случае id перехода в состояние активно, неактивно, выполнено.
        // Дизайнер настривает связи объектов в файле квеста, а программисту сообщает
        // идентификаторы переходов и когда эти переходы надо делать.
        [SerializeField] 
        private int _toActiveId, _toInactiveId, _toCompletedId;

        private QuestActionState _state; // Для удобства хэширую сюда состояние из QuestActionHandler

        [SerializeField]
        private Transform _target;
        private Vector3 _startPosition;
        [Inject]
        private TVFInput _input;
        private TVFRotate _rotate;

        private void OnEnable()
        {
            // Подписываемся на событие изменения состояния активности.
            _actionHandler.StateChanged += OnStateChanged;

            _input.GOClicked += OnGOClicked;
            _input.CursorMoved += OnCursorMoved;
        }

        private void OnDisable()
        {
            _actionHandler.StateChanged -= OnStateChanged;
            _input.GOClicked -= OnGOClicked;
            _input.CursorMoved -= OnCursorMoved;
        }

        private void Start()
        {
            _rotate = GetComponent<TVFRotate>();
            _startPosition = transform.position;
            _state = _actionHandler.CurrentState; // Начальное состояние активности.
        }

        // При измении состояния активности.
        private void OnStateChanged()
        {
            _state = _actionHandler.CurrentState;

            // Условный дизайнер сказал:
            // Данная активность визуально для игрока имеет три состояния,
            // 1. Inactive - шестерня лежит там, где была в начале, статична.
            // 2. Active - шестерня двигается за курсором игрока.
            // 3. Completed - шестерня встает в позицию target и крутится.

            // Состояние поменялось, смотрим, что делать с объектом на сцене.

            switch (_state)
            {
                // Когда неактивна - ставим на старт
                case QuestActionState.Inactive:
                    transform.position = _startPosition;
                    _rotate.enabled = false;
                    break;
                // Когда активна - ничего не делаем (сделаем по событию OnCursorMoved).
                case QuestActionState.Active:
                    _rotate.enabled = false;
                    break;
                // "Выполнена" - ставим в target и включаем вращение.
                case QuestActionState.Completed:
                    transform.position = _target.position;
                    _rotate.enabled = true;
                    break;
            }
        }

        // По щелчку мыши
        private void OnGOClicked(GameObject go)
        {
            // Условный дизайнер сказал, что из кода мне надо делать переходы:
            // 1. Когда игрок щелкнул по объекту и он в состоянии Inactive - совершаем переход с id 0 (_toActiveId)
            // 2. Когда игрок щелкнул по объекту и он в состоянии Active - совершаем переход с id 1 (_toInactiveId)
            // 3. Если шестерня близко к target - переход с id 2 (_toCompletedId).

            // Если попали в шестерню.
            if (go.transform.parent == transform)
            {
                // Пункт 1.
                if (_state == QuestActionState.Inactive)
                {
                    _actionHandler.MakeTransition(_toActiveId); // Переход с id 0
                }
                // Пункт 2.
                else // if (_state == QuestActionState.Active) - можно не писать.
                {
                    // Здесь может быть ситуация, когда активность например в состоянии Completed, но
                    // в файле квеста дизайнер указал в переходе с id 1, что переход возможен ТОЛЬКО из состояния Active.
                    // Поэтому если что-то не так - переход просто будет проигнорирован.
                    _actionHandler.MakeTransition(_toInactiveId); // Переход с id 1
                }
                // Пункт 3 проверяем в Update, расстояние до цели.
            }
        }

        private void OnCursorMoved(Vector2 position)
        {
            // Если шестерня активна - двигается за курсором.
            if (_state == QuestActionState.Active)
            {
                Vector3 cursorPos = position;
                cursorPos.z = 2.5f;
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(cursorPos);
                transform.position = new(transform.position.x, worldPoint.y, worldPoint.z);
            }
        }

        private void Update()
        {
            // Чтобы избежать лишних Update
            if (_state == QuestActionState.Completed)
                return;

            // Если растояние до шестерни небольшое, совершаем переход с id 2 (_toCompletedId).
            if (Vector3.Distance(transform.position, _target.position) < 0.2f)
                _actionHandler.MakeTransition(_toCompletedId);

            // Таким образом мы НЕ знаем о других объектах в квесте
            // Мы управляем только собственными перходами, которые разрешены дизайнером, через MakeTransition.
            // Далее квест сам решает, какие объекты как поменялись по своей логике, настроенной дизайнером. В том числе и нас он может отказаться менять.
            // Поэтому мы отслеживаем смену своего состояния через событие StateChanged и отображаем на сцене.

            // PS в ряде случаев можно больше инфы брать из квеста и о других объектах, напрмиер для отображения статистики (см. TVFQuestProgressView).
        }
    }
}