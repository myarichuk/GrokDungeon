using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using GrokDungeon.Models;

namespace GrokDungeon.CharacterManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private Profile _currentProfile;

    [ObservableProperty]
    private Player? _selectedCharacter;

    [ObservableProperty]
    private string _newCharacterName = "";

    [ObservableProperty]
    private string _selectedClass = "Fighter";

    public ObservableCollection<Player> Characters { get; } = new();
    public ObservableCollection<string> Classes { get; } = new() { "Fighter", "Wizard" };

    public MainViewModel()
    {
        CurrentProfile = new Profile();
        LoadProfile();
    }

    private void LoadProfile()
    {
        // For now, let's load some defaults or an empty profile.
        // Real implementation would load from RavenDB or a local file.
        Characters.Clear();
        foreach (var c in CurrentProfile.Characters)
        {
            Characters.Add(c);
        }
    }

    [RelayCommand]
    private void CreateCharacter()
    {
        if (string.IsNullOrWhiteSpace(NewCharacterName)) return;

        Player newChar = SelectedClass switch
        {
            "Fighter" => CharacterFactory.Create5eFighter(NewCharacterName),
            "Wizard" => CharacterFactory.Create5eWizard(NewCharacterName),
            _ => CharacterFactory.Create5eFighter(NewCharacterName)
        };

        CurrentProfile.Characters.Add(newChar);
        Characters.Add(newChar);
        SelectedCharacter = newChar;
        NewCharacterName = "";
    }
}
