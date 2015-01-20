﻿// Copyright (c) 2015 Eamon Woortman
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SavegameMenu : BaseMenu {
    public delegate void LoadButtonPressed(string filename);
    public delegate void SaveButtonPressed(string filename, int savegameId);

    public SaveButtonPressed OnSaveButtonPressed;
    public LoadButtonPressed OnLoadButtonPressed;
    public delegate void VoidDelegate();
    public VoidDelegate OnHasSaved;
    [HideInInspector]
    public string SavegameType;

    public string SaveName {
        get {
            return saveName;
        }
    }

    private const string NoSavegamesFoundText = "No savegames found";
    private const string LoadingGamesText = "Loading games...";
    private const string SavingGameText = "Saving game...";
    private const string LoadingGameText = "Loading game...";

    private static GUIContent deleteButtonContent = new GUIContent("-", "delete");
    private List<Savegame> saveGames;
    private int selectedNameIndex = -1, oldSelectedNameIndex = -1;
    private string saveName = "";
    private string[] saveGameNames = { LoadingGamesText };
    private bool isLoading = true;
    private string statusText = LoadingGamesText;
    private bool isExpanded;
    private int deleteSavegameIndex = -1;

    public SavegameMenu() {
        windowRect = new Rect(10, 10, 200, 260);
    }

    public void LoadSavegames() {
        statusText = LoadingGamesText;
        backendManager.LoadGames(SavegameType);
    }

    private void Start() {
        backendManager.OnSaveGameSucces += OnSaveGameSuccess;
        backendManager.OnSaveGameFailed += OnSaveGameFailed;
        backendManager.OnGamesLoaded += OnGamesLoaded;

        isExpanded = PlayerPrefs.GetInt("expandSaveMenu", 1) == 1 ? true : false;
    }

    private void OnSaveGameSuccess() {
        LoadSavegames();
    }

    private void OnSaveGameFailed(string error) {
        isLoading = false;
        saveGameNames = new string[] { NoSavegamesFoundText };
    }

    private void OnGamesLoaded(List<Savegame> games) {
        isLoading = false;
        saveGames = games;

        if (games.Count != 0) {
            saveGameNames = saveGames.Select(game => game.Name).ToArray();
        } else {
            saveGameNames = new string[] { NoSavegamesFoundText };
        }
        statusText = "";
    }

    private int GetSaveIdByIndex(int index) {
        return GetSaveId(saveGameNames[index]);
    }
         
    private int GetSaveId(string savegameName) {
        foreach (Savegame savegame in saveGames) {
            if (savegame.Name == savegameName) {
                return savegame.Id;
            }
        }
        return -1;
    }

    private void DoSave() {
        statusText = SavingGameText;
        isLoading = true;
        int saveId = GetSaveId(saveName);

        if (OnSaveButtonPressed != null) {
            OnSaveButtonPressed(SaveName, saveId);
        }
    }

    private void OverwriteConfirmed() {
        DoSave();
        enabled = true;
    }
    private void PopupClosed() {
        enabled = true;
    }


    private void OnSaveDeleted() {
        statusText = "Savegame deleted.";
        enabled = true;
        LoadSavegames();
    }

    private void OnSaveDeletedFailed(string errString) {
        statusText = "Deletion failed: " + errString;
        enabled = true;
    }

    private void DeleteConfirmed() {
        enabled = true;
        isLoading = true;
        statusText = "Deleting savegame...";

        int savegameId = GetSaveIdByIndex(deleteSavegameIndex);
        backendManager.DeleteSavegame(savegameId);
    }

    private void ShowWindow(int id) {
        bool newIsExpanded = GUILayout.Toggle(isExpanded, "  Show/Hide");
        if (newIsExpanded != isExpanded) {
            isExpanded = newIsExpanded;
            PlayerPrefs.SetInt("expandSaveMenu", isExpanded ? 1 : 0);
        }

        if (!isExpanded) {
            return;
        }

        GUILayout.BeginVertical();
        GUILayout.Label("Save games");
        bool savegamesFound = (saveGameNames[0] != NoSavegamesFoundText);
        GUI.enabled = savegamesFound && !isLoading;

        GUILayout.BeginHorizontal();
        selectedNameIndex = GUILayout.SelectionGrid(selectedNameIndex, saveGameNames, 1, GUILayout.Width(150));
        if (selectedNameIndex != oldSelectedNameIndex) {
            saveName = saveGameNames[selectedNameIndex];
            oldSelectedNameIndex = selectedNameIndex;
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        for (int i = 0; i < saveGameNames.Length; i++) {
            if (GUILayout.Button(deleteButtonContent, GUILayout.Width(21), GUILayout.Height(21))) {
                deleteSavegameIndex = i;
                ConfirmPopup popup = ConfirmPopup.Create("Deleting savegame", "You are about to delete the savegame '" + saveGameNames[i] + "', are you sure?");
                popup.OnConfirmed += DeleteConfirmed;
                popup.OnCanceled += PopupClosed;
                enabled = false;
            }
        } 
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        saveName = GUILayout.TextField(saveName);
        GUILayout.Label("Status: " + statusText);
        GUILayout.BeginHorizontal();

        GUI.enabled = (SaveName != "");
        if (GUILayout.Button("Save")) {
            int saveId = GetSaveId(saveName);
            if (saveId != -1) {
                ConfirmPopup popup = ConfirmPopup.Create("Overwriting savegame", "You are about to overwrite the savegame '"+saveName+"', are you sure?");
                popup.OnConfirmed += OverwriteConfirmed;
                popup.OnCanceled += PopupClosed;
                enabled = false;
            } else if (saveGameNames.Length == 5) {
                ConfirmPopup popup = ConfirmPopup.Create("Savegame limit reached", "You have reached the limit(5) of savegames. Please overwrite an existing savegame.", true);
                popup.OnCanceled += PopupClosed;
                popup.OnConfirmed += PopupClosed;
                enabled = false;
            } else {
                DoSave();
            }
        }

        GUI.enabled = savegamesFound && selectedNameIndex != -1;
        if (GUILayout.Button("Load")) {
            statusText = LoadingGameText;
            if (OnLoadButtonPressed != null) {
                OnLoadButtonPressed(saveGames[selectedNameIndex].File);
            }
        }
        GUI.enabled = true;

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void OnGUI() {
        GUI.skin = Skin;
        if (isExpanded) {
            windowRect.height = 260;
        } else {
            windowRect.height = 50;
        }
        windowRect = GUILayout.Window(0, windowRect, ShowWindow, "Load/save menu");
    }

    public void SetStatus(string status) {
        statusText = status;
    }
}
