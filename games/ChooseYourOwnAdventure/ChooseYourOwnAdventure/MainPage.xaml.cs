﻿using BuildingGames.Slides;
using Orbit.Engine;

namespace BuildingGames;

public partial class MainPage : ContentPage
{
    private readonly IGameSceneManager gameSceneManager;
    private readonly ControllerManager controllerManager;
    private readonly AchievementManager achievementManager;
    private readonly IDeviceDisplay deviceDisplay;
    private static bool multipleScreens;

    public MainPage(
        IGameSceneManager gameSceneManager,
        ControllerManager controllerManager,
        AchievementManager achievementManager,
        IDeviceDisplay deviceDisplay,
        IServiceProvider serviceProvider)
	{
		InitializeComponent();

        this.gameSceneManager = gameSceneManager;
        this.controllerManager = controllerManager;
        this.achievementManager = achievementManager;
        this.deviceDisplay = deviceDisplay;
        LoadSlide(SlideDeck.CurrentSlideType);

        this.controllerManager.Initialise();

        if (multipleScreens == false)
        {
            multipleScreens = true;
            Application.Current.OpenWindow(new Window(serviceProvider.GetRequiredService<PresenterPage>()));
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        this.achievementManager.AchievementUnlocked += AchievementManager_AchievementUnlocked;
        this.controllerManager.ButtonPressed += ControllerManager_ButtonPressed;

        this.deviceDisplay.KeepScreenOn = true;
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        this.achievementManager.AchievementUnlocked -= AchievementManager_AchievementUnlocked;
        this.controllerManager.ButtonPressed -= ControllerManager_ButtonPressed;
    }

    private async void AchievementManager_AchievementUnlocked(Achievement obj)
    {
        try
        {
            this.AchievementTitle.Text = obj.Name;
            this.AchievementDescription.Text = obj.Description;

            this.AchievementBanner.Scale = 0;
            this.AchievementBanner.IsVisible = true;

            await this.AchievementBanner.ScaleTo(1, 350, Easing.CubicInOut);

            await Task.Delay(5000);

            await this.AchievementBanner.FadeTo(0, 350, Easing.CubicInOut);

            this.AchievementBanner.Scale = 0;
            this.AchievementBanner.IsVisible = false;
        }
        catch (Exception ex)
        {

        }
    }

    private void ControllerManager_ButtonPressed(ControllerButton controllerButton)
    {
        if (this.controllerManager.Mode == ControlMode.Navigation &&
            controllerButton == ControllerButton.NavigateForward)
        {
            ProgressSlides();
        }
        else if (this.controllerManager.Mode == ControlMode.Navigation &&
            controllerButton == ControllerButton.NavigateBackward)
        {
            GoBack();
        }
    }

    async void LoadSlide(Type sceneType)
    {
        if (GameView.Scene is SlideSceneBase previousScene)
        {
            previousScene.Back -= OnCurrentSceneBack;
            previousScene.Next -= OnCurrentSceneNext;
        }

        if (sceneType.IsAssignableTo(typeof(SlideSceneBase)))
        {
            this.gameSceneManager.Stop();
            this.gameSceneManager.LoadScene(sceneType, GameView);

            if (GameView.Scene is SlideSceneBase nextScene)
            {
                nextScene.Back += OnCurrentSceneBack;
                nextScene.Next += OnCurrentSceneNext;
            }

            this.gameSceneManager.Start();
        }
        else if (sceneType.IsAssignableTo(typeof(ContentPage)))
        {
            try
            {
                await Shell.Current.GoToAsync($"/{sceneType.Name}");
            }
            catch (Exception ex)
            {

            }
        }
    }

    private void OnCurrentSceneNext(SlideSceneBase sender)
    {
        if (sender is IDestinationKnowingScene destinationKnowingScene)
        {
            SlideDeck.SetCurrentSlideType(destinationKnowingScene.DestinationSceneType);
            this.LoadSlide(destinationKnowingScene.DestinationSceneType);
        }
        else if (SlideDeck.GetNextSlideType() is Type nextSlideType)
        {
            this.LoadSlide(nextSlideType);
        }
    }

    private void OnCurrentSceneBack(SlideSceneBase sender)
    {
        GoBack();
    }

    void GameView_StartInteraction(object sender, TouchEventArgs e)
    {
        ProgressSlides();
    }

    void ProgressSlides()
    {
        if (GameView.Scene is SlideSceneBase slideSceneBase &&
            slideSceneBase.CanProgress)
        {
            slideSceneBase.Progress();
        }
    }

    void GoBack()
    {
        if (SlideDeck.GetPreviousSlideType() is Type previousSlideType)
        {
            this.LoadSlide(previousSlideType);
        }
    }
}
