using BigProject.Managers;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace BigProject.Systems.HUD
{
    [Flags]
    public enum HUDWidgetRoutineType
    {
        Show = 0x1,
        Hide = 0x2,
    }

    /// <summary>
    /// Фасад HUD. Работает с IHUDWidget's.
    /// </summary>
    public class HUD : IDisposable
    {
        private Dictionary<int, IHUDWidget> _widgets = new();
        private Dictionary<IHUDWidget, List<WidgetRoutine>> _widgetsRoutines = new();

        private class WidgetRoutine
        {
            public CancellationTokenSource cts;
            public HUDWidgetRoutineType type;
        }

        public HUD()
        {

        }

        /// <summary>
        /// Добавляет виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        /// <param name="widget">виджет.</param>
        public void AddWidget(int id, IHUDWidget widget)
        {
            if (widget == null)
            {
                Debug.LogWarning($"HUD system try to add null widget with id {id}");
                return;
            }

            if (_widgets.ContainsKey(id))
            {
                Debug.LogWarning($"HUD system try to add widgets with same id {id}. Type of widget: {widget.GetType().Name}");
                return;
            }

            _widgets.Add(id, widget);
        }

        /// <summary>
        /// Удаляет виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        public void RemoveWidget(int id)
        {
            if (_widgets.ContainsKey(id))
            {
                ClearRoutines(id);
                _widgets.Remove(id);
            }
        }

        /// <summary>
        /// Удаляет все виджеты.
        /// </summary>
        public void RemoveAllWidgets()
        {
            foreach (var id in _widgets.Keys)
            {
                ClearRoutines(id);
            }

            _widgets.Clear();
            _widgetsRoutines.Clear();
        }

        /// <summary>
        /// Отображает виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        /// <param name="timeOffset">Время с момента вызова до отображения в секундах.</param>
        /// <param name="time">Время отображения.</param>
        public void ShowWidget(int id, float timeOffset = 0f, float time = float.PositiveInfinity)
        {
            if (!IsActualWidgetCommand(id, timeOffset, time))
            {
                Debug.LogWarning($"Widget {id} show command will be ignored.");
                return;
            }

            IHUDWidget widget = _widgets[id];

            if (timeOffset == 0f && time == float.PositiveInfinity)
            {
                GameLogManager.Info($"Show HUD widget: {id}");
                widget.Show();
                return;
            }

            HUDWidgetRoutineType type = HUDWidgetRoutineType.Show;

            if (time != float.PositiveInfinity)
            {
                type |= HUDWidgetRoutineType.Hide;
            }

            if (HasWidgetRoutine(widget, type))
            {
                Debug.LogWarning($"HUD try to start {type} routine for widget {id}, but it already started.");
                return;
            }

            _ = WidgetRoutineAsync(widget, type, timeOffset, time);
        }

        /// <summary>
        /// Скрывает виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        /// <param name="timeOffset">Время с момента вызова до скрытия в секундах.</param>
        public void HideWidget(int id, float timeOffset = 0f)
        {
            if (!IsActualWidgetCommand(id, timeOffset))
            {
                Debug.LogWarning($"Widget {id} hide command will be ignored.");
                return;
            }

            IHUDWidget widget = _widgets[id];

            if (timeOffset == 0f)
            {
                GameLogManager.Info($"Hide HUD widget: {id}");
                widget.Hide();
                return;
            }

            if (HasWidgetRoutine(widget, HUDWidgetRoutineType.Hide))
            {
                Debug.LogWarning($"HUD try to start {HUDWidgetRoutineType.Hide} routine for widget {id}, but it already started.");
                return;
            }

            _ = WidgetRoutineAsync(widget, HUDWidgetRoutineType.Hide, timeOffset, 0f);
        }

        /// <summary>
        /// Отображает все виджеты из списка.
        /// </summary>
        /// <param name="ids">Список id виджетов.</param>
        /// <param name="timeOffset">Время с момента вызова до отображения в секундах.</param>
        /// <param name="time">Время отображения.</param>
        public void ShowWidgets(IEnumerable<int> ids, float timeOffset = 0f, float time = float.PositiveInfinity)
        {
            if (ids == null)
            {
                Debug.LogWarning("HUD try to show empty list of widgets.");
                return;
            }

            foreach (int id in ids)
            {
                ShowWidget(id, timeOffset, time);
            }
        }

        /// <summary>
        /// Скрывает все виджеты из списка.
        /// </summary>
        /// <param name="ids">Список id виджетов.</param>
        /// <param name="timeOffset">Время с момента вызова до скрытия в секундах.</param>
        public void HideWidgets(IEnumerable<int> ids, float timeOffset = 0f)
        {
            if (ids == null)
            {
                Debug.LogWarning("HUD try to hide empty list of widgets.");
                return;
            }

            foreach (int id in ids)
            {
                HideWidget(id, timeOffset);
            }
        }

        /// <summary>
        /// Возвращает виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        /// <returns>Виджет или null, если не найден.</returns>
        public IHUDWidget GetWidget(int id)
        {
            return _widgets.GetValueOrDefault(id);
        }

        /// <summary>
        /// Возвращает виджет.
        /// </summary>
        /// <param name="id">id виджета.</param>
        /// <param name="widget">Возвращаемый виджет.</param>
        /// <returns>True если виджет найден.</returns>
        public bool TryGetWidget(int id, out IHUDWidget widget)
        {
            if (_widgets.TryGetValue(id, out widget))
            {
                return true;
            }

            widget = null;
            return false;
        }

        public void Dispose()
        {
            RemoveAllWidgets();
        }

        private bool HasWidgetRoutine(IHUDWidget widget, HUDWidgetRoutineType type)
        {
            if (_widgetsRoutines.TryGetValue(widget, out List<WidgetRoutine> routines))
            {
                return routines.Find(x => (x.type & type) != 0) != null;
            }

            return false;
        }

        private bool IsActualWidgetCommand(int id, float timeOffset, float time = 0f)
        {
            if (!_widgets.ContainsKey(id))
            {
                Debug.LogWarning($"HUD hasn't widget with id: {id}.");
                return false;
            }

            if (timeOffset < 0f || time < 0f)
            {
                Debug.LogWarning($"HUD unable to show widget with negative time. Widget id: {id}.");
                return false;
            }

            return true;
        }

        private async Awaitable WidgetRoutineAsync(IHUDWidget widget, HUDWidgetRoutineType type, float timeOffset, float time)
        {
            GameLogManager.Info($"Starting widget {widget.GetType().Name} routine of type {type}...");
            CancellationTokenSource cts = new();

            if (!_widgetsRoutines.ContainsKey(widget))
            {
                _widgetsRoutines.Add(widget, new List<WidgetRoutine>());
            }

            WidgetRoutine routine = new() { cts = new(), type = type };
            _widgetsRoutines[widget].Add(routine);
            bool isShowRoutine = (type & HUDWidgetRoutineType.Show) == HUDWidgetRoutineType.Show;
            bool isHideRoutine = (type & HUDWidgetRoutineType.Hide) == HUDWidgetRoutineType.Hide;

            try
            {
                if (isShowRoutine && isHideRoutine)
                {
                    await ShowAndHideWidgetAsync(widget, timeOffset, time, routine.cts.Token);
                }
                else if (isShowRoutine)
                {
                    await ShowWidgetAsync(widget, timeOffset, routine.cts.Token);
                }
                else
                {
                    await HideWidgetAsync(widget, timeOffset, routine.cts.Token);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Debug.LogError($"Widget async routine crashed. {ex.Message}");
            }

            try
            {
                routine.cts.Dispose();
                _widgetsRoutines[widget].Remove(routine);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unable to remove widget routine. {ex.Message}");
            }
        }

        private async Awaitable ShowWidgetAsync(IHUDWidget widget, float timeOffset, CancellationToken ct)
        {
            await Awaitable.WaitForSecondsAsync(timeOffset, ct);
            widget.Show();
            GameLogManager.Info($"HUD widget {widget.GetType().Name} show routine finished.");
        }

        private async Awaitable HideWidgetAsync(IHUDWidget widget, float timeOffset, CancellationToken ct)
        {
            await Awaitable.WaitForSecondsAsync(timeOffset, ct);
            widget.Hide();
            GameLogManager.Info($"HUD widget {widget.GetType().Name} show routine finished.");
        }

        private async Awaitable ShowAndHideWidgetAsync(IHUDWidget widget, float timeOffset, float time, CancellationToken ct)
        {
            await ShowWidgetAsync(widget, timeOffset, ct);
            await HideWidgetAsync(widget, time, ct);
        }

        private void ClearRoutines(int widgetId)
        {
            IHUDWidget widget = _widgets[widgetId];

            if (widget != null && _widgetsRoutines.TryGetValue(widget, out List<WidgetRoutine> routines))
            {
                foreach (WidgetRoutine routine in routines)
                {
                    routine.cts.Cancel();
                    routine.cts.Dispose();
                }

                routines.Clear();
            }
        }
    }
}