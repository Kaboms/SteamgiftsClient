using Avalonia.Controls;
using Avalonia.Threading;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace SteamgiftsClient.ViewModels
{
    public class PageNavigatorViewModel : ViewModelBase
    {
        #region Reactive Commands
        public ReactiveCommand<Unit, int> GoToFirst { get; }
        public ReactiveCommand<Unit, int> GoToLast { get; }
        public ReactiveCommand<Unit, int> GoPreviousCommand { get; }
        public ReactiveCommand<Unit, int> GoNextCommand { get; }
        #endregion

        #region Reactive Properties
        public ObservableCollection<Button> NavigationButtons { get; }
            = new ObservableCollection<Button>();

        [Reactive]
        public int SelectedPage { get; set; } = 1;

        [Reactive]
        public int PagesCount { get; set; }

        [Reactive]
        public bool ShowGoPrevious { get; private set; }

        [Reactive]
        public bool ShowGoNext { get; private set; }

        [Reactive]
        public bool ShowDummyToFirst { get; private set; }

        [Reactive]
        public bool ShowShortcutToFirst { get; private set; }

        [Reactive]
        public bool ShowDummyToLast { get; private set; }

        [Reactive]
        public bool ShowShortcutToLast { get; private set; }
        #endregion

        public int MaxVisiblePages { get; set; }

        public PageNavigatorViewModel(int maxVisiblePages = 1, int pagesCount = 1)
        {
            MaxVisiblePages = maxVisiblePages;
            PagesCount = pagesCount;

            GoPreviousCommand = ReactiveCommand.Create(() => SelectedPage--);
            GoNextCommand = ReactiveCommand.Create(() => SelectedPage++);

            GoToFirst = ReactiveCommand.Create(() => SelectedPage = 1);
            GoToLast = ReactiveCommand.Create(() => SelectedPage = PagesCount);

            this.WhenPropertyChanged(x => x.PagesCount)
                .Subscribe(pagesCount =>
                {
                    if (NavigationButtons.Count < MaxVisiblePages)
                    {
                        int buttonsToAdd = ((MaxVisiblePages > pagesCount.Value) ? pagesCount.Value : MaxVisiblePages) - NavigationButtons.Count;
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            for (int i = 0; i < buttonsToAdd; i++)
                            {
                                var button = new Button();

                                ReactiveCommand<Unit, int> switchToCommand = ReactiveCommand.Create(() =>
                                {
                                    int pageToSwitch;

                                    if (int.TryParse(button.Content.ToString(), out pageToSwitch))
                                        return SwitchPageTo(pageToSwitch);

                                    return SelectedPage;
                                });
                                button.Command = switchToCommand;
                                NavigationButtons.Add(button);
                            }
                        });
                    }

                    UpdateButtonsNumbers(SelectedPage, pagesCount.Value);
                });

            this.WhenPropertyChanged(x => x.SelectedPage, notifyOnInitialValue: false)
                .Subscribe(selectedPage => UpdateButtonsNumbers(selectedPage.Value, PagesCount));
        }

        private void UpdateButtonsNumbers(int selectedPage, int pagesCount)
        {
            ShowGoPrevious = selectedPage > 1;
            ShowGoNext = selectedPage < pagesCount;

            int minOffset = (MaxVisiblePages < pagesCount ? MaxVisiblePages : pagesCount) / 2;

            int centerPage;
            if (SelectedPage <= minOffset)
                centerPage = minOffset + 1;
            else if (SelectedPage > pagesCount - minOffset)
                centerPage = pagesCount - minOffset;
            else
                centerPage = SelectedPage;

            int firstVisiblePage = centerPage - minOffset;
            if (firstVisiblePage < 1)
                firstVisiblePage = 1;

            int lastVisiblePage = centerPage + minOffset;
            if (lastVisiblePage > pagesCount)
                lastVisiblePage = pagesCount;

            ShowDummyToFirst = firstVisiblePage > 2;
            ShowShortcutToFirst = firstVisiblePage > 1;

            ShowDummyToLast = lastVisiblePage < pagesCount - 1;
            ShowShortcutToLast = lastVisiblePage < pagesCount;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                int pageNumber = firstVisiblePage;
                foreach (Button button in NavigationButtons)
                {
                    button.IsEnabled = pageNumber != SelectedPage;
                    button.Content = pageNumber;

                    ++pageNumber;
                }
            });
        }

        public int SwitchPageTo(int pageNumber)
        {
            if (pageNumber > 0 && pageNumber <= PagesCount)
                SelectedPage = pageNumber;

            return SelectedPage;
        }
    }
}
